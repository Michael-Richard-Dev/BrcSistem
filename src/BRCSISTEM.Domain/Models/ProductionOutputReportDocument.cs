using System;
using System.Globalization;
using System.Linq;

namespace BRCSISTEM.Domain.Models
{
    public sealed class ProductionOutputReportDocument
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public ProductionOutputReportDocument()
        {
            Items = Array.Empty<ProductionOutputReportItem>();
        }

        public string Number { get; set; }

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string Purpose { get; set; }

        public string Shift { get; set; }

        public string MovementDateTime { get; set; }

        public string Status { get; set; }

        public ProductionOutputReportItem[] Items { get; set; }

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

        public string PurposeDisplay
        {
            get { return string.IsNullOrWhiteSpace(Purpose) ? "SAIDA DE PRODUCAO" : Purpose; }
        }

        public string MovementDateDisplay
        {
            get { return FormatDateTime(MovementDateTime); }
        }

        public decimal TotalSent
        {
            get { return (Items ?? Array.Empty<ProductionOutputReportItem>()).Sum(item => item == null ? 0M : item.QuantitySent); }
        }

        public decimal TotalReturned
        {
            get { return (Items ?? Array.Empty<ProductionOutputReportItem>()).Sum(item => item == null ? 0M : item.QuantityReturned); }
        }

        public decimal TotalConsumed
        {
            get { return (Items ?? Array.Empty<ProductionOutputReportItem>()).Sum(item => item == null ? 0M : item.QuantityConsumed); }
        }

        public string TotalSentText
        {
            get { return TotalSent.ToString("N2", PtBr); }
        }

        public string TotalReturnedText
        {
            get { return TotalReturned.ToString("N2", PtBr); }
        }

        public string TotalConsumedText
        {
            get { return TotalConsumed.ToString("N2", PtBr); }
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
