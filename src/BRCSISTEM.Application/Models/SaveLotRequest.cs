namespace BRCSISTEM.Application.Models
{
    public sealed class SaveLotRequest
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string MaterialCode { get; set; }

        public string SupplierCode { get; set; }

        public string ExpirationDate { get; set; }

        public string Status { get; set; }

        public string ActorUserName { get; set; }
    }
}
