using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class ProductionOutputForm : Form
    {
        private enum ScreenMode
        {
            Creation,
            Consultation
        }

        private readonly CompositionRoot _compositionRoot;
        private readonly ProductionOutputController _productionOutputController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private LookupOption[] _productOptions;
        private LookupOption[] _warehouseOptions;
        private LookupOption[] _materialOptions;
        private LookupOption[] _lotOptions;
        private readonly List<ProductionOutputItemDetail> _items;

        private TextBox _numberTextBox;
        private TextBox _purposeTextBox;
        private ComboBox _warehouseComboBox;
        private TextBox _movementDateTimeTextBox;
        private ComboBox _shiftComboBox;
        private ComboBox _productComboBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private TextBox _quantitySentTextBox;
        private TextBox _quantityReturnedTextBox;
        private TextBox _quantityConsumedTextBox;
        private DataGridView _itemsGrid;
        private Button _saveButton;
        private Button _updateButton;
        private Button _cancelButton;
        private Button _itemApplyButton;
        private Label _statusLabel;
        private Label _itemCountLabel;
        private Label _stockLabel;

        private ScreenMode _mode;
        private int _editingItemIndex;
        private bool _isRefreshingReferences;
        private string _lockedNumber;
        private string _loadedOutputStatus;

        public ProductionOutputForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _productionOutputController = compositionRoot.CreateProductionOutputController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _productOptions = new LookupOption[0];
            _warehouseOptions = new LookupOption[0];
            _materialOptions = new LookupOption[0];
            _lotOptions = new LookupOption[0];
            _items = new List<ProductionOutputItemDetail>();
            _editingItemIndex = -1;

            InitializeComponent();
            Load += (sender, args) => LoadData();
            FormClosing += OnFormClosing;
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Saida de Producao";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1320, 780);
            MinimumSize = new Size(1180, 700);
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
            var group = new GroupBox { Dock = DockStyle.Top, Height = 190, Text = "Dados da Saida", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var line1 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 10, AutoSize = true };
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));

            line1.Controls.Add(CreateFieldLabel("No Saida:"), 0, 0);
            _numberTextBox = new TextBox { Dock = DockStyle.Top, ReadOnly = true, Font = new Font("Segoe UI", 10F) };
            line1.Controls.Add(_numberTextBox, 1, 0);
            line1.Controls.Add(CreateButton("Buscar", (sender, args) => OpenOutputLookup()), 2, 0);

            line1.Controls.Add(CreateFieldLabel("Finalidade:"), 3, 0);
            _purposeTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            line1.Controls.Add(_purposeTextBox, 4, 0);
            line1.Controls.Add(CreateButton("Novo", (sender, args) => ClearForm(confirm: false, releaseLock: true, regenerateNumber: true)), 5, 0);
            line1.Controls.Add(CreateButton("Atual", (sender, args) => _purposeTextBox.Focus()), 6, 0);
            line1.Controls.Add(CreateButton("Prod", (sender, args) => OpenProductManagement()), 7, 0);
            line1.Controls.Add(CreateButton("Emb", (sender, args) => OpenPackagingManagement()), 8, 0);
            line1.Controls.Add(CreateButton("Lote", (sender, args) => OpenLotManagement()), 9, 0);

            var line2 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 11, AutoSize = true };
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));

            line2.Controls.Add(CreateFieldLabel("Almoxarifado:"), 0, 0);
            _warehouseComboBox = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _warehouseComboBox.SelectedIndexChanged += (sender, args) => OnWarehouseSelectionChanged();
            line2.Controls.Add(_warehouseComboBox, 1, 0);
            line2.Controls.Add(CreateButton("Atu", (sender, args) => ReloadWarehouses()), 2, 0);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenWarehouseLookup()), 3, 0);
            line2.Controls.Add(CreateButton("Novo", (sender, args) => OpenWarehouseManagement()), 4, 0);

            line2.Controls.Add(CreateFieldLabel("Data/Hora Saida:"), 5, 0);
            _movementDateTimeTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            _movementDateTimeTextBox.Leave += (sender, args) =>
            {
                _movementDateTimeTextBox.Text = NormalizeDateTimeInput(_movementDateTimeTextBox.Text);
                OnMovementDateChanged();
            };
            line2.Controls.Add(_movementDateTimeTextBox, 6, 0);

            line2.Controls.Add(CreateFieldLabel("Turno:"), 7, 0);
            _shiftComboBox = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _shiftComboBox.Items.AddRange(new object[] { "1o TURNO", "2o TURNO" });
            line2.Controls.Add(_shiftComboBox, 8, 0);

            _statusLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(0, 10, 0, 0) };
            _stockLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93), Margin = new Padding(0, 10, 0, 0) };

            root.Controls.Add(line1, 0, 0);
            root.Controls.Add(line2, 0, 1);
            root.Controls.Add(_statusLabel, 0, 2);
            root.Controls.Add(_stockLabel, 0, 3);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildItemPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, Height = 150, Text = "Adicionar / Editar Item", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var line = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), WrapContents = true, AutoScroll = true };

            line.Controls.Add(CreateFieldLabel("Produto:"));
            _productComboBox = new ComboBox { Width = 240, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line.Controls.Add(_productComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadProducts()));
            line.Controls.Add(CreateButton("Busca", (sender, args) => OpenProductLookup()));

            line.Controls.Add(CreateFieldLabel("Material:"));
            _materialComboBox = new ComboBox { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _materialComboBox.SelectedIndexChanged += (sender, args) => OnMaterialSelectionChanged();
            line.Controls.Add(_materialComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadMaterials()));
            line.Controls.Add(CreateButton("Busca", (sender, args) => OpenMaterialLookup()));

            line.Controls.Add(CreateFieldLabel("Lote:"));
            _lotComboBox = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _lotComboBox.SelectedIndexChanged += (sender, args) => UpdateStockIndicator();
            line.Controls.Add(_lotComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadLots()));
            line.Controls.Add(CreateButton("Busca", (sender, args) => OpenLotLookup()));

            line.Controls.Add(CreateFieldLabel("Qtd Envio:"));
            _quantitySentTextBox = new TextBox { Width = 100, Font = new Font("Segoe UI", 10F) };
            _quantitySentTextBox.TextChanged += (sender, args) => OnQuantityInputsChanged();
            line.Controls.Add(_quantitySentTextBox);

            line.Controls.Add(CreateFieldLabel("Qtd Retorno:"));
            _quantityReturnedTextBox = new TextBox { Width = 100, Font = new Font("Segoe UI", 10F), Text = "0,00" };
            _quantityReturnedTextBox.TextChanged += (sender, args) => OnQuantityInputsChanged();
            line.Controls.Add(_quantityReturnedTextBox);

            line.Controls.Add(CreateFieldLabel("Qtd Consumida:"));
            _quantityConsumedTextBox = new TextBox { Width = 110, Font = new Font("Segoe UI", 10F), ReadOnly = true };
            line.Controls.Add(_quantityConsumedTextBox);

            _itemApplyButton = CreateButton("Adicionar", (sender, args) => AddOrUpdateItem());
            line.Controls.Add(_itemApplyButton);
            line.Controls.Add(CreateButton("Editar", (sender, args) => StartEditingSelectedItem()));
            line.Controls.Add(CreateButton("Remover", (sender, args) => RemoveSelectedItem()));
            line.Controls.Add(CreateButton("Limpar Item", (sender, args) => ClearItemEditor()));

            group.Controls.Add(line);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Itens da Saida", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "item", HeaderText = "ITEM", DataPropertyName = nameof(ProductionOutputItemRow.ItemNumber), Width = 55 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "produto", HeaderText = "PRODUTO", DataPropertyName = nameof(ProductionOutputItemRow.ProductDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "material", HeaderText = "MATERIAL", DataPropertyName = nameof(ProductionOutputItemRow.MaterialDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "lote", HeaderText = "LOTE", DataPropertyName = nameof(ProductionOutputItemRow.LotDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "envio", HeaderText = "ENVIO", DataPropertyName = nameof(ProductionOutputItemRow.QuantitySentText), Width = 95 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "retorno", HeaderText = "RETORNO", DataPropertyName = nameof(ProductionOutputItemRow.QuantityReturnedText), Width = 95 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "consumo", HeaderText = "CONSUMO", DataPropertyName = nameof(ProductionOutputItemRow.QuantityConsumedText), Width = 100 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(ProductionOutputItemRow.Status), Width = 110 });
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
            _saveButton = CreateButton("Salvar (F2)", (sender, args) => SaveOutput());
            _updateButton = CreateButton("Alterar (F3)", (sender, args) => UpdateOutput());
            _cancelButton = CreateButton("Cancelar Baixa (F6)", (sender, args) => CancelOutput());
            actions.Controls.Add(_saveButton);
            actions.Controls.Add(_updateButton);
            actions.Controls.Add(CreateButton("Limpar (F5)", (sender, args) => ClearForm(confirm: true, releaseLock: true, regenerateNumber: true)));
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
    }
}
