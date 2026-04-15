using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IStockSummaryGateway
    {
        IReadOnlyCollection<WarehouseSummary> LoadWarehouses(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<PackagingSummary> LoadMaterials(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<StockSummaryEntry> LoadEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, StockSummaryQuery query);
    }
}
