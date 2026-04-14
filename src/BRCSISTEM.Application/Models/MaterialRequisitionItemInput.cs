namespace BRCSISTEM.Application.Models
{
    public sealed class MaterialRequisitionItemInput
    {
        public string MaterialCode { get; set; }

        public string LotCode { get; set; }

        public decimal Quantity { get; set; }
    }
}
