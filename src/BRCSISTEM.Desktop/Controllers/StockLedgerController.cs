using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class StockLedgerController
    {
        private readonly StockLedgerService _stockLedgerService;

        public StockLedgerController(StockLedgerService stockLedgerService)
        {
            _stockLedgerService = stockLedgerService;
        }

        public SupplierSummary[] LoadSuppliers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockLedgerService.LoadSuppliers(configuration, profile);
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile, string supplierCode)
        {
            return _stockLedgerService.LoadMaterials(configuration, profile, supplierCode);
        }

        public WarehouseSummary[] LoadWarehouses(AppConfiguration configuration, DatabaseProfile profile, string supplierCode)
        {
            return _stockLedgerService.LoadWarehouses(configuration, profile, supplierCode);
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile, string materialCode, string supplierCode)
        {
            return _stockLedgerService.LoadLots(configuration, profile, materialCode, supplierCode);
        }

        public StockLedgerEntry[] LoadEntries(AppConfiguration configuration, DatabaseProfile profile, StockLedgerQuery query)
        {
            return _stockLedgerService.LoadEntries(configuration, profile, query);
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockLedgerQuery query, int rowCount)
        {
            _stockLedgerService.RegisterCsvExport(configuration, profile, userName, query, rowCount);
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockLedgerQuery query, int rowCount)
        {
            _stockLedgerService.RegisterPdfExport(configuration, profile, userName, query, rowCount);
        }
    }
}
