using System;
using System.Globalization;

namespace BRCSISTEM.Domain.Models
{
    public sealed class DocumentDateEntry
    {
        public string DocumentNumber { get; set; }

        public string Supplier { get; set; }

        public string Date { get; set; }

        public string Status { get; set; }

        public string OriginWarehouse { get; set; }

        public string DestinationWarehouse { get; set; }

        // ── Campos extras usados pela tela "Alterar Data de Entrada" ──────────
        public string SupplierName { get; set; }

        public string Warehouse { get; set; }

        public string WarehouseName { get; set; }

        public int ItemCount { get; set; }

        public string Shift { get; set; }

        public string Purpose { get; set; }

        public string OriginWarehouseName { get; set; }

        public string DestinationWarehouseName { get; set; }

        public string DisplayLabel => string.IsNullOrWhiteSpace(Supplier)
            ? DocumentNumber
            : $"{DocumentNumber} / {Supplier}";

        public string TransferDisplayLabel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DocumentNumber))
                {
                    return string.Empty;
                }

                return DocumentNumber + " - " + FormatDateTime(Date);
            }
        }

        public string ProductionOutputDisplayLabel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DocumentNumber))
                {
                    return string.Empty;
                }

                return DocumentNumber + " - " + FormatDateTime(Date);
            }
        }

        // Espelha "{numero} - {forn_nome} - {data_br}" de
        // views/bd_alterar_data_entrada.py::_carregar_notas.
        public string NoteDisplayLabel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DocumentNumber))
                {
                    return string.Empty;
                }

                return DocumentNumber + " - " + (SupplierName ?? string.Empty) + " - " + FormatDateTime(Date);
            }
        }

        private static string FormatDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "yyyy-MM-dd" };
            return DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))
                : value;
        }
    }
}
