using System.Collections.Generic;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IInventoryReportGateway
    {
        IReadOnlyCollection<InventoryReportEntry> LoadInventories(DatabaseProfile profile, ConnectionResilienceSettings settings);

        InventoryReportDocument LoadDocument(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);
    }
}
