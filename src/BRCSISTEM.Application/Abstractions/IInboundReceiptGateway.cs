using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IInboundReceiptGateway
    {
        IReadOnlyCollection<PackagingSummary> LoadMaterialsBySupplier(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode);

        IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode, string materialCode);

        IReadOnlyCollection<InboundReceiptSummary> SearchReceipts(DatabaseProfile profile, ConnectionResilienceSettings settings, string filter);

        InboundReceiptDetail LoadReceiptDetail(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode);

        RecordLockResult TryLockReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode, string userName);

        void ReleaseReceiptLock(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode, string userName);

        bool IsMaterialBrcEnabled(DatabaseProfile profile, ConnectionResilienceSettings settings, string materialCode);

        decimal GetActiveStockBalance(DatabaseProfile profile, ConnectionResilienceSettings settings, string materialCode, string lotCode, string warehouseCode);

        string GetParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string defaultValue);

        void CreateReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveInboundReceiptRequest request);

        void UpdateReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveInboundReceiptRequest request);

        void CancelReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode, string userName);
    }
}
