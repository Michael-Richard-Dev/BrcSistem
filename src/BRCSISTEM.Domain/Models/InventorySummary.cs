using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InventorySummary
    {
        public string Number { get; set; }

        public string Status { get; set; }

        public string CreatedAt { get; set; }

        public string OpenedAt { get; set; }

        public string ClosedAt { get; set; }

        public string FinalizedAt { get; set; }

        public int ItemCount { get; set; }

        public int OpenPointCount { get; set; }

        public string LockedBy { get; set; }

        public string CreatedAtDisplay
        {
            get { return FormatDateTime(CreatedAt); }
        }

        public string OpenedAtDisplay
        {
            get { return FormatDateTime(OpenedAt); }
        }

        public string ClosedAtDisplay
        {
            get { return FormatDateTime(ClosedAt); }
        }

        public string FinalizedAtDisplay
        {
            get { return FormatDateTime(FinalizedAt); }
        }

        private static string FormatDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm" };
            DateTime parsed;
            return DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))
                : value;
        }
    }
}
