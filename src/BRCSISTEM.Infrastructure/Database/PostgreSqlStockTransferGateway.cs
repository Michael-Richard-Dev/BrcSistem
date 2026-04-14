using System;
using System.Collections.Generic;
using System.Data.Common;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlStockTransferGateway : IStockTransferGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlStockTransferGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public string GenerateNextTransferNumber(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero
                    FROM transferencias
                    WHERE numero ~ '^T[0-9]+$'
                    ORDER BY CAST(SUBSTRING(numero FROM 2) AS INTEGER) DESC
                    LIMIT 1";
                var result = command.ExecuteScalar();
                var current = result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
                if (string.IsNullOrWhiteSpace(current) || current.Length < 2)
                {
                    return "T00001";
                }

                int sequence;
                return int.TryParse(current.Substring(1), out sequence)
                    ? "T" + (sequence + 1).ToString("00000")
                    : "T00001";
            }
        }

        public IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName)
        {
            var items = new List<WarehouseSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                bool hasRestrictions;
                using (var restrictionCommand = connection.CreateCommand())
                {
                    restrictionCommand.CommandText = "SELECT COUNT(*) FROM usuario_almoxarifados WHERE UPPER(usuario) = UPPER(@usuario)";
                    restrictionCommand.Parameters.Add(CreateParameter(restrictionCommand, "@usuario", userName));
                    hasRestrictions = Convert.ToInt32(restrictionCommand.ExecuteScalar() ?? 0) > 0;
                }

                command.CommandText = hasRestrictions
                    ? @"
                        SELECT a.codigo,
                               COALESCE(a.nome, '') AS nome,
                               COALESCE(a.empresa, '') AS empresa,
                               COALESCE(a.empresa_nome, '') AS empresa_nome,
                               a.status,
                               a.versao
                        FROM almoxarifados a
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS max_versao
                            FROM almoxarifados
                            GROUP BY codigo
                        ) ax ON ax.codigo = a.codigo AND ax.max_versao = a.versao
                        INNER JOIN usuario_almoxarifados ua ON ua.codigo_almoxarifado = a.codigo
                        WHERE a.status = 'ATIVO'
                          AND UPPER(ua.usuario) = UPPER(@usuario)
                        ORDER BY a.nome, a.codigo"
                    : @"
                        SELECT a.codigo,
                               COALESCE(a.nome, '') AS nome,
                               COALESCE(a.empresa, '') AS empresa,
                               COALESCE(a.empresa_nome, '') AS empresa_nome,
                               a.status,
                               a.versao
                        FROM almoxarifados a
                        INNER JOIN (
                            SELECT codigo, MAX(versao) AS max_versao
                            FROM almoxarifados
                            GROUP BY codigo
                        ) ax ON ax.codigo = a.codigo AND ax.max_versao = a.versao
                        WHERE a.status = 'ATIVO'
                        ORDER BY a.nome, a.codigo";
                command.Parameters.Add(CreateParameter(command, "@usuario", userName));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new WarehouseSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Name = ReadString(reader, "nome"),
                            CompanyCode = ReadString(reader, "empresa"),
                            CompanyName = ReadString(reader, "empresa_nome"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<PackagingSummary> LoadMaterialsByWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode, string movementDateTime)
        {
            var items = new List<PackagingSummary>();
            if (string.IsNullOrWhiteSpace(warehouseCode))
            {
                return items;
            }

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    WITH saldos AS (
                        SELECT m.material,
                               COALESCE(SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ), 0) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND m.almoxarifado = @almoxarifado
                          AND (@data_movimento = '' OR m.data_movimento <= @data_movimento)
                        GROUP BY m.material
                        HAVING COALESCE(SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ), 0) > 0
                    )
                    SELECT e.codigo,
                           COALESCE(e.descricao, '') AS descricao,
                           COALESCE(e.habilitado_brc, FALSE) AS habilitado_brc,
                           e.status,
                           e.versao,
                           s.saldo
                    FROM saldos s
                    INNER JOIN embalagens e ON e.codigo = s.material
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM embalagens
                        GROUP BY codigo
                    ) ex ON ex.codigo = e.codigo AND ex.max_versao = e.versao
                    WHERE e.status = 'ATIVO'
                    ORDER BY COALESCE(e.descricao, ''), e.codigo";
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@data_movimento", movementDateTime ?? string.Empty));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new PackagingSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Description = ReadString(reader, "descricao"),
                            IsBrcEnabled = ReadBoolean(reader, "habilitado_brc"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                            StockBalance = ReadDecimal(reader, "saldo"),
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<LotSummary> LoadLotsByWarehouseAndMaterial(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode, string materialCode, string movementDateTime)
        {
            var items = new List<LotSummary>();
            if (string.IsNullOrWhiteSpace(warehouseCode) || string.IsNullOrWhiteSpace(materialCode))
            {
                return items;
            }

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    WITH saldos AS (
                        SELECT m.lote,
                               COALESCE(SUM(
                                   CASE
                                       WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                       WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                       ELSE 0
                                   END
                               ), 0) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND m.almoxarifado = @almoxarifado
                          AND m.material = @material
                          AND (@data_movimento = '' OR m.data_movimento <= @data_movimento)
                        GROUP BY m.lote
                        HAVING COALESCE(SUM(
                            CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END
                        ), 0) > 0
                    )
                    SELECT l.codigo,
                           COALESCE(l.nome, '') AS nome,
                           COALESCE(l.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(l.fornecedor, '') AS fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           COALESCE(l.validade, '') AS validade,
                           l.status,
                           l.versao,
                           s.saldo
                    FROM saldos s
                    INNER JOIN lotes l ON l.codigo = s.lote
                    INNER JOIN (
                        SELECT codigo, MAX(versao) AS max_versao
                        FROM lotes
                        GROUP BY codigo
                    ) lx ON lx.codigo = l.codigo AND lx.max_versao = l.versao
                    LEFT JOIN embalagens e ON e.codigo = l.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN fornecedores f ON f.codigo = l.fornecedor
                        AND f.versao = (
                            SELECT MAX(versao)
                            FROM fornecedores x
                            WHERE x.codigo = f.codigo
                        )
                    WHERE l.status = 'ATIVO'
                      AND l.material = @material
                    ORDER BY COALESCE(l.validade, ''), COALESCE(l.nome, ''), l.codigo";
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@material", materialCode));
                command.Parameters.Add(CreateParameter(command, "@data_movimento", movementDateTime ?? string.Empty));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new LotSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Name = ReadString(reader, "nome"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_desc"),
                            SupplierCode = ReadString(reader, "fornecedor"),
                            SupplierName = ReadString(reader, "fornecedor_nome"),
                            ExpirationDate = NormalizeDateText(ReadString(reader, "validade")),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                            StockBalance = ReadDecimal(reader, "saldo"),
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<StockTransferSummary> SearchTransfers(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter)
        {
            var items = new List<StockTransferSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT t.numero,
                           COALESCE(t.dt_movimento, '') AS dt_movimento,
                           COALESCE(t.almox_origem, '') AS almox_origem,
                           COALESCE(ao.nome, '') AS almox_origem_nome,
                           COALESCE(t.almox_destino, '') AS almox_destino,
                           COALESCE(ad.nome, '') AS almox_destino_nome,
                           COALESCE(t.status, '') AS status,
                           COALESCE(t.versao, 0) AS versao,
                           COALESCE(t.bloqueado_por, '') AS bloqueado_por
                    FROM transferencias t
                    INNER JOIN (
                        SELECT numero, MAX(versao) AS max_versao
                        FROM transferencias
                        GROUP BY numero
                    ) tx ON tx.numero = t.numero AND tx.max_versao = t.versao
                    LEFT JOIN almoxarifados ao ON ao.codigo = t.almox_origem
                        AND ao.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = ao.codigo
                        )
                    LEFT JOIN almoxarifados ad ON ad.codigo = t.almox_destino
                        AND ad.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = ad.codigo
                        )
                    WHERE (@texto = ''
                        OR t.numero ILIKE @texto_like
                        OR COALESCE(ao.nome, '') ILIKE @texto_like
                        OR COALESCE(ad.nome, '') ILIKE @texto_like)
                    ORDER BY t.dt_movimento DESC, t.numero DESC";
                command.Parameters.Add(CreateParameter(command, "@texto", filter ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@texto_like", "%" + (filter ?? string.Empty) + "%"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StockTransferSummary
                        {
                            Number = ReadString(reader, "numero"),
                            OriginWarehouseCode = ReadString(reader, "almox_origem"),
                            OriginWarehouseName = ReadString(reader, "almox_origem_nome"),
                            DestinationWarehouseCode = ReadString(reader, "almox_destino"),
                            DestinationWarehouseName = ReadString(reader, "almox_destino_nome"),
                            MovementDateTime = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                            LockedBy = ReadString(reader, "bloqueado_por"),
                        });
                    }
                }
            }

            return items;
        }

        public StockTransferDetail LoadTransferDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            {
                var header = LoadLatestTransferHeader(connection, null, number);
                if (header == null)
                {
                    return null;
                }

                var items = new List<StockTransferItemDetail>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(i.item_numero, 0) AS item_numero,
                               i.material,
                               COALESCE(e.descricao, '') AS material_desc,
                               i.lote,
                               COALESCE(l.nome, '') AS lote_nome,
                               COALESCE(i.quantidade, 0) AS quantidade,
                               COALESCE(i.status, '') AS status
                        FROM transferencias_itens i
                        LEFT JOIN embalagens e ON e.codigo = i.material
                            AND e.versao = (
                                SELECT MAX(versao)
                                FROM embalagens x
                                WHERE x.codigo = e.codigo
                            )
                        LEFT JOIN lotes l ON l.codigo = i.lote
                            AND l.versao = (
                                SELECT MAX(versao)
                                FROM lotes x
                                WHERE x.codigo = l.codigo
                            )
                        WHERE i.numero = @numero
                          AND i.versao = @versao
                        ORDER BY COALESCE(i.item_numero, 0), UPPER(COALESCE(e.descricao, '')), UPPER(COALESCE(l.nome, '')), i.material, i.lote";
                    command.Parameters.Add(CreateParameter(command, "@numero", number));
                    command.Parameters.Add(CreateParameter(command, "@versao", header.Version));

                    using (var reader = command.ExecuteReader())
                    {
                        var visualItemNumber = 0;
                        while (reader.Read())
                        {
                            visualItemNumber++;
                            var storedItemNumber = ReadInt(reader, "item_numero");
                            items.Add(new StockTransferItemDetail
                            {
                                ItemNumber = storedItemNumber > 0 ? storedItemNumber : visualItemNumber,
                                MaterialCode = ReadString(reader, "material"),
                                MaterialDescription = ReadString(reader, "material_desc"),
                                LotCode = ReadString(reader, "lote"),
                                LotName = ReadString(reader, "lote_nome"),
                                Quantity = ReadDecimal(reader, "quantidade"),
                                Status = ReadString(reader, "status"),
                            });
                        }
                    }
                }

                return new StockTransferDetail
                {
                    Number = header.Number,
                    OriginWarehouseCode = header.OriginWarehouseCode,
                    OriginWarehouseName = header.OriginWarehouseName,
                    DestinationWarehouseCode = header.DestinationWarehouseCode,
                    DestinationWarehouseName = header.DestinationWarehouseName,
                    MovementDateTime = NormalizeDateTimeText(header.MovementDateTime),
                    Status = header.Status,
                    Version = header.Version,
                    LockedBy = header.LockedBy,
                    Items = items.ToArray(),
                };
            }
        }

        public string GetParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string defaultValue)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COALESCE(valor, '') FROM parametros WHERE chave = @chave LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@chave", key));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(result))
                    ? defaultValue
                    : Convert.ToString(result);
            }
        }

        public decimal GetStockBalanceAt(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedTransferNumber)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COALESCE(SUM(
                        CASE
                            WHEN tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN quantidade
                            WHEN tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -quantidade
                            ELSE 0
                        END
                    ), 0)
                    FROM movimentos_estoque
                    WHERE status = 'ATIVO'
                      AND material = @material
                      AND lote = @lote
                      AND almoxarifado = @almoxarifado
                      AND (@data_movimento = '' OR data_movimento <= @data_movimento)
                      AND (
                          @documento_excluido = ''
                          OR NOT (documento_tipo = 'TRANSFERENCIA' AND documento_numero = @documento_excluido)
                      )";
                command.Parameters.Add(CreateParameter(command, "@material", materialCode));
                command.Parameters.Add(CreateParameter(command, "@lote", lotCode));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@data_movimento", movementDateTime ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@documento_excluido", excludedTransferNumber ?? string.Empty));
                return Convert.ToDecimal(command.ExecuteScalar() ?? 0M);
            }
        }
    }
}
