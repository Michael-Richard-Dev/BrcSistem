using System;
using System.Collections.Generic;
using System.Linq;

namespace BRCSISTEM.Domain.Models
{
    public sealed class UserIdentity
    {
        public UserIdentity()
        {
            PermissionKeys = Array.Empty<string>();
        }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string UserType { get; set; }

        public IReadOnlyCollection<string> PermissionKeys { get; set; }

        public bool IsAdministrator
        {
            get
            {
                return string.Equals(UserType, "Administrador", StringComparison.OrdinalIgnoreCase)
                    || PermissionKeys.Contains("*", StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
