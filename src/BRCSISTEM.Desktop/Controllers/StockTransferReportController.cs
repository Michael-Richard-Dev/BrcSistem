using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class StockTransferReportController
    {
        private readonly StockTransferReportService _stockTransferReportService;

        public StockTransferReportController(StockTransferReportService stockTransferReportService)
        {
            _stockTransferReportService = stockTransferReportService;
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _stockTransferReportService.LoadWarehousesForUser(configuration, profile, userName);
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockTransferReportService.LoadMaterials(configuration, profile);
        }

        public StockTransferReportEntry[] SearchEntries(AppConfiguration configuration, DatabaseProfile profile, StockTransferReportQuery query)
        {
            return _stockTransferReportService.SearchEntries(configuration, profile, query);
        }

        public StockTransferReportDocument LoadDocument(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _stockTransferReportService.LoadDocument(configuration, profile, number);
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockTransferReportQuery query, int rowCount)
        {
            _stockTransferReportService.RegisterCsvExport(configuration, profile, userName, query, rowCount);
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, int itemCount)
        {
            _stockTransferReportService.RegisterPdfExport(configuration, profile, userName, number, itemCount);
        }
    }
}
