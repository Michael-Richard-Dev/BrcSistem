using System;
using System.Collections.Generic;
using BRCSISTEM.Application.Abstractions;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Services
{
    public sealed class DatabaseMaintenanceService
    {
        private readonly IDatabaseMaintenanceGateway _gateway;
        private readonly IAuditTrailService _auditTrailService;

        public DatabaseMaintenanceService(IDatabaseMaintenanceGateway gateway, IAuditTrailService auditTrailService)
        {
            _gateway = gateway;
            _auditTrailService = auditTrailService;
        }

        // ── Audit log ──────────────────────────────────────────────────────────

        public IReadOnlyCollection<AuditLogEntry> LoadAuditLog(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string filterUser,
            string filterAction,
            string filterDateFrom,
            string filterDateTo,
            string searchText,
            int pageSize,
            int offset)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadAuditLog(profile, settings, filterUser, filterAction, filterDateFrom, filterDateTo, searchText, pageSize, offset);
        }

        public int CountAuditLog(
            AppConfiguration configuration,
            DatabaseProfile profile,
            string filterUser,
            string filterAction,
            string filterDateFrom,
            string filterDateTo,
            string searchText)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.CountAuditLog(profile, settings, filterUser, filterAction, filterDateFrom, filterDateTo, searchText);
        }

        public IReadOnlyCollection<string> LoadAuditUsers(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadAuditUsers(profile, settings);
        }

        public IReadOnlyCollection<string> LoadAuditActions(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadAuditActions(profile, settings);
        }

        // ── System parameters ──────────────────────────────────────────────────

        public IReadOnlyCollection<SystemParameter> LoadSystemParameters(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadSystemParameters(profile, settings);
        }

        public void SaveSystemParameter(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string key, string value)
        {
            var settings = GetSettings(configuration, profile);
            _gateway.SaveSystemParameter(profile, settings, key, value);
            SafeAudit(profile, actorUserName, "Alteracao de parametro",
                $"Tela=Parametros; Chave={key}; Valor={value}", settings);
        }

        // ── Shifts ─────────────────────────────────────────────────────────────

        public IReadOnlyCollection<ShiftSummary> LoadShifts(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadShifts(profile, settings);
        }

        public void AddShift(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Informe o nome do turno.");
            var settings = GetSettings(configuration, profile);
            _gateway.AddShift(profile, settings, name, description);
            SafeAudit(profile, actorUserName, "Turno adicionado", $"Tela=Parametros; Turno={name.Trim()}", settings);
        }

        public void UpdateShift(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, int id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Informe o nome do turno.");
            var settings = GetSettings(configuration, profile);
            _gateway.UpdateShift(profile, settings, id, name, description);
            SafeAudit(profile, actorUserName, "Turno atualizado", $"Tela=Parametros; Id={id}; Turno={name.Trim()}", settings);
        }

        public void DeleteShift(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, int id, string name)
        {
            var settings = GetSettings(configuration, profile);
            _gateway.DeleteShift(profile, settings, id);
            SafeAudit(profile, actorUserName, "Turno excluido", $"Tela=Parametros; Turno={name}", settings);
        }

        // ── Requisition reasons ────────────────────────────────────────────────

        public IReadOnlyCollection<RequisitionReasonSummary> LoadRequisitionReasons(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadRequisitionReasons(profile, settings);
        }

        public void AddRequisitionReason(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Informe o nome do motivo.");
            var settings = GetSettings(configuration, profile);
            _gateway.AddRequisitionReason(profile, settings, name, description);
            SafeAudit(profile, actorUserName, "Motivo de requisicao adicionado", $"Tela=Parametros; Motivo={name.Trim()}", settings);
        }

        public void UpdateRequisitionReason(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, int id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Informe o nome do motivo.");
            var settings = GetSettings(configuration, profile);
            _gateway.UpdateRequisitionReason(profile, settings, id, name, description);
            SafeAudit(profile, actorUserName, "Motivo de requisicao atualizado", $"Tela=Parametros; Id={id}; Motivo={name.Trim()}", settings);
        }

        public void DeleteRequisitionReason(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, int id, string name)
        {
            var settings = GetSettings(configuration, profile);
            _gateway.DeleteRequisitionReason(profile, settings, id);
            SafeAudit(profile, actorUserName, "Motivo de requisicao excluido", $"Tela=Parametros; Motivo={name}", settings);
        }

        // ── Warehouse access ───────────────────────────────────────────────────

        public IReadOnlyCollection<WarehouseAccessEntry> LoadGrantedWarehouseAccess(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadGrantedWarehouseAccess(profile, settings, userName);
        }

        public IReadOnlyCollection<WarehouseAccessEntry> LoadAvailableWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadAvailableWarehousesForUser(profile, settings, userName);
        }

        public void GrantWarehouseAccess(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string targetUser, string warehouseCode)
        {
            var settings = GetSettings(configuration, profile);
            _gateway.GrantWarehouseAccess(profile, settings, targetUser, warehouseCode);
            SafeAudit(profile, actorUserName, "Acesso ao almoxarifado concedido",
                $"Tela=Parametros; Usuario={targetUser}; Almoxarifado={warehouseCode}", settings);
        }

        public void RevokeWarehouseAccess(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string targetUser, string warehouseCode)
        {
            var settings = GetSettings(configuration, profile);
            _gateway.RevokeWarehouseAccess(profile, settings, targetUser, warehouseCode);
            SafeAudit(profile, actorUserName, "Acesso ao almoxarifado revogado",
                $"Tela=Parametros; Usuario={targetUser}; Almoxarifado={warehouseCode}", settings);
        }

        // ── Locked records ─────────────────────────────────────────────────────

        public IReadOnlyCollection<OpenMovementLockSummary> LoadLockedRecords(AppConfiguration configuration, DatabaseProfile profile, string tableName, string documentNumber, string supplier)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadLockedRecords(profile, settings, tableName, documentNumber, supplier);
        }

        public void UnlockRecord(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string tableName, string documentNumber, string supplier)
        {
            var settings = GetSettings(configuration, profile);
            _gateway.UnlockRecord(profile, settings, tableName, documentNumber, supplier);
            SafeAudit(profile, actorUserName, "Registro desbloqueado (admin)",
                $"Tela=Parametros; Tabela={tableName}; Documento={documentNumber}", settings);
        }

        // ── Remove note ────────────────────────────────────────────────────────

        public DocumentMaintenanceHeader LoadNoteHeader(AppConfiguration configuration, DatabaseProfile profile, string number, string supplier)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadNoteHeader(profile, settings, number, supplier);
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadNoteItems(AppConfiguration configuration, DatabaseProfile profile, string number, string supplier)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadNoteItems(profile, settings, number, supplier);
        }

        public RemoveNoteResult RemoveNote(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string supplier)
        {
            var settings = GetSettings(configuration, profile);
            var result = _gateway.RemoveNote(profile, settings, number, supplier);
            SafeAudit(profile, actorUserName, "REMOCAO_NOTA",
                $"Nota {result.Number} - Fornecedor {result.Supplier} - {result.RemovedItems} itens - {result.RemovedMovements} movimentos", settings);
            return result;
        }

        public IReadOnlyCollection<InboundReceiptReactivationEntry> SearchCancelledInboundReceipts(AppConfiguration configuration, DatabaseProfile profile, string number, string supplier, int limit)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.SearchCancelledInboundReceipts(profile, settings, number, supplier, limit);
        }

        public void ReactivateInboundReceipt(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string supplier, int version)
        {
            var settings = GetSettings(configuration, profile);
            _gateway.ReactivateInboundReceipt(profile, settings, number, supplier, version);

            var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            SafeAudit(profile, actorUserName, "Movimentacao de entrada",
                $"Tela=Reativar; Acao=Reativar; Usuario={actorUserName}; Numero={number}; Fornecedor={supplier}; Versao={version}; DataHora={timestamp}; Status=De CANCELADA para ATIVO",
                settings);
            SafeAudit(profile, actorUserName, "Movimentos de estoque",
                $"Reativacao de nota cancelada; Numero={number}; Fornecedor={supplier}; Versao={version}; Usuario={actorUserName}",
                settings);
        }

        // ── Remove transfer ────────────────────────────────────────────────────

        public DocumentMaintenanceHeader LoadTransferHeader(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadTransferHeader(profile, settings, number);
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadTransferItems(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadTransferItems(profile, settings, number);
        }

        public RemoveTransferResult RemoveTransfer(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number)
        {
            var settings = GetSettings(configuration, profile);
            var result = _gateway.RemoveTransfer(profile, settings, number);
            SafeAudit(profile, actorUserName, "REMOCAO_TRANSFERENCIA",
                $"Transferencia {result.Number} - {result.RemovedItems} itens - {result.RemovedMovements} movimentos", settings);
            return result;
        }

        // ── Remove production output ───────────────────────────────────────────

        public DocumentMaintenanceHeader LoadProductionOutputHeader(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadProductionOutputHeader(profile, settings, number);
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadProductionOutputItems(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadProductionOutputItems(profile, settings, number);
        }

        public RemoveProductionOutputResult RemoveProductionOutput(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number)
        {
            var settings = GetSettings(configuration, profile);
            var result = _gateway.RemoveProductionOutput(profile, settings, number);
            SafeAudit(profile, actorUserName, "REMOCAO_SAIDA",
                $"Saida de Producao {result.Number} - {result.RemovedItems} itens - {result.RemovedMovements} movimentos", settings);
            return result;
        }

        // ── Remove requisition ─────────────────────────────────────────────────

        public DocumentMaintenanceHeader LoadRequisitionHeader(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadRequisitionHeader(profile, settings, number);
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadRequisitionItems(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadRequisitionItems(profile, settings, number);
        }

        public RemoveRequisitionResult RemoveRequisition(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number)
        {
            var settings = GetSettings(configuration, profile);
            var result = _gateway.RemoveRequisition(profile, settings, number);
            SafeAudit(profile, actorUserName, "REMOCAO_REQUISICAO",
                $"Requisicao {result.Number} - {result.RemovedItems} itens - {result.RemovedMovements} movimentos", settings);
            return result;
        }

        // ── Change note date ───────────────────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveNotes(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadActiveNotes(profile, settings);
        }

        public ChangeDateResult ChangeNoteDate(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string supplier, string newDate)
        {
            if (string.IsNullOrWhiteSpace(newDate)) throw new InvalidOperationException("Informe a nova data.");
            var settings = GetSettings(configuration, profile);
            var result = _gateway.ChangeNoteDate(profile, settings, number, supplier, newDate);
            var resolvedSupplier = !string.IsNullOrEmpty(result.Supplier) ? result.Supplier : supplier;
            // Mensagem segue views/bd_alterar_data_entrada.py::_alterar
            SafeAudit(profile, actorUserName, "ALTERAR_DATA_ENTRADA",
                $"Nota {number} - Fornecedor: {resolvedSupplier} - Data alterada para {newDate} (linhas: notas={result.HeaderRowsUpdated}, mov={result.MovementRowsUpdated})",
                settings);
            return result;
        }

        // ── Change transfer date ───────────────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveTransfers(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadActiveTransfers(profile, settings);
        }

        public ChangeDateResult ChangeTransferDate(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string newDate)
        {
            if (string.IsNullOrWhiteSpace(newDate)) throw new InvalidOperationException("Informe a nova data.");
            var settings = GetSettings(configuration, profile);
            var result = _gateway.ChangeTransferDate(profile, settings, number, newDate);
            SafeAudit(profile, actorUserName, "ALTERAR_DATA_TRANSFERENCIA",
                $"Transferencia {number}: data alterada para {newDate} (linhas: transf={result.HeaderRowsUpdated}, mov={result.MovementRowsUpdated})",
                settings);
            return result;
        }

        // ── Change production output date ──────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveProductionOutputs(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadActiveProductionOutputs(profile, settings);
        }

        public ChangeDateResult ChangeProductionOutputDate(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string newDate)
        {
            if (string.IsNullOrWhiteSpace(newDate)) throw new InvalidOperationException("Informe a nova data.");
            var settings = GetSettings(configuration, profile);
            var result = _gateway.ChangeProductionOutputDate(profile, settings, number, newDate);
            SafeAudit(profile, actorUserName, "ALTERAR_DATA_SAIDA_PRODUCAO",
                $"Saida {number}: data alterada para {newDate} (linhas: saida={result.HeaderRowsUpdated}, mov={result.MovementRowsUpdated})",
                settings);
            return result;
        }

        // ── Alert: divergent lot ───────────────────────────────────────────────

        public IReadOnlyCollection<DivergentLotEntry> DiagnoseDivergentLotEntries(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.DiagnoseDivergentLotEntries(profile, settings);
        }

        public void FixDivergentLotEntry(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, long movementId)
        {
            var settings = GetSettings(configuration, profile);
            _gateway.FixDivergentLotEntry(profile, settings, movementId);
            SafeAudit(profile, actorUserName, "Movimento com lote divergente inativado (alerta_entrada_lote_divergente)",
                $"Tela=AlertaLoteDivergente; MovimentoId={movementId}", settings);
        }

        // ── Alert: lot x material inconsistency ────────────────────────────────

        public IReadOnlyCollection<LotMaterialInconsistencyEntry> DiagnoseLotMaterialInconsistencies(AppConfiguration configuration, DatabaseProfile profile, string filterWarehouse, string filterMaterial, string filterLot)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.DiagnoseLotMaterialInconsistencies(profile, settings, filterWarehouse, filterMaterial, filterLot);
        }

        // ── Alert: negative stock ──────────────────────────────────────────────

        public IReadOnlyCollection<NegativeStockEntry> DiagnoseNegativeStock(AppConfiguration configuration, DatabaseProfile profile, string filterWarehouse, string filterMaterial, string filterLot)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.DiagnoseNegativeStock(profile, settings, filterWarehouse, filterMaterial, filterLot);
        }

        // ── Alert: duplicate lots ──────────────────────────────────────────────

        public IReadOnlyCollection<DuplicateLotEntry> DiagnoseDuplicateLotsByMaterial(AppConfiguration configuration, DatabaseProfile profile, string filterMaterial, string filterLotName, string filterLotCode)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.DiagnoseDuplicateLotsByMaterial(profile, settings, filterMaterial, filterLotName, filterLotCode);
        }

        // ── Alert: duplicate note movements ───────────────────────────────────

        public IReadOnlyCollection<DuplicateNoteMovementGroup> DiagnoseDuplicateNoteMovements(AppConfiguration configuration, DatabaseProfile profile)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.DiagnoseDuplicateNoteMovements(profile, settings);
        }

        public IReadOnlyCollection<DuplicateNoteMovementDetail> LoadDuplicateNoteMovementDetails(AppConfiguration configuration, DatabaseProfile profile, string noteNumber, string supplier)
        {
            var settings = GetSettings(configuration, profile);
            return _gateway.LoadDuplicateNoteMovementDetails(profile, settings, noteNumber, supplier);
        }

        public InactivateDuplicatesResult InactivateDuplicateNoteMovements(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, long[] movementIds, string noteNumber)
        {
            var settings = GetSettings(configuration, profile);
            var result = _gateway.InactivateDuplicateNoteMovements(profile, settings, movementIds);
            // Espelha registrar_log do Python inativar_movimentos_duplicados_notas:
            //   LIMPEZA_DUPLICIDADE_NOTA | IDs solicitados=[..] | IDs encontrados=[..] | IDs inativados=[..]
            SafeAudit(profile, actorUserName, "LIMPEZA_DUPLICIDADE_NOTA",
                $"Tela=AlertaMovDuplicados; Nota={noteNumber}; Solicitados=[{string.Join(",", result.RequestedIds)}]; Encontrados=[{string.Join(",", result.FoundIds)}]; Inativados=[{string.Join(",", result.InactivatedIds)}]",
                settings);
            return result;
        }

        // ── Private helpers ────────────────────────────────────────────────────

        private static ConnectionResilienceSettings GetSettings(AppConfiguration configuration, DatabaseProfile profile)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (profile == null) throw new InvalidOperationException("Banco de dados nao informado.");
            configuration.Normalize();
            return configuration.ConnectionSettings ?? ConnectionResilienceSettings.CreateDefault();
        }

        private void SafeAudit(DatabaseProfile profile, string userName, string action, string details, ConnectionResilienceSettings settings)
        {
            try { _auditTrailService.Write(profile, userName, action, details, settings); }
            catch { }
        }
    }
}
