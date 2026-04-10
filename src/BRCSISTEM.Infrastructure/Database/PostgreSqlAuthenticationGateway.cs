using System;
using System.Data.Common;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlAuthenticationGateway : IAuthenticationGateway
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlAuthenticationGateway(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public UserAccount FindUser(DatabaseProfile profile, string userName, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT usuario, COALESCE(nome, usuario) AS nome, senha, salt, tipo, status
                    FROM usuarios
                    WHERE lower(usuario) = lower(@usuario)
                    ORDER BY versao DESC
                    LIMIT 1";
                command.Parameters.Add(CreateParameter(command, "@usuario", userName));

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new UserAccount
                    {
                        UserName = ReadString(reader, "usuario"),
                        DisplayName = ReadString(reader, "nome"),
                        PasswordHash = ReadString(reader, "senha"),
                        Salt = ReadString(reader, "salt"),
                        UserType = ReadString(reader, "tipo"),
                        Status = ReadString(reader, "status"),
                    };
                }
            }
        }

        public System.Collections.Generic.IReadOnlyCollection<string> LoadPermissionKeys(DatabaseProfile profile, string userType, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT permissoes FROM tipos_usuario WHERE tipo = @tipo";
                command.Parameters.Add(CreateParameter(command, "@tipo", userType));
                var raw = command.ExecuteScalar();
                var value = raw == null || raw == DBNull.Value ? string.Empty : Convert.ToString(raw);

                if (string.IsNullOrWhiteSpace(value))
                {
                    return Array.Empty<string>();
                }

                return value
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(permission => permission.Trim())
                    .Where(permission => permission.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
        }

        public void UpdatePassword(DatabaseProfile profile, string userName, string passwordHash, string salt, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE usuarios
                    SET senha = @senha,
                        salt = @salt,
                        dt_hr_alteracao = @agora
                    WHERE lower(usuario) = lower(@usuario)
                      AND versao = (
                          SELECT MAX(versao)
                          FROM usuarios AS ultima
                          WHERE lower(ultima.usuario) = lower(@usuario)
                      )";
                command.Parameters.Add(CreateParameter(command, "@senha", passwordHash));
                command.Parameters.Add(CreateParameter(command, "@salt", salt));
                command.Parameters.Add(CreateParameter(command, "@agora", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                command.Parameters.Add(CreateParameter(command, "@usuario", userName));
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
    }
}
