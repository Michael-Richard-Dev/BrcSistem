using System;
using System.Linq;
using BRCSISTEM.Domain.Catalog;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class ModuleCatalogService
    {
        private readonly ModuleDefinition[] _modules = LegacyModuleCatalog.Create();

        public ModuleDefinition[] GetModulesFor(UserIdentity identity)
        {
            if (identity == null || identity.IsAdministrator)
            {
                return _modules
                    .OrderBy(module => module.Group, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(module => module.Title, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }

            return _modules
                .Where(module => string.IsNullOrWhiteSpace(module.RequiredPermission)
                    || identity.PermissionKeys.Contains(module.RequiredPermission, StringComparer.OrdinalIgnoreCase)
                    || identity.PermissionKeys.Contains("*", StringComparer.OrdinalIgnoreCase))
                .OrderBy(module => module.Group, StringComparer.OrdinalIgnoreCase)
                .ThenBy(module => module.Title, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
