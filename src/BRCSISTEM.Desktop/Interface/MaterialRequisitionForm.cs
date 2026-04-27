using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class MaterialRequisitionForm : Form
    {
        private enum ScreenMode
        {
            Creation,
            Consultation
        }

        private readonly CompositionRoot _compositionRoot;
        private readonly MaterialRequisitionController _materialRequisitionController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private LookupOption[] _warehouseOptions;
        private LookupOption[] _materialOptions;
        private LookupOption[] _lotOptions;
        private RequisitionReasonSummary[] _reasonOptions;
        private readonly List<MaterialRequisitionItemDetail> _items;

        private TextBox _numberTextBox;
        private ComboBox _warehouseComboBox;
        private TextBox _movementDateTimeTextBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private TextBox _quantityTextBox;
        private ComboBox _reasonComboBox;
        private DataGridView _itemsGrid;
        private DataGridView _quickStockGrid;
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
        private string _loadedRequisitionStatus;

        public MaterialRequisitionForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _materialRequisitionController = compositionRoot.CreateMaterialRequisitionController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _warehouseOptions = new LookupOption[0];
            _materialOptions = new LookupOption[0];
            _lotOptions = new LookupOption[0];
            _reasonOptions = new RequisitionReasonSummary[0];
            _items = new List<MaterialRequisitionItemDetail>();
            _editingItemIndex = -1;

            InitializeComponent();
            Load += (sender, args) => LoadData();
            FormClosing += OnFormClosing;
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Requisicao de Materiais";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1380, 790);
            MinimumSize = new Size(1180, 700);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildHeaderPanel(), 0, 0);
            root.Controls.Add(BuildContentPanel(), 0, 1);
            root.Controls.Add(BuildFooterPanel(), 0, 2);
            Controls.Add(root);
        }

        private Control BuildHeaderPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, Height = 170, Text = "Dados da Requisicao", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var line1 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 8, AutoSize = true };
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));

            line1.Controls.Add(CreateFieldLabel("No Requisicao:"), 0, 0);
            _numberTextBox = new TextBox { Dock = DockStyle.Top, ReadOnly = true, Font = new Font("Segoe UI", 10F) };
            line1.Controls.Add(_numberTextBox, 1, 0);
            line1.Controls.Add(CreateButton("Buscar", (sender, args) => OpenRequisitionLookup()), 2, 0);
            line1.Controls.Add(CreateButton("Novo", (sender, args) => ClearForm(confirm: false, releaseLock: true, regenerateNumber: true)), 3, 0);
            line1.Controls.Add(CreateFieldLabel("Data/Hora:"), 4, 0);
            _movementDateTimeTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            _movementDateTimeTextBox.Leave += (sender, args) =>
            {
                _movementDateTimeTextBox.Text = NormalizeDateTimeInput(_movementDateTimeTextBox.Text);
                OnMovementDateChanged();
            };
            line1.Controls.Add(_movementDateTimeTextBox, 5, 0);
            line1.Controls.Add(new Panel { Dock = DockStyle.Fill }, 6, 0);
            line1.Controls.Add(CreateButton("Atual", (sender, args) =>
            {
                _movementDateTimeTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                OnMovementDateChanged();
            }), 7, 0);

            var line2 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 6, AutoSize = true };
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));

            line2.Controls.Add(CreateFieldLabel("Almoxarifado:"), 0, 0);
            _warehouseComboBox = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _warehouseComboBox.SelectedIndexChanged += (sender, args) => OnWarehouseSelectionChanged();
            line2.Controls.Add(_warehouseComboBox, 1, 0);
            line2.Controls.Add(CreateButton("Atu", (sender, args) => ReloadWarehouses()), 2, 0);
            line2.Controls.Add(CreateButton("Bus", (sender, args) => OpenWarehouseLookup()), 3, 0);
            line2.Controls.Add(CreateButton("Novo", (sender, args) => OpenWarehouseManagement()), 4, 0);
            line2.Controls.Add(CreateButton("Saldo", (sender, args) => RefreshQuickStockPanel()), 5, 0);

            _statusLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(0, 10, 0, 0) };
            _stockLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93), Margin = new Padding(0, 6, 0, 0) };

            root.Controls.Add(line1, 0, 0);
            root.Controls.Add(line2, 0, 1);
            root.Controls.Add(_statusLabel, 0, 2);
            root.Controls.Add(_stockLabel, 0, 3);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildContentPanel()
        {
            var content = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Margin = new Padding(0, 8, 0, 8) };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));

            var left = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            left.Controls.Add(BuildItemPanel(), 0, 0);
            left.Controls.Add(BuildGridPanel(), 0, 1);

            content.Controls.Add(left, 0, 0);
            content.Controls.Add(BuildQuickStockPanel(), 1, 0);
            return content;
        }

        private Control BuildItemPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, Height = 145, Text = "Adicionar / Editar Item", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var line = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), WrapContents = true, AutoScroll = true };

            line.Controls.Add(CreateFieldLabel("Material:"));
            _materialComboBox = new ComboBox { Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _materialComboBox.SelectedIndexChanged += (sender, args) => OnMaterialSelectionChanged();
            line.Controls.Add(_materialComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadMaterials()));
            line.Controls.Add(CreateButton("Busca", (sender, args) => OpenMaterialLookup()));
            line.Controls.Add(CreateButton("Emb", (sender, args) => OpenPackagingManagement()));

            line.Controls.Add(CreateFieldLabel("Lote:"));
            _lotComboBox = new ComboBox { Width = 240, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _lotComboBox.SelectedIndexChanged += (sender, args) => UpdateStockIndicator();
            line.Controls.Add(_lotComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadLots()));
            line.Controls.Add(CreateButton("Busca", (sender, args) => OpenLotLookup()));
            line.Controls.Add(CreateButton("Lote", (sender, args) => OpenLotManagement()));

            line.Controls.Add(CreateFieldLabel("Qtd Baixa:"));
            _quantityTextBox = new TextBox { Width = 110, Font = new Font("Segoe UI", 10F) };
            _quantityTextBox.TextChanged += (sender, args) => UpdateStockIndicator();
            line.Controls.Add(_quantityTextBox);

            line.Controls.Add(CreateFieldLabel("Motivo:"));
            _reasonComboBox = new ComboBox { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line.Controls.Add(_reasonComboBox);
            line.Controls.Add(CreateButton("Atu", (sender, args) => ReloadReasons()));

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
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Itens da Requisicao", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "item", HeaderText = "ITEM", DataPropertyName = nameof(MaterialRequisitionItemRow.ItemNumber), Width = 55 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "material", HeaderText = "MATERIAL", DataPropertyName = nameof(MaterialRequisitionItemRow.MaterialDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "lote", HeaderText = "LOTE", DataPropertyName = nameof(MaterialRequisitionItemRow.LotDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "qtdEnvio", HeaderText = "QTD ENVIO", DataPropertyName = nameof(MaterialRequisitionItemRow.QuantitySentText), Width = 110 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "qtdBaixa", HeaderText = "QTD BAIXA", DataPropertyName = nameof(MaterialRequisitionItemRow.QuantityLoweredText), Width = 110 });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(MaterialRequisitionItemRow.Status), Width = 110 });
            _itemsGrid.CellDoubleClick += (sender, args) => StartEditingSelectedItem();
            group.Controls.Add(_itemsGrid);
            return group;
        }

        private Control BuildQuickStockPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Saldo Rapido por Lote", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Margin = new Padding(12, 0, 0, 0) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(8) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var top = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            top.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Mostra ate 50 lotes com saldo no almoxarifado selecionado.",
                Margin = new Padding(0, 8, 12, 0),
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.DimGray,
            });
            top.Controls.Add(CreateButton("Atualizar", (sender, args) => RefreshQuickStockPanel()));

            _quickStockGrid = new DataGridView
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
            _quickStockGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "almox", HeaderText = "ALMOX", DataPropertyName = nameof(QuickStockRow.WarehouseCode), Width = 70 });
            _quickStockGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "material", HeaderText = "MATERIAL", DataPropertyName = nameof(QuickStockRow.MaterialDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _quickStockGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "lote", HeaderText = "LOTE", DataPropertyName = nameof(QuickStockRow.LotDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _quickStockGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "validade", HeaderText = "VALIDADE", DataPropertyName = nameof(QuickStockRow.ExpirationDate), Width = 95 });
            _quickStockGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "saldo", HeaderText = "SALDO", DataPropertyName = nameof(QuickStockRow.BalanceText), Width = 90 });
            _quickStockGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "lancado", HeaderText = "LANCADO", DataPropertyName = nameof(QuickStockRow.LaunchedText), Width = 95 });
            _quickStockGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "saldoFinal", HeaderText = "SALDO FINAL", DataPropertyName = nameof(QuickStockRow.FinalBalanceText), Width = 105 });
            _quickStockGrid.CellDoubleClick += (sender, args) => UseSelectedQuickStockItem();

            var footer = new Label
            {
                AutoSize = true,
                Text = "Dica: clique duplo em um lote para preencher material e lote no item.",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.DimGray,
            };

            root.Controls.Add(top, 0, 0);
            root.Controls.Add(_quickStockGrid, 0, 1);
            root.Controls.Add(footer, 0, 2);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, AutoSize = true };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var actions = new FlowLayoutPanel { Dock = DockStyle.Left, AutoSize = true };
            _saveButton = CreateButton("Salvar (F2)", (sender, args) => SaveRequisition());
            _updateButton = CreateButton("Alterar (F3)", (sender, args) => UpdateRequisition());
            _cancelButton = CreateButton("Cancelar (F6)", (sender, args) => CancelRequisition());
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
