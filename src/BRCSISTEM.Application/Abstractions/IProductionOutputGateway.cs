using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IProductionOutputGateway
    {
        string GenerateNextOutputNumber(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<PackagingSummary> LoadMaterialsByWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode, string movementDateTime);

        IReadOnlyCollection<LotSummary> LoadLotsByWarehouseAndMaterial(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode, string materialCode, string movementDateTime);

        IReadOnlyCollection<ProductionOutputSummary> SearchOutputs(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter);

        ProductionOutputDetail LoadOutputDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        RecordLockResult TryLockOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);

        void ReleaseOutputLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);

        string GetParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string defaultValue);

        decimal GetStockBalanceAt(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedOutputNumber);

        void CreateOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveProductionOutputRequest request);

        void UpdateOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveProductionOutputRequest request);

        void CancelOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);
    }
}
