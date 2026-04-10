using System;
using System.Data.Common;
using System.Threading;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Database
{
    public sealed class PostgreSqlConnectionFactory : IDatabaseConnectionFactory
    {
        public DbConnection Open(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            settings = settings ?? ConnectionResilienceSettings.CreateDefault();
            var factory = PostgreSqlProviderLoader.LoadFactory();
            var connectionString = BuildConnectionString(profile, settings);
            var attempts = Math.Max(1, settings.Retries);
            Exception lastError = null;

            for (var attempt = 1; attempt <= attempts; attempt++)
            {
                try
                {
                    var connection = factory.CreateConnection();
                    if (connection == null)
                    {
                        throw new InvalidOperationException("O provider Npgsql nao conseguiu criar uma conexao.");
                    }

                    connection.ConnectionString = connectionString;
                    connection.Open();
                    return connection;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    if (attempt == attempts)
                    {
                        break;
                    }

                    var wait = Math.Min(settings.MaxRetryWaitSeconds, settings.RetryWaitSeconds * Math.Pow(settings.RetryBackoff, attempt - 1));
                    Thread.Sleep(TimeSpan.FromSeconds(wait));
                }
            }

            throw new InvalidOperationException("Falha ao conectar no PostgreSQL. " + ExtractDetailedMessage(lastError), lastError);
        }

        public ConnectionTestResult TestConnection(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            try
            {
                using (var connection = Open(profile, settings))
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT 1";
                    command.ExecuteScalar();
                    return ConnectionTestResult.Ok("Conexao com PostgreSQL validada com sucesso.");
                }
            }
            catch (Exception ex)
            {
                return ConnectionTestResult.Fail("Falha ao testar conexao: " + ExtractDetailedMessage(ex));
            }
        }

        private static string BuildConnectionString(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            return string.Join(";",
                "Host=" + profile.Host,
                "Port=" + profile.Port,
                "Database=" + profile.Database,
                "Username=" + profile.User,
                "Password=" + profile.Password,
                "Timeout=" + settings.ConnectTimeoutSeconds,
                "Command Timeout=" + settings.ConnectTimeoutSeconds,
                "Keepalive=" + settings.KeepAlivesIdle,
                "Application Name=" + (string.IsNullOrWhiteSpace(settings.ApplicationName) ? "BRCSISTEM" : settings.ApplicationName),
                "Client Encoding=UTF8",
                "Pooling=true");
        }

        private static string ExtractDetailedMessage(Exception exception)
        {
            if (exception == null)
            {
                return "Erro nao identificado.";
            }

            var current = exception;
            while (current.InnerException != null)
            {
                current = current.InnerException;
            }

            return string.IsNullOrWhiteSpace(current.Message)
                ? exception.Message
                : current.Message;
        }
    }
}
