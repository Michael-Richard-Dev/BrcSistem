namespace BRCSISTEM.Domain.Models
{
    public sealed class DuplicateLotEntry
    {
        public string Material { get; set; }

        public string MaterialName { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public string Supplier { get; set; }

        public string SupplierName { get; set; }

        public string Validity { get; set; }

        public int DuplicateCount { get; set; }

        public string GroupCodes { get; set; }
    }
}
