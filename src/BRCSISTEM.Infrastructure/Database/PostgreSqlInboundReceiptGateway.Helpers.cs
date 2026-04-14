using System;
using System.Data.Common;
using System.Globalization;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed partial class PostgreSqlInboundReceiptGateway
    {
        private static NoteHeaderRecord LoadLatestNoteHeader(DbConnection connection, DbTransaction transaction, string number, string supplierCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT n.numero,
                           n.fornecedor,
                           COALESCE(f.nome, '') AS fornecedor_nome,
                           COALESCE(n.almoxarifado, '') AS almoxarifado,
                           COALESCE(a.nome, '') AS almoxarifado_nome,
                           COALESCE(n.dt_emissao, '') AS dt_emissao,
                           COALESCE(n.dt_movimento, '') AS dt_movimento,
                           COALESCE(n.status, '') AS status,
                           COALESCE(n.versao, 0) AS versao,
                           COALESCE(n.bloqueado_por, '') AS bloqueado_por
                    FROM notas n
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
                    WHERE n.numero = @numero
                      AND n.fornecedor = @fornecedor
                    ORDER BY n.versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new NoteHeaderRecord
                    {
                        Number = ReadString(reader, "numero"),
                        SupplierCode = ReadString(reader, "fornecedor"),
                        SupplierName = ReadString(reader, "fornecedor_nome"),
                        WarehouseCode = ReadString(reader, "almoxarifado"),
                        WarehouseName = ReadString(reader, "almoxarifado_nome"),
                        EmissionDate = ReadString(reader, "dt_emissao"),
                        MovementDateTime = ReadString(reader, "dt_movimento"),
                        Status = ReadString(reader, "status"),
                        Version = ReadInt(reader, "versao"),
                        LockedBy = ReadString(reader, "bloqueado_por"),
                    };
                }
            }
        }

        private static int GetNextReceiptVersion(DbConnection connection, DbTransaction transaction, string number, string supplierCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COALESCE(MAX(versao), 0) + 1
                    FROM notas
                    WHERE numero = @numero
                      AND fornecedor = @fornecedor";
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));
                return Convert.ToInt32(command.ExecuteScalar() ?? 1);
            }
        }

        private static string GetLotExpiration(DbConnection connection, DbTransaction transaction, string lotCode, string supplierCode)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    SELECT COALESCE(validade, '')
                    FROM lotes
                    WHERE codigo = @codigo
                      AND fornecedor = @fornecedor
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@codigo", lotCode));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? string.Empty : Convert.ToString(result);
            }
        }

        private static void ReleaseLockInternal(DbConnection connection, DbTransaction transaction, string number, string supplierCode, string userName, bool updateHeader)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE registro_bloqueios
                       SET ativo = FALSE,
                           data_liberacao = CURRENT_TIMESTAMP,
                           dt_hr_alteracao = CURRENT_TIMESTAMP
                     WHERE tabela = 'notas'
                       AND registro_chave = @chave
                       AND ativo = TRUE
                       AND (@usuario IS NULL OR UPPER(usuario) = UPPER(@usuario))";
                command.Parameters.Add(CreateParameter(command, "@chave", BuildLockKey(number, supplierCode)));
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
                    UPDATE notas
                       SET bloqueado_por = NULL,
                           bloqueado_em = NULL,
                           dt_hr_alteracao = @agora
                     WHERE numero = @numero
                       AND fornecedor = @fornecedor
                       AND versao = (
                           SELECT MAX(versao)
                           FROM notas x
                           WHERE x.numero = @numero
                             AND x.fornecedor = @fornecedor
                       )";
                command.Parameters.Add(CreateParameter(command, "@agora", NowText()));
                command.Parameters.Add(CreateParameter(command, "@numero", number));
                command.Parameters.Add(CreateParameter(command, "@fornecedor", supplierCode));
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

        private static string BuildLockKey(string number, string supplierCode)
        {
            return "numero=" + number + "|fornecedor=" + supplierCode;
        }

        private static string DigitsOnly(string value)
        {
            var source = value ?? string.Empty;
            var chars = new char[source.Length];
            var position = 0;
            foreach (var character in source)
            {
                if (!char.IsDigit(character))
                {
                    continue;
                }

                chars[position++] = character;
            }

            return new string(chars, 0, position);
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

        private sealed class NoteHeaderRecord
        {
            public string Number { get; set; }

            public string SupplierCode { get; set; }

            public string SupplierName { get; set; }

            public string WarehouseCode { get; set; }

            public string WarehouseName { get; set; }

            public string EmissionDate { get; set; }

            public string MovementDateTime { get; set; }

            public string Status { get; set; }

            public int Version { get; set; }

            public string LockedBy { get; set; }
        }
    }
}
