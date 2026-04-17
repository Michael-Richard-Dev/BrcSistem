namespace BRCSISTEM.Domain.Models
{
    public sealed class DocumentMaintenanceHeader
    {
        public string DocumentNumber { get; set; }

        public string Supplier { get; set; }

        public string DocumentType { get; set; }

        public string Date { get; set; }

        public string EmissionDate { get; set; }

        public string Purpose { get; set; }

        public string Status { get; set; }

        public string LockedBy { get; set; }

        public string Warehouse { get; set; }

        public string OriginWarehouse { get; set; }

        public string DestinationWarehouse { get; set; }

        public string UserName { get; set; }

        public string DisplayLabel => string.IsNullOrWhiteSpace(Supplier)
            ? DocumentNumber
            : $"{DocumentNumber} / {Supplier}";
    }
}
