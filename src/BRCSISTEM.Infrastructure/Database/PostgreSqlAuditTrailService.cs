using System;
using System.Data.Common;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlAuditTrailService : IAuditTrailService
    {
        private readonly IDatabaseConnectionFactory _connectionFactory;

        public PostgreSqlAuditTrailService(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Write(DatabaseProfile profile, string userName, string action, string details, ConnectionResilienceSettings settings)
        {
            using (var connection = _connectionFactory.Open(profile, settings))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO logs_auditoria (usuario, acao, detalhes, dt_hr)
                    VALUES (@usuario, @acao, @detalhes, @agora)";
                command.Parameters.Add(CreateParameter(command, "@usuario", userName));
                command.Parameters.Add(CreateParameter(command, "@acao", action));
                command.Parameters.Add(CreateParameter(command, "@detalhes", details));
                command.Parameters.Add(CreateParameter(command, "@agora", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
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
    }
}
