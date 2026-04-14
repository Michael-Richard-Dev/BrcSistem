using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockTransferSummary
    {
        public string Number { get; set; }

        public string OriginWarehouseCode { get; set; }

        public string OriginWarehouseName { get; set; }

        public string DestinationWarehouseCode { get; set; }

        public string DestinationWarehouseName { get; set; }

        public string MovementDateTime { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public string LockedBy { get; set; }

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

        public string MovementDateTimeDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MovementDateTime))
                {
                    return string.Empty;
                }

                var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "yyyy-MM-dd" };
                DateTime parsed;
                return DateTime.TryParseExact(MovementDateTime.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))
                    : MovementDateTime;
            }
        }
    }
}
