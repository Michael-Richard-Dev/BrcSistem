using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockTransferReportEntry
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string Number { get; set; }

        public int ItemNumber { get; set; }

        public string OriginWarehouseCode { get; set; }

        public string OriginWarehouseName { get; set; }

        public string DestinationWarehouseCode { get; set; }

        public string DestinationWarehouseName { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialDescription { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public decimal Quantity { get; set; }

        public string MovementDateTime { get; set; }

        public string Status { get; set; }

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

        public string MaterialDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MaterialCode))
                {
                    return MaterialDescription ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(MaterialDescription)
                    ? MaterialCode
                    : MaterialCode + " - " + MaterialDescription;
            }
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
                return DateTime.TryParseExact(MovementDateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                    ? parsed.ToString("dd/MM/yyyy", PtBr)
                    : MovementDateTime;
            }
        }

        public string QuantityText
        {
            get { return Quantity.ToString("N2", PtBr); }
        }
    }
}
