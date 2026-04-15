using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class InboundReceiptReportController
    {
        private readonly InboundReceiptReportService _inboundReceiptReportService;

        public InboundReceiptReportController(InboundReceiptReportService inboundReceiptReportService)
        {
            _inboundReceiptReportService = inboundReceiptReportService;
        }

        public SupplierSummary[] LoadSuppliers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _inboundReceiptReportService.LoadSuppliers(configuration, profile);
        }

        public InboundReceiptReportEntry[] SearchEntries(AppConfiguration configuration, DatabaseProfile profile, InboundReceiptReportQuery query)
        {
            return _inboundReceiptReportService.SearchEntries(configuration, profile, query);
        }

        public InboundReceiptReportDocument LoadDocument(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode)
        {
            return _inboundReceiptReportService.LoadDocument(configuration, profile, number, supplierCode);
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, InboundReceiptReportQuery query, int rowCount)
        {
            _inboundReceiptReportService.RegisterCsvExport(configuration, profile, userName, query, rowCount);
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, string supplierCode, int itemCount)
        {
            _inboundReceiptReportService.RegisterPdfExport(configuration, profile, userName, number, supplierCode, itemCount);
        }
    }
}
