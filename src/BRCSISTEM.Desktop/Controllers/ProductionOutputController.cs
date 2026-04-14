using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class ProductionOutputController
    {
        private readonly ProductionOutputService _productionOutputService;

        public ProductionOutputController(ProductionOutputService productionOutputService)
        {
            _productionOutputService = productionOutputService;
        }

        public string GenerateNextOutputNumber(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _productionOutputService.GenerateNextOutputNumber(configuration, profile);
        }

        public ProductSummary[] LoadProducts(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _productionOutputService.LoadProducts(configuration, profile);
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _productionOutputService.LoadWarehousesForUser(configuration, profile, userName);
        }

        public PackagingSummary[] LoadMaterialsByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime)
        {
            return _productionOutputService.LoadMaterialsByWarehouse(configuration, profile, warehouseCode, movementDateTime);
        }

        public LotSummary[] LoadLotsByWarehouseAndMaterial(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string materialCode, string movementDateTime)
        {
            return _productionOutputService.LoadLotsByWarehouseAndMaterial(configuration, profile, warehouseCode, materialCode, movementDateTime);
        }

        public decimal GetAvailableStockBalance(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedOutputNumber)
        {
            return _productionOutputService.GetAvailableStockBalance(
                configuration,
                profile,
                materialCode,
                lotCode,
                warehouseCode,
                movementDateTime,
                excludedOutputNumber);
        }

        public ProductionOutputSummary[] SearchOutputs(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            return _productionOutputService.SearchOutputs(configuration, profile, filter);
        }

        public ProductionOutputDetail LoadOutput(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _productionOutputService.LoadOutput(configuration, profile, number);
        }

        public RecordLockResult TryLockOutput(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            return _productionOutputService.TryLockOutput(configuration, profile, number, userName);
        }

        public void ReleaseOutputLock(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            _productionOutputService.ReleaseOutputLock(configuration, profile, number, userName);
        }

        public void CreateOutput(AppConfiguration configuration, DatabaseProfile profile, SaveProductionOutputRequest request)
        {
            _productionOutputService.CreateOutput(configuration, profile, request);
        }

        public void UpdateOutput(AppConfiguration configuration, DatabaseProfile profile, SaveProductionOutputRequest request)
        {
            _productionOutputService.UpdateOutput(configuration, profile, request);
        }

        public void CancelOutput(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            _productionOutputService.CancelOutput(configuration, profile, number, userName);
        }
    }
}
