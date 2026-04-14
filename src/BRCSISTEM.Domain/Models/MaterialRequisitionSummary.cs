using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class MaterialRequisitionSummary
    {
        public string Number { get; set; }

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string MovementDateTime { get; set; }

        public int ItemCount { get; set; }

        public string MaterialsPreview { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public string LockedBy { get; set; }

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

                var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "yyyy-MM-dd" };
                DateTime parsed;
                return DateTime.TryParseExact(MovementDateTime.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))
                    : MovementDateTime;
            }
        }
    }
}
