using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class ProductionOutputForm
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

        private void ReloadAllReferences(string warehouseCode = null, string productCode = null, string materialCode = null, string lotCode = null)
        {
            _isRefreshingReferences = true;
            try
            {
                warehouseCode = warehouseCode ?? GetSelectedCode(_warehouseComboBox);
                productCode = productCode ?? GetSelectedCode(_productComboBox);
                materialCode = materialCode ?? GetSelectedCode(_materialComboBox);
                lotCode = lotCode ?? GetSelectedCode(_lotComboBox);

                _productOptions = ToLookupOptions(_productionOutputController.LoadProducts(_configuration, _databaseProfile));
                _warehouseOptions = ToLookupOptions(_productionOutputController.LoadWarehousesForUser(_configuration, _databaseProfile, _identity.UserName));
                _materialOptions = ToLookupOptions(_productionOutputController.LoadMaterialsByWarehouse(_configuration, _databaseProfile, warehouseCode, _movementDateTimeTextBox?.Text));
                _lotOptions = ToLookupOptions(_productionOutputController.LoadLotsByWarehouseAndMaterial(_configuration, _databaseProfile, warehouseCode, materialCode, _movementDateTimeTextBox?.Text));

                BindCombo(_productComboBox, _productOptions, productCode);
                BindCombo(_warehouseComboBox, _warehouseOptions, warehouseCode);
                BindCombo(_materialComboBox, _materialOptions, materialCode);
                BindCombo(_lotComboBox, _lotOptions, lotCode);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            UpdateConsumedQuantity();
            UpdateStockIndicator();
        }

        private void ReloadProducts()
        {
            var selectedProduct = GetSelectedCode(_productComboBox);
            _productOptions = ToLookupOptions(_productionOutputController.LoadProducts(_configuration, _databaseProfile));
            BindCombo(_productComboBox, _productOptions, selectedProduct);
        }

        private void ReloadWarehouses()
        {
            ReloadAllReferences(GetSelectedCode(_warehouseComboBox), GetSelectedCode(_productComboBox), GetSelectedCode(_materialComboBox), GetSelectedCode(_lotComboBox));
        }

        private void ReloadMaterials()
        {
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            var selectedMaterial = GetSelectedCode(_materialComboBox);
            _materialOptions = ToLookupOptions(_productionOutputController.LoadMaterialsByWarehouse(_configuration, _databaseProfile, warehouseCode, _movementDateTimeTextBox.Text));
            BindCombo(_materialComboBox, _materialOptions, selectedMaterial);
            ReloadLots();
        }

        private void ReloadLots()
        {
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            var materialCode = GetSelectedCode(_materialComboBox);
            var selectedLot = GetSelectedCode(_lotComboBox);
            _lotOptions = ToLookupOptions(_productionOutputController.LoadLotsByWarehouseAndMaterial(_configuration, _databaseProfile, warehouseCode, materialCode, _movementDateTimeTextBox.Text));
            BindCombo(_lotComboBox, _lotOptions, selectedLot);
            UpdateStockIndicator();
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
                _materialOptions = ToLookupOptions(_productionOutputController.LoadMaterialsByWarehouse(
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
                return;
            }

            ReloadMaterials();
        }

        private void OnQuantityInputsChanged()
        {
            UpdateConsumedQuantity();
            UpdateStockIndicator();
        }

        private void OpenOutputLookup()
        {
            using (var dialog = new ProductionOutputLookupForm(_productionOutputController, _configuration, _databaseProfile))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedOutput == null)
                {
                    return;
                }

                var selected = dialog.SelectedOutput;
                var requiresLock = !string.Equals(selected.Status, "CANCELADO", StringComparison.OrdinalIgnoreCase);
                var shouldAcquireLock = requiresLock && !string.Equals(_lockedNumber, selected.Number, StringComparison.OrdinalIgnoreCase);

                try
                {
                    if (shouldAcquireLock)
                    {
                        ReleaseCurrentLockSafe();
                        var lockResult = _productionOutputController.TryLockOutput(_configuration, _databaseProfile, selected.Number, _identity.UserName);
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

                    var detail = _productionOutputController.LoadOutput(_configuration, _databaseProfile, selected.Number);
                    ApplyOutputDetail(detail, requiresLock);
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

        private void ApplyOutputDetail(ProductionOutputDetail detail, bool lockHeld)
        {
            _loadedOutputStatus = detail.Status;
            _numberTextBox.Text = detail.Number ?? string.Empty;
            _purposeTextBox.Text = detail.Purpose ?? string.Empty;
            _movementDateTimeTextBox.Text = detail.MovementDateTime ?? string.Empty;
            SelectShift(detail.Shift);
            ReloadAllReferences(detail.WarehouseCode, null, null, null);
            EnsureOptionPresent(ref _warehouseOptions, _warehouseComboBox, detail.WarehouseCode, detail.WarehouseName);

            _items.Clear();
            _items.AddRange(detail.Items ?? Array.Empty<ProductionOutputItemDetail>());
            _mode = ScreenMode.Consultation;
            if (!lockHeld)
            {
                _lockedNumber = null;
            }

            RefreshItemGrid();
            ClearItemEditor();
            ApplyModeState();
            SetStatus(
                string.Equals(detail.Status, "CANCELADO", StringComparison.OrdinalIgnoreCase)
                    ? "Saida cancelada carregada em modo leitura."
                    : "Saida carregada para consulta.",
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
                throw new InvalidOperationException("Informe a Data/Hora da saida corretamente antes de adicionar itens.");
            }
        }

        private ProductionOutputItemDetail BuildEditorItem()
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

            var quantitySent = ParseQuantity(_quantitySentTextBox.Text);
            var quantityReturned = ParseQuantity(_quantityReturnedTextBox.Text);
            if (quantitySent <= 0M)
            {
                throw new InvalidOperationException("Quantidade de envio deve ser maior que zero.");
            }

            if (quantityReturned < 0M)
            {
                throw new InvalidOperationException("Quantidade de retorno nao pode ser negativa.");
            }

            if (quantityReturned > quantitySent)
            {
                throw new InvalidOperationException("Quantidade de retorno nao pode ser maior que a quantidade de envio.");
            }

            var quantityConsumed = quantitySent - quantityReturned;
            if (quantityConsumed <= 0M)
            {
                throw new InvalidOperationException("A quantidade consumida deve ser maior que zero.");
            }

            return new ProductionOutputItemDetail
            {
                ProductCode = GetSelectedCode(_productComboBox),
                ProductDescription = GetSelectedDescription(_productComboBox),
                MaterialCode = materialCode,
                MaterialDescription = GetSelectedDescription(_materialComboBox),
                LotCode = lotCode,
                LotName = GetSelectedDescription(_lotComboBox),
                QuantitySent = quantitySent,
                QuantityReturned = quantityReturned,
                QuantityConsumed = quantityConsumed,
                Status = "ATIVO",
            };
        }

        private void ValidateDuplicateItem(ProductionOutputItemDetail item, int indexToIgnore)
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
                    throw new InvalidOperationException("Este material e lote ja foram adicionados nesta saida.");
                }
            }
        }

        private void RefreshItemGrid()
        {
            var rows = _items
                .Select((item, index) => new { SourceIndex = index, Item = item })
                .OrderBy(item => item.Item.ProductDisplay ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Item.MaterialDisplay ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Item.LotDisplay ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select((item, visualIndex) => new ProductionOutputItemRow
                {
                    SourceIndex = item.SourceIndex,
                    ItemNumber = visualIndex + 1,
                    ProductDisplay = item.Item.ProductDisplay,
                    MaterialDisplay = item.Item.MaterialDisplay,
                    LotDisplay = item.Item.LotDisplay,
                    QuantitySentText = item.Item.QuantitySentText,
                    QuantityReturnedText = item.Item.QuantityReturnedText,
                    QuantityConsumedText = item.Item.QuantityConsumedText,
                    Status = item.Item.Status,
                })
                .ToArray();
            _itemsGrid.DataSource = rows;
            _itemCountLabel.Text = "Itens na saida: " + _items.Count;
        }

        private void StartEditingSelectedItem()
        {
            if (!(_itemsGrid.CurrentRow?.DataBoundItem is ProductionOutputItemRow row))
            {
                SetStatus("Selecione um item para editar.", true);
                return;
            }

            _editingItemIndex = row.SourceIndex;
            var item = _items[_editingItemIndex];

            EnsureOptionPresent(ref _productOptions, _productComboBox, item.ProductCode, item.ProductDescription);
            EnsureOptionPresent(ref _materialOptions, _materialComboBox, item.MaterialCode, item.MaterialDescription);

            _isRefreshingReferences = true;
            try
            {
                _lotOptions = ToLookupOptions(_productionOutputController.LoadLotsByWarehouseAndMaterial(
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
            _quantitySentTextBox.Text = item.QuantitySent.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
            _quantityReturnedTextBox.Text = item.QuantityReturned.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
            UpdateConsumedQuantity();
            UpdateStockIndicator();
            ApplyModeState();
            SetStatus("Item carregado para edicao.", false);
        }

        private void RemoveSelectedItem()
        {
            if (!(_itemsGrid.CurrentRow?.DataBoundItem is ProductionOutputItemRow row))
            {
                SetStatus("Selecione um item para remover.", true);
                return;
            }

            _items.RemoveAt(row.SourceIndex);
            _editingItemIndex = -1;
            RefreshItemGrid();
            ClearItemEditor();
            ApplyModeState();
            SetStatus("Item removido.", false);
        }

        private SaveProductionOutputRequest BuildSaveRequest()
        {
            return new SaveProductionOutputRequest
            {
                Number = _numberTextBox.Text,
                WarehouseCode = GetSelectedCode(_warehouseComboBox),
                Purpose = _purposeTextBox.Text,
                Shift = _shiftComboBox.Text,
                MovementDateTime = _movementDateTimeTextBox.Text,
                ActorUserName = _identity.UserName,
                Items = _items.Select(item => new ProductionOutputItemInput
                {
                    ProductCode = item.ProductCode,
                    MaterialCode = item.MaterialCode,
                    LotCode = item.LotCode,
                    QuantitySent = item.QuantitySent,
                    QuantityReturned = item.QuantityReturned,
                    QuantityConsumed = item.QuantityConsumed,
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
            _loadedOutputStatus = "ATIVO";
            _editingItemIndex = -1;
            _items.Clear();
            RefreshItemGrid();

            _isRefreshingReferences = true;
            try
            {
                BindCombo(_productComboBox, _productOptions, null);
                BindCombo(_warehouseComboBox, _warehouseOptions, null);
                _materialOptions = new LookupOption[0];
                _lotOptions = new LookupOption[0];
                BindCombo(_materialComboBox, _materialOptions, null);
                BindCombo(_lotComboBox, _lotOptions, null);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            _numberTextBox.Text = regenerateNumber ? _productionOutputController.GenerateNextOutputNumber(_configuration, _databaseProfile) : string.Empty;
            _purposeTextBox.Text = "SAIDA DE PRODUCAO";
            _movementDateTimeTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            SelectShift("1o TURNO");
            ClearItemEditor();
            ApplyModeState();
            SetStatus("Preencha os dados da saida e adicione os itens.", false);
        }

        private void ClearItemEditor()
        {
            _editingItemIndex = -1;
            _productComboBox.SelectedIndex = -1;
            _materialComboBox.SelectedIndex = -1;
            _lotComboBox.SelectedIndex = -1;
            _quantitySentTextBox.Clear();
            _quantityReturnedTextBox.Text = "0,00";
            UpdateConsumedQuantity();
            UpdateStockIndicator();
            ApplyModeState();
        }

        private void ApplyModeState()
        {
            var readOnlyCancelled = _mode == ScreenMode.Consultation && string.Equals(_loadedOutputStatus, "CANCELADO", StringComparison.OrdinalIgnoreCase);
            var consultationMode = _mode == ScreenMode.Consultation;
            var hasWarehouse = !string.IsNullOrWhiteSpace(GetSelectedCode(_warehouseComboBox));
            var hasMaterial = !string.IsNullOrWhiteSpace(GetSelectedCode(_materialComboBox));

            _numberTextBox.ReadOnly = true;
            _purposeTextBox.ReadOnly = consultationMode || readOnlyCancelled;
            _movementDateTimeTextBox.ReadOnly = consultationMode || readOnlyCancelled;
            _shiftComboBox.Enabled = !consultationMode && !readOnlyCancelled;
            _warehouseComboBox.Enabled = !consultationMode && !readOnlyCancelled && _items.Count == 0;

            _productComboBox.Enabled = !consultationMode && !readOnlyCancelled;
            _materialComboBox.Enabled = !consultationMode && !readOnlyCancelled && hasWarehouse;
            _lotComboBox.Enabled = !consultationMode && !readOnlyCancelled && hasWarehouse && hasMaterial;
            _quantitySentTextBox.ReadOnly = readOnlyCancelled;
            _quantityReturnedTextBox.ReadOnly = readOnlyCancelled;
            _quantityConsumedTextBox.ReadOnly = true;

            _itemApplyButton.Enabled = !readOnlyCancelled && (_mode == ScreenMode.Creation || _editingItemIndex >= 0);
            _itemApplyButton.Text = _editingItemIndex >= 0 ? "Aplicar Item" : "Adicionar";
            _saveButton.Enabled = _mode == ScreenMode.Creation && _items.Count > 0;
            _updateButton.Enabled = consultationMode && !readOnlyCancelled && _items.Count > 0;
            _cancelButton.Enabled = consultationMode && !readOnlyCancelled;
        }

        private void OpenProductLookup()
        {
            using (var dialog = new ProdutoSelecaoForm(_productOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_productComboBox, _productOptions, dialog.SelectedOption.Code);
                }
            }
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

        private void OpenProductManagement()
        {
            using (var dialog = new ProductManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadProducts();
        }

        private void OpenPackagingManagement()
        {
            using (var dialog = new PackagingManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadMaterials();
        }

        private void OpenWarehouseManagement()
        {
            using (var dialog = new WarehouseManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadWarehouses();
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
                ValidateDuplicateItem(item, _editingItemIndex);
                var availableBalance = _productionOutputController.GetAvailableStockBalance(
                    _configuration,
                    _databaseProfile,
                    item.MaterialCode,
                    item.LotCode,
                    GetSelectedCode(_warehouseComboBox),
                    _movementDateTimeTextBox.Text,
                    _mode == ScreenMode.Consultation ? _numberTextBox.Text : null);
                if (item.QuantityConsumed > availableBalance)
                {
                    throw new InvalidOperationException(
                        "Saldo insuficiente para o lote selecionado.\n\n"
                        + "Saldo disponivel: " + availableBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + "\n"
                        + "Quantidade consumida: " + item.QuantityConsumed.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")));
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
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void SaveOutput()
        {
            try
            {
                _productionOutputController.CreateOutput(_configuration, _databaseProfile, BuildSaveRequest());
                SetStatus("Saida salva com sucesso.", false);
                ClearForm(confirm: false, releaseLock: true, regenerateNumber: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateOutput()
        {
            try
            {
                if (_mode != ScreenMode.Consultation)
                {
                    throw new InvalidOperationException("Consulte uma saida existente antes de alterar.");
                }

                _productionOutputController.UpdateOutput(_configuration, _databaseProfile, BuildSaveRequest());
                SetStatus("Saida alterada com sucesso.", false);
                ClearForm(confirm: false, releaseLock: true, regenerateNumber: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void CancelOutput()
        {
            if (_mode != ScreenMode.Consultation)
            {
                ShowError(new InvalidOperationException("Consulte uma saida existente antes de cancelar."));
                return;
            }

            if (MessageBox.Show(this, "Deseja realmente cancelar a saida de producao selecionada?", "Cancelar Saida", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _productionOutputController.CancelOutput(_configuration, _databaseProfile, _numberTextBox.Text, _identity.UserName);
                SetStatus("Saida cancelada com sucesso.", false);
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
                if (string.IsNullOrWhiteSpace(_lockedNumber))
                {
                    return;
                }

                _productionOutputController.ReleaseOutputLock(_configuration, _databaseProfile, _lockedNumber, _identity.UserName);
            }
            catch
            {
            }
            finally
            {
                _lockedNumber = null;
            }
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
                _stockLabel.Text = "Saldo disponivel: -";
                return;
            }

            try
            {
                var balance = _productionOutputController.GetAvailableStockBalance(
                    _configuration,
                    _databaseProfile,
                    materialCode,
                    lotCode,
                    warehouseCode,
                    _movementDateTimeTextBox.Text,
                    _mode == ScreenMode.Consultation ? _numberTextBox.Text : null);
                _stockLabel.Text = "Saldo disponivel do lote: " + balance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
            }
            catch
            {
                _stockLabel.Text = "Saldo disponivel: -";
            }
        }

        private void UpdateConsumedQuantity()
        {
            decimal quantitySent;
            decimal quantityReturned;
            if (!TryParseQuantity(_quantitySentTextBox.Text, out quantitySent))
            {
                quantitySent = 0M;
            }

            if (!TryParseQuantity(_quantityReturnedTextBox.Text, out quantityReturned))
            {
                quantityReturned = 0M;
            }

            var quantityConsumed = quantitySent - quantityReturned;
            _quantityConsumedTextBox.Text = quantityConsumed.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
        }

        private void SelectShift(string shift)
        {
            var value = string.IsNullOrWhiteSpace(shift) ? "1o TURNO" : shift.Trim();
            var items = _shiftComboBox.Items.Cast<object>().Select(item => Convert.ToString(item)).ToArray();
            var index = Array.FindIndex(items, item => string.Equals(item, value, StringComparison.OrdinalIgnoreCase));
            _shiftComboBox.SelectedIndex = index >= 0 ? index : 0;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) SaveOutput();
            else if (e.KeyCode == Keys.F3) UpdateOutput();
            else if (e.KeyCode == Keys.F4) CloseForm();
            else if (e.KeyCode == Keys.F5) ClearForm(confirm: true, releaseLock: true, regenerateNumber: true);
            else if (e.KeyCode == Keys.F6) CancelOutput();
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

        private static LookupOption[] ToLookupOptions(ProductSummary[] items)
        {
            return (items ?? new ProductSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Description, Status = item.Status }).ToArray();
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

        private sealed class ProductionOutputItemRow
        {
            public int SourceIndex { get; set; }

            public int ItemNumber { get; set; }

            public string ProductDisplay { get; set; }

            public string MaterialDisplay { get; set; }

            public string LotDisplay { get; set; }

            public string QuantitySentText { get; set; }

            public string QuantityReturnedText { get; set; }

            public string QuantityConsumedText { get; set; }

            public string Status { get; set; }
        }
    }
}
