using System;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class AppBootstrapService
    {
        private readonly IAppConfigurationStore _configurationStore;
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly IDatabaseBootstrapper _databaseBootstrapper;

        public AppBootstrapService(
            IAppConfigurationStore configurationStore,
            IDatabaseConnectionFactory connectionFactory,
            IDatabaseBootstrapper databaseBootstrapper)
        {
            _configurationStore = configurationStore;
            _connectionFactory = connectionFactory;
            _databaseBootstrapper = databaseBootstrapper;
        }

        public AppConfiguration LoadConfiguration()
        {
            var configuration = _configurationStore.Load();
            configuration.Normalize();
            return configuration;
        }

        public void SaveConfiguration(AppConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Normalize();
            configuration.IsConfigured = configuration.DatabaseProfiles.Count > 0;
            if (!configuration.ConfigDate.HasValue)
            {
                configuration.ConfigDate = DateTime.UtcNow;
            }

            _configurationStore.Save(configuration);
        }

        public ConnectionTestResult TestConnection(AppConfiguration configuration, DatabaseProfile profile)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (profile == null)
            {
                return ConnectionTestResult.Fail("Selecione um banco antes de testar.");
            }

            return _connectionFactory.TestConnection(profile, configuration.ConnectionSettings);
        }

        public DatabaseProfile EnsureActiveProfileReady(AppConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Normalize();
            var profile = configuration.GetActiveProfile();
            if (profile == null)
            {
                throw new InvalidOperationException("Nenhum banco ativo foi configurado.");
            }

            _databaseBootstrapper.EnsureCoreSchema(profile, configuration.GetEffectiveFirstUser(), configuration.ConnectionSettings);
            return profile;
        }

        public DatabaseProfile EnsureProfileReady(AppConfiguration configuration, string profileId)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration.Normalize();
            var profile = configuration.GetProfile(profileId);
            if (profile == null)
            {
                throw new InvalidOperationException("Banco de dados selecionado nao foi encontrado.");
            }

            _databaseBootstrapper.EnsureCoreSchema(profile, configuration.GetEffectiveFirstUser(), configuration.ConnectionSettings);
            return profile;
        }
    }
}
