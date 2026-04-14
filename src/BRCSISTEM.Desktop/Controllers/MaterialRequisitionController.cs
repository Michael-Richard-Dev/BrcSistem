using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class MaterialRequisitionController
    {
        private readonly MaterialRequisitionService _materialRequisitionService;

        public MaterialRequisitionController(MaterialRequisitionService materialRequisitionService)
        {
            _materialRequisitionService = materialRequisitionService;
        }

        public string GenerateNextRequisitionNumber(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _materialRequisitionService.GenerateNextRequisitionNumber(configuration, profile);
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _materialRequisitionService.LoadWarehousesForUser(configuration, profile, userName);
        }

        public PackagingSummary[] LoadMaterialsByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime)
        {
            return _materialRequisitionService.LoadMaterialsByWarehouse(configuration, profile, warehouseCode, movementDateTime);
        }

        public LotSummary[] LoadLotsByWarehouseAndMaterial(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string materialCode, string movementDateTime)
        {
            return _materialRequisitionService.LoadLotsByWarehouseAndMaterial(configuration, profile, warehouseCode, materialCode, movementDateTime);
        }

        public RequisitionReasonSummary[] LoadReasons(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _materialRequisitionService.LoadReasons(configuration, profile);
        }

        public QuickStockBalanceSummary[] LoadQuickStockBalances(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string warehouseCode,
            string materialCode,
            string movementDateTime,
            string excludedRequisitionNumber)
        {
            return _materialRequisitionService.LoadQuickStockBalances(
                configuration,
                profile,
                warehouseCode,
                materialCode,
                movementDateTime,
                excludedRequisitionNumber);
        }

        public decimal GetAvailableStockBalance(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedRequisitionNumber)
        {
            return _materialRequisitionService.GetAvailableStockBalance(
                configuration,
                profile,
                materialCode,
                lotCode,
                warehouseCode,
                movementDateTime,
                excludedRequisitionNumber);
        }

        public MaterialRequisitionSummary[] SearchRequisitions(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            return _materialRequisitionService.SearchRequisitions(configuration, profile, filter);
        }

        public MaterialRequisitionDetail LoadRequisition(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _materialRequisitionService.LoadRequisition(configuration, profile, number);
        }

        public RecordLockResult TryLockRequisition(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            return _materialRequisitionService.TryLockRequisition(configuration, profile, number, userName);
        }

        public void ReleaseRequisitionLock(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            _materialRequisitionService.ReleaseRequisitionLock(configuration, profile, number, userName);
        }

        public void CreateRequisition(AppConfiguration configuration, DatabaseProfile profile, SaveMaterialRequisitionRequest request)
        {
            _materialRequisitionService.CreateRequisition(configuration, profile, request);
        }

        public void UpdateRequisition(AppConfiguration configuration, DatabaseProfile profile, SaveMaterialRequisitionRequest request)
        {
            _materialRequisitionService.UpdateRequisition(configuration, profile, request);
        }

        public void CancelRequisition(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            _materialRequisitionService.CancelRequisition(configuration, profile, number, userName);
        }
    }
}
