namespace BRCSISTEM.Domain.Models
{
    public sealed class NegativeStockEntry
    {
        public long MovementId { get; set; }

        public string Warehouse { get; set; }

        public string Material { get; set; }

        public string MaterialName { get; set; }

        public string Lot { get; set; }

        public string MovementDate { get; set; }

        public string MovementType { get; set; }

        public string DocumentNumber { get; set; }

        public decimal Quantity { get; set; }

        public decimal RunningBalance { get; set; }
    }
}
