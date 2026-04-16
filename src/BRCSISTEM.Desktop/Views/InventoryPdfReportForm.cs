using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class InventoryPdfReportForm : Form
    {
        private readonly InventoryReportController _inventoryReportController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private InventoryReportEntry[] _inventoryOptions;
        private InventoryReportDocument _currentDocument;

        private ComboBox _inventoryComboBox;
        private DataGridView _itemsGrid;
        private DataGridView _movementsGrid;
        private Label _statusSummaryLabel;
        private Label _creatorSummaryLabel;
        private Label _datesSummaryLabel;
        private Label _totalsSummaryLabel;
        private Label _observationSummaryLabel;
        private Label _statusLabel;

        public InventoryPdfReportForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _inventoryReportController = compositionRoot.CreateInventoryReportController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _inventoryOptions = Array.Empty<InventoryReportEntry>();

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Relatorio de Inventario (PDF)";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1460, 860);
            MinimumSize = new Size(1220, 760);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildFilterPanel(), 0, 0);
            root.Controls.Add(BuildSummaryPanel(), 0, 1);
            root.Controls.Add(BuildCenterPanel(), 0, 2);
            root.Controls.Add(BuildFooterPanel(), 0, 3);
            Controls.Add(root);
        }

        private Control BuildFilterPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, AutoSize = true, Text = "Filtros", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 2, AutoSize = true };

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = "Relatorio de inventario com foco em divergencias entre saldo do sistema, contagem e movimentos de ajuste.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.DimGray,
                Margin = new Padding(0, 0, 0, 8),
            };

            var line = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line.Controls.Add(CreateFieldLabel("Inventario:"));
            _inventoryComboBox = new ComboBox
            {
                Width = 360,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
            };
            line.Controls.Add(_inventoryComboBox);
            line.Controls.Add(CreateButton("Atualizar", (sender, args) => LoadInventories()));
            line.Controls.Add(CreateButton("Consultar", (sender, args) => QueryDocument()));
            line.Controls.Add(CreateButton("Gerar PDF", (sender, args) => ExportPdf()));
            line.Controls.Add(CreateButton("Gerar CSV", (sender, args) => ExportCsv()));
            line.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            root.Controls.Add(titleLabel, 0, 0);
            root.Controls.Add(line, 0, 1);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildSummaryPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, AutoSize = true, Text = "Resumo do Inventario", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 3, AutoSize = true };

            var line1 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            _statusSummaryLabel = CreateSummaryLabel("Status: -");
            _creatorSummaryLabel = CreateSummaryLabel("Criador: -");
            _datesSummaryLabel = CreateSummaryLabel("Abertura/Fechamento: -");
            line1.Controls.Add(_statusSummaryLabel);
            line1.Controls.Add(_creatorSummaryLabel);
            line1.Controls.Add(_datesSummaryLabel);

            _totalsSummaryLabel = CreateSummaryLabel("Itens: 0 | Divergentes: 0 | Entradas: 0,00 | Saidas: 0,00");
            _observationSummaryLabel = CreateSummaryLabel("Observacao: -");
            _observationSummaryLabel.MaximumSize = new Size(1380, 0);

            root.Controls.Add(line1, 0, 0);
            root.Controls.Add(_totalsSummaryLabel, 0, 1);
            root.Controls.Add(_observationSummaryLabel, 0, 2);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildCenterPanel()
        {
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 380,
                BorderStyle = BorderStyle.FixedSingle,
            };

            var itemsGroup = new GroupBox { Dock = DockStyle.Fill, Text = "Itens e Divergencias", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            AddGridColumn(_itemsGrid, "almox", "ALMOX", nameof(InventoryReportItem.WarehouseCode), 80, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_itemsGrid, "material", "MATERIAL", nameof(InventoryReportItem.MaterialDisplay), 280, DataGridViewContentAlignment.MiddleLeft, true);
            AddGridColumn(_itemsGrid, "lote", "LOTE", nameof(InventoryReportItem.LotDisplay), 200, DataGridViewContentAlignment.MiddleLeft);
            AddGridColumn(_itemsGrid, "sistema", "SALDO SISTEMA", nameof(InventoryReportItem.SystemBalanceText), 110, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn(_itemsGrid, "contado", "QTD CONTADA", nameof(InventoryReportItem.CountedQuantityText), 110, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn(_itemsGrid, "divergencia", "DIVERGENCIA", nameof(InventoryReportItem.AdjustmentQuantityText), 110, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn(_itemsGrid, "tipo", "TIPO AJUSTE", nameof(InventoryReportItem.AdjustmentType), 110, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_itemsGrid, "validade", "VALIDADE", nameof(InventoryReportItem.ExpirationDateDisplay), 110, DataGridViewContentAlignment.MiddleCenter);
            _itemsGrid.RowPrePaint += OnItemsGridRowPrePaint;
            itemsGroup.Controls.Add(_itemsGrid);

            var movementsGroup = new GroupBox { Dock = DockStyle.Fill, Text = "Movimentos de Ajuste Lancados", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _movementsGrid = new DataGridView
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
            AddGridColumn(_movementsGrid, "data", "DATA/HORA", nameof(InventoryReportMovement.MovementDateDisplay), 130, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_movementsGrid, "item", "ITEM", nameof(InventoryReportMovement.ItemNumberText), 55, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_movementsGrid, "tipo", "TIPO", nameof(InventoryReportMovement.Type), 110, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_movementsGrid, "almox", "ALMOX", nameof(InventoryReportMovement.WarehouseCode), 75, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_movementsGrid, "material", "MATERIAL", nameof(InventoryReportMovement.MaterialCode), 110, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_movementsGrid, "lote", "LOTE", nameof(InventoryReportMovement.LotCode), 110, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_movementsGrid, "quantidade", "QUANTIDADE", nameof(InventoryReportMovement.QuantityText), 110, DataGridViewContentAlignment.MiddleRight);
            AddGridColumn(_movementsGrid, "fornecedor", "FORNECEDOR", nameof(InventoryReportMovement.SupplierCode), 120, DataGridViewContentAlignment.MiddleCenter);
            AddGridColumn(_movementsGrid, "usuario", "USUARIO", nameof(InventoryReportMovement.UserName), 140, DataGridViewContentAlignment.MiddleCenter, true);
            AddGridColumn(_movementsGrid, "status", "STATUS", nameof(InventoryReportMovement.Status), 110, DataGridViewContentAlignment.MiddleCenter);
            _movementsGrid.RowPrePaint += OnMovementsGridRowPrePaint;
            movementsGroup.Controls.Add(_movementsGrid);

            split.Panel1.Controls.Add(itemsGroup);
            split.Panel2.Controls.Add(movementsGroup);
            return split;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
                Margin = new Padding(0, 4, 0, 4),
            };
            root.Controls.Add(_statusLabel, 0, 0);
            return root;
        }

        private static void AddGridColumn(DataGridView grid, string name, string headerText, string dataPropertyName, int width, DataGridViewContentAlignment alignment, bool fill = false)
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
            grid.Columns.Add(column);
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

        private static Label CreateSummaryLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 4, 18, 4),
            };
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
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
