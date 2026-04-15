using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class StockSummaryController
    {
        private readonly StockSummaryService _stockSummaryService;

        public StockSummaryController(StockSummaryService stockSummaryService)
        {
            _stockSummaryService = stockSummaryService;
        }

        public WarehouseSummary[] LoadWarehouses(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockSummaryService.LoadWarehouses(configuration, profile);
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockSummaryService.LoadMaterials(configuration, profile);
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockSummaryService.LoadLots(configuration, profile);
        }

        public StockSummaryEntry[] LoadEntries(AppConfiguration configuration, DatabaseProfile profile, StockSummaryQuery query)
        {
            return _stockSummaryService.LoadEntries(configuration, profile, query);
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockSummaryQuery query, int rowCount)
        {
            _stockSummaryService.RegisterCsvExport(configuration, profile, userName, query, rowCount);
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockSummaryQuery query, int rowCount)
        {
            _stockSummaryService.RegisterPdfExport(configuration, profile, userName, query, rowCount);
        }
    }
}
