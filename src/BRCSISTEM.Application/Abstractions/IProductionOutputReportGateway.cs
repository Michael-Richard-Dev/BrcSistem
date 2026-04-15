using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IProductionOutputReportGateway
    {
        IReadOnlyCollection<WarehouseSummary> LoadWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<ProductionOutputReportEntry> SearchEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, ProductionOutputReportQuery query);

        ProductionOutputReportDocument LoadDocument(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);
    }
}
