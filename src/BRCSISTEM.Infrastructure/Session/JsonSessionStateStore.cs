using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Infrastructure.Session
{
    public sealed class JsonSessionStateStore : ISessionStateStore
    {
        private readonly string _sessionFilePath;
        private readonly JavaScriptSerializer _serializer;

        public JsonSessionStateStore(string sessionFilePath)
        {
            _sessionFilePath = sessionFilePath;
            _serializer = new JavaScriptSerializer();
        }

        public SessionState Load(string userName)
        {
            if (!File.Exists(_sessionFilePath))
            {
                return new SessionState { UserName = userName };
            }

            var payload = _serializer.DeserializeObject(File.ReadAllText(_sessionFilePath)) as IDictionary<string, object>;
            if (payload == null)
            {
                return new SessionState { UserName = userName };
            }

            var storedUser = payload.ContainsKey("usuario") ? Convert.ToString(payload["usuario"]) : null;
            if (!string.Equals(storedUser, userName, StringComparison.OrdinalIgnoreCase))
            {
                return new SessionState { UserName = userName };
            }

            var state = new SessionState
            {
                UserName = userName,
                SavedAt = ParseDate(payload.ContainsKey("timestamp") ? Convert.ToString(payload["timestamp"]) : null),
            };

            if (payload.ContainsKey("janelas_abertas") && payload["janelas_abertas"] is object[] openModules)
            {
                foreach (var module in openModules)
                {
                    var data = module as IDictionary<string, object>;
                    if (data == null)
                    {
                        continue;
                    }

                    state.OpenModules.Add(new OpenModuleState
                    {
                        ModuleKey = data.ContainsKey("tipo") ? Convert.ToString(data["tipo"]) : null,
                        Title = data.ContainsKey("titulo") ? Convert.ToString(data["titulo"]) : null,
                    });
                }
            }

            return state;
        }

        public void Save(SessionState state)
        {
            var directory = Path.GetDirectoryName(_sessionFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var modules = new List<object>();
            foreach (var module in state.OpenModules)
            {
                modules.Add(new Dictionary<string, object>
                {
                    ["tipo"] = module.ModuleKey,
                    ["titulo"] = module.Title,
                });
            }

            var payload = new Dictionary<string, object>
            {
                ["usuario"] = state.UserName,
                ["timestamp"] = state.SavedAt.ToString("O", CultureInfo.InvariantCulture),
                ["janelas_abertas"] = modules.ToArray(),
            };

            File.WriteAllText(_sessionFilePath, _serializer.Serialize(payload));
        }

        public void Clear(string userName)
        {
            if (!File.Exists(_sessionFilePath))
            {
                return;
            }

            var currentState = Load(userName);
            if (string.Equals(currentState.UserName, userName, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(_sessionFilePath);
            }
        }

        private static DateTime ParseDate(string value)
        {
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed))
            {
                return parsed;
            }

            return DateTime.UtcNow;
        }
    }
}
