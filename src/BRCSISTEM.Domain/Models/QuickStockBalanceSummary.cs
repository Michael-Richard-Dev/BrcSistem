using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class QuickStockBalanceSummary
    {
        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialDescription { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public string ExpirationDate { get; set; }

        public decimal Balance { get; set; }

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

        public string BalanceText
        {
            get { return Balance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
        }
    }
}
