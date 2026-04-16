namespace BRCSISTEM.Domain.Models
{
    public sealed class DocumentDateEntry
    {
        public string DocumentNumber { get; set; }

        public string Supplier { get; set; }

        public string Date { get; set; }

        public string Status { get; set; }

        public string OriginWarehouse { get; set; }

        public string DestinationWarehouse { get; set; }

        public string DisplayLabel => string.IsNullOrWhiteSpace(Supplier)
            ? DocumentNumber
            : $"{DocumentNumber} / {Supplier}";
    }
}
