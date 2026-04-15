using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInventoryGateway
    {
        public IReadOnlyCollection<InventorySummary> SearchInventories(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter)
        {
            var items = new List<InventorySummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    WITH ultimos AS (
                        SELECT i.numero,
                               MAX(i.versao) AS max_versao
                        FROM inventarios i
                        GROUP BY i.numero
                    )
                    SELECT i.numero,
                           COALESCE(i.status, '') AS status,
                           COALESCE(i.dt_hr_criacao, '') AS dt_hr_criacao,
                           COALESCE(i.dt_abertura, '') AS dt_abertura,
                           COALESCE(i.dt_fechamento, '') AS dt_fechamento,
                           COALESCE(i.dt_finalizacao, '') AS dt_finalizacao,
                           COALESCE(i.bloqueado_por, '') AS bloqueado_por,
                           (SELECT COUNT(*) FROM inventarios_itens ii WHERE ii.numero = i.numero AND ii.status = 'ATIVO') AS itens,
                           (SELECT COUNT(*) FROM inventario_pontos p WHERE p.numero = i.numero AND p.status = 'ABERTO') AS pontos_abertos
                    FROM inventarios i
                    INNER JOIN ultimos u ON u.numero = i.numero AND u.max_versao = i.versao
                    WHERE (
                        @filtro = ''
                        OR UPPER(i.numero) LIKE UPPER(@filtro_like)
                        OR UPPER(COALESCE(i.status, '')) LIKE UPPER(@filtro_like)
                        OR UPPER(COALESCE(i.usuario, '')) LIKE UPPER(@filtro_like)
                    )
                    ORDER BY COALESCE(i.dt_hr_alteracao, i.dt_hr_criacao) DESC, i.numero DESC";
                command.Parameters.Add(CreateParameter(command, "@filtro", filter ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@filtro_like", "%" + (filter ?? string.Empty) + "%"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventorySummary
                        {
                            Number = ReadString(reader, "numero"),
                            Status = ReadString(reader, "status"),
                            CreatedAt = ReadString(reader, "dt_hr_criacao"),
                            OpenedAt = ReadString(reader, "dt_abertura"),
                            ClosedAt = ReadString(reader, "dt_fechamento"),
                            FinalizedAt = ReadString(reader, "dt_finalizacao"),
                            LockedBy = ReadString(reader, "bloqueado_por"),
                            ItemCount = ReadInt(reader, "itens"),
                            OpenPointCount = ReadInt(reader, "pontos_abertos"),
                        });
                    }
                }
            }

            return items;
        }

        public InventoryDetail LoadInventoryDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction())
            {
                var header = LoadLatestInventoryHeader(connection, transaction, number);
                if (header == null)
                {
                    transaction.Commit();
                    return null;
                }

                var detail = new InventoryDetail
                {
                    Number = header.Number,
                    Status = header.Status,
                    CreatedBy = header.CreatedBy,
                    Observation = header.Observation,
                    MaxOpenPoints = header.MaxOpenPoints,
                    Version = header.Version,
                    LockedBy = header.LockedBy,
                    CreatedAt = header.CreatedAt,
                    ScheduledAt = header.ScheduledAt,
                    OpenedAt = header.OpenedAt,
                    ClosedAt = header.ClosedAt,
                    FinalizedAt = header.FinalizedAt,
                    ClosedBy = header.ClosedBy,
                    CanceledBy = header.CanceledBy,
                    CancellationReason = header.CancellationReason,
                    Items = LoadInventoryItems(connection, transaction, number).ToArray(),
                    Points = LoadInventoryPoints(connection, transaction, number).ToArray(),
                    Counts = LoadInventoryCounts(connection, transaction, number).ToArray(),
                };

                transaction.Commit();
                return detail;
            }
        }

        public InventoryItemConflict FindItemConflict(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string inventoryNumber,
            string warehouseCode,
            string materialCode,
            string lotCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT i.numero,
                           COALESCE(i.status, '') AS status
                    FROM inventarios_itens ii
                    INNER JOIN inventarios i ON i.numero = ii.numero
                    WHERE ii.almoxarifado = @almoxarifado
                      AND ii.material = @material
                      AND ii.lote = @lote
                      AND COALESCE(ii.status, 'ATIVO') = 'ATIVO'
                      AND i.numero <> @numero_atual
                      AND i.versao = (
                          SELECT MAX(ix.versao)
                          FROM inventarios ix
                          WHERE ix.numero = i.numero
                      )
                      AND UPPER(COALESCE(i.status, 'PENDENTE')) IN ('INICIADO', 'EM_CONTAGEM', 'FECHADO')
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@material", materialCode));
                command.Parameters.Add(CreateParameter(command, "@lote", lotCode));
                command.Parameters.Add(CreateParameter(command, "@numero_atual", inventoryNumber ?? string.Empty));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new InventoryItemConflict
                    {
                        InventoryNumber = ReadString(reader, "numero"),
                        InventoryStatus = ReadString(reader, "status"),
                    };
                }
            }
        }

        public IReadOnlyCollection<OpenMovementLockSummary> LoadOpenMovements(DatabaseProfile profile, ConnectionResilienceSettings settings, int limit)
        {
            var items = new List<OpenMovementLockSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 'ENTRADA' AS tipo, n.numero AS documento, n.bloqueado_por AS usuario, n.bloqueado_em AS dt_hr
                    FROM notas n
                    WHERE n.status = 'ATIVO'
                      AND COALESCE(n.bloqueado_por, '') <> ''
                      AND n.versao = (
                          SELECT MAX(x.versao)
                          FROM notas x
                          WHERE x.numero = n.numero AND x.fornecedor = n.fornecedor
                      )
                    UNION ALL
                    SELECT 'TRANSFERENCIA' AS tipo, t.numero AS documento, t.bloqueado_por AS usuario, t.bloqueado_em AS dt_hr
                    FROM transferencias t
                    WHERE t.status = 'ATIVO'
                      AND COALESCE(t.bloqueado_por, '') <> ''
                      AND t.versao = (
                          SELECT MAX(x.versao)
                          FROM transferencias x
                          WHERE x.numero = t.numero
                      )
                    UNION ALL
                    SELECT 'SAIDA_PRODUCAO' AS tipo, sp.numero AS documento, sp.bloqueado_por AS usuario, sp.bloqueado_em AS dt_hr
                    FROM saidas_producao sp
                    WHERE sp.status = 'ATIVO'
                      AND COALESCE(sp.bloqueado_por, '') <> ''
                      AND sp.versao = (
                          SELECT MAX(x.versao)
                          FROM saidas_producao x
                          WHERE x.numero = sp.numero
                      )
                    UNION ALL
                    SELECT 'REQUISICAO' AS tipo, r.numero AS documento, r.bloqueado_por AS usuario, r.bloqueado_em AS dt_hr
                    FROM requisicoes r
                    WHERE r.status = 'ATIVO'
                      AND COALESCE(r.bloqueado_por, '') <> ''
                      AND r.versao = (
                          SELECT MAX(x.versao)
                          FROM requisicoes x
                          WHERE x.numero = r.numero
                      )
                    ORDER BY dt_hr DESC NULLS LAST
                    LIMIT @limite";
                command.Parameters.Add(CreateParameter(command, "@limite", limit));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new OpenMovementLockSummary
                        {
                            Type = ReadString(reader, "tipo"),
                            DocumentNumber = ReadString(reader, "documento"),
                            UserName = ReadString(reader, "usuario"),
                            LockedAt = ReadString(reader, "dt_hr"),
                        });
                    }
                }
            }

            return items;
        }

        public string GetParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string defaultValue)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT valor FROM parametros WHERE chave = @chave";
                command.Parameters.Add(CreateParameter(command, "@chave", key));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? defaultValue : Convert.ToString(result);
            }
        }

        public decimal GetStockBalanceAt(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string materialCode,
            string lotCode,
            string movementDateTime)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var transaction = connection.BeginTransaction())
            {
                var value = GetStockBalanceAtInternal(connection, transaction, warehouseCode, materialCode, lotCode, movementDateTime);
                transaction.Commit();
                return value;
            }
        }

        private static List<InventoryItemDetail> LoadInventoryItems(DbConnection connection, DbTransaction transaction, string number)
        {
            var items = new List<InventoryItemDetail>();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT ROW_NUMBER() OVER (ORDER BY ii.almoxarifado, ii.material, ii.lote) AS item_numero,
                           ii.almoxarifado,
                           ii.material,
                           COALESCE(e.descricao, '') AS material_nome,
                           ii.lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(ii.saldo_sistema, 0) AS saldo_sistema,
                           ii.quantidade_contada,
                           ii.ajuste,
                           COALESCE(ii.tipo_ajuste, '') AS tipo_ajuste,
                           COALESCE(ii.status, '') AS status
                    FROM inventarios_itens ii
                    LEFT JOIN embalagens e ON e.codigo = ii.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = ii.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE ii.numero = @numero
                      AND ii.status = 'ATIVO'
                    ORDER BY ii.almoxarifado, ii.material, ii.lote";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventoryItemDetail
                        {
                            ItemNumber = ReadInt(reader, "item_numero"),
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_nome"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            SystemBalance = ReadDecimal(reader, "saldo_sistema"),
                            CountedQuantity = reader.IsDBNull(reader.GetOrdinal("quantidade_contada")) ? (decimal?)null : ReadDecimal(reader, "quantidade_contada"),
                            AdjustmentQuantity = reader.IsDBNull(reader.GetOrdinal("ajuste")) ? (decimal?)null : ReadDecimal(reader, "ajuste"),
                            AdjustmentType = ReadString(reader, "tipo_ajuste"),
                            Status = ReadString(reader, "status"),
                        });
                    }
                }
            }

            return items;
        }

        private static List<InventoryPointSummary> LoadInventoryPoints(DbConnection connection, DbTransaction transaction, string number)
        {
            var items = new List<InventoryPointSummary>();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT id,
                           COALESCE(nome_ponto, '') AS nome_ponto,
                           COALESCE(ip_ponto, '') AS ip_ponto,
                           COALESCE(computador, '') AS computador,
                           COALESCE(usuario_abertura, '') AS usuario_abertura,
                           COALESCE(usuario_fechamento, '') AS usuario_fechamento,
                           COALESCE(status, '') AS status,
                           COALESCE(dt_abertura, '') AS dt_abertura,
                           COALESCE(dt_fechamento, '') AS dt_fechamento,
                           COALESCE(dt_heartbeat, '') AS dt_heartbeat
                    FROM inventario_pontos
                    WHERE numero = @numero
                    ORDER BY id";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventoryPointSummary
                        {
                            Id = ReadInt(reader, "id"),
                            PointName = ReadString(reader, "nome_ponto"),
                            IpAddress = ReadString(reader, "ip_ponto"),
                            ComputerName = ReadString(reader, "computador"),
                            OpenedBy = ReadString(reader, "usuario_abertura"),
                            ClosedBy = ReadString(reader, "usuario_fechamento"),
                            Status = ReadString(reader, "status"),
                            OpenedAt = ReadString(reader, "dt_abertura"),
                            ClosedAt = ReadString(reader, "dt_fechamento"),
                            HeartbeatAt = ReadString(reader, "dt_heartbeat"),
                        });
                    }
                }
            }

            return items;
        }

        private static List<InventoryCountSummary> LoadInventoryCounts(DbConnection connection, DbTransaction transaction, string number)
        {
            var items = new List<InventoryCountSummary>();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT c.id,
                           COALESCE(c.ponto_id, 0) AS ponto_id,
                           COALESCE(c.almoxarifado, '') AS almoxarifado,
                           COALESCE(c.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_nome,
                           COALESCE(c.lote, '') AS lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(c.quantidade, 0) AS quantidade,
                           COALESCE(c.usuario, '') AS usuario,
                           COALESCE(c.dt_hr, '') AS dt_hr
                    FROM inventario_contagens c
                    LEFT JOIN embalagens e ON e.codigo = c.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = c.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE c.numero = @numero
                      AND c.status = 'ATIVO'
                    ORDER BY c.id DESC
                    LIMIT 300";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventoryCountSummary
                        {
                            Id = ReadInt(reader, "id"),
                            PointId = ReadInt(reader, "ponto_id"),
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_nome"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            UserName = ReadString(reader, "usuario"),
                            CountedAt = ReadString(reader, "dt_hr"),
                        });
                    }
                }
            }

            return items;
        }
    }
}
