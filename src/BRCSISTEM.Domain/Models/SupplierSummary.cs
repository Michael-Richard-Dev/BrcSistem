namespace BRCSISTEM.Domain.Models
{
    public sealed class SupplierSummary
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Cnpj { get; set; }

        public string City { get; set; }

        public bool IsBrcEnabled { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }
    }
}
