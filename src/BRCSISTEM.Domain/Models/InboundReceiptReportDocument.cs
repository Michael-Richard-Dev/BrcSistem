using System;
using System.Globalization;
using System.Linq;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InboundReceiptReportDocument
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public InboundReceiptReportDocument()
        {
            Items = Array.Empty<InboundReceiptReportItem>();
        }

        public string Number { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string EmissionDate { get; set; }

        public string ReceiptDateTime { get; set; }

        public string Status { get; set; }

        public InboundReceiptReportItem[] Items { get; set; }

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

        public string WarehouseDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(WarehouseCode))
                {
                    return WarehouseName ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(WarehouseName)
                    ? WarehouseCode
                    : WarehouseCode + " - " + WarehouseName;
            }
        }

        public string EmissionDateDisplay
        {
            get { return FormatDate(EmissionDate); }
        }

        public string ReceiptDateDisplay
        {
            get { return FormatDateTime(ReceiptDateTime); }
        }

        public decimal TotalQuantity
        {
            get { return (Items ?? Array.Empty<InboundReceiptReportItem>()).Sum(item => item == null ? 0M : item.Quantity); }
        }

        public string TotalQuantityText
        {
            get { return TotalQuantity.ToString("N2", PtBr); }
        }

        private static string FormatDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss" };
            return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy", PtBr)
                : value;
        }

        private static string FormatDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy" };
            return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", PtBr)
                : value;
        }
    }
}
