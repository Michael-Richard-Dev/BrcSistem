using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class StockMovementReportController
    {
        private readonly StockMovementReportService _stockMovementReportService;

        public StockMovementReportController(StockMovementReportService stockMovementReportService)
        {
            _stockMovementReportService = stockMovementReportService;
        }

        public WarehouseSummary[] LoadWarehouses(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockMovementReportService.LoadWarehouses(configuration, profile);
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockMovementReportService.LoadMaterials(configuration, profile);
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockMovementReportService.LoadLots(configuration, profile);
        }

        public StockMovementReportRow[] LoadRows(AppConfiguration configuration, DatabaseProfile profile, StockMovementReportQuery query)
        {
            return _stockMovementReportService.LoadRows(configuration, profile, query);
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockMovementReportQuery query, int rowCount)
        {
            _stockMovementReportService.RegisterCsvExport(configuration, profile, userName, query, rowCount);
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockMovementReportQuery query, int rowCount)
        {
            _stockMovementReportService.RegisterPdfExport(configuration, profile, userName, query, rowCount);
        }
    }
}
