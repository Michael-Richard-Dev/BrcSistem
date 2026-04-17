using System.Collections.Generic;
using BRCSISTEM.Application.Services;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Controllers
{
    public sealed class DatabaseMaintenanceController
    {
        private readonly DatabaseMaintenanceService _service;

        public DatabaseMaintenanceController(DatabaseMaintenanceService service)
        {
            _service = service;
        }

        // ── Audit log ──────────────────────────────────────────────────────────

        public IReadOnlyCollection<AuditLogEntry> LoadAuditLog(AppConfiguration configuration, DatabaseProfile profile, string filterUser, string filterAction, string filterDateFrom, string filterDateTo, string searchText, int pageSize, int offset)
        {
            return _service.LoadAuditLog(configuration, profile, filterUser, filterAction, filterDateFrom, filterDateTo, searchText, pageSize, offset);
        }

        public int CountAuditLog(AppConfiguration configuration, DatabaseProfile profile, string filterUser, string filterAction, string filterDateFrom, string filterDateTo, string searchText)
        {
            return _service.CountAuditLog(configuration, profile, filterUser, filterAction, filterDateFrom, filterDateTo, searchText);
        }

        public IReadOnlyCollection<string> LoadAuditUsers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.LoadAuditUsers(configuration, profile);
        }

        public IReadOnlyCollection<string> LoadAuditActions(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.LoadAuditActions(configuration, profile);
        }

        // ── System parameters ──────────────────────────────────────────────────

        public IReadOnlyCollection<SystemParameter> LoadSystemParameters(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.LoadSystemParameters(configuration, profile);
        }

        public void SaveSystemParameter(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string key, string value)
        {
            _service.SaveSystemParameter(configuration, profile, actorUserName, key, value);
        }

        // ── Shifts ─────────────────────────────────────────────────────────────

        public IReadOnlyCollection<ShiftSummary> LoadShifts(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.LoadShifts(configuration, profile);
        }

        public void AddShift(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string name, string description)
        {
            _service.AddShift(configuration, profile, actorUserName, name, description);
        }

        public void UpdateShift(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, int id, string name, string description)
        {
            _service.UpdateShift(configuration, profile, actorUserName, id, name, description);
        }

        public void DeleteShift(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, int id, string name)
        {
            _service.DeleteShift(configuration, profile, actorUserName, id, name);
        }

        // ── Requisition reasons ────────────────────────────────────────────────

        public IReadOnlyCollection<RequisitionReasonSummary> LoadRequisitionReasons(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.LoadRequisitionReasons(configuration, profile);
        }

        public void AddRequisitionReason(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string name, string description)
        {
            _service.AddRequisitionReason(configuration, profile, actorUserName, name, description);
        }

        public void UpdateRequisitionReason(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, int id, string name, string description)
        {
            _service.UpdateRequisitionReason(configuration, profile, actorUserName, id, name, description);
        }

        public void DeleteRequisitionReason(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, int id, string name)
        {
            _service.DeleteRequisitionReason(configuration, profile, actorUserName, id, name);
        }

        // ── Warehouse access ───────────────────────────────────────────────────

        public IReadOnlyCollection<WarehouseAccessEntry> LoadGrantedWarehouseAccess(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _service.LoadGrantedWarehouseAccess(configuration, profile, userName);
        }

        public IReadOnlyCollection<WarehouseAccessEntry> LoadAvailableWarehousesForUser(AppConfiguration configuration, DatabaseProfile profile, string userName)
        {
            return _service.LoadAvailableWarehousesForUser(configuration, profile, userName);
        }

        public void GrantWarehouseAccess(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string targetUser, string warehouseCode)
        {
            _service.GrantWarehouseAccess(configuration, profile, actorUserName, targetUser, warehouseCode);
        }

        public void RevokeWarehouseAccess(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string targetUser, string warehouseCode)
        {
            _service.RevokeWarehouseAccess(configuration, profile, actorUserName, targetUser, warehouseCode);
        }

        // ── Locked records ─────────────────────────────────────────────────────

        public IReadOnlyCollection<OpenMovementLockSummary> LoadLockedRecords(AppConfiguration configuration, DatabaseProfile profile, string tableName, string documentNumber, string supplier)
        {
            return _service.LoadLockedRecords(configuration, profile, tableName, documentNumber, supplier);
        }

        public void UnlockRecord(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string tableName, string documentNumber, string supplier)
        {
            _service.UnlockRecord(configuration, profile, actorUserName, tableName, documentNumber, supplier);
        }

        // ── Remove documents ───────────────────────────────────────────────────

        public DocumentMaintenanceHeader LoadNoteHeader(AppConfiguration configuration, DatabaseProfile profile, string number, string supplier)
        {
            return _service.LoadNoteHeader(configuration, profile, number, supplier);
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadNoteItems(AppConfiguration configuration, DatabaseProfile profile, string number, string supplier)
        {
            return _service.LoadNoteItems(configuration, profile, number, supplier);
        }

        public void RemoveNote(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string supplier)
        {
            _service.RemoveNote(configuration, profile, actorUserName, number, supplier);
        }

        public IReadOnlyCollection<InboundReceiptReactivationEntry> SearchCancelledInboundReceipts(AppConfiguration configuration, DatabaseProfile profile, string number, string supplier, int limit)
        {
            return _service.SearchCancelledInboundReceipts(configuration, profile, number, supplier, limit);
        }

        public void ReactivateInboundReceipt(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string supplier, int version)
        {
            _service.ReactivateInboundReceipt(configuration, profile, actorUserName, number, supplier, version);
        }

        public DocumentMaintenanceHeader LoadTransferHeader(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _service.LoadTransferHeader(configuration, profile, number);
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadTransferItems(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _service.LoadTransferItems(configuration, profile, number);
        }

        public void RemoveTransfer(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number)
        {
            _service.RemoveTransfer(configuration, profile, actorUserName, number);
        }

        public DocumentMaintenanceHeader LoadProductionOutputHeader(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _service.LoadProductionOutputHeader(configuration, profile, number);
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadProductionOutputItems(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _service.LoadProductionOutputItems(configuration, profile, number);
        }

        public void RemoveProductionOutput(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number)
        {
            _service.RemoveProductionOutput(configuration, profile, actorUserName, number);
        }

        public DocumentMaintenanceHeader LoadRequisitionHeader(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _service.LoadRequisitionHeader(configuration, profile, number);
        }

        public IReadOnlyCollection<DocumentMaintenanceItem> LoadRequisitionItems(AppConfiguration configuration, DatabaseProfile profile, string number)
        {
            return _service.LoadRequisitionItems(configuration, profile, number);
        }

        public void RemoveRequisition(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number)
        {
            _service.RemoveRequisition(configuration, profile, actorUserName, number);
        }

        // ── Change dates ───────────────────────────────────────────────────────

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveNotes(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.LoadActiveNotes(configuration, profile);
        }

        public ChangeDateResult ChangeNoteDate(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string supplier, string newDate)
        {
            return _service.ChangeNoteDate(configuration, profile, actorUserName, number, supplier, newDate);
        }

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveTransfers(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.LoadActiveTransfers(configuration, profile);
        }

        public ChangeDateResult ChangeTransferDate(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string newDate)
        {
            return _service.ChangeTransferDate(configuration, profile, actorUserName, number, newDate);
        }

        public IReadOnlyCollection<DocumentDateEntry> LoadActiveProductionOutputs(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.LoadActiveProductionOutputs(configuration, profile);
        }

        public ChangeDateResult ChangeProductionOutputDate(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, string number, string newDate)
        {
            return _service.ChangeProductionOutputDate(configuration, profile, actorUserName, number, newDate);
        }

        // ── Alerts ─────────────────────────────────────────────────────────────

        public IReadOnlyCollection<DivergentLotEntry> DiagnoseDivergentLotEntries(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.DiagnoseDivergentLotEntries(configuration, profile);
        }

        public void FixDivergentLotEntry(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, long movementId)
        {
            _service.FixDivergentLotEntry(configuration, profile, actorUserName, movementId);
        }

        public IReadOnlyCollection<NegativeStockEntry> DiagnoseNegativeStock(AppConfiguration configuration, DatabaseProfile profile, string filterWarehouse, string filterMaterial, string filterLot)
        {
            return _service.DiagnoseNegativeStock(configuration, profile, filterWarehouse, filterMaterial, filterLot);
        }

        public IReadOnlyCollection<LotMaterialInconsistencyEntry> DiagnoseLotMaterialInconsistencies(AppConfiguration configuration, DatabaseProfile profile, string filterWarehouse, string filterMaterial, string filterLot)
        {
            return _service.DiagnoseLotMaterialInconsistencies(configuration, profile, filterWarehouse, filterMaterial, filterLot);
        }

        public IReadOnlyCollection<DuplicateLotEntry> DiagnoseDuplicateLotsByMaterial(AppConfiguration configuration, DatabaseProfile profile, string filterMaterial, string filterLotName, string filterLotCode)
        {
            return _service.DiagnoseDuplicateLotsByMaterial(configuration, profile, filterMaterial, filterLotName, filterLotCode);
        }

        public IReadOnlyCollection<DuplicateNoteMovementGroup> DiagnoseDuplicateNoteMovements(AppConfiguration configuration, DatabaseProfile profile)
        {
            return _service.DiagnoseDuplicateNoteMovements(configuration, profile);
        }

        public IReadOnlyCollection<DuplicateNoteMovementDetail> LoadDuplicateNoteMovementDetails(AppConfiguration configuration, DatabaseProfile profile, string noteNumber, string supplier)
        {
            return _service.LoadDuplicateNoteMovementDetails(configuration, profile, noteNumber, supplier);
        }

        public InactivateDuplicatesResult InactivateDuplicateNoteMovements(AppConfiguration configuration, DatabaseProfile profile, string actorUserName, long[] movementIds, string noteNumber)
        {
            return _service.InactivateDuplicateNoteMovements(configuration, profile, actorUserName, movementIds, noteNumber);
        }
    }
}
