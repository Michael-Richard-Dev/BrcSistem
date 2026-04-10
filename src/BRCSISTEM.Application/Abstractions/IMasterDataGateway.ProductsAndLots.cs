using System.Collections.Generic;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public partial interface IMasterDataGateway
    {
        IReadOnlyCollection<ProductSummary> LoadProducts(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void CreateProduct(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveProductRequest request);

        void UpdateProduct(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveProductRequest request);

        void InactivateProduct(DatabaseProfile profile, ConnectionResilienceSettings settings, string productCode);

        string GenerateNextLotCode(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<LotSummary> LoadLots(DatabaseProfile profile, ConnectionResilienceSettings settings);

        string CreateLot(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveLotRequest request);

        void UpdateLot(DatabaseProfile profile, ConnectionResilienceSettings settings, SaveLotRequest request);

        void InactivateLot(DatabaseProfile profile, ConnectionResilienceSettings settings, string lotCode);
    }
}
