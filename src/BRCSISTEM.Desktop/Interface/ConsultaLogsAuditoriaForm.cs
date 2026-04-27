using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class ConsultaLogsAuditoriaForm : Form
    {
        private readonly DatabaseMaintenanceController _databaseMaintenanceController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private ComboBox _periodComboBox;
        private ComboBox _userComboBox;
        private ComboBox _actionComboBox;
        private TextBox _searchTextBox;
        private DataGridView _logsGrid;
        private Label _infoLabel;
        private Label _pageLabel;
        private Button _previousButton;
        private Button _nextButton;
        private Label _statusLabel;

        private int _currentPage;
        private int _pageSize;
        private int _totalRecords;
        private AuditLogEntry[] _currentEntries;

        public ConsultaLogsAuditoriaForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _databaseMaintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _currentPage = 1;
            _pageSize = 50;
            _currentEntries = Array.Empty<AuditLogEntry>();

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Consulta de Logs";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1180, 700);
            MinimumSize = new Size(920, 560);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 5 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = "Consulta de Logs e Auditoria",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 0, 0, 10),
            };

            root.Controls.Add(titleLabel, 0, 0);
            root.Controls.Add(BuildFiltersPanel(), 0, 1);
            root.Controls.Add(BuildResultsPanel(), 0, 2);
            root.Controls.Add(BuildPagingPanel(), 0, 3);
            root.Controls.Add(BuildFooterPanel(), 0, 4);
            Controls.Add(root);
        }

        private Control BuildFiltersPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, AutoSize = true, Text = "Filtros de Pesquisa", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 2, AutoSize = true };

            var line1 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line1.Controls.Add(CreateFieldLabel("Periodo:"));
            _periodComboBox = new ComboBox
            {
                Width = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
            };
            _periodComboBox.Items.AddRange(new object[] { "Hoje", "Ultimos 7 dias", "Ultimos 30 dias", "Ultimos 90 dias", "Todos" });
            _periodComboBox.SelectedIndexChanged += (sender, args) => SearchFromFirstPage();
            line1.Controls.Add(_periodComboBox);

            line1.Controls.Add(CreateFieldLabel("Usuario:"));
            _userComboBox = new ComboBox
            {
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
            };
            _userComboBox.SelectedIndexChanged += (sender, args) => SearchFromFirstPage();
            line1.Controls.Add(_userComboBox);

            line1.Controls.Add(CreateFieldLabel("Acao:"));
            _actionComboBox = new ComboBox
            {
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
            };
            _actionComboBox.SelectedIndexChanged += (sender, args) => SearchFromFirstPage();
            line1.Controls.Add(_actionComboBox);

            var line2 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line2.Controls.Add(CreateFieldLabel("Pesquisar:"));
            _searchTextBox = new TextBox { Width = 480, Font = new Font("Segoe UI", 10F) };
            _searchTextBox.KeyDown += OnSearchTextBoxKeyDown;
            line2.Controls.Add(_searchTextBox);
            line2.Controls.Add(CreateButton("Pesquisar", (sender, args) => SearchFromFirstPage()));
            line2.Controls.Add(CreateButton("Limpar", (sender, args) => ClearFilters()));
            line2.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            root.Controls.Add(line1, 0, 0);
            root.Controls.Add(line2, 0, 1);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildResultsPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Resultados", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _logsGrid = new DataGridView
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
            _logsGrid.CellDoubleClick += OnGridCellDoubleClick;

            _logsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Width = 70,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _logsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "dateTime",
                HeaderText = "Data/Hora",
                DataPropertyName = "DateTime",
                Width = 150,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _logsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "userName",
                HeaderText = "Usuario",
                DataPropertyName = "UserName",
                Width = 180,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _logsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "action",
                HeaderText = "Acao",
                DataPropertyName = "Action",
                Width = 220,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _logsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "detailsSummary",
                HeaderText = "Detalhes",
                DataPropertyName = "DetailsSummary",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });

            group.Controls.Add(_logsGrid);
            return group;
        }

        private Control BuildPagingPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var infoPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            _infoLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F), ForeColor = Color.DimGray, Margin = new Padding(0, 8, 0, 0) };
            _pageLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93), Margin = new Padding(18, 8, 0, 0) };
            infoPanel.Controls.Add(_infoLabel);
            infoPanel.Controls.Add(_pageLabel);

            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            _previousButton = CreateButton("Anterior", (sender, args) => GoToPreviousPage());
            _nextButton = CreateButton("Proxima", (sender, args) => GoToNextPage());
            buttonPanel.Controls.Add(_previousButton);
            buttonPanel.Controls.Add(_nextButton);

            root.Controls.Add(infoPanel, 0, 0);
            root.Controls.Add(buttonPanel, 1, 0);
            return root;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
                Margin = new Padding(0, 6, 0, 0),
            };
            root.Controls.Add(_statusLabel, 0, 0);
            return root;
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

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }

        private void ShowError(Exception exception)
        {
            SetStatus(exception.Message, true);
            MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
