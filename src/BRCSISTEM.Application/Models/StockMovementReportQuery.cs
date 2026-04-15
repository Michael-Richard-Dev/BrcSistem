namespace BRCSISTEM.Application.Models
{
    public sealed class StockMovementReportQuery
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string LotCode { get; set; }

        public bool OnlyRowsWithMovementOrPositiveBalance { get; set; }
    }
}
