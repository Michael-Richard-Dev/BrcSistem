using System;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class MainSidebarService
    {
        private readonly IMainSidebarGateway _mainSidebarGateway;

        public MainSidebarService(IMainSidebarGateway mainSidebarGateway)
        {
            _mainSidebarGateway = mainSidebarGateway;
        }

        public MainSidebarSnapshot LoadSnapshot(AppConfiguration configuration, DatabaseProfile profile)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return _mainSidebarGateway.LoadSnapshot(
                profile,
                configuration.ConnectionSettings ?? ConnectionResilienceSettings.CreateDefault());
        }
    }
}
