using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockSummaryEntry
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public string ExpirationDate { get; set; }

        public decimal Quantity { get; set; }

        public string WarehouseDisplay
        {
            get { return BuildDisplay(WarehouseCode, WarehouseName); }
        }

        public string MaterialDisplay
        {
            get { return BuildDisplay(MaterialCode, MaterialName); }
        }

        public string LotDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LotCode))
                {
                    return "SEM LOTE";
                }

                return BuildDisplay(LotCode, LotName);
            }
        }

        public string ExpirationDateDisplay
        {
            get
            {
                DateTime parsed;
                return DateTime.TryParseExact((ExpirationDate ?? string.Empty).Trim(), new[] { "yyyy-MM-dd", "dd/MM/yyyy", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy", PtBr)
                    : (ExpirationDate ?? "N/D");
            }
        }

        public string QuantityText
        {
            get { return Quantity.ToString("N2", PtBr); }
        }

        private static string BuildDisplay(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return string.Empty;
            }

            return string.IsNullOrWhiteSpace(name)
                ? code
                : code + " - " + name.Trim();
        }
    }
}
