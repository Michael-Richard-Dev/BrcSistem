using System;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class InboundReceiptReportService
    {
        private readonly IInboundReceiptReportGateway _inboundReceiptReportGateway;
        private readonly IAuditTrailService _auditTrailService;

        public InboundReceiptReportService(IInboundReceiptReportGateway inboundReceiptReportGateway, IAuditTrailService auditTrailService)
        {
            _inboundReceiptReportGateway = inboundReceiptReportGateway;
            _auditTrailService = auditTrailService;
        }

        public SupplierSummary[] LoadSuppliers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _inboundReceiptReportGateway.LoadSuppliers(profile, GetSettings(configuration, profile))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public InboundReceiptReportEntry[] SearchEntries(AppConfiguration configuration, DatabaseProfile profile, InboundReceiptReportQuery query)
        {
            var normalized = NormalizeQuery(query);
            return _inboundReceiptReportGateway.SearchEntries(profile, GetSettings(configuration, profile), normalized)
                .OrderByDescending(item => ParseMovementDate(item.ReceiptDateTime))
                .ThenByDescending(item => ParseNumeric(item.Number))
                .ThenBy(item => item.MaterialName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.MaterialCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public InboundReceiptReportDocument LoadDocument(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode)
        {
            var document = _inboundReceiptReportGateway.LoadDocument(
                profile,
                GetSettings(configuration, profile),
                NormalizeReceiptNumber(number),
                NormalizeRequiredReferenceCode(supplierCode, "fornecedor"));
            if (document == null)
            {
                throw new InvalidOperationException("Nota nao encontrada para o relatorio.");
            }

            return document;
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, InboundReceiptReportQuery query, int rowCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Exportacao",
                "Tela=RelatorioEntradaPdf; Acao=CSV; Registros=" + rowCount
                + "; Filtros=" + FormatQueryForAudit(NormalizeQuery(query)),
                GetSettings(configuration, profile));
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, string supplierCode, int itemCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Geracao PDF",
                "Tela=RelatorioEntradaPdf; Documento=" + NormalizeReceiptNumber(number)
                + "; Fornecedor=" + NormalizeRequiredReferenceCode(supplierCode, "fornecedor")
                + "; Itens=" + itemCount,
                GetSettings(configuration, profile));
        }

        private static InboundReceiptReportQuery NormalizeQuery(InboundReceiptReportQuery query)
        {
            if (query == null)
            {
                query = new InboundReceiptReportQuery();
            }

            var normalized = new InboundReceiptReportQuery
            {
                StartDate = NormalizeDate(query.StartDate),
                EndDate = NormalizeDate(query.EndDate),
                ReceiptNumber = NormalizeReceiptNumber(query.ReceiptNumber, allowEmpty: true),
                SupplierCode = NormalizeReferenceCode(query.SupplierCode),
                ExcludeCanceled = query.ExcludeCanceled,
            };

            if (!string.IsNullOrWhiteSpace(normalized.StartDate)
                && !string.IsNullOrWhiteSpace(normalized.EndDate)
                && ParseStoredDate(normalized.EndDate) < ParseStoredDate(normalized.StartDate))
            {
                throw new InvalidOperationException("A data final nao pode ser menor que a data inicial.");
            }

            return normalized;
        }

        private static string NormalizeDate(string value)
        {
            var trimmed = NormalizeText(value);
            if (trimmed.Length == 0)
            {
                return string.Empty;
            }

            DateTime parsed;
            if (!DateTime.TryParseExact(trimmed, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                throw new InvalidOperationException("Informe uma data valida no formato dd/MM/yyyy.");
            }

            return parsed.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private static string NormalizeReceiptNumber(string value, bool allowEmpty = false)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length == 0 && !allowEmpty)
            {
                throw new InvalidOperationException("Informe a nota fiscal.");
            }

            return digits;
        }

        private static string NormalizeRequiredReferenceCode(string value, string description)
        {
            var normalized = NormalizeReferenceCode(value);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                throw new InvalidOperationException("Selecione o " + description + ".");
            }

            return normalized;
        }

        private static string NormalizeReferenceCode(string value)
        {
            return NormalizeText(value).ToUpperInvariant();
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
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
            DateTime parsed;
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static DateTime ParseMovementDate(string value)
        {
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "yyyy-MM-dd" };
            DateTime parsed;
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static int ParseNumeric(string value)
        {
            int parsed;
            return int.TryParse(value, out parsed) ? parsed : int.MinValue;
        }

        private static string FormatQueryForAudit(InboundReceiptReportQuery query)
        {
            return "DtIni=" + (query.StartDate ?? string.Empty)
                + "; DtFim=" + (query.EndDate ?? string.Empty)
                + "; Nota=" + (query.ReceiptNumber ?? string.Empty)
                + "; Fornecedor=" + (query.SupplierCode ?? string.Empty)
                + "; ExcluirCanceladas=" + query.ExcludeCanceled;
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
