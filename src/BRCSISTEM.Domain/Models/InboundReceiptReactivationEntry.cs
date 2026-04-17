namespace BRCSISTEM.Domain.Models
{
    public sealed class InboundReceiptReactivationEntry
    {
        public string Number { get; set; }

        public string Supplier { get; set; }

        public string Warehouse { get; set; }

        public int Version { get; set; }

        public string EmissionDate { get; set; }

        public string Status { get; set; }
    }
}
