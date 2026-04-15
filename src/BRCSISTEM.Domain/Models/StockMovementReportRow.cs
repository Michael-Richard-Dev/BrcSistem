using System;
using System.Globalization;
using System.Linq;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockMovementReportRow
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string LotCode { get; set; }

        public string LotName { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }

        public string ExpirationDate { get; set; }

        public decimal OpeningBalance { get; set; }

        public decimal Entries { get; set; }

        public decimal TransferIn { get; set; }

        public decimal TransferOut { get; set; }

        public decimal ProductionOutput { get; set; }

        public decimal Requisition { get; set; }

        public decimal Inventory { get; set; }

        public decimal FinalBalance { get; set; }

        public bool IsTotalRow { get; set; }

        public string WarehouseDisplay
        {
            get { return IsTotalRow ? string.Empty : (WarehouseCode ?? string.Empty); }
        }

        public string LotDisplay
        {
            get { return IsTotalRow ? string.Empty : BuildDisplay(LotCode, LotName); }
        }

        public string MaterialDisplay
        {
            get { return IsTotalRow ? "*** TOTAL GERAL ***" : BuildDisplay(MaterialCode, MaterialName); }
        }

        public string ExpirationDateDisplay
        {
            get { return IsTotalRow ? string.Empty : FormatDate(ExpirationDate); }
        }

        public string OpeningBalanceText
        {
            get { return OpeningBalance.ToString("N2", PtBr); }
        }

        public string EntriesText
        {
            get { return Entries.ToString("N2", PtBr); }
        }

        public string TransferInText
        {
            get { return TransferIn.ToString("N2", PtBr); }
        }

        public string TransferOutText
        {
            get { return TransferOut.ToString("N2", PtBr); }
        }

        public string ProductionOutputText
        {
            get { return ProductionOutput.ToString("N2", PtBr); }
        }

        public string RequisitionText
        {
            get { return Requisition.ToString("N2", PtBr); }
        }

        public string InventoryText
        {
            get { return Inventory.ToString("N2", PtBr); }
        }

        public string FinalBalanceText
        {
            get { return FinalBalance.ToString("N2", PtBr); }
        }

        public bool HasMovement
        {
            get
            {
                return Entries != 0M
                    || TransferIn != 0M
                    || TransferOut != 0M
                    || ProductionOutput != 0M
                    || Requisition != 0M
                    || Inventory != 0M;
            }
        }

        public int? DaysToExpiration
        {
            get
            {
                if (IsTotalRow || string.IsNullOrWhiteSpace(ExpirationDate))
                {
                    return null;
                }

                DateTime parsed;
                if (!TryParseDate(ExpirationDate, out parsed))
                {
                    return null;
                }

                return (parsed.Date - DateTime.Today).Days;
            }
        }

        public string ExpirationRiskCategory
        {
            get
            {
                if (IsTotalRow)
                {
                    return "TOTAL";
                }

                var days = DaysToExpiration;
                if (!days.HasValue)
                {
                    return string.Empty;
                }

                if (days.Value <= 15)
                {
                    return "CRITICO";
                }

                if (days.Value <= 30)
                {
                    return "ALERTA";
                }

                if (days.Value <= 45)
                {
                    return "ATENCAO";
                }

                return string.Empty;
            }
        }

        public static StockMovementReportRow CreateTotal(StockMovementReportRow[] rows)
        {
            rows = rows ?? new StockMovementReportRow[0];
            return new StockMovementReportRow
            {
                IsTotalRow = true,
                OpeningBalance = rows.Sum(item => item.OpeningBalance),
                Entries = rows.Sum(item => item.Entries),
                TransferIn = rows.Sum(item => item.TransferIn),
                TransferOut = rows.Sum(item => item.TransferOut),
                ProductionOutput = rows.Sum(item => item.ProductionOutput),
                Requisition = rows.Sum(item => item.Requisition),
                Inventory = rows.Sum(item => item.Inventory),
                FinalBalance = rows.Sum(item => item.FinalBalance),
            };
        }

        private static string BuildDisplay(string code, string description)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return description ?? string.Empty;
            }

            return string.IsNullOrWhiteSpace(description)
                ? code
                : code + " - " + description;
        }

        private static string FormatDate(string value)
        {
            DateTime parsed;
            return TryParseDate(value, out parsed)
                ? parsed.ToString("dd/MM/yyyy", PtBr)
                : (value ?? string.Empty);
        }

        private static bool TryParseDate(string value, out DateTime parsed)
        {
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy" };
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                || DateTime.TryParse((value ?? string.Empty).Trim(), PtBr, DateTimeStyles.None, out parsed)
                || DateTime.TryParse((value ?? string.Empty).Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
        }
    }
}
