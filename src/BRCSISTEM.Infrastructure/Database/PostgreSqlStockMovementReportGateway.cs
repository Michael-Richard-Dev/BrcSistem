using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlStockMovementReportGateway : IStockMovementReportGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlStockMovementReportGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IReadOnlyCollection<WarehouseSummary> LoadWarehouses(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<WarehouseSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT a.codigo,
                           COALESCE(a.nome, '') AS nome,
                           COALESCE(a.empresa, '') AS empresa,
                           COALESCE(a.empresa_nome, '') AS empresa_nome,
                           COALESCE(a.status, 'ATIVO') AS status,
                           COALESCE(a.versao, 0) AS versao
                    FROM almoxarifados a
                    WHERE a.status = 'ATIVO'
                      AND a.versao = (
                          SELECT MAX(versao)
                          FROM almoxarifados x
                          WHERE x.codigo = a.codigo
                      )
                    ORDER BY a.codigo";

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

        public IReadOnlyCollection<PackagingSummary> LoadMaterials(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<PackagingSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT e.codigo,
                           COALESCE(e.descricao, '') AS descricao,
                           COALESCE(e.habilitado_brc, FALSE) AS habilitado_brc,
                           COALESCE(e.status, 'ATIVO') AS status,
                           COALESCE(e.versao, 0) AS versao
                    FROM embalagens e
                    WHERE e.status = 'ATIVO'
                      AND e.versao = (
                          SELECT MAX(versao)
                          FROM embalagens x
                          WHERE x.codigo = e.codigo
                      )
                    ORDER BY e.codigo";

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
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<LotSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT l.codigo,
                           COALESCE(l.nome, '') AS nome,
                           COALESCE(l.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(l.fornecedor, '') AS fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           COALESCE(l.validade, '') AS validade,
                           COALESCE(l.status, 'ATIVO') AS status,
                           COALESCE(l.versao, 0) AS versao
                    FROM lotes l
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
                      AND l.versao = (
                          SELECT MAX(versao)
                          FROM lotes x
                          WHERE x.codigo = l.codigo
                      )
                    ORDER BY l.codigo";

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
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<StockMovementReportRow> SearchRows(DatabaseProfile profile, ConnectionResilienceSettings settings, StockMovementReportQuery query)
        {
            var items = new List<StockMovementReportRow>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                var dataInicial = !string.IsNullOrWhiteSpace(query != null ? query.StartDate : null)
                    ? ParseBrDate(query.StartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : "1900-01-01";
                var dataFinal = !string.IsNullOrWhiteSpace(query != null ? query.EndDate : null)
                    ? ParseBrDate(query.EndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    : DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                command.CommandText = @"
                    WITH combinacoes AS (
                        SELECT DISTINCT almoxarifado, material, lote
                        FROM movimentos_estoque
                        WHERE status = 'ATIVO'
                          AND almoxarifado IS NOT NULL
                          AND material IS NOT NULL
                          AND lote IS NOT NULL
                    ),
                    saldo_inicial AS (
                        SELECT
                            m.almoxarifado,
                            m.material,
                            m.lote,
                            COALESCE(SUM(
                                CASE
                                    WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                    WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                    ELSE 0
                                END
                            ), 0) AS valor
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND DATE(m.data_movimento) < DATE(@data_inicial)
                        GROUP BY m.almoxarifado, m.material, m.lote
                    ),
                    movimentos_periodo AS (
                        SELECT
                            m.almoxarifado,
                            m.material,
                            m.lote,
                            COALESCE(SUM(CASE
                                WHEN m.tipo = 'ENTRADA' AND COALESCE(m.documento_tipo, '') <> 'INVENTARIO' THEN ABS(m.quantidade)
                                ELSE 0
                            END), 0) AS entradas,
                            COALESCE(SUM(CASE WHEN m.tipo = 'TRANSFERENCIA_ENTRADA' THEN ABS(m.quantidade) ELSE 0 END), 0) AS transf_entrada,
                            COALESCE(SUM(CASE WHEN m.tipo = 'TRANSFERENCIA_SAIDA' THEN ABS(m.quantidade) ELSE 0 END), 0) AS transf_saida,
                            COALESCE(SUM(CASE WHEN m.tipo = 'SAIDA_PRODUCAO' THEN ABS(m.quantidade) ELSE 0 END), 0) AS saida_producao,
                            COALESCE(SUM(CASE
                                WHEN m.tipo = 'REQUISICAO' AND COALESCE(m.documento_tipo, '') <> 'INVENTARIO' THEN ABS(m.quantidade)
                                ELSE 0
                            END), 0) AS requisicao,
                            COALESCE(SUM(CASE
                                WHEN COALESCE(m.documento_tipo, '') = 'INVENTARIO' AND m.tipo = 'ENTRADA' THEN ABS(m.quantidade)
                                WHEN COALESCE(m.documento_tipo, '') = 'INVENTARIO' AND m.tipo = 'REQUISICAO' THEN -ABS(m.quantidade)
                                ELSE 0
                            END), 0) AS inventario
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND DATE(m.data_movimento) >= DATE(@data_inicial)
                          AND DATE(m.data_movimento) <= DATE(@data_final)
                        GROUP BY m.almoxarifado, m.material, m.lote
                    )
                    SELECT
                        c.almoxarifado,
                        c.material,
                        c.lote,
                        COALESCE(a.nome, '') AS almoxarifado_nome,
                        COALESCE(e.descricao, '') AS material_nome,
                        COALESCE(l.nome, '') AS lote_nome,
                        COALESCE(l.validade, '') AS validade,
                        COALESCE(si.valor, 0) AS saldo_inicial,
                        COALESCE(mp.entradas, 0) AS entradas,
                        COALESCE(mp.transf_entrada, 0) AS transf_entrada,
                        COALESCE(mp.transf_saida, 0) AS transf_saida,
                        COALESCE(mp.saida_producao, 0) AS saida_producao,
                        COALESCE(mp.requisicao, 0) AS requisicao,
                        COALESCE(mp.inventario, 0) AS inventario
                    FROM combinacoes c
                    LEFT JOIN almoxarifados a ON c.almoxarifado = a.codigo
                        AND a.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = a.codigo
                        )
                    LEFT JOIN embalagens e ON c.material = e.codigo
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON c.lote = l.codigo
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    LEFT JOIN saldo_inicial si ON c.almoxarifado = si.almoxarifado
                        AND c.material = si.material
                        AND c.lote = si.lote
                    LEFT JOIN movimentos_periodo mp ON c.almoxarifado = mp.almoxarifado
                        AND c.material = mp.material
                        AND c.lote = mp.lote
                    WHERE (@almoxarifado = '' OR c.almoxarifado = @almoxarifado)
                      AND (@material = '' OR c.material = @material)
                      AND (@lote = '' OR c.lote = @lote)
                    ORDER BY c.almoxarifado, c.material, c.lote";

                command.Parameters.Add(CreateParameter(command, "@data_inicial", dataInicial));
                command.Parameters.Add(CreateParameter(command, "@data_final", dataFinal));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", query != null ? query.WarehouseCode : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@material", query != null ? query.MaterialCode : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@lote", query != null ? query.LotCode : string.Empty));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var openingBalance = ReadDecimal(reader, "saldo_inicial");
                        var entries = ReadDecimal(reader, "entradas");
                        var transferIn = ReadDecimal(reader, "transf_entrada");
                        var transferOut = ReadDecimal(reader, "transf_saida");
                        var productionOutput = ReadDecimal(reader, "saida_producao");
                        var requisition = ReadDecimal(reader, "requisicao");
                        var inventory = ReadDecimal(reader, "inventario");

                        items.Add(new StockMovementReportRow
                        {
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            WarehouseName = ReadString(reader, "almoxarifado_nome"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            ExpirationDate = NormalizeDateText(ReadString(reader, "validade")),
                            OpeningBalance = openingBalance,
                            Entries = entries,
                            TransferIn = transferIn,
                            TransferOut = transferOut,
                            ProductionOutput = productionOutput,
                            Requisition = requisition,
                            Inventory = inventory,
                            FinalBalance = openingBalance + entries + transferIn - transferOut - productionOutput - requisition + inventory,
                        });
                    }
                }
            }

            return items;
        }

        private static DateTime ParseBrDate(string value)
        {
            DateTime parsed;
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static string NormalizeDateText(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            DateTime parsed;
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss" };
            if (DateTime.TryParseExact(rawValue.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return parsed.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            return rawValue.Trim();
        }

        private static DbParameter CreateParameter(DbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        private static string ReadString(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? null : Convert.ToString(reader.GetValue(ordinal));
        }

        private static bool ReadBoolean(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return !reader.IsDBNull(ordinal) && Convert.ToBoolean(reader.GetValue(ordinal));
        }

        private static int ReadInt(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal));
        }

        private static decimal ReadDecimal(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0M : Convert.ToDecimal(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }
    }
}
