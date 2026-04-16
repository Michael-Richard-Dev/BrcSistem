using System;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class InventoryReportService
    {
        private readonly IInventoryReportGateway _inventoryReportGateway;
        private readonly IAuditTrailService _auditTrailService;

        public InventoryReportService(IInventoryReportGateway inventoryReportGateway, IAuditTrailService auditTrailService)
        {
            _inventoryReportGateway = inventoryReportGateway;
            _auditTrailService = auditTrailService;
        }

        public InventoryReportEntry[] LoadInventories(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _inventoryReportGateway.LoadInventories(profile, GetSettings(configuration, profile))
                .OrderByDescending(item => ParseStoredDate(item.ReferenceDateTime))
                .ThenByDescending(item => ExtractNumericSuffix(item.Number))
                .ThenByDescending(item => item.Number ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public InventoryReportDocument LoadDocument(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var document = _inventoryReportGateway.LoadDocument(
                profile,
                GetSettings(configuration, profile),
                NormalizeInventoryNumber(number));

            if (document == null)
            {
                throw new InvalidOperationException("Inventario nao encontrado para o relatorio.");
            }

            return document;
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, int itemCount, int movementCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Exportacao",
                "Tela=RelatorioInventarioPdf; Acao=CSV; Documento=" + NormalizeInventoryNumber(number)
                + "; Itens=" + itemCount
                + "; Movimentos=" + movementCount,
                GetSettings(configuration, profile));
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, int divergentItemCount, int movementCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Geracao PDF",
                "Tela=RelatorioInventarioPdf; Documento=" + NormalizeInventoryNumber(number)
                + "; Divergencias=" + divergentItemCount
                + "; Movimentos=" + movementCount,
                GetSettings(configuration, profile));
        }

        private static string NormalizeInventoryNumber(string value)
        {
            var normalized = NormalizeText(value).Replace(" ", string.Empty).ToUpperInvariant();
            if (normalized.Length == 0)
            {
                throw new InvalidOperationException("Selecione um inventario.");
            }

            return normalized;
        }

        private static string NormalizeActor(string value)
        {
            var normalized = NormalizeText(value);
            return normalized.Length == 0 ? "sistema" : normalized;
        }

        private static string NormalizeText(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static DateTime ParseStoredDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DateTime.MinValue;
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm" };
            return DateTime.TryParseExact(value.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static int ExtractNumericSuffix(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return int.MinValue;
            }

            var digits = new string(value.Where(char.IsDigit).ToArray());
            int parsed;
            return int.TryParse(digits, out parsed) ? parsed : int.MinValue;
        }

        private static ConnectionResilienceSettings GetSettings(AppConfiguration configuration, DatabaseProfile profile)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (profile == null)
            {
                throw new InvalidOperationException("Banco de dados nao informado.");
            }

            configuration.Normalize();
            return configuration.ConnectionSettings ?? ConnectionResilienceSettings.CreateDefault();
        }

        private void SafeAudit(DatabaseProfile profile, string userName, string action, string details, ConnectionResilienceSettings settings)
        {
            try
            {
                _auditTrailService.Write(profile, userName, action, details, settings);
            }
            catch
            {
            }
        }
    }
}
