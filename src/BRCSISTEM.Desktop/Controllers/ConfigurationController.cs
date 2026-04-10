using System;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class ConfigurationController
    {
        private readonly AppBootstrapService _appBootstrapService;

        public ConfigurationController(AppBootstrapService appBootstrapService)
        {
            _appBootstrapService = appBootstrapService;
        }

        public AppConfiguration LoadConfiguration()
        {
            return _appBootstrapService.LoadConfiguration();
        }

        public void SaveConfiguration(AppConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _appBootstrapService.SaveConfiguration(configuration);
        }

        public ConnectionTestResult TestConnection(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _appBootstrapService.TestConnection(configuration, profile);
        }
    }
}
