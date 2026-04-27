using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class ProductionOutputPdfReportForm : Form
    {
        private readonly ProductionOutputReportController _productionOutputReportController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private LookupOption[] _warehouseOptions;
        private LookupOption[] _productOptions;
        private ProductionOutputReportEntry[] _rows;

        private TextBox _startDateTextBox;
        private TextBox _endDateTextBox;
        private TextBox _outputNumberTextBox;
        private ComboBox _warehouseComboBox;
        private ComboBox _productComboBox;
        private ComboBox _shiftComboBox;
        private CheckBox _excludeCanceledCheckBox;
        private DataGridView _grid;
        private Label _summaryLabel;
        private Label _statusLabel;

        public ProductionOutputPdfReportForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _productionOutputReportController = compositionRoot.CreateProductionOutputReportController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _warehouseOptions = new LookupOption[0];
            _productOptions = new LookupOption[0];
            _rows = new ProductionOutputReportEntry[0];

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Relatorio de Saida de Producao";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1460, 820);
            MinimumSize = new Size(1180, 700);
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
            var group = new GroupBox { Dock = DockStyle.Top, AutoSize = true, Text = "Filtros", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 3, AutoSize = true };

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = "Relatorio analitico das saidas de producao, com detalhamento por item, CSV e PDF para conferencia.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                Margin = new Padding(0, 0, 0, 8),
            };

            var line1 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line1.Controls.Add(CreateFieldLabel("Data Inicio:"));
            _startDateTextBox = new TextBox { Width = 110, Font = new Font("Segoe UI", 10F) };
            _startDateTextBox.Leave += (sender, args) => _startDateTextBox.Text = NormalizeDateInput(_startDateTextBox.Text);
            line1.Controls.Add(_startDateTextBox);
            line1.Controls.Add(CreateFieldLabel("Data Fim:"));
            _endDateTextBox = new TextBox { Width = 110, Font = new Font("Segoe UI", 10F) };
            _endDateTextBox.Leave += (sender, args) => _endDateTextBox.Text = NormalizeDateInput(_endDateTextBox.Text);
            line1.Controls.Add(_endDateTextBox);
            line1.Controls.Add(CreateFieldLabel("Saida:"));
            _outputNumberTextBox = new TextBox { Width = 150, Font = new Font("Segoe UI", 10F), CharacterCasing = CharacterCasing.Upper };
            _outputNumberTextBox.TextChanged += (sender, args) => NormalizeOutputNumberInput();
            _outputNumberTextBox.KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                {
                    args.Handled = true;
                    args.SuppressKeyPress = true;
                    QueryEntries();
                }
            };
            line1.Controls.Add(_outputNumberTextBox);
            line1.Controls.Add(CreateFieldLabel("Turno:"));
            _shiftComboBox = new ComboBox { Width = 130, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _shiftComboBox.Items.AddRange(new object[] { "TODOS", "1o TURNO", "2o TURNO" });
            _shiftComboBox.SelectedIndex = 0;
            line1.Controls.Add(_shiftComboBox);

            var line2 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line2.Controls.Add(CreateFieldLabel("Almoxarifado:"));
            _warehouseComboBox = new ComboBox { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line2.Controls.Add(_warehouseComboBox);
            line2.Controls.Add(CreateButton("Buscar", (sender, args) => OpenWarehouseLookup()));
            line2.Controls.Add(CreateFieldLabel("Produto:"));
            _productComboBox = new ComboBox { Width = 340, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            line2.Controls.Add(_productComboBox);
            line2.Controls.Add(CreateButton("Buscar", (sender, args) => OpenProductLookup()));
            _excludeCanceledCheckBox = new CheckBox
            {
                AutoSize = true,
                Text = "Excluir Canceladas",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(14, 8, 0, 0),
            };
            line2.Controls.Add(_excludeCanceledCheckBox);
            line2.Controls.Add(CreateButton("Filtrar", (sender, args) => QueryEntries()));
            line2.Controls.Add(CreateButton("Limpar Filtros", (sender, args) => ClearFilters()));
            line2.Controls.Add(CreateButton("Gerar PDF", (sender, args) => ExportPdf()));
            line2.Controls.Add(CreateButton("Gerar CSV", (sender, args) => ExportCsv()));
            line2.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            root.Controls.Add(titleLabel, 0, 0);
            root.Controls.Add(line1, 0, 1);
            root.Controls.Add(line2, 0, 2);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Itens de Saida de Producao", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            };
            AddGridColumn("saida", "SAIDA", nameof(ProductionOutputReportEntry.Number), 110, DataGridViewContentAlignment.MiddleLeft);
            AddGridColumn("data", "DATA SAIDA", nameof(ProductionOutputReportEntry.MovementDateDisplay), 110, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn("almoxarifado", "ALMOXARIFADO", nameof(ProductionOutputReportEntry.WarehouseDisplay), 180, DataGridViewContentAlignment.MiddleLeft, true);
            AddGridColumn("produto", "PRODUTO", nameof(ProductionOutputReportEntry.ProductDisplay), 200, DataGridViewContentAlignment.MiddleLeft, true);
            AddGridColumn("material", "MATERIAL", nameof(ProductionOutputReportEntry.MaterialDisplay), 230, DataGridViewContentAlignment.MiddleLeft, true);
            AddGridColumn("lote", "LOTE", nameof(ProductionOutputReportEntry.LotDisplay), 170, DataGridViewContentAlignment.MiddleLeft, true);
            AddGridColumn("envio", "QTD ENVIO", nameof(ProductionOutputReportEntry.QuantitySentText), 90, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("retorno", "QTD RETORNO", nameof(ProductionOutputReportEntry.QuantityReturnedText), 95, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("consumida", "QTD CONSUMIDA", nameof(ProductionOutputReportEntry.QuantityConsumedText), 105, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn("turno", "TURNO", nameof(ProductionOutputReportEntry.Shift), 90, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn("status", "STATUS", nameof(ProductionOutputReportEntry.Status), 110, DataGridViewContentAlignment.MiddleCenter);
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
}
