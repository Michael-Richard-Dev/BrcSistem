using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class ProductionOutputService
    {
        private readonly IMasterDataGateway _masterDataGateway;
        private readonly IProductionOutputGateway _productionOutputGateway;
        private readonly IAuditTrailService _auditTrailService;

        public ProductionOutputService(
            IMasterDataGateway masterDataGateway,
            IProductionOutputGateway productionOutputGateway,
            IAuditTrailService auditTrailService)
        {
            _masterDataGateway = masterDataGateway;
            _productionOutputGateway = productionOutputGateway;
            _auditTrailService = auditTrailService;
        }

        public string GenerateNextOutputNumber(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _productionOutputGateway.GenerateNextOutputNumber(profile, settings);
        }

        public ProductSummary[] LoadProducts(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.LoadProducts(profile, settings)
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _productionOutputGateway.LoadWarehousesForUser(profile, settings, NormalizeActor(userName))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterialsByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime)
        {
            var settings = GetSettings(configuration, profile);
            return _productionOutputGateway.LoadMaterialsByWarehouse(
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
            return _productionOutputGateway.LoadLotsByWarehouseAndMaterial(
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
            string excludedOutputNumber)
        {
            var settings = GetSettings(configuration, profile);
            if (string.IsNullOrWhiteSpace(materialCode)
                || string.IsNullOrWhiteSpace(lotCode)
                || string.IsNullOrWhiteSpace(warehouseCode)
                || string.IsNullOrWhiteSpace(movementDateTime))
            {
                return 0M;
            }

            return _productionOutputGateway.GetStockBalanceAt(
                profile,
                settings,
                NormalizeReferenceCode(materialCode),
                NormalizeReferenceCode(lotCode),
                NormalizeReferenceCode(warehouseCode),
                ParseMovementDateTime(movementDateTime, "Data/Hora da saida invalida.").ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                NormalizeOutputNumber(excludedOutputNumber, allowEmpty: true));
        }

        public ProductionOutputSummary[] SearchOutputs(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            var settings = GetSettings(configuration, profile);
            return _productionOutputGateway.SearchOutputs(profile, settings, NormalizeText(filter))
                .OrderByDescending(item => ParseStoredDate(item.MovementDateTime))
                .ThenByDescending(item => ExtractNumericSuffix(item.Number))
                .ThenByDescending(item => item.Number ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public ProductionOutputDetail LoadOutput(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeOutputNumber(number);
            var detail = _productionOutputGateway.LoadOutputDetail(profile, settings, normalizedNumber);
            if (detail == null)
            {
                throw new InvalidOperationException("Saida de producao nao encontrada.");
            }

            return detail;
        }

        public RecordLockResult TryLockOutput(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _productionOutputGateway.TryLockOutput(profile, settings, NormalizeOutputNumber(number), NormalizeActor(userName));
        }

        public void ReleaseOutputLock(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeOutputNumber(number, allowEmpty: true);
            if (string.IsNullOrWhiteSpace(normalizedNumber))
            {
                return;
            }

            _productionOutputGateway.ReleaseOutputLock(profile, settings, normalizedNumber, NormalizeActor(userName));
        }

        public void CreateOutput(AppConfiguration configuration, DatabaseProfile profile, SaveProductionOutputRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeRequest(request, allowExistingOutput: false, profile, settings);
            _productionOutputGateway.CreateOutput(profile, settings, normalized);

            SafeAudit(
                profile,
                normalized.ActorUserName,
                "Movimentacao de saida de producao",
                "Tela=SaidaProducao; Acao=Salvar; Documento=" + normalized.Number
                + "; Almox=" + normalized.WarehouseCode
                + "; DataHora=" + normalized.MovementDateTime
                + "; Finalidade=" + normalized.Purpose
                + "; Turno=" + normalized.Shift
                + "; Itens=" + FormatAuditItems(normalized.Items),
                settings);
        }

        public void UpdateOutput(AppConfiguration configuration, DatabaseProfile profile, SaveProductionOutputRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeRequest(request, allowExistingOutput: true, profile, settings);
            _productionOutputGateway.UpdateOutput(profile, settings, normalized);

            SafeAudit(
                profile,
                normalized.ActorUserName,
                "Movimentacao de saida de producao",
                "Tela=SaidaProducao; Acao=Alterar; Documento=" + normalized.Number
                + "; Almox=" + normalized.WarehouseCode
                + "; DataHora=" + normalized.MovementDateTime
                + "; Finalidade=" + normalized.Purpose
                + "; Turno=" + normalized.Shift
                + "; Itens=" + FormatAuditItems(normalized.Items),
                settings);
        }

        public void CancelOutput(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeOutputNumber(number);
            var normalizedUserName = NormalizeActor(userName);
            var detail = _productionOutputGateway.LoadOutputDetail(profile, settings, normalizedNumber);
            if (detail == null)
            {
                throw new InvalidOperationException("Saida de producao nao encontrada.");
            }

            if (!string.Equals(detail.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("A saida selecionada ja esta cancelada ou inativa.");
            }

            ValidateBusinessLocks(profile, settings, detail.MovementDateTime);
            _productionOutputGateway.CancelOutput(profile, settings, normalizedNumber, normalizedUserName);

            SafeAudit(
                profile,
                normalizedUserName,
                "Movimentacao de saida de producao",
                "Tela=SaidaProducao; Acao=Cancelar; Documento=" + normalizedNumber
                + "; Antes=" + FormatAuditItems(detail.Items.Select(item => new ProductionOutputItemInput
                {
                    ProductCode = item.ProductCode,
                    MaterialCode = item.MaterialCode,
                    LotCode = item.LotCode,
                    QuantitySent = item.QuantitySent,
                    QuantityReturned = item.QuantityReturned,
                    QuantityConsumed = item.QuantityConsumed,
                }).ToArray()),
                settings);
        }

        private SaveProductionOutputRequest NormalizeRequest(
            SaveProductionOutputRequest request,
            bool allowExistingOutput,
            DatabaseProfile profile,
            ConnectionResilienceSettings settings)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Number = NormalizeOutputNumber(request.Number, allowEmpty: !allowExistingOutput);
            if (string.IsNullOrWhiteSpace(request.Number))
            {
                request.Number = _productionOutputGateway.GenerateNextOutputNumber(profile, settings);
            }

            request.WarehouseCode = NormalizeRequiredReferenceCode(request.WarehouseCode, "almoxarifado");
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            request.Purpose = NormalizePurpose(request.Purpose);
            request.Shift = NormalizeShift(request.Shift);
            request.Items = NormalizeItems(request.Items);

            var movementDate = ParseMovementDateTime(request.MovementDateTime, "Data/Hora da saida invalida.");
            if (movementDate > DateTime.Now)
            {
                throw new InvalidOperationException("A Data/Hora da saida nao pode ser futura.");
            }

            request.MovementDateTime = movementDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ValidateBusinessLocks(profile, settings, request.MovementDateTime);
            ValidateOutputItems(profile, settings, request.WarehouseCode, request.MovementDateTime, allowExistingOutput ? request.Number : null, request.Items);

            var existing = _productionOutputGateway.LoadOutputDetail(profile, settings, request.Number);
            if (!allowExistingOutput)
            {
                if (existing != null && string.Equals(existing.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "A saida " + request.Number + " ja esta ativa. Consulte o documento e use Alterar para criar uma nova versao.");
                }
            }
            else if (existing == null || !string.Equals(existing.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Saida de producao nao encontrada para alteracao.");
            }

            return request;
        }

        private void ValidateBusinessLocks(DatabaseProfile profile, ConnectionResilienceSettings settings, string movementDateValue)
        {
            var movementDateTime = ParseStoredDate(movementDateValue);
            DateTime closingDate;
            var closingDateRaw = _productionOutputGateway.GetParameter(profile, settings, "data_fechamento", string.Empty);
            if (TryParseClosingDate(closingDateRaw, out closingDate) && movementDateTime <= closingDate)
            {
                throw new InvalidOperationException("Nao e permitido registrar saidas antes da data de fechamento.");
            }

            int limitDays;
            var limitDaysRaw = _productionOutputGateway.GetParameter(profile, settings, "limite_dias_saida", "7");
            if (int.TryParse(limitDaysRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out limitDays) && limitDays > 0)
            {
                var minimumDate = DateTime.Now.AddDays(-limitDays);
                if (movementDateTime < minimumDate)
                {
                    throw new InvalidOperationException("Saidas so podem ser registradas nos ultimos " + limitDays + " dias.");
                }
            }
        }

        private void ValidateOutputItems(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string warehouseCode,
            string movementDateTime,
            string excludedOutputNumber,
            ProductionOutputItemInput[] items)
        {
            if (items == null || items.Length == 0)
            {
                throw new InvalidOperationException("Adicione ao menos 1 item.");
            }

            var issues = new List<string>();
            var duplicateKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                if (item == null)
                {
                    throw new InvalidOperationException("Existe item invalido na saida.");
                }

                item.ProductCode = NormalizeReferenceCode(item.ProductCode);
                item.MaterialCode = NormalizeRequiredReferenceCode(item.MaterialCode, "material");
                item.LotCode = NormalizeRequiredReferenceCode(item.LotCode, "lote");

                if (item.QuantitySent < 0M)
                {
                    throw new InvalidOperationException("Quantidade de envio nao pode ser negativa.");
                }

                if (item.QuantityReturned < 0M)
                {
                    throw new InvalidOperationException("Quantidade de retorno nao pode ser negativa.");
                }

                if (item.QuantityReturned > item.QuantitySent)
                {
                    throw new InvalidOperationException("Quantidade de retorno nao pode ser maior que a quantidade de envio.");
                }

                item.QuantityConsumed = item.QuantitySent - item.QuantityReturned;
                if (item.QuantityConsumed <= 0M)
                {
                    throw new InvalidOperationException("A quantidade consumida deve ser maior que zero.");
                }

                var duplicateKey = item.MaterialCode + "|" + item.LotCode;
                if (!duplicateKeys.Add(duplicateKey))
                {
                    throw new InvalidOperationException("Nao e permitido repetir o mesmo material e lote na saida.");
                }

                var availableBalance = _productionOutputGateway.GetStockBalanceAt(
                    profile,
                    settings,
                    item.MaterialCode,
                    item.LotCode,
                    warehouseCode,
                    movementDateTime,
                    excludedOutputNumber);

                if (item.QuantityConsumed > availableBalance)
                {
                    issues.Add(
                        "Material: " + item.MaterialCode + "\n"
                        + "Lote: " + item.LotCode + "\n"
                        + "Saldo disponivel: " + availableBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Falta: " + (item.QuantityConsumed - availableBalance).ToString("N2", CultureInfo.GetCultureInfo("pt-BR")));
                }
            }

            if (issues.Count > 0)
            {
                throw new InvalidOperationException(
                    "Nao foi possivel concluir a operacao.\n\n"
                    + "Os itens abaixo nao possuem saldo suficiente:\n\n"
                    + string.Join("\n\n", issues.ToArray()));
            }
        }

        private static ProductionOutputItemInput[] NormalizeItems(ProductionOutputItemInput[] items)
        {
            return (items ?? Array.Empty<ProductionOutputItemInput>())
                .Select(item => item ?? new ProductionOutputItemInput())
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

        private static string NormalizeOutputNumber(string value, bool allowEmpty = false)
        {
            var normalized = NormalizeText(value).ToUpperInvariant();
            if (normalized.Length == 0 && !allowEmpty)
            {
                throw new InvalidOperationException("Informe o numero da saida.");
            }

            return normalized;
        }

        private static string NormalizePurpose(string value)
        {
            var normalized = CollapseSpaces(value).ToUpperInvariant();
            return string.IsNullOrWhiteSpace(normalized) ? "SAIDA DE PRODUCAO" : normalized;
        }

        private static string NormalizeShift(string value)
        {
            var normalized = CollapseSpaces(value).ToUpperInvariant();
            return string.IsNullOrWhiteSpace(normalized) ? "1o TURNO" : normalized;
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

        private static string CollapseSpaces(string value)
        {
            var text = (value ?? string.Empty)
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ");
            return string.Join(" ", text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private static int ExtractNumericSuffix(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            int parsed;
            return int.TryParse(digits, out parsed) ? parsed : int.MinValue;
        }

        private static string FormatAuditItems(IEnumerable<ProductionOutputItemInput> items)
        {
            var entries = (items ?? Array.Empty<ProductionOutputItemInput>())
                .Where(item => item != null)
                .Select(item =>
                    "(prod=" + (string.IsNullOrWhiteSpace(item.ProductCode) ? "N/A" : item.ProductCode)
                    + "; mat=" + item.MaterialCode
                    + "; lote=" + item.LotCode
                    + "; envio=" + item.QuantitySent.ToString("0.##", CultureInfo.InvariantCulture)
                    + "; retorno=" + item.QuantityReturned.ToString("0.##", CultureInfo.InvariantCulture)
                    + "; consumo=" + item.QuantityConsumed.ToString("0.##", CultureInfo.InvariantCulture) + ")")
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
    }
}
