using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class InboundReceiptForm : Form
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

        private TextBox _numberTextBox;
        private ComboBox _supplierComboBox;
        private ComboBox _warehouseComboBox;
        private TextBox _emissionDateTextBox;
        private TextBox _receiptDateTimeTextBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private TextBox _quantityTextBox;
        private DataGridView _itemsGrid;
        private Button _saveButton;
        private Button _updateButton;
        private Button _cancelButton;
        private Label _statusLabel;
        private Label _brcLabel;
        private Label _itemCountLabel;

        private ScreenMode _mode;
        private int _editingItemIndex;
        private bool _hasChanges;
        private bool _isRefreshingReferences;
        private string _lockedNumber;
        private string _lockedSupplierCode;
        private string _loadedReceiptStatus;

        public InboundReceiptForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
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
            Load += (sender, args) => LoadData();
            FormClosing += OnFormClosing;
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Entrada de Estoque (NF)";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1280, 760);
            MinimumSize = new Size(1100, 680);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildHeaderPanel(), 0, 0);
            root.Controls.Add(BuildItemPanel(), 0, 1);
            root.Controls.Add(BuildGridPanel(), 0, 2);
            root.Controls.Add(BuildFooterPanel(), 0, 3);
            Controls.Add(root);
        }

        private Control BuildHeaderPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, Height = 180, Text = "Dados da Nota", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var line1 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 12, AutoSize = true };
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));

            line1.Controls.Add(CreateFieldLabel("No Nota:"), 0, 0);
            _numberTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            _numberTextBox.TextChanged += (sender, args) => NormalizeNoteNumberInput();
            line1.Controls.Add(_numberTextBox, 1, 0);
            line1.Controls.Add(CreateButton("Buscar", (sender, args) => OpenReceiptLookup()), 2, 0);

            line1.Controls.Add(CreateFieldLabel("Fornecedor:"), 3, 0);
            _supplierComboBox = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _supplierComboBox.SelectedIndexChanged += (sender, args) => OnSupplierSelectionChanged();
            line1.Controls.Add(_supplierComboBox, 4, 0);
            line1.Controls.Add(CreateButton("Atualizar", (sender, args) => ReloadSuppliers()), 5, 0);
            line1.Controls.Add(CreateButton("Buscar", (sender, args) => OpenSupplierLookup()), 6, 0);

            line1.Controls.Add(CreateFieldLabel("Almoxarifado:"), 7, 0);
            _warehouseComboBox = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line1.Controls.Add(_warehouseComboBox, 8, 0);
            line1.Controls.Add(CreateButton("Atualizar", (sender, args) => ReloadWarehouses()), 9, 0);
            line1.Controls.Add(CreateButton("Buscar", (sender, args) => OpenWarehouseLookup()), 10, 0);
            line1.Controls.Add(CreateButton("Novo", (sender, args) => OpenWarehouseManagement()), 11, 0);

            var line2 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 8, AutoSize = true };
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            line2.Controls.Add(CreateFieldLabel("Data Emissao:"), 0, 0);
            _emissionDateTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            _emissionDateTextBox.Leave += (sender, args) => _emissionDateTextBox.Text = NormalizeDateInput(_emissionDateTextBox.Text);
            line2.Controls.Add(_emissionDateTextBox, 1, 0);

            line2.Controls.Add(CreateFieldLabel("Data/Hora Recebimento:"), 2, 0);
            _receiptDateTimeTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            _receiptDateTimeTextBox.Leave += (sender, args) => _receiptDateTimeTextBox.Text = NormalizeDateTimeInput(_receiptDateTimeTextBox.Text);
            line2.Controls.Add(_receiptDateTimeTextBox, 3, 0);
            line2.Controls.Add(CreateButton("Novo Forn", (sender, args) => OpenSupplierManagement()), 4, 0);
            line2.Controls.Add(CreateButton("Nova Emb", (sender, args) => OpenPackagingManagement()), 5, 0);
            line2.Controls.Add(CreateButton("Novo Lote", (sender, args) => OpenLotManagement()), 6, 0);

            _statusLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(0, 10, 0, 0) };

            root.Controls.Add(line1, 0, 0);
            root.Controls.Add(line2, 0, 1);
            root.Controls.Add(_statusLabel, 0, 2);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildItemPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, Height = 110, Text = "Adicionar / Editar Item", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var line = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), WrapContents = true, AutoScroll = true };

            line.Controls.Add(CreateFieldLabel("Material:"));
            _materialComboBox = new ComboBox { Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _materialComboBox.SelectedIndexChanged += (sender, args) => OnMaterialSelectionChanged();
            line.Controls.Add(_materialComboBox);
            line.Controls.Add(CreateButton("Atualizar", (sender, args) => ReloadMaterials()));
            line.Controls.Add(CreateButton("Buscar", (sender, args) => OpenMaterialLookup()));

            line.Controls.Add(CreateFieldLabel("Lote:"));
            _lotComboBox = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line.Controls.Add(_lotComboBox);
            line.Controls.Add(CreateButton("Atualizar", (sender, args) => ReloadLots()));
            line.Controls.Add(CreateButton("Buscar", (sender, args) => OpenLotLookup()));

            line.Controls.Add(CreateFieldLabel("Quantidade:"));
            _quantityTextBox = new TextBox { Width = 120, Font = new Font("Segoe UI", 10F) };
            line.Controls.Add(_quantityTextBox);

            line.Controls.Add(CreateButton("Adicionar", (sender, args) => AddOrUpdateItem()));
            line.Controls.Add(CreateButton("Editar", (sender, args) => StartEditingSelectedItem()));
            line.Controls.Add(CreateButton("Remover", (sender, args) => RemoveSelectedItem()));
            line.Controls.Add(CreateButton("Limpar Item", (sender, args) => ClearItemEditor()));

            _brcLabel = new Label { AutoSize = true, Text = "BRC: -", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.DimGray, Margin = new Padding(12, 10, 0, 0) };
            line.Controls.Add(_brcLabel);

            group.Controls.Add(line);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Itens Lancados", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _itemsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
            };
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "item", HeaderText = "ITEM", DataPropertyName = nameof(InboundReceiptItemRow.ItemNumber), Width = 60 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "material", HeaderText = "MATERIAL", DataPropertyName = nameof(InboundReceiptItemRow.MaterialDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "lote", HeaderText = "LOTE", DataPropertyName = nameof(InboundReceiptItemRow.LotDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "quantidade", HeaderText = "QUANTIDADE", DataPropertyName = nameof(InboundReceiptItemRow.QuantityText), Width = 120 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(InboundReceiptItemRow.Status), Width = 110 });
            _itemsGrid.CellDoubleClick += (sender, args) => StartEditingSelectedItem();
            group.Controls.Add(_itemsGrid);
            return group;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, AutoSize = true };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var actions = new FlowLayoutPanel { Dock = DockStyle.Left, AutoSize = true };
            _saveButton = CreateButton("Salvar (F2)", (sender, args) => SaveReceipt());
            _updateButton = CreateButton("Alterar (F3)", (sender, args) => UpdateReceipt());
            _cancelButton = CreateButton("Cancelar Nota (F6)", (sender, args) => CancelReceipt());
            actions.Controls.Add(_saveButton);
            actions.Controls.Add(_updateButton);
            actions.Controls.Add(CreateButton("Limpar (F5)", (sender, args) => ClearForm(confirm: true, releaseLock: true)));
            actions.Controls.Add(_cancelButton);

            var center = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            _itemCountLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93), Margin = new Padding(0, 8, 0, 0) };
            center.Controls.Add(_itemCountLabel);

            var right = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true };
            right.Controls.Add(CreateButton("Fechar (F4)", (sender, args) => CloseForm()));

            root.Controls.Add(actions, 0, 0);
            root.Controls.Add(center, 1, 0);
            root.Controls.Add(right, 2, 0);
            return root;
        }

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
