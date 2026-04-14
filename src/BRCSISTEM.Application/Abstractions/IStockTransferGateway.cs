using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IStockTransferGateway
    {
        string GenerateNextTransferNumber(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<PackagingSummary> LoadMaterialsByWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode, string movementDateTime);

        IReadOnlyCollection<LotSummary> LoadLotsByWarehouseAndMaterial(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode, string materialCode, string movementDateTime);

        IReadOnlyCollection<StockTransferSummary> SearchTransfers(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter);

        StockTransferDetail LoadTransferDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        RecordLockResult TryLockTransfer(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);

        void ReleaseTransferLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);

        string GetParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string defaultValue);

        decimal GetStockBalanceAt(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedTransferNumber);

        void CreateTransfer(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveStockTransferRequest request);

        void UpdateTransfer(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveStockTransferRequest request);

        void CancelTransfer(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string userName);
    }
}
