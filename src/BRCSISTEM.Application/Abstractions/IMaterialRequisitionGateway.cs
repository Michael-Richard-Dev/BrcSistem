using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IMaterialRequisitionGateway
    {
        string GenerateNextRequisitionNumber(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<PackagingSummary> LoadMaterialsByWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode, string movementDateTime);

        IReadOnlyCollection<LotSummary> LoadLotsByWarehouseAndMaterial(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode, string materialCode, string movementDateTime);

        IReadOnlyCollection<RequisitionReasonSummary> LoadReasons(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<QuickStockBalanceSummary> LoadQuickStockBalances(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string materialCode,
            string movementDateTime,
            string excludedRequisitionNumber);

        IReadOnlyCollection<MaterialRequisitionSummary> SearchRequisitions(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter);

        MaterialRequisitionDetail LoadRequisitionDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        RecordLockResult TryLockRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);

        void ReleaseRequisitionLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);

        string GetParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string defaultValue);

        decimal GetStockBalanceAt(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedRequisitionNumber);

        void CreateRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveMaterialRequisitionRequest request);

        void UpdateRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveMaterialRequisitionRequest request);

        void CancelRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);
    }
}
