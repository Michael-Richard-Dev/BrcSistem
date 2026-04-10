using System;
using System.Collections.Generic;

namespace BRCSISTEM.Domain.Models
{
    public sealed class UserTypeDetail
    {
        public UserTypeDetail()
        {
            PermissionKeys = Array.Empty<string>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public IReadOnlyCollection<string> PermissionKeys { get; set; }
    }
}
