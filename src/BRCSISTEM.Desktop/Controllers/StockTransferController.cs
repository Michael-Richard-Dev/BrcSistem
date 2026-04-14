using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class StockTransferController
    {
        private readonly StockTransferService _stockTransferService;

        public StockTransferController(StockTransferService stockTransferService)
        {
            _stockTransferService = stockTransferService;
        }

        public string GenerateNextTransferNumber(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockTransferService.GenerateNextTransferNumber(configuration, profile);
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _stockTransferService.LoadWarehousesForUser(configuration, profile, userName);
        }

        public PackagingSummary[] LoadMaterialsByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime)
        {
            return _stockTransferService.LoadMaterialsByWarehouse(configuration, profile, warehouseCode, movementDateTime);
        }

        public LotSummary[] LoadLotsByWarehouseAndMaterial(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string materialCode, string movementDateTime)
        {
            return _stockTransferService.LoadLotsByWarehouseAndMaterial(configuration, profile, warehouseCode, materialCode, movementDateTime);
        }

        public decimal GetAvailableStockBalance(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedTransferNumber)
        {
            return _stockTransferService.GetAvailableStockBalance(
                configuration,
                profile,
                materialCode,
                lotCode,
                warehouseCode,
                movementDateTime,
                excludedTransferNumber);
        }

        public StockTransferSummary[] SearchTransfers(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            return _stockTransferService.SearchTransfers(configuration, profile, filter);
        }

        public StockTransferDetail LoadTransfer(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _stockTransferService.LoadTransfer(configuration, profile, number);
        }

        public RecordLockResult TryLockTransfer(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            return _stockTransferService.TryLockTransfer(configuration, profile, number, userName);
        }

        public void ReleaseTransferLock(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            _stockTransferService.ReleaseTransferLock(configuration, profile, number, userName);
        }

        public void CreateTransfer(AppConfiguration configuration, DatabaseProfile profile, SaveStockTransferRequest request)
        {
            _stockTransferService.CreateTransfer(configuration, profile, request);
        }

        public void UpdateTransfer(AppConfiguration configuration, DatabaseProfile profile, SaveStockTransferRequest request)
        {
            _stockTransferService.UpdateTransfer(configuration, profile, request);
        }

        public void CancelTransfer(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            _stockTransferService.CancelTransfer(configuration, profile, number, userName);
        }
    }
}
