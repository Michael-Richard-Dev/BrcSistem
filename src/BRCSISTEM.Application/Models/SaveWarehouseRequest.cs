namespace BRCSISTEM.Application.Models
{
    public sealed class SaveWarehouseRequest
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        public string Status { get; set; }

        public string ActorUserName { get; set; }
    }
}
