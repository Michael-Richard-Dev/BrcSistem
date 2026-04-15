using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InboundReceiptReportItem
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }

        public decimal Quantity { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public string ExpirationDate { get; set; }

        public string ReceiptDateTime { get; set; }

        public string Status { get; set; }

        public string MaterialDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MaterialCode))
                {
                    return MaterialName ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(MaterialName)
                    ? MaterialCode
                    : MaterialCode + " - " + MaterialName;
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

        public string ExpirationDateDisplay
        {
            get { return FormatDate(ExpirationDate); }
        }

        public string ReceiptDateDisplay
        {
            get { return FormatDate(ReceiptDateTime); }
        }

        private static string FormatDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "N/I";
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss" };
            return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy", PtBr)
                : value;
        }
    }
}
