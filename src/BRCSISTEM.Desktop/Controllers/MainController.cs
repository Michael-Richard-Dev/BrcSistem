using System;
using System.Linq;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class MainController
    {
        private readonly ModuleCatalogService _moduleCatalogService;
        private readonly SessionStateService _sessionStateService;
        private readonly MainSidebarService _mainSidebarService;

        public MainController(ModuleCatalogService moduleCatalogService, SessionStateService sessionStateService, MainSidebarService mainSidebarService)
        {
            _moduleCatalogService = moduleCatalogService;
            _sessionStateService = sessionStateService;
            _mainSidebarService = mainSidebarService;
        }

        public ModuleDefinition[] LoadModules(UserIdentity identity)
        {
            return _moduleCatalogService.GetModulesFor(identity);
        }

        public SessionState LoadSession(UserIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }

            return _sessionStateService.Load(identity.UserName);
        }

        public MainSidebarSnapshot LoadSidebarSnapshot(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _mainSidebarService.LoadSnapshot(configuration, profile);
        }

        public void RegisterModuleOpen(UserIdentity identity, ModuleDefinition module)
        {
            if (identity == null || module == null)
            {
                return;
            }

            var session = _sessionStateService.Load(identity.UserName);
            session.UserName = identity.UserName;
            session.SavedAt = DateTime.UtcNow;

            var existing = session.OpenModules.FirstOrDefault(item => string.Equals(item.ModuleKey, module.Key, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                session.OpenModules.Remove(existing);
            }

            session.OpenModules.Insert(0, new OpenModuleState
            {
                ModuleKey = module.Key,
                Title = module.Title,
            });

            while (session.OpenModules.Count > 8)
            {
                session.OpenModules.RemoveAt(session.OpenModules.Count - 1);
            }

            _sessionStateService.Save(session);
        }
    }
}
