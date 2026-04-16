using System;
using System.Globalization;
using System.Linq;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InventoryReportDocument
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public InventoryReportDocument()
        {
            Items = Array.Empty<InventoryReportItem>();
            Movements = Array.Empty<InventoryReportMovement>();
        }

        public string Number { get; set; }

        public string Status { get; set; }

        public string CreatedBy { get; set; }

        public string ClosedBy { get; set; }

        public string CanceledBy { get; set; }

        public string CancellationReason { get; set; }

        public string Observation { get; set; }

        public int MaxOpenPoints { get; set; }

        public string CreatedAt { get; set; }

        public string OpenedAt { get; set; }

        public string ClosedAt { get; set; }

        public string FinalizedAt { get; set; }

        public InventoryReportItem[] Items { get; set; }

        public InventoryReportMovement[] Movements { get; set; }

        public InventoryReportItem[] DivergentItems
        {
            get { return (Items ?? Array.Empty<InventoryReportItem>()).Where(item => item != null && item.IsDivergent).ToArray(); }
        }

        public int DivergentItemCount
        {
            get { return DivergentItems.Length; }
        }

        public decimal TotalEntryAdjustments
        {
            get { return (Items ?? Array.Empty<InventoryReportItem>()).Where(item => item != null && item.AdjustmentQuantity > 0M).Sum(item => item.AdjustmentQuantity); }
        }

        public decimal TotalOutputAdjustments
        {
            get { return (Items ?? Array.Empty<InventoryReportItem>()).Where(item => item != null && item.AdjustmentQuantity < 0M).Sum(item => Math.Abs(item.AdjustmentQuantity)); }
        }

        public string TotalEntryAdjustmentsText
        {
            get { return TotalEntryAdjustments.ToString("N2", PtBr); }
        }

        public string TotalOutputAdjustmentsText
        {
            get { return TotalOutputAdjustments.ToString("N2", PtBr); }
        }

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

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm" };
            return DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", PtBr)
                : value;
        }
    }
}
