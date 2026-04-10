using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public partial interface IMasterDataGateway
    {
        IReadOnlyCollection<SupplierSummary> LoadSuppliers(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void CreateSupplier(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveSupplierRequest request);

        void UpdateSupplier(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveSupplierRequest request);

        void InactivateSupplier(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode);

        IReadOnlyCollection<PackagingSummary> LoadPackagings(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void CreatePackaging(DatabaseProfile profile, ConnectionResilienceSettings settings, SavePackagingRequest request);

        void UpdatePackaging(DatabaseProfile profile, ConnectionResilienceSettings settings, SavePackagingRequest request);

        void InactivatePackaging(DatabaseProfile profile, ConnectionResilienceSettings settings, string packagingCode);

        IReadOnlyCollection<WarehouseSummary> LoadWarehouses(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void CreateWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveWarehouseRequest request);

        void UpdateWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveWarehouseRequest request);

        void InactivateWarehouse(DatabaseProfile profile, ConnectionResilienceSettings settings, string warehouseCode);
    }
}
