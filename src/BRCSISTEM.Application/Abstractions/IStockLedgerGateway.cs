using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IStockLedgerGateway
    {
        IReadOnlyCollection<SupplierSummary> LoadSuppliers(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<PackagingSummary> LoadMaterials(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode);

        IReadOnlyCollection<WarehouseSummary> LoadWarehouses(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode);

        IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings, string materialCode, string supplierCode);

        IReadOnlyCollection<StockLedgerEntry> SearchEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, StockLedgerQuery query);

        decimal GetInitialBalance(DatabaseProfile profile, ConnectionResilienceSettings settings, StockLedgerQuery query);
    }
}
