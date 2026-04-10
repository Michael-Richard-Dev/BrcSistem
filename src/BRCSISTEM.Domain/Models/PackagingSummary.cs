namespace BRCSISTEM.Domain.Models
{
    public sealed class PackagingSummary
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public bool IsBrcEnabled { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public decimal StockBalance { get; set; }
    }
}
