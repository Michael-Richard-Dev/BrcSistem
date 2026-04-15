using System;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class StockTransferReportService
    {
        private readonly IMasterDataGateway _masterDataGateway;
        private readonly IStockTransferReportGateway _stockTransferReportGateway;
        private readonly IAuditTrailService _auditTrailService;

        public StockTransferReportService(
            IMasterDataGateway masterDataGateway,
            IStockTransferReportGateway stockTransferReportGateway,
            IAuditTrailService auditTrailService)
        {
            _masterDataGateway = masterDataGateway;
            _stockTransferReportGateway = stockTransferReportGateway;
            _auditTrailService = auditTrailService;
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _stockTransferReportGateway.LoadWarehousesForUser(profile, GetSettings(configuration, profile), NormalizeActor(userName))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _masterDataGateway.LoadPackagings(profile, GetSettings(configuration, profile))
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public StockTransferReportEntry[] SearchEntries(AppConfiguration configuration, DatabaseProfile profile, StockTransferReportQuery query)
        {
            var normalized = NormalizeQuery(query);
            return _stockTransferReportGateway.SearchEntries(profile, GetSettings(configuration, profile), normalized)
                .OrderByDescending(item => ParseMovementDate(item.MovementDateTime))
                .ThenByDescending(item => ParseTransferSequence(item.Number))
                .ThenBy(item => item.ItemNumber)
                .ThenBy(item => item.MaterialDescription ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.MaterialCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public StockTransferReportDocument LoadDocument(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var document = _stockTransferReportGateway.LoadDocument(
                profile,
                GetSettings(configuration, profile),
                NormalizeTransferNumber(number));
            if (document == null)
            {
                throw new InvalidOperationException("Transferencia nao encontrada para o relatorio.");
            }

            return document;
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockTransferReportQuery query, int rowCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Exportacao",
                "Tela=RelatorioTransferenciaPdf; Acao=CSV; Registros=" + rowCount
                + "; Filtros=" + FormatQueryForAudit(NormalizeQuery(query)),
                GetSettings(configuration, profile));
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, string number, int itemCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Geracao PDF",
                "Tela=RelatorioTransferenciaPdf; Documento=" + NormalizeTransferNumber(number)
                + "; Itens=" + itemCount,
                GetSettings(configuration, profile));
        }

        private static StockTransferReportQuery NormalizeQuery(StockTransferReportQuery query)
        {
            if (query == null)
            {
                query = new StockTransferReportQuery();
            }

            var normalized = new StockTransferReportQuery
            {
                StartDate = NormalizeDate(query.StartDate),
                EndDate = NormalizeDate(query.EndDate),
                TransferNumber = NormalizeTransferNumber(query.TransferNumber, true),
                OriginWarehouseCode = NormalizeReferenceCode(query.OriginWarehouseCode),
                DestinationWarehouseCode = NormalizeReferenceCode(query.DestinationWarehouseCode),
                MaterialCode = NormalizeReferenceCode(query.MaterialCode),
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

        private static string NormalizeTransferNumber(string value, bool allowEmpty = false)
        {
            var normalized = NormalizeText(value).Replace(" ", string.Empty).ToUpperInvariant();
            if (normalized.Length == 0 && !allowEmpty)
            {
                throw new InvalidOperationException("Informe a transferencia.");
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

        private static int ParseTransferSequence(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return int.MinValue;
            }

            var digits = new string(value.Where(char.IsDigit).ToArray());
            int parsed;
            return int.TryParse(digits, out parsed) ? parsed : int.MinValue;
        }

        private static string FormatQueryForAudit(StockTransferReportQuery query)
        {
            return "DtIni=" + (query.StartDate ?? string.Empty)
                + "; DtFim=" + (query.EndDate ?? string.Empty)
                + "; Transferencia=" + (query.TransferNumber ?? string.Empty)
                + "; Origem=" + (query.OriginWarehouseCode ?? string.Empty)
                + "; Destino=" + (query.DestinationWarehouseCode ?? string.Empty)
                + "; Material=" + (query.MaterialCode ?? string.Empty)
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
