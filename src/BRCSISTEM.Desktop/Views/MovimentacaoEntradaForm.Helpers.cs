using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class MovimentacaoEntradaForm
    {
        private void ApplyActionIcons()
        {
            SetIcon(_btnNumberLookup, "search_black.png");
            SetIcon(_btnSupplierRefresh, "refresh_black.png");
            SetIcon(_btnSupplierLookup, "search_black.png");
            SetIcon(_btnSupplierNew, "new_black.png");
            SetIcon(_btnWarehouseRefresh, "refresh_black.png");
            SetIcon(_btnWarehouseLookup, "search_black.png");
            SetIcon(_btnMaterialRefresh, "refresh_black.png");
            SetIcon(_btnMaterialLookup, "search_black.png");
            SetIcon(_btnMaterialNew, "new_black.png");
            SetIcon(_btnLotRefresh, "refresh_black.png");
            SetIcon(_btnLotLookup, "search_black.png");
            SetIcon(_btnLotNew, "new_black.png");
            SetIcon(_btnItemAdd, "add_black.png");
            SetIcon(_btnItemEdit, "edit_black.png");
            SetIcon(_btnItemRemove, "delete_black.png");
            SetIcon(_btnItemClear, "clear_black.png");

            SetIcon(_saveButton, "save_black.png");
            SetIcon(_updateButton, "update_black.png");
            SetIcon(_clearButton, "clear_black.png");
            SetIcon(_cancelButton, "cancel_black.png");
            SetIcon(_closeButton, "close_black.png");
        }

        private static void SetIcon(Button button, string fileName)
        {
            if (button == null)
            {
                return;
            }

            var path = FindIconPath(fileName);
            if (path == null)
            {
                return;
            }

            try
            {
                using (var source = Image.FromFile(path))
                {
                    button.Image = new Bitmap(source, new Size(18, 18));
                }

                button.ImageAlign = ContentAlignment.MiddleCenter;
                if (!string.IsNullOrWhiteSpace(button.Text))
                {
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
            }
            catch
            {
                button.Image = null;
            }
        }

        private static string FindIconPath(string fileName)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDirectory, "icones", fileName),
                Path.Combine(baseDirectory, "..", "..", "..", "..", "icones", fileName),
                Path.Combine(baseDirectory, "..", "..", "..", "..", "..", "icones", fileName),
                Path.Combine(Environment.CurrentDirectory, "icones", fileName),
                Path.Combine(Environment.CurrentDirectory, "..", "icones", fileName),
            };

            foreach (var candidate in candidates)
            {
                var fullPath = Path.GetFullPath(candidate);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }

        private void ReloadAllReferences(string supplierCode = null, string warehouseCode = null, string materialCode = null, string lotCode = null)
        {
            _isRefreshingReferences = true;
            try
            {
                supplierCode = supplierCode ?? GetSelectedCode(_supplierComboBox);
                warehouseCode = warehouseCode ?? GetSelectedCode(_warehouseComboBox);
                materialCode = materialCode ?? GetSelectedCode(_materialComboBox);
                lotCode = lotCode ?? GetSelectedCode(_lotComboBox);

                _supplierOptions = ToLookupOptions(_inboundReceiptController.LoadSuppliers(_configuration, _databaseProfile));
                _warehouseOptions = ToLookupOptions(_inboundReceiptController.LoadWarehousesForUser(_configuration, _databaseProfile, _identity.UserName));
                _materialOptions = ToLookupOptions(_inboundReceiptController.LoadMaterialsBySupplier(_configuration, _databaseProfile, supplierCode));
                _lotOptions = ToLookupOptions(_inboundReceiptController.LoadLots(_configuration, _databaseProfile, supplierCode, materialCode));

                BindCombo(_supplierComboBox, _supplierOptions, supplierCode);
                BindCombo(_warehouseComboBox, _warehouseOptions, warehouseCode);
                BindCombo(_materialComboBox, _materialOptions, materialCode);
                BindCombo(_lotComboBox, _lotOptions, lotCode);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            UpdateBrcIndicator();
        }

        private void ReloadSuppliers()
        {
            ReloadAllReferences(GetSelectedCode(_supplierComboBox), GetSelectedCode(_warehouseComboBox), GetSelectedCode(_materialComboBox), GetSelectedCode(_lotComboBox));
        }

        private void ReloadWarehouses()
        {
            ReloadAllReferences(GetSelectedCode(_supplierComboBox), GetSelectedCode(_warehouseComboBox), GetSelectedCode(_materialComboBox), GetSelectedCode(_lotComboBox));
        }

        private void ReloadMaterials()
        {
            var selectedMaterial = GetSelectedCode(_materialComboBox);
            var supplierCode = GetSelectedCode(_supplierComboBox);
            _isRefreshingReferences = true;
            try
            {
                _materialOptions = ToLookupOptions(_inboundReceiptController.LoadMaterialsBySupplier(_configuration, _databaseProfile, supplierCode));
                BindCombo(_materialComboBox, _materialOptions, selectedMaterial);
                _lotOptions = ToLookupOptions(_inboundReceiptController.LoadLots(_configuration, _databaseProfile, supplierCode, selectedMaterial));
                BindCombo(_lotComboBox, _lotOptions, GetSelectedCode(_lotComboBox));
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            UpdateBrcIndicator();
        }

        private void ReloadLots()
        {
            var supplierCode = GetSelectedCode(_supplierComboBox);
            var materialCode = GetSelectedCode(_materialComboBox);
            _lotOptions = ToLookupOptions(_inboundReceiptController.LoadLots(_configuration, _databaseProfile, supplierCode, materialCode));
            BindCombo(_lotComboBox, _lotOptions, GetSelectedCode(_lotComboBox));
        }

        private void OnSupplierSelectionChanged()
        {
            if (_isRefreshingReferences)
            {
                return;
            }

            _isRefreshingReferences = true;
            try
            {
                _materialOptions = ToLookupOptions(_inboundReceiptController.LoadMaterialsBySupplier(_configuration, _databaseProfile, GetSelectedCode(_supplierComboBox)));
                BindCombo(_materialComboBox, _materialOptions, null);
                _lotOptions = new LookupOption[0];
                BindCombo(_lotComboBox, _lotOptions, null);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            UpdateBrcIndicator();
        }

        private void OnMaterialSelectionChanged()
        {
            if (_isRefreshingReferences)
            {
                return;
            }

            _lotOptions = ToLookupOptions(_inboundReceiptController.LoadLots(
                _configuration,
                _databaseProfile,
                GetSelectedCode(_supplierComboBox),
                GetSelectedCode(_materialComboBox)));
            BindCombo(_lotComboBox, _lotOptions, null);
            UpdateBrcIndicator();
        }

        private void OpenReceiptLookup()
        {
            using (var dialog = new ReceiptLookupForm(_inboundReceiptController, _configuration, _databaseProfile))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || dialog.SelectedReceipt == null)
                {
                    return;
                }

                var selected = dialog.SelectedReceipt;
                var requiresLock = !string.Equals(selected.Status, "CANCELADA", StringComparison.OrdinalIgnoreCase);
                var shouldAcquireLock = requiresLock
                    && (!string.Equals(_lockedNumber, selected.Number, StringComparison.OrdinalIgnoreCase)
                        || !string.Equals(_lockedSupplierCode, selected.SupplierCode, StringComparison.OrdinalIgnoreCase));

                try
                {
                    if (shouldAcquireLock)
                    {
                        ReleaseCurrentLockSafe();
                        var lockResult = _inboundReceiptController.TryLockReceipt(_configuration, _databaseProfile, selected.Number, selected.SupplierCode, _identity.UserName);
                        if (!lockResult.Success)
                        {
                            throw new InvalidOperationException(lockResult.Message);
                        }

                        _lockedNumber = selected.Number;
                        _lockedSupplierCode = selected.SupplierCode;
                    }
                    else if (!requiresLock)
                    {
                        ReleaseCurrentLockSafe();
                    }

                    var detail = _inboundReceiptController.LoadReceipt(_configuration, _databaseProfile, selected.Number, selected.SupplierCode);
                    ApplyReceiptDetail(detail, requiresLock);
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

        private void ApplyReceiptDetail(InboundReceiptDetail detail, bool lockHeld)
        {
            _loadedReceiptStatus = detail.Status;
            ReloadAllReferences(detail.SupplierCode, detail.WarehouseCode, null, null);
            EnsureOptionPresent(ref _supplierOptions, _supplierComboBox, detail.SupplierCode, detail.SupplierName);
            EnsureOptionPresent(ref _warehouseOptions, _warehouseComboBox, detail.WarehouseCode, detail.WarehouseName);

            _numberTextBox.Text = detail.Number ?? string.Empty;
            _emissionDateTextBox.Text = detail.EmissionDate ?? string.Empty;
            _receiptDateTimeTextBox.Text = detail.ReceiptDateTime ?? string.Empty;

            _items.Clear();
            _items.AddRange(detail.Items ?? Array.Empty<InboundReceiptItemDetail>());
            _mode = ScreenMode.Consultation;
            if (!lockHeld)
            {
                _lockedNumber = null;
                _lockedSupplierCode = null;
            }

            RefreshItemGrid();
            ClearItemEditor();
            ApplyModeState();
            SetStatus(
                string.Equals(detail.Status, "CANCELADA", StringComparison.OrdinalIgnoreCase)
                    ? "Nota cancelada carregada em modo leitura."
                    : "Nota carregada para consulta.",
                false);
        }

        private void ValidateHeaderForItems()
        {
            if (string.IsNullOrWhiteSpace(_numberTextBox.Text))
            {
                throw new InvalidOperationException("Informe o numero da nota antes de adicionar itens.");
            }

            if (string.IsNullOrWhiteSpace(GetSelectedCode(_supplierComboBox)))
            {
                throw new InvalidOperationException("Selecione o fornecedor antes de adicionar itens.");
            }

            if (string.IsNullOrWhiteSpace(GetSelectedCode(_warehouseComboBox)))
            {
                throw new InvalidOperationException("Selecione o almoxarifado antes de adicionar itens.");
            }

            DateTime emissionDate;
            DateTime receiptDateTime;
            if (!DateTime.TryParseExact(_emissionDateTextBox.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out emissionDate))
            {
                throw new InvalidOperationException("Informe a Data de Emissao corretamente antes de adicionar itens.");
            }

            if (!DateTime.TryParseExact(_receiptDateTimeTextBox.Text, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out receiptDateTime))
            {
                throw new InvalidOperationException("Informe a Data/Hora de Recebimento corretamente antes de adicionar itens.");
            }
        }

        private InboundReceiptItemDetail BuildEditorItem()
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
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Quantidade deve ser maior que zero.");
            }

            return new InboundReceiptItemDetail
            {
                MaterialCode = materialCode,
                MaterialDescription = GetSelectedDescription(_materialComboBox),
                LotCode = lotCode,
                LotName = GetSelectedDescription(_lotComboBox),
                Quantity = quantity,
                Status = "ATIVO",
            };
        }

        private void ValidateDuplicateItem(InboundReceiptItemDetail item, int indexToIgnore)
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
                    throw new InvalidOperationException("Este material e lote ja foram adicionados nesta nota.");
                }
            }
        }

        private void RefreshItemGrid()
        {
            var supplierDisplay = _supplierComboBox.SelectedItem is LookupOption selected
                ? (string.IsNullOrWhiteSpace(selected.Code) ? selected.Description : selected.Code + " - " + selected.Description)
                : string.Empty;

            var rows = _items
                .Select((item, index) => new
                {
                    SourceIndex = index,
                    Item = item,
                })
                .OrderBy(item => item.Item.MaterialDisplay ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Item.LotDisplay ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select((item, visualIndex) => new InboundReceiptItemRow
                {
                    SourceIndex = item.SourceIndex,
                    ItemNumber = visualIndex + 1,
                    SupplierDisplay = supplierDisplay,
                    MaterialDisplay = item.Item.MaterialDisplay,
                    LotDisplay = item.Item.LotDisplay,
                    QuantityText = item.Item.QuantityText,
                    Status = item.Item.Status,
                })
                .ToArray();
            _itemsGrid.DataSource = rows;
            _itemCountLabel.Text = "Itens na nota: " + _items.Count;
        }

        private void StartEditingSelectedItem()
        {
            if (!(_itemsGrid.CurrentRow?.DataBoundItem is InboundReceiptItemRow row))
            {
                SetStatus("Selecione um item para editar.", true);
                return;
            }

            _quantityOnlyEditMode = false;
            _editingItemIndex = row.SourceIndex;
            var item = _items[_editingItemIndex];

            LoadItemIntoEditor(item);
            ApplyModeState();
            SetStatus("Item carregado para edicao.", false);
        }

        private void StartQuantityOnlyEditSelectedItem()
        {
            if (_mode != ScreenMode.Consultation)
            {
                SetStatus("Consulte uma nota antes de alterar a quantidade.", true);
                return;
            }

            if (IsReceiptReadOnlyForItemEdit())
            {
                SetStatus("Nota cancelada/inativa nao permite alteracao.", true);
                return;
            }

            if (!(_itemsGrid.CurrentRow?.DataBoundItem is InboundReceiptItemRow row))
            {
                SetStatus("Selecione um item para alterar a quantidade.", true);
                return;
            }

            _editingItemIndex = row.SourceIndex;
            _quantityOnlyEditMode = true;
            LoadItemIntoEditor(_items[_editingItemIndex]);
            ApplyModeState();
            _quantityTextBox.Focus();
            _quantityTextBox.SelectAll();
            SetStatus("Altere apenas a quantidade e clique em Alterar para confirmar.", false);
        }

        private void LoadItemIntoEditor(InboundReceiptItemDetail item)
        {
            EnsureOptionPresent(ref _materialOptions, _materialComboBox, item.MaterialCode, item.MaterialDescription);
            _isRefreshingReferences = true;
            try
            {
                _lotOptions = ToLookupOptions(_inboundReceiptController.LoadLots(
                    _configuration,
                    _databaseProfile,
                    GetSelectedCode(_supplierComboBox),
                    item.MaterialCode));
                BindCombo(_lotComboBox, _lotOptions, null);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            EnsureOptionPresent(ref _lotOptions, _lotComboBox, item.LotCode, item.LotName);
            _quantityTextBox.Text = item.Quantity.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
            UpdateBrcIndicator();
        }

        private void ConfirmQuantityOnlyEdit()
        {
            if (_editingItemIndex < 0 || _editingItemIndex >= _items.Count)
            {
                SetStatus("Selecione um item para alterar a quantidade.", true);
                return;
            }

            if (IsReceiptReadOnlyForItemEdit())
            {
                SetStatus("Nota cancelada/inativa nao permite alteracao.", true);
                return;
            }

            try
            {
                var item = _items[_editingItemIndex];
                var previousQuantity = item.Quantity;
                var quantity = ParseQuantity(_quantityTextBox.Text);
                if (quantity <= 0)
                {
                    throw new InvalidOperationException("Quantidade deve ser maior que zero.");
                }

                item.Quantity = quantity;
                _quantityOnlyEditMode = false;
                _editingItemIndex = -1;
                RefreshItemGrid();
                ClearItemEditor();
                ApplyModeState();

                if (TryUpdateReceipt(clearAfterSuccess: false, successMessage: "Quantidade alterada e nota salva com sucesso."))
                {
                    SetStatus("Quantidade alterada com sucesso.", false);
                    return;
                }

                item.Quantity = previousQuantity;
                RefreshItemGrid();
                ApplyModeState();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void RemoveSelectedItem()
        {
            if (!(_itemsGrid.CurrentRow?.DataBoundItem is InboundReceiptItemRow row))
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

        private SaveInboundReceiptRequest BuildSaveRequest()
        {
            return new SaveInboundReceiptRequest
            {
                Number = _numberTextBox.Text,
                SupplierCode = GetSelectedCode(_supplierComboBox),
                WarehouseCode = GetSelectedCode(_warehouseComboBox),
                EmissionDate = _emissionDateTextBox.Text,
                ReceiptDateTime = _receiptDateTimeTextBox.Text,
                ActorUserName = _identity.UserName,
                Items = _items.Select(item => new InboundReceiptItemInput
                {
                    MaterialCode = item.MaterialCode,
                    LotCode = item.LotCode,
                    Quantity = item.Quantity,
                }).ToArray(),
            };
        }

        private void ClearForm(bool confirm, bool releaseLock)
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
            _loadedReceiptStatus = "ATIVO";
            _numberTextBox.Clear();
            _isRefreshingReferences = true;
            try
            {
                _supplierComboBox.SelectedIndex = -1;
                _warehouseComboBox.SelectedIndex = -1;
                _materialOptions = ToLookupOptions(_inboundReceiptController.LoadMaterials(_configuration, _databaseProfile));
                BindCombo(_materialComboBox, _materialOptions, null);
                _lotOptions = new LookupOption[0];
                BindCombo(_lotComboBox, _lotOptions, null);
            }
            finally
            {
                _isRefreshingReferences = false;
            }

            _emissionDateTextBox.Text = DateTime.Today.ToString("dd/MM/yyyy");
            _receiptDateTimeTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            _items.Clear();
            _editingItemIndex = -1;
            RefreshItemGrid();
            ClearItemEditor();
            ApplyModeState();
            SetStatus("Preencha os dados da nota e adicione os itens.", false);
        }

        private void ClearItemEditor()
        {
            _quantityOnlyEditMode = false;
            _editingItemIndex = -1;
            _materialComboBox.SelectedIndex = -1;
            _lotComboBox.SelectedIndex = -1;
            _quantityTextBox.Clear();
            UpdateBrcIndicator();
        }

        private void ApplyModeState()
        {
            var readOnlyCancelled = IsReceiptReadOnlyForItemEdit();
            var headerLocked = _mode == ScreenMode.Consultation || _items.Count > 0;
            var quantityOnlyEdit = _quantityOnlyEditMode && !readOnlyCancelled;
            var headerEditable = !quantityOnlyEdit && !headerLocked && !readOnlyCancelled;
            var itemEditable = !quantityOnlyEdit && !readOnlyCancelled;

            _saveButton.Enabled = _mode == ScreenMode.Creation && !readOnlyCancelled && !quantityOnlyEdit;
            _updateButton.Enabled = _mode == ScreenMode.Consultation && !readOnlyCancelled;
            _cancelButton.Enabled = _mode == ScreenMode.Consultation && !readOnlyCancelled && !quantityOnlyEdit;

            _numberTextBox.Enabled = headerEditable;
            _supplierComboBox.Enabled = headerEditable;
            _warehouseComboBox.Enabled = headerEditable;
            _emissionDateTextBox.Enabled = !readOnlyCancelled && !quantityOnlyEdit && (_mode == ScreenMode.Creation ? !headerLocked : true);
            _receiptDateTimeTextBox.Enabled = !readOnlyCancelled && !quantityOnlyEdit && (_mode == ScreenMode.Creation ? !headerLocked : true);
            _materialComboBox.Enabled = itemEditable;
            _lotComboBox.Enabled = itemEditable;
            _quantityTextBox.Enabled = !readOnlyCancelled;

            _btnNumberLookup.Enabled = !quantityOnlyEdit && !readOnlyCancelled;
            _btnSupplierRefresh.Enabled = headerEditable;
            _btnSupplierLookup.Enabled = headerEditable;
            _btnSupplierNew.Enabled = headerEditable;
            _btnWarehouseRefresh.Enabled = headerEditable;
            _btnWarehouseLookup.Enabled = headerEditable;
            _btnMaterialRefresh.Enabled = itemEditable;
            _btnMaterialLookup.Enabled = itemEditable;
            _btnMaterialNew.Enabled = itemEditable;
            _btnLotRefresh.Enabled = itemEditable;
            _btnLotLookup.Enabled = itemEditable;
            _btnLotNew.Enabled = itemEditable;
            _btnItemAdd.Enabled = itemEditable;
            _btnItemEdit.Enabled = itemEditable;
            _btnItemRemove.Enabled = itemEditable;
            _btnItemClear.Enabled = !readOnlyCancelled;
        }

        private bool IsReceiptReadOnlyForItemEdit()
        {
            return _mode == ScreenMode.Consultation
                && (string.Equals(_loadedReceiptStatus, "CANCELADA", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(_loadedReceiptStatus, "INATIVO", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(_loadedReceiptStatus, "INATIVA", StringComparison.OrdinalIgnoreCase));
        }

        private void ReleaseCurrentLockSafe()
        {
            if (string.IsNullOrWhiteSpace(_lockedNumber) || string.IsNullOrWhiteSpace(_lockedSupplierCode) || _configuration == null)
            {
                _lockedNumber = null;
                _lockedSupplierCode = null;
                return;
            }

            try
            {
                _inboundReceiptController.ReleaseReceiptLock(_configuration, _databaseProfile, _lockedNumber, _lockedSupplierCode, _identity.UserName);
            }
            catch
            {
            }

            _lockedNumber = null;
            _lockedSupplierCode = null;
        }

        private void OpenSupplierLookup()
        {
            using (var dialog = new LookupSelectionForm("Selecionar Fornecedor", "Nome", _supplierOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_supplierComboBox, _supplierOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenMaterialLookup()
        {
            using (var dialog = new LookupSelectionForm("Selecionar Material", "Descricao", _materialOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_materialComboBox, _materialOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenWarehouseLookup()
        {
            using (var dialog = new LookupSelectionForm("Selecionar Almoxarifado", "Nome", _warehouseOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_warehouseComboBox, _warehouseOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenLotLookup()
        {
            if (string.IsNullOrWhiteSpace(GetSelectedCode(_supplierComboBox)))
            {
                SetStatus("Selecione o fornecedor antes de buscar lotes.", true);
                return;
            }

            ReloadLots();
            using (var dialog = new LookupSelectionForm("Selecionar Lote", "Nome", _lotOptions))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_lotComboBox, _lotOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenSupplierManagement()
        {
            var supplierCode = GetSelectedCode(_supplierComboBox);
            using (var dialog = new SupplierManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadAllReferences(supplierCode, GetSelectedCode(_warehouseComboBox), GetSelectedCode(_materialComboBox), GetSelectedCode(_lotComboBox));
        }

        private void OpenPackagingManagement()
        {
            var supplierCode = GetSelectedCode(_supplierComboBox);
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            using (var dialog = new PackagingManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadAllReferences(supplierCode, warehouseCode, GetSelectedCode(_materialComboBox), GetSelectedCode(_lotComboBox));
        }

        private void OpenWarehouseManagement()
        {
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            using (var dialog = new WarehouseManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                dialog.ShowDialog(this);
            }

            ReloadAllReferences(GetSelectedCode(_supplierComboBox), warehouseCode, GetSelectedCode(_materialComboBox), GetSelectedCode(_lotComboBox));
        }

        private void OpenLotManagement()
        {
            using (var dialog = new LotManagementForm(_compositionRoot, _identity, _databaseProfile, GetSelectedCode(_materialComboBox), GetSelectedCode(_supplierComboBox)))
            {
                dialog.ShowDialog(this);
            }

            ReloadAllReferences(GetSelectedCode(_supplierComboBox), GetSelectedCode(_warehouseComboBox), GetSelectedCode(_materialComboBox), GetSelectedCode(_lotComboBox));
        }

        private void UpdateBrcIndicator()
        {
            try
            {
                var materialCode = GetSelectedCode(_materialComboBox);
                if (string.IsNullOrWhiteSpace(materialCode))
                {
                    _brcLabel.Text = "BRC: -";
                    _brcLabel.ForeColor = Color.DimGray;
                    return;
                }

                var enabled = _inboundReceiptController.IsMaterialBrcEnabled(_configuration, _databaseProfile, materialCode);
                _brcLabel.Text = enabled ? "BRC: Sim" : "BRC: Nao";
                _brcLabel.ForeColor = enabled ? Color.FromArgb(0, 102, 204) : Color.DimGray;
            }
            catch
            {
                _brcLabel.Text = "BRC: -";
                _brcLabel.ForeColor = Color.DimGray;
            }
        }

        private void NormalizeNoteNumberInput()
        {
            var digits = new string((_numberTextBox.Text ?? string.Empty).Where(char.IsDigit).ToArray());
            if (!string.Equals(_numberTextBox.Text, digits, StringComparison.Ordinal))
            {
                _numberTextBox.Text = digits;
                _numberTextBox.SelectionStart = _numberTextBox.Text.Length;
            }
        }

        private void CloseForm()
        {
            DialogResult = _hasChanges ? DialogResult.OK : DialogResult.Cancel;
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

        private static string NormalizeDateInput(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).Take(8).ToArray());
            if (digits.Length > 2) digits = digits.Insert(2, "/");
            if (digits.Length > 5) digits = digits.Insert(5, "/");
            return digits;
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

            var normalized = (value ?? string.Empty).Trim().Replace(".", string.Empty).Replace(",", ".");
            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            throw new InvalidOperationException("Quantidade invalida.");
        }

        private static LookupOption[] ToLookupOptions(SupplierSummary[] items)
        {
            return (items ?? new SupplierSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
        }

        private static LookupOption[] ToLookupOptions(PackagingSummary[] items)
        {
            return (items ?? new PackagingSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Description, Status = item.Status }).ToArray();
        }

        private static LookupOption[] ToLookupOptions(WarehouseSummary[] items)
        {
            return (items ?? new WarehouseSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
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

        private static void SelectOptionByCode(ComboBox comboBox, LookupOption[] options, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            for (var index = 0; index < options.Length; index++)
            {
                if (string.Equals(options[index].Code, code, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
            }

            comboBox.SelectedIndex = -1;
        }

        private static void EnsureOptionPresent(ref LookupOption[] options, ComboBox comboBox, string code, string description)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            if (options.All(item => !string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase)))
            {
                options = options.Concat(new[]
                {
                    new LookupOption
                    {
                        Code = code,
                        Description = string.IsNullOrWhiteSpace(description) ? code : description,
                        Status = "INATIVO",
                    },
                }).OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase).ToArray();
                BindCombo(comboBox, options, code);
                return;
            }

            SelectOptionByCode(comboBox, options, code);
        }

        private static string GetSelectedCode(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as LookupOption)?.Code;
        }

        private static string GetSelectedDescription(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as LookupOption)?.Description;
        }

        private sealed class InboundReceiptItemRow
        {
            public int SourceIndex { get; set; }

            public int ItemNumber { get; set; }

            public string SupplierDisplay { get; set; }

            public string MaterialDisplay { get; set; }

            public string LotDisplay { get; set; }

            public string QuantityText { get; set; }

            public string Status { get; set; }
        }
    }
}
