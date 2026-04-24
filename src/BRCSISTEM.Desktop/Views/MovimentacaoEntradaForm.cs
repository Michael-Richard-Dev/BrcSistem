using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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
        private readonly bool _isDesignerInstance;

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

        public MovimentacaoEntradaForm()
            : this(null, null, null, true)
        {
        }

        public MovimentacaoEntradaForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile)
            : this(compositionRoot, identity, databaseProfile, false)
        {
        }

        private MovimentacaoEntradaForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile,
            bool designerCtor)
        {
            _compositionRoot = compositionRoot;
            _identity = identity;
            _databaseProfile = databaseProfile;
            _isDesignerInstance = designerCtor;

            if (!designerCtor)
            {
                if (_compositionRoot == null)
                {
                    throw new ArgumentNullException(nameof(compositionRoot));
                }

                _inboundReceiptController = _compositionRoot.CreateInboundReceiptController();
                _configurationController = _compositionRoot.CreateConfigurationController();
            }

            _supplierOptions = new LookupOption[0];
            _materialOptions = new LookupOption[0];
            _warehouseOptions = new LookupOption[0];
            _lotOptions = new LookupOption[0];
            _items = new List<InboundReceiptItemDetail>();
            _editingItemIndex = -1;

            InitializeComponent();

            if (!IsDesignModeActive)
            {
                WireRuntimeEvents();
                ApplyActionIcons();
                ConfigureItemsGridColumns();
            }
        }

        private bool IsDesignModeActive
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                    || _isDesignerInstance
                    || DesignMode
                    || (Site != null && Site.DesignMode);
            }
        }

        private void WireRuntimeEvents()
        {
            Load += OnFormLoad;
            FormClosing += OnFormClosingHandler;
            KeyDown += OnFormKeyDown;

            _numberTextBox.TextChanged += OnNumberTextChanged;
            _supplierComboBox.SelectedIndexChanged += OnSupplierComboChanged;
            _materialComboBox.SelectedIndexChanged += OnMaterialComboChanged;
            _emissionDateTextBox.Leave += OnEmissionDateLeave;
            _receiptDateTimeTextBox.Leave += OnReceiptDateLeave;
            _itemsGrid.CellDoubleClick += OnItemsGridCellDoubleClick;

            _btnNumberLookup.Click += OnBtnNumberLookupClick;
            _btnSupplierRefresh.Click += OnBtnSupplierRefreshClick;
            _btnSupplierLookup.Click += OnBtnSupplierLookupClick;
            _btnSupplierNew.Click += OnBtnSupplierNewClick;
            _btnWarehouseRefresh.Click += OnBtnWarehouseRefreshClick;
            _btnWarehouseLookup.Click += OnBtnWarehouseLookupClick;
            _btnMaterialRefresh.Click += OnBtnMaterialRefreshClick;
            _btnMaterialLookup.Click += OnBtnMaterialLookupClick;
            _btnMaterialNew.Click += OnBtnMaterialNewClick;
            _btnLotRefresh.Click += OnBtnLotRefreshClick;
            _btnLotLookup.Click += OnBtnLotLookupClick;
            _btnLotNew.Click += OnBtnLotNewClick;

            _btnItemAdd.Click += OnBtnItemAddClick;
            _btnItemEdit.Click += OnBtnItemEditClick;
            _btnItemRemove.Click += OnBtnItemRemoveClick;
            _btnItemClear.Click += OnBtnItemClearClick;

            _saveButton.Click += OnBtnSaveClick;
            _updateButton.Click += OnBtnUpdateClick;
            _clearButton.Click += OnBtnClearClick;
            _cancelButton.Click += OnBtnCancelClick;
            _closeButton.Click += OnBtnCloseClick;
        }

        private void ConfigureItemsGridColumns()
        {
            if (_itemsGrid == null)
            {
                return;
            }

            _itemsGrid.AutoGenerateColumns = false;
            _itemsGrid.Columns.Clear();

            var colItem = new DataGridViewTextBoxColumn
            {
                Name = "item",
                HeaderText = "ITEM",
                DataPropertyName = nameof(InboundReceiptItemRow.ItemNumber),
                Width = 60
            };
            colItem.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colFornecedor = new DataGridViewTextBoxColumn
            {
                Name = "fornecedor",
                HeaderText = "FORNECEDOR",
                DataPropertyName = nameof(InboundReceiptItemRow.SupplierDisplay),
                Width = 220
            };
            colFornecedor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            var colMaterial = new DataGridViewTextBoxColumn
            {
                Name = "material",
                HeaderText = "MATERIAL",
                DataPropertyName = nameof(InboundReceiptItemRow.MaterialDisplay),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 55
            };
            colMaterial.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            var colLote = new DataGridViewTextBoxColumn
            {
                Name = "lote",
                HeaderText = "LOTE",
                DataPropertyName = nameof(InboundReceiptItemRow.LotDisplay),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 45
            };
            colLote.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            var colQuantidade = new DataGridViewTextBoxColumn
            {
                Name = "quantidade",
                HeaderText = "QUANTIDADE",
                DataPropertyName = nameof(InboundReceiptItemRow.QuantityText),
                Width = 120
            };
            colQuantidade.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var colStatus = new DataGridViewTextBoxColumn
            {
                Name = "status",
                HeaderText = "STATUS",
                DataPropertyName = nameof(InboundReceiptItemRow.Status),
                Width = 100
            };
            colStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            _itemsGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                colItem,
                colFornecedor,
                colMaterial,
                colLote,
                colQuantidade,
                colStatus
            });
        }

        // ===============================
        // Form lifecycle handlers
        // ===============================
        private void OnFormLoad(object sender, EventArgs e)
        {
            if (IsDesignModeActive || _configurationController == null)
            {
                return;
            }

            LoadData();
        }

        private void OnFormClosingHandler(object sender, FormClosingEventArgs e)
        {
            if (IsDesignModeActive)
            {
                return;
            }

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
            if (IsDesignModeActive || _configurationController == null)
            {
                return;
            }

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
            if (IsDesignModeActive)
            {
                return;
            }

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
            if (IsDesignModeActive || _inboundReceiptController == null)
            {
                return;
            }

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
            if (IsDesignModeActive || _inboundReceiptController == null)
            {
                return;
            }

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
            if (IsDesignModeActive || _inboundReceiptController == null)
            {
                return;
            }

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
