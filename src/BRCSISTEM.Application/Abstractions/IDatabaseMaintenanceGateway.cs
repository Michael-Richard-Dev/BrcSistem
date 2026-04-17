using System.Collections.Generic;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Application.Abstractions
{
    public interface IDatabaseMaintenanceGateway
    {
        // ── Audit log ──────────────────────────────────────────────────────────
        IReadOnlyCollection<AuditLogEntry> LoadAuditLog(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string filterUser,
            string filterAction,
            string filterDateFrom,
            string filterDateTo,
            string searchText,
            int pageSize,
            int offset);

        int CountAuditLog(
            DatabaseProfile profile,
            ConnectionResilienceSettings settings,
            string filterUser,
            string filterAction,
            string filterDateFrom,
            string filterDateTo,
            string searchText);

        IReadOnlyCollection<string> LoadAuditUsers(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<string> LoadAuditActions(DatabaseProfile profile, ConnectionResilienceSettings settings);

        // ── System parameters ──────────────────────────────────────────────────
        IReadOnlyCollection<SystemParameter> LoadSystemParameters(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void SaveSystemParameter(DatabaseProfile profile, ConnectionResilienceSettings settings, string key, string value);

        // ── Shifts (turnos) ────────────────────────────────────────────────────
        IReadOnlyCollection<ShiftSummary> LoadShifts(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void AddShift(DatabaseProfile profile, ConnectionResilienceSettings settings, string name, string description);

        void UpdateShift(DatabaseProfile profile, ConnectionResilienceSettings settings, int id, string name, string description);

        void DeleteShift(DatabaseProfile profile, ConnectionResilienceSettings settings, int id);

        // ── Requisition reasons (motivos) ──────────────────────────────────────
        IReadOnlyCollection<RequisitionReasonSummary> LoadRequisitionReasons(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void AddRequisitionReason(DatabaseProfile profile, ConnectionResilienceSettings settings, string name, string description);

        void UpdateRequisitionReason(DatabaseProfile profile, ConnectionResilienceSettings settings, int id, string name, string description);

        void DeleteRequisitionReason(DatabaseProfile profile, ConnectionResilienceSettings settings, int id);

        // ── Warehouse access ───────────────────────────────────────────────────
        IReadOnlyCollection<WarehouseAccessEntry> LoadGrantedWarehouseAccess(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        IReadOnlyCollection<WarehouseAccessEntry> LoadAvailableWarehousesForUser(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName);

        void GrantWarehouseAccess(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName, string warehouseCode, string createdByUser);

        void RevokeWarehouseAccess(DatabaseProfile profile, ConnectionResilienceSettings settings, string userName, string warehouseCode);

        // ── Locked records ─────────────────────────────────────────────────────
        IReadOnlyCollection<OpenMovementLockSummary> LoadLockedRecords(DatabaseProfile profile, ConnectionResilienceSettings settings, string tableName, string documentNumber, string supplier);

        void UnlockRecord(DatabaseProfile profile, ConnectionResilienceSettings settings, string tableName, string documentNumber, string supplier);

        // ── Remove note (bd_remover_nota) ──────────────────────────────────────
        DocumentMaintenanceHeader LoadNoteHeader(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier);

        IReadOnlyCollection<DocumentMaintenanceItem> LoadNoteItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier);

        RemoveNoteResult RemoveNote(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier);

        IReadOnlyCollection<InboundReceiptReactivationEntry> SearchCancelledInboundReceipts(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier, int limit);

        void ReactivateInboundReceipt(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier, int version);

        // ── Remove transfer (bd_remover_transferencia) ─────────────────────────
        DocumentMaintenanceHeader LoadTransferHeader(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        IReadOnlyCollection<DocumentMaintenanceItem> LoadTransferItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        RemoveTransferResult RemoveTransfer(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        // ── Remove production output (bd_remover_saida) ────────────────────────
        DocumentMaintenanceHeader LoadProductionOutputHeader(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        IReadOnlyCollection<DocumentMaintenanceItem> LoadProductionOutputItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        RemoveProductionOutputResult RemoveProductionOutput(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        // ── Remove requisition (bd_remover_requisicao) ─────────────────────────
        DocumentMaintenanceHeader LoadRequisitionHeader(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        IReadOnlyCollection<DocumentMaintenanceItem> LoadRequisitionItems(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        RemoveRequisitionResult RemoveRequisition(DatabaseProfile profile, ConnectionResilienceSettings settings, string number);

        // ── Change note date (bd_alterar_data_entrada) ─────────────────────────
        IReadOnlyCollection<DocumentDateEntry> LoadActiveNotes(DatabaseProfile profile, ConnectionResilienceSettings settings);

        ChangeDateResult ChangeNoteDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string supplier, string newDate);

        // ── Change transfer date (bd_alterar_data_transferencia) ───────────────
        IReadOnlyCollection<DocumentDateEntry> LoadActiveTransfers(DatabaseProfile profile, ConnectionResilienceSettings settings);

        ChangeDateResult ChangeTransferDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string newDate);

        // ── Change production output date (bd_alterar_data_saida_producao) ──────
        IReadOnlyCollection<DocumentDateEntry> LoadActiveProductionOutputs(DatabaseProfile profile, ConnectionResilienceSettings settings);

        ChangeDateResult ChangeProductionOutputDate(DatabaseProfile profile, ConnectionResilienceSettings settings, string number, string newDate);

        // ── Alert: divergent lot entries ───────────────────────────────────────
        IReadOnlyCollection<DivergentLotEntry> DiagnoseDivergentLotEntries(DatabaseProfile profile, ConnectionResilienceSettings settings);

        void FixDivergentLotEntry(DatabaseProfile profile, ConnectionResilienceSettings settings, long movementId);

        // ── Alert: negative stock ──────────────────────────────────────────────
        IReadOnlyCollection<NegativeStockEntry> DiagnoseNegativeStock(DatabaseProfile profile, ConnectionResilienceSettings settings, string filterWarehouse, string filterMaterial, string filterLot);

        // ── Alert: duplicate lots by material ─────────────────────────────────
        IReadOnlyCollection<DuplicateLotEntry> DiagnoseDuplicateLotsByMaterial(DatabaseProfile profile, ConnectionResilienceSettings settings, string filterMaterial, string filterLotName, string filterLotCode);

        // ── Alert: lot x material inconsistency (bd_inconsistencias_lote_material) ──
        IReadOnlyCollection<LotMaterialInconsistencyEntry> DiagnoseLotMaterialInconsistencies(DatabaseProfile profile, ConnectionResilienceSettings settings, string filterWarehouse, string filterMaterial, string filterLot);

        // ── Alert: duplicate note movements ───────────────────────────────────
        IReadOnlyCollection<DuplicateNoteMovementGroup> DiagnoseDuplicateNoteMovements(DatabaseProfile profile, ConnectionResilienceSettings settings);

        IReadOnlyCollection<DuplicateNoteMovementDetail> LoadDuplicateNoteMovementDetails(DatabaseProfile profile, ConnectionResilienceSettings settings, string noteNumber, string supplier);

        InactivateDuplicatesResult InactivateDuplicateNoteMovements(DatabaseProfile profile, ConnectionResilienceSettings settings, long[] movementIds);
    }
}
