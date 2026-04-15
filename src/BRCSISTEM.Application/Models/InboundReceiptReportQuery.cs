namespace BRCSISTEM.Application.Models
{
    public sealed class InboundReceiptReportQuery
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string ReceiptNumber { get; set; }

        public string SupplierCode { get; set; }

        public bool ExcludeCanceled { get; set; }
    }
}
