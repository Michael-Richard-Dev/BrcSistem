using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InventoryItemDetail
    {
        public int ItemNumber { get; set; }

        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialDescription { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public decimal SystemBalance { get; set; }

        public decimal? CountedQuantity { get; set; }

        public decimal? AdjustmentQuantity { get; set; }

        public string AdjustmentType { get; set; }

        public string Status { get; set; }

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

        public string SystemBalanceText
        {
            get { return SystemBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
        }

        public string CountedQuantityText
        {
            get { return CountedQuantity.HasValue ? CountedQuantity.Value.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) : string.Empty; }
        }

        public string InputQuantityText
        {
            get
            {
                var adjustment = AdjustmentQuantity.GetValueOrDefault();
                return adjustment > 0M ? adjustment.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) : string.Empty;
            }
        }

        public string OutputQuantityText
        {
            get
            {
                var adjustment = AdjustmentQuantity.GetValueOrDefault();
                return adjustment < 0M ? (-adjustment).ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) : string.Empty;
            }
        }

        public string FinalBalanceText
        {
            get
            {
                var finalBalance = CountedQuantity ?? SystemBalance;
                return finalBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
            }
        }

        public string AdjustmentQuantityText
        {
            get { return AdjustmentQuantity.HasValue ? AdjustmentQuantity.Value.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) : "0,00"; }
        }
    }
}
