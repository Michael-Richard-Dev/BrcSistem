namespace BRCSISTEM.Application.Models
{
    public sealed class StockTransferReportQuery
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string TransferNumber { get; set; }

        public string OriginWarehouseCode { get; set; }

        public string DestinationWarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string UserName { get; set; }

        public bool ExcludeCanceled { get; set; }
    }
}
