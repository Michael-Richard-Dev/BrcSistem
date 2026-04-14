using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class InboundReceiptService
    {
        private readonly IMasterDataGateway _masterDataGateway;
        private readonly IInboundReceiptGateway _inboundReceiptGateway;
        private readonly IAuditTrailService _auditTrailService;

        public InboundReceiptService(
            IMasterDataGateway masterDataGateway,
            IInboundReceiptGateway inboundReceiptGateway,
            IAuditTrailService auditTrailService)
        {
            _masterDataGateway = masterDataGateway;
            _inboundReceiptGateway = inboundReceiptGateway;
            _auditTrailService = auditTrailService;
        }

        public SupplierSummary[] LoadSuppliers(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.LoadSuppliers(profile, settings)
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterials(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _masterDataGateway.LoadPackagings(profile, settings)
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterialsBySupplier(AppConfiguration configuration, DatabaseProfile profile, string supplierCode)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedSupplierCode = NormalizeReferenceCode(supplierCode);
            if (string.IsNullOrWhiteSpace(normalizedSupplierCode))
            {
                return LoadMaterials(configuration, profile);
            }

            return _inboundReceiptGateway.LoadMaterialsBySupplier(profile, settings, normalizedSupplierCode)
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _inboundReceiptGateway.LoadWarehousesForUser(profile, settings, NormalizeActor(userName))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public LotSummary[] LoadLots(AppConfiguration configuration, DatabaseProfile profile, string supplierCode, string materialCode)
        {
            var settings = GetSettings(configuration, profile);
            return _inboundReceiptGateway.LoadLots(
                    profile,
                    settings,
                    NormalizeReferenceCode(supplierCode),
                    NormalizeReferenceCode(materialCode))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public bool IsMaterialBrcEnabled(AppConfiguration configuration, DatabaseProfile profile, string materialCode)
        {
            var settings = GetSettings(configuration, profile);
            return _inboundReceiptGateway.IsMaterialBrcEnabled(profile, settings, NormalizeReferenceCode(materialCode));
        }

        public InboundReceiptSummary[] SearchReceipts(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            var settings = GetSettings(configuration, profile);
            return _inboundReceiptGateway.SearchReceipts(profile, settings, NormalizeText(filter))
                .OrderByDescending(item => ParseMovementDate(item.MovementDateTime))
                .ThenByDescending(item => ParseNumeric(item.Number))
                .ThenByDescending(item => item.Number ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public InboundReceiptDetail LoadReceipt(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeReceiptNumber(number);
            var normalizedSupplierCode = NormalizeRequiredReferenceCode(supplierCode, "fornecedor");
            var detail = _inboundReceiptGateway.LoadReceiptDetail(profile, settings, normalizedNumber, normalizedSupplierCode);
            if (detail == null)
            {
                throw new InvalidOperationException("Nota nao encontrada.");
            }

            return detail;
        }

        public RecordLockResult TryLockReceipt(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _inboundReceiptGateway.TryLockReceipt(
                profile,
                settings,
                NormalizeReceiptNumber(number),
                NormalizeRequiredReferenceCode(supplierCode, "fornecedor"),
                NormalizeActor(userName));
        }

        public void ReleaseReceiptLock(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeReceiptNumber(number, allowEmpty: true);
            var normalizedSupplierCode = NormalizeReferenceCode(supplierCode);
            if (string.IsNullOrWhiteSpace(normalizedNumber) || string.IsNullOrWhiteSpace(normalizedSupplierCode))
            {
                return;
            }

            _inboundReceiptGateway.ReleaseReceiptLock(profile, settings, normalizedNumber, normalizedSupplierCode, NormalizeActor(userName));
        }

        public void CreateReceipt(AppConfiguration configuration, DatabaseProfile profile, SaveInboundReceiptRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeRequest(request, allowExistingReceipt: false, profile, settings);
            _inboundReceiptGateway.CreateReceipt(profile, settings, normalized);

            SafeAudit(
                profile,
                normalized.ActorUserName,
                "Movimentacao de entrada",
                "Tela=Entrada; Acao=Salvar; Documento=" + normalized.Number + "; Fornecedor=" + normalized.SupplierCode
                + "; Almox=" + normalized.WarehouseCode + "; DataHora=" + normalized.ReceiptDateTime
                + "; Itens=" + FormatAuditItems(normalized.Items),
                settings);
        }

        public void UpdateReceipt(AppConfiguration configuration, DatabaseProfile profile, SaveInboundReceiptRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeRequest(request, allowExistingReceipt: true, profile, settings);
            _inboundReceiptGateway.UpdateReceipt(profile, settings, normalized);

            SafeAudit(
                profile,
                normalized.ActorUserName,
                "Movimentacao de entrada",
                "Tela=Entrada; Acao=Alterar; Documento=" + normalized.Number + "; Fornecedor=" + normalized.SupplierCode
                + "; Almox=" + normalized.WarehouseCode + "; DataHora=" + normalized.ReceiptDateTime
                + "; Itens=" + FormatAuditItems(normalized.Items),
                settings);
        }

        public void CancelReceipt(AppConfiguration configuration, DatabaseProfile profile, string number, string supplierCode, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeReceiptNumber(number);
            var normalizedSupplierCode = NormalizeRequiredReferenceCode(supplierCode, "fornecedor");
            var normalizedUserName = NormalizeActor(userName);
            var detail = _inboundReceiptGateway.LoadReceiptDetail(profile, settings, normalizedNumber, normalizedSupplierCode);
            if (detail == null)
            {
                throw new InvalidOperationException("Nota nao encontrada.");
            }

            if (!string.Equals(detail.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("A nota selecionada ja esta cancelada ou inativa.");
            }

            ValidateBusinessLocks(profile, settings, detail.ReceiptDateTime);
            ValidateCancelStockBalance(profile, settings, detail);
            _inboundReceiptGateway.CancelReceipt(profile, settings, normalizedNumber, normalizedSupplierCode, normalizedUserName);

            SafeAudit(
                profile,
                normalizedUserName,
                "Movimentacao de entrada",
                "Tela=Entrada; Acao=Cancelar; Documento=" + normalizedNumber + "; Fornecedor=" + normalizedSupplierCode
                + "; Antes=" + FormatAuditItems(detail.Items.Select(item => new InboundReceiptItemInput
                {
                    MaterialCode = item.MaterialCode,
                    LotCode = item.LotCode,
                    Quantity = item.Quantity,
                }).ToArray()),
                settings);
        }

        private SaveInboundReceiptRequest NormalizeRequest(
            SaveInboundReceiptRequest request,
            bool allowExistingReceipt,
            DatabaseProfile profile,
            ConnectionResilienceSettings settings)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Number = NormalizeReceiptNumber(request.Number);
            request.SupplierCode = NormalizeRequiredReferenceCode(request.SupplierCode, "fornecedor");
            request.WarehouseCode = NormalizeRequiredReferenceCode(request.WarehouseCode, "almoxarifado");
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            request.Items = NormalizeItems(request.Items);

            var emissionDate = ParseBrazilianDate(request.EmissionDate, "Data de Emissao invalida.");
            var movementDate = ParseBrazilianDateTime(request.ReceiptDateTime, "Data/Hora de Recebimento invalida.");
            if (movementDate.Date > DateTime.Today)
            {
                throw new InvalidOperationException("A Data/Hora de Recebimento nao pode ser futura.");
            }

            if (movementDate < emissionDate.Date)
            {
                throw new InvalidOperationException("Data de Recebimento nao pode ser anterior a Emissao.");
            }

            ValidateBusinessLocks(profile, settings, movementDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            ValidateReceiptItems(profile, settings, request.SupplierCode, request.Items);

            request.EmissionDate = emissionDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            request.ReceiptDateTime = movementDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            if (!allowExistingReceipt)
            {
                var existing = _inboundReceiptGateway.LoadReceiptDetail(profile, settings, request.Number, request.SupplierCode);
                if (existing != null && string.Equals(existing.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "A nota " + request.Number + " ja foi lancada para este fornecedor. Consulte a nota e use Alterar para criar uma nova versao.");
                }
            }

            return request;
        }

        private void ValidateBusinessLocks(DatabaseProfile profile, ConnectionResilienceSettings settings, string movementDateValue)
        {
            DateTime movementDateTime = ParseStoredMovementDate(movementDateValue);
            DateTime closingDate;
            var closingDateRaw = _inboundReceiptGateway.GetParameter(profile, settings, "data_fechamento", string.Empty);
            if (TryParseClosingDate(closingDateRaw, out closingDate) && movementDateTime <= closingDate)
            {
                throw new InvalidOperationException("Nao e permitido registrar entradas antes da data de fechamento.");
            }

            int limitDays;
            var limitDaysRaw = _inboundReceiptGateway.GetParameter(profile, settings, "limite_dias_entrada", "7");
            if (int.TryParse(limitDaysRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out limitDays) && limitDays > 0)
            {
                var minimumDate = DateTime.Now.AddDays(-limitDays);
                if (movementDateTime < minimumDate)
                {
                    throw new InvalidOperationException("Entradas so podem ser registradas nos ultimos " + limitDays + " dias.");
                }
            }
        }

        private void ValidateReceiptItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string supplierCode, InboundReceiptItemInput[] items)
        {
            if (items == null || items.Length == 0)
            {
                throw new InvalidOperationException("Adicione ao menos 1 item.");
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                if (item == null)
                {
                    throw new InvalidOperationException("Existe item invalido na nota.");
                }

                item.MaterialCode = NormalizeRequiredReferenceCode(item.MaterialCode, "material");
                item.LotCode = NormalizeRequiredReferenceCode(item.LotCode, "lote").ToUpperInvariant();
                if (item.Quantity <= 0)
                {
                    throw new InvalidOperationException("Quantidade deve ser maior que zero.");
                }

                var duplicateKey = item.MaterialCode + "|" + item.LotCode;
                if (!seen.Add(duplicateKey))
                {
                    throw new InvalidOperationException("Nao e permitido repetir o mesmo material e lote na nota.");
                }

                var lot = _inboundReceiptGateway.LoadLots(profile, settings, supplierCode, item.MaterialCode)
                    .FirstOrDefault(candidate => string.Equals(candidate.Code, item.LotCode, StringComparison.OrdinalIgnoreCase));
                if (lot == null)
                {
                    throw new InvalidOperationException(
                        "O lote '" + item.LotCode + "' nao foi localizado para o material '" + item.MaterialCode + "' e fornecedor '" + supplierCode + "'.");
                }
            }
        }

        private void ValidateCancelStockBalance(DatabaseProfile profile, ConnectionResilienceSettings settings, InboundReceiptDetail detail)
        {
            var issues = new List<string>();
            foreach (var item in detail.Items)
            {
                var currentBalance = _inboundReceiptGateway.GetActiveStockBalance(profile, settings, item.MaterialCode, item.LotCode, detail.WarehouseCode);
                var balanceAfterCancel = currentBalance - item.Quantity;
                if (balanceAfterCancel < 0)
                {
                    issues.Add(
                        "Material: " + item.MaterialDisplay + "\n"
                        + "Lote: " + item.LotDisplay + "\n"
                        + "Saldo atual: " + currentBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Falta apos cancelamento: " + Math.Abs(balanceAfterCancel).ToString("N2", CultureInfo.GetCultureInfo("pt-BR")));
                }
            }

            if (issues.Count > 0)
            {
                throw new InvalidOperationException(
                    "Nao foi possivel cancelar a nota " + detail.Number + ".\n\n"
                    + "Os itens abaixo nao possuem saldo suficiente para o cancelamento:\n\n"
                    + string.Join("\n\n", issues.ToArray()));
            }
        }

        private static InboundReceiptItemInput[] NormalizeItems(InboundReceiptItemInput[] items)
        {
            return (items ?? Array.Empty<InboundReceiptItemInput>())
                .Select(item => item ?? new InboundReceiptItemInput())
                .ToArray();
        }

        private static DateTime ParseBrazilianDate(string value, string errorMessage)
        {
            DateTime parsed;
            if (!DateTime.TryParseExact((value ?? string.Empty).Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return parsed.Date;
        }

        private static DateTime ParseBrazilianDateTime(string value, string errorMessage)
        {
            DateTime parsed;
            if (!DateTime.TryParseExact((value ?? string.Empty).Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return parsed;
        }

        private static DateTime ParseStoredMovementDate(string value)
        {
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "yyyy-MM-dd" };
            DateTime parsed;
            if (DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return parsed;
            }

            throw new InvalidOperationException("Data/Hora de Recebimento invalida.");
        }

        private static bool TryParseClosingDate(string value, out DateTime closingDate)
        {
            var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy" };
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out closingDate);
        }

        private static string NormalizeReceiptNumber(string value, bool allowEmpty = false)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length == 0 && !allowEmpty)
            {
                throw new InvalidOperationException("Informe o numero da nota.");
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
            return string.IsNullOrWhiteSpace(value) ? "sistema" : value.Trim();
        }

        private static string NormalizeText(string value)
        {
            return (value ?? string.Empty).Trim();
        }

        private static DateTime ParseMovementDate(string value)
        {
            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "yyyy-MM-dd" };
            return DateTime.TryParseExact(value ?? string.Empty, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static int ParseNumeric(string value)
        {
            int parsed;
            return int.TryParse(value, out parsed) ? parsed : int.MinValue;
        }

        private static string FormatAuditItems(IEnumerable<InboundReceiptItemInput> items)
        {
            var entries = (items ?? Array.Empty<InboundReceiptItemInput>())
                .Where(item => item != null)
                .Select(item => "(mat=" + item.MaterialCode + "; lote=" + item.LotCode + "; qtd=" + item.Quantity.ToString("0.##", CultureInfo.InvariantCulture) + ")")
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
