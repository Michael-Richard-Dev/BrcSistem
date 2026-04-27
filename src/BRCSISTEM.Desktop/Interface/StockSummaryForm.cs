using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class StockSummaryForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly StockSummaryController _stockSummaryController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly Font _warehouseRowFont;
        private readonly Font _materialRowFont;
        private readonly Timer _filterRefreshTimer;

        private AppConfiguration _configuration;
        private LookupOption[] _warehouseOptions;
        private LookupOption[] _materialOptions;
        private LookupOption[] _lotOptions;
        private StockSummaryEntry[] _entries;
        private StockSummaryDisplayRow[] _displayRows;

        private TextBox _referenceDateTextBox;
        private ComboBox _warehouseComboBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private DataGridView _grid;
        private Label _summaryLabel;
        private Label _infoLabel;
        private Label _statusLabel;

        private bool _isRefreshingFilters;

        public StockSummaryForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _stockSummaryController = compositionRoot.CreateStockSummaryController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _warehouseRowFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            _materialRowFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            _filterRefreshTimer = new Timer { Interval = 350 };
            _filterRefreshTimer.Tick += (sender, args) =>
            {
                _filterRefreshTimer.Stop();
                QueryEntries();
            };
            _warehouseOptions = new LookupOption[0];
            _materialOptions = new LookupOption[0];
            _lotOptions = new LookupOption[0];
            _entries = new StockSummaryEntry[0];
            _displayRows = new StockSummaryDisplayRow[0];

            InitializeComponent();
            Load += (sender, args) => LoadData();
            FormClosed += (sender, args) =>
            {
                _filterRefreshTimer.Dispose();
                _warehouseRowFont.Dispose();
                _materialRowFont.Dispose();
            };
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Resumo Sintetico de Estoque";
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1180, 720);
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
            var group = new GroupBox { Dock = DockStyle.Top, AutoSize = true, Text = "Filtros e Acoes", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 3, AutoSize = true };

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = "Posicao de estoque consolidada por almoxarifado, material e lote ate a data de referencia.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                Margin = new Padding(0, 0, 0, 8),
            };

            var line1 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line1.Controls.Add(CreateFieldLabel("Data de Referencia:"));
            _referenceDateTextBox = new TextBox { Width = 110, Font = new Font("Segoe UI", 10F) };
            _referenceDateTextBox.Leave += (sender, args) =>
            {
                _referenceDateTextBox.Text = NormalizeDateInput(_referenceDateTextBox.Text);
                ScheduleRefresh();
            };
            _referenceDateTextBox.KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                {
                    args.Handled = true;
                    args.SuppressKeyPress = true;
                    QueryEntries();
                }
            };
            line1.Controls.Add(_referenceDateTextBox);
            line1.Controls.Add(CreateButton("Hoje", (sender, args) => ApplyQuickDate(0)));
            line1.Controls.Add(CreateButton("Ontem", (sender, args) => ApplyQuickDate(1)));
            line1.Controls.Add(CreateButton("7 dias", (sender, args) => ApplyQuickDate(7)));
            line1.Controls.Add(CreateButton("30 dias", (sender, args) => ApplyQuickDate(30)));
            line1.Controls.Add(CreateButton("Atualizar (F5)", (sender, args) => QueryEntries()));
            line1.Controls.Add(CreateButton("Gerar PDF", (sender, args) => ExportPdf()));
            line1.Controls.Add(CreateButton("Exportar CSV", (sender, args) => ExportCsv()));

            var line2 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line2.Controls.Add(CreateFieldLabel("Almoxarifado:"));
            _warehouseComboBox = new ComboBox { Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _warehouseComboBox.SelectedIndexChanged += (sender, args) => ScheduleRefresh();
            line2.Controls.Add(_warehouseComboBox);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenWarehouseLookup()));
            line2.Controls.Add(CreateFieldLabel("Material:"));
            _materialComboBox = new ComboBox { Width = 330, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _materialComboBox.SelectedIndexChanged += (sender, args) => ScheduleRefresh();
            line2.Controls.Add(_materialComboBox);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenMaterialLookup()));
            line2.Controls.Add(CreateFieldLabel("Lote:"));
            _lotComboBox = new ComboBox { Width = 260, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _lotComboBox.SelectedIndexChanged += (sender, args) => ScheduleRefresh();
            line2.Controls.Add(_lotComboBox);
            line2.Controls.Add(CreateButton("Busca", (sender, args) => OpenLotLookup()));
            line2.Controls.Add(CreateButton("Limpar", (sender, args) => ClearFilters()));
            line2.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            root.Controls.Add(titleLabel, 0, 0);
            root.Controls.Add(line1, 0, 1);
            root.Controls.Add(line2, 0, 2);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Posicao de Estoque", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(8), RowCount = 2 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var summaryPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
            summaryPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            summaryPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            _summaryLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(10, 77, 140),
                Margin = new Padding(0, 0, 0, 8),
            };
            _infoLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(0, 2, 0, 8),
            };
            summaryPanel.Controls.Add(_summaryLabel, 0, 0);
            summaryPanel.Controls.Add(_infoLabel, 1, 0);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
            };
            AddGridColumn("item", "ITEM HIERARQUICO", nameof(StockSummaryDisplayRow.HierarchyText), 640, DataGridViewContentAlignment.MiddleLeft, true);
            AddGridColumn("quantidade", "QUANTIDADE", nameof(StockSummaryDisplayRow.QuantityText), 120, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("validade", "DATA VALIDADE", nameof(StockSummaryDisplayRow.ExpirationDateDisplay), 120, DataGridViewContentAlignment.MiddleCenter);
            _grid.RowPrePaint += OnGridRowPrePaint;
            _grid.CellDoubleClick += OnGridCellDoubleClick;

            root.Controls.Add(summaryPanel, 0, 0);
            root.Controls.Add(_grid, 0, 1);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
            };
            root.Controls.Add(_statusLabel, 0, 0);
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
                SortMode = DataGridViewColumnSortMode.NotSortable,
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

    internal enum StockSummaryRowKind
    {
        WarehouseHeader,
        MaterialHeader,
        LotItem,
    }

    internal sealed class StockSummaryDisplayRow
    {
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

        public string HierarchyText { get; set; }

        public string ExpirationDateDisplay { get; set; }

        public decimal Quantity { get; set; }

        public StockSummaryRowKind RowKind { get; set; }

        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string LotCode { get; set; }

        public string QuantityText
        {
            get { return Quantity.ToString("N2", PtBr); }
        }
    }
}
