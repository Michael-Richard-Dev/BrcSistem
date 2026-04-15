namespace BRCSISTEM.Application.Models
{
    public sealed class StockSummaryQuery
    {
        public string ReferenceDate { get; set; }

        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string LotCode { get; set; }
    }
}
