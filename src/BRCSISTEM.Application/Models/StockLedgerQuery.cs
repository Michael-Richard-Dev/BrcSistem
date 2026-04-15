namespace BRCSISTEM.Application.Models
{
    public sealed class StockLedgerQuery
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string SupplierCode { get; set; }

        public string MaterialCode { get; set; }

        public string LotCode { get; set; }

        public string WarehouseCode { get; set; }

        public string MovementType { get; set; }

        public bool IncludeInactive { get; set; }
    }
}
