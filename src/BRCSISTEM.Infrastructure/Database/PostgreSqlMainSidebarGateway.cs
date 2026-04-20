using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlMainSidebarGateway : IMainSidebarGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlMainSidebarGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public MainSidebarSnapshot LoadSnapshot(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var snapshot = new MainSidebarSnapshot();

            using (var connection = _connectionFactory.Open(profile, settings))
            {
                snapshot.FifoEntries = LoadFifo(connection);
                snapshot.CadastroRows = LoadCadastros(connection);
                snapshot.VolumeRows = LoadVolumeEstoque(connection);
                snapshot.AuditRows = LoadAuditoria(connection);
                snapshot.ActiveUsersCount = LoadActiveUsersCount(connection);
                snapshot.RecentAccesses = LoadRecentAccesses(connection);
            }

            return snapshot;
        }

        private static MainSidebarFifoEntry[] LoadFifo(DbConnection connection)
        {
            var items = new List<MainSidebarFifoEntry>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    WITH saldos AS (
                        SELECT
                            m.lote,
                            l.nome AS lote_nome,
                            l.validade,
                            e.descricao AS material,
                            SUM(CASE
                                WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END) AS saldo
                        FROM movimentos_estoque m
                        INNER JOIN lotes l ON m.lote = l.codigo
                            AND l.versao = (SELECT MAX(versao) FROM lotes WHERE codigo = l.codigo)
                        INNER JOIN embalagens e ON m.material = e.codigo
                            AND e.versao = (SELECT MAX(versao) FROM embalagens WHERE codigo = e.codigo)
                        WHERE m.status = 'ATIVO'
                          AND l.validade IS NOT NULL
                          AND l.validade != ''
                          AND l.validade::date BETWEEN CURRENT_DATE AND CURRENT_DATE + INTERVAL '60 days'
                        GROUP BY m.lote, l.nome, l.validade, e.descricao
                        HAVING SUM(CASE
                            WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                            WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                            ELSE 0
                        END) > 0
                    )
                    SELECT lote, lote_nome, validade, material, saldo
                    FROM saldos
                    ORDER BY validade, lote
                    LIMIT 10";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MainSidebarFifoEntry
                        {
                            LotCode = ReadString(reader, "lote"),
                            LotName = ReadString(reader, "lote_nome"),
                            Material = ReadString(reader, "material"),
                            ExpirationDate = NormalizeDateText(reader, "validade", true),
                            Balance = ReadDecimal(reader, "saldo"),
                        });
                    }
                }
            }

            return items.ToArray();
        }

        private static MainSidebarCadastroRow[] LoadCadastros(DbConnection connection)
        {
            return new[]
            {
                LoadCadastroRow(connection, "Embalagens", "embalagens", includeBrc: true),
                LoadCadastroRow(connection, "Lotes", "lotes", includeBrc: false),
                LoadCadastroRow(connection, "Fornecedores", "fornecedores", includeBrc: false),
                LoadCadastroRow(connection, "Produtos", "produtos", includeBrc: false),
            };
        }

        private static MainSidebarCadastroRow LoadCadastroRow(DbConnection connection, string label, string tableName, bool includeBrc)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = includeBrc
                    ? @"
                        SELECT
                            COUNT(*) AS total,
                            SUM(CASE WHEN status = 'ATIVO' THEN 1 ELSE 0 END) AS ativos,
                            SUM(CASE WHEN status = 'INATIVO' THEN 1 ELSE 0 END) AS inativos,
                            SUM(CASE WHEN habilitado_brc = TRUE THEN 1 ELSE 0 END) AS brc
                        FROM embalagens
                        WHERE versao = (SELECT MAX(versao) FROM embalagens e2 WHERE e2.codigo = embalagens.codigo)"
                    : @"
                        SELECT
                            COUNT(*) AS total,
                            SUM(CASE WHEN status = 'ATIVO' THEN 1 ELSE 0 END) AS ativos,
                            SUM(CASE WHEN status = 'INATIVO' THEN 1 ELSE 0 END) AS inativos
                        FROM " + tableName + @"
                        WHERE versao = (SELECT MAX(versao) FROM " + tableName + " x2 WHERE x2.codigo = " + tableName + ".codigo)";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new MainSidebarCadastroRow
                        {
                            TableName = label,
                            ActiveCount = ReadInt(reader, "ativos"),
                            InactiveCount = ReadInt(reader, "inativos"),
                            TotalCount = ReadInt(reader, "total"),
                            BrcCount = includeBrc ? (int?)ReadInt(reader, "brc") : null,
                        };
                    }
                }
            }

            return new MainSidebarCadastroRow { TableName = label };
        }

        private static MainSidebarVolumeRow[] LoadVolumeEstoque(DbConnection connection)
        {
            var items = new List<MainSidebarVolumeRow>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        a.codigo || ' - ' || a.nome AS almox,
                        SUM(CASE
                            WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                            WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                            ELSE 0
                        END) AS volume
                    FROM movimentos_estoque m
                    INNER JOIN almoxarifados a ON m.almoxarifado = a.codigo
                        AND a.versao = (SELECT MAX(versao) FROM almoxarifados WHERE codigo = a.codigo)
                    WHERE m.status = 'ATIVO'
                    GROUP BY a.codigo, a.nome
                    HAVING SUM(CASE
                        WHEN m.tipo IN ('ENTRADA', 'TRANSFERENCIA_ENTRADA') THEN m.quantidade
                        WHEN m.tipo IN ('SAIDA', 'REQUISICAO', 'TRANSFERENCIA_SAIDA', 'SAIDA_PRODUCAO') THEN -m.quantidade
                        ELSE 0
                    END) > 0
                    ORDER BY volume DESC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MainSidebarVolumeRow
                        {
                            WarehouseDisplay = ReadString(reader, "almox"),
                            Volume = ReadDecimal(reader, "volume"),
                        });
                    }
                }
            }

            return items.ToArray();
        }

        private static MainSidebarAuditRow[] LoadAuditoria(DbConnection connection)
        {
            return new[]
            {
                new MainSidebarAuditRow { Label = "Estoque negativo", Count = QueryInt(connection, @"
                    WITH movs AS (
                        SELECT
                            m.almoxarifado,
                            m.material,
                            m.lote,
                            m.id,
                            m.data_movimento,
                            SUM(
                                CASE
                                    WHEN m.tipo IN ('ENTRADA','TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                    WHEN m.tipo IN ('SAIDA','REQUISICAO','TRANSFERENCIA_SAIDA','SAIDA_PRODUCAO') THEN -m.quantidade
                                    ELSE 0
                                END
                            ) OVER (
                                PARTITION BY m.almoxarifado, m.material, m.lote
                                ORDER BY m.data_movimento, m.id
                            ) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                    )
                    SELECT COUNT(*) AS total
                    FROM movs
                    WHERE saldo < 0") },
                new MainSidebarAuditRow { Label = "Diverg. lote x mat.", Count = QueryInt(connection, @"
                    WITH saldos AS (
                        SELECT
                            m.almoxarifado,
                            m.material,
                            m.lote,
                            SUM(CASE
                                WHEN m.tipo IN ('ENTRADA','TRANSFERENCIA_ENTRADA') THEN m.quantidade
                                WHEN m.tipo IN ('SAIDA','REQUISICAO','TRANSFERENCIA_SAIDA','SAIDA_PRODUCAO') THEN -m.quantidade
                                ELSE 0
                            END) AS saldo
                        FROM movimentos_estoque m
                        WHERE m.status = 'ATIVO'
                        GROUP BY m.almoxarifado, m.material, m.lote
                        HAVING SUM(CASE
                            WHEN m.tipo IN ('ENTRADA','TRANSFERENCIA_ENTRADA') THEN m.quantidade
                            WHEN m.tipo IN ('SAIDA','REQUISICAO','TRANSFERENCIA_SAIDA','SAIDA_PRODUCAO') THEN -m.quantidade
                            ELSE 0
                        END) > 0
                    )
                    SELECT COUNT(*) AS total
                    FROM saldos s
                    LEFT JOIN lotes l ON l.codigo = s.lote
                        AND l.versao = (SELECT MAX(versao) FROM lotes x WHERE x.codigo = l.codigo)
                        AND l.status = 'ATIVO'
                    WHERE l.material IS NOT NULL
                      AND l.material <> ''
                      AND s.material <> l.material") },
                new MainSidebarAuditRow { Label = "Lote duplicado", Count = QueryInt(connection, @"
                    WITH lotes_ativos AS (
                        SELECT l.codigo, l.nome, l.material
                        FROM lotes l
                        JOIN (
                            SELECT codigo, MAX(versao) AS versao
                            FROM lotes
                            GROUP BY codigo
                        ) lv ON lv.codigo = l.codigo AND lv.versao = l.versao
                        WHERE l.status = 'ATIVO'
                          AND COALESCE(l.material, '') <> ''
                          AND TRIM(COALESCE(l.nome, '')) <> ''
                    ),
                    grupos_duplicados AS (
                        SELECT
                            material,
                            UPPER(TRIM(COALESCE(nome, ''))) AS nome_norm
                        FROM lotes_ativos
                        GROUP BY material, UPPER(TRIM(COALESCE(nome, '')))
                        HAVING COUNT(*) > 1
                    )
                    SELECT COUNT(*) AS total
                    FROM grupos_duplicados") },
            };
        }

        private static int LoadActiveUsersCount(DbConnection connection)
        {
            return QueryInt(connection, @"
                SELECT COUNT(*) AS total
                FROM usuarios u
                WHERE versao = (SELECT MAX(versao) FROM usuarios WHERE usuario = u.usuario)
                  AND status = 'ATIVO'");
        }

        private static MainSidebarUserAccessRow[] LoadRecentAccesses(DbConnection connection)
        {
            var items = new List<MainSidebarUserAccessRow>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT
                        l.usuario,
                        MAX(
                            CASE
                                WHEN l.dt_hr ~ '^[0-9]{2}/[0-9]{2}/[0-9]{4}' THEN to_timestamp(l.dt_hr, 'DD/MM/YYYY HH24:MI:SS')
                                WHEN l.dt_hr ~ '^[0-9]{4}-[0-9]{2}-[0-9]{2}' THEN to_timestamp(l.dt_hr, 'YYYY-MM-DD HH24:MI:SS')
                                ELSE NULL
                            END
                        ) AS ultimo_acesso
                    FROM logs_auditoria l
                    INNER JOIN usuarios u ON l.usuario = u.usuario
                        AND u.versao = (SELECT MAX(versao) FROM usuarios WHERE usuario = u.usuario)
                        AND u.status = 'ATIVO'
                    GROUP BY l.usuario
                    ORDER BY MAX(
                        CASE
                            WHEN l.dt_hr ~ '^[0-9]{2}/[0-9]{2}/[0-9]{4}' THEN to_timestamp(l.dt_hr, 'DD/MM/YYYY HH24:MI:SS')
                            WHEN l.dt_hr ~ '^[0-9]{4}-[0-9]{2}-[0-9]{2}' THEN to_timestamp(l.dt_hr, 'YYYY-MM-DD HH24:MI:SS')
                            ELSE NULL
                        END
                    ) DESC NULLS LAST
                    LIMIT 10";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MainSidebarUserAccessRow
                        {
                            UserName = ReadString(reader, "usuario"),
                            LastAccessText = NormalizeDateTimeText(reader, "ultimo_acesso"),
                        });
                    }
                }
            }

            return items.ToArray();
        }

        private static int QueryInt(DbConnection connection, string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                var result = command.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32(result, CultureInfo.InvariantCulture);
            }
        }

        private static string NormalizeDateTimeText(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
            {
                return "-";
            }

            var value = reader.GetValue(ordinal);
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            }

            DateTime parsed;
            return DateTime.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                : Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private static string NormalizeDateText(DbDataReader reader, string column, bool allowDateTime)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
            {
                return string.Empty;
            }

            var value = reader.GetValue(ordinal);
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            var text = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            DateTime parsed;
            var formats = allowDateTime
                ? new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss" }
                : new[] { "dd/MM/yyyy", "yyyy-MM-dd" };

            return DateTime.TryParseExact(text.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                : text.Trim();
        }

        private static string ReadString(DbDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            return reader.IsDBNull(ordinal) ? string.Empty : Convert.ToString(reader.GetValue(ordinal), CultureInfo.InvariantCulture);
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
