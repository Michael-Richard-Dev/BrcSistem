using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class StockTransferService
    {
        private readonly IStockTransferGateway _stockTransferGateway;
        private readonly IAuditTrailService _auditTrailService;

        public StockTransferService(IStockTransferGateway stockTransferGateway, IAuditTrailService auditTrailService)
        {
            _stockTransferGateway = stockTransferGateway;
            _auditTrailService = auditTrailService;
        }

        public string GenerateNextTransferNumber(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _stockTransferGateway.GenerateNextTransferNumber(profile, settings);
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _stockTransferGateway.LoadWarehousesForUser(profile, settings, NormalizeActor(userName))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterialsByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime)
        {
            var settings = GetSettings(configuration, profile);
            return _stockTransferGateway.LoadMaterialsByWarehouse(
                    profile,
                    settings,
                    NormalizeReferenceCode(warehouseCode),
                    NormalizeMovementDateInput(movementDateTime))
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public LotSummary[] LoadLotsByWarehouseAndMaterial(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string materialCode, string movementDateTime)
        {
            var settings = GetSettings(configuration, profile);
            return _stockTransferGateway.LoadLotsByWarehouseAndMaterial(
                    profile,
                    settings,
                    NormalizeReferenceCode(warehouseCode),
                    NormalizeReferenceCode(materialCode),
                    NormalizeMovementDateInput(movementDateTime))
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => ParseStoredDate(item.ExpirationDate))
                .ThenBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public decimal GetAvailableStockBalance(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedTransferNumber)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(materialCode)
                || string.IsNullOrWhiteSpace(lotCode)
                || string.IsNullOrWhiteSpace(warehouseCode)
                || string.IsNullOrWhiteSpace(movementDateTime))
            {
                return 0M;
            }

            return _stockTransferGateway.GetStockBalanceAt(
                profile,
                settings,
                NormalizeReferenceCode(materialCode),
                NormalizeReferenceCode(lotCode),
                NormalizeReferenceCode(warehouseCode),
                ParseMovementDateTime(movementDateTime, "Data/Hora da transferencia invalida.").ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                NormalizeTransferNumber(excludedTransferNumber, allowEmpty: true));
        }

        public StockTransferSummary[] SearchTransfers(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            var settings = GetSettings(configuration, profile);
            return _stockTransferGateway.SearchTransfers(profile, settings, NormalizeText(filter))
                .OrderByDescending(item => ParseStoredDate(item.MovementDateTime))
                .ThenByDescending(item => ExtractNumericSuffix(item.Number))
                .ThenByDescending(item => item.Number ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public StockTransferDetail LoadTransfer(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeTransferNumber(number);
            var detail = _stockTransferGateway.LoadTransferDetail(profile, settings, normalizedNumber);
            if (detail == null)
            {
                throw new InvalidOperationException("Transferencia nao encontrada.");
            }

            return detail;
        }

        public RecordLockResult TryLockTransfer(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _stockTransferGateway.TryLockTransfer(profile, settings, NormalizeTransferNumber(number), NormalizeActor(userName));
        }

        public void ReleaseTransferLock(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeTransferNumber(number, allowEmpty: true);
            if (string.IsNullOrWhiteSpace(normalizedNumber))
            {
                return;
            }

            _stockTransferGateway.ReleaseTransferLock(profile, settings, normalizedNumber, NormalizeActor(userName));
        }

        public void CreateTransfer(AppConfiguration configuration, DatabaseProfile profile, SaveStockTransferRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeRequest(request, allowExistingTransfer: false, profile, settings);
            _stockTransferGateway.CreateTransfer(profile, settings, normalized);

            SafeAudit(
                profile,
                normalized.ActorUserName,
                "Movimentacao de transferencia",
                "Tela=Transferencia; Acao=Salvar; Documento=" + normalized.Number
                + "; DataHora=" + normalized.MovementDateTime
                + "; AlmoxOrigem=" + normalized.OriginWarehouseCode
                + "; AlmoxDestino=" + normalized.DestinationWarehouseCode
                + "; Itens=" + FormatAuditItems(normalized.Items),
                settings);
        }

        public void UpdateTransfer(AppConfiguration configuration, DatabaseProfile profile, SaveStockTransferRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeRequest(request, allowExistingTransfer: true, profile, settings);
            _stockTransferGateway.UpdateTransfer(profile, settings, normalized);

            SafeAudit(
                profile,
                normalized.ActorUserName,
                "Movimentacao de transferencia",
                "Tela=Transferencia; Acao=Alterar; Documento=" + normalized.Number
                + "; DataHora=" + normalized.MovementDateTime
                + "; AlmoxOrigem=" + normalized.OriginWarehouseCode
                + "; AlmoxDestino=" + normalized.DestinationWarehouseCode
                + "; Itens=" + FormatAuditItems(normalized.Items),
                settings);
        }

        public void CancelTransfer(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeTransferNumber(number);
            var normalizedUserName = NormalizeActor(userName);
            var detail = _stockTransferGateway.LoadTransferDetail(profile, settings, normalizedNumber);
            if (detail == null)
            {
                throw new InvalidOperationException("Transferencia nao encontrada.");
            }

            if (!string.Equals(detail.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("A transferencia selecionada ja esta cancelada ou inativa.");
            }

            ValidateBusinessLocks(profile, settings, detail.MovementDateTime);
            ValidateDestinationBalanceForCancellation(profile, settings, detail);
            _stockTransferGateway.CancelTransfer(profile, settings, normalizedNumber, normalizedUserName);

            SafeAudit(
                profile,
                normalizedUserName,
                "Movimentacao de transferencia",
                "Tela=Transferencia; Acao=Cancelar; Documento=" + normalizedNumber
                + "; Antes=" + FormatAuditItems(detail.Items.Select(item => new StockTransferItemInput
                {
                    MaterialCode = item.MaterialCode,
                    LotCode = item.LotCode,
                    Quantity = item.Quantity,
                }).ToArray()),
                settings);
        }

        private SaveStockTransferRequest NormalizeRequest(
            SaveStockTransferRequest request,
            bool allowExistingTransfer,
            DatabaseProfile profile,
            ConnectionResilienceSettings settings)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Number = NormalizeTransferNumber(request.Number, allowEmpty: !allowExistingTransfer);
            if (string.IsNullOrWhiteSpace(request.Number))
            {
                request.Number = _stockTransferGateway.GenerateNextTransferNumber(profile, settings);
            }

            request.OriginWarehouseCode = NormalizeRequiredReferenceCode(request.OriginWarehouseCode, "almoxarifado de origem");
            request.DestinationWarehouseCode = NormalizeRequiredReferenceCode(request.DestinationWarehouseCode, "almoxarifado de destino");
            if (string.Equals(request.OriginWarehouseCode, request.DestinationWarehouseCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("O almoxarifado de origem e destino nao podem ser iguais.");
            }

            request.ActorUserName = NormalizeActor(request.ActorUserName);
            request.Items = NormalizeItems(request.Items);

            var movementDate = ParseMovementDateTime(request.MovementDateTime, "Data/Hora da transferencia invalida.");
            if (movementDate > DateTime.Now)
            {
                throw new InvalidOperationException("A Data/Hora da transferencia nao pode ser futura.");
            }

            request.MovementDateTime = movementDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ValidateBusinessLocks(profile, settings, request.MovementDateTime);
            ValidateTransferItems(
                profile,
                settings,
                request.OriginWarehouseCode,
                request.MovementDateTime,
                allowExistingTransfer ? request.Number : null,
                request.Items);

            var existing = _stockTransferGateway.LoadTransferDetail(profile, settings, request.Number);
            if (!allowExistingTransfer)
            {
                if (existing != null && string.Equals(existing.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "A transferencia " + request.Number + " ja esta ativa. Consulte o documento e use Alterar para criar uma nova versao.");
                }
            }
            else if (existing == null || !string.Equals(existing.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Transferencia nao encontrada para alteracao.");
            }

            return request;
        }

        private void ValidateBusinessLocks(DatabaseProfile profile, ConnectionResilienceSettings settings, string movementDateValue)
        {
            var movementDateTime = ParseStoredDate(movementDateValue);
            DateTime closingDate;
            var closingDateRaw = _stockTransferGateway.GetParameter(profile, settings, "data_fechamento", string.Empty);
            if (TryParseClosingDate(closingDateRaw, out closingDate) && movementDateTime <= closingDate)
            {
                throw new InvalidOperationException("Nao e permitido registrar transferencias antes da data de fechamento.");
            }

            int limitDays;
            var limitDaysRaw = _stockTransferGateway.GetParameter(profile, settings, "limite_dias_transferencia", "7");
            if (int.TryParse(limitDaysRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out limitDays) && limitDays > 0)
            {
                var minimumDate = DateTime.Now.AddDays(-limitDays);
                if (movementDateTime < minimumDate)
                {
                    throw new InvalidOperationException("Transferencias so podem ser registradas nos ultimos " + limitDays + " dias.");
                }
            }
        }

        private void ValidateTransferItems(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string originWarehouseCode,
            string movementDateTime,
            string excludedTransferNumber,
            StockTransferItemInput[] items)
        {
            if (items == null || items.Length == 0)
            {
                throw new InvalidOperationException("Adicione ao menos 1 item.");
            }

            var groupedItems = new Dictionary<string, AggregatedTransferItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                if (item == null)
                {
                    throw new InvalidOperationException("Existe item invalido na transferencia.");
                }

                item.MaterialCode = NormalizeRequiredReferenceCode(item.MaterialCode, "material");
                item.LotCode = NormalizeRequiredReferenceCode(item.LotCode, "lote");
                if (item.Quantity <= 0M)
                {
                    throw new InvalidOperationException("Quantidade deve ser maior que zero.");
                }

                var key = item.MaterialCode + "|" + item.LotCode;
                AggregatedTransferItem grouped;
                if (!groupedItems.TryGetValue(key, out grouped))
                {
                    grouped = new AggregatedTransferItem
                    {
                        MaterialCode = item.MaterialCode,
                        LotCode = item.LotCode,
                        Quantity = 0M,
                    };
                    groupedItems[key] = grouped;
                }

                grouped.Quantity += item.Quantity;
            }

            var issues = new List<string>();
            foreach (var grouped in groupedItems.Values)
            {
                var availableBalance = _stockTransferGateway.GetStockBalanceAt(
                    profile,
                    settings,
                    grouped.MaterialCode,
                    grouped.LotCode,
                    originWarehouseCode,
                    movementDateTime,
                    excludedTransferNumber);

                if (grouped.Quantity > availableBalance)
                {
                    issues.Add(
                        "Material: " + grouped.MaterialCode + "\n"
                        + "Lote: " + grouped.LotCode + "\n"
                        + "Saldo disponivel: " + availableBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Falta: " + (grouped.Quantity - availableBalance).ToString("N2", CultureInfo.GetCultureInfo("pt-BR")));
                }
            }

            if (issues.Count > 0)
            {
                throw new InvalidOperationException(
                    "Nao foi possivel concluir a operacao.\n\n"
                    + "Os itens abaixo nao possuem saldo suficiente no almoxarifado de origem:\n\n"
                    + string.Join("\n\n", issues.ToArray()));
            }
        }

        private void ValidateDestinationBalanceForCancellation(DatabaseProfile profile, ConnectionResilienceSettings settings, StockTransferDetail detail)
        {
            var groupedItems = detail.Items
                .Where(item => item != null)
                .GroupBy(
                    item => (item.MaterialCode ?? string.Empty).Trim().ToUpperInvariant() + "|" + (item.LotCode ?? string.Empty).Trim().ToUpperInvariant(),
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => new AggregatedTransferItem
                {
                    MaterialCode = group.First().MaterialCode,
                    LotCode = group.First().LotCode,
                    Quantity = group.Sum(item => item.Quantity),
                })
                .ToArray();

            var issues = new List<string>();
            foreach (var grouped in groupedItems)
            {
                var currentBalance = _stockTransferGateway.GetStockBalanceAt(
                    profile,
                    settings,
                    NormalizeReferenceCode(grouped.MaterialCode),
                    NormalizeReferenceCode(grouped.LotCode),
                    NormalizeReferenceCode(detail.DestinationWarehouseCode),
                    string.Empty,
                    null);

                if (grouped.Quantity > currentBalance)
                {
                    issues.Add(
                        "Material: " + grouped.MaterialCode + "\n"
                        + "Lote: " + grouped.LotCode + "\n"
                        + "Saldo atual no destino: " + currentBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Falta: " + (grouped.Quantity - currentBalance).ToString("N2", CultureInfo.GetCultureInfo("pt-BR")));
                }
            }

            if (issues.Count > 0)
            {
                throw new InvalidOperationException(
                    "Cancelamento bloqueado.\n\n"
                    + "Nao ha saldo suficiente no almoxarifado de destino para reverter a transferencia:\n\n"
                    + string.Join("\n\n", issues.ToArray()));
            }
        }

        private static StockTransferItemInput[] NormalizeItems(StockTransferItemInput[] items)
        {
            return (items ?? Array.Empty<StockTransferItemInput>())
                .Select(item => item ?? new StockTransferItemInput())
                .ToArray();
        }

        private static DateTime ParseMovementDateTime(string value, string errorMessage)
        {
            DateTime parsed;
            if (!DateTime.TryParseExact((value ?? string.Empty).Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return parsed;
        }

        private static DateTime ParseStoredDate(string value)
        {
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "yyyy-MM-dd" };
            DateTime parsed;
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static bool TryParseClosingDate(string value, out DateTime closingDate)
        {
            var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy" };
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out closingDate);
        }

        private static string NormalizeTransferNumber(string value, bool allowEmpty = false)
        {
            var normalized = NormalizeText(value).ToUpperInvariant();
            if (normalized.Length == 0 && !allowEmpty)
            {
                throw new InvalidOperationException("Informe o numero da transferencia.");
            }

            return normalized;
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
            return string.IsNullOrWhiteSpace(value) ? "sistema" : value.Trim();
        }

        private static string NormalizeMovementDateInput(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            DateTime parsed;
            return DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                : string.Empty;
        }

        private static string NormalizeText(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static int ExtractNumericSuffix(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            int parsed;
            return int.TryParse(digits, out parsed) ? parsed : int.MinValue;
        }

        private static string FormatAuditItems(IEnumerable<StockTransferItemInput> items)
        {
            var entries = (items ?? Array.Empty<StockTransferItemInput>())
                .Where(item => item != null)
                .Select(item =>
                    "(mat=" + item.MaterialCode
                    + "; lote=" + item.LotCode
                    + "; qtd=" + item.Quantity.ToString("0.##", CultureInfo.InvariantCulture) + ")")
                .ToArray();
            return string.Join(" | ", entries);
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

        private sealed class AggregatedTransferItem
        {
            public string MaterialCode { get; set; }

            public string LotCode { get; set; }

            public decimal Quantity { get; set; }
        }
    }
}
