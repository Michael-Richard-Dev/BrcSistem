using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    /// <summary>
    /// Porte fiel de views/bd_alterar_data_entrada.py (classe AlterarDataEntrada).
    /// Permite escolher uma nota de entrada ATIVO, exibir os dados em um grid
    /// "Campo/Valor" e alterar a data/hora atualizando notas (dt_movimento +
    /// dt_emissao) e movimentos_estoque (data_movimento) em uma unica transacao.
    /// </summary>
    public sealed partial class NoteDateChangeForm : Form
    {
        private readonly DatabaseMaintenanceController _databaseMaintenanceController;
        private readonly ConfigurationController       _configurationController;
        private readonly UserIdentity    _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private DocumentDateEntry[] _notes;

        private ComboBox     _noteComboBox;
        private TextBox      _newDateTextBox;
        private DataGridView _detailsGrid;
        private Label        _statusLabel;

        public NoteDateChangeForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _databaseMaintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
            _configurationController       = compositionRoot.CreateConfigurationController();
            _identity        = identity;
            _databaseProfile = databaseProfile;
            _notes           = Array.Empty<DocumentDateEntry>();

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text          = "BRCSISTEM - Alterar Data de Entrada";
            StartPosition = FormStartPosition.CenterParent;
            Size          = new Size(960, 600);
            MinimumSize   = new Size(780, 460);
            BackColor     = Color.White;
            KeyPreview    = true;
            KeyDown      += OnFormKeyDown;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var titleLabel = new Label
            {
                AutoSize  = true,
                Text      = "Alterar Data de Entrada de Estoque",
                Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin    = new Padding(0, 0, 0, 10),
            };

            root.Controls.Add(titleLabel,           0, 0);
            root.Controls.Add(BuildSelectionPanel(), 0, 1);
            root.Controls.Add(BuildDetailsPanel(),   0, 2);
            root.Controls.Add(BuildFooterPanel(),    0, 3);
            Controls.Add(root);
        }

        private Control BuildSelectionPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, AutoSize = true, Text = "Selecionar Nota de Entrada", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root  = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 2, AutoSize = true };

            var line1 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line1.Controls.Add(CreateFieldLabel("Nota de Entrada:"));
            _noteComboBox = new ComboBox
            {
                Width         = 420,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10F),
            };
            _noteComboBox.SelectedIndexChanged += (sender, args) => UpdateDetails();
            line1.Controls.Add(_noteComboBox);
            line1.Controls.Add(CreateButton("Atualizar", (sender, args) => LoadNotes()));

            var line2 = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            line2.Controls.Add(CreateFieldLabel("Nova Data/Hora:"));
            _newDateTextBox = new TextBox { Width = 220, Font = new Font("Segoe UI", 10F) };
            line2.Controls.Add(_newDateTextBox);
            line2.Controls.Add(new Label
            {
                AutoSize  = true,
                Text      = "(DD/MM/YYYY HH:MM)",
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                Margin    = new Padding(0, 8, 14, 4),
            });
            line2.Controls.Add(CreateButton("Alterar Data", (sender, args) => ChangeDate()));
            line2.Controls.Add(CreateButton("Limpar",       (sender, args) => ClearForm()));
            line2.Controls.Add(CreateButton("Fechar",       (sender, args) => Close()));

            root.Controls.Add(line1, 0, 0);
            root.Controls.Add(line2, 0, 1);
            group.Controls.Add(root);
            return group;
        }

        private Control BuildDetailsPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Dados da Nota de Entrada Selecionada", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _detailsGrid = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                ReadOnly              = true,
                AutoGenerateColumns   = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                MultiSelect           = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible     = false,
                BackgroundColor       = Color.White,
            };

            var fieldColumn = new DataGridViewTextBoxColumn
            {
                Name             = "field",
                HeaderText       = "Campo",
                DataPropertyName = "Field",
                Width            = 220,
                SortMode         = DataGridViewColumnSortMode.NotSortable,
            };
            var valueColumn = new DataGridViewTextBoxColumn
            {
                Name             = "value",
                HeaderText       = "Valor",
                DataPropertyName = "Value",
                AutoSizeMode     = DataGridViewAutoSizeColumnMode.Fill,
                SortMode         = DataGridViewColumnSortMode.NotSortable,
            };

            _detailsGrid.Columns.Add(fieldColumn);
            _detailsGrid.Columns.Add(valueColumn);
            group.Controls.Add(_detailsGrid);
            return group;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, AutoSize = true };
            _statusLabel = new Label
            {
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
                Margin    = new Padding(0, 6, 0, 0),
            };
            root.Controls.Add(_statusLabel, 0, 0);
            return root;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize  = true,
                Text      = text,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin    = new Padding(0, 8, 0, 4),
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
            _statusLabel.Text      = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }
    }
}
