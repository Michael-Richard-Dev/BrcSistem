namespace BRCSISTEM.Application.Models
{
    public sealed class InboundReceiptItemInput
    {
        public string MaterialCode { get; set; }

        public string LotCode { get; set; }

        public decimal Quantity { get; set; }
    }
}
