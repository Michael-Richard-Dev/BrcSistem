using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlDatabaseMaintenanceGateway
    {
        public StockMovementSyncDiagnostic DiagnoseStockMovementSynchronization(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            {
                var notes = LoadMissingNoteMovements(connection, null);
                var transferOutputs = LoadMissingTransferOutputMovements(connection, null);
                var transferInputs = LoadMissingTransferInputMovements(connection, null);
                var requisitions = LoadMissingRequisitionMovements(connection, null);
                var productionOutputs = LoadMissingProductionOutputMovements(connection, null);

                var items = new List<StockMovementSyncItem>();
                items.AddRange(notes);
                items.AddRange(transferOutputs);
                items.AddRange(transferInputs);
                items.AddRange(requisitions);
                items.AddRange(productionOutputs);

                return new StockMovementSyncDiagnostic
                {
                    ActiveMovements = CountActiveStockMovements(connection, null),
                    MissingNotes = notes.Count,
                    MissingTransferOutputs = transferOutputs.Count,
                    MissingTransferInputs = transferInputs.Count,
                    MissingRequisitions = requisitions.Count,
                    MissingProductionOutputs = productionOutputs.Count,
                    Items = items,
                };
            }
        }

        public StockMovementSyncResult SynchronizeMissingStockMovements(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                var notes = LoadMissingNoteMovements(connection, transaction);
                var transferOutputs = LoadMissingTransferOutputMovements(connection, transaction);
                var transferInputs = LoadMissingTransferInputMovements(connection, transaction);
                var requisitions = LoadMissingRequisitionMovements(connection, transaction);
                var productionOutputs = LoadMissingProductionOutputMovements(connection, transaction);

                var expectedItems = notes.Count
                    + transferOutputs.Count
                    + transferInputs.Count
                    + requisitions.Count
                    + productionOutputs.Count;

                if (expectedItems <= 0)
                {
                    transaction.Rollback();
                    return new StockMovementSyncResult();
                }

                var insertedItems = 0;
                insertedItems += InsertMissingMovements(connection, transaction, notes);
                insertedItems += InsertMissingMovements(connection, transaction, transferOutputs);
                insertedItems += InsertMissingMovements(connection, transaction, transferInputs);
                insertedItems += InsertMissingMovements(connection, transaction, requisitions);
                insertedItems += InsertMissingMovements(connection, transaction, productionOutputs);

                transaction.Commit();
                return new StockMovementSyncResult
                {
                    ExpectedItems = expectedItems,
                    InsertedItems = insertedItems,
                };
            }
        }

        private static int CountActiveStockMovements(DbConnection connection, DbTransaction transaction)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "SELECT COUNT(*) FROM movimentos_estoque WHERE status = 'ATIVO'";
                return System.Convert.ToInt32(command.ExecuteScalar() ?? 0);
            }
        }

        private static int InsertMissingMovements(DbConnection connection, DbTransaction transaction, IReadOnlyCollection<StockMovementSyncItem> items)
        {
            var insertedItems = 0;
            foreach (var item in items)
            {
                using (var command = connection.CreateCommand())
                {
                    command.Transaction = transaction;
                    command.CommandText = @"
                        INSERT INTO movimentos_estoque
                            (documento_numero, documento_tipo, documento_item, data_movimento, tipo, fornecedor, almoxarifado, material, lote, quantidade, produto_utilizado, vencimento, usuario, status, dt_hr_criacao, dt_hr_alteracao)
                        SELECT
                            @numero, @documento_tipo, @documento_item, @data_movimento, @tipo, @fornecedor, @almoxarifado, @material, @lote, @quantidade, @produto_utilizado, @vencimento, @usuario, 'ATIVO', @agora, @agora
                        WHERE NOT EXISTS (
                            SELECT 1
                            FROM movimentos_estoque me
                            WHERE me.status = 'ATIVO'
                              AND me.documento_numero = @numero
                              AND me.documento_tipo = @documento_tipo
                              AND me.tipo = @tipo
                              AND COALESCE(me.fornecedor, '') = COALESCE(@fornecedor, '')
                              AND COALESCE(me.almoxarifado, '') = COALESCE(@almoxarifado, '')
                              AND COALESCE(me.material, '') = COALESCE(@material, '')
                              AND COALESCE(me.lote, '') = COALESCE(@lote, '')
                              AND COALESCE(me.quantidade, 0) = COALESCE(@quantidade, 0)
                              AND COALESCE(me.produto_utilizado, '') = COALESCE(@produto_utilizado, '')
                        )";

                    AddParameter(command, "numero", item.DocumentNumber ?? string.Empty);
                    AddParameter(command, "documento_tipo", item.DocumentType ?? string.Empty);
                    AddParameter(command, "documento_item", item.DocumentItem > 0 ? (object)item.DocumentItem : null);
                    AddParameter(command, "data_movimento", item.MovementDate ?? string.Empty);
                    AddParameter(command, "tipo", item.MovementType ?? string.Empty);
                    AddParameter(command, "fornecedor", item.SupplierToWrite);
                    AddParameter(command, "almoxarifado", item.Warehouse ?? string.Empty);
                    AddParameter(command, "material", item.Material ?? string.Empty);
                    AddParameter(command, "lote", item.Lot ?? string.Empty);
                    AddParameter(command, "quantidade", item.Quantity);
                    AddParameter(command, "produto_utilizado", string.IsNullOrWhiteSpace(item.ProductUsed) ? null : (object)item.ProductUsed);
                    AddParameter(command, "vencimento", item.ExpirationDate ?? string.Empty);
                    AddParameter(command, "usuario", "admin");
                    AddParameter(command, "agora", NowText());

                    insertedItems += command.ExecuteNonQuery();
                }
            }

            return insertedItems;
        }

        private static List<StockMovementSyncItem> LoadMissingNoteMovements(DbConnection connection, DbTransaction transaction)
        {
            var items = new List<StockMovementSyncItem>();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    WITH notas_ativas AS (
                        SELECT n.numero,
                               n.fornecedor,
                               COALESCE(n.dt_movimento, '') AS data_movimento,
                               COALESCE(n.almoxarifado, '') AS almoxarifado,
                               n.versao
                        FROM notas n
                        WHERE n.status = 'ATIVO'
                          AND n.versao = (
                              SELECT MAX(nx.versao)
                              FROM notas nx
                              WHERE nx.numero = n.numero
                                AND nx.fornecedor = n.fornecedor
                          )
                    ),
                    lotes_atuais AS (
                        SELECT l.codigo,
                               COALESCE(l.fornecedor, '') AS fornecedor_lote,
                               COALESCE(l.validade::text, '') AS vencimento
                        FROM lotes l
                        WHERE l.versao = (
                            SELECT MAX(lx.versao)
                            FROM lotes lx
                            WHERE lx.codigo = l.codigo
                        )
                    ),
                    itens_esperados AS (
                        SELECT na.numero AS documento_numero,
                               ROW_NUMBER() OVER (
                                   PARTITION BY na.numero, na.fornecedor
                                   ORDER BY COALESCE(ni.dt_hr_criacao, ''), ni.material, ni.lote, ni.quantidade
                               ) AS documento_item,
                               na.data_movimento,
                               na.almoxarifado,
                               na.fornecedor AS fornecedor_doc,
                               COALESCE(la.fornecedor_lote, '') AS fornecedor_lote,
                               COALESCE(ni.material, '') AS material,
                               COALESCE(ni.lote, '') AS lote,
                               COALESCE(ni.quantidade, 0) AS quantidade,
                               COALESCE(la.vencimento, '') AS vencimento,
                               ROW_NUMBER() OVER (
                                   PARTITION BY na.numero, na.fornecedor, na.almoxarifado, COALESCE(ni.material, ''), COALESCE(ni.lote, ''), COALESCE(ni.quantidade, 0)
                                   ORDER BY COALESCE(ni.dt_hr_criacao, ''), ni.material, ni.lote
                               ) AS match_seq
                        FROM notas_ativas na
                        INNER JOIN notas_itens ni
                            ON ni.numero = na.numero
                           AND ni.fornecedor = na.fornecedor
                           AND ni.versao = na.versao
                           AND COALESCE(ni.status, '') = 'ATIVO'
                        LEFT JOIN lotes_atuais la ON la.codigo = ni.lote
                    ),
                    movimentos_ativos AS (
                        SELECT me.documento_numero,
                               COALESCE(me.fornecedor, '') AS fornecedor_doc,
                               COALESCE(me.almoxarifado, '') AS almoxarifado,
                               COALESCE(me.material, '') AS material,
                               COALESCE(me.lote, '') AS lote,
                               COALESCE(me.quantidade, 0) AS quantidade,
                               ROW_NUMBER() OVER (
                                   PARTITION BY me.documento_numero, COALESCE(me.fornecedor, ''), COALESCE(me.almoxarifado, ''), COALESCE(me.material, ''), COALESCE(me.lote, ''), COALESCE(me.quantidade, 0)
                                   ORDER BY me.id
                               ) AS match_seq
                        FROM movimentos_estoque me
                        WHERE me.status = 'ATIVO'
                          AND me.documento_tipo = 'NOTA'
                          AND me.tipo = 'ENTRADA'
                    )
                    SELECT i.documento_numero,
                           i.documento_item,
                           i.data_movimento,
                           i.almoxarifado,
                           i.fornecedor_doc,
                           i.fornecedor_lote,
                           i.material,
                           i.lote,
                           i.quantidade,
                           i.vencimento
                    FROM itens_esperados i
                    LEFT JOIN movimentos_ativos m
                        ON m.documento_numero = i.documento_numero
                       AND m.fornecedor_doc = i.fornecedor_doc
                       AND m.almoxarifado = i.almoxarifado
                       AND m.material = i.material
                       AND m.lote = i.lote
                       AND m.quantidade = i.quantidade
                       AND m.match_seq = i.match_seq
                    WHERE m.documento_numero IS NULL
                    ORDER BY i.documento_numero, i.documento_item";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StockMovementSyncItem
                        {
                            Category = "NOTA",
                            DocumentNumber = ReadString(reader, "documento_numero"),
                            DocumentItem = ReadInt(reader, "documento_item"),
                            MovementDate = ReadString(reader, "data_movimento"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                            DocumentSupplier = ReadString(reader, "fornecedor_doc"),
                            LotSupplier = ReadString(reader, "fornecedor_lote"),
                            Material = ReadString(reader, "material"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            ExpirationDate = ReadString(reader, "vencimento"),
                            DocumentType = "NOTA",
                            MovementType = "ENTRADA",
                        });
                    }
                }
            }

            return items;
        }

        private static List<StockMovementSyncItem> LoadMissingTransferOutputMovements(DbConnection connection, DbTransaction transaction)
        {
            return LoadMissingTransferMovements(connection, transaction, "TRANSFERENCIA_SAIDA", "almox_origem");
        }

        private static List<StockMovementSyncItem> LoadMissingTransferInputMovements(DbConnection connection, DbTransaction transaction)
        {
            return LoadMissingTransferMovements(connection, transaction, "TRANSFERENCIA_ENTRADA", "almox_destino");
        }

        private static List<StockMovementSyncItem> LoadMissingTransferMovements(DbConnection connection, DbTransaction transaction, string movementType, string warehouseColumn)
        {
            var items = new List<StockMovementSyncItem>();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $@"
                    WITH transferencias_ativas AS (
                        SELECT t.numero,
                               COALESCE(t.dt_movimento, '') AS data_movimento,
                               COALESCE(t.{warehouseColumn}, '') AS almoxarifado,
                               t.versao
                        FROM transferencias t
                        WHERE t.status = 'ATIVO'
                          AND t.versao = (
                              SELECT MAX(tx.versao)
                              FROM transferencias tx
                              WHERE tx.numero = t.numero
                          )
                    ),
                    lotes_atuais AS (
                        SELECT l.codigo,
                               COALESCE(l.fornecedor, '') AS fornecedor_lote,
                               COALESCE(l.validade::text, '') AS vencimento
                        FROM lotes l
                        WHERE l.versao = (
                            SELECT MAX(lx.versao)
                            FROM lotes lx
                            WHERE lx.codigo = l.codigo
                        )
                    ),
                    itens_esperados AS (
                        SELECT ta.numero AS documento_numero,
                               COALESCE(ti.item_numero, ROW_NUMBER() OVER (
                                   PARTITION BY ta.numero
                                   ORDER BY COALESCE(ti.dt_hr_criacao, ''), ti.material, ti.lote, ti.quantidade
                               )) AS documento_item,
                               ta.data_movimento,
                               ta.almoxarifado,
                               '' AS fornecedor_doc,
                               COALESCE(la.fornecedor_lote, '') AS fornecedor_lote,
                               COALESCE(ti.material, '') AS material,
                               COALESCE(ti.lote, '') AS lote,
                               COALESCE(ti.quantidade, 0) AS quantidade,
                               COALESCE(la.vencimento, '') AS vencimento,
                               ROW_NUMBER() OVER (
                                   PARTITION BY ta.numero, ta.almoxarifado, COALESCE(la.fornecedor_lote, ''), COALESCE(ti.material, ''), COALESCE(ti.lote, ''), COALESCE(ti.quantidade, 0)
                                   ORDER BY COALESCE(ti.dt_hr_criacao, ''), COALESCE(ti.item_numero, 0), ti.material, ti.lote
                               ) AS match_seq
                        FROM transferencias_ativas ta
                        INNER JOIN transferencias_itens ti
                            ON ti.numero = ta.numero
                           AND ti.versao = ta.versao
                           AND COALESCE(ti.status, '') = 'ATIVO'
                        LEFT JOIN lotes_atuais la ON la.codigo = ti.lote
                    ),
                    movimentos_ativos AS (
                        SELECT me.documento_numero,
                               COALESCE(me.almoxarifado, '') AS almoxarifado,
                               COALESCE(me.fornecedor, '') AS fornecedor_lote,
                               COALESCE(me.material, '') AS material,
                               COALESCE(me.lote, '') AS lote,
                               COALESCE(me.quantidade, 0) AS quantidade,
                               ROW_NUMBER() OVER (
                                   PARTITION BY me.documento_numero, COALESCE(me.almoxarifado, ''), COALESCE(me.fornecedor, ''), COALESCE(me.material, ''), COALESCE(me.lote, ''), COALESCE(me.quantidade, 0)
                                   ORDER BY me.id
                               ) AS match_seq
                        FROM movimentos_estoque me
                        WHERE me.status = 'ATIVO'
                          AND me.documento_tipo = 'TRANSFERENCIA'
                          AND me.tipo = @tipo
                    )
                    SELECT i.documento_numero,
                           i.documento_item,
                           i.data_movimento,
                           i.almoxarifado,
                           i.fornecedor_doc,
                           i.fornecedor_lote,
                           i.material,
                           i.lote,
                           i.quantidade,
                           i.vencimento
                    FROM itens_esperados i
                    LEFT JOIN movimentos_ativos m
                        ON m.documento_numero = i.documento_numero
                       AND m.almoxarifado = i.almoxarifado
                       AND m.fornecedor_lote = i.fornecedor_lote
                       AND m.material = i.material
                       AND m.lote = i.lote
                       AND m.quantidade = i.quantidade
                       AND m.match_seq = i.match_seq
                    WHERE m.documento_numero IS NULL
                    ORDER BY i.documento_numero, i.documento_item";

                AddParameter(command, "tipo", movementType);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StockMovementSyncItem
                        {
                            Category = movementType,
                            DocumentNumber = ReadString(reader, "documento_numero"),
                            DocumentItem = ReadInt(reader, "documento_item"),
                            MovementDate = ReadString(reader, "data_movimento"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                            DocumentSupplier = ReadString(reader, "fornecedor_doc"),
                            LotSupplier = ReadString(reader, "fornecedor_lote"),
                            Material = ReadString(reader, "material"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            ExpirationDate = ReadString(reader, "vencimento"),
                            DocumentType = "TRANSFERENCIA",
                            MovementType = movementType,
                        });
                    }
                }
            }

            return items;
        }

        private static List<StockMovementSyncItem> LoadMissingRequisitionMovements(DbConnection connection, DbTransaction transaction)
        {
            var items = new List<StockMovementSyncItem>();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    WITH requisicoes_ativas AS (
                        SELECT r.numero,
                               COALESCE(r.dt_movimento, '') AS data_movimento,
                               r.versao
                        FROM requisicoes r
                        WHERE r.status = 'ATIVO'
                          AND r.versao = (
                              SELECT MAX(rx.versao)
                              FROM requisicoes rx
                              WHERE rx.numero = r.numero
                          )
                    ),
                    lotes_atuais AS (
                        SELECT l.codigo,
                               COALESCE(l.fornecedor, '') AS fornecedor_lote,
                               COALESCE(l.validade::text, '') AS vencimento
                        FROM lotes l
                        WHERE l.versao = (
                            SELECT MAX(lx.versao)
                            FROM lotes lx
                            WHERE lx.codigo = l.codigo
                        )
                    ),
                    itens_esperados AS (
                        SELECT ra.numero AS documento_numero,
                               COALESCE(ri.item_numero, ROW_NUMBER() OVER (
                                   PARTITION BY ra.numero
                                   ORDER BY COALESCE(ri.dt_hr_criacao, ''), ri.material, ri.lote, ri.quantidade
                               )) AS documento_item,
                               ra.data_movimento,
                               COALESCE(ri.almoxarifado, '') AS almoxarifado,
                               '' AS fornecedor_doc,
                               COALESCE(la.fornecedor_lote, '') AS fornecedor_lote,
                               COALESCE(ri.material, '') AS material,
                               COALESCE(ri.lote, '') AS lote,
                               COALESCE(ri.quantidade, 0) AS quantidade,
                               COALESCE(la.vencimento, '') AS vencimento,
                               ROW_NUMBER() OVER (
                                   PARTITION BY ra.numero, COALESCE(ri.almoxarifado, ''), COALESCE(la.fornecedor_lote, ''), COALESCE(ri.material, ''), COALESCE(ri.lote, ''), COALESCE(ri.quantidade, 0)
                                   ORDER BY COALESCE(ri.dt_hr_criacao, ''), COALESCE(ri.item_numero, 0), ri.material, ri.lote
                               ) AS match_seq
                        FROM requisicoes_ativas ra
                        INNER JOIN requisicoes_itens ri
                            ON ri.numero = ra.numero
                           AND ri.versao = ra.versao
                           AND COALESCE(ri.status, '') = 'ATIVO'
                        LEFT JOIN lotes_atuais la ON la.codigo = ri.lote
                    ),
                    movimentos_ativos AS (
                        SELECT me.documento_numero,
                               COALESCE(me.almoxarifado, '') AS almoxarifado,
                               COALESCE(me.fornecedor, '') AS fornecedor_lote,
                               COALESCE(me.material, '') AS material,
                               COALESCE(me.lote, '') AS lote,
                               COALESCE(me.quantidade, 0) AS quantidade,
                               ROW_NUMBER() OVER (
                                   PARTITION BY me.documento_numero, COALESCE(me.almoxarifado, ''), COALESCE(me.fornecedor, ''), COALESCE(me.material, ''), COALESCE(me.lote, ''), COALESCE(me.quantidade, 0)
                                   ORDER BY me.id
                               ) AS match_seq
                        FROM movimentos_estoque me
                        WHERE me.status = 'ATIVO'
                          AND me.documento_tipo = 'REQUISICAO'
                          AND me.tipo = 'REQUISICAO'
                    )
                    SELECT i.documento_numero,
                           i.documento_item,
                           i.data_movimento,
                           i.almoxarifado,
                           i.fornecedor_doc,
                           i.fornecedor_lote,
                           i.material,
                           i.lote,
                           i.quantidade,
                           i.vencimento
                    FROM itens_esperados i
                    LEFT JOIN movimentos_ativos m
                        ON m.documento_numero = i.documento_numero
                       AND m.almoxarifado = i.almoxarifado
                       AND m.fornecedor_lote = i.fornecedor_lote
                       AND m.material = i.material
                       AND m.lote = i.lote
                       AND m.quantidade = i.quantidade
                       AND m.match_seq = i.match_seq
                    WHERE m.documento_numero IS NULL
                    ORDER BY i.documento_numero, i.documento_item";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StockMovementSyncItem
                        {
                            Category = "REQUISICAO",
                            DocumentNumber = ReadString(reader, "documento_numero"),
                            DocumentItem = ReadInt(reader, "documento_item"),
                            MovementDate = ReadString(reader, "data_movimento"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                            DocumentSupplier = ReadString(reader, "fornecedor_doc"),
                            LotSupplier = ReadString(reader, "fornecedor_lote"),
                            Material = ReadString(reader, "material"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            ExpirationDate = ReadString(reader, "vencimento"),
                            DocumentType = "REQUISICAO",
                            MovementType = "REQUISICAO",
                        });
                    }
                }
            }

            return items;
        }

        private static List<StockMovementSyncItem> LoadMissingProductionOutputMovements(DbConnection connection, DbTransaction transaction)
        {
            var items = new List<StockMovementSyncItem>();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    WITH saidas_ativas AS (
                        SELECT sp.numero,
                               COALESCE(sp.dt_movimento, '') AS data_movimento,
                               sp.versao
                        FROM saidas_producao sp
                        WHERE sp.status = 'ATIVO'
                          AND sp.versao = (
                              SELECT MAX(sx.versao)
                              FROM saidas_producao sx
                              WHERE sx.numero = sp.numero
                          )
                    ),
                    lotes_atuais AS (
                        SELECT l.codigo,
                               COALESCE(l.fornecedor, '') AS fornecedor_lote,
                               COALESCE(l.validade::text, '') AS vencimento
                        FROM lotes l
                        WHERE l.versao = (
                            SELECT MAX(lx.versao)
                            FROM lotes lx
                            WHERE lx.codigo = l.codigo
                        )
                    ),
                    itens_esperados AS (
                        SELECT sa.numero AS documento_numero,
                               ROW_NUMBER() OVER (
                                   PARTITION BY sa.numero
                                   ORDER BY COALESCE(si.dt_hr_criacao, ''), si.material, si.lote, COALESCE(si.produto, '')
                               ) AS documento_item,
                               sa.data_movimento,
                               COALESCE(si.almoxarifado, '') AS almoxarifado,
                               '' AS fornecedor_doc,
                               COALESCE(la.fornecedor_lote, '') AS fornecedor_lote,
                               COALESCE(si.material, '') AS material,
                               COALESCE(si.lote, '') AS lote,
                               COALESCE(si.qtd_consumida, COALESCE(si.quantidade, 0)) AS quantidade,
                               COALESCE(la.vencimento, '') AS vencimento,
                               COALESCE(si.produto, '') AS produto_utilizado,
                               ROW_NUMBER() OVER (
                                   PARTITION BY sa.numero, COALESCE(si.almoxarifado, ''), COALESCE(la.fornecedor_lote, ''), COALESCE(si.material, ''), COALESCE(si.lote, ''), COALESCE(si.qtd_consumida, COALESCE(si.quantidade, 0)), COALESCE(si.produto, '')
                                   ORDER BY COALESCE(si.dt_hr_criacao, ''), si.material, si.lote, COALESCE(si.produto, '')
                               ) AS match_seq
                        FROM saidas_ativas sa
                        INNER JOIN saidas_producao_itens si
                            ON si.numero = sa.numero
                           AND si.versao = sa.versao
                           AND COALESCE(si.status, '') = 'ATIVO'
                        LEFT JOIN lotes_atuais la ON la.codigo = si.lote
                    ),
                    movimentos_ativos AS (
                        SELECT me.documento_numero,
                               COALESCE(me.almoxarifado, '') AS almoxarifado,
                               COALESCE(me.fornecedor, '') AS fornecedor_lote,
                               COALESCE(me.material, '') AS material,
                               COALESCE(me.lote, '') AS lote,
                               COALESCE(me.quantidade, 0) AS quantidade,
                               COALESCE(me.produto_utilizado, '') AS produto_utilizado,
                               ROW_NUMBER() OVER (
                                   PARTITION BY me.documento_numero, COALESCE(me.almoxarifado, ''), COALESCE(me.fornecedor, ''), COALESCE(me.material, ''), COALESCE(me.lote, ''), COALESCE(me.quantidade, 0), COALESCE(me.produto_utilizado, '')
                                   ORDER BY me.id
                               ) AS match_seq
                        FROM movimentos_estoque me
                        WHERE me.status = 'ATIVO'
                          AND me.documento_tipo = 'SAIDA_PRODUCAO'
                          AND me.tipo = 'SAIDA_PRODUCAO'
                    )
                    SELECT i.documento_numero,
                           i.documento_item,
                           i.data_movimento,
                           i.almoxarifado,
                           i.fornecedor_doc,
                           i.fornecedor_lote,
                           i.material,
                           i.lote,
                           i.quantidade,
                           i.vencimento,
                           i.produto_utilizado
                    FROM itens_esperados i
                    LEFT JOIN movimentos_ativos m
                        ON m.documento_numero = i.documento_numero
                       AND m.almoxarifado = i.almoxarifado
                       AND m.fornecedor_lote = i.fornecedor_lote
                       AND m.material = i.material
                       AND m.lote = i.lote
                       AND m.quantidade = i.quantidade
                       AND m.produto_utilizado = i.produto_utilizado
                       AND m.match_seq = i.match_seq
                    WHERE m.documento_numero IS NULL
                    ORDER BY i.documento_numero, i.documento_item";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StockMovementSyncItem
                        {
                            Category = "SAIDA_PRODUCAO",
                            DocumentNumber = ReadString(reader, "documento_numero"),
                            DocumentItem = ReadInt(reader, "documento_item"),
                            MovementDate = ReadString(reader, "data_movimento"),
                            Warehouse = ReadString(reader, "almoxarifado"),
                            DocumentSupplier = ReadString(reader, "fornecedor_doc"),
                            LotSupplier = ReadString(reader, "fornecedor_lote"),
                            Material = ReadString(reader, "material"),
                            Lot = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            ExpirationDate = ReadString(reader, "vencimento"),
                            DocumentType = "SAIDA_PRODUCAO",
                            MovementType = "SAIDA_PRODUCAO",
                            ProductUsed = ReadString(reader, "produto_utilizado"),
                        });
                    }
                }
            }

            return items;
        }
    }
}
