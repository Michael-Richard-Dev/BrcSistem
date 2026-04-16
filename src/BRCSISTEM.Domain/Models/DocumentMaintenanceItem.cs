namespace BRCSISTEM.Domain.Models
{
    public sealed class DocumentMaintenanceItem
    {
        public string Material { get; set; }

        public string MaterialName { get; set; }

        public string Lot { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; }

        public string Warehouse { get; set; }
    }
}
