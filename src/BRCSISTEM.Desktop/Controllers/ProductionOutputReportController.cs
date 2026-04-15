using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class ProductionOutputReportController
    {
        private readonly ProductionOutputReportService _productionOutputReportService;

        public ProductionOutputReportController(ProductionOutputReportService productionOutputReportService)
        {
            _productionOutputReportService = productionOutputReportService;
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _productionOutputReportService.LoadWarehousesForUser(configuration, profile, userName);
        }

        public ProductSummary[] LoadProducts(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _productionOutputReportService.LoadProducts(configuration, profile);
        }

        public ProductionOutputReportEntry[] SearchEntries(AppConfiguration configuration, DatabaseProfile profile, ProductionOutputReportQuery query)
        {
            return _productionOutputReportService.SearchEntries(configuration, profile, query);
        }

        public ProductionOutputReportDocument LoadDocument(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _productionOutputReportService.LoadDocument(configuration, profile, number);
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, ProductionOutputReportQuery query, int rowCount)
        {
            _productionOutputReportService.RegisterCsvExport(configuration, profile, userName, query, rowCount);
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, int itemCount)
        {
            _productionOutputReportService.RegisterPdfExport(configuration, profile, userName, number, itemCount);
        }
    }
}
