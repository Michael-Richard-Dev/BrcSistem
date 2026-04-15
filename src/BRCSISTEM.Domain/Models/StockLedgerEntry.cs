using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockLedgerEntry
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string MovementDateTime { get; set; }

        public string DocumentNumber { get; set; }

        public string DocumentType { get; set; }

        public string MovementType { get; set; }

        public string MaterialDisplay { get; set; }

        public string LotDisplay { get; set; }

        public string ExpirationDate { get; set; }

        public string WarehouseDisplay { get; set; }

        public string SupplierDisplay { get; set; }

        public decimal Quantity { get; set; }

        public string UserName { get; set; }

        public string Status { get; set; }

        public int MovementId { get; set; }

        public string CreatedAt { get; set; }

        public decimal RunningBalance { get; set; }

        public string DisplayType
        {
            get
            {
                return string.Equals(DocumentType, "INVENTARIO", StringComparison.OrdinalIgnoreCase)
                    ? "INVENTARIO"
                    : (MovementType ?? string.Empty);
            }
        }

        public string DocumentDisplay
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DocumentType))
                {
                    return DocumentNumber ?? string.Empty;
                }

                return string.IsNullOrWhiteSpace(DocumentNumber)
                    ? "(" + DocumentType + ")"
                    : DocumentNumber + " (" + DocumentType + ")";
            }
        }

        public string MovementDateTimeDisplay
        {
            get { return FormatDateTime(MovementDateTime); }
        }

        public string ExpirationDateDisplay
        {
            get { return FormatDate(ExpirationDate); }
        }

        public string CreatedAtDisplay
        {
            get { return FormatDateTime(CreatedAt); }
        }

        public string QuantityText
        {
            get { return Quantity.ToString("N2", PtBr); }
        }

        public string RunningBalanceText
        {
            get { return RunningBalance.ToString("N2", PtBr); }
        }

        public bool IsInactive
        {
            get { return !string.Equals(Status, "ATIVO", StringComparison.OrdinalIgnoreCase); }
        }

        public string RowCategory
        {
            get
            {
                if (IsInactive)
                {
                    return "INATIVO";
                }

                if (string.Equals(DisplayType, "INVENTARIO", StringComparison.OrdinalIgnoreCase))
                {
                    return "INVENTARIO";
                }

                if ((DisplayType ?? string.Empty).IndexOf("ENTRADA", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "ENTRADA";
                }

                if ((DisplayType ?? string.Empty).IndexOf("SAIDA", StringComparison.OrdinalIgnoreCase) >= 0
                    || string.Equals(DisplayType, "REQUISICAO", StringComparison.OrdinalIgnoreCase))
                {
                    return "SAIDA";
                }

                if ((DisplayType ?? string.Empty).IndexOf("TRANSFERENCIA", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return "TRANSFERENCIA";
                }

                return string.Empty;
            }
        }

        private static string FormatDate(string value)
        {
            if (!TryParseDate(value, out var parsed))
            {
                return value ?? string.Empty;
            }

            return parsed.ToString("dd/MM/yyyy", PtBr);
        }

        private static string FormatDateTime(string value)
        {
            if (!TryParseDate(value, out var parsed))
            {
                return value ?? string.Empty;
            }

            return parsed.ToString("dd/MM/yyyy HH:mm", PtBr);
        }

        private static bool TryParseDate(string value, out DateTime parsed)
        {
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "dd/MM/yyyy HH:mm", "dd/MM/yyyy" };
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                || DateTime.TryParse((value ?? string.Empty).Trim(), PtBr, DateTimeStyles.None, out parsed)
                || DateTime.TryParse((value ?? string.Empty).Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
        }
    }
}
