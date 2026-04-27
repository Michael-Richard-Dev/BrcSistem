using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class MaterialRequisitionForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                _configuration.Normalize();
                ReloadAllReferences();
                ClearForm(confirm: false, releaseLock: true, regenerateNumber: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ReloadAllReferences(string warehouseCode = null, string materialCode = null, string lotCode = null, string reasonName = null)
        {
            _isRefreshingReferences = true;
            try
            {
                warehouseCode = warehouseCode ?? GetSelectedCode(_warehouseComboBox);
                materialCode = materialCode ?? GetSelectedCode(_materialComboBox);
                lotCode = lotCode ?? GetSelectedCode(_lotComboBox);
                reasonName = string.IsNullOrWhiteSpace(reasonName) ? GetSelectedReason() : reasonName;

                _warehouseOptions = ToLookupOptions(_materialRequisitionController.LoadWarehousesForUser(_configuration, _databaseProfile, _identity.UserName));
                _materialOptions = ToLookupOptions(_materialRequisitionController.LoadMaterialsByWarehouse(_configuration, _databaseProfile, warehouseCode, _movementDateTimeTextBox?.Text));
                _lotOptions = ToLookupOptions(_materialRequisitionController.LoadLotsByWarehouseAndMaterial(_configuration, _databaseProfile, warehouseCode, materialCode, _movementDateTimeTextBox?.Text));
                _reasonOptions = _materialRequisitionController.LoadReasons(_configuration, _databaseProfile);

                BindCombo(_warehouseComboBox, _warehouseOptions, warehouseCode);
                BindCombo(_materialComboBox, _materialOptions, materialCode);
                BindCombo(_lotComboBox, _lotOptions, lotCode);
                BindReasonCombo(reasonName);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            UpdateStockIndicator();
            RefreshQuickStockPanel();
        }

        private void ReloadWarehouses()
        {
            ReloadAllReferences(
                GetSelectedCode(_warehouseComboBox),
                GetSelectedCode(_materialComboBox),
                GetSelectedCode(_lotComboBox),
                GetSelectedReason());
        }

        private void ReloadMaterials()
        {
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            var selectedMaterial = GetSelectedCode(_materialComboBox);
            _materialOptions = ToLookupOptions(_materialRequisitionController.LoadMaterialsByWarehouse(_configuration, _databaseProfile, warehouseCode, _movementDateTimeTextBox.Text));
            BindCombo(_materialComboBox, _materialOptions, selectedMaterial);
            ReloadLots();
            RefreshQuickStockPanel();
        }

        private void ReloadLots()
        {
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            var materialCode = GetSelectedCode(_materialComboBox);
            var selectedLot = GetSelectedCode(_lotComboBox);
            _lotOptions = ToLookupOptions(_materialRequisitionController.LoadLotsByWarehouseAndMaterial(_configuration, _databaseProfile, warehouseCode, materialCode, _movementDateTimeTextBox.Text));
            BindCombo(_lotComboBox, _lotOptions, selectedLot);
            UpdateStockIndicator();
            RefreshQuickStockPanel();
        }

        private void ReloadReasons()
        {
            _reasonOptions = _materialRequisitionController.LoadReasons(_configuration, _databaseProfile);
            BindReasonCombo(GetSelectedReason());
        }

        private void OnWarehouseSelectionChanged()
        {
            if (_isRefreshingReferences)
            {
                return;
            }

            _isRefreshingReferences = true;
            try
            {
                _materialOptions = ToLookupOptions(_materialRequisitionController.LoadMaterialsByWarehouse(
                    _configuration,
                    _databaseProfile,
                    GetSelectedCode(_warehouseComboBox),
                    _movementDateTimeTextBox.Text));
                BindCombo(_materialComboBox, _materialOptions, null);
                _lotOptions = new LookupOption[0];
                BindCombo(_lotComboBox, _lotOptions, null);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            UpdateStockIndicator();
            RefreshQuickStockPanel();
            ApplyModeState();
        }

        private void OnMaterialSelectionChanged()
        {
            if (_isRefreshingReferences)
            {
                return;
            }

            ReloadLots();
            ApplyModeState();
        }

        private void OnMovementDateChanged()
        {
            if (_isRefreshingReferences || string.IsNullOrWhiteSpace(GetSelectedCode(_warehouseComboBox)))
            {
                UpdateStockIndicator();
                RefreshQuickStockPanel();
                return;
            }

            ReloadMaterials();
        }

        private void OpenRequisitionLookup()
        {
            using (var dialog = new MaterialRequisitionLookupForm(_materialRequisitionController, _configuration, _databaseProfile))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedRequisition == null)
                {
                    return;
                }

                var selected = dialog.SelectedRequisition;
                var requiresLock = !string.Equals(selected.Status, "CANCELADO", StringComparison.OrdinalIgnoreCase);
                var shouldAcquireLock = requiresLock && !string.Equals(_lockedNumber, selected.Number, StringComparison.OrdinalIgnoreCase);

                try
                {
                    if (shouldAcquireLock)
                    {
                        ReleaseCurrentLockSafe();
                        var lockResult = _materialRequisitionController.TryLockRequisition(_configuration, _databaseProfile, selected.Number, _identity.UserName);
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

                    var detail = _materialRequisitionController.LoadRequisition(_configuration, _databaseProfile, selected.Number);
                    ApplyRequisitionDetail(detail, requiresLock);
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

        private void ApplyRequisitionDetail(MaterialRequisitionDetail detail, bool lockHeld)
        {
            _loadedRequisitionStatus = detail.Status;
            _numberTextBox.Text = detail.Number ?? string.Empty;
            _movementDateTimeTextBox.Text = detail.MovementDateTime ?? string.Empty;
            ReloadAllReferences(detail.WarehouseCode, null, null, "Requisicao");
            EnsureOptionPresent(ref _warehouseOptions, _warehouseComboBox, detail.WarehouseCode, detail.WarehouseName);

            _items.Clear();
            _items.AddRange(detail.Items ?? Array.Empty<MaterialRequisitionItemDetail>());
            _mode = ScreenMode.Consultation;
            if (!lockHeld)
            {
                _lockedNumber = null;
            }

            RefreshItemGrid();
            ClearItemEditor();
            ApplyModeState();
            RefreshQuickStockPanel();
            SetStatus(
                string.Equals(detail.Status, "CANCELADO", StringComparison.OrdinalIgnoreCase)
                    ? "Requisicao cancelada carregada em modo leitura."
                    : "Requisicao carregada para consulta.",
                false);
        }

        private void ValidateHeaderForItems()
        {
            if (string.IsNullOrWhiteSpace(GetSelectedCode(_warehouseComboBox)))
            {
                throw new InvalidOperationException("Selecione o almoxarifado antes de adicionar itens.");
            }

            DateTime movementDateTime;
            if (!DateTime.TryParseExact(_movementDateTimeTextBox.Text, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out movementDateTime))
            {
                throw new InvalidOperationException("Informe a Data/Hora da requisicao corretamente antes de adicionar itens.");
            }
        }

        private MaterialRequisitionItemDetail BuildEditorItem()
        {
            var materialCode = GetSelectedCode(_materialComboBox);
            var lotCode = GetSelectedCode(_lotComboBox);
            if (string.IsNullOrWhiteSpace(materialCode))
            {
                throw new InvalidOperationException("Selecione o material.");
            }

            if (string.IsNullOrWhiteSpace(lotCode))
            {
                throw new InvalidOperationException("Selecione o lote.");
            }

            var quantity = ParseQuantity(_quantityTextBox.Text);
            if (quantity <= 0M)
            {
                throw new InvalidOperationException("Quantidade deve ser maior que zero.");
            }

            return new MaterialRequisitionItemDetail
            {
                MaterialCode = materialCode,
                MaterialDescription = GetSelectedDescription(_materialComboBox),
                LotCode = lotCode,
                LotName = GetSelectedDescription(_lotComboBox),
                Quantity = quantity,
                Status = "ATIVO",
            };
        }

        private bool HasDuplicateItemCombination(MaterialRequisitionItemDetail item, int indexToIgnore)
        {
            for (var index = 0; index < _items.Count; index++)
            {
                if (index == indexToIgnore)
                {
                    continue;
                }

                var existing = _items[index];
                if (string.Equals(existing.MaterialCode, item.MaterialCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(existing.LotCode, item.LotCode, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private decimal GetReservedQuantity(string materialCode, string lotCode, int indexToIgnore)
        {
            return _items
                .Select((item, index) => new { Item = item, Index = index })
                .Where(entry => entry.Index != indexToIgnore)
                .Where(entry => string.Equals(entry.Item.MaterialCode, materialCode, StringComparison.OrdinalIgnoreCase))
                .Where(entry => string.Equals(entry.Item.LotCode, lotCode, StringComparison.OrdinalIgnoreCase))
                .Sum(entry => entry.Item.Quantity);
        }

        private void RefreshItemGrid()
        {
            var rows = _items
                .Select((item, index) => new MaterialRequisitionItemRow
                {
                    SourceIndex = index,
                    ItemNumber = index + 1,
                    MaterialDisplay = item.MaterialDisplay,
                    LotDisplay = item.LotDisplay,
                    QuantitySentText = item.QuantityText,
                    QuantityLoweredText = item.QuantityText,
                    Status = item.Status,
                })
                .ToArray();
            _itemsGrid.DataSource = rows;
            _itemCountLabel.Text = "Itens na requisicao: " + _items.Count;
        }

        private void StartEditingSelectedItem()
        {
            if (!(_itemsGrid.CurrentRow?.DataBoundItem is MaterialRequisitionItemRow row))
            {
                SetStatus("Selecione um item para editar.", true);
                return;
            }

            _editingItemIndex = row.SourceIndex;
            var item = _items[_editingItemIndex];

            EnsureOptionPresent(ref _materialOptions, _materialComboBox, item.MaterialCode, item.MaterialDescription);

            _isRefreshingReferences = true;
            try
            {
                _lotOptions = ToLookupOptions(_materialRequisitionController.LoadLotsByWarehouseAndMaterial(
                    _configuration,
                    _databaseProfile,
                    GetSelectedCode(_warehouseComboBox),
                    item.MaterialCode,
                    _movementDateTimeTextBox.Text));
                BindCombo(_lotComboBox, _lotOptions, null);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            EnsureOptionPresent(ref _lotOptions, _lotComboBox, item.LotCode, item.LotName);
            _quantityTextBox.Text = item.Quantity.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
            UpdateStockIndicator();
            ApplyModeState();
            SetStatus("Item carregado para edicao.", false);
        }

        private void RemoveSelectedItem()
        {
            if (!(_itemsGrid.CurrentRow?.DataBoundItem is MaterialRequisitionItemRow row))
            {
                SetStatus("Selecione um item para remover.", true);
                return;
            }

            _items.RemoveAt(row.SourceIndex);
            _editingItemIndex = -1;
            RefreshItemGrid();
            ClearItemEditor();
            ApplyModeState();
            RefreshQuickStockPanel();
            SetStatus("Item removido.", false);
        }

        private SaveMaterialRequisitionRequest BuildSaveRequest()
        {
            return new SaveMaterialRequisitionRequest
            {
                Number = _numberTextBox.Text,
                WarehouseCode = GetSelectedCode(_warehouseComboBox),
                MovementDateTime = _movementDateTimeTextBox.Text,
                ReasonName = GetSelectedReason(),
                ActorUserName = _identity.UserName,
                Items = _items.Select(item => new MaterialRequisitionItemInput
                {
                    MaterialCode = item.MaterialCode,
                    LotCode = item.LotCode,
                    Quantity = item.Quantity,
                }).ToArray(),
            };
        }

        private void ClearForm(bool confirm, bool releaseLock, bool regenerateNumber)
        {
            if (confirm && MessageBox.Show(this, "Deseja limpar os dados da tela?", "Limpar Formulario", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            if (releaseLock)
            {
                ReleaseCurrentLockSafe();
            }

            _mode = ScreenMode.Creation;
            _loadedRequisitionStatus = "ATIVO";
            _editingItemIndex = -1;
            _items.Clear();
            RefreshItemGrid();

            _isRefreshingReferences = true;
            try
            {
                BindCombo(_warehouseComboBox, _warehouseOptions, null);
                _materialOptions = new LookupOption[0];
                _lotOptions = new LookupOption[0];
                BindCombo(_materialComboBox, _materialOptions, null);
                BindCombo(_lotComboBox, _lotOptions, null);
                BindReasonCombo("Requisicao");
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            _numberTextBox.Text = regenerateNumber ? _materialRequisitionController.GenerateNextRequisitionNumber(_configuration, _databaseProfile) : string.Empty;
            _movementDateTimeTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ClearItemEditor();
            RefreshQuickStockPanel();
            ApplyModeState();
            SetStatus("Preencha os dados da requisicao e adicione os itens.", false);
        }

        private void ClearItemEditor()
        {
            _editingItemIndex = -1;
            _materialComboBox.SelectedIndex = -1;
            _lotComboBox.SelectedIndex = -1;
            _quantityTextBox.Clear();
            UpdateStockIndicator();
            ApplyModeState();
        }

        private void ApplyModeState()
        {
            var readOnlyCancelled = _mode == ScreenMode.Consultation && string.Equals(_loadedRequisitionStatus, "CANCELADO", StringComparison.OrdinalIgnoreCase);
            var canEdit = !readOnlyCancelled;
            var hasWarehouse = !string.IsNullOrWhiteSpace(GetSelectedCode(_warehouseComboBox));
            var hasMaterial = !string.IsNullOrWhiteSpace(GetSelectedCode(_materialComboBox));

            _numberTextBox.ReadOnly = true;
            _warehouseComboBox.Enabled = canEdit;
            _movementDateTimeTextBox.ReadOnly = !canEdit;

            _materialComboBox.Enabled = canEdit && hasWarehouse;
            _lotComboBox.Enabled = canEdit && hasWarehouse && hasMaterial;
            _quantityTextBox.ReadOnly = !canEdit;
            _reasonComboBox.Enabled = canEdit;

            _itemApplyButton.Enabled = canEdit && hasWarehouse && hasMaterial;
            _itemApplyButton.Text = _editingItemIndex >= 0 ? "Aplicar Item" : "Adicionar";
            _saveButton.Enabled = _mode == ScreenMode.Creation && canEdit && _items.Count > 0;
            _updateButton.Enabled = _mode == ScreenMode.Consultation && canEdit && _items.Count > 0;
            _cancelButton.Enabled = _mode == ScreenMode.Consultation && canEdit;
        }

        private void OpenWarehouseLookup()
        {
            using (var dialog = new AlmoxarifadoSelecaoForm(_warehouseOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_warehouseComboBox, _warehouseOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenMaterialLookup()
        {
            if (string.IsNullOrWhiteSpace(GetSelectedCode(_warehouseComboBox)))
            {
                SetStatus("Selecione o almoxarifado antes de buscar materiais.", true);
                return;
            }

            ReloadMaterials();
            using (var dialog = new MaterialSelecaoForm(_materialOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_materialComboBox, _materialOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenLotLookup()
        {
            if (string.IsNullOrWhiteSpace(GetSelectedCode(_warehouseComboBox)))
            {
                SetStatus("Selecione o almoxarifado antes de buscar lotes.", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(GetSelectedCode(_materialComboBox)))
            {
                SetStatus("Selecione o material antes de buscar lotes.", true);
                return;
            }

            ReloadLots();
            using (var dialog = new LoteSelecaoForm(_lotOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_lotComboBox, _lotOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenWarehouseManagement()
        {
            using (var dialog = new WarehouseManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadWarehouses();
        }

        private void OpenPackagingManagement()
        {
            using (var dialog = new PackagingManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadMaterials();
        }

        private void OpenLotManagement()
        {
            using (var dialog = new LotManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadLots();
        }

        private void AddOrUpdateItem()
        {
            try
            {
                ValidateHeaderForItems();
                var item = BuildEditorItem();

                if (_editingItemIndex >= 0)
                {
                    if (HasDuplicateItemCombination(item, _editingItemIndex))
                    {
                        throw new InvalidOperationException(
                            "Este material/lote ja esta lancado para esta requisicao.\n\n"
                            + "Altere o item existente em vez de duplicar.");
                    }
                }
                else if (HasDuplicateItemCombination(item, -1))
                {
                    var confirmDuplicate = MessageBox.Show(
                        this,
                        "Este material e lote ja foram adicionados a requisicao.\n\n"
                        + "Deseja adicionar novamente?\n\n"
                        + "O saldo disponivel sera validado considerando a soma de todas as quantidades.",
                        "Item Duplicado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (confirmDuplicate != DialogResult.Yes)
                    {
                        return;
                    }
                }

                var databaseBalance = _materialRequisitionController.GetAvailableStockBalance(
                    _configuration,
                    _databaseProfile,
                    item.MaterialCode,
                    item.LotCode,
                    GetSelectedCode(_warehouseComboBox),
                    _movementDateTimeTextBox.Text,
                    _mode == ScreenMode.Consultation ? _numberTextBox.Text : null);

                var reservedQuantity = GetReservedQuantity(item.MaterialCode, item.LotCode, _editingItemIndex);
                var availableBalance = databaseBalance - reservedQuantity;
                if (item.Quantity > availableBalance)
                {
                    throw new InvalidOperationException(
                        "Saldo insuficiente para o lote selecionado.\n\n"
                        + "Saldo base: " + databaseBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Pre-lancado na requisicao: " + reservedQuantity.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Disponivel para este item: " + availableBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Quantidade informada: " + item.Quantity.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")));
                }

                if (_editingItemIndex >= 0)
                {
                    _items[_editingItemIndex] = item;
                    SetStatus("Item atualizado.", false);
                }
                else
                {
                    _items.Add(item);
                    SetStatus("Item adicionado.", false);
                }

                RefreshItemGrid();
                ClearItemEditor();
                RefreshQuickStockPanel();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void SaveRequisition()
        {
            try
            {
                _materialRequisitionController.CreateRequisition(_configuration, _databaseProfile, BuildSaveRequest());
                SetStatus("Requisicao salva com sucesso.", false);
                ClearForm(confirm: false, releaseLock: true, regenerateNumber: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateRequisition()
        {
            try
            {
                if (_mode != ScreenMode.Consultation)
                {
                    throw new InvalidOperationException("Consulte uma requisicao existente antes de alterar.");
                }

                _materialRequisitionController.UpdateRequisition(_configuration, _databaseProfile, BuildSaveRequest());
                SetStatus("Requisicao alterada com sucesso.", false);
                ClearForm(confirm: false, releaseLock: true, regenerateNumber: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void CancelRequisition()
        {
            if (_mode != ScreenMode.Consultation)
            {
                ShowError(new InvalidOperationException("Consulte uma requisicao existente antes de cancelar."));
                return;
            }

            if (MessageBox.Show(
                    this,
                    "Deseja realmente cancelar a requisicao selecionada?\n\n"
                    + "Esta operacao inativara os movimentos de estoque relacionados.",
                    "Cancelar Requisicao",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _materialRequisitionController.CancelRequisition(_configuration, _databaseProfile, _numberTextBox.Text, _identity.UserName);
                SetStatus("Requisicao cancelada com sucesso.", false);
                ClearForm(confirm: false, releaseLock: true, regenerateNumber: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ReleaseCurrentLockSafe()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_lockedNumber) || _configuration == null)
                {
                    return;
                }

                _materialRequisitionController.ReleaseRequisitionLock(_configuration, _databaseProfile, _lockedNumber, _identity.UserName);
            }
            catch
            {
            }
            finally
            {
                _lockedNumber = null;
            }
        }

        private void RefreshQuickStockPanel()
        {
            if (_quickStockGrid == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(GetSelectedCode(_warehouseComboBox)) || string.IsNullOrWhiteSpace(_movementDateTimeTextBox.Text))
            {
                _quickStockGrid.DataSource = Array.Empty<QuickStockRow>();
                return;
            }

            try
            {
                var items = _materialRequisitionController.LoadQuickStockBalances(
                    _configuration,
                    _databaseProfile,
                    GetSelectedCode(_warehouseComboBox),
                    GetSelectedCode(_materialComboBox),
                    _movementDateTimeTextBox.Text,
                    _mode == ScreenMode.Consultation ? _numberTextBox.Text : null);

                var rows = items
                    .Select(item =>
                    {
                        var launched = _items
                            .Where(current => string.Equals(current.MaterialCode, item.MaterialCode, StringComparison.OrdinalIgnoreCase))
                            .Where(current => string.Equals(current.LotCode, item.LotCode, StringComparison.OrdinalIgnoreCase))
                            .Sum(current => current.Quantity);

                        return new QuickStockRow
                        {
                            WarehouseCode = item.WarehouseCode,
                            MaterialCode = item.MaterialCode,
                            MaterialDescription = item.MaterialDescription,
                            LotCode = item.LotCode,
                            LotName = item.LotName,
                            ExpirationDate = item.ExpirationDate,
                            Balance = item.Balance,
                            Launched = launched,
                            FinalBalance = item.Balance - launched,
                        };
                    })
                    .ToArray();
                _quickStockGrid.DataSource = rows;
            }
            catch
            {
                _quickStockGrid.DataSource = Array.Empty<QuickStockRow>();
            }
        }

        private void UseSelectedQuickStockItem()
        {
            if (!(_quickStockGrid.CurrentRow?.DataBoundItem is QuickStockRow row))
            {
                return;
            }

            EnsureOptionPresent(ref _materialOptions, _materialComboBox, row.MaterialCode, row.MaterialDescription);
            SelectOptionByCode(_materialComboBox, _materialOptions, row.MaterialCode);
            ReloadLots();
            EnsureOptionPresent(ref _lotOptions, _lotComboBox, row.LotCode, row.LotName);
            SelectOptionByCode(_lotComboBox, _lotOptions, row.LotCode);
            UpdateStockIndicator();
            ApplyModeState();
            SetStatus("Material e lote preenchidos a partir do saldo rapido.", false);
        }

        private void UpdateStockIndicator()
        {
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            var materialCode = GetSelectedCode(_materialComboBox);
            var lotCode = GetSelectedCode(_lotComboBox);
            if (string.IsNullOrWhiteSpace(warehouseCode)
                || string.IsNullOrWhiteSpace(materialCode)
                || string.IsNullOrWhiteSpace(lotCode)
                || string.IsNullOrWhiteSpace(_movementDateTimeTextBox.Text))
            {
                _stockLabel.Text = "Saldo do almoxarifado: -";
                _stockLabel.ForeColor = Color.FromArgb(27, 54, 93);
                return;
            }

            try
            {
                var databaseBalance = _materialRequisitionController.GetAvailableStockBalance(
                    _configuration,
                    _databaseProfile,
                    materialCode,
                    lotCode,
                    warehouseCode,
                    _movementDateTimeTextBox.Text,
                    _mode == ScreenMode.Consultation ? _numberTextBox.Text : null);
                var reservedQuantity = GetReservedQuantity(materialCode, lotCode, _editingItemIndex);
                decimal requestedQuantity;
                if (!TryParseQuantity(_quantityTextBox.Text, out requestedQuantity))
                {
                    requestedQuantity = 0M;
                }

                var availableBalance = databaseBalance - reservedQuantity;
                var projectedBalance = availableBalance - requestedQuantity;
                _stockLabel.Text =
                    "Saldo base: " + databaseBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"))
                    + " | Pre-lancado: " + reservedQuantity.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"))
                    + " | Disponivel: " + availableBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"))
                    + " | Apos item: " + projectedBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
                _stockLabel.ForeColor = projectedBalance < 0M ? Color.Firebrick : Color.FromArgb(27, 54, 93);
            }
            catch
            {
                _stockLabel.Text = "Saldo do almoxarifado: -";
                _stockLabel.ForeColor = Color.FromArgb(27, 54, 93);
            }
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) SaveRequisition();
            else if (e.KeyCode == Keys.F3) UpdateRequisition();
            else if (e.KeyCode == Keys.F4) CloseForm();
            else if (e.KeyCode == Keys.F5) ClearForm(confirm: true, releaseLock: true, regenerateNumber: true);
            else if (e.KeyCode == Keys.F6) CancelRequisition();
            else if (e.KeyCode == Keys.Insert) AddOrUpdateItem();
            else if (e.KeyCode == Keys.Delete) RemoveSelectedItem();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            ReleaseCurrentLockSafe();
        }

        private void CloseForm()
        {
            ReleaseCurrentLockSafe();
            Close();
        }

        private void ShowError(Exception exception)
        {
            SetStatus(exception.Message, true);
            MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }

        private static string NormalizeDateTimeInput(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).Take(12).ToArray());
            if (digits.Length > 2) digits = digits.Insert(2, "/");
            if (digits.Length > 5) digits = digits.Insert(5, "/");
            if (digits.Length > 10) digits = digits.Insert(10, " ");
            if (digits.Length > 13) digits = digits.Insert(13, ":");
            return digits;
        }

        private static decimal ParseQuantity(string value)
        {
            var culture = CultureInfo.GetCultureInfo("pt-BR");
            decimal parsed;
            if (decimal.TryParse((value ?? string.Empty).Trim(), NumberStyles.Number, culture, out parsed))
            {
                return parsed;
            }

            if (decimal.TryParse((value ?? string.Empty).Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            throw new InvalidOperationException("Quantidade invalida.");
        }

        private static bool TryParseQuantity(string value, out decimal parsed)
        {
            var culture = CultureInfo.GetCultureInfo("pt-BR");
            return decimal.TryParse((value ?? string.Empty).Trim(), NumberStyles.Number, culture, out parsed)
                || decimal.TryParse((value ?? string.Empty).Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out parsed);
        }

        private static LookupOption[] ToLookupOptions(WarehouseSummary[] items)
        {
            return (items ?? new WarehouseSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
        }

        private static LookupOption[] ToLookupOptions(PackagingSummary[] items)
        {
            return (items ?? new PackagingSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Description, Status = item.Status }).ToArray();
        }

        private static LookupOption[] ToLookupOptions(LotSummary[] items)
        {
            return (items ?? new LotSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
        }

        private static void BindCombo(ComboBox comboBox, LookupOption[] options, string selectedCode)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            comboBox.Items.AddRange(options.Cast<object>().ToArray());
            comboBox.EndUpdate();
            SelectOptionByCode(comboBox, options, selectedCode);
        }

        private static void SelectOptionByCode(ComboBox comboBox, LookupOption[] options, string selectedCode)
        {
            if (string.IsNullOrWhiteSpace(selectedCode))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            for (var index = 0; index < options.Length; index++)
            {
                if (!string.Equals(options[index].Code, selectedCode, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                comboBox.SelectedIndex = index;
                return;
            }

            comboBox.SelectedIndex = -1;
        }

        private static string GetSelectedCode(ComboBox comboBox)
        {
            return comboBox.SelectedItem is LookupOption option ? option.Code : string.Empty;
        }

        private static string GetSelectedDescription(ComboBox comboBox)
        {
            return comboBox.SelectedItem is LookupOption option ? option.Description : string.Empty;
        }

        private static void EnsureOptionPresent(ref LookupOption[] options, ComboBox comboBox, string code, string description)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            var existing = options.FirstOrDefault(item => string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                options = options.Concat(new[]
                {
                    new LookupOption
                    {
                        Code = code,
                        Description = description,
                        Status = "ATIVO",
                    }
                }).OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase).ToArray();
                BindCombo(comboBox, options, code);
                return;
            }

            SelectOptionByCode(comboBox, options, code);
        }

        private void BindReasonCombo(string selectedReason)
        {
            var names = (_reasonOptions ?? Array.Empty<RequisitionReasonSummary>())
                .Where(item => item != null && item.IsActive)
                .Select(item => item.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            _reasonComboBox.BeginUpdate();
            _reasonComboBox.Items.Clear();
            _reasonComboBox.Items.AddRange(names.Cast<object>().ToArray());
            _reasonComboBox.EndUpdate();

            SelectReasonByName(string.IsNullOrWhiteSpace(selectedReason) ? "Requisicao" : selectedReason);
            if (_reasonComboBox.SelectedIndex < 0 && _reasonComboBox.Items.Count > 0)
            {
                _reasonComboBox.SelectedIndex = 0;
            }
        }

        private void SelectReasonByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _reasonComboBox.SelectedIndex = -1;
                return;
            }

            for (var index = 0; index < _reasonComboBox.Items.Count; index++)
            {
                if (!string.Equals(Convert.ToString(_reasonComboBox.Items[index]), name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                _reasonComboBox.SelectedIndex = index;
                return;
            }

            _reasonComboBox.SelectedIndex = -1;
        }

        private string GetSelectedReason()
        {
            return _reasonComboBox?.SelectedItem == null ? string.Empty : Convert.ToString(_reasonComboBox.SelectedItem);
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label { AutoSize = true, Text = text, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93), Margin = new Padding(0, 8, 0, 4) };
        }

        private sealed class MaterialRequisitionItemRow
        {
            public int SourceIndex { get; set; }

            public int ItemNumber { get; set; }

            public string MaterialDisplay { get; set; }

            public string LotDisplay { get; set; }

            public string QuantitySentText { get; set; }

            public string QuantityLoweredText { get; set; }

            public string Status { get; set; }
        }

        private sealed class QuickStockRow
        {
            public string WarehouseCode { get; set; }

            public string MaterialCode { get; set; }

            public string MaterialDescription { get; set; }

            public string LotCode { get; set; }

            public string LotName { get; set; }

            public string ExpirationDate { get; set; }

            public decimal Balance { get; set; }

            public decimal Launched { get; set; }

            public decimal FinalBalance { get; set; }

            public string MaterialDisplay
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(MaterialCode))
                    {
                        return MaterialDescription ?? string.Empty;
                    }

                    return string.IsNullOrWhiteSpace(MaterialDescription)
                        ? MaterialCode
                        : MaterialCode + " - " + MaterialDescription;
                }
            }

            public string LotDisplay
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(LotCode))
                    {
                        return LotName ?? string.Empty;
                    }

                    return string.IsNullOrWhiteSpace(LotName)
                        ? LotCode
                        : LotCode + " - " + LotName;
                }
            }

            public string BalanceText
            {
                get { return Balance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
            }

            public string LaunchedText
            {
                get { return Launched.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
            }

            public string FinalBalanceText
            {
                get { return FinalBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")); }
            }
        }
    }
}
