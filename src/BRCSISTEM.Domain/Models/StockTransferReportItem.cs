using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockTransferReportItem
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public int ItemNumber { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialDescription { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public decimal Quantity { get; set; }

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
                    return string.IsNullOrWhiteSpace(LotName) ? "N/I" : LotName;
                }

                return string.IsNullOrWhiteSpace(LotName)
                    ? LotCode
                    : LotCode + " - " + LotName;
            }
        }

        public string QuantityText
        {
            get { return Quantity.ToString("N2", PtBr); }
        }
    }
}
