using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IStockTransferReportGateway
    {
        IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<StockTransferReportEntry> SearchEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, StockTransferReportQuery query);

        StockTransferReportDocument LoadDocument(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);
    }
}
