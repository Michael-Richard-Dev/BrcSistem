using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class StockMovementReportForm : Form
    {
        private readonly StockMovementReportController _stockMovementReportController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly Font _totalRowFont;

        private AppConfiguration _configuration;
        private LookupOption[] _warehouseOptions;
        private LookupOption[] _materialOptions;
        private LookupOption[] _lotOptions;
        private StockMovementReportRow[] _rows;

        private TextBox _startDateTextBox;
        private TextBox _endDateTextBox;
        private ComboBox _warehouseComboBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private CheckBox _onlyMovementOrBalanceCheckBox;
        private DataGridView _grid;
        private Label _summaryLabel;
        private Label _statusLabel;

        private string _sortColumn;
        private bool _sortAscending;

        public StockMovementReportForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _stockMovementReportController = compositionRoot.CreateStockMovementReportController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _totalRowFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            _warehouseOptions = new LookupOption[0];
            _materialOptions = new LookupOption[0];
            _lotOptions = new LookupOption[0];
            _rows = new StockMovementReportRow[0];
            _sortColumn = string.Empty;
            _sortAscending = true;

            InitializeComponent();
            Load += (sender, args) => LoadData();
            FormClosed += (sender, args) => _totalRowFont.Dispose();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Relatorio de Movimentacao de Estoque";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1280, 720);
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
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 4, AutoSize = true };

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = "Resumo analitico por almoxarifado, lote e material com saldo inicial, movimentacoes e saldo final.",
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

            var line2 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line2.Controls.Add(CreateFieldLabel("Almoxarifado:"));
            _warehouseComboBox = new ComboBox { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line2.Controls.Add(_warehouseComboBox);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenWarehouseLookup()));
            line2.Controls.Add(CreateFieldLabel("Material:"));
            _materialComboBox = new ComboBox { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line2.Controls.Add(_materialComboBox);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenMaterialLookup()));
            line2.Controls.Add(CreateFieldLabel("Lote:"));
            _lotComboBox = new ComboBox { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line2.Controls.Add(_lotComboBox);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenLotLookup()));

            var line3 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            _onlyMovementOrBalanceCheckBox = new CheckBox
            {
                AutoSize = true,
                Text = "Somente com movimentacao ou saldo > 0 (inicial/final)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 8, 14, 0),
            };
            line3.Controls.Add(_onlyMovementOrBalanceCheckBox);
            line3.Controls.Add(CreateButton("Filtrar", (sender, args) => QueryRows()));
            line3.Controls.Add(CreateButton("Limpar Filtros", (sender, args) => ClearFilters()));
            line3.Controls.Add(CreateButton("Gerar PDF", (sender, args) => ExportPdf()));
            line3.Controls.Add(CreateButton("Gerar CSV", (sender, args) => ExportCsv()));
            line3.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            root.Controls.Add(titleLabel, 0, 0);
            root.Controls.Add(line1, 0, 1);
            root.Controls.Add(line2, 0, 2);
            root.Controls.Add(line3, 0, 3);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Movimentacoes de Estoque", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            AddGridColumn("almoxarifado", "ALMOX", nameof(StockMovementReportRow.WarehouseDisplay), 70, DataGridViewContentAlignment.MiddleLeft);
            AddGridColumn("lote", "LOTE", nameof(StockMovementReportRow.LotDisplay), 230, DataGridViewContentAlignment.MiddleLeft);
            AddGridColumn("material", "MATERIAL", nameof(StockMovementReportRow.MaterialDisplay), 320, DataGridViewContentAlignment.MiddleLeft, true);
            AddGridColumn("validade", "VALIDADE", nameof(StockMovementReportRow.ExpirationDateDisplay), 95, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn("saldoInicial", "SALDO INICIAL", nameof(StockMovementReportRow.OpeningBalanceText), 110, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("entradas", "ENTRADA (+)", nameof(StockMovementReportRow.EntriesText), 95, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("transfEntrada", "TRANSF (+)", nameof(StockMovementReportRow.TransferInText), 95, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("transfSaida", "TRANSF (-)", nameof(StockMovementReportRow.TransferOutText), 95, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("saidaProducao", "SAIDA PROD (-)", nameof(StockMovementReportRow.ProductionOutputText), 110, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("requisicao", "REQUISICAO (-)", nameof(StockMovementReportRow.RequisitionText), 105, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("inventario", "INVENTARIO (+/-)", nameof(StockMovementReportRow.InventoryText), 115, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("saldoFinal", "SALDO FINAL (=)", nameof(StockMovementReportRow.FinalBalanceText), 110, DataGridViewContentAlignment.MiddleRight);
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
            };
            root.Controls.Add(_summaryLabel, 0, 0);
            root.Controls.Add(_statusLabel, 0, 1);
            return root;
        }

        private void AddGridColumn(string name, string headerText, string dataPropertyName, int width, DataGridViewContentAlignment alignment, bool fill = false)
        {
            var column = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = dataPropertyName,
                Width = width,
                SortMode = DataGridViewColumnSortMode.Programmatic,
                AutoSizeMode = fill ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None,
            };
            column.DefaultCellStyle.Alignment = alignment;
            _grid.Columns.Add(column);
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
