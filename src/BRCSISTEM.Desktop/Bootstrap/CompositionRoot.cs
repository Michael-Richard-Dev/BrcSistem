using System;
using System.IO;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Infrastructure.Configuration;
using BRCSISTEM.Infrastructure.Database;
using BRCSISTEM.Infrastructure.Session;

namespace BRCSISTEM.Desktop.Bootstrap
{
    public sealed class CompositionRoot
    {
        private readonly AppBootstrapService _appBootstrapService;
        private readonly AuthenticationService _authenticationService;
        private readonly AdministrationService _administrationService;
        private readonly MasterDataService _masterDataService;
        private readonly InboundReceiptService _inboundReceiptService;
        private readonly ProductionOutputService _productionOutputService;
        private readonly StockTransferService _stockTransferService;
        private readonly MaterialRequisitionService _materialRequisitionService;
        private readonly InventoryService _inventoryService;
        private readonly ModuleCatalogService _moduleCatalogService;
        private readonly SessionStateService _sessionStateService;

        private CompositionRoot(
            AppBootstrapService appBootstrapService,
            AuthenticationService authenticationService,
            AdministrationService administrationService,
            MasterDataService masterDataService,
            InboundReceiptService inboundReceiptService,
            ProductionOutputService productionOutputService,
            StockTransferService stockTransferService,
            MaterialRequisitionService materialRequisitionService,
            InventoryService inventoryService,
            ModuleCatalogService moduleCatalogService,
            SessionStateService sessionStateService)
        {
            _appBootstrapService = appBootstrapService;
            _authenticationService = authenticationService;
            _administrationService = administrationService;
            _masterDataService = masterDataService;
            _inboundReceiptService = inboundReceiptService;
            _productionOutputService = productionOutputService;
            _stockTransferService = stockTransferService;
            _materialRequisitionService = materialRequisitionService;
            _inventoryService = inventoryService;
            _moduleCatalogService = moduleCatalogService;
            _sessionStateService = sessionStateService;
        }

        public static CompositionRoot Create()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var configDirectory = Path.Combine(baseDirectory, "config");
            var configurationStore = new JsonAppConfigurationStore(Path.Combine(configDirectory, "config_db.json"));
            var connectionFactory = new PostgreSqlConnectionFactory();
            var bootstrapper = new PostgreSqlBootstrapper(connectionFactory);
            var authenticationGateway = new PostgreSqlAuthenticationGateway(connectionFactory);
            var administrationGateway = new PostgreSqlAdministrationGateway(connectionFactory);
            var masterDataGateway = new PostgreSqlMasterDataGateway(connectionFactory);
            var inboundReceiptGateway = new PostgreSqlInboundReceiptGateway(connectionFactory);
            var productionOutputGateway = new PostgreSqlProductionOutputGateway(connectionFactory);
            var stockTransferGateway = new PostgreSqlStockTransferGateway(connectionFactory);
            var materialRequisitionGateway = new PostgreSqlMaterialRequisitionGateway(connectionFactory);
            var inventoryGateway = new PostgreSqlInventoryGateway(connectionFactory);
            var auditTrailService = new PostgreSqlAuditTrailService(connectionFactory);
            var sessionStore = new JsonSessionStateStore(Path.Combine(configDirectory, "session_state.json"));

            return new CompositionRoot(
                new AppBootstrapService(configurationStore, connectionFactory, bootstrapper),
                new AuthenticationService(bootstrapper, authenticationGateway, auditTrailService),
                new AdministrationService(administrationGateway, auditTrailService),
                new MasterDataService(masterDataGateway, auditTrailService),
                new InboundReceiptService(masterDataGateway, inboundReceiptGateway, auditTrailService),
                new ProductionOutputService(masterDataGateway, productionOutputGateway, auditTrailService),
                new StockTransferService(stockTransferGateway, auditTrailService),
                new MaterialRequisitionService(materialRequisitionGateway, auditTrailService),
                new InventoryService(inventoryGateway, auditTrailService),
                new ModuleCatalogService(),
                new SessionStateService(sessionStore));
        }

        public ConfigurationController CreateConfigurationController()
        {
            return new ConfigurationController(_appBootstrapService);
        }

        public AuthenticationController CreateAuthenticationController()
        {
            return new AuthenticationController(_authenticationService);
        }

        public MainController CreateMainController()
        {
            return new MainController(_moduleCatalogService, _sessionStateService);
        }

        public AdministrationController CreateAdministrationController()
        {
            return new AdministrationController(_administrationService);
        }

        public MasterDataController CreateMasterDataController()
        {
            return new MasterDataController(_masterDataService);
        }

        public InboundReceiptController CreateInboundReceiptController()
        {
            return new InboundReceiptController(_inboundReceiptService);
        }

        public ProductionOutputController CreateProductionOutputController()
        {
            return new ProductionOutputController(_productionOutputService);
        }

        public StockTransferController CreateStockTransferController()
        {
            return new StockTransferController(_stockTransferService);
        }

        public MaterialRequisitionController CreateMaterialRequisitionController()
        {
            return new MaterialRequisitionController(_materialRequisitionService);
        }

        public InventoryController CreateInventoryController()
        {
            return new InventoryController(_inventoryService);
        }
    }
}
