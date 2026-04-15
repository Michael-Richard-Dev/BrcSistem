using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlInboundReceiptReportGateway : IInboundReceiptReportGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlInboundReceiptReportGateway(IDatabaseConnectionFactory connectionFactory)
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
                    SELECT f.codigo,
                           COALESCE(f.nome, '') AS nome,
                           COALESCE(f.cnpj, '') AS cnpj,
                           COALESCE(f.status, 'ATIVO') AS status,
                           COALESCE(f.versao, 0) AS versao
                    FROM fornecedores f
                    WHERE f.versao = (
                        SELECT MAX(versao)
                        FROM fornecedores x
                        WHERE x.codigo = f.codigo
                    )
                    ORDER BY nome, codigo";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new SupplierSummary
                        {
                            Code = ReadString(reader, "codigo"),
                            Name = ReadString(reader, "nome"),
                            Cnpj = ReadString(reader, "cnpj"),
                            Status = ReadString(reader, "status"),
                            Version = ReadInt(reader, "versao"),
                        });
                    }
                }
            }

            return items;
        }

        public IReadOnlyCollection<InboundReceiptReportEntry> SearchEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, InboundReceiptReportQuery query)
        {
            var items = new List<InboundReceiptReportEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT n.numero,
                           n.fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           ni.material,
                           COALESCE(e.descricao, '') AS material_nome,
                           COALESCE(ni.quantidade, 0) AS quantidade,
                           ni.lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(l.validade, '') AS validade,
                           COALESCE(n.dt_movimento, '') AS dt_movimento,
                           CASE
                               WHEN UPPER(COALESCE(n.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(n.status, '')
                               ELSE COALESCE(NULLIF(TRIM(ni.status), ''), NULLIF(TRIM(n.status), ''), 'ATIVO')
                           END AS status_efetivo
                    FROM notas n
                    INNER JOIN (
                        SELECT numero, fornecedor, MAX(versao) AS max_versao
                        FROM notas
                        GROUP BY numero, fornecedor
                    ) nx ON nx.numero = n.numero AND nx.fornecedor = n.fornecedor AND nx.max_versao = n.versao
                    INNER JOIN notas_itens ni ON ni.numero = n.numero
                        AND ni.fornecedor = n.fornecedor
                        AND ni.versao = n.versao
                    LEFT JOIN fornecedores f ON f.codigo = n.fornecedor
                        AND f.versao = (
                            SELECT MAX(versao)
                            FROM fornecedores x
                            WHERE x.codigo = f.codigo
                        )
                    LEFT JOIN embalagens e ON e.codigo = ni.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = ni.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE (@data_inicial = '' OR DATE(n.dt_movimento) >= DATE(@data_inicial))
                      AND (@data_final = '' OR DATE(n.dt_movimento) <= DATE(@data_final))
                      AND (@numero = '' OR n.numero LIKE @numero_like)
                      AND (@fornecedor = '' OR n.fornecedor = @fornecedor)
                      AND (
                          @excluir_canceladas = FALSE
                          OR LOWER(
                              CASE
                                  WHEN UPPER(COALESCE(n.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(n.status, '')
                                  ELSE COALESCE(NULLIF(TRIM(ni.status), ''), NULLIF(TRIM(n.status), ''), 'ATIVO')
                              END
                          ) NOT IN ('cancelado', 'cancelada')
                      )
                    ORDER BY n.dt_movimento DESC, n.numero DESC, UPPER(COALESCE(e.descricao, ni.material)), ni.material";
                command.Parameters.Add(CreateParameter(command, "@data_inicial", ConvertBrDate(query != null ? query.StartDate : null)));
                command.Parameters.Add(CreateParameter(command, "@data_final", ConvertBrDate(query != null ? query.EndDate : null)));
                command.Parameters.Add(CreateParameter(command, "@numero", query != null ? query.ReceiptNumber ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@numero_like", "%" + (query != null ? query.ReceiptNumber ?? string.Empty : string.Empty) + "%"));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", query != null ? query.SupplierCode ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@excluir_canceladas", query != null && query.ExcludeCanceled));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InboundReceiptReportEntry
                        {
                            Number = ReadString(reader, "numero"),
                            SupplierCode = ReadString(reader, "fornecedor"),
                            SupplierName = ReadString(reader, "fornecedor_nome"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            ReceiptDateTime = ReadString(reader, "dt_movimento"),
                            Status = ReadString(reader, "status_efetivo"),
                            ExpirationDate = NormalizeDateText(ReadString(reader, "validade")),
                        });
                    }
                }
            }

            return items;
        }

        public InboundReceiptReportDocument LoadDocument(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT n.numero,
                           n.fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           COALESCE(n.almoxarifado, '') AS almoxarifado,
                           COALESCE(a.nome, '') AS almoxarifado_nome,
                           COALESCE(n.dt_emissao, '') AS dt_emissao,
                           COALESCE(n.dt_movimento, '') AS dt_movimento,
                           COALESCE(n.status, '') AS status_nota,
                           ni.material,
                           COALESCE(e.descricao, '') AS material_nome,
                           COALESCE(ni.quantidade, 0) AS quantidade,
                           ni.lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(l.validade, '') AS validade,
                           CASE
                               WHEN UPPER(COALESCE(n.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(n.status, '')
                               ELSE COALESCE(NULLIF(TRIM(ni.status), ''), NULLIF(TRIM(n.status), ''), 'ATIVO')
                           END AS status_item
                    FROM notas n
                    INNER JOIN (
                        SELECT numero, fornecedor, MAX(versao) AS max_versao
                        FROM notas
                        GROUP BY numero, fornecedor
                    ) nx ON nx.numero = n.numero AND nx.fornecedor = n.fornecedor AND nx.max_versao = n.versao
                    INNER JOIN notas_itens ni ON ni.numero = n.numero
                        AND ni.fornecedor = n.fornecedor
                        AND ni.versao = n.versao
                    LEFT JOIN fornecedores f ON f.codigo = n.fornecedor
                        AND f.versao = (
                            SELECT MAX(versao)
                            FROM fornecedores x
                            WHERE x.codigo = f.codigo
                        )
                    LEFT JOIN almoxarifados a ON a.codigo = n.almoxarifado
                        AND a.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = a.codigo
                        )
                    LEFT JOIN embalagens e ON e.codigo = ni.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = ni.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE n.numero = @numero
                      AND n.fornecedor = @fornecedor
                    ORDER BY UPPER(COALESCE(e.descricao, ni.material)), ni.material, UPPER(COALESCE(l.nome, ni.lote)), ni.lote";
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    var items = new List<InboundReceiptReportItem>();
                    var document = new InboundReceiptReportDocument
                    {
                        Number = ReadString(reader, "numero"),
                        SupplierCode = ReadString(reader, "fornecedor"),
                        SupplierName = ReadString(reader, "fornecedor_nome"),
                        WarehouseCode = ReadString(reader, "almoxarifado"),
                        WarehouseName = ReadString(reader, "almoxarifado_nome"),
                        EmissionDate = NormalizeDateText(ReadString(reader, "dt_emissao")),
                        ReceiptDateTime = NormalizeDateTimeText(ReadString(reader, "dt_movimento")),
                        Status = ReadString(reader, "status_nota"),
                    };

                    do
                    {
                        items.Add(new InboundReceiptReportItem
                        {
                            MaterialCode = ReadString(reader, "material"),
                            MaterialName = ReadString(reader, "material_nome"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            ExpirationDate = NormalizeDateText(ReadString(reader, "validade")),
                            ReceiptDateTime = NormalizeDateTimeText(ReadString(reader, "dt_movimento")),
                            Status = ReadString(reader, "status_item"),
                        });
                    }
                    while (reader.Read());

                    document.Items = items.ToArray();
                    return document;
                }
            }
        }

        private static string ConvertBrDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            DateTime parsed;
            return DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                : string.Empty;
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

        private static string NormalizeDateTimeText(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm" };
            if (DateTime.TryParseExact(rawValue.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
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
