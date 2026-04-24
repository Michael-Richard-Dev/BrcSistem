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
            ApplyVisualDefaults();

            if (!IsDesignModeActive)
            {
                WireRuntimeEvents();
                ApplyActionIcons();
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

        private void ApplyVisualDefaults()
        {
            ConfigureFieldLabel(_numberLabel, "No Nota:");
            ConfigureFieldLabel(_supplierLabel, "Fornecedor:");
            ConfigureFieldLabel(_warehouseLabel, "Almoxarifado:");
            ConfigureFieldLabel(_emissionDateLabel, "Data Emissao:");
            ConfigureFieldLabel(_receiptDateLabel, "Data/Hora Recebimento:");
            _receiptDateLabel.Margin = new Padding(20, 0, 3, 0);

            ConfigureFieldLabel(_materialLabel, "Material:");
            ConfigureFieldLabel(_lotLabel, "Lote:");
            ConfigureFieldLabel(_quantityLabel, "Quantidade:");

            ConfigureCellTextBox(_numberTextBox);
            ConfigureCellTextBox(_emissionDateTextBox);
            ConfigureCellTextBox(_receiptDateTimeTextBox);
            ConfigureCellTextBox(_quantityTextBox);
            _quantityTextBox.TextAlign = HorizontalAlignment.Right;

            ConfigureCellCombo(_supplierComboBox);
            ConfigureCellCombo(_warehouseComboBox);
            ConfigureCellCombo(_materialComboBox);
            ConfigureCellCombo(_lotComboBox);

            ConfigureIconButton(_btnNumberLookup, "Buscar nota");
            ConfigureIconButton(_btnSupplierRefresh, "Atualizar");
            ConfigureIconButton(_btnSupplierLookup, "Buscar");
            ConfigureIconButton(_btnSupplierNew, "Novo fornecedor");

            ConfigureIconButton(_btnWarehouseRefresh, "Atualizar");
            ConfigureIconButton(_btnWarehouseLookup, "Buscar");

            ConfigureIconButton(_btnMaterialRefresh, "Atualizar");
            ConfigureIconButton(_btnMaterialLookup, "Buscar");
            ConfigureIconButton(_btnMaterialNew, "Nova embalagem");

            ConfigureIconButton(_btnLotRefresh, "Atualizar");
            ConfigureIconButton(_btnLotLookup, "Buscar");
            ConfigureIconButton(_btnLotNew, "Novo lote");

            ConfigureIconButton(_btnItemAdd, "Adicionar");
            ConfigureIconButton(_btnItemEdit, "Editar");
            ConfigureIconButton(_btnItemRemove, "Remover");
            ConfigureIconButton(_btnItemClear, "Limpar item");

            ConfigureActionButton(_saveButton, "Salvar Nota (F2)", 170);
            ConfigureActionButton(_updateButton, "Alterar (F3)", 146);
            ConfigureActionButton(_clearButton, "Limpar (F5)", 138);
            ConfigureActionButton(_cancelButton, "Cancelar Nota (F6)", 184);
            ConfigureActionButton(_closeButton, "Fechar (F4)", 136);

            _actionsLabel.Dock = DockStyle.Fill;
            _actionsLabel.AutoSize = false;
            _actionsLabel.Text = "Acoes:";
            _actionsLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            _actionsLabel.ForeColor = Color.FromArgb(102, 102, 102);
            _actionsLabel.TextAlign = ContentAlignment.MiddleRight;
            _actionsLabel.Margin = new Padding(0, 0, 5, 0);

            _brcLabel.Dock = DockStyle.Fill;
            _brcLabel.AutoSize = false;
            _brcLabel.Text = "BRC: -";
            _brcLabel.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            _brcLabel.ForeColor = Color.FromArgb(102, 102, 102);
            _brcLabel.TextAlign = ContentAlignment.MiddleLeft;
            _brcLabel.Margin = new Padding(16, 0, 3, 0);

            _statusLabel.Dock = DockStyle.Fill;
            _statusLabel.AutoSize = false;
            _statusLabel.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            _statusLabel.ForeColor = Color.SeaGreen;
            _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            _statusLabel.Margin = new Padding(3, 4, 3, 0);

            var headerStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9.25F, FontStyle.Bold),
                BackColor = Color.FromArgb(240, 240, 240),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            _itemsGrid.ColumnHeadersDefaultCellStyle = headerStyle;
            ConfigureItemsGridColumns();
        }

        private void ConfigureItemsGridColumns()
        {
            if (_itemsGrid == null)
            {
                return;
            }

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

        private static void ConfigureFieldLabel(Label label, string text)
        {
            label.Dock = DockStyle.Fill;
            label.AutoSize = false;
            label.Text = text;
            label.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(27, 54, 93);
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Margin = new Padding(3, 0, 6, 0);
        }

        private static void ConfigureCellTextBox(TextBox textBox)
        {
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            textBox.AutoSize = false;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 9.75F);
            textBox.Height = 29;
            textBox.Margin = new Padding(5, 4, 5, 4);
            textBox.MinimumSize = new Size(0, 29);
        }

        private static void ConfigureCellCombo(ComboBox comboBox)
        {
            comboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Font = new Font("Segoe UI", 9.75F);
            comboBox.Height = 29;
            comboBox.Margin = new Padding(5, 4, 5, 4);
            comboBox.MinimumSize = new Size(0, 29);
            comboBox.FlatStyle = FlatStyle.Standard;
        }

        private static void ConfigureIconButton(Button button, string accessibleName)
        {
            button.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button.Height = 30;
            button.MinimumSize = new Size(32, 30);
            button.Text = string.Empty;
            button.FlatStyle = FlatStyle.Standard;
            button.Margin = new Padding(2, 4, 2, 4);
            button.Padding = new Padding(0);
            button.UseVisualStyleBackColor = true;
            button.AccessibleName = accessibleName;
            button.ImageAlign = ContentAlignment.MiddleCenter;
        }

        private static void ConfigureActionButton(Button button, string text, int width)
        {
            button.Text = text;
            button.AutoSize = false;
            button.Height = 34;
            button.Width = width;
            button.FlatStyle = FlatStyle.Standard;
            button.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular);
            button.Margin = new Padding(5, 2, 5, 2);
            button.Padding = new Padding(8, 0, 8, 0);
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleCenter;
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
