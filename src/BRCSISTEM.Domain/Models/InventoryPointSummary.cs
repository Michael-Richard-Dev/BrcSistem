using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InventoryPointSummary
    {
        public int Id { get; set; }

        public string PointName { get; set; }

        public string IpAddress { get; set; }

        public string ComputerName { get; set; }

        public string OpenedBy { get; set; }

        public string ClosedBy { get; set; }

        public string Status { get; set; }

        public string OpenedAt { get; set; }

        public string ClosedAt { get; set; }

        public string HeartbeatAt { get; set; }

        public string OpenedAtDisplay
        {
            get { return FormatDateTime(OpenedAt); }
        }

        public string ClosedAtDisplay
        {
            get { return FormatDateTime(ClosedAt); }
        }

        public string HeartbeatAtDisplay
        {
            get { return FormatDateTime(HeartbeatAt); }
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
