using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlProductionOutputReportGateway : IProductionOutputReportGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlProductionOutputReportGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
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

        public IReadOnlyCollection<ProductionOutputReportEntry> SearchEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, ProductionOutputReportQuery query)
        {
            var items = new List<ProductionOutputReportEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT sp.numero,
                           COALESCE(sp.finalidade, '') AS finalidade,
                           COALESCE(sp.turno, '') AS turno,
                           COALESCE(sp.dt_movimento, '') AS dt_movimento,
                           COALESCE(spi.almoxarifado, '') AS almoxarifado,
                           COALESCE(a.nome, '') AS almoxarifado_nome,
                           COALESCE(spi.produto, '') AS produto,
                           COALESCE(p.descricao, '') AS produto_desc,
                           COALESCE(spi.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(spi.lote, '') AS lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(spi.qtd_envio, 0) AS qtd_envio,
                           COALESCE(spi.qtd_retorno, 0) AS qtd_retorno,
                           COALESCE(spi.qtd_consumida, COALESCE(spi.quantidade, 0)) AS qtd_consumida,
                           CASE
                               WHEN UPPER(COALESCE(sp.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(sp.status, '')
                               ELSE COALESCE(NULLIF(TRIM(spi.status), ''), NULLIF(TRIM(sp.status), ''), 'ATIVO')
                           END AS status_efetivo
                    FROM saidas_producao sp
                    INNER JOIN (
                        SELECT numero, MAX(versao) AS max_versao
                        FROM saidas_producao
                        GROUP BY numero
                    ) sx ON sx.numero = sp.numero AND sx.max_versao = sp.versao
                    INNER JOIN saidas_producao_itens spi ON spi.numero = sp.numero
                        AND spi.versao = sp.versao
                    LEFT JOIN almoxarifados a ON a.codigo = spi.almoxarifado
                        AND a.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = a.codigo
                        )
                    LEFT JOIN produtos p ON p.codigo = spi.produto
                        AND p.versao = (
                            SELECT MAX(versao)
                            FROM produtos x
                            WHERE x.codigo = p.codigo
                        )
                    LEFT JOIN embalagens e ON e.codigo = spi.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = spi.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE (@data_inicial = '' OR DATE(sp.dt_movimento) >= DATE(@data_inicial))
                      AND (@data_final = '' OR DATE(sp.dt_movimento) <= DATE(@data_final))
                      AND (@numero = '' OR sp.numero ILIKE @numero_like)
                      AND (@almoxarifado = '' OR spi.almoxarifado = @almoxarifado)
                      AND (@produto = '' OR spi.produto = @produto)
                      AND (@turno = '' OR UPPER(COALESCE(sp.turno, '')) = @turno)
                      AND (
                          @usuario = ''
                          OR NOT EXISTS (
                              SELECT 1
                              FROM usuario_almoxarifados ua0
                              WHERE UPPER(ua0.usuario) = UPPER(@usuario)
                          )
                          OR EXISTS (
                              SELECT 1
                              FROM usuario_almoxarifados ua1
                              WHERE UPPER(ua1.usuario) = UPPER(@usuario)
                                AND ua1.codigo_almoxarifado = spi.almoxarifado
                          )
                      )
                      AND (
                          @excluir_canceladas = FALSE
                          OR LOWER(
                              CASE
                                  WHEN UPPER(COALESCE(sp.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(sp.status, '')
                                  ELSE COALESCE(NULLIF(TRIM(spi.status), ''), NULLIF(TRIM(sp.status), ''), 'ATIVO')
                              END
                          ) NOT IN ('cancelado', 'cancelada')
                      )
                    ORDER BY sp.dt_movimento DESC,
                             sp.numero DESC,
                             UPPER(COALESCE(p.descricao, spi.produto)),
                             UPPER(COALESCE(e.descricao, spi.material)),
                             UPPER(COALESCE(l.nome, spi.lote))";
                command.Parameters.Add(CreateParameter(command, "@data_inicial", ConvertBrDate(query != null ? query.StartDate : null)));
                command.Parameters.Add(CreateParameter(command, "@data_final", ConvertBrDate(query != null ? query.EndDate : null)));
                command.Parameters.Add(CreateParameter(command, "@numero", query != null ? query.OutputNumber ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@numero_like", "%" + (query != null ? query.OutputNumber ?? string.Empty : string.Empty) + "%"));
                command.Parameters.Add(CreateParameter(command, "@almoxarifado", query != null ? query.WarehouseCode ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@produto", query != null ? query.ProductCode ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@turno", (query != null ? query.Shift ?? string.Empty : string.Empty).ToUpperInvariant()));
                command.Parameters.Add(CreateParameter(command, "@usuario", query != null ? query.UserName ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@excluir_canceladas", query != null && query.ExcludeCanceled));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new ProductionOutputReportEntry
                        {
                            Number = ReadString(reader, "numero"),
                            Purpose = ReadString(reader, "finalidade"),
                            Shift = ReadString(reader, "turno"),
                            MovementDateTime = NormalizeDateTimeText(ReadString(reader, "dt_movimento")),
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            WarehouseName = ReadString(reader, "almoxarifado_nome"),
                            ProductCode = ReadString(reader, "produto"),
                            ProductDescription = ReadString(reader, "produto_desc"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_desc"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            QuantitySent = ReadDecimal(reader, "qtd_envio"),
                            QuantityReturned = ReadDecimal(reader, "qtd_retorno"),
                            QuantityConsumed = ReadDecimal(reader, "qtd_consumida"),
                            Status = ReadString(reader, "status_efetivo"),
                        });
                    }
                }
            }

            return items;
        }

        public ProductionOutputReportDocument LoadDocument(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT sp.numero,
                           COALESCE(sp.finalidade, '') AS finalidade,
                           COALESCE(sp.turno, '') AS turno,
                           COALESCE(sp.dt_movimento, '') AS dt_movimento,
                           COALESCE(sp.status, '') AS status_saida,
                           COALESCE(spi.almoxarifado, '') AS almoxarifado,
                           COALESCE(a.nome, '') AS almoxarifado_nome,
                           COALESCE(spi.produto, '') AS produto,
                           COALESCE(p.descricao, '') AS produto_desc,
                           COALESCE(spi.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(spi.lote, '') AS lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(spi.qtd_envio, 0) AS qtd_envio,
                           COALESCE(spi.qtd_retorno, 0) AS qtd_retorno,
                           COALESCE(spi.qtd_consumida, COALESCE(spi.quantidade, 0)) AS qtd_consumida,
                           CASE
                               WHEN UPPER(COALESCE(sp.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(sp.status, '')
                               ELSE COALESCE(NULLIF(TRIM(spi.status), ''), NULLIF(TRIM(sp.status), ''), 'ATIVO')
                           END AS status_item
                    FROM saidas_producao sp
                    INNER JOIN (
                        SELECT numero, MAX(versao) AS max_versao
                        FROM saidas_producao
                        GROUP BY numero
                    ) sx ON sx.numero = sp.numero AND sx.max_versao = sp.versao
                    INNER JOIN saidas_producao_itens spi ON spi.numero = sp.numero
                        AND spi.versao = sp.versao
                    LEFT JOIN almoxarifados a ON a.codigo = spi.almoxarifado
                        AND a.versao = (
                            SELECT MAX(versao)
                            FROM almoxarifados x
                            WHERE x.codigo = a.codigo
                        )
                    LEFT JOIN produtos p ON p.codigo = spi.produto
                        AND p.versao = (
                            SELECT MAX(versao)
                            FROM produtos x
                            WHERE x.codigo = p.codigo
                        )
                    LEFT JOIN embalagens e ON e.codigo = spi.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = spi.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE sp.numero = @numero
                    ORDER BY UPPER(COALESCE(p.descricao, spi.produto)),
                             UPPER(COALESCE(e.descricao, spi.material)),
                             UPPER(COALESCE(l.nome, spi.lote))";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    var items = new List<ProductionOutputReportItem>();
                    var document = new ProductionOutputReportDocument
                    {
                        Number = ReadString(reader, "numero"),
                        WarehouseCode = ReadString(reader, "almoxarifado"),
                        WarehouseName = ReadString(reader, "almoxarifado_nome"),
                        Purpose = ReadString(reader, "finalidade"),
                        Shift = ReadString(reader, "turno"),
                        MovementDateTime = NormalizeDateTimeText(ReadString(reader, "dt_movimento")),
                        Status = ReadString(reader, "status_saida"),
                    };

                    do
                    {
                        if (string.IsNullOrWhiteSpace(document.WarehouseCode))
                        {
                            document.WarehouseCode = ReadString(reader, "almoxarifado");
                            document.WarehouseName = ReadString(reader, "almoxarifado_nome");
                        }

                        items.Add(new ProductionOutputReportItem
                        {
                            ProductCode = ReadString(reader, "produto"),
                            ProductDescription = ReadString(reader, "produto_desc"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_desc"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            QuantitySent = ReadDecimal(reader, "qtd_envio"),
                            QuantityReturned = ReadDecimal(reader, "qtd_retorno"),
                            QuantityConsumed = ReadDecimal(reader, "qtd_consumida"),
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
                : value.Trim();
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
