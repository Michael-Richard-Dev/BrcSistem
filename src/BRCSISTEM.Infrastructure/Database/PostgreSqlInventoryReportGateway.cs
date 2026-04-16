using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlInventoryReportGateway : IInventoryReportGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlInventoryReportGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IReadOnlyCollection<InventoryReportEntry> LoadInventories(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var items = new List<InventoryReportEntry>();
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT i.numero,
                           COALESCE(i.status, '') AS status,
                           COALESCE(i.dt_hr_criacao, '') AS dt_hr_criacao,
                           COALESCE(i.dt_finalizacao, '') AS dt_finalizacao
                    FROM inventarios i
                    WHERE i.versao = (
                        SELECT MAX(x.versao)
                        FROM inventarios x
                        WHERE x.numero = i.numero
                    )
                    ORDER BY COALESCE(i.dt_hr_alteracao, i.dt_hr_criacao) DESC NULLS LAST,
                             i.numero DESC
                    LIMIT 400";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventoryReportEntry
                        {
                            Number = ReadString(reader, "numero"),
                            Status = ReadString(reader, "status"),
                            CreatedAt = ReadString(reader, "dt_hr_criacao"),
                            FinalizedAt = ReadString(reader, "dt_finalizacao"),
                        });
                    }
                }
            }

            return items;
        }

        public InventoryReportDocument LoadDocument(DatabaseProfile profile, ConnectionResilienceSettings settings, string number)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            {
                var header = LoadHeader(connection, number);
                if (header == null)
                {
                    return null;
                }

                header.Items = LoadItems(connection, number).ToArray();
                header.Movements = LoadMovements(connection, number).ToArray();
                return header;
            }
        }

        private static InventoryReportDocument LoadHeader(DbConnection connection, string number)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT numero,
                           COALESCE(status, '') AS status,
                           COALESCE(usuario, '') AS usuario,
                           COALESCE(fechado_por, '') AS fechado_por,
                           COALESCE(cancelado_por, '') AS cancelado_por,
                           COALESCE(motivo_cancelamento, '') AS motivo_cancelamento,
                           COALESCE(observacao, '') AS observacao,
                           COALESCE(max_pontos, 0) AS max_pontos,
                           COALESCE(dt_hr_criacao, '') AS dt_hr_criacao,
                           COALESCE(dt_abertura, '') AS dt_abertura,
                           COALESCE(dt_fechamento, '') AS dt_fechamento,
                           COALESCE(dt_finalizacao, '') AS dt_finalizacao
                    FROM inventarios
                    WHERE numero = @numero
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new InventoryReportDocument
                    {
                        Number = ReadString(reader, "numero"),
                        Status = ReadString(reader, "status"),
                        CreatedBy = ReadString(reader, "usuario"),
                        ClosedBy = ReadString(reader, "fechado_por"),
                        CanceledBy = ReadString(reader, "cancelado_por"),
                        CancellationReason = ReadString(reader, "motivo_cancelamento"),
                        Observation = ReadString(reader, "observacao"),
                        MaxOpenPoints = ReadInt(reader, "max_pontos"),
                        CreatedAt = NormalizeDateTimeText(ReadString(reader, "dt_hr_criacao")),
                        OpenedAt = NormalizeDateTimeText(ReadString(reader, "dt_abertura")),
                        ClosedAt = NormalizeDateTimeText(ReadString(reader, "dt_fechamento")),
                        FinalizedAt = NormalizeDateTimeText(ReadString(reader, "dt_finalizacao")),
                    };
                }
            }
        }

        private static List<InventoryReportItem> LoadItems(DbConnection connection, string number)
        {
            var items = new List<InventoryReportItem>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT ii.almoxarifado,
                           ii.material,
                           COALESCE(e.descricao, '') AS material_desc,
                           ii.lote,
                           COALESCE(l.nome, '') AS lote_nome,
                           COALESCE(ii.saldo_sistema, 0) AS saldo_sistema,
                           COALESCE(ii.quantidade_contada, 0) AS quantidade_contada,
                           COALESCE(ii.ajuste, COALESCE(ii.quantidade_contada, 0) - COALESCE(ii.saldo_sistema, 0)) AS ajuste,
                           COALESCE(
                               ii.tipo_ajuste,
                               CASE
                                   WHEN COALESCE(ii.quantidade_contada, 0) - COALESCE(ii.saldo_sistema, 0) > 0 THEN 'ENTRADA'
                                   WHEN COALESCE(ii.quantidade_contada, 0) - COALESCE(ii.saldo_sistema, 0) < 0 THEN 'REQUISICAO'
                                   ELSE 'SEM_AJUSTE'
                               END
                           ) AS tipo_ajuste,
                           COALESCE(l.validade, '') AS validade,
                           COALESCE(ii.status, 'ATIVO') AS status
                    FROM inventarios_itens ii
                    LEFT JOIN embalagens e ON e.codigo = ii.material
                        AND e.versao = (
                            SELECT MAX(x.versao)
                            FROM embalagens x
                            WHERE x.codigo = e.codigo
                        )
                    LEFT JOIN lotes l ON l.codigo = ii.lote
                        AND l.material = ii.material
                        AND l.versao = (
                            SELECT MAX(x.versao)
                            FROM lotes x
                            WHERE x.codigo = l.codigo
                              AND x.material = l.material
                        )
                    WHERE ii.numero = @numero
                      AND COALESCE(ii.status, 'ATIVO') = 'ATIVO'
                      AND ii.versao = (
                          SELECT MAX(x.versao)
                          FROM inventarios_itens x
                          WHERE x.numero = ii.numero
                            AND x.almoxarifado = ii.almoxarifado
                            AND x.material = ii.material
                            AND x.lote = ii.lote
                      )
                    ORDER BY ii.almoxarifado, ii.material, ii.lote";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventoryReportItem
                        {
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            MaterialCode = ReadString(reader, "material"),
                            MaterialDescription = ReadString(reader, "material_desc"),
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            SystemBalance = ReadDecimal(reader, "saldo_sistema"),
                            CountedQuantity = ReadDecimal(reader, "quantidade_contada"),
                            AdjustmentQuantity = ReadDecimal(reader, "ajuste"),
                            AdjustmentType = ReadString(reader, "tipo_ajuste"),
                            ExpirationDate = ReadString(reader, "validade"),
                            Status = ReadString(reader, "status"),
                        });
                    }
                }
            }

            return items;
        }

        private static List<InventoryReportMovement> LoadMovements(DbConnection connection, string number)
        {
            var items = new List<InventoryReportMovement>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT COALESCE(data_movimento, '') AS data_movimento,
                           COALESCE(documento_item, 0) AS documento_item,
                           COALESCE(tipo, '') AS tipo,
                           COALESCE(fornecedor, '') AS fornecedor,
                           COALESCE(almoxarifado, '') AS almoxarifado,
                           COALESCE(material, '') AS material,
                           COALESCE(lote, '') AS lote,
                           COALESCE(quantidade, 0) AS quantidade,
                           COALESCE(vencimento, '') AS vencimento,
                           COALESCE(usuario, '') AS usuario,
                           COALESCE(status, '') AS status
                    FROM movimentos_estoque
                    WHERE documento_tipo = 'INVENTARIO'
                      AND documento_numero = @numero
                    ORDER BY status DESC, documento_item";
                command.Parameters.Add(CreateParameter(command, "@numero", number));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventoryReportMovement
                        {
                            MovementDateTime = NormalizeDateTimeText(ReadString(reader, "data_movimento")),
                            ItemNumber = ReadInt(reader, "documento_item"),
                            Type = ReadString(reader, "tipo"),
                            SupplierCode = ReadString(reader, "fornecedor"),
                            WarehouseCode = ReadString(reader, "almoxarifado"),
                            MaterialCode = ReadString(reader, "material"),
                            LotCode = ReadString(reader, "lote"),
                            Quantity = ReadDecimal(reader, "quantidade"),
                            ExpirationDate = ReadString(reader, "vencimento"),
                            UserName = ReadString(reader, "usuario"),
                            Status = ReadString(reader, "status"),
                        });
                    }
                }
            }

            return items;
        }

        private static string NormalizeDateTimeText(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm" };
            return DateTime.TryParseExact(rawValue.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                : rawValue.Trim();
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
            return reader.IsDBNull(ordinal) ? 0 : Convert.ToInt32(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }

        private static decimal ReadDecimal(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? 0M : Convert.ToDecimal(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
        }
    }
}
