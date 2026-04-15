using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class InventoryController
    {
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public InventoryPermissions ResolvePermissions(UserIdentity identity)
        {
            return _inventoryService.ResolvePermissions(identity);
        }

        public string GenerateNextInventoryNumber(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _inventoryService.GenerateNextInventoryNumber(configuration, profile);
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _inventoryService.LoadWarehousesForUser(configuration, profile, userName);
        }

        public PackagingSummary[] LoadMaterialsByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime, bool onlyBrc)
        {
            return _inventoryService.LoadMaterialsByWarehouse(configuration, profile, warehouseCode, movementDateTime, onlyBrc);
        }

        public LotSummary[] LoadLotsByWarehouseAndMaterial(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string materialCode, string movementDateTime)
        {
            return _inventoryService.LoadLotsByWarehouseAndMaterial(configuration, profile, warehouseCode, materialCode, movementDateTime);
        }

        public InventoryPlanningCandidateItem[] LoadPlanningCandidatesByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime, bool onlyBrc)
        {
            return _inventoryService.LoadPlanningCandidatesByWarehouse(configuration, profile, warehouseCode, movementDateTime, onlyBrc);
        }

        public InventorySummary[] SearchInventories(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            return _inventoryService.SearchInventories(configuration, profile, filter);
        }

        public InventoryDetail LoadInventory(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _inventoryService.LoadInventory(configuration, profile, number);
        }

        public RecordLockResult TryLockInventory(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            return _inventoryService.TryLockInventory(configuration, profile, number, userName);
        }

        public void ReleaseInventoryLock(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            _inventoryService.ReleaseInventoryLock(configuration, profile, number, userName);
        }

        public InventoryItemConflict FindItemConflict(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string warehouseCode, string materialCode, string lotCode)
        {
            return _inventoryService.FindItemConflict(configuration, profile, inventoryNumber, warehouseCode, materialCode, lotCode);
        }

        public OpenMovementLockSummary[] LoadOpenMovements(AppConfiguration configuration, DatabaseProfile profile, int limit)
        {
            return _inventoryService.LoadOpenMovements(configuration, profile, limit);
        }

        public decimal GetStockBalanceAt(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string materialCode, string lotCode, string movementDateTime)
        {
            return _inventoryService.GetStockBalanceAt(configuration, profile, warehouseCode, materialCode, lotCode, movementDateTime);
        }

        public void CreateInventory(AppConfiguration configuration, DatabaseProfile profile, SaveInventoryRequest request)
        {
            _inventoryService.CreateInventory(configuration, profile, request);
        }

        public void UpdateInventoryPlanning(AppConfiguration configuration, DatabaseProfile profile, SaveInventoryRequest request)
        {
            _inventoryService.UpdateInventoryPlanning(configuration, profile, request);
        }

        public InventoryPointSummary AddPoint(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, InventoryPointInput point, string userName)
        {
            return _inventoryService.AddPoint(configuration, profile, inventoryNumber, point, userName);
        }

        public void ClosePoint(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId, string userName)
        {
            _inventoryService.ClosePoint(configuration, profile, inventoryNumber, pointId, userName);
        }

        public void ReopenPoint(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId, string userName)
        {
            _inventoryService.ReopenPoint(configuration, profile, inventoryNumber, pointId, userName);
        }

        public void DeletePoint(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId, string userName)
        {
            _inventoryService.DeletePoint(configuration, profile, inventoryNumber, pointId, userName);
        }

        public void RegisterCount(AppConfiguration configuration, DatabaseProfile profile, RegisterInventoryCountRequest request)
        {
            _inventoryService.RegisterCount(configuration, profile, request);
        }

        public void TouchPointHeartbeat(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId)
        {
            _inventoryService.TouchPointHeartbeat(configuration, profile, inventoryNumber, pointId);
        }

        public int ApplyZeroCounts(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId, string userName, string ipAddress, string computerName)
        {
            return _inventoryService.ApplyZeroCounts(configuration, profile, inventoryNumber, pointId, userName, ipAddress, computerName);
        }

        public void StartInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName, bool allowEarlyStart)
        {
            _inventoryService.StartInventory(configuration, profile, inventoryNumber, userName, allowEarlyStart);
        }

        public void CloseInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName)
        {
            _inventoryService.CloseInventory(configuration, profile, inventoryNumber, userName);
        }

        public void ReopenInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName)
        {
            _inventoryService.ReopenInventory(configuration, profile, inventoryNumber, userName);
        }

        public int FinalizeInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName)
        {
            return _inventoryService.FinalizeInventory(configuration, profile, inventoryNumber, userName);
        }

        public void CancelInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName, string reason)
        {
            _inventoryService.CancelInventory(configuration, profile, inventoryNumber, userName, reason);
        }
    }
}
