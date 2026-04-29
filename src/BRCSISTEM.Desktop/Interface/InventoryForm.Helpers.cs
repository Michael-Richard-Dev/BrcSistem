using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;
using BRCSISTEM.Desktop.Interface.ContagemInventario;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class InventoryForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                _configuration.Normalize();
                _permissions = _inventoryController.ResolvePermissions(_identity);
                ClearForm(confirm: false, releaseLock: true, regenerateNumber: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ReloadAllReferences(string warehouseCode, string materialCode, string lotCode)
        {
            _isRefreshingReferences = true;
            try
            {
                warehouseCode = warehouseCode ?? GetSelectedCode(_warehouseComboBox);
                materialCode = materialCode ?? GetSelectedCode(_materialComboBox);
                lotCode = lotCode ?? GetSelectedCode(_lotComboBox);

                _warehouseOptions = ToLookupOptions(_inventoryController.LoadWarehousesForUser(_configuration, _databaseProfile, _identity.UserName));
                BindCombo(_warehouseComboBox, _warehouseOptions, warehouseCode);

                warehouseCode = GetSelectedCode(_warehouseComboBox);
                if (string.IsNullOrWhiteSpace(warehouseCode))
                {
                    _materialOptions = Array.Empty<LookupOption>();
                    _lotOptions = Array.Empty<LookupOption>();
                    BindCombo(_materialComboBox, _materialOptions, null);
                    BindCombo(_lotComboBox, _lotOptions, null);
                    return;
                }

                _materialOptions = ToLookupOptions(_inventoryController.LoadMaterialsByWarehouse(
                    _configuration,
                    _databaseProfile,
                    warehouseCode,
                    _createdTextBox?.Text,
                    _onlyBrcCheckBox?.Checked ?? false));
                BindCombo(_materialComboBox, _materialOptions, materialCode);

                materialCode = GetSelectedCode(_materialComboBox);
                if (string.IsNullOrWhiteSpace(materialCode))
                {
                    _lotOptions = Array.Empty<LookupOption>();
                    BindCombo(_lotComboBox, _lotOptions, null);
                    return;
                }

                _lotOptions = ToLookupOptions(_inventoryController.LoadLotsByWarehouseAndMaterial(
                    _configuration,
                    _databaseProfile,
                    warehouseCode,
                    materialCode,
                    _createdTextBox?.Text));
                BindCombo(_lotComboBox, _lotOptions, lotCode);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            UpdateStockIndicator();
        }

        private void ReloadWarehouses()
        {
            ReloadAllReferences(GetSelectedCode(_warehouseComboBox), GetSelectedCode(_materialComboBox), GetSelectedCode(_lotComboBox));
        }

        private void ReloadMaterials()
        {
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            var selectedMaterial = GetSelectedCode(_materialComboBox);

            if (string.IsNullOrWhiteSpace(warehouseCode))
            {
                _materialOptions = Array.Empty<LookupOption>();
                BindCombo(_materialComboBox, _materialOptions, null);
                _lotOptions = Array.Empty<LookupOption>();
                BindCombo(_lotComboBox, _lotOptions, null);
                UpdateStockIndicator();
                return;
            }

            _materialOptions = ToLookupOptions(_inventoryController.LoadMaterialsByWarehouse(_configuration, _databaseProfile, warehouseCode, _createdTextBox.Text, _onlyBrcCheckBox.Checked));
            BindCombo(_materialComboBox, _materialOptions, selectedMaterial);
            ReloadLots();
        }

        private void ReloadLots()
        {
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            var materialCode = GetSelectedCode(_materialComboBox);
            var selectedLot = GetSelectedCode(_lotComboBox);

            if (string.IsNullOrWhiteSpace(warehouseCode) || string.IsNullOrWhiteSpace(materialCode))
            {
                _lotOptions = Array.Empty<LookupOption>();
                BindCombo(_lotComboBox, _lotOptions, null);
                UpdateStockIndicator();
                return;
            }

            _lotOptions = ToLookupOptions(_inventoryController.LoadLotsByWarehouseAndMaterial(_configuration, _databaseProfile, warehouseCode, materialCode, _createdTextBox.Text));
            BindCombo(_lotComboBox, _lotOptions, selectedLot);
            UpdateStockIndicator();
        }

        private void OnWarehouseSelectionChanged()
        {
            if (_isRefreshingReferences)
            {
                return;
            }

            ReloadMaterials();
        }

        private void OnMaterialSelectionChanged()
        {
            if (_isRefreshingReferences)
            {
                return;
            }

            ReloadLots();
        }

        private void UpdateStockIndicator()
        {
            try
            {
                var warehouseCode = GetSelectedCode(_warehouseComboBox);
                var materialCode = GetSelectedCode(_materialComboBox);
                var lotCode = GetSelectedCode(_lotComboBox);
                if (string.IsNullOrWhiteSpace(warehouseCode)
                    || string.IsNullOrWhiteSpace(materialCode)
                    || string.IsNullOrWhiteSpace(lotCode))
                {
                    _stockLabel.Text = "Saldo atual do item selecionado: 0,00";
                    return;
                }

                var balance = _inventoryController.GetStockBalanceAt(_configuration, _databaseProfile, warehouseCode, materialCode, lotCode, _createdTextBox.Text);
                _stockLabel.Text = "Saldo atual do item selecionado: " + balance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
            }
            catch
            {
                _stockLabel.Text = "Saldo atual do item selecionado: 0,00";
            }
        }

        private void ClearForm(bool confirm, bool releaseLock, bool regenerateNumber)
        {
            if (releaseLock)
            {
                ReleaseCurrentLockSafe();
            }

            _isPersisted = false;
            _isReadOnly = false;
            _currentStatus = "PENDENTE";
            _draftItems.Clear();
            _draftPoints.Clear();
            _currentCounts = Array.Empty<InventoryCountSummary>();
            _temporaryPointSequence = -1;

            _numberTextBox.Text = regenerateNumber
                ? _inventoryController.GenerateNextInventoryNumber(_configuration, _databaseProfile)
                : string.Empty;
            _createdTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            _scheduledTextBox.Text = string.Empty;
            _maxPointsNumeric.Value = 1;
            _observationTextBox.Text = string.Empty;
            _openedLabel.Text = string.Empty;
            _closedLabel.Text = string.Empty;
            _finalizedLabel.Text = string.Empty;
            _statusValueLabel.Text = "PENDENTE";

            RefreshDraftViews();
            ReloadAllReferences(GetSelectedCode(_warehouseComboBox), null, null);
            ApplyModeState();
        }

        private void OpenInventoryLookup()
        {
            using (var dialog = new InventoryLookupForm(_inventoryController, _configuration, _databaseProfile))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedInventory == null)
                {
                    return;
                }

                var selected = dialog.SelectedInventory;
                var requiresLock = !string.Equals(selected.Status, "ENCERRADO", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(selected.Status, "CANCELADO", StringComparison.OrdinalIgnoreCase);
                var shouldAcquireLock = requiresLock && !string.Equals(_lockedNumber, selected.Number, StringComparison.OrdinalIgnoreCase);

                try
                {
                    if (shouldAcquireLock)
                    {
                        ReleaseCurrentLockSafe();
                        var lockResult = _inventoryController.TryLockInventory(_configuration, _databaseProfile, selected.Number, _identity.UserName);
                        if (!lockResult.Success)
                        {
                            throw new InvalidOperationException(lockResult.Message);
                        }

                        _lockedNumber = selected.Number;
                    }
                    else if (!requiresLock)
                    {
                        ReleaseCurrentLockSafe();
                    }

                    var detail = _inventoryController.LoadInventory(_configuration, _databaseProfile, selected.Number);
                    ApplyInventoryDetail(detail, requiresLock);
                }
                catch (Exception exception)
                {
                    if (shouldAcquireLock)
                    {
                        ReleaseCurrentLockSafe();
                    }

                    ShowError(exception);
                }
            }
        }

        private void ApplyInventoryDetail(InventoryDetail detail, bool lockHeld)
        {
            _isPersisted = true;
            _currentStatus = NormalizeStatus(detail.Status);
            _isReadOnly = !lockHeld || string.Equals(_currentStatus, "ENCERRADO", StringComparison.OrdinalIgnoreCase) || string.Equals(_currentStatus, "CANCELADO", StringComparison.OrdinalIgnoreCase);

            _numberTextBox.Text = detail.Number ?? string.Empty;
            _createdTextBox.Text = detail.CreatedAtDisplay;
            _scheduledTextBox.Text = detail.ScheduledAtDisplay;
            _maxPointsNumeric.Value = detail.MaxOpenPoints <= 0 ? 1 : Math.Min(20, detail.MaxOpenPoints);
            _observationTextBox.Text = detail.Observation ?? string.Empty;
            _statusValueLabel.Text = detail.Status ?? string.Empty;
            _openedLabel.Text = detail.OpenedAtDisplay;
            _closedLabel.Text = detail.ClosedAtDisplay;
            _finalizedLabel.Text = detail.FinalizedAtDisplay;
            _currentCounts = detail.Counts ?? Array.Empty<InventoryCountSummary>();

            _draftItems.Clear();
            _draftItems.AddRange(detail.Items ?? Array.Empty<InventoryItemDetail>());
            _draftPoints.Clear();
            _draftPoints.AddRange(detail.Points ?? Array.Empty<InventoryPointSummary>());

            var firstItem = _draftItems.FirstOrDefault();
            ReloadAllReferences(firstItem?.WarehouseCode, firstItem?.MaterialCode, firstItem?.LotCode);
            RefreshDraftViews();
            ApplyModeState();
        }

        private void RefreshCurrentInventory()
        {
            if (!_isPersisted || string.IsNullOrWhiteSpace(_numberTextBox.Text))
            {
                RefreshDraftViews();
                ApplyModeState();
                return;
            }

            var detail = _inventoryController.LoadInventory(_configuration, _databaseProfile, _numberTextBox.Text);
            var lockHeld = !string.IsNullOrWhiteSpace(_lockedNumber)
                && string.Equals(_lockedNumber, detail.Number, StringComparison.OrdinalIgnoreCase);
            ApplyInventoryDetail(detail, lockHeld);
        }

        private void RefreshDraftViews()
        {
            _itemsGrid.DataSource = null;
            _itemsGrid.DataSource = _draftItems.ToArray();
            _pointsGrid.DataSource = null;
            _pointsGrid.DataSource = _draftPoints.ToArray();
            _countsGrid.DataSource = null;
            _countsGrid.DataSource = _currentCounts;
            _summaryLabel.Text = "Itens: " + _draftItems.Count + " | Pontos: " + _draftPoints.Count + " | Leituras: " + _currentCounts.Length;
        }

        private void AddItem()
        {
            try
            {
                EnsurePlanningEditable();
                var warehouseCode = GetSelectedCode(_warehouseComboBox);
                var materialCode = GetSelectedCode(_materialComboBox);
                var lotCode = GetSelectedCode(_lotComboBox);
                if (string.IsNullOrWhiteSpace(warehouseCode) || string.IsNullOrWhiteSpace(materialCode) || string.IsNullOrWhiteSpace(lotCode))
                {
                    throw new InvalidOperationException("Informe almoxarifado, material e lote.");
                }

                if (_draftItems.Any(item => string.Equals(item.WarehouseCode, warehouseCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(item.MaterialCode, materialCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(item.LotCode, lotCode, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Item ja adicionado.");
                }

                var conflict = _inventoryController.FindItemConflict(_configuration, _databaseProfile, _numberTextBox.Text, warehouseCode, materialCode, lotCode);
                if (conflict != null && !string.IsNullOrWhiteSpace(conflict.InventoryNumber))
                {
                    throw new InvalidOperationException(
                        "Item ja esta em inventario aberto.\nInventario: " + conflict.InventoryNumber + "\nStatus: " + conflict.InventoryStatus);
                }

                var balance = _inventoryController.GetStockBalanceAt(_configuration, _databaseProfile, warehouseCode, materialCode, lotCode, _createdTextBox.Text);
                _draftItems.Add(new InventoryItemDetail
                {
                    WarehouseCode = warehouseCode,
                    MaterialCode = materialCode,
                    MaterialDescription = GetSelectedDescription(_materialComboBox),
                    LotCode = lotCode,
                    LotName = GetSelectedDescription(_lotComboBox),
                    SystemBalance = balance,
                    Status = "ATIVO",
                });
                RefreshDraftViews();
                ApplyModeState();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void AddAllFromWarehouse()
        {
            try
            {
                EnsurePlanningEditable();
                var warehouseCode = GetSelectedCode(_warehouseComboBox);
                if (string.IsNullOrWhiteSpace(warehouseCode))
                {
                    throw new InvalidOperationException("Selecione um almoxarifado.");
                }

                var candidates = _inventoryController.LoadPlanningCandidatesByWarehouse(_configuration, _databaseProfile, warehouseCode, _createdTextBox.Text, _onlyBrcCheckBox.Checked);
                if (candidates.Length == 0)
                {
                    throw new InvalidOperationException(_onlyBrcCheckBox.Checked
                        ? "Nenhum item BRC encontrado para este almoxarifado."
                        : "Nenhum item encontrado para este almoxarifado.");
                }

                var added = 0;
                var blocked = 0;
                foreach (var candidate in candidates)
                {
                    if (_draftItems.Any(item => string.Equals(item.WarehouseCode, candidate.WarehouseCode, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(item.MaterialCode, candidate.MaterialCode, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(item.LotCode, candidate.LotCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    var conflict = _inventoryController.FindItemConflict(_configuration, _databaseProfile, _numberTextBox.Text, candidate.WarehouseCode, candidate.MaterialCode, candidate.LotCode);
                    if (conflict != null && !string.IsNullOrWhiteSpace(conflict.InventoryNumber))
                    {
                        blocked++;
                        continue;
                    }

                    _draftItems.Add(new InventoryItemDetail
                    {
                        WarehouseCode = candidate.WarehouseCode,
                        MaterialCode = candidate.MaterialCode,
                        MaterialDescription = candidate.MaterialDescription,
                        LotCode = candidate.LotCode,
                        LotName = candidate.LotName,
                        SystemBalance = candidate.SystemBalance,
                        Status = "ATIVO",
                    });
                    added++;
                }

                RefreshDraftViews();
                ApplyModeState();
                MessageBox.Show(this,
                    "Itens incluidos: " + added + (blocked > 0 ? "\nItens bloqueados por conflito em outro inventario aberto: " + blocked : string.Empty),
                    "Inventario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void RemoveSelectedItem()
        {
            try
            {
                EnsurePlanningEditable();
                if (!(_itemsGrid.CurrentRow?.DataBoundItem is InventoryItemDetail item))
                {
                    throw new InvalidOperationException("Selecione um item.");
                }

                _draftItems.RemoveAll(current =>
                    string.Equals(current.WarehouseCode, item.WarehouseCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(current.MaterialCode, item.MaterialCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(current.LotCode, item.LotCode, StringComparison.OrdinalIgnoreCase));
                RefreshDraftViews();
                ApplyModeState();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ClearItems()
        {
            try
            {
                EnsurePlanningEditable();
                if (MessageBox.Show(this, "Deseja limpar todos os itens planejados?", "Confirmacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                _draftItems.Clear();
                RefreshDraftViews();
                ApplyModeState();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void SaveInventory()
        {
            try
            {
                var request = BuildSaveRequest();
                _inventoryController.CreateInventory(_configuration, _databaseProfile, request);
                var lockResult = _inventoryController.TryLockInventory(_configuration, _databaseProfile, request.Number, _identity.UserName);
                if (lockResult.Success)
                {
                    _lockedNumber = request.Number;
                }

                RefreshCurrentInventoryByNumber(request.Number, lockHeld: lockResult.Success);
                SetStatus("Inventario salvo com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateInventory()
        {
            try
            {
                EnsurePlanningEditable();
                var request = BuildSaveRequest();
                _inventoryController.UpdateInventoryPlanning(_configuration, _databaseProfile, request);
                RefreshCurrentInventory();
                SetStatus("Planejamento atualizado com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void StartInventory()
        {
            try
            {
                EnsurePermission(_permissions.CanCount, "Usuario sem permissao para iniciar ou contar inventario.");
                EnsurePersisted();
                var allowEarlyStart = false;
                if (!string.IsNullOrWhiteSpace(_scheduledTextBox.Text)
                    && DateTime.TryParseExact(_scheduledTextBox.Text, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var scheduledAt)
                    && DateTime.Now < scheduledAt)
                {
                    allowEarlyStart = MessageBox.Show(
                        this,
                        "Inventario programado para abrir em " + _scheduledTextBox.Text + ".\nDeseja iniciar antes do horario programado?",
                        "Abertura antecipada",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes;
                    if (!allowEarlyStart)
                    {
                        return;
                    }
                }

                _inventoryController.StartInventory(_configuration, _databaseProfile, _numberTextBox.Text, _identity.UserName, allowEarlyStart);
                RefreshCurrentInventory();
                SetStatus("Inventario iniciado.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void CloseInventory()
        {
            try
            {
                EnsurePermission(_permissions.CanClose, "Usuario sem permissao para fechar inventario.");
                EnsurePersisted();
                _inventoryController.CloseInventory(_configuration, _databaseProfile, _numberTextBox.Text, _identity.UserName);
                RefreshCurrentInventory();
                SetStatus("Inventario fechado.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ReopenInventory()
        {
            try
            {
                EnsurePermission(_permissions.CanClose, "Usuario sem permissao para reabrir inventario.");
                EnsurePersisted();
                _inventoryController.ReopenInventory(_configuration, _databaseProfile, _numberTextBox.Text, _identity.UserName);
                RefreshCurrentInventory();
                SetStatus("Inventario reaberto para contagem.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void FinalizeInventory()
        {
            try
            {
                EnsurePermission(_permissions.CanClose, "Usuario sem permissao para encerrar inventario.");
                EnsurePersisted();
                var adjustments = _inventoryController.FinalizeInventory(_configuration, _databaseProfile, _numberTextBox.Text, _identity.UserName);
                _lockedNumber = null;
                RefreshCurrentInventoryByNumber(_numberTextBox.Text, lockHeld: false);
                SetStatus("Inventario encerrado. Ajustes gerados: " + adjustments + ".", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void CancelInventory()
        {
            try
            {
                EnsurePermission(_permissions.CanCancel, "Usuario sem permissao para cancelar inventario.");
                EnsurePersisted();
                var reason = PromptText("Motivo do cancelamento (opcional):", "Cancelar inventario");
                _inventoryController.CancelInventory(_configuration, _databaseProfile, _numberTextBox.Text, _identity.UserName, reason);
                _lockedNumber = null;
                RefreshCurrentInventoryByNumber(_numberTextBox.Text, lockHeld: false);
                SetStatus("Inventario cancelado.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void CreatePoint()
        {
            try
            {
                EnsurePermission(_permissions.CanCount, "Usuario sem permissao de contagem.");
                var point = PromptPoint();
                if (point == null)
                {
                    return;
                }

                if (!_isPersisted)
                {
                    EnsureDraftPointAvailability(point);
                    point.Id = _temporaryPointSequence--;
                    point.Status = "ABERTO";
                    _draftPoints.Add(point);
                    RefreshDraftViews();
                    ApplyModeState();
                    return;
                }

                _inventoryController.AddPoint(
                    _configuration,
                    _databaseProfile,
                    _numberTextBox.Text,
                    new InventoryPointInput
                    {
                        PointName = point.PointName,
                        IpAddress = point.IpAddress,
                        ComputerName = point.ComputerName,
                        Status = "ABERTO",
                    },
                    _identity.UserName);
                RefreshCurrentInventory();
                SetStatus("Ponto adicionado.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void OpenSelectedPoint()
        {
            try
            {
                EnsurePermission(_permissions.CanCount, "Usuario sem permissao de contagem.");
                EnsurePersisted();
                var point = GetSelectedPoint();
                if (point == null || point.Id <= 0 || !string.Equals(point.Status, "ABERTO", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Selecione um ponto ABERTO.");
                }

                if (!string.Equals(_currentStatus, "INICIADO", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(_currentStatus, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Leitura permitida apenas em inventario INICIADO ou EM_CONTAGEM.");
                }

                if (_countWindows.TryGetValue(point.Id, out var existing) && !existing.IsDisposed)
                {
                    existing.Focus();
                    return;
                }

                var window = new ContagemInventarioForm(_inventoryController, _configuration, _databaseProfile, _identity, _numberTextBox.Text, point.Id, () => RefreshCurrentInventory());
                _countWindows[point.Id] = window;
                window.FormClosed += (sender, args) => _countWindows.Remove(point.Id);
                window.Show(this);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void CloseSelectedPoint()
        {
            try
            {
                EnsurePermission(_permissions.CanCount, "Usuario sem permissao de contagem.");
                EnsurePersisted();
                var point = GetSelectedPoint();
                if (point == null || point.Id <= 0)
                {
                    throw new InvalidOperationException("Selecione um ponto salvo.");
                }

                _inventoryController.ClosePoint(_configuration, _databaseProfile, _numberTextBox.Text, point.Id, _identity.UserName);
                RefreshCurrentInventory();
                SetStatus("Ponto fechado.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ReopenSelectedPoint()
        {
            try
            {
                EnsurePermission(_permissions.CanCount, "Usuario sem permissao de contagem.");
                EnsurePersisted();
                var point = GetSelectedPoint();
                if (point == null || point.Id <= 0)
                {
                    throw new InvalidOperationException("Selecione um ponto salvo.");
                }

                _inventoryController.ReopenPoint(_configuration, _databaseProfile, _numberTextBox.Text, point.Id, _identity.UserName);
                RefreshCurrentInventory();
                SetStatus("Ponto reaberto.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void DeleteSelectedPoint()
        {
            try
            {
                EnsurePermission(_permissions.CanCount, "Usuario sem permissao de contagem.");
                var point = GetSelectedPoint();
                if (point == null)
                {
                    throw new InvalidOperationException("Selecione um ponto.");
                }

                if (point.Id <= 0)
                {
                    _draftPoints.RemoveAll(item => item.Id == point.Id);
                    RefreshDraftViews();
                    ApplyModeState();
                    return;
                }

                EnsurePersisted();
                _inventoryController.DeletePoint(_configuration, _databaseProfile, _numberTextBox.Text, point.Id, _identity.UserName);
                RefreshCurrentInventory();
                SetStatus("Ponto excluido.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ApplyZeroCounts()
        {
            try
            {
                EnsurePermission(_permissions.CanCount, "Usuario sem permissao de contagem.");
                EnsurePersisted();
                var point = GetSelectedPoint();
                if (point == null || point.Id <= 0)
                {
                    throw new InvalidOperationException("Selecione um ponto salvo.");
                }

                var inserted = _inventoryController.ApplyZeroCounts(_configuration, _databaseProfile, _numberTextBox.Text, point.Id, _identity.UserName, string.Empty, Environment.MachineName);
                RefreshCurrentInventory();
                SetStatus("Lancamento zero realizado para " + inserted + " item(ns).", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private SaveInventoryRequest BuildSaveRequest()
        {
            return new SaveInventoryRequest
            {
                Number = _numberTextBox.Text,
                CreatedDateTime = _createdTextBox.Text,
                ScheduledDateTime = _scheduledTextBox.Text,
                Observation = _observationTextBox.Text,
                MaxOpenPoints = Convert.ToInt32(_maxPointsNumeric.Value),
                ActorUserName = _identity.UserName,
                Items = _draftItems.Select(item => new InventoryItemInput
                {
                    WarehouseCode = item.WarehouseCode,
                    MaterialCode = item.MaterialCode,
                    LotCode = item.LotCode,
                }).ToArray(),
                Points = _draftPoints.Select(point => new InventoryPointInput
                {
                    Id = point.Id,
                    PointName = point.PointName,
                    IpAddress = point.IpAddress,
                    ComputerName = point.ComputerName,
                    Status = string.IsNullOrWhiteSpace(point.Status) ? "ABERTO" : point.Status,
                }).ToArray(),
            };
        }

        private void EnsureDraftPointAvailability(InventoryPointSummary point)
        {
            var openPoints = _draftPoints.Where(item => string.Equals(item.Status, "ABERTO", StringComparison.OrdinalIgnoreCase)).ToArray();
            if (openPoints.Length >= Convert.ToInt32(_maxPointsNumeric.Value))
            {
                throw new InvalidOperationException("Limite de pontos abertos atingido.");
            }

            if (!string.IsNullOrWhiteSpace(point.IpAddress)
                && openPoints.Any(item => string.Equals(item.IpAddress, point.IpAddress, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Nao e permitido abrir dois pontos no mesmo IP neste inventario.");
            }
        }

        private InventoryPointSummary GetSelectedPoint()
        {
            return _pointsGrid.CurrentRow?.DataBoundItem as InventoryPointSummary;
        }

        private void RefreshCurrentInventoryByNumber(string number, bool lockHeld)
        {
            var detail = _inventoryController.LoadInventory(_configuration, _databaseProfile, number);
            ApplyInventoryDetail(detail, lockHeld);
        }

        private void ApplyModeState()
        {
            var planningEditable = !_isReadOnly && (!_isPersisted || string.Equals(_currentStatus, "PENDENTE", StringComparison.OrdinalIgnoreCase));
            _scheduledTextBox.ReadOnly = !planningEditable;
            _maxPointsNumeric.Enabled = planningEditable;
            _observationTextBox.ReadOnly = !planningEditable;
            _warehouseComboBox.Enabled = planningEditable;
            _materialComboBox.Enabled = planningEditable;
            _lotComboBox.Enabled = planningEditable;
            _onlyBrcCheckBox.Enabled = planningEditable;

            _saveButton.Enabled = !_isPersisted && planningEditable && _permissions.CanOpen;
            _updateButton.Enabled = _isPersisted && planningEditable && _permissions.CanOpen;
            _startButton.Enabled = _isPersisted && string.Equals(_currentStatus, "PENDENTE", StringComparison.OrdinalIgnoreCase) && !_isReadOnly && _permissions.CanCount;
            _closeButton.Enabled = _isPersisted && string.Equals(_currentStatus, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase) && !_isReadOnly && _permissions.CanClose;
            _reopenButton.Enabled = _isPersisted && string.Equals(_currentStatus, "FECHADO", StringComparison.OrdinalIgnoreCase) && !_isReadOnly && _permissions.CanClose;
            _finalizeButton.Enabled = _isPersisted && string.Equals(_currentStatus, "FECHADO", StringComparison.OrdinalIgnoreCase) && !_isReadOnly && _permissions.CanClose;
            _cancelButton.Enabled = _isPersisted && !_isReadOnly && _permissions.CanCancel && IsCancelableStatus(_currentStatus);

            _newPointButton.Enabled = _permissions.CanCount && !_isReadOnly && (!_isPersisted || string.Equals(_currentStatus, "PENDENTE", StringComparison.OrdinalIgnoreCase) || string.Equals(_currentStatus, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase));
            _openPointButton.Enabled = _permissions.CanCount && _isPersisted;
            _closePointButton.Enabled = _permissions.CanCount && _isPersisted && !_isReadOnly;
            _reopenPointButton.Enabled = _permissions.CanCount && _isPersisted && !_isReadOnly;
            _deletePointButton.Enabled = _permissions.CanCount && (!_isPersisted || !_isReadOnly);
            _zeroButton.Enabled = _permissions.CanCount && _isPersisted && !_isReadOnly && (string.Equals(_currentStatus, "INICIADO", StringComparison.OrdinalIgnoreCase) || string.Equals(_currentStatus, "EM_CONTAGEM", StringComparison.OrdinalIgnoreCase));
        }

        private void EnsurePlanningEditable()
        {
            EnsurePermission(_permissions.CanOpen, "Usuario sem permissao para alterar o planejamento do inventario.");
            if (_isReadOnly)
            {
                throw new InvalidOperationException("Inventario em modo somente leitura.");
            }

            if (_isPersisted && !string.Equals(_currentStatus, "PENDENTE", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("O planejamento so pode ser alterado enquanto o inventario estiver PENDENTE.");
            }
        }

        private void EnsurePersisted()
        {
            if (!_isPersisted || string.IsNullOrWhiteSpace(_numberTextBox.Text))
            {
                throw new InvalidOperationException("Salve o inventario antes de continuar.");
            }
        }

        private static void EnsurePermission(bool allowed, string errorMessage)
        {
            if (!allowed)
            {
                throw new InvalidOperationException(errorMessage);
            }
        }

        private void ReleaseCurrentLockSafe()
        {
            if (string.IsNullOrWhiteSpace(_lockedNumber))
            {
                return;
            }

            try
            {
                _inventoryController.ReleaseInventoryLock(_configuration, _databaseProfile, _lockedNumber, _identity.UserName);
            }
            catch
            {
            }

            _lockedNumber = null;
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            ReleaseCurrentLockSafe();
            foreach (var window in _countWindows.Values.ToArray())
            {
                if (!window.IsDisposed)
                {
                    window.Close();
                }
            }
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                if (_isPersisted)
                {
                    RefreshCurrentInventory();
                }
                else
                {
                    ReloadWarehouses();
                }
            }
        }

        private void ShowError(Exception exception)
        {
            SetStatus(exception.Message, true);
            MessageBox.Show(this, exception.Message, "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SetStatus(string message, bool isError)
        {
            _statusValueLabel.Text = string.IsNullOrWhiteSpace(message) ? _currentStatus : message;
            _statusValueLabel.ForeColor = isError ? System.Drawing.Color.Firebrick : System.Drawing.Color.SeaGreen;
        }

        private static bool IsCancelableStatus(string status)
        {
            var normalized = NormalizeStatus(status);
            return normalized == "PENDENTE" || normalized == "INICIADO" || normalized == "EM_CONTAGEM" || normalized == "FECHADO";
        }

        private static string NormalizeStatus(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
        }

        private static string NormalizeDateTimeInput(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (DateTime.TryParseExact(value.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                return parsed.ToString("dd/MM/yyyy HH:mm");
            }

            return value.Trim();
        }

        private static LookupOption[] ToLookupOptions(WarehouseSummary[] items)
        {
            return (items ?? Array.Empty<WarehouseSummary>())
                .Select(item => new LookupOption { Code = item.Code, Description = item.Name })
                .ToArray();
        }

        private static LookupOption[] ToLookupOptions(PackagingSummary[] items)
        {
            return (items ?? Array.Empty<PackagingSummary>())
                .Select(item => new LookupOption { Code = item.Code, Description = item.Description })
                .ToArray();
        }

        private static LookupOption[] ToLookupOptions(LotSummary[] items)
        {
            return (items ?? Array.Empty<LotSummary>())
                .Select(item => new LookupOption { Code = item.Code, Description = item.Name })
                .ToArray();
        }

        private static void BindCombo(ComboBox comboBox, LookupOption[] options, string selectedCode)
        {
            comboBox.DataSource = null;
            comboBox.DisplayMember = nameof(LookupOption.Display);
            comboBox.ValueMember = nameof(LookupOption.Code);
            comboBox.DataSource = options ?? Array.Empty<LookupOption>();

            if (comboBox.Items.Count > 0)
            {
                SelectOptionByCode(comboBox, selectedCode);
                if (comboBox.SelectedIndex < 0)
                {
                    comboBox.SelectedIndex = 0;
                }
            }
        }

        private static void SelectOptionByCode(ComboBox comboBox, string selectedCode)
        {
            if (string.IsNullOrWhiteSpace(selectedCode))
            {
                return;
            }

            for (var index = 0; index < comboBox.Items.Count; index++)
            {
                if (comboBox.Items[index] is LookupOption option
                    && string.Equals(option.Code, selectedCode, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
            }
        }

        private static string GetSelectedCode(ComboBox comboBox)
        {
            return comboBox?.SelectedItem is LookupOption option ? option.Code : string.Empty;
        }

        private static string GetSelectedDescription(ComboBox comboBox)
        {
            return comboBox?.SelectedItem is LookupOption option ? option.Description : string.Empty;
        }

        private InventoryPointSummary PromptPoint()
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Novo ponto";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new System.Drawing.Size(420, 190);
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ShowInTaskbar = false;

                var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 4, ColumnCount = 2 };
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                for (var row = 0; row < 4; row++)
                {
                    root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }

                var nameTextBox = new TextBox { Dock = DockStyle.Top, Font = new System.Drawing.Font("Segoe UI", 10F) };
                var ipTextBox = new TextBox { Dock = DockStyle.Top, Font = new System.Drawing.Font("Segoe UI", 10F) };
                var computerTextBox = new TextBox { Dock = DockStyle.Top, Font = new System.Drawing.Font("Segoe UI", 10F), Text = Environment.MachineName };

                root.Controls.Add(CreateFieldLabel("Nome do ponto:"), 0, 0);
                root.Controls.Add(nameTextBox, 1, 0);
                root.Controls.Add(CreateFieldLabel("IP do ponto:"), 0, 1);
                root.Controls.Add(ipTextBox, 1, 1);
                root.Controls.Add(CreateFieldLabel("Computador:"), 0, 2);
                root.Controls.Add(computerTextBox, 1, 2);

                var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
                var okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, AutoSize = true, FlatStyle = FlatStyle.System };
                var cancelButton = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, AutoSize = true, FlatStyle = FlatStyle.System };
                buttons.Controls.Add(okButton);
                buttons.Controls.Add(cancelButton);
                root.Controls.Add(buttons, 1, 3);

                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;
                dialog.Controls.Add(root);

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return null;
                }

                return new InventoryPointSummary
                {
                    PointName = nameTextBox.Text?.Trim(),
                    IpAddress = ipTextBox.Text?.Trim(),
                    ComputerName = computerTextBox.Text?.Trim(),
                    Status = "ABERTO",
                };
            }
        }

        private string PromptText(string label, string title)
        {
            using (var dialog = new Form())
            {
                dialog.Text = title;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.ClientSize = new System.Drawing.Size(420, 140);
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ShowInTaskbar = false;

                var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 3 };
                var labelControl = new Label { AutoSize = true, Text = label, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) };
                var valueTextBox = new TextBox { Dock = DockStyle.Top, Font = new System.Drawing.Font("Segoe UI", 10F) };
                var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, AutoSize = true };
                var okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, AutoSize = true, FlatStyle = FlatStyle.System };
                var cancelButton = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, AutoSize = true, FlatStyle = FlatStyle.System };
                buttons.Controls.Add(okButton);
                buttons.Controls.Add(cancelButton);

                root.Controls.Add(labelControl, 0, 0);
                root.Controls.Add(valueTextBox, 0, 1);
                root.Controls.Add(buttons, 0, 2);
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;
                dialog.Controls.Add(root);

                return dialog.ShowDialog(this) == DialogResult.OK ? valueTextBox.Text?.Trim() ?? string.Empty : string.Empty;
            }
        }

        private sealed class LookupOption
        {
            public string Code { get; set; }

            public string Description { get; set; }

            public string Display
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(Code))
                    {
                        return Description ?? string.Empty;
                    }

                    return string.IsNullOrWhiteSpace(Description)
                        ? Code
                        : Code + " - " + Description;
                }
            }
        }
    }
}
