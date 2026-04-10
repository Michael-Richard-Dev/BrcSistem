using System;

namespace BRCSISTEM.Domain.Models
{
    public sealed class LotSummary
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialDescription { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string ExpirationDate { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public decimal StockBalance { get; set; }

        public string MaterialDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MaterialCode))
                {
                    return MaterialDescription ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(MaterialDescription)
                    ? MaterialCode
                    : MaterialCode + " - " + MaterialDescription;
            }
        }

        public string SupplierDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SupplierCode))
                {
                    return SupplierName ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(SupplierName)
                    ? SupplierCode
                    : SupplierCode + " - " + SupplierName;
            }
        }
    }
}
