using System;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class ProductionOutputReportService
    {
        private readonly IMasterDataGateway _masterDataGateway;
        private readonly IProductionOutputReportGateway _productionOutputReportGateway;
        private readonly IAuditTrailService _auditTrailService;

        public ProductionOutputReportService(
            IMasterDataGateway masterDataGateway,
            IProductionOutputReportGateway productionOutputReportGateway,
            IAuditTrailService auditTrailService)
        {
            _masterDataGateway = masterDataGateway;
            _productionOutputReportGateway = productionOutputReportGateway;
            _auditTrailService = auditTrailService;
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _productionOutputReportGateway.LoadWarehousesForUser(profile, GetSettings(configuration, profile), NormalizeActor(userName))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public ProductSummary[] LoadProducts(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _masterDataGateway.LoadProducts(profile, GetSettings(configuration, profile))
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public ProductionOutputReportEntry[] SearchEntries(AppConfiguration configuration, DatabaseProfile profile, ProductionOutputReportQuery query)
        {
            var normalized = NormalizeQuery(query);
            return _productionOutputReportGateway.SearchEntries(profile, GetSettings(configuration, profile), normalized)
                .OrderByDescending(item => ParseMovementDate(item.MovementDateTime))
                .ThenByDescending(item => ParseOutputSequence(item.Number))
                .ThenBy(item => item.ProductDescription ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.MaterialDescription ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.MaterialCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public ProductionOutputReportDocument LoadDocument(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var document = _productionOutputReportGateway.LoadDocument(
                profile,
                GetSettings(configuration, profile),
                NormalizeOutputNumber(number));
            if (document == null)
            {
                throw new InvalidOperationException("Saida de producao nao encontrada para o relatorio.");
            }

            return document;
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, ProductionOutputReportQuery query, int rowCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Exportacao",
                "Tela=RelatorioSaidaProducaoPdf; Acao=CSV; Registros=" + rowCount
                + "; Filtros=" + FormatQueryForAudit(NormalizeQuery(query)),
                GetSettings(configuration, profile));
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, int itemCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Geracao PDF",
                "Tela=RelatorioSaidaProducaoPdf; Documento=" + NormalizeOutputNumber(number)
                + "; Itens=" + itemCount,
                GetSettings(configuration, profile));
        }

        private static ProductionOutputReportQuery NormalizeQuery(ProductionOutputReportQuery query)
        {
            if (query == null)
            {
                query = new ProductionOutputReportQuery();
            }

            var normalized = new ProductionOutputReportQuery
            {
                StartDate = NormalizeDate(query.StartDate),
                EndDate = NormalizeDate(query.EndDate),
                OutputNumber = NormalizeOutputNumber(query.OutputNumber, true),
                WarehouseCode = NormalizeReferenceCode(query.WarehouseCode),
                ProductCode = NormalizeReferenceCode(query.ProductCode),
                Shift = NormalizeShift(query.Shift),
                UserName = NormalizeText(query.UserName),
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

        private static string NormalizeOutputNumber(string value, bool allowEmpty = false)
        {
            var normalized = NormalizeText(value).Replace(" ", string.Empty).ToUpperInvariant();
            if (normalized.Length == 0 && !allowEmpty)
            {
                throw new InvalidOperationException("Informe a saida de producao.");
            }

            return normalized;
        }

        private static string NormalizeReferenceCode(string value)
        {
            return NormalizeText(value).ToUpperInvariant();
        }

        private static string NormalizeShift(string value)
        {
            var normalized = NormalizeText(value);
            if (normalized.Length == 0 || string.Equals(normalized, "TODOS", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (normalized.StartsWith("1", StringComparison.OrdinalIgnoreCase))
            {
                return "1o TURNO";
            }

            if (normalized.StartsWith("2", StringComparison.OrdinalIgnoreCase))
            {
                return "2o TURNO";
            }

            return normalized.ToUpperInvariant();
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

        private static int ParseOutputSequence(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return int.MinValue;
            }

            var digits = new string(value.Where(char.IsDigit).ToArray());
            int parsed;
            return int.TryParse(digits, out parsed) ? parsed : int.MinValue;
        }

        private static string FormatQueryForAudit(ProductionOutputReportQuery query)
        {
            return "DtIni=" + (query.StartDate ?? string.Empty)
                + "; DtFim=" + (query.EndDate ?? string.Empty)
                + "; Saida=" + (query.OutputNumber ?? string.Empty)
                + "; Almoxarifado=" + (query.WarehouseCode ?? string.Empty)
                + "; Produto=" + (query.ProductCode ?? string.Empty)
                + "; Turno=" + (query.Shift ?? string.Empty)
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
