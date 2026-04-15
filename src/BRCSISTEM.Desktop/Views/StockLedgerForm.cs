using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class StockLedgerForm : Form
    {
        private readonly StockLedgerController _stockLedgerController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly StockLedgerQuery _initialQuery;

        private AppConfiguration _configuration;
        private LookupOption[] _supplierOptions;
        private LookupOption[] _materialOptions;
        private LookupOption[] _lotOptions;
        private LookupOption[] _warehouseOptions;
        private StockLedgerEntry[] _entries;

        private TextBox _startDateTextBox;
        private TextBox _endDateTextBox;
        private ComboBox _supplierComboBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private ComboBox _warehouseComboBox;
        private ComboBox _typeComboBox;
        private CheckBox _includeInactiveCheckBox;
        private DataGridView _grid;
        private Label _summaryLabel;
        private Label _statusLabel;

        private bool _isRefreshingFilters;
        private string _sortColumn;
        private bool _sortAscending;

        public StockLedgerForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
            : this(compositionRoot, identity, databaseProfile, null)
        {
        }

        public StockLedgerForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile, StockLedgerQuery initialQuery)
        {
            _stockLedgerController = compositionRoot.CreateStockLedgerController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _initialQuery = initialQuery;
            _supplierOptions = new LookupOption[0];
            _materialOptions = new LookupOption[0];
            _lotOptions = new LookupOption[0];
            _warehouseOptions = new LookupOption[0];
            _entries = new StockLedgerEntry[0];
            _sortColumn = "data";
            _sortAscending = false;

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Conta Corrente de Estoque";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1560, 860);
            MinimumSize = new Size(1260, 720);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildFilterPanel(), 0, 0);
            root.Controls.Add(BuildGridPanel(), 0, 1);
            root.Controls.Add(BuildFooterPanel(), 0, 2);
            Controls.Add(root);
        }

        private Control BuildFilterPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, AutoSize = true, Text = "Filtros de Consulta", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 5, AutoSize = true };

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = "Extrato analitico das movimentacoes de estoque com saldo acumulado por filtro.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                Margin = new Padding(0, 0, 0, 8),
            };

            var line1 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line1.Controls.Add(CreateFieldLabel("Data Inicial:"));
            _startDateTextBox = new TextBox { Width = 100, Font = new Font("Segoe UI", 10F) };
            _startDateTextBox.Leave += (sender, args) => _startDateTextBox.Text = NormalizeDateInput(_startDateTextBox.Text);
            line1.Controls.Add(_startDateTextBox);
            line1.Controls.Add(CreateFieldLabel("Data Final:"));
            _endDateTextBox = new TextBox { Width = 100, Font = new Font("Segoe UI", 10F) };
            _endDateTextBox.Leave += (sender, args) => _endDateTextBox.Text = NormalizeDateInput(_endDateTextBox.Text);
            line1.Controls.Add(_endDateTextBox);
            line1.Controls.Add(CreateButton("Hoje", (sender, args) => ApplyQuickPeriod(0)));
            line1.Controls.Add(CreateButton("7 dias", (sender, args) => ApplyQuickPeriod(7)));
            line1.Controls.Add(CreateButton("30 dias", (sender, args) => ApplyQuickPeriod(30)));
            line1.Controls.Add(CreateButton("90 dias", (sender, args) => ApplyQuickPeriod(90)));
            line1.Controls.Add(CreateButton("Tudo", (sender, args) => ApplyQuickPeriod(null)));

            var line2 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line2.Controls.Add(CreateFieldLabel("Fornecedor:"));
            _supplierComboBox = new ComboBox { Width = 310, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _supplierComboBox.SelectedIndexChanged += (sender, args) => OnSupplierChanged();
            line2.Controls.Add(_supplierComboBox);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenSupplierLookup()));
            line2.Controls.Add(CreateFieldLabel("Material:"));
            _materialComboBox = new ComboBox { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _materialComboBox.SelectedIndexChanged += (sender, args) => OnMaterialChanged();
            line2.Controls.Add(_materialComboBox);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenMaterialLookup()));

            var line3 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line3.Controls.Add(CreateFieldLabel("Lote:"));
            _lotComboBox = new ComboBox { Width = 270, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line3.Controls.Add(_lotComboBox);
            line3.Controls.Add(CreateButton("Busca", (sender, args) => OpenLotLookup()));
            line3.Controls.Add(CreateFieldLabel("Almoxarifado:"));
            _warehouseComboBox = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line3.Controls.Add(_warehouseComboBox);
            line3.Controls.Add(CreateButton("Busca", (sender, args) => OpenWarehouseLookup()));

            var line4 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line4.Controls.Add(CreateFieldLabel("Tipo:"));
            _typeComboBox = new ComboBox { Width = 210, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _typeComboBox.Items.AddRange(new object[] { "TODOS", "INVENTARIO", "ENTRADA", "TRANSFERENCIA_ENTRADA", "TRANSFERENCIA_SAIDA", "REQUISICAO", "SAIDA_PRODUCAO", "SAIDA" });
            _typeComboBox.SelectedIndex = 0;
            line4.Controls.Add(_typeComboBox);

            _includeInactiveCheckBox = new CheckBox
            {
                AutoSize = true,
                Text = "Mostrar Inativos",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(18, 8, 0, 0),
            };
            line4.Controls.Add(_includeInactiveCheckBox);
            line4.Controls.Add(CreateButton("Limpar Filtros", (sender, args) => ClearFilters()));
            line4.Controls.Add(CreateButton("Filtrar", (sender, args) => QueryEntries()));
            line4.Controls.Add(CreateButton("Gerar PDF", (sender, args) => ExportPdf()));
            line4.Controls.Add(CreateButton("Exportar (Ctrl+P)", (sender, args) => ExportCsv()));
            line4.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            root.Controls.Add(titleLabel, 0, 0);
            root.Controls.Add(line1, 0, 1);
            root.Controls.Add(line2, 0, 2);
            root.Controls.Add(line3, 0, 3);
            root.Controls.Add(line4, 0, 4);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Movimentacoes", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _grid = new DataGridView
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
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
            };
            AddGridColumn("data", "DATA/HORA", nameof(StockLedgerEntry.MovementDateTimeDisplay), 125);
            AddGridColumn("documento", "DOCUMENTO", nameof(StockLedgerEntry.DocumentDisplay), 130);
            AddGridColumn("tipo", "TIPO", nameof(StockLedgerEntry.DisplayType), 135);
            AddGridColumn("material", "MATERIAL", nameof(StockLedgerEntry.MaterialDisplay), 260, true);
            AddGridColumn("lote", "LOTE", nameof(StockLedgerEntry.LotDisplay), 220, true);
            AddGridColumn("validade", "VALIDADE", nameof(StockLedgerEntry.ExpirationDateDisplay), 95);
            AddGridColumn("almox", "ALMOXARIFADO", nameof(StockLedgerEntry.WarehouseDisplay), 180, true);
            AddGridColumn("fornecedor", "FORNECEDOR", nameof(StockLedgerEntry.SupplierDisplay), 200, true);
            AddGridColumn("quantidade", "QUANTIDADE", nameof(StockLedgerEntry.QuantityText), 105);
            AddGridColumn("saldo", "SALDO ACUM.", nameof(StockLedgerEntry.RunningBalanceText), 110);
            AddGridColumn("criacao", "DT. CRIACAO", nameof(StockLedgerEntry.CreatedAtDisplay), 125);
            AddGridColumn("usuarioCriacao", "USUARIO CRIACAO", nameof(StockLedgerEntry.UserName), 120);
            AddGridColumn("usuario", "USUARIO", nameof(StockLedgerEntry.UserName), 120);
            AddGridColumn("status", "STATUS", nameof(StockLedgerEntry.Status), 90);
            _grid.ColumnHeaderMouseClick += OnGridColumnHeaderMouseClick;
            _grid.RowPrePaint += OnGridRowPrePaint;
            group.Controls.Add(_grid);
            return group;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            _summaryLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 4, 0, 4),
            };
            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
                Margin = new Padding(0, 0, 0, 0),
            };
            root.Controls.Add(_summaryLabel, 0, 0);
            root.Controls.Add(_statusLabel, 0, 1);
            return root;
        }

        private void AddGridColumn(string name, string headerText, string dataPropertyName, int width, bool fill = false)
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = dataPropertyName,
                Width = width,
                SortMode = DataGridViewColumnSortMode.Programmatic,
                AutoSizeMode = fill ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None,
            });
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 8, 0, 4),
            };
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
    }
}
