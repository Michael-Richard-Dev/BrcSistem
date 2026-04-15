using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InboundReceiptReportEntry
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string Number { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }

        public decimal Quantity { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public string ReceiptDateTime { get; set; }

        public string Status { get; set; }

        public string ExpirationDate { get; set; }

        public string SupplierDisplay
        {
            get
            {
                return string.IsNullOrWhiteSpace(SupplierName) ? (SupplierCode ?? string.Empty) : SupplierName;
            }
        }

        public string MaterialCodeDisplay
        {
            get { return MaterialCode ?? string.Empty; }
        }

        public string MaterialNameDisplay
        {
            get { return string.IsNullOrWhiteSpace(MaterialName) ? (MaterialCode ?? string.Empty) : MaterialName; }
        }

        public string QuantityText
        {
            get { return Quantity.ToString("N2", PtBr); }
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

        public string ReceiptDateDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ReceiptDateTime))
                {
                    return string.Empty;
                }

                DateTime parsed;
                var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy" };
                return DateTime.TryParseExact(ReceiptDateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy", PtBr)
                    : ReceiptDateTime;
            }
        }

        public string ExpirationDateDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExpirationDate))
                {
                    return "N/I";
                }

                DateTime parsed;
                var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss" };
                return DateTime.TryParseExact(ExpirationDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy", PtBr)
                    : ExpirationDate;
            }
        }
    }
}
