using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlStockLedgerGateway : IStockLedgerGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlStockLedgerGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IReadOnlyCollection<SupplierSummary> LoadSuppliers(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<SupplierSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DISTINCT m.fornecedor AS codigo,
                           COALESCE(f.nome, m.fornecedor, '') AS nome,
                           COALESCE(f.status, 'ATIVO') AS status
                    FROM movimentos_estoque m
                    LEFT JOIN fornecedores f ON f.codigo = m.fornecedor
                        AND f.versao = (
                            SELECT MAX(versao)
                            FROM fornecedores x
                            WHERE x.codigo = f.codigo
                        )
                    WHERE m.status = 'ATIVO'
                      AND NULLIF(TRIM(COALESCE(m.fornecedor, '')), '') IS NOT NULL
                    ORDER BY nome, codigo";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new SupplierSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Name = ReadString(reader, "nome"),
                            Status = ReadString(reader, "status"),
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<PackagingSummary> LoadMaterials(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode)
        {
            var items = new List<PackagingSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DISTINCT m.material AS codigo,
                           COALESCE(e.descricao, m.material, '') AS descricao,
                           COALESCE(e.habilitado_brc, FALSE) AS habilitado_brc,
                           COALESCE(e.status, 'ATIVO') AS status,
                           COALESCE(e.versao, 0) AS versao
                    FROM movimentos_estoque m
                    LEFT JOIN embalagens e ON e.codigo = m.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    WHERE m.status = 'ATIVO'
                      AND NULLIF(TRIM(COALESCE(m.material, '')), '') IS NOT NULL
                      AND (@fornecedor = '' OR m.fornecedor = @fornecedor)
                    ORDER BY descricao, codigo";
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode ?? string.Empty));

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

        public IReadOnlyCollection<WarehouseSummary> LoadWarehouses(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode)
        {
            var items = new List<WarehouseSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DISTINCT m.almoxarifado AS codigo,
                           COALESCE(a.nome, m.almoxarifado, '') AS nome,
                           COALESCE(a.empresa, '') AS empresa,
                           COALESCE(a.empresa_nome, '') AS empresa_nome,
                           COALESCE(a.status, 'ATIVO') AS status,
                           COALESCE(a.versao, 0) AS versao
                    FROM movimentos_estoque m
                    LEFT JOIN almoxarifados a ON a.codigo = m.almoxarifado
                        AND a.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = a.codigo
                        )
                    WHERE m.status = 'ATIVO'
                      AND NULLIF(TRIM(COALESCE(m.almoxarifado, '')), '') IS NOT NULL
                      AND (@fornecedor = '' OR m.fornecedor = @fornecedor)
                    ORDER BY nome, codigo";
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode ?? string.Empty));

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

        public IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings, string materialCode, string supplierCode)
        {
            var items = new List<LotSummary>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT DISTINCT m.lote AS codigo,
                           COALESCE(l.nome, m.lote, '') AS nome,
                           COALESCE(l.material, m.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(l.fornecedor, m.fornecedor, '') AS fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           COALESCE(l.validade, m.vencimento, '') AS validade,
                           COALESCE(l.status, 'ATIVO') AS status,
                           COALESCE(l.versao, 0) AS versao
                    FROM movimentos_estoque m
                    LEFT JOIN lotes l ON l.codigo = m.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    LEFT JOIN embalagens e ON e.codigo = COALESCE(l.material, m.material)
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN fornecedores f ON f.codigo = COALESCE(l.fornecedor, m.fornecedor)
                        AND f.versao = (
                            SELECT MAX(versao)
                            FROM fornecedores x
                            WHERE x.codigo = f.codigo
                        )
                    WHERE m.status = 'ATIVO'
                      AND NULLIF(TRIM(COALESCE(m.lote, '')), '') IS NOT NULL
                      AND (@material = '' OR m.material = @material)
                      AND (@fornecedor = '' OR m.fornecedor = @fornecedor)
                    ORDER BY nome, codigo";
                command.Parameters.Add(CreateParameter(command, "@material", materialCode ?? string.Empty));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode ?? string.Empty));

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

        public IReadOnlyCollection<StockLedgerEntry> SearchEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, StockLedgerQuery query)
        {
            var items = new List<StockLedgerEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                var whereClauses = new List<string> { "1=1" };
                AppendCommonFilters(command, query, whereClauses, "m.", includeDateRange: true, initialBalanceOnly: false);

                command.CommandText = @"
                    SELECT m.data_movimento,
                           COALESCE(m.documento_numero, '') AS documento_numero,
                           COALESCE(m.documento_tipo, '') AS documento_tipo,
                           COALESCE(m.tipo, '') AS tipo,
                           COALESCE(p.codigo || ' - ' || p.descricao, m.material, '') AS material_desc,
                           COALESCE(l.codigo || ' - ' || l.nome, m.lote, '') AS lote_desc,
                           COALESCE(l.validade, m.vencimento, '') AS data_validade,
                           COALESCE(a.codigo || ' - ' || a.nome, m.almoxarifado, '') AS almox_desc,
                           COALESCE(f.codigo || ' - ' || f.nome, m.fornecedor, '') AS fornecedor_desc,
                           COALESCE(m.quantidade, 0) AS quantidade,
                           COALESCE(m.usuario, '') AS usuario,
                           COALESCE(m.status, '') AS status,
                           COALESCE(m.id, 0) AS id,
                           COALESCE(m.dt_hr_criacao, '') AS dt_hr_criacao
                    FROM movimentos_estoque m
                    LEFT JOIN embalagens p ON p.codigo = m.material
                        AND p.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = p.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = m.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    LEFT JOIN almoxarifados a ON a.codigo = m.almoxarifado
                        AND a.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = a.codigo
                        )
                    LEFT JOIN fornecedores f ON f.codigo = m.fornecedor
                        AND f.versao = (
                            SELECT MAX(versao)
                            FROM fornecedores x
                            WHERE x.codigo = f.codigo
                        )
                    WHERE " + string.Join(" AND ", whereClauses) + @"
                    ORDER BY m.data_movimento DESC, m.id DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StockLedgerEntry
                        {
                            MovementDateTime = ReadString(reader, "data_movimento"),
                            DocumentNumber = ReadString(reader, "documento_numero"),
                            DocumentType = ReadString(reader, "documento_tipo"),
                            MovementType = ReadString(reader, "tipo"),
                            MaterialDisplay = ReadString(reader, "material_desc"),
                            LotDisplay = ReadString(reader, "lote_desc"),
                            ExpirationDate = NormalizeDateText(ReadString(reader, "data_validade")),
                            WarehouseDisplay = ReadString(reader, "almox_desc"),
                            SupplierDisplay = ReadString(reader, "fornecedor_desc"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            UserName = ReadString(reader, "usuario"),
                            Status = ReadString(reader, "status"),
                            MovementId = ReadInt(reader, "id"),
                            CreatedAt = ReadString(reader, "dt_hr_criacao"),
                        });
                    }
                }
            }

            return items;
        }

        public decimal GetInitialBalance(DatabaseProfile profile, ConnectionResilienceSettings settings, StockLedgerQuery query)
        {
            if (query == null || string.IsNullOrWhiteSpace(query.StartDate))
            {
                return 0M;
            }

            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                var whereClauses = new List<string> { "1=1" };
                AppendCommonFilters(command, query, whereClauses, string.Empty, includeDateRange: false, initialBalanceOnly: true);
                command.CommandText = @"
                    SELECT COALESCE(SUM(
                        CASE
                            WHEN tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN quantidade
                            WHEN tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -quantidade
                            ELSE 0
                        END
                    ), 0)
                    FROM movimentos_estoque
                    WHERE " + string.Join(" AND ", whereClauses);

                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value
                    ? 0M
                    : Convert.ToDecimal(result, CultureInfo.InvariantCulture);
            }
        }

        private static void AppendCommonFilters(DbCommand command, StockLedgerQuery query, List<string> whereClauses, string prefix, bool includeDateRange, bool initialBalanceOnly)
        {
            if (query == null)
            {
                return;
            }

            if (!query.IncludeInactive)
            {
                whereClauses.Add(prefix + "status = 'ATIVO'");
            }

            if (!string.IsNullOrWhiteSpace(query.StartDate))
            {
                var startIsoDate = ParseBrDate(query.StartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                if (includeDateRange)
                {
                    whereClauses.Add("DATE(" + prefix + "data_movimento) >= DATE(@data_inicial)");
                    command.Parameters.Add(CreateParameter(command, "@data_inicial", startIsoDate));
                }
                else if (initialBalanceOnly)
                {
                    whereClauses.Add("DATE(" + prefix + "data_movimento) < DATE(@data_inicial)");
                    command.Parameters.Add(CreateParameter(command, "@data_inicial", startIsoDate));
                }
            }

            if (includeDateRange && !string.IsNullOrWhiteSpace(query.EndDate))
            {
                whereClauses.Add("DATE(" + prefix + "data_movimento) <= DATE(@data_final)");
                command.Parameters.Add(CreateParameter(command, "@data_final", ParseBrDate(query.EndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrWhiteSpace(query.MaterialCode))
            {
                whereClauses.Add(prefix + "material = @material");
                command.Parameters.Add(CreateParameter(command, "@material", query.MaterialCode));
            }

            if (!string.IsNullOrWhiteSpace(query.LotCode))
            {
                whereClauses.Add(prefix + "lote = @lote");
                command.Parameters.Add(CreateParameter(command, "@lote", query.LotCode));
            }

            if (!string.IsNullOrWhiteSpace(query.WarehouseCode))
            {
                whereClauses.Add(prefix + "almoxarifado = @almoxarifado");
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", query.WarehouseCode));
            }

            if (!string.IsNullOrWhiteSpace(query.SupplierCode))
            {
                whereClauses.Add(prefix + "fornecedor = @fornecedor");
                command.Parameters.Add(CreateParameter(command, "@fornecedor", query.SupplierCode));
            }

            if (!string.IsNullOrWhiteSpace(query.MovementType))
            {
                if (string.Equals(query.MovementType, "INVENTARIO", StringComparison.OrdinalIgnoreCase))
                {
                    whereClauses.Add(prefix + "documento_tipo = 'INVENTARIO'");
                }
                else
                {
                    whereClauses.Add(prefix + "tipo = @tipo");
                    command.Parameters.Add(CreateParameter(command, "@tipo", query.MovementType));
                }
            }
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
