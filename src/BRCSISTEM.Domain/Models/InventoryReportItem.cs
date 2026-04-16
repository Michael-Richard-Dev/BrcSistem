using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InventoryReportItem
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialDescription { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public decimal SystemBalance { get; set; }

        public decimal CountedQuantity { get; set; }

        public decimal AdjustmentQuantity { get; set; }

        public string AdjustmentType { get; set; }

        public string ExpirationDate { get; set; }

        public string Status { get; set; }

        public bool IsDivergent
        {
            get { return Math.Abs(AdjustmentQuantity) > 0.0000001M; }
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
                    return string.IsNullOrWhiteSpace(LotName) ? "N/I" : LotName;
                }

                return string.IsNullOrWhiteSpace(LotName)
                    ? LotCode
                    : LotCode + " - " + LotName;
            }
        }

        public string SystemBalanceText
        {
            get { return SystemBalance.ToString("N2", PtBr); }
        }

        public string CountedQuantityText
        {
            get { return CountedQuantity.ToString("N2", PtBr); }
        }

        public string AdjustmentQuantityText
        {
            get { return AdjustmentQuantity.ToString("N2", PtBr); }
        }

        public string ExpirationDateDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExpirationDate))
                {
                    return string.Empty;
                }

                DateTime parsed;
                var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "yyyy-MM-dd HH:mm:ss" };
                return DateTime.TryParseExact(ExpirationDate.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy", PtBr)
                    : ExpirationDate;
            }
        }
    }
}
