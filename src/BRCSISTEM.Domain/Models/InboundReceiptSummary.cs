using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InboundReceiptSummary
    {
        public string Number { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string MovementDateTime { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public string LockedBy { get; set; }

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

        public string MovementDateTimeDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MovementDateTime))
                {
                    return string.Empty;
                }

                DateTime parsed;
                var formats = new[]
                {
                    "yyyy-MM-dd HH:mm:ss",
                    "yyyy-MM-dd HH:mm",
                    "dd/MM/yyyy HH:mm",
                    "dd/MM/yyyy"
                };

                if (DateTime.TryParseExact(MovementDateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                {
                    return parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                }

                return MovementDateTime;
            }
        }
    }
}
