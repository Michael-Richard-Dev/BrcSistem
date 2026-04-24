using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class MovimentacaoEntradaForm : Form
    {
        private enum ScreenMode
        {
            Creation,
            Consultation
        }

        private readonly CompositionRoot _compositionRoot;
        private readonly InboundReceiptController _inboundReceiptController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private LookupOption[] _supplierOptions;
        private LookupOption[] _materialOptions;
        private LookupOption[] _warehouseOptions;
        private LookupOption[] _lotOptions;
        private readonly List<InboundReceiptItemDetail> _items;

        private ScreenMode _mode;
        private int _editingItemIndex;
        private bool _hasChanges;
        private bool _isRefreshingReferences;
        private string _lockedNumber;
        private string _lockedSupplierCode;
        private string _loadedReceiptStatus;

        public MovimentacaoEntradaForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _inboundReceiptController = compositionRoot.CreateInboundReceiptController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _supplierOptions = new LookupOption[0];
            _materialOptions = new LookupOption[0];
            _warehouseOptions = new LookupOption[0];
            _lotOptions = new LookupOption[0];
            _items = new List<InboundReceiptItemDetail>();
            _editingItemIndex = -1;

            InitializeComponent();
            ApplyActionIcons();
        }

        // ===============================
        // Form lifecycle handlers
        // ===============================
        private void OnFormLoad(object sender, EventArgs e)
        {
            LoadData();
        }

        private void OnFormClosingHandler(object sender, FormClosingEventArgs e)
        {
            ReleaseCurrentLockSafe();
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) SaveReceipt();
            else if (e.KeyCode == Keys.F3) UpdateReceipt();
            else if (e.KeyCode == Keys.F4) CloseForm();
            else if (e.KeyCode == Keys.F5) ClearForm(confirm: true, releaseLock: true);
            else if (e.KeyCode == Keys.F6) CancelReceipt();
        }

        // ===============================
        // Event handler wrappers (Designer-friendly, no lambdas)
        // ===============================
        private void OnNumberTextChanged(object sender, EventArgs e) { NormalizeNoteNumberInput(); }
        private void OnSupplierComboChanged(object sender, EventArgs e) { OnSupplierSelectionChanged(); }
        private void OnMaterialComboChanged(object sender, EventArgs e) { OnMaterialSelectionChanged(); }
        private void OnEmissionDateLeave(object sender, EventArgs e) { _emissionDateTextBox.Text = NormalizeDateInput(_emissionDateTextBox.Text); }
        private void OnReceiptDateLeave(object sender, EventArgs e) { _receiptDateTimeTextBox.Text = NormalizeDateTimeInput(_receiptDateTimeTextBox.Text); }
        private void OnItemsGridCellDoubleClick(object sender, DataGridViewCellEventArgs e) { StartEditingSelectedItem(); }

        private void OnBtnNumberLookupClick(object sender, EventArgs e) { OpenReceiptLookup(); }
        private void OnBtnSupplierRefreshClick(object sender, EventArgs e) { ReloadSuppliers(); }
        private void OnBtnSupplierLookupClick(object sender, EventArgs e) { OpenSupplierLookup(); }
        private void OnBtnSupplierNewClick(object sender, EventArgs e) { OpenSupplierManagement(); }
        private void OnBtnWarehouseRefreshClick(object sender, EventArgs e) { ReloadWarehouses(); }
        private void OnBtnWarehouseLookupClick(object sender, EventArgs e) { OpenWarehouseLookup(); }
        private void OnBtnMaterialRefreshClick(object sender, EventArgs e) { ReloadMaterials(); }
        private void OnBtnMaterialLookupClick(object sender, EventArgs e) { OpenMaterialLookup(); }
        private void OnBtnMaterialNewClick(object sender, EventArgs e) { OpenPackagingManagement(); }
        private void OnBtnLotRefreshClick(object sender, EventArgs e) { ReloadLots(); }
        private void OnBtnLotLookupClick(object sender, EventArgs e) { OpenLotLookup(); }
        private void OnBtnLotNewClick(object sender, EventArgs e) { OpenLotManagement(); }

        private void OnBtnItemAddClick(object sender, EventArgs e) { AddOrUpdateItem(); }
        private void OnBtnItemEditClick(object sender, EventArgs e) { StartEditingSelectedItem(); }
        private void OnBtnItemRemoveClick(object sender, EventArgs e) { RemoveSelectedItem(); }
        private void OnBtnItemClearClick(object sender, EventArgs e) { ClearItemEditor(); }

        private void OnBtnSaveClick(object sender, EventArgs e) { SaveReceipt(); }
        private void OnBtnUpdateClick(object sender, EventArgs e) { UpdateReceipt(); }
        private void OnBtnClearClick(object sender, EventArgs e) { ClearForm(confirm: true, releaseLock: true); }
        private void OnBtnCancelClick(object sender, EventArgs e) { CancelReceipt(); }
        private void OnBtnCloseClick(object sender, EventArgs e) { CloseForm(); }

        // ===============================
        // Core operations
        // ===============================
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                ReloadAllReferences();
                ClearForm(confirm: false, releaseLock: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void AddOrUpdateItem()
        {
            try
            {
                ValidateHeaderForItems();
                var item = BuildEditorItem();
                if (_editingItemIndex >= 0)
                {
                    ValidateDuplicateItem(item, _editingItemIndex);
                    _items[_editingItemIndex] = item;
                    SetStatus("Item alterado com sucesso.", false);
                }
                else
                {
                    ValidateDuplicateItem(item, -1);
                    _items.Add(item);
                    SetStatus("Item adicionado com sucesso.", false);
                }

                _editingItemIndex = -1;
                RefreshItemGrid();
                ClearItemEditor();
                ApplyModeState();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void SaveReceipt()
        {
            if (_mode != ScreenMode.Creation)
            {
                SetStatus("Use Alterar para atualizar uma nota consultada.", true);
                return;
            }

            try
            {
                _inboundReceiptController.CreateReceipt(_configuration, _databaseProfile, BuildSaveRequest());
                _hasChanges = true;
                MessageBox.Show(this, "Nota salva com sucesso.", "Entrada de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm(confirm: false, releaseLock: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateReceipt()
        {
            if (_mode != ScreenMode.Consultation)
            {
                SetStatus("Consulte uma nota antes de alterar.", true);
                return;
            }

            try
            {
                _inboundReceiptController.UpdateReceipt(_configuration, _databaseProfile, BuildSaveRequest());
                _hasChanges = true;
                MessageBox.Show(this, "Nota alterada com sucesso.", "Entrada de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm(confirm: false, releaseLock: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void CancelReceipt()
        {
            if (_mode != ScreenMode.Consultation)
            {
                SetStatus("Consulte uma nota antes de cancelar.", true);
                return;
            }

            if (MessageBox.Show(this, "Deseja realmente cancelar esta nota fiscal?", "Confirmar Cancelamento", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _inboundReceiptController.CancelReceipt(_configuration, _databaseProfile, _numberTextBox.Text, GetSelectedCode(_supplierComboBox), _identity.UserName);
                _hasChanges = true;
                MessageBox.Show(this, "Nota cancelada com sucesso.", "Entrada de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm(confirm: false, releaseLock: true);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }
    }
}
