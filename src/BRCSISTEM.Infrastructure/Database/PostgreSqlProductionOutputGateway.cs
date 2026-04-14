using System;
using System.Collections.Generic;
using System.Data.Common;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlProductionOutputGateway : IProductionOutputGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlProductionOutputGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public string GenerateNextOutputNumber(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero
                    FROM saidas_producao
                    WHERE numero ~ '^SP[0-9]+$'
                    ORDER BY CAST(SUBSTRING(numero FROM 3) AS INTEGER) DESC
                    LIMIT 1";
                var result = command.ExecuteScalar();
                var current = result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
                if (string.IsNullOrWhiteSpace(current) || current.Length < 3)
                {
                    return "SP000001";
                }

                int sequence;
                return int.TryParse(current.Substring(2), out sequence)
                    ? "SP" + (sequence + 1).ToString("000000")
                    : "SP000001";
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

        public IReadOnlyCollection<ProductionOutputSummary> SearchOutputs(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter)
        {
            var items = new List<ProductionOutputSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT sp.numero,
                           COALESCE(sp.finalidade, '') AS finalidade,
                           COALESCE(sp.turno, '') AS turno,
                           COALESCE(sp.dt_movimento, '') AS dt_movimento,
                           COALESCE(sp.status, '') AS status,
                           COALESCE(sp.versao, 0) AS versao,
                           COALESCE(sp.bloqueado_por, '') AS bloqueado_por,
                           COALESCE(al.codigo, '') AS almoxarifado,
                           COALESCE(al.nome, '') AS almoxarifado_nome
                    FROM saidas_producao sp
                    INNER JOIN (
                        SELECT numero, MAX(versao) AS max_versao
                        FROM saidas_producao
                        GROUP BY numero
                    ) sx ON sx.numero = sp.numero AND sx.max_versao = sp.versao
                    LEFT JOIN LATERAL (
                        SELECT spi.almoxarifado
                        FROM saidas_producao_itens spi
                        WHERE spi.numero = sp.numero
                          AND spi.versao = sp.versao
                        ORDER BY spi.material, spi.lote
                        LIMIT 1
                    ) almox ON TRUE
                    LEFT JOIN almoxarifados al ON al.codigo = almox.almoxarifado
                        AND al.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = al.codigo
                        )
                    WHERE (@texto = ''
                        OR sp.numero ILIKE @texto_like
                        OR COALESCE(sp.finalidade, '') ILIKE @texto_like)
                    ORDER BY sp.dt_movimento DESC, sp.numero DESC";
                command.Parameters.Add(CreateParameter(command, "@texto", filter ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@texto_like", "%" + (filter ?? string.Empty) + "%"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new ProductionOutputSummary
                        {
                            Number = ReadString(reader, "numero"),
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            WarehouseName = ReadString(reader, "almoxarifado_nome"),
                            Purpose = ReadString(reader, "finalidade"),
                            Shift = ReadString(reader, "turno"),
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

        public ProductionOutputDetail LoadOutputDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            {
                var header = LoadLatestOutputHeader(connection, null, number);
                if (header == null)
                {
                    return null;
                }

                string warehouseCode = null;
                string warehouseName = null;
                var items = new List<ProductionOutputItemDetail>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COALESCE(i.produto, '') AS produto,
                               COALESCE(p.descricao, '') AS produto_desc,
                               i.material,
                               COALESCE(e.descricao, '') AS material_desc,
                               i.lote,
                               COALESCE(l.nome, '') AS lote_nome,
                               COALESCE(i.qtd_envio, 0) AS qtd_envio,
                               COALESCE(i.qtd_retorno, 0) AS qtd_retorno,
                               COALESCE(i.qtd_consumida, COALESCE(i.quantidade, 0)) AS qtd_consumida,
                               COALESCE(i.status, '') AS status,
                               COALESCE(i.almoxarifado, '') AS almoxarifado,
                               COALESCE(a.nome, '') AS almoxarifado_nome
                        FROM saidas_producao_itens i
                        LEFT JOIN produtos p ON p.codigo = i.produto
                            AND p.versao = (
                                SELECT MAX(versao)
                                FROM produtos x
                                WHERE x.codigo = p.codigo
                            )
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
                        LEFT JOIN almoxarifados a ON a.codigo = i.almoxarifado
                            AND a.versao = (
                                SELECT MAX(versao)
                                FROM almoxarifados x
                                WHERE x.codigo = a.codigo
                            )
                        WHERE i.numero = @numero
                          AND i.versao = @versao
                        ORDER BY UPPER(COALESCE(p.descricao, '')), UPPER(COALESCE(e.descricao, '')), UPPER(COALESCE(l.nome, '')), i.material, i.lote";
                    command.Parameters.Add(CreateParameter(command, "@numero", number));
                    command.Parameters.Add(CreateParameter(command, "@versao", header.Version));

                    using (var reader = command.ExecuteReader())
                    {
                        var itemNumber = 0;
                        while (reader.Read())
                        {
                            itemNumber++;
                            if (string.IsNullOrWhiteSpace(warehouseCode))
                            {
                                warehouseCode = ReadString(reader, "almoxarifado");
                                warehouseName = ReadString(reader, "almoxarifado_nome");
                            }

                            items.Add(new ProductionOutputItemDetail
                            {
                                ItemNumber = itemNumber,
                                ProductCode = ReadString(reader, "produto"),
                                ProductDescription = ReadString(reader, "produto_desc"),
                                MaterialCode = ReadString(reader, "material"),
                                MaterialDescription = ReadString(reader, "material_desc"),
                                LotCode = ReadString(reader, "lote"),
                                LotName = ReadString(reader, "lote_nome"),
                                QuantitySent = ReadDecimal(reader, "qtd_envio"),
                                QuantityReturned = ReadDecimal(reader, "qtd_retorno"),
                                QuantityConsumed = ReadDecimal(reader, "qtd_consumida"),
                                Status = ReadString(reader, "status"),
                            });
                        }
                    }
                }

                return new ProductionOutputDetail
                {
                    Number = header.Number,
                    WarehouseCode = warehouseCode,
                    WarehouseName = warehouseName,
                    Purpose = header.Purpose,
                    Shift = header.Shift,
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
            string excludedOutputNumber)
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
                          OR NOT (documento_tipo = 'SAIDA_PRODUCAO' AND documento_numero = @documento_excluido)
                      )";
                command.Parameters.Add(CreateParameter(command, "@material", materialCode));
                command.Parameters.Add(CreateParameter(command, "@lote", lotCode));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", warehouseCode));
                command.Parameters.Add(CreateParameter(command, "@data_movimento", movementDateTime ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@documento_excluido", excludedOutputNumber ?? string.Empty));
                return Convert.ToDecimal(command.ExecuteScalar() ?? 0M);
            }
        }
    }
}
