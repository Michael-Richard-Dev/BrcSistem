using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class RemoveNoteForm : Form
    {
        private readonly DatabaseMaintenanceController _databaseMaintenanceController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private DocumentMaintenanceHeader _noteHeader;

        private TextBox _numberTextBox;
        private TextBox _supplierTextBox;
        private DataGridView _headerGrid;
        private DataGridView _itemsGrid;
        private Button _removeButton;

        public RemoveNoteForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _databaseMaintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            Load += (sender, args) => _configuration = _configurationController.LoadConfiguration();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Remover Nota de Entrada";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(950, 650);
            MinimumSize = new Size(900, 620);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                RowCount = 4,
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Remover Nota de Entrada",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 10),
            }, 0, 0);

            root.Controls.Add(BuildSearchArea(), 0, 1);
            root.Controls.Add(BuildGridArea(), 0, 2);
            root.Controls.Add(BuildButtons(), 0, 3);

            Controls.Add(root);
        }

        private Control BuildSearchArea()
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Top,
                Text = "Localizar Nota",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(10),
                AutoSize = true,
            };

            var row = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            row.Controls.Add(CreateFieldLabel("Numero:"));
            _numberTextBox = new TextBox { Width = 120, Font = new Font("Segoe UI", 10F) };
            row.Controls.Add(_numberTextBox);
            row.Controls.Add(CreateFieldLabel("Fornecedor:"));
            _supplierTextBox = new TextBox { Width = 220, Font = new Font("Segoe UI", 10F) };
            row.Controls.Add(_supplierTextBox);
            row.Controls.Add(CreateButton("Buscar", (sender, args) => SearchNote()));

            group.Controls.Add(row);
            return group;
        }

        private Control BuildGridArea()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2 };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 42F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 58F));

            var headerGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Dados da Nota",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(6),
            };
            _headerGrid = new DataGridView
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
            _headerGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "CAMPO", DataPropertyName = "Field", Width = 220, SortMode = DataGridViewColumnSortMode.NotSortable });
            _headerGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "VALOR", DataPropertyName = "Value", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, SortMode = DataGridViewColumnSortMode.NotSortable });
            headerGroup.Controls.Add(_headerGrid);

            var itemsGroup = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Itens da Nota",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(6),
            };
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
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "MATERIAL", DataPropertyName = nameof(DocumentMaintenanceItem.Material), Width = 180, SortMode = DataGridViewColumnSortMode.NotSortable });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "LOTE", DataPropertyName = nameof(DocumentMaintenanceItem.Lot), Width = 180, SortMode = DataGridViewColumnSortMode.NotSortable });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ALMOX", DataPropertyName = nameof(DocumentMaintenanceItem.Warehouse), Width = 160, SortMode = DataGridViewColumnSortMode.NotSortable });
            _itemsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "QUANTIDADE", DataPropertyName = nameof(DocumentMaintenanceItem.Quantity), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, SortMode = DataGridViewColumnSortMode.NotSortable });
            itemsGroup.Controls.Add(_itemsGrid);

            root.Controls.Add(headerGroup, 0, 0);
            root.Controls.Add(itemsGroup, 0, 1);
            return root;
        }

        private Control BuildButtons()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0),
            };

            _removeButton = CreateButton("Remover Nota", (sender, args) => ConfirmRemoval());
            _removeButton.Enabled = false;
            panel.Controls.Add(_removeButton);
            panel.Controls.Add(CreateButton("Limpar", (sender, args) => ClearForm()));

            var close = CreateButton("Fechar", (sender, args) => Close());
            close.Margin = new Padding(20, 3, 0, 3);
            panel.Controls.Add(close);

            return panel;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Margin = new Padding(0, 8, 6, 4),
            };
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(5, 3, 0, 3) };
            button.Click += handler;
            return button;
        }

        private sealed class DetailRow
        {
            public string Field { get; set; }
            public string Value { get; set; }
        }
    }
}
