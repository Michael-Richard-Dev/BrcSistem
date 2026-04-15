using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlStockTransferReportGateway : IStockTransferReportGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlStockTransferReportGateway(IDatabaseConnectionFactory connectionFactory)
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

        public IReadOnlyCollection<StockTransferReportEntry> SearchEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, StockTransferReportQuery query)
        {
            var items = new List<StockTransferReportEntry>();
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
                           COALESCE(ti.item_numero, 0) AS item_numero,
                           COALESCE(ti.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(ti.lote, '') AS lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(ti.quantidade, 0) AS quantidade,
                           CASE
                               WHEN UPPER(COALESCE(t.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(t.status, '')
                               ELSE COALESCE(NULLIF(TRIM(ti.status), ''), NULLIF(TRIM(t.status), ''), 'ATIVO')
                           END AS status_efetivo
                    FROM transferencias t
                    INNER JOIN (
                        SELECT numero, MAX(versao) AS max_versao
                        FROM transferencias
                        GROUP BY numero
                    ) tx ON tx.numero = t.numero AND tx.max_versao = t.versao
                    INNER JOIN transferencias_itens ti ON ti.numero = t.numero
                        AND ti.versao = t.versao
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
                    LEFT JOIN embalagens e ON e.codigo = ti.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = ti.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE (@data_inicial = '' OR DATE(t.dt_movimento) >= DATE(@data_inicial))
                      AND (@data_final = '' OR DATE(t.dt_movimento) <= DATE(@data_final))
                      AND (@numero = '' OR t.numero ILIKE @numero_like)
                      AND (@origem = '' OR t.almox_origem = @origem)
                      AND (@destino = '' OR t.almox_destino = @destino)
                      AND (@material = '' OR ti.material = @material)
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
                                AND ua1.codigo_almoxarifado IN (t.almox_origem, t.almox_destino)
                          )
                      )
                      AND (
                          @excluir_canceladas = FALSE
                          OR LOWER(
                              CASE
                                  WHEN UPPER(COALESCE(t.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(t.status, '')
                                  ELSE COALESCE(NULLIF(TRIM(ti.status), ''), NULLIF(TRIM(t.status), ''), 'ATIVO')
                              END
                          ) NOT IN ('cancelado', 'cancelada')
                      )
                    ORDER BY t.dt_movimento DESC,
                             t.numero DESC,
                             COALESCE(ti.item_numero, 0),
                             UPPER(COALESCE(e.descricao, ti.material)),
                             UPPER(COALESCE(l.nome, ti.lote))";
                command.Parameters.Add(CreateParameter(command, "@data_inicial", ConvertBrDate(query != null ? query.StartDate : null)));
                command.Parameters.Add(CreateParameter(command, "@data_final", ConvertBrDate(query != null ? query.EndDate : null)));
                command.Parameters.Add(CreateParameter(command, "@numero", query != null ? query.TransferNumber ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@numero_like", "%" + (query != null ? query.TransferNumber ?? string.Empty : string.Empty) + "%"));
                command.Parameters.Add(CreateParameter(command, "@origem", query != null ? query.OriginWarehouseCode ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@destino", query != null ? query.DestinationWarehouseCode ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@material", query != null ? query.MaterialCode ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@usuario", query != null ? query.UserName ?? string.Empty : string.Empty));
                command.Parameters.Add(CreateParameter(command, "@excluir_canceladas", query != null && query.ExcludeCanceled));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new StockTransferReportEntry
                        {
                            Number = ReadString(reader, "numero"),
                            ItemNumber = ReadInt(reader, "item_numero"),
                            OriginWarehouseCode = ReadString(reader, "almox_origem"),
                            OriginWarehouseName = ReadString(reader, "almox_origem_nome"),
                            DestinationWarehouseCode = ReadString(reader, "almox_destino"),
                            DestinationWarehouseName = ReadString(reader, "almox_destino_nome"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_desc"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            MovementDateTime = NormalizeDateTimeText(ReadString(reader, "dt_movimento")),
                            Status = ReadString(reader, "status_efetivo"),
                        });
                    }
                }
            }

            return items;
        }

        public StockTransferReportDocument LoadDocument(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
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
                           COALESCE(t.status, '') AS status_transferencia,
                           COALESCE(ti.item_numero, 0) AS item_numero,
                           COALESCE(ti.material, '') AS material,
                           COALESCE(e.descricao, '') AS material_desc,
                           COALESCE(ti.lote, '') AS lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(ti.quantidade, 0) AS quantidade,
                           CASE
                               WHEN UPPER(COALESCE(t.status, '')) IN ('CANCELADO', 'CANCELADA') THEN COALESCE(t.status, '')
                               ELSE COALESCE(NULLIF(TRIM(ti.status), ''), NULLIF(TRIM(t.status), ''), 'ATIVO')
                           END AS status_item
                    FROM transferencias t
                    INNER JOIN (
                        SELECT numero, MAX(versao) AS max_versao
                        FROM transferencias
                        GROUP BY numero
                    ) tx ON tx.numero = t.numero AND tx.max_versao = t.versao
                    INNER JOIN transferencias_itens ti ON ti.numero = t.numero
                        AND ti.versao = t.versao
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
                    LEFT JOIN embalagens e ON e.codigo = ti.material
                        AND e.versao = (
                            SELECT MAX(versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = ti.lote
                        AND l.versao = (
                            SELECT MAX(versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                        )
                    WHERE t.numero = @numero
                    ORDER BY COALESCE(ti.item_numero, 0),
                             UPPER(COALESCE(e.descricao, ti.material)),
                             UPPER(COALESCE(l.nome, ti.lote))";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    var items = new List<StockTransferReportItem>();
                    var document = new StockTransferReportDocument
                    {
                        Number = ReadString(reader, "numero"),
                        OriginWarehouseCode = ReadString(reader, "almox_origem"),
                        OriginWarehouseName = ReadString(reader, "almox_origem_nome"),
                        DestinationWarehouseCode = ReadString(reader, "almox_destino"),
                        DestinationWarehouseName = ReadString(reader, "almox_destino_nome"),
                        MovementDateTime = NormalizeDateTimeText(ReadString(reader, "dt_movimento")),
                        Status = ReadString(reader, "status_transferencia"),
                    };

                    do
                    {
                        items.Add(new StockTransferReportItem
                        {
                            ItemNumber = ReadInt(reader, "item_numero"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_desc"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            Quantity = ReadDecimal(reader, "quantidade"),
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
