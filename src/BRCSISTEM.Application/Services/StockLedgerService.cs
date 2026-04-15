using System;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class StockLedgerService
    {
        private static readonly string[] PositiveMovementTypes = { "ENTRADA", "TRANSFERENCIA_ENTRADA" };
        private static readonly string[] NegativeMovementTypes = { "SAIDA", "REQUISICAO", "TRANSFERENCIA_SAIDA", "SAIDA_PRODUCAO" };

        private readonly IStockLedgerGateway _stockLedgerGateway;
        private readonly IAuditTrailService _auditTrailService;

        public StockLedgerService(IStockLedgerGateway stockLedgerGateway, IAuditTrailService auditTrailService)
        {
            _stockLedgerGateway = stockLedgerGateway;
            _auditTrailService = auditTrailService;
        }

        public SupplierSummary[] LoadSuppliers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _stockLedgerGateway.LoadSuppliers(profile, GetSettings(configuration, profile))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile, string supplierCode)
        {
            return _stockLedgerGateway.LoadMaterials(profile, GetSettings(configuration, profile), NormalizeReferenceCode(supplierCode))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public WarehouseSummary[] LoadWarehouses(AppConfiguration configuration, DatabaseProfile profile, string supplierCode)
        {
            return _stockLedgerGateway.LoadWarehouses(profile, GetSettings(configuration, profile), NormalizeReferenceCode(supplierCode))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile, string materialCode, string supplierCode)
        {
            return _stockLedgerGateway.LoadLots(
                    profile,
                    GetSettings(configuration, profile),
                    NormalizeReferenceCode(materialCode),
                    NormalizeReferenceCode(supplierCode))
                .OrderBy(item => ParseStoredDate(item.ExpirationDate))
                .ThenBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public StockLedgerEntry[] LoadEntries(AppConfiguration configuration, DatabaseProfile profile, StockLedgerQuery query)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeQuery(query);
            var entries = _stockLedgerGateway.SearchEntries(profile, settings, normalized)
                .OrderBy(item => ParseStoredDate(item.MovementDateTime))
                .ThenBy(item => item.MovementId)
                .ToArray();

            var runningBalance = _stockLedgerGateway.GetInitialBalance(profile, settings, normalized);
            foreach (var entry in entries)
            {
                if (PositiveMovementTypes.Contains(entry.MovementType ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                {
                    runningBalance += entry.Quantity;
                }
                else if (NegativeMovementTypes.Contains(entry.MovementType ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                {
                    runningBalance -= entry.Quantity;
                }

                entry.RunningBalance = runningBalance;
            }

            return entries
                .OrderByDescending(item => ParseStoredDate(item.MovementDateTime))
                .ThenByDescending(item => item.MovementId)
                .ToArray();
        }

        public void RegisterCsvExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockLedgerQuery query, int rowCount)
        {
            var normalized = NormalizeQuery(query, allowEmptyDateRange: true);
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Exportacao",
                "Tela=ContaCorrenteEstoque; Acao=CSV; Registros=" + rowCount
                + "; Filtros=" + FormatQueryForAudit(normalized),
                GetSettings(configuration, profile));
        }

        public void RegisterPdfExport(AppConfiguration configuration, DatabaseProfile profile, string userName, StockLedgerQuery query, int rowCount)
        {
            var normalized = NormalizeQuery(query, allowEmptyDateRange: true);
            SafeAudit(
                profile,
                NormalizeActor(userName),
                "Geracao PDF",
                "Tela=ContaCorrenteEstoque; Acao=PDF; Registros=" + rowCount
                + "; Filtros=" + FormatQueryForAudit(normalized),
                GetSettings(configuration, profile));
        }

        private static StockLedgerQuery NormalizeQuery(StockLedgerQuery query, bool allowEmptyDateRange = false)
        {
            if (query == null)
            {
                query = new StockLedgerQuery();
            }

            var normalized = new StockLedgerQuery
            {
                StartDate = NormalizeDate(query.StartDate),
                EndDate = NormalizeDate(query.EndDate),
                SupplierCode = NormalizeReferenceCode(query.SupplierCode),
                MaterialCode = NormalizeReferenceCode(query.MaterialCode),
                LotCode = NormalizeReferenceCode(query.LotCode),
                WarehouseCode = NormalizeReferenceCode(query.WarehouseCode),
                MovementType = NormalizeMovementType(query.MovementType),
                IncludeInactive = query.IncludeInactive,
            };

            if (!allowEmptyDateRange
                && !string.IsNullOrWhiteSpace(normalized.StartDate)
                && !string.IsNullOrWhiteSpace(normalized.EndDate)
                && ParseStoredDate(normalized.EndDate) < ParseStoredDate(normalized.StartDate))
            {
                throw new InvalidOperationException("A data final nao pode ser menor que a data inicial.");
            }

            return normalized;
        }

        private static string NormalizeDate(string value)
        {
            var trimmed = (value ?? string.Empty).Trim();
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

        private static string NormalizeMovementType(string value)
        {
            var normalized = NormalizeText(value).ToUpperInvariant();
            if (normalized.Length == 0)
            {
                return string.Empty;
            }

            var allowed = new[]
            {
                "INVENTARIO",
                "ENTRADA",
                "TRANSFERENCIA_ENTRADA",
                "TRANSFERENCIA_SAIDA",
                "REQUISICAO",
                "SAIDA_PRODUCAO",
                "SAIDA",
            };

            if (!allowed.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Tipo de movimento invalido para a consulta.");
            }

            return normalized;
        }

        private static string NormalizeReferenceCode(string value)
        {
            return NormalizeText(value);
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
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "yyyy-MM-dd" };
            DateTime parsed;
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static string FormatQueryForAudit(StockLedgerQuery query)
        {
            return "DtIni=" + (query.StartDate ?? string.Empty)
                + "; DtFim=" + (query.EndDate ?? string.Empty)
                + "; Fornecedor=" + (query.SupplierCode ?? string.Empty)
                + "; Material=" + (query.MaterialCode ?? string.Empty)
                + "; Lote=" + (query.LotCode ?? string.Empty)
                + "; Almox=" + (query.WarehouseCode ?? string.Empty)
                + "; Tipo=" + (query.MovementType ?? string.Empty)
                + "; Inativos=" + query.IncludeInactive;
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
