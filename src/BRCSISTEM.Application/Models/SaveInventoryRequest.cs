namespace BRCSISTEM.Application.Models
{
    public sealed class SaveInventoryRequest
    {
        public SaveInventoryRequest()
        {
            Items = new InventoryItemInput[0];
            Points = new InventoryPointInput[0];
        }

        public string Number { get; set; }

        public string CreatedDateTime { get; set; }

        public string ScheduledDateTime { get; set; }

        public string Observation { get; set; }

        public int MaxOpenPoints { get; set; }

        public string ActorUserName { get; set; }

        public InventoryItemInput[] Items { get; set; }

        public InventoryPointInput[] Points { get; set; }
    }
}
