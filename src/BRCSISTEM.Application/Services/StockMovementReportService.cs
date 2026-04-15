using System;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class StockMovementReportService
    {
        private readonly IStockMovementReportGateway _stockMovementReportGateway;
        private readonly IAuditTrailService _auditTrailService;

        public StockMovementReportService(IStockMovementReportGateway stockMovementReportGateway, IAuditTrailService auditTrailService)
        {
            _stockMovementReportGateway = stockMovementReportGateway;
            _auditTrailService = auditTrailService;
        }

        public WarehouseSummary[] LoadWarehouses(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockMovementReportGateway.LoadWarehouses(profile, GetSettings(configuration, profile))
                .OrderBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockMovementReportGateway.LoadMaterials(profile, GetSettings(configuration, profile))
                .OrderBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockMovementReportGateway.LoadLots(profile, GetSettings(configuration, profile))
                .OrderBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public StockMovementReportRow[] LoadRows(AppConfiguration configuration, DatabaseProfile profile, StockMovementReportQuery query)
        {
            var normalized = NormalizeQuery(query);
            var rows = _stockMovementReportGateway.SearchRows(profile, GetSettings(configuration, profile), normalized)
                .OrderBy(item => item.WarehouseCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.MaterialCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.LotCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (normalized.OnlyRowsWithMovementOrPositiveBalance)
            {
                rows = rows
                    .Where(item => item.HasMovement || item.OpeningBalance > 0M || item.FinalBalance > 0M)
                    .ToArray();
            }

            return rows;
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockMovementReportQuery query, int rowCount)
        {
            var normalized = NormalizeQuery(query);
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Exportacao",
                "Tela=RelatorioMovimentacaoEstoque; Acao=CSV; Registros=" + rowCount
                + "; Filtros=" + FormatQueryForAudit(normalized),
                GetSettings(configuration, profile));
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockMovementReportQuery query, int rowCount)
        {
            var normalized = NormalizeQuery(query);
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Geracao PDF",
                "Tela=RelatorioMovimentacaoEstoque; Acao=PDF; Registros=" + rowCount
                + "; Filtros=" + FormatQueryForAudit(normalized),
                GetSettings(configuration, profile));
        }

        private static StockMovementReportQuery NormalizeQuery(StockMovementReportQuery query)
        {
            if (query == null)
            {
                query = new StockMovementReportQuery();
            }

            var normalized = new StockMovementReportQuery
            {
                StartDate = NormalizeDate(query.StartDate),
                EndDate = NormalizeDate(query.EndDate),
                WarehouseCode = NormalizeText(query.WarehouseCode),
                MaterialCode = NormalizeText(query.MaterialCode),
                LotCode = NormalizeText(query.LotCode),
                OnlyRowsWithMovementOrPositiveBalance = query.OnlyRowsWithMovementOrPositiveBalance,
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

        private static string NormalizeText(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static string NormalizeActor(string value)
        {
            var normalized = NormalizeText(value);
            return normalized.Length == 0 ? "sistema" : normalized;
        }

        private static DateTime ParseStoredDate(string value)
        {
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
            DateTime parsed;
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static string FormatQueryForAudit(StockMovementReportQuery query)
        {
            return "DtIni=" + (query.StartDate ?? string.Empty)
                + "; DtFim=" + (query.EndDate ?? string.Empty)
                + "; Almox=" + (query.WarehouseCode ?? string.Empty)
                + "; Material=" + (query.MaterialCode ?? string.Empty)
                + "; Lote=" + (query.LotCode ?? string.Empty)
                + "; SomenteMovOuSaldo=" + query.OnlyRowsWithMovementOrPositiveBalance;
        }

        private static ConnectionResilienceSettings GetSettings(AppConfiguration configuration, DatabaseProfile profile)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return configuration.ConnectionSettings ?? new ConnectionResilienceSettings();
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
