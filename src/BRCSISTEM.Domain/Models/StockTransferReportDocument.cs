using System;
using System.Globalization;
using System.Linq;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockTransferReportDocument
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public StockTransferReportDocument()
        {
            Items = Array.Empty<StockTransferReportItem>();
        }

        public string Number { get; set; }

        public string OriginWarehouseCode { get; set; }

        public string OriginWarehouseName { get; set; }

        public string DestinationWarehouseCode { get; set; }

        public string DestinationWarehouseName { get; set; }

        public string MovementDateTime { get; set; }

        public string Status { get; set; }

        public StockTransferReportItem[] Items { get; set; }

        public string OriginWarehouseDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(OriginWarehouseCode))
                {
                    return OriginWarehouseName ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(OriginWarehouseName)
                    ? OriginWarehouseCode
                    : OriginWarehouseCode + " - " + OriginWarehouseName;
            }
        }

        public string DestinationWarehouseDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DestinationWarehouseCode))
                {
                    return DestinationWarehouseName ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(DestinationWarehouseName)
                    ? DestinationWarehouseCode
                    : DestinationWarehouseCode + " - " + DestinationWarehouseName;
            }
        }

        public string MovementDateDisplay
        {
            get { return FormatDateTime(MovementDateTime); }
        }

        public decimal TotalQuantity
        {
            get { return (Items ?? Array.Empty<StockTransferReportItem>()).Sum(item => item == null ? 0M : item.Quantity); }
        }

        public string TotalQuantityText
        {
            get { return TotalQuantity.ToString("N2", PtBr); }
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
