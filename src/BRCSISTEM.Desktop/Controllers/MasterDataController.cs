using BRCSISTEM.Application.Models;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed partial class MasterDataController
    {
        private readonly MasterDataService _masterDataService;

        public MasterDataController(MasterDataService masterDataService)
        {
            _masterDataService = masterDataService;
        }

        public SupplierSummary[] LoadSuppliers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _masterDataService.LoadSuppliers(configuration, profile);
        }

        public void CreateSupplier(AppConfiguration configuration, DatabaseProfile profile, SaveSupplierRequest request)
        {
            _masterDataService.CreateSupplier(configuration, profile, request);
        }

        public void UpdateSupplier(AppConfiguration configuration, DatabaseProfile profile, SaveSupplierRequest request)
        {
            _masterDataService.UpdateSupplier(configuration, profile, request);
        }

        public void InactivateSupplier(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string supplierCode)
        {
            _masterDataService.InactivateSupplier(configuration, profile, actorUserName, supplierCode);
        }

        public PackagingSummary[] LoadPackagings(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _masterDataService.LoadPackagings(configuration, profile);
        }

        public void CreatePackaging(AppConfiguration configuration, DatabaseProfile profile, SavePackagingRequest request)
        {
            _masterDataService.CreatePackaging(configuration, profile, request);
        }

        public void UpdatePackaging(AppConfiguration configuration, DatabaseProfile profile, SavePackagingRequest request)
        {
            _masterDataService.UpdatePackaging(configuration, profile, request);
        }

        public void InactivatePackaging(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string packagingCode)
        {
            _masterDataService.InactivatePackaging(configuration, profile, actorUserName, packagingCode);
        }

        public WarehouseSummary[] LoadWarehouses(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _masterDataService.LoadWarehouses(configuration, profile);
        }

        public void CreateWarehouse(AppConfiguration configuration, DatabaseProfile profile, SaveWarehouseRequest request)
        {
            _masterDataService.CreateWarehouse(configuration, profile, request);
        }

        public void UpdateWarehouse(AppConfiguration configuration, DatabaseProfile profile, SaveWarehouseRequest request)
        {
            _masterDataService.UpdateWarehouse(configuration, profile, request);
        }

        public void InactivateWarehouse(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string warehouseCode)
        {
            _masterDataService.InactivateWarehouse(configuration, profile, actorUserName, warehouseCode);
        }
    }
}
