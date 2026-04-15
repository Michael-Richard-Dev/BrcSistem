using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlStockSummaryGateway : IStockSummaryGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlStockSummaryGateway(IDatabaseConnectionFactory connectionFactory)
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
                    SELECT codigo,
                           COALESCE(nome, '') AS nome,
                           COALESCE(empresa, '') AS empresa,
                           COALESCE(empresa_nome, '') AS empresa_nome,
                           COALESCE(status, 'ATIVO') AS status,
                           COALESCE(versao, 0) AS versao
                    FROM almoxarifados a
                    WHERE a.status = 'ATIVO'
                      AND a.versao = (
                          SELECT MAX(versao)
                          FROM almoxarifados x
                          WHERE x.codigo = a.codigo
                      )
                    ORDER BY nome, codigo";

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
                    SELECT codigo,
                           COALESCE(descricao, '') AS descricao,
                           COALESCE(habilitado_brc, FALSE) AS habilitado_brc,
                           COALESCE(status, 'ATIVO') AS status,
                           COALESCE(versao, 0) AS versao
                    FROM embalagens e
                    WHERE e.status = 'ATIVO'
                      AND e.versao = (
                          SELECT MAX(versao)
                          FROM embalagens x
                          WHERE x.codigo = e.codigo
                      )
                    ORDER BY descricao, codigo";

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
                    ORDER BY nome, codigo";

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

        public IReadOnlyCollection<StockSummaryEntry> LoadEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, StockSummaryQuery query)
        {
            var items = new List<StockSummaryEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    WITH saldos AS (
                        SELECT
                            m.almoxarifado,
                            m.material,
                            m.lote,
                            COALESCE(SUM(
                                CASE
                                    WHEN COALESCE(m.documento_tipo, '') = 'INVENTARIO' AND m.tipo = 'REQUISICAO' THEN -ABS(m.quantidade)
                                    WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN ABS(m.quantidade)
                                    WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -ABS(m.quantidade)
                                    ELSE 0
                                END
                            ), 0) AS quantidade
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                          AND DATE(m.data_movimento) <= DATE(@data_referencia)
                          AND NULLIF(TRIM(COALESCE(m.lote, '')), '') IS NOT NULL
                          AND (@almoxarifado = '' OR m.almoxarifado = @almoxarifado)
                          AND (@material = '' OR m.material = @material)
                          AND (@lote = '' OR m.lote = @lote)
                        GROUP BY m.almoxarifado, m.material, m.lote
                        HAVING COALESCE(SUM(
                            CASE
                                WHEN COALESCE(m.documento_tipo, '') = 'INVENTARIO' AND m.tipo = 'REQUISICAO' THEN -ABS(m.quantidade)
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN ABS(m.quantidade)
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -ABS(m.quantidade)
                                ELSE 0
                            END
                        ), 0) > 0
                    )
                    SELECT
                        s.almoxarifado,
                        COALESCE(a.nome, '') AS almoxarifado_nome,
                        s.material,
                        COALESCE(e.descricao, '') AS material_nome,
                        s.lote,
                        COALESCE(l.nome, '') AS lote_nome,
                        COALESCE(l.validade, '') AS validade,
                        s.quantidade
                    FROM saldos s
                    LEFT JOIN almoxarifados a ON a.codigo = s.almoxarifado
                        AND a.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = a.codigo
                        )
                    LEFT JOIN embalagens e ON e.codigo = s.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = s.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    ORDER BY s.almoxarifado, s.material, s.lote";

                command.Parameters.Add(CreateParameter(command, "@data_referencia", ParseBrDate(query.ReferenceDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", query.WarehouseCode ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@material", query.MaterialCode ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@lote", query.LotCode ?? string.Empty));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StockSummaryEntry
                        {
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            WarehouseName = ReadString(reader, "almoxarifado_nome"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            ExpirationDate = NormalizeDateText(ReadString(reader, "validade")),
                            Quantity = ReadDecimal(reader, "quantidade"),
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
