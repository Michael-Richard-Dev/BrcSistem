using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class ProductionOutputItemDetail
    {
        public int ItemNumber { get; set; }

        public string ProductCode { get; set; }

        public string ProductDescription { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialDescription { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public decimal QuantitySent { get; set; }

        public decimal QuantityReturned { get; set; }

        public decimal QuantityConsumed { get; set; }

        public string Status { get; set; }

        public string ProductDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ProductCode))
                {
                    return string.IsNullOrWhiteSpace(ProductDescription) ? "N/A" : ProductDescription;
                }

                return string.IsNullOrWhiteSpace(ProductDescription)
                    ? ProductCode
                    : ProductCode + " - " + ProductDescription;
            }
        }

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

        public string LotDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LotCode))
                {
                    return LotName ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(LotName)
                    ? LotCode
                    : LotCode + " - " + LotName;
            }
        }

        public string QuantitySentText
        {
            get { return QuantitySent.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
        }

        public string QuantityReturnedText
        {
            get { return QuantityReturned.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
        }

        public string QuantityConsumedText
        {
            get { return QuantityConsumed.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
        }
    }
}
