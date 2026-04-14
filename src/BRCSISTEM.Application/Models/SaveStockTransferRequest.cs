namespace BRCSISTEM.Application.Models
{
    public sealed class SaveStockTransferRequest
    {
        public SaveStockTransferRequest()
        {
            Items = new StockTransferItemInput[0];
        }

        public string Number { get; set; }

        public string OriginWarehouseCode { get; set; }

        public string DestinationWarehouseCode { get; set; }

        public string MovementDateTime { get; set; }

        public string ActorUserName { get; set; }

        public StockTransferItemInput[] Items { get; set; }
    }
}
