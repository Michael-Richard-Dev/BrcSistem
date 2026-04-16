using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InventoryReportEntry
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string Number { get; set; }

        public string Status { get; set; }

        public string CreatedAt { get; set; }

        public string FinalizedAt { get; set; }

        public string ReferenceDateTime
        {
            get { return string.IsNullOrWhiteSpace(FinalizedAt) ? CreatedAt : FinalizedAt; }
        }

        public string ReferenceDateDisplay
        {
            get { return FormatDateTime(ReferenceDateTime); }
        }

        public string DisplayText
        {
            get
            {
                return (Number ?? string.Empty)
                    + " | "
                    + (string.IsNullOrWhiteSpace(Status) ? "-" : Status)
                    + " | "
                    + (string.IsNullOrWhiteSpace(ReferenceDateDisplay) ? "-" : ReferenceDateDisplay);
            }
        }

        public override string ToString()
        {
            return DisplayText;
        }

        private static string FormatDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm" };
            return DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", PtBr)
                : value;
        }
    }
}
