using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class InboundReceiptReactivationForm : Form
    {
        private readonly DatabaseMaintenanceController _databaseMaintenanceController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private InboundReceiptReactivationEntry[] _entries;

        private TextBox _numberTextBox;
        private TextBox _supplierTextBox;
        private DataGridView _grid;
        private Label _statusLabel;

        public InboundReceiptReactivationForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _databaseMaintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _entries = Array.Empty<InboundReceiptReactivationEntry>();

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Reativar Nota de Entrada";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(800, 500);
            MinimumSize = new Size(800, 500);
            MaximumSize = new Size(800, 500);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                RowCount = 4,
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildHeader(), 0, 0);
            root.Controls.Add(BuildSearchPanel(), 0, 1);
            root.Controls.Add(BuildGridPanel(), 0, 2);
            root.Controls.Add(BuildFooter(), 0, 3);
            Controls.Add(root);
        }

        private Control BuildHeader()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = true,
                Margin = new Padding(0, 0, 0, 10),
            };

            panel.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Reativar Nota de Entrada",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 4, 10, 4),
            });
            panel.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Voltar notas canceladas para status ATIVO",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(102, 102, 102),
                Margin = new Padding(0, 8, 0, 4),
            });

            return panel;
        }

        private Control BuildSearchPanel()
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Text = "Buscar Notas Canceladas",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoSize = true,
                WrapContents = true,
            };

            panel.Controls.Add(CreateFieldLabel("Numero:"));
            _numberTextBox = new TextBox { Width = 110, Font = new Font("Segoe UI", 10F) };
            _numberTextBox.KeyDown += OnSearchKeyDown;
            panel.Controls.Add(_numberTextBox);

            panel.Controls.Add(CreateFieldLabel("Fornecedor:"));
            _supplierTextBox = new TextBox { Width = 110, Font = new Font("Segoe UI", 10F) };
            _supplierTextBox.KeyDown += OnSearchKeyDown;
            panel.Controls.Add(_supplierTextBox);

            panel.Controls.Add(CreateButton("Pesquisar", (sender, args) => SearchCancelledReceipts()));
            panel.Controls.Add(CreateButton("Limpar", (sender, args) => ClearFilters()));
            panel.Controls.Add(CreateButton("Todos Cancelados", (sender, args) => LoadAllCancelledReceipts()));

            group.Controls.Add(panel);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Notas Canceladas",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(8),
            };

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
            };

            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Numero",
                DataPropertyName = nameof(InboundReceiptReactivationEntry.Number),
                Width = 90,
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Fornecedor",
                DataPropertyName = nameof(InboundReceiptReactivationEntry.Supplier),
                Width = 110,
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Almoxarifado",
                DataPropertyName = nameof(InboundReceiptReactivationEntry.Warehouse),
                Width = 130,
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Versao",
                DataPropertyName = nameof(InboundReceiptReactivationEntry.Version),
                Width = 70,
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Data Emissao",
                DataPropertyName = nameof(InboundReceiptReactivationEntry.EmissionDate),
                Width = 120,
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = nameof(InboundReceiptReactivationEntry.Status),
                Width = 90,
            });

            _grid.CellDoubleClick += (sender, args) =>
            {
                if (args.RowIndex >= 0)
                {
                    ReactivateSelectedReceipt();
                }
            };

            group.Controls.Add(_grid);
            return group;
        }

        private Control BuildFooter()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0),
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
                Margin = new Padding(0, 8, 0, 0),
            };

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                WrapContents = false,
            };
            buttons.Controls.Add(CreateButton("Reativar Selecionada", (sender, args) => ReactivateSelectedReceipt()));
            buttons.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            panel.Controls.Add(_statusLabel, 0, 0);
            panel.Controls.Add(buttons, 1, 0);
            return panel;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 8, 6, 4),
            };
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                FlatStyle = FlatStyle.System,
                Margin = new Padding(6, 3, 0, 3),
            };
            button.Click += handler;
            return button;
        }

        private void SetStatus(string message, bool isError)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = isError ? Color.Firebrick : Color.SeaGreen;
        }

        private void ShowError(string title, Exception exception)
        {
            SetStatus(exception.Message, true);
            MessageBox.Show(this, title + ":\n" + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
