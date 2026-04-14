namespace BRCSISTEM.Application.Models
{
    public sealed class ProductionOutputItemInput
    {
        public string ProductCode { get; set; }

        public string MaterialCode { get; set; }

        public string LotCode { get; set; }

        public decimal QuantitySent { get; set; }

        public decimal QuantityReturned { get; set; }

        public decimal QuantityConsumed { get; set; }
    }
}
