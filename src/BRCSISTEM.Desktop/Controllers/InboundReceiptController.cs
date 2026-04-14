using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class InboundReceiptController
    {
        private readonly InboundReceiptService _inboundReceiptService;

        public InboundReceiptController(InboundReceiptService inboundReceiptService)
        {
            _inboundReceiptService = inboundReceiptService;
        }

        public SupplierSummary[] LoadSuppliers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _inboundReceiptService.LoadSuppliers(configuration, profile);
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _inboundReceiptService.LoadMaterials(configuration, profile);
        }

        public PackagingSummary[] LoadMaterialsBySupplier(AppConfiguration configuration, DatabaseProfile profile, string supplierCode)
        {
            return _inboundReceiptService.LoadMaterialsBySupplier(configuration, profile, supplierCode);
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _inboundReceiptService.LoadWarehousesForUser(configuration, profile, userName);
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile, string supplierCode, string materialCode)
        {
            return _inboundReceiptService.LoadLots(configuration, profile, supplierCode, materialCode);
        }

        public bool IsMaterialBrcEnabled(AppConfiguration configuration, DatabaseProfile profile, string materialCode)
        {
            return _inboundReceiptService.IsMaterialBrcEnabled(configuration, profile, materialCode);
        }

        public InboundReceiptSummary[] SearchReceipts(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            return _inboundReceiptService.SearchReceipts(configuration, profile, filter);
        }

        public InboundReceiptDetail LoadReceipt(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode)
        {
            return _inboundReceiptService.LoadReceipt(configuration, profile, number, supplierCode);
        }

        public RecordLockResult TryLockReceipt(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode, string userName)
        {
            return _inboundReceiptService.TryLockReceipt(configuration, profile, number, supplierCode, userName);
        }

        public void ReleaseReceiptLock(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode, string userName)
        {
            _inboundReceiptService.ReleaseReceiptLock(configuration, profile, number, supplierCode, userName);
        }

        public void CreateReceipt(AppConfiguration configuration, DatabaseProfile profile, SaveInboundReceiptRequest request)
        {
            _inboundReceiptService.CreateReceipt(configuration, profile, request);
        }

        public void UpdateReceipt(AppConfiguration configuration, DatabaseProfile profile, SaveInboundReceiptRequest request)
        {
            _inboundReceiptService.UpdateReceipt(configuration, profile, request);
        }

        public void CancelReceipt(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode, string userName)
        {
            _inboundReceiptService.CancelReceipt(configuration, profile, number, supplierCode, userName);
        }
    }
}
