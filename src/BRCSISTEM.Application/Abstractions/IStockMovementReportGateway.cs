using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IStockMovementReportGateway
    {
        IReadOnlyCollection<WarehouseSummary> LoadWarehouses(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<PackagingSummary> LoadMaterials(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<StockMovementReportRow> SearchRows(DatabaseProfile profile, ConnectionResilienceSettings settings, StockMovementReportQuery query);
    }
}
