using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class MaterialRequisitionService
    {
        private readonly IMaterialRequisitionGateway _materialRequisitionGateway;
        private readonly IAuditTrailService _auditTrailService;

        public MaterialRequisitionService(IMaterialRequisitionGateway materialRequisitionGateway, IAuditTrailService auditTrailService)
        {
            _materialRequisitionGateway = materialRequisitionGateway;
            _auditTrailService = auditTrailService;
        }

        public string GenerateNextRequisitionNumber(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _materialRequisitionGateway.GenerateNextRequisitionNumber(profile, settings);
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _materialRequisitionGateway.LoadWarehousesForUser(profile, settings, NormalizeActor(userName))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterialsByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime)
        {
            var settings = GetSettings(configuration, profile);
            return _materialRequisitionGateway.LoadMaterialsByWarehouse(
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
            return _materialRequisitionGateway.LoadLotsByWarehouseAndMaterial(
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

        public RequisitionReasonSummary[] LoadReasons(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _materialRequisitionGateway.LoadReasons(profile, settings)
                .Where(item => item != null && item.IsActive)
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public QuickStockBalanceSummary[] LoadQuickStockBalances(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string warehouseCode,
            string materialCode,
            string movementDateTime,
            string excludedRequisitionNumber)
        {
            var settings = GetSettings(configuration, profile);
            return _materialRequisitionGateway.LoadQuickStockBalances(
                    profile,
                    settings,
                    NormalizeReferenceCode(warehouseCode),
                    NormalizeReferenceCode(materialCode),
                    NormalizeMovementDateInput(movementDateTime),
                    NormalizeRequisitionNumber(excludedRequisitionNumber, allowEmpty: true))
                .OrderBy(item => item.MaterialDescription ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.MaterialCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => ParseStoredDate(item.ExpirationDate))
                .ThenBy(item => item.LotName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.LotCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public decimal GetAvailableStockBalance(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string materialCode,
            string lotCode,
            string warehouseCode,
            string movementDateTime,
            string excludedRequisitionNumber)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(materialCode)
                || string.IsNullOrWhiteSpace(lotCode)
                || string.IsNullOrWhiteSpace(warehouseCode)
                || string.IsNullOrWhiteSpace(movementDateTime))
            {
                return 0M;
            }

            return _materialRequisitionGateway.GetStockBalanceAt(
                profile,
                settings,
                NormalizeReferenceCode(materialCode),
                NormalizeReferenceCode(lotCode),
                NormalizeReferenceCode(warehouseCode),
                ParseMovementDateTime(movementDateTime, "Data/Hora da requisicao invalida.").ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                NormalizeRequisitionNumber(excludedRequisitionNumber, allowEmpty: true));
        }

        public MaterialRequisitionSummary[] SearchRequisitions(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            var settings = GetSettings(configuration, profile);
            return _materialRequisitionGateway.SearchRequisitions(profile, settings, NormalizeText(filter))
                .OrderByDescending(item => ParseStoredDate(item.MovementDateTime))
                .ThenByDescending(item => ExtractNumericSuffix(item.Number))
                .ThenByDescending(item => item.Number ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public MaterialRequisitionDetail LoadRequisition(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeRequisitionNumber(number);
            var detail = _materialRequisitionGateway.LoadRequisitionDetail(profile, settings, normalizedNumber);
            if (detail == null)
            {
                throw new InvalidOperationException("Requisicao nao encontrada.");
            }

            return detail;
        }

        public RecordLockResult TryLockRequisition(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _materialRequisitionGateway.TryLockRequisition(profile, settings, NormalizeRequisitionNumber(number), NormalizeActor(userName));
        }

        public void ReleaseRequisitionLock(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeRequisitionNumber(number, allowEmpty: true);
            if (string.IsNullOrWhiteSpace(normalizedNumber))
            {
                return;
            }

            _materialRequisitionGateway.ReleaseRequisitionLock(profile, settings, normalizedNumber, NormalizeActor(userName));
        }

        public void CreateRequisition(AppConfiguration configuration, DatabaseProfile profile, SaveMaterialRequisitionRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeRequest(request, allowExistingRequisition: false, profile, settings);
            _materialRequisitionGateway.CreateRequisition(profile, settings, normalized);

            SafeAudit(
                profile,
                normalized.ActorUserName,
                "Movimentacao de requisicao",
                "Tela=Requisicao; Acao=Salvar; Documento=" + normalized.Number
                + "; DataHora=" + normalized.MovementDateTime
                + "; Almox=" + normalized.WarehouseCode
                + "; Motivo=" + normalized.ReasonName
                + "; Itens=" + FormatAuditItems(normalized.Items),
                settings);
        }

        public void UpdateRequisition(AppConfiguration configuration, DatabaseProfile profile, SaveMaterialRequisitionRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeRequest(request, allowExistingRequisition: true, profile, settings);
            _materialRequisitionGateway.UpdateRequisition(profile, settings, normalized);

            SafeAudit(
                profile,
                normalized.ActorUserName,
                "Movimentacao de requisicao",
                "Tela=Requisicao; Acao=Alterar; Documento=" + normalized.Number
                + "; DataHora=" + normalized.MovementDateTime
                + "; Almox=" + normalized.WarehouseCode
                + "; Motivo=" + normalized.ReasonName
                + "; Itens=" + FormatAuditItems(normalized.Items),
                settings);
        }

        public void CancelRequisition(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeRequisitionNumber(number);
            var normalizedUserName = NormalizeActor(userName);
            var detail = _materialRequisitionGateway.LoadRequisitionDetail(profile, settings, normalizedNumber);
            if (detail == null)
            {
                throw new InvalidOperationException("Requisicao nao encontrada.");
            }

            if (!string.Equals(detail.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("A requisicao selecionada ja esta cancelada ou inativa.");
            }

            ValidateBusinessLocks(profile, settings, detail.MovementDateTime);
            _materialRequisitionGateway.CancelRequisition(profile, settings, normalizedNumber, normalizedUserName);

            SafeAudit(
                profile,
                normalizedUserName,
                "Movimentacao de requisicao",
                "Tela=Requisicao; Acao=Cancelar; Documento=" + normalizedNumber
                + "; Antes=" + FormatAuditItems(detail.Items.Select(item => new MaterialRequisitionItemInput
                {
                    MaterialCode = item.MaterialCode,
                    LotCode = item.LotCode,
                    Quantity = item.Quantity,
                }).ToArray()),
                settings);
        }

        private SaveMaterialRequisitionRequest NormalizeRequest(
            SaveMaterialRequisitionRequest request,
            bool allowExistingRequisition,
            DatabaseProfile profile,
            ConnectionResilienceSettings settings)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Number = NormalizeRequisitionNumber(request.Number, allowEmpty: !allowExistingRequisition);
            if (string.IsNullOrWhiteSpace(request.Number))
            {
                request.Number = _materialRequisitionGateway.GenerateNextRequisitionNumber(profile, settings);
            }

            request.WarehouseCode = NormalizeRequiredReferenceCode(request.WarehouseCode, "almoxarifado");
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            request.ReasonName = NormalizeReason(request.ReasonName);
            request.Items = NormalizeItems(request.Items);

            var movementDate = ParseMovementDateTime(request.MovementDateTime, "Data/Hora da requisicao invalida.");
            if (movementDate > DateTime.Now)
            {
                throw new InvalidOperationException("A Data/Hora da requisicao nao pode ser futura.");
            }

            request.MovementDateTime = movementDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ValidateBusinessLocks(profile, settings, request.MovementDateTime);
            ValidateItems(profile, settings, request.WarehouseCode, request.MovementDateTime, allowExistingRequisition ? request.Number : null, request.Items);

            var existing = _materialRequisitionGateway.LoadRequisitionDetail(profile, settings, request.Number);
            if (!allowExistingRequisition)
            {
                if (existing != null && string.Equals(existing.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "A requisicao " + request.Number + " ja esta ativa. Consulte o documento e use Alterar para criar uma nova versao.");
                }
            }
            else if (existing == null || !string.Equals(existing.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Requisicao nao encontrada para alteracao.");
            }

            return request;
        }

        private void ValidateBusinessLocks(DatabaseProfile profile, ConnectionResilienceSettings settings, string movementDateValue)
        {
            var movementDateTime = ParseStoredDate(movementDateValue);
            DateTime closingDate;
            var closingDateRaw = _materialRequisitionGateway.GetParameter(profile, settings, "data_fechamento", string.Empty);
            if (TryParseClosingDate(closingDateRaw, out closingDate) && movementDateTime <= closingDate)
            {
                throw new InvalidOperationException("Nao e permitido registrar requisicoes antes da data de fechamento.");
            }

            int limitDays;
            var limitDaysRaw = _materialRequisitionGateway.GetParameter(profile, settings, "limite_dias_saida", "7");
            if (int.TryParse(limitDaysRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out limitDays) && limitDays > 0)
            {
                var minimumDate = DateTime.Now.AddDays(-limitDays);
                if (movementDateTime < minimumDate)
                {
                    throw new InvalidOperationException("Requisicoes so podem ser registradas nos ultimos " + limitDays + " dias.");
                }
            }
        }

        private void ValidateItems(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string movementDateTime,
            string excludedRequisitionNumber,
            MaterialRequisitionItemInput[] items)
        {
            if (items == null || items.Length == 0)
            {
                throw new InvalidOperationException("Adicione ao menos 1 item.");
            }

            var groupedItems = new Dictionary<string, AggregatedItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                if (item == null)
                {
                    throw new InvalidOperationException("Existe item invalido na requisicao.");
                }

                item.MaterialCode = NormalizeRequiredReferenceCode(item.MaterialCode, "material");
                item.LotCode = NormalizeRequiredReferenceCode(item.LotCode, "lote");
                if (item.Quantity <= 0M)
                {
                    throw new InvalidOperationException("Quantidade deve ser maior que zero.");
                }

                var key = item.MaterialCode + "|" + item.LotCode;
                AggregatedItem aggregated;
                if (!groupedItems.TryGetValue(key, out aggregated))
                {
                    aggregated = new AggregatedItem
                    {
                        MaterialCode = item.MaterialCode,
                        LotCode = item.LotCode,
                        Quantity = 0M,
                    };
                    groupedItems[key] = aggregated;
                }

                aggregated.Quantity += item.Quantity;
            }

            var issues = new List<string>();
            foreach (var aggregated in groupedItems.Values)
            {
                var availableBalance = _materialRequisitionGateway.GetStockBalanceAt(
                    profile,
                    settings,
                    aggregated.MaterialCode,
                    aggregated.LotCode,
                    warehouseCode,
                    movementDateTime,
                    excludedRequisitionNumber);

                if (aggregated.Quantity > availableBalance)
                {
                    issues.Add(
                        "Material: " + aggregated.MaterialCode + "\n"
                        + "Lote: " + aggregated.LotCode + "\n"
                        + "Saldo disponivel: " + availableBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Falta: " + (aggregated.Quantity - availableBalance).ToString("N2", CultureInfo.GetCultureInfo("pt-BR")));
                }
            }

            if (issues.Count > 0)
            {
                throw new InvalidOperationException(
                    "Nao foi possivel concluir a operacao.\n\n"
                    + "Os itens abaixo nao possuem saldo suficiente no almoxarifado:\n\n"
                    + string.Join("\n\n", issues.ToArray()));
            }
        }

        private static MaterialRequisitionItemInput[] NormalizeItems(MaterialRequisitionItemInput[] items)
        {
            return (items ?? Array.Empty<MaterialRequisitionItemInput>())
                .Select(item => item ?? new MaterialRequisitionItemInput())
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

        private static string NormalizeRequisitionNumber(string value, bool allowEmpty = false)
        {
            var normalized = NormalizeText(value).ToUpperInvariant();
            if (normalized.Length == 0 && !allowEmpty)
            {
                throw new InvalidOperationException("Informe o numero da requisicao.");
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

        private static string NormalizeReason(string value)
        {
            var normalized = NormalizeText(value);
            return string.IsNullOrWhiteSpace(normalized) ? "Outro" : normalized;
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

        private static string FormatAuditItems(IEnumerable<MaterialRequisitionItemInput> items)
        {
            var entries = (items ?? Array.Empty<MaterialRequisitionItemInput>())
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

        private sealed class AggregatedItem
        {
            public string MaterialCode { get; set; }

            public string LotCode { get; set; }

            public decimal Quantity { get; set; }
        }
    }
}
