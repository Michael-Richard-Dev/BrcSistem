using System;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class StockSummaryService
    {
        private readonly IStockSummaryGateway _stockSummaryGateway;
        private readonly IAuditTrailService _auditTrailService;

        public StockSummaryService(IStockSummaryGateway stockSummaryGateway, IAuditTrailService auditTrailService)
        {
            _stockSummaryGateway = stockSummaryGateway;
            _auditTrailService = auditTrailService;
        }

        public WarehouseSummary[] LoadWarehouses(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockSummaryGateway.LoadWarehouses(profile, GetSettings(configuration, profile))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockSummaryGateway.LoadMaterials(profile, GetSettings(configuration, profile))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockSummaryGateway.LoadLots(profile, GetSettings(configuration, profile))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public StockSummaryEntry[] LoadEntries(AppConfiguration configuration, DatabaseProfile profile, StockSummaryQuery query)
        {
            var normalized = NormalizeQuery(query);
            return _stockSummaryGateway.LoadEntries(profile, GetSettings(configuration, profile), normalized)
                .OrderBy(item => item.WarehouseCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.MaterialCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.LotCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockSummaryQuery query, int rowCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Exportacao",
                "Tela=ResumoSintetico; Acao=CSV; Registros=" + rowCount + "; Filtros=" + FormatQueryForAudit(NormalizeQuery(query)),
                GetSettings(configuration, profile));
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockSummaryQuery query, int rowCount)
        {
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Geracao PDF",
                "Tela=ResumoSintetico; Acao=PDF; Registros=" + rowCount + "; Filtros=" + FormatQueryForAudit(NormalizeQuery(query)),
                GetSettings(configuration, profile));
        }

        private static StockSummaryQuery NormalizeQuery(StockSummaryQuery query)
        {
            if (query == null)
            {
                query = new StockSummaryQuery();
            }

            return new StockSummaryQuery
            {
                ReferenceDate = NormalizeDate(query.ReferenceDate),
                WarehouseCode = NormalizeText(query.WarehouseCode),
                MaterialCode = NormalizeText(query.MaterialCode),
                LotCode = NormalizeText(query.LotCode),
            };
        }

        private static string NormalizeDate(string value)
        {
            var trimmed = NormalizeText(value);
            if (trimmed.Length == 0)
            {
                throw new InvalidOperationException("Informe a data de referencia no formato dd/MM/yyyy.");
            }

            DateTime parsed;
            if (!DateTime.TryParseExact(trimmed, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                throw new InvalidOperationException("Informe a data de referencia no formato dd/MM/yyyy.");
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

        private static string FormatQueryForAudit(StockSummaryQuery query)
        {
            return "DataRef=" + (query.ReferenceDate ?? string.Empty)
                + "; Almox=" + (query.WarehouseCode ?? string.Empty)
                + "; Material=" + (query.MaterialCode ?? string.Empty)
                + "; Lote=" + (query.LotCode ?? string.Empty);
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
