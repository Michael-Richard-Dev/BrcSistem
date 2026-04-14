using System;
using System.Data.Common;
using System.Globalization;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlProductionOutputGateway
    {
        private static OutputHeaderRecord LoadLatestOutputHeader(DbConnection connection, DbTransaction transaction, string number)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT numero,
                           COALESCE(finalidade, '') AS finalidade,
                           COALESCE(dt_movimento, '') AS dt_movimento,
                           COALESCE(turno, '') AS turno,
                           COALESCE(status, '') AS status,
                           COALESCE(versao, 0) AS versao,
                           COALESCE(bloqueado_por, '') AS bloqueado_por
                    FROM saidas_producao
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

                    return new OutputHeaderRecord
                    {
                        Number = ReadString(reader, "numero"),
                        Purpose = ReadString(reader, "finalidade"),
                        MovementDateTime = ReadString(reader, "dt_movimento"),
                        Shift = ReadString(reader, "turno"),
                        Status = ReadString(reader, "status"),
                        Version = ReadInt(reader, "versao"),
                        LockedBy = ReadString(reader, "bloqueado_por"),
                    };
                }
            }
        }

        private static string GetLotSupplier(DbConnection connection, DbTransaction transaction, string lotCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COALESCE(fornecedor, '')
                    FROM lotes
                    WHERE codigo = @codigo
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", lotCode));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
            }
        }

        private static string GetLotExpiration(DbConnection connection, DbTransaction transaction, string lotCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COALESCE(validade, '')
                    FROM lotes
                    WHERE codigo = @codigo
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", lotCode));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
            }
        }

        private static void ReleaseLockInternal(DbConnection connection, DbTransaction transaction, string number, string userName, bool updateHeader)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE registro_bloqueios
                       SET ativo = FALSE,
                           data_liberacao = CURRENT_TIMESTAMP,
                           dt_hr_alteracao = CURRENT_TIMESTAMP
                     WHERE tabela = 'saidas_producao'
                       AND registro_chave = @chave
                       AND ativo = TRUE
                       AND (@usuario IS NULL OR UPPER(usuario) = UPPER(@usuario))";
                command.Parameters.Add(CreateParameter(command, "@chave", BuildLockKey(number)));
                command.Parameters.Add(CreateParameter(command, "@usuario", string.IsNullOrWhiteSpace(userName) ? (object)DBNull.Value : userName));
                command.ExecuteNonQuery();
            }

            if (!updateHeader)
            {
                return;
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE saidas_producao
                       SET bloqueado_por = NULL,
                           bloqueado_em = NULL,
                           dt_hr_alteracao = @agora
                     WHERE numero = @numero
                       AND versao = (
                           SELECT MAX(versao)
                           FROM saidas_producao x
                           WHERE x.numero = @numero
                       )";
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.ExecuteNonQuery();
            }
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

        private static string BuildLockKey(string number)
        {
            return "numero=" + number;
        }

        private static void ExecuteNonQuery(DbConnection connection, DbTransaction transaction, string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
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

        private static string NowText()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private sealed class OutputHeaderRecord
        {
            public string Number { get; set; }

            public string Purpose { get; set; }

            public string MovementDateTime { get; set; }

            public string Shift { get; set; }

            public string Status { get; set; }

            public int Version { get; set; }

            public string LockedBy { get; set; }
        }
    }
}
