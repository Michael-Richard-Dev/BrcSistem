using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IInventoryGateway
    {
        string GenerateNextInventoryNumber(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<PackagingSummary> LoadMaterialsByWarehouse(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string movementDateTime,
            bool onlyBrc);

        IReadOnlyCollection<LotSummary> LoadLotsByWarehouseAndMaterial(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string materialCode,
            string movementDateTime);

        IReadOnlyCollection<InventoryPlanningCandidateItem> LoadPlanningCandidatesByWarehouse(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string movementDateTime,
            bool onlyBrc);

        IReadOnlyCollection<InventorySummary> SearchInventories(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter);

        InventoryDetail LoadInventoryDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        RecordLockResult TryLockInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);

        void ReleaseInventoryLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);

        InventoryItemConflict FindItemConflict(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string inventoryNumber,
            string warehouseCode,
            string materialCode,
            string lotCode);

        IReadOnlyCollection<OpenMovementLockSummary> LoadOpenMovements(DatabaseProfile profile, ConnectionResilienceSettings settings, int limit);

        string GetParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string defaultValue);

        decimal GetStockBalanceAt(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string materialCode,
            string lotCode,
            string movementDateTime);

        void CreateInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveInventoryRequest request);

        void UpdateInventoryPlanning(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveInventoryRequest request);

        InventoryPointSummary AddPoint(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string inventoryNumber,
            InventoryPointInput point,
            string actorUserName);

        void ClosePoint(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId, string actorUserName);

        void ReopenPoint(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId, string actorUserName);

        void DeletePoint(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId, string actorUserName);

        void RegisterCount(DatabaseProfile profile, ConnectionResilienceSettings settings, RegisterInventoryCountRequest request);

        void TouchPointHeartbeat(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId);

        int ApplyZeroCounts(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, int pointId, string actorUserName, string ipAddress, string computerName);

        void StartInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName, bool allowEarlyStart);

        void CloseInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName);

        void ReopenInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName);

        int FinalizeInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName);

        void CancelInventory(DatabaseProfile profile, ConnectionResilienceSettings settings, string inventoryNumber, string actorUserName, string reason);
    }
}
