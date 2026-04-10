using System;
using System.Collections.Generic;
using System.Linq;

namespace BRCSISTEM.Domain.Models
{
    public sealed class AppConfiguration
    {
        public AppConfiguration()
        {
            DatabaseProfiles = new Dictionary<string, DatabaseProfile>(StringComparer.OrdinalIgnoreCase);
            ConnectionSettings = ConnectionResilienceSettings.CreateDefault();
            FirstUser = new FirstUserSeed();
            AlternateFirstUser = new FirstUserSeed();
        }

        public bool IsConfigured { get; set; }

        public bool LegacyConfigured { get; set; }

        public DateTime? ConfigDate { get; set; }

        public string ActiveDatabaseId { get; set; }

        public FirstUserSeed FirstUser { get; set; }

        public FirstUserSeed AlternateFirstUser { get; set; }

        public ConnectionResilienceSettings ConnectionSettings { get; set; }

        public Dictionary<string, DatabaseProfile> DatabaseProfiles { get; private set; }

        public void Normalize()
        {
            if (DatabaseProfiles == null)
            {
                DatabaseProfiles = new Dictionary<string, DatabaseProfile>(StringComparer.OrdinalIgnoreCase);
            }

            var normalized = new Dictionary<string, DatabaseProfile>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in DatabaseProfiles.ToArray())
            {
                var profile = pair.Value ?? new DatabaseProfile();
                profile.Id = string.IsNullOrWhiteSpace(profile.Id) ? pair.Key : profile.Id;
                if (string.IsNullOrWhiteSpace(profile.Id))
                {
                    profile.Id = BuildProfileId(profile.Name);
                }

                profile.Name = string.IsNullOrWhiteSpace(profile.Name) ? profile.Id : profile.Name;
                profile.Port = profile.Port <= 0 ? 5432 : profile.Port;
                profile.Kind = string.IsNullOrWhiteSpace(profile.Kind) ? "rede" : profile.Kind;
                normalized[profile.Id] = profile;
            }

            DatabaseProfiles = normalized;

            if (!string.IsNullOrWhiteSpace(ActiveDatabaseId) && !DatabaseProfiles.ContainsKey(ActiveDatabaseId))
            {
                ActiveDatabaseId = null;
            }

            if (string.IsNullOrWhiteSpace(ActiveDatabaseId))
            {
                ActiveDatabaseId = DatabaseProfiles.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
            }

            ConnectionSettings = ConnectionSettings ?? ConnectionResilienceSettings.CreateDefault();
            FirstUser = FirstUser ?? new FirstUserSeed();
            AlternateFirstUser = AlternateFirstUser ?? new FirstUserSeed();
            IsConfigured = IsConfigured || LegacyConfigured || DatabaseProfiles.Count > 0;
        }

        public FirstUserSeed GetEffectiveFirstUser()
        {
            if (FirstUser != null && FirstUser.HasValues)
            {
                return FirstUser;
            }

            return AlternateFirstUser ?? new FirstUserSeed();
        }

        public IReadOnlyCollection<DatabaseProfile> GetOrderedProfiles()
        {
            Normalize();
            return DatabaseProfiles.Values
                .OrderByDescending(profile => string.Equals(profile.Id, ActiveDatabaseId, StringComparison.OrdinalIgnoreCase))
                .ThenBy(profile => profile.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public DatabaseProfile GetActiveProfile()
        {
            Normalize();
            if (string.IsNullOrWhiteSpace(ActiveDatabaseId))
            {
                return null;
            }

            DatabaseProfiles.TryGetValue(ActiveDatabaseId, out var profile);
            return profile;
        }

        public DatabaseProfile GetProfile(string profileId)
        {
            Normalize();
            if (string.IsNullOrWhiteSpace(profileId))
            {
                return null;
            }

            DatabaseProfiles.TryGetValue(profileId, out var profile);
            return profile;
        }

        public string EnsureProfileId(DatabaseProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            profile.Id = string.IsNullOrWhiteSpace(profile.Id) ? BuildProfileId(profile.Name) : profile.Id.Trim();
            if (string.IsNullOrWhiteSpace(profile.Id))
            {
                profile.Id = "brc_" + DateTime.UtcNow.Ticks;
            }

            return profile.Id;
        }

        private static string BuildProfileId(string name)
        {
            var raw = string.IsNullOrWhiteSpace(name) ? "brc" : name.Trim().ToLowerInvariant();
            var chars = raw.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray();
            var normalized = new string(chars).Trim('_');
            return string.IsNullOrWhiteSpace(normalized) ? "brc" : normalized;
        }
    }
}
