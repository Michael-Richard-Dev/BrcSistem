using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InventoryReportMovement
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string MovementDateTime { get; set; }

        public int ItemNumber { get; set; }

        public string Type { get; set; }

        public string SupplierCode { get; set; }

        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string LotCode { get; set; }

        public decimal Quantity { get; set; }

        public string ExpirationDate { get; set; }

        public string UserName { get; set; }

        public string Status { get; set; }

        public string MovementDateDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MovementDateTime))
                {
                    return string.Empty;
                }

                DateTime parsed;
                var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy" };
                return DateTime.TryParseExact(MovementDateTime.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy HH:mm", PtBr)
                    : MovementDateTime;
            }
        }

        public string ItemNumberText
        {
            get { return ItemNumber <= 0 ? string.Empty : ItemNumber.ToString(CultureInfo.InvariantCulture); }
        }

        public string QuantityText
        {
            get { return Quantity.ToString("N2", PtBr); }
        }
    }
}
