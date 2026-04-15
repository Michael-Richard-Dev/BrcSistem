using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InventoryCountSummary
    {
        public int Id { get; set; }

        public int PointId { get; set; }

        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialDescription { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public decimal Quantity { get; set; }

        public string UserName { get; set; }

        public string CountedAt { get; set; }

        public string ItemDisplay
        {
            get
            {
                var material = string.IsNullOrWhiteSpace(MaterialDescription)
                    ? MaterialCode ?? string.Empty
                    : (MaterialCode ?? string.Empty) + " - " + MaterialDescription;
                return (WarehouseCode ?? string.Empty) + "/" + material + "/" + (LotCode ?? string.Empty);
            }
        }

        public string QuantityText
        {
            get { return Quantity.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
        }

        public string CountedAtDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CountedAt))
                {
                    return string.Empty;
                }

                var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm" };
                DateTime parsed;
                return DateTime.TryParseExact(CountedAt.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))
                    : CountedAt;
            }
        }
    }
}
