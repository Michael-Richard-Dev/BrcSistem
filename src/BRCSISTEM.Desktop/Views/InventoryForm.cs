using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class InventoryForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly InventoryController _inventoryController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private InventoryPermissions _permissions;
        private LookupOption[] _warehouseOptions;
        private LookupOption[] _materialOptions;
        private LookupOption[] _lotOptions;
        private readonly List<InventoryItemDetail> _draftItems;
        private readonly List<InventoryPointSummary> _draftPoints;
        private readonly Dictionary<int, InventoryCountForm> _countWindows;

        private TextBox _numberTextBox;
        private TextBox _createdTextBox;
        private TextBox _scheduledTextBox;
        private NumericUpDown _maxPointsNumeric;
        private TextBox _observationTextBox;
        private ComboBox _warehouseComboBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private CheckBox _onlyBrcCheckBox;
        private Label _statusValueLabel;
        private Label _openedLabel;
        private Label _closedLabel;
        private Label _finalizedLabel;
        private Label _stockLabel;
        private Label _summaryLabel;
        private DataGridView _itemsGrid;
        private DataGridView _pointsGrid;
        private DataGridView _countsGrid;
        private Button _saveButton;
        private Button _updateButton;
        private Button _startButton;
        private Button _closeButton;
        private Button _reopenButton;
        private Button _finalizeButton;
        private Button _cancelButton;
        private Button _newPointButton;
        private Button _openPointButton;
        private Button _closePointButton;
        private Button _reopenPointButton;
        private Button _deletePointButton;
        private Button _zeroButton;

        private InventoryCountSummary[] _currentCounts;
        private bool _isRefreshingReferences;
        private bool _isPersisted;
        private bool _isReadOnly;
        private string _currentStatus;
        private string _lockedNumber;
        private int _temporaryPointSequence;

        public InventoryForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _inventoryController = compositionRoot.CreateInventoryController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _warehouseOptions = Array.Empty<LookupOption>();
            _materialOptions = Array.Empty<LookupOption>();
            _lotOptions = Array.Empty<LookupOption>();
            _draftItems = new List<InventoryItemDetail>();
            _draftPoints = new List<InventoryPointSummary>();
            _countWindows = new Dictionary<int, InventoryCountForm>();
            _currentCounts = Array.Empty<InventoryCountSummary>();
            _temporaryPointSequence = -1;

            InitializeComponent();
            Load += (sender, args) => LoadData();
            FormClosing += OnFormClosing;
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Inventario de Estoque";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1450, 860);
            MinimumSize = new Size(1240, 760);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildHeaderPanel(), 0, 0);
            root.Controls.Add(BuildPlanningPanel(), 0, 1);
            root.Controls.Add(BuildCenterPanel(), 0, 2);
            root.Controls.Add(BuildFooterPanel(), 0, 3);
            Controls.Add(root);
        }

        private Control BuildHeaderPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, Height = 185, Text = "Dados do Inventario", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var line1 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 10, AutoSize = true };
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            line1.Controls.Add(CreateFieldLabel("No Inventario:"), 0, 0);
            _numberTextBox = new TextBox { Dock = DockStyle.Top, ReadOnly = true, Font = new Font("Segoe UI", 10F) };
            line1.Controls.Add(_numberTextBox, 1, 0);
            line1.Controls.Add(CreateButton("Buscar", (sender, args) => OpenInventoryLookup()), 2, 0);
            line1.Controls.Add(CreateButton("Novo", (sender, args) => ClearForm(confirm: false, releaseLock: true, regenerateNumber: true)), 3, 0);
            line1.Controls.Add(CreateFieldLabel("Criacao:"), 4, 0);
            _createdTextBox = new TextBox { Dock = DockStyle.Top, ReadOnly = true, Font = new Font("Segoe UI", 10F) };
            line1.Controls.Add(_createdTextBox, 5, 0);
            line1.Controls.Add(CreateFieldLabel("Programado:"), 6, 0);
            _scheduledTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            _scheduledTextBox.Leave += (sender, args) => _scheduledTextBox.Text = NormalizeDateTimeInput(_scheduledTextBox.Text);
            line1.Controls.Add(_scheduledTextBox, 7, 0);
            line1.Controls.Add(CreateButton("Atual", (sender, args) => _createdTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm")), 8, 0);
            _statusValueLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(8, 8, 0, 0) };
            line1.Controls.Add(_statusValueLabel, 9, 0);

            var line2 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 8, AutoSize = true };
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            line2.Controls.Add(CreateFieldLabel("Abertura:"), 0, 0);
            _openedLabel = CreateInfoLabel();
            line2.Controls.Add(_openedLabel, 1, 0);
            line2.Controls.Add(CreateFieldLabel("Fechamento:"), 2, 0);
            _closedLabel = CreateInfoLabel();
            line2.Controls.Add(_closedLabel, 3, 0);
            line2.Controls.Add(CreateFieldLabel("Finalizacao:"), 4, 0);
            _finalizedLabel = CreateInfoLabel();
            line2.Controls.Add(_finalizedLabel, 5, 0);
            line2.Controls.Add(CreateFieldLabel("Max pontos:"), 6, 0);
            _maxPointsNumeric = new NumericUpDown { Minimum = 1, Maximum = 20, Value = 1, Width = 70, Font = new Font("Segoe UI", 10F) };
            line2.Controls.Add(_maxPointsNumeric, 7, 0);

            var line3 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 4, AutoSize = true };
            line3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            line3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            line3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));

            line3.Controls.Add(CreateFieldLabel("Observacao:"), 0, 0);
            _observationTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F), MaxLength = 40 };
            line3.Controls.Add(_observationTextBox, 1, 0);
            _summaryLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93), Margin = new Padding(12, 8, 0, 0) };
            line3.Controls.Add(_summaryLabel, 3, 0);

            _stockLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93) };

            root.Controls.Add(line1, 0, 0);
            root.Controls.Add(line2, 0, 1);
            root.Controls.Add(line3, 0, 2);
            root.Controls.Add(_stockLabel, 0, 3);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildPlanningPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, Height = 120, Text = "Planejamento de Itens", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var line = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), WrapContents = true, AutoScroll = true };

            line.Controls.Add(CreateFieldLabel("Almoxarifado:"));
            _warehouseComboBox = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _warehouseComboBox.SelectedIndexChanged += (sender, args) => OnWarehouseSelectionChanged();
            line.Controls.Add(_warehouseComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadWarehouses()));

            line.Controls.Add(CreateFieldLabel("Material:"));
            _materialComboBox = new ComboBox { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _materialComboBox.SelectedIndexChanged += (sender, args) => OnMaterialSelectionChanged();
            line.Controls.Add(_materialComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadMaterials()));

            line.Controls.Add(CreateFieldLabel("Lote:"));
            _lotComboBox = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _lotComboBox.SelectedIndexChanged += (sender, args) => UpdateStockIndicator();
            line.Controls.Add(_lotComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadLots()));

            _onlyBrcCheckBox = new CheckBox { AutoSize = true, Text = "Somente BRC", Font = new Font("Segoe UI", 9F), Margin = new Padding(18, 10, 0, 0) };
            _onlyBrcCheckBox.CheckedChanged += (sender, args) => ReloadMaterials();
            line.Controls.Add(_onlyBrcCheckBox);

            line.Controls.Add(CreateButton("Adicionar", (sender, args) => AddItem()));
            line.Controls.Add(CreateButton("Add Todos", (sender, args) => AddAllFromWarehouse()));
            line.Controls.Add(CreateButton("Remover", (sender, args) => RemoveSelectedItem()));
            line.Controls.Add(CreateButton("Limpar", (sender, args) => ClearItems()));

            group.Controls.Add(line);
            return group;
        }

        private Control BuildCenterPanel()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 780,
                BorderStyle = BorderStyle.FixedSingle,
            };

            var itemsGroup = new GroupBox { Dock = DockStyle.Fill, Text = "Itens do Inventario", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "almox", HeaderText = "ALMOX", DataPropertyName = nameof(InventoryItemDetail.WarehouseCode), Width = 80 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "material", HeaderText = "MATERIAL", DataPropertyName = nameof(InventoryItemDetail.MaterialDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "lote", HeaderText = "LOTE", DataPropertyName = nameof(InventoryItemDetail.LotDisplay), Width = 180 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "saldo", HeaderText = "SALDO", DataPropertyName = nameof(InventoryItemDetail.SystemBalanceText), Width = 95 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "entrada", HeaderText = "ENTRADA", DataPropertyName = nameof(InventoryItemDetail.InputQuantityText), Width = 95 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "saida", HeaderText = "SAIDA", DataPropertyName = nameof(InventoryItemDetail.OutputQuantityText), Width = 95 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "final", HeaderText = "SALDO FINAL", DataPropertyName = nameof(InventoryItemDetail.FinalBalanceText), Width = 110 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ajuste", HeaderText = "AJUSTE", DataPropertyName = nameof(InventoryItemDetail.AdjustmentQuantityText), Width = 95 });
            itemsGroup.Controls.Add(_itemsGrid);
            split.Panel1.Controls.Add(itemsGroup);

            var rightRoot = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3 };
            rightRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F));
            rightRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            rightRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var pointsGroup = new GroupBox { Dock = DockStyle.Fill, Text = "Pontos de Contagem", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _pointsGrid = new DataGridView
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
            _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = nameof(InventoryPointSummary.Id), Width = 60 });
            _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "nome", HeaderText = "PONTO", DataPropertyName = nameof(InventoryPointSummary.PointName), Width = 120 });
            _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ip", HeaderText = "IP", DataPropertyName = nameof(InventoryPointSummary.IpAddress), Width = 110 });
            _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "pc", HeaderText = "COMPUTADOR", DataPropertyName = nameof(InventoryPointSummary.ComputerName), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _pointsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(InventoryPointSummary.Status), Width = 90 });
            _pointsGrid.CellDoubleClick += (sender, args) => OpenSelectedPoint();
            pointsGroup.Controls.Add(_pointsGrid);

            var pointActions = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(0, 8, 0, 8), WrapContents = true };
            _newPointButton = CreateButton("Novo Ponto", (sender, args) => CreatePoint());
            _openPointButton = CreateButton("Abrir Ponto", (sender, args) => OpenSelectedPoint());
            _closePointButton = CreateButton("Fechar Ponto", (sender, args) => CloseSelectedPoint());
            _reopenPointButton = CreateButton("Reabrir Ponto", (sender, args) => ReopenSelectedPoint());
            _deletePointButton = CreateButton("Excluir Ponto", (sender, args) => DeleteSelectedPoint());
            _zeroButton = CreateButton("Lancar Zero", (sender, args) => ApplyZeroCounts());
            pointActions.Controls.Add(_newPointButton);
            pointActions.Controls.Add(_openPointButton);
            pointActions.Controls.Add(_closePointButton);
            pointActions.Controls.Add(_reopenPointButton);
            pointActions.Controls.Add(_deletePointButton);
            pointActions.Controls.Add(_zeroButton);

            var countsGroup = new GroupBox { Dock = DockStyle.Fill, Text = "Ultimas Leituras", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _countsGrid = new DataGridView
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
            _countsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = nameof(InventoryCountSummary.Id), Width = 60 });
            _countsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "data", HeaderText = "DATA/HORA", DataPropertyName = nameof(InventoryCountSummary.CountedAtDisplay), Width = 140 });
            _countsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ponto", HeaderText = "PONTO", DataPropertyName = nameof(InventoryCountSummary.PointId), Width = 70 });
            _countsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "item", HeaderText = "ITEM", DataPropertyName = nameof(InventoryCountSummary.ItemDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _countsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "qtd", HeaderText = "QTD", DataPropertyName = nameof(InventoryCountSummary.QuantityText), Width = 90 });
            _countsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "usuario", HeaderText = "USUARIO", DataPropertyName = nameof(InventoryCountSummary.UserName), Width = 90 });
            countsGroup.Controls.Add(_countsGrid);

            rightRoot.Controls.Add(pointsGroup, 0, 0);
            rightRoot.Controls.Add(pointActions, 0, 1);
            rightRoot.Controls.Add(countsGroup, 0, 2);
            split.Panel2.Controls.Add(rightRoot);

            return split;
        }

        private Control BuildFooterPanel()
        {
            var footer = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 8, 0, 0), AutoSize = true };
            _cancelButton = CreateButton("Cancelar", (sender, args) => CancelInventory());
            _finalizeButton = CreateButton("Encerrar", (sender, args) => FinalizeInventory());
            _reopenButton = CreateButton("Reabrir", (sender, args) => ReopenInventory());
            _closeButton = CreateButton("Fechar", (sender, args) => CloseInventory());
            _startButton = CreateButton("Iniciar", (sender, args) => StartInventory());
            _updateButton = CreateButton("Alterar", (sender, args) => UpdateInventory());
            _saveButton = CreateButton("Salvar", (sender, args) => SaveInventory());

            footer.Controls.Add(_cancelButton);
            footer.Controls.Add(_finalizeButton);
            footer.Controls.Add(_reopenButton);
            footer.Controls.Add(_closeButton);
            footer.Controls.Add(_startButton);
            footer.Controls.Add(_updateButton);
            footer.Controls.Add(_saveButton);
            return footer;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label { AutoSize = true, Text = text, Margin = new Padding(0, 8, 0, 0), Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        }

        private static Label CreateInfoLabel()
        {
            return new Label { AutoSize = true, Font = new Font("Segoe UI", 9F), Margin = new Padding(0, 8, 0, 0) };
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
        }
    }
}
