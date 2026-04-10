using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Configuration
{
    public sealed class JsonAppConfigurationStore : IAppConfigurationStore
    {
        private readonly string _configPath;
        private readonly JavaScriptSerializer _serializer;

        public JsonAppConfigurationStore(string configPath)
        {
            _configPath = configPath;
            _serializer = new JavaScriptSerializer();
        }

        public AppConfiguration Load()
        {
            var configuration = new AppConfiguration();
            if (!File.Exists(_configPath))
            {
                return configuration;
            }

            var json = File.ReadAllText(_configPath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return configuration;
            }

            var payload = _serializer.DeserializeObject(json) as IDictionary<string, object>;
            if (payload == null)
            {
                return configuration;
            }

            configuration.IsConfigured = GetBool(payload, "configurado") || GetBool(payload, "configured");
            configuration.LegacyConfigured = GetBool(payload, "configured");
            configuration.ConfigDate = GetDate(payload, "config_date");
            configuration.ActiveDatabaseId = GetString(payload, "banco_ativo");
            configuration.ConnectionSettings = ParseConnectionSettings(GetDictionary(payload, "conexao"));
            configuration.FirstUser = ParseFirstUser(GetDictionary(payload, "primeiro_usuario"));
            configuration.AlternateFirstUser = ParseFirstUser(GetDictionary(payload, "first_user"));

            var banks = GetDictionary(payload, "bancos");
            if (banks != null)
            {
                foreach (var pair in banks)
                {
                    var bankData = pair.Value as IDictionary<string, object>;
                    if (bankData == null)
                    {
                        continue;
                    }

                    configuration.DatabaseProfiles[pair.Key] = ParseProfile(pair.Key, bankData);
                }
            }

            configuration.Normalize();
            return configuration;
        }

        public void Save(AppConfiguration configuration)
        {
            configuration.Normalize();
            var directory = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var banks = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var profile in configuration.GetOrderedProfiles())
            {
                banks[profile.Id] = new Dictionary<string, object>
                {
                    ["nome"] = profile.Name,
                    ["descricao"] = profile.Description,
                    ["host"] = profile.Host,
                    ["port"] = profile.Port,
                    ["database"] = profile.Database,
                    ["user"] = profile.User,
                    ["password"] = profile.Password,
                    ["tipo"] = string.IsNullOrWhiteSpace(profile.Kind) ? "rede" : profile.Kind,
                };
            }

            var payload = new Dictionary<string, object>
            {
                ["configurado"] = configuration.IsConfigured,
                ["config_date"] = (configuration.ConfigDate ?? DateTime.UtcNow).ToString("O", CultureInfo.InvariantCulture),
                ["banco_ativo"] = configuration.ActiveDatabaseId,
                ["primeiro_usuario"] = new Dictionary<string, object>
                {
                    ["usuario"] = configuration.GetEffectiveFirstUser().UserName,
                    ["nome"] = configuration.GetEffectiveFirstUser().Name,
                    ["senha"] = configuration.GetEffectiveFirstUser().Password,
                },
                ["conexao"] = new Dictionary<string, object>
                {
                    ["connect_timeout"] = configuration.ConnectionSettings.ConnectTimeoutSeconds,
                    ["retries"] = configuration.ConnectionSettings.Retries,
                    ["retry_wait"] = configuration.ConnectionSettings.RetryWaitSeconds,
                    ["retry_backoff"] = configuration.ConnectionSettings.RetryBackoff,
                    ["max_retry_wait"] = configuration.ConnectionSettings.MaxRetryWaitSeconds,
                    ["reconnect_wait"] = configuration.ConnectionSettings.ReconnectWaitSeconds,
                    ["keepalives"] = configuration.ConnectionSettings.KeepAlives,
                    ["keepalives_idle"] = configuration.ConnectionSettings.KeepAlivesIdle,
                    ["keepalives_interval"] = configuration.ConnectionSettings.KeepAlivesInterval,
                    ["keepalives_count"] = configuration.ConnectionSettings.KeepAlivesCount,
                    ["application_name"] = configuration.ConnectionSettings.ApplicationName,
                },
                ["bancos"] = banks,
            };

            File.WriteAllText(_configPath, _serializer.Serialize(payload));
        }

        private static DatabaseProfile ParseProfile(string key, IDictionary<string, object> payload)
        {
            return new DatabaseProfile
            {
                Id = key,
                Name = GetString(payload, "nome"),
                Description = GetString(payload, "descricao"),
                Host = GetString(payload, "host"),
                Port = GetInt(payload, "port", 5432),
                Database = GetString(payload, "database"),
                User = GetString(payload, "user"),
                Password = GetString(payload, "password"),
                Kind = GetString(payload, "tipo"),
            };
        }

        private static FirstUserSeed ParseFirstUser(IDictionary<string, object> payload)
        {
            if (payload == null)
            {
                return new FirstUserSeed();
            }

            return new FirstUserSeed
            {
                UserName = GetString(payload, "usuario"),
                Name = GetString(payload, "nome"),
                Password = GetString(payload, "senha"),
            };
        }

        private static ConnectionResilienceSettings ParseConnectionSettings(IDictionary<string, object> payload)
        {
            if (payload == null)
            {
                return ConnectionResilienceSettings.CreateDefault();
            }

            return new ConnectionResilienceSettings
            {
                ConnectTimeoutSeconds = GetInt(payload, "connect_timeout", 5),
                Retries = GetInt(payload, "retries", 3),
                RetryWaitSeconds = GetDouble(payload, "retry_wait", 1.0),
                RetryBackoff = GetDouble(payload, "retry_backoff", 1.5),
                MaxRetryWaitSeconds = GetDouble(payload, "max_retry_wait", 5.0),
                ReconnectWaitSeconds = GetDouble(payload, "reconnect_wait", 2.0),
                KeepAlives = GetInt(payload, "keepalives", 1),
                KeepAlivesIdle = GetInt(payload, "keepalives_idle", 30),
                KeepAlivesInterval = GetInt(payload, "keepalives_interval", 10),
                KeepAlivesCount = GetInt(payload, "keepalives_count", 5),
                ApplicationName = GetString(payload, "application_name") ?? "BRCSISTEM",
            };
        }

        private static IDictionary<string, object> GetDictionary(IDictionary<string, object> payload, string key)
        {
            if (payload == null || !payload.ContainsKey(key))
            {
                return null;
            }

            return payload[key] as IDictionary<string, object>;
        }

        private static string GetString(IDictionary<string, object> payload, string key)
        {
            if (payload == null || !payload.ContainsKey(key) || payload[key] == null)
            {
                return null;
            }

            return Convert.ToString(payload[key], CultureInfo.InvariantCulture);
        }

        private static bool GetBool(IDictionary<string, object> payload, string key)
        {
            if (payload == null || !payload.ContainsKey(key) || payload[key] == null)
            {
                return false;
            }

            var raw = Convert.ToString(payload[key], CultureInfo.InvariantCulture);
            return string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(raw, "1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(raw, "sim", StringComparison.OrdinalIgnoreCase);
        }

        private static int GetInt(IDictionary<string, object> payload, string key, int fallback)
        {
            if (payload == null || !payload.ContainsKey(key) || payload[key] == null)
            {
                return fallback;
            }

            if (int.TryParse(Convert.ToString(payload[key], CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        private static double GetDouble(IDictionary<string, object> payload, string key, double fallback)
        {
            if (payload == null || !payload.ContainsKey(key) || payload[key] == null)
            {
                return fallback;
            }

            if (double.TryParse(Convert.ToString(payload[key], CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return fallback;
        }

        private static DateTime? GetDate(IDictionary<string, object> payload, string key)
        {
            var value = GetString(payload, key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
