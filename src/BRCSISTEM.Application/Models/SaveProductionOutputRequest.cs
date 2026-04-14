namespace BRCSISTEM.Application.Models
{
    public sealed class SaveProductionOutputRequest
    {
        public SaveProductionOutputRequest()
        {
            Items = new ProductionOutputItemInput[0];
        }

        public string Number { get; set; }

        public string WarehouseCode { get; set; }

        public string Purpose { get; set; }

        public string Shift { get; set; }

        public string MovementDateTime { get; set; }

        public string ActorUserName { get; set; }

        public ProductionOutputItemInput[] Items { get; set; }
    }
}
