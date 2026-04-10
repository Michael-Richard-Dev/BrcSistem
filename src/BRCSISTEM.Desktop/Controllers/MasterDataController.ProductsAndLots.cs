using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed partial class MasterDataController
    {
        public ProductSummary[] LoadProducts(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _masterDataService.LoadProducts(configuration, profile);
        }

        public void CreateProduct(AppConfiguration configuration, DatabaseProfile profile, SaveProductRequest request)
        {
            _masterDataService.CreateProduct(configuration, profile, request);
        }

        public void UpdateProduct(AppConfiguration configuration, DatabaseProfile profile, SaveProductRequest request)
        {
            _masterDataService.UpdateProduct(configuration, profile, request);
        }

        public void InactivateProduct(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string productCode)
        {
            _masterDataService.InactivateProduct(configuration, profile, actorUserName, productCode);
        }

        public string GenerateNextLotCode(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _masterDataService.GenerateNextLotCode(configuration, profile);
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _masterDataService.LoadLots(configuration, profile);
        }

        public string CreateLot(AppConfiguration configuration, DatabaseProfile profile, SaveLotRequest request)
        {
            return _masterDataService.CreateLot(configuration, profile, request);
        }

        public void UpdateLot(AppConfiguration configuration, DatabaseProfile profile, SaveLotRequest request)
        {
            _masterDataService.UpdateLot(configuration, profile, request);
        }

        public void InactivateLot(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string lotCode)
        {
            _masterDataService.InactivateLot(configuration, profile, actorUserName, lotCode);
        }
    }
}
