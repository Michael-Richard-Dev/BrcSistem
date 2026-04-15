using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IInboundReceiptReportGateway
    {
        IReadOnlyCollection<SupplierSummary> LoadSuppliers(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<InboundReceiptReportEntry> SearchEntries(DatabaseProfile profile, ConnectionResilienceSettings settings, InboundReceiptReportQuery query);

        InboundReceiptReportDocument LoadDocument(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplierCode);
    }
}
