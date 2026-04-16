using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class InventoryReportController
    {
        private readonly InventoryReportService _inventoryReportService;

        public InventoryReportController(InventoryReportService inventoryReportService)
        {
            _inventoryReportService = inventoryReportService;
        }

        public InventoryReportEntry[] LoadInventories(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _inventoryReportService.LoadInventories(configuration, profile);
        }

        public InventoryReportDocument LoadDocument(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _inventoryReportService.LoadDocument(configuration, profile, number);
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, int itemCount, int movementCount)
        {
            _inventoryReportService.RegisterCsvExport(configuration, profile, userName, number, itemCount, movementCount);
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, int divergentItemCount, int movementCount)
        {
            _inventoryReportService.RegisterPdfExport(configuration, profile, userName, number, divergentItemCount, movementCount);
        }
    }
}
