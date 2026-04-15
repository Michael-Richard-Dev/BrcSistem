using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class InventoryService
    {
        private static readonly string[] CancelableStatuses = { "PENDENTE", "INICIADO", "EM_CONTAGEM", "FECHADO" };

        private readonly IInventoryGateway _inventoryGateway;
        private readonly IAuditTrailService _auditTrailService;

        public InventoryService(IInventoryGateway inventoryGateway, IAuditTrailService auditTrailService)
        {
            _inventoryGateway = inventoryGateway;
            _auditTrailService = auditTrailService;
        }

        public InventoryPermissions ResolvePermissions(UserIdentity identity)
        {
            if (identity == null)
            {
                return new InventoryPermissions();
            }

            var keys = identity.PermissionKeys ?? Array.Empty<string>();
            var hasFullInventoryPermission = identity.IsAdministrator
                || keys.Contains("movimentacao_inventario", StringComparer.OrdinalIgnoreCase);

            return new InventoryPermissions
            {
                CanOpen = hasFullInventoryPermission || keys.Contains("inventario_abrir", StringComparer.OrdinalIgnoreCase),
                CanCount = hasFullInventoryPermission || keys.Contains("inventario_contar", StringComparer.OrdinalIgnoreCase),
                CanClose = hasFullInventoryPermission || keys.Contains("inventario_fechar", StringComparer.OrdinalIgnoreCase),
                CanCancel = hasFullInventoryPermission || keys.Contains("inventario_cancelar", StringComparer.OrdinalIgnoreCase),
            };
        }

        public string GenerateNextInventoryNumber(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _inventoryGateway.GenerateNextInventoryNumber(profile, GetSettings(configuration, profile));
        }

        public WarehouseSummary[] LoadWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _inventoryGateway.LoadWarehousesForUser(profile, GetSettings(configuration, profile), NormalizeActor(userName))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public PackagingSummary[] LoadMaterialsByWarehouse(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string movementDateTime, bool onlyBrc)
        {
            return _inventoryGateway.LoadMaterialsByWarehouse(
                    profile,
                    GetSettings(configuration, profile),
                    NormalizeRequiredReferenceCode(warehouseCode, "almoxarifado"),
                    NormalizeMovementDateInput(movementDateTime),
                    onlyBrc)
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public LotSummary[] LoadLotsByWarehouseAndMaterial(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string materialCode, string movementDateTime)
        {
            return _inventoryGateway.LoadLotsByWarehouseAndMaterial(
                    profile,
                    GetSettings(configuration, profile),
                    NormalizeRequiredReferenceCode(warehouseCode, "almoxarifado"),
                    NormalizeRequiredReferenceCode(materialCode, "material"),
                    NormalizeMovementDateInput(movementDateTime))
                .OrderBy(item => ParseStoredDate(item.ExpirationDate))
                .ThenBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public InventoryPlanningCandidateItem[] LoadPlanningCandidatesByWarehouse(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string warehouseCode,
            string movementDateTime,
            bool onlyBrc)
        {
            return _inventoryGateway.LoadPlanningCandidatesByWarehouse(
                    profile,
                    GetSettings(configuration, profile),
                    NormalizeRequiredReferenceCode(warehouseCode, "almoxarifado"),
                    NormalizeMovementDateInput(movementDateTime),
                    onlyBrc)
                .OrderBy(item => item.MaterialDescription ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.MaterialCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.LotName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.LotCode ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public InventorySummary[] SearchInventories(AppConfiguration configuration, DatabaseProfile profile, string filter)
        {
            return _inventoryGateway.SearchInventories(profile, GetSettings(configuration, profile), NormalizeText(filter))
                .OrderByDescending(item => ParseStoredDate(item.CreatedAt))
                .ThenByDescending(item => ExtractNumericSuffix(item.Number))
                .ThenByDescending(item => item.Number ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public InventoryDetail LoadInventory(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var detail = _inventoryGateway.LoadInventoryDetail(
                profile,
                GetSettings(configuration, profile),
                NormalizeInventoryNumber(number));

            if (detail == null)
            {
                throw new InvalidOperationException("Inventario nao encontrado.");
            }

            return detail;
        }

        public RecordLockResult TryLockInventory(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            return _inventoryGateway.TryLockInventory(
                profile,
                GetSettings(configuration, profile),
                NormalizeInventoryNumber(number),
                NormalizeActor(userName));
        }

        public void ReleaseInventoryLock(AppConfiguration configuration, DatabaseProfile profile, string number, string userName)
        {
            var normalizedNumber = NormalizeInventoryNumber(number, allowEmpty: true);
            if (string.IsNullOrWhiteSpace(normalizedNumber))
            {
                return;
            }

            _inventoryGateway.ReleaseInventoryLock(
                profile,
                GetSettings(configuration, profile),
                normalizedNumber,
                NormalizeActor(userName));
        }

        public InventoryItemConflict FindItemConflict(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string inventoryNumber,
            string warehouseCode,
            string materialCode,
            string lotCode)
        {
            return _inventoryGateway.FindItemConflict(
                profile,
                GetSettings(configuration, profile),
                NormalizeInventoryNumber(inventoryNumber, allowEmpty: true),
                NormalizeRequiredReferenceCode(warehouseCode, "almoxarifado"),
                NormalizeRequiredReferenceCode(materialCode, "material"),
                NormalizeRequiredReferenceCode(lotCode, "lote"));
        }

        public OpenMovementLockSummary[] LoadOpenMovements(AppConfiguration configuration, DatabaseProfile profile, int limit)
        {
            return _inventoryGateway.LoadOpenMovements(profile, GetSettings(configuration, profile), Math.Max(1, limit))
                .OrderByDescending(item => ParseStoredDate(item.LockedAt))
                .ToArray();
        }

        public decimal GetStockBalanceAt(AppConfiguration configuration, DatabaseProfile profile, string warehouseCode, string materialCode, string lotCode, string movementDateTime)
        {
            return _inventoryGateway.GetStockBalanceAt(
                profile,
                GetSettings(configuration, profile),
                NormalizeRequiredReferenceCode(warehouseCode, "almoxarifado"),
                NormalizeRequiredReferenceCode(materialCode, "material"),
                NormalizeRequiredReferenceCode(lotCode, "lote"),
                NormalizeMovementDateInput(movementDateTime));
        }

        public void CreateInventory(AppConfiguration configuration, DatabaseProfile profile, SaveInventoryRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeSaveRequest(request, profile, settings, allowExisting: false);
            _inventoryGateway.CreateInventory(profile, settings, normalized);
            SafeAudit(profile, normalized.ActorUserName, "Movimentacao de inventario",
                "Tela=Inventario; Acao=Salvar; Documento=" + normalized.Number
                + "; Programado=" + normalized.ScheduledDateTime
                + "; Pontos=" + normalized.MaxOpenPoints
                + "; Itens=" + normalized.Items.Length,
                settings);
        }

        public void UpdateInventoryPlanning(AppConfiguration configuration, DatabaseProfile profile, SaveInventoryRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeSaveRequest(request, profile, settings, allowExisting: true);
            _inventoryGateway.UpdateInventoryPlanning(profile, settings, normalized);
            SafeAudit(profile, normalized.ActorUserName, "Movimentacao de inventario",
                "Tela=Inventario; Acao=Alterar; Documento=" + normalized.Number
                + "; Programado=" + normalized.ScheduledDateTime
                + "; Pontos=" + normalized.MaxOpenPoints
                + "; Itens=" + normalized.Items.Length,
                settings);
        }

        public InventoryPointSummary AddPoint(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, InventoryPointInput point, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedPoint = NormalizePoint(point);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            var created = _inventoryGateway.AddPoint(profile, settings, normalizedNumber, normalizedPoint, normalizedUser);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=PontoNovo; Documento=" + normalizedNumber
                + "; Ponto=" + normalizedPoint.PointName
                + "; IP=" + normalizedPoint.IpAddress,
                settings);
            return created;
        }

        public void ClosePoint(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            _inventoryGateway.ClosePoint(profile, settings, normalizedNumber, pointId, normalizedUser);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=PontoFechar; Documento=" + normalizedNumber + "; Ponto=" + pointId,
                settings);
        }

        public void ReopenPoint(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            _inventoryGateway.ReopenPoint(profile, settings, normalizedNumber, pointId, normalizedUser);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=PontoReabrir; Documento=" + normalizedNumber + "; Ponto=" + pointId,
                settings);
        }

        public void DeletePoint(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            _inventoryGateway.DeletePoint(profile, settings, normalizedNumber, pointId, normalizedUser);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=PontoExcluir; Documento=" + normalizedNumber + "; Ponto=" + pointId,
                settings);
        }

        public void RegisterCount(AppConfiguration configuration, DatabaseProfile profile, RegisterInventoryCountRequest request)
        {
            var settings = GetSettings(configuration, profile);
            var normalized = NormalizeCountRequest(request);
            _inventoryGateway.RegisterCount(profile, settings, normalized);
            SafeAudit(profile, normalized.ActorUserName, "Movimentacao de inventario",
                "Tela=Inventario; Acao=Contagem; Documento=" + normalized.InventoryNumber
                + "; Ponto=" + normalized.PointId
                + "; Item=" + normalized.WarehouseCode + "/" + normalized.MaterialCode + "/" + normalized.LotCode
                + "; Quantidade=" + normalized.Quantity.ToString("0.##", CultureInfo.InvariantCulture),
                settings);
        }

        public void TouchPointHeartbeat(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId)
        {
            _inventoryGateway.TouchPointHeartbeat(
                profile,
                GetSettings(configuration, profile),
                NormalizeInventoryNumber(inventoryNumber),
                pointId);
        }

        public int ApplyZeroCounts(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, int pointId, string userName, string ipAddress, string computerName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            var count = _inventoryGateway.ApplyZeroCounts(
                profile,
                settings,
                normalizedNumber,
                pointId,
                normalizedUser,
                NormalizeText(ipAddress),
                NormalizeText(computerName));
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=LancarZero; Documento=" + normalizedNumber + "; Ponto=" + pointId + "; Itens=" + count,
                settings);
            return count;
        }

        public void StartInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName, bool allowEarlyStart)
        {
            var settings = GetSettings(configuration, profile);
            EnsureNoOpenMovements(profile, settings);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            _inventoryGateway.StartInventory(profile, settings, normalizedNumber, normalizedUser, allowEarlyStart);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=Iniciar; Documento=" + normalizedNumber,
                settings);
        }

        public void CloseInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            _inventoryGateway.CloseInventory(profile, settings, normalizedNumber, normalizedUser);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=Fechar; Documento=" + normalizedNumber,
                settings);
        }

        public void ReopenInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName)
        {
            var settings = GetSettings(configuration, profile);
            EnsureNoOpenMovements(profile, settings);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            _inventoryGateway.ReopenInventory(profile, settings, normalizedNumber, normalizedUser);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=Reabrir; Documento=" + normalizedNumber,
                settings);
        }

        public int FinalizeInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            var adjustments = _inventoryGateway.FinalizeInventory(profile, settings, normalizedNumber, normalizedUser);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=Encerrar; Documento=" + normalizedNumber + "; Ajustes=" + adjustments,
                settings);
            return adjustments;
        }

        public void CancelInventory(AppConfiguration configuration, DatabaseProfile profile, string inventoryNumber, string userName, string reason)
        {
            var settings = GetSettings(configuration, profile);
            var normalizedNumber = NormalizeInventoryNumber(inventoryNumber);
            var normalizedUser = NormalizeActor(userName);
            var normalizedReason = NormalizeText(reason);
            var detail = _inventoryGateway.LoadInventoryDetail(profile, settings, normalizedNumber);
            if (detail == null)
            {
                throw new InvalidOperationException("Inventario nao encontrado.");
            }

            if (!CancelableStatuses.Contains(NormalizeStatus(detail.Status), StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Somente inventario pendente, iniciado, em contagem ou fechado pode ser cancelado.");
            }

            var requireDifferentCanceler = NormalizeYesNo(_inventoryGateway.GetParameter(profile, settings, "inventario_cancelador_diferente_criador", "SIM"));
            if (requireDifferentCanceler
                && !string.IsNullOrWhiteSpace(detail.CreatedBy)
                && string.Equals(detail.CreatedBy, normalizedUser, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Quem criou o inventario nao pode cancelar. Use outro usuario com permissao.");
            }

            _inventoryGateway.CancelInventory(profile, settings, normalizedNumber, normalizedUser, normalizedReason);
            SafeAudit(profile, normalizedUser, "Movimentacao de inventario",
                "Tela=Inventario; Acao=Cancelar; Documento=" + normalizedNumber + "; Motivo=" + normalizedReason,
                settings);
        }

        private SaveInventoryRequest NormalizeSaveRequest(
            SaveInventoryRequest request,
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            bool allowExisting)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.Number = NormalizeInventoryNumber(request.Number, allowEmpty: !allowExisting);
            if (string.IsNullOrWhiteSpace(request.Number))
            {
                request.Number = _inventoryGateway.GenerateNextInventoryNumber(profile, settings);
            }

            request.ActorUserName = NormalizeActor(request.ActorUserName);
            request.Observation = NormalizeObservation(request.Observation);
            request.MaxOpenPoints = NormalizeMaxOpenPoints(request.MaxOpenPoints);
            request.CreatedDateTime = NormalizeCreatedDate(request.CreatedDateTime);
            request.ScheduledDateTime = NormalizeOptionalDateTime(request.ScheduledDateTime);
            request.Items = NormalizeItems(request.Items);
            request.Points = NormalizePoints(request.Points);

            if (request.Items.Length == 0)
            {
                throw new InvalidOperationException("Adicione ao menos 1 item.");
            }

            EnsureNoDuplicateItems(request.Items);
            ValidateItemConflicts(profile, settings, request.Number, request.Items);
            ValidatePointDrafts(request.Points, request.MaxOpenPoints);

            var existing = _inventoryGateway.LoadInventoryDetail(profile, settings, request.Number);
            if (!allowExisting)
            {
                if (existing != null && !string.Equals(NormalizeStatus(existing.Status), "CANCELADO", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("O inventario " + request.Number + " ja existe. Consulte o documento e use Alterar.");
                }
            }
            else
            {
                if (existing == null)
                {
                    throw new InvalidOperationException("Inventario nao encontrado para alteracao.");
                }

                if (!string.Equals(NormalizeStatus(existing.Status), "PENDENTE", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Somente inventarios pendentes podem ter o planejamento alterado.");
                }
            }

            return request;
        }

        private RegisterInventoryCountRequest NormalizeCountRequest(RegisterInventoryCountRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.InventoryNumber = NormalizeInventoryNumber(request.InventoryNumber);
            request.ActorUserName = NormalizeActor(request.ActorUserName);
            request.WarehouseCode = NormalizeRequiredReferenceCode(request.WarehouseCode, "almoxarifado");
            request.MaterialCode = NormalizeRequiredReferenceCode(request.MaterialCode, "material");
            request.LotCode = NormalizeRequiredReferenceCode(request.LotCode, "lote");
            request.IpAddress = NormalizeText(request.IpAddress);
            request.ComputerName = NormalizeText(request.ComputerName);
            if (request.Quantity < 0M)
            {
                throw new InvalidOperationException("Quantidade contada nao pode ser negativa.");
            }

            request.CountedAt = NormalizeOptionalDateTime(request.CountedAt);
            if (string.IsNullOrWhiteSpace(request.CountedAt))
            {
                request.CountedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }

            request.EnsureOriginUid();
            return request;
        }

        private void EnsureNoOpenMovements(DatabaseProfile profile, ConnectionResilienceSettings settings)
        {
            var openMovements = _inventoryGateway.LoadOpenMovements(profile, settings, 20).ToArray();
            if (openMovements.Length == 0)
            {
                return;
            }

            var preview = string.Join(
                "\n",
                openMovements.Take(8).Select(item => (item.Type ?? string.Empty) + " " + (item.DocumentNumber ?? string.Empty) + " usuario=" + (item.UserName ?? string.Empty)).ToArray());

            throw new InvalidOperationException(
                "Nao e permitido abrir ou iniciar inventario com lancamentos em andamento.\n\n" + preview);
        }

        private void ValidateItemConflicts(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string inventoryNumber,
            IEnumerable<InventoryItemInput> items)
        {
            foreach (var item in items)
            {
                var conflict = _inventoryGateway.FindItemConflict(
                    profile,
                    settings,
                    inventoryNumber,
                    item.WarehouseCode,
                    item.MaterialCode,
                    item.LotCode);

                if (conflict != null && !string.IsNullOrWhiteSpace(conflict.InventoryNumber))
                {
                    throw new InvalidOperationException(
                        "Item ja esta em inventario aberto.\n"
                        + "Inventario: " + conflict.InventoryNumber + "\n"
                        + "Status: " + conflict.InventoryStatus);
                }
            }
        }

        private static void EnsureNoDuplicateItems(IEnumerable<InventoryItemInput> items)
        {
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in items)
            {
                var key = item.WarehouseCode + "|" + item.MaterialCode + "|" + item.LotCode;
                if (!keys.Add(key))
                {
                    throw new InvalidOperationException("Ha item duplicado no planejamento do inventario: " + key.Replace("|", "/") + ".");
                }
            }
        }

        private static void ValidatePointDrafts(IEnumerable<InventoryPointInput> points, int maxOpenPoints)
        {
            var openPoints = (points ?? Array.Empty<InventoryPointInput>())
                .Where(point => string.Equals(NormalizeStatus(point.Status), "ABERTO", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (openPoints.Length > maxOpenPoints)
            {
                throw new InvalidOperationException("Limite de pontos abertos atingido (" + openPoints.Length + "/" + maxOpenPoints + ").");
            }

            var ips = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var point in openPoints)
            {
                if (string.IsNullOrWhiteSpace(point.IpAddress))
                {
                    continue;
                }

                if (!ips.Add(point.IpAddress))
                {
                    throw new InvalidOperationException("Nao e permitido abrir dois pontos no mesmo IP neste inventario.");
                }
            }
        }

        private static InventoryItemInput[] NormalizeItems(IEnumerable<InventoryItemInput> items)
        {
            return (items ?? Array.Empty<InventoryItemInput>())
                .Select(item => item ?? new InventoryItemInput())
                .Select(item =>
                {
                    item.WarehouseCode = NormalizeRequiredReferenceCode(item.WarehouseCode, "almoxarifado");
                    item.MaterialCode = NormalizeRequiredReferenceCode(item.MaterialCode, "material");
                    item.LotCode = NormalizeRequiredReferenceCode(item.LotCode, "lote");
                    return item;
                })
                .ToArray();
        }

        private static InventoryPointInput[] NormalizePoints(IEnumerable<InventoryPointInput> points)
        {
            return (points ?? Array.Empty<InventoryPointInput>())
                .Select(point => point ?? new InventoryPointInput())
                .Select(NormalizePoint)
                .ToArray();
        }

        private static InventoryPointInput NormalizePoint(InventoryPointInput point)
        {
            point.PointName = NormalizeText(point.PointName);
            if (string.IsNullOrWhiteSpace(point.PointName))
            {
                throw new InvalidOperationException("Informe o nome do ponto.");
            }

            point.IpAddress = NormalizeText(point.IpAddress);
            point.ComputerName = NormalizeText(point.ComputerName);
            point.Status = string.IsNullOrWhiteSpace(point.Status) ? "ABERTO" : NormalizeStatus(point.Status);
            return point;
        }

        private static string NormalizeCreatedDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }

            return ParseRequiredDateTime(value, "Data/Hora de criacao invalida.").ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static string NormalizeOptionalDateTime(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return ParseRequiredDateTime(value, "Data/Hora invalida.").ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static DateTime ParseRequiredDateTime(string value, string errorMessage)
        {
            DateTime parsed;
            var formats = new[] { "dd/MM/yyyy HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm" };
            if (!DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
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

        private static string NormalizeInventoryNumber(string value, bool allowEmpty = false)
        {
            var normalized = NormalizeText(value).ToUpperInvariant();
            if (normalized.Length == 0 && !allowEmpty)
            {
                throw new InvalidOperationException("Informe o numero do inventario.");
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

        private static string NormalizeObservation(string value)
        {
            var normalized = NormalizeText(value);
            return normalized.Length <= 40 ? normalized : normalized.Substring(0, 40);
        }

        private static int NormalizeMaxOpenPoints(int value)
        {
            var normalized = value <= 0 ? 1 : value;
            if (normalized < 1 || normalized > 20)
            {
                throw new InvalidOperationException("Max pontos deve estar entre 1 e 20.");
            }

            return normalized;
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

        private static string NormalizeStatus(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
        }

        private static bool NormalizeYesNo(string value)
        {
            var normalized = NormalizeStatus(value);
            return normalized == "SIM"
                || normalized == "1"
                || normalized == "TRUE"
                || normalized == "YES"
                || normalized == "ON";
        }

        private static int ExtractNumericSuffix(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
            int parsed;
            return int.TryParse(digits, out parsed) ? parsed : int.MinValue;
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
