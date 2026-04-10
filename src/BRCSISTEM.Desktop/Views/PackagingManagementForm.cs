using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class PackagingManagementForm : Form
    {
        private readonly MasterDataController _masterDataController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private PackagingSummary[] _allPackagings;
        private DataGridView _grid;
        private TextBox _codeTextBox;
        private TextBox _descriptionTextBox;
        private ComboBox _statusComboBox;
        private CheckBox _brcCheckBox;
        private CheckBox _hideInactiveCheckBox;
        private Label _totalLabel;
        private Label _activeLabel;
        private Label _inactiveLabel;
        private Label _brcLabel;
        private Label _statusLabel;
        private string _selectedCode;
        private string _sortColumn = "codigo";
        private bool _sortAscending = true;
        private bool _hasChanges;

        public PackagingManagementForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _masterDataController = compositionRoot.CreateMasterDataController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _allPackagings = new PackagingSummary[0];

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Cadastro de Embalagens";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1020, 650);
            MinimumSize = new Size(940, 610);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.Controls.Add(BuildFormPanel(), 0, 0);
            root.Controls.Add(BuildGridPanel(), 0, 1);
            root.Controls.Add(BuildFooterPanel(), 0, 2);
            Controls.Add(root);
        }

        private Control BuildFormPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Top, Height = 145, Text = "Dados da Embalagem", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 4 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            layout.Controls.Add(CreateFieldLabel("Codigo:"), 0, 0);
            _codeTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            layout.Controls.Add(_codeTextBox, 1, 0);

            layout.Controls.Add(CreateFieldLabel("Descricao:"), 2, 0);
            _descriptionTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            layout.Controls.Add(_descriptionTextBox, 3, 0);

            layout.Controls.Add(CreateFieldLabel("Status:"), 0, 1);
            _statusComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Width = 160 };
            _statusComboBox.Items.AddRange(new object[] { "ATIVO", "INATIVO" });
            layout.Controls.Add(_statusComboBox, 1, 1);

            _brcCheckBox = new CheckBox { AutoSize = true, Text = "Habilitado para BRC", Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 6, 0, 0) };
            layout.Controls.Add(_brcCheckBox, 3, 1);

            _statusLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(0, 8, 0, 0) };
            layout.Controls.Add(_statusLabel, 3, 2);

            group.Controls.Add(layout);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Registros (ultima versao por codigo)", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "codigo", HeaderText = "CODIGO", DataPropertyName = nameof(PackagingSummary.Code), Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "descricao", HeaderText = "DESCRICAO", DataPropertyName = nameof(PackagingSummary.Description), Width = 280, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "brc", HeaderText = "BRC", DataPropertyName = nameof(PackagingSummary.IsBrcEnabled), Width = 80 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(PackagingSummary.Status), Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "versao", HeaderText = "VERSAO", DataPropertyName = nameof(PackagingSummary.Version), Width = 90 });
            _grid.SelectionChanged += (sender, args) => PopulateSelectedPackaging();
            _grid.ColumnHeaderMouseClick += OnGridColumnHeaderMouseClick;
            _grid.CellFormatting += OnGridCellFormatting;
            _grid.RowPrePaint += OnGridRowPrePaint;
            group.Controls.Add(_grid);
            return group;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, AutoSize = true };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var actions = new FlowLayoutPanel { Dock = DockStyle.Left, AutoSize = true };
            actions.Controls.Add(CreateButton("Adicionar (F2)", (sender, args) => CreatePackaging()));
            actions.Controls.Add(CreateButton("Alterar (F3)", (sender, args) => UpdatePackaging()));
            actions.Controls.Add(CreateButton("Inativar (F6)", (sender, args) => InactivatePackaging()));
            actions.Controls.Add(CreateButton("Limpar (F5)", (sender, args) => ResetForm()));

            var stats = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            stats.Controls.Add(CreateStatCaption("Total:", Color.FromArgb(52, 73, 94), 18));
            _totalLabel = CreateStatValue(Color.FromArgb(52, 73, 94));
            stats.Controls.Add(_totalLabel);
            stats.Controls.Add(CreateStatCaption("Ativos:", Color.SeaGreen, 0));
            _activeLabel = CreateStatValue(Color.SeaGreen);
            stats.Controls.Add(_activeLabel);
            stats.Controls.Add(CreateStatCaption("Inativos:", Color.Firebrick, 0));
            _inactiveLabel = CreateStatValue(Color.Firebrick);
            stats.Controls.Add(_inactiveLabel);
            stats.Controls.Add(CreateStatCaption("BRC:", Color.DarkGoldenrod, 0));
            _brcLabel = CreateStatValue(Color.DarkGoldenrod);
            stats.Controls.Add(_brcLabel);
            stats.Controls.Add(new Label { AutoSize = true, Text = "Com estoque", ForeColor = Color.FromArgb(85, 139, 47), Font = new Font("Segoe UI", 8F), Margin = new Padding(16, 8, 8, 0), BackColor = Color.FromArgb(232, 245, 233) });
            stats.Controls.Add(new Label { AutoSize = true, Text = "Cinza = Inativo", ForeColor = Color.Gray, Font = new Font("Segoe UI", 8F), Margin = new Padding(0, 8, 8, 0) });
            _hideInactiveCheckBox = new CheckBox { AutoSize = true, Text = "Ocultar inativos", Checked = true, Font = new Font("Segoe UI", 9F), Margin = new Padding(4, 6, 0, 0) };
            _hideInactiveCheckBox.CheckedChanged += (sender, args) => RefreshGrid();
            stats.Controls.Add(_hideInactiveCheckBox);

            var right = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true };
            right.Controls.Add(CreateButton("Fechar (F4)", (sender, args) => CloseForm()));

            root.Controls.Add(actions, 0, 0);
            root.Controls.Add(stats, 1, 0);
            root.Controls.Add(right, 2, 0);
            return root;
        }

        private void LoadData()
        {
            _configuration = _configurationController.LoadConfiguration();
            LoadPackagings();
        }

        private void LoadPackagings()
        {
            _allPackagings = _masterDataController.LoadPackagings(_configuration, _databaseProfile);
            UpdateStatistics();
            RefreshGrid();
            ResetForm(false);
        }

        private void RefreshGrid()
        {
            var filtered = _allPackagings.Where(item => !_hideInactiveCheckBox.Checked || !string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase)).ToArray();
            _grid.DataSource = SortPackagings(filtered);
        }

        private void PopulateSelectedPackaging()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is PackagingSummary packaging))
            {
                return;
            }

            _selectedCode = packaging.Code;
            _codeTextBox.Text = packaging.Code ?? string.Empty;
            _codeTextBox.ReadOnly = true;
            _descriptionTextBox.Text = packaging.Description ?? string.Empty;
            _statusComboBox.SelectedItem = packaging.Status;
            _brcCheckBox.Checked = packaging.IsBrcEnabled;
            SetStatus("Embalagem carregada.", false);
        }

        private void CreatePackaging()
        {
            try
            {
                _masterDataController.CreatePackaging(_configuration, _databaseProfile, BuildRequest());
                _hasChanges = true;
                LoadPackagings();
                ResetForm();
                SetStatus("Embalagem adicionada com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdatePackaging()
        {
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                SetStatus("Selecione uma embalagem.", true);
                return;
            }

            try
            {
                var request = BuildRequest();
                request.Code = _selectedCode;
                _masterDataController.UpdatePackaging(_configuration, _databaseProfile, request);
                _hasChanges = true;
                LoadPackagings();
                SelectRow(_selectedCode);
                SetStatus("Embalagem alterada com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void InactivatePackaging()
        {
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                SetStatus("Selecione uma embalagem.", true);
                return;
            }

            if (MessageBox.Show(this, "Tem certeza que deseja inativar a embalagem '" + _selectedCode + "'?", "Confirmar Inativacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _masterDataController.InactivatePackaging(_configuration, _databaseProfile, _identity.UserName, _selectedCode);
                _hasChanges = true;
                LoadPackagings();
                ResetForm();
                SetStatus("Embalagem inativada com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private SavePackagingRequest BuildRequest()
        {
            return new SavePackagingRequest
            {
                Code = _codeTextBox.Text,
                Description = _descriptionTextBox.Text,
                Status = Convert.ToString(_statusComboBox.SelectedItem),
                IsBrcEnabled = _brcCheckBox.Checked,
                ActorUserName = _identity.UserName,
            };
        }

        private void UpdateStatistics()
        {
            _totalLabel.Text = _allPackagings.Length.ToString();
            _activeLabel.Text = _allPackagings.Count(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
            _inactiveLabel.Text = _allPackagings.Count(item => !string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
            _brcLabel.Text = _allPackagings.Count(item => item.IsBrcEnabled).ToString();
        }

        private void ResetForm(bool clearSelection = true)
        {
            _selectedCode = null;
            _codeTextBox.ReadOnly = false;
            _codeTextBox.Clear();
            _descriptionTextBox.Clear();
            _statusComboBox.SelectedItem = "ATIVO";
            _brcCheckBox.Checked = false;
            if (clearSelection)
            {
                _grid.ClearSelection();
            }

            SetStatus("Preencha os dados da embalagem.", false);
        }

        private void SelectRow(string code)
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.DataBoundItem is PackagingSummary item && string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase))
                {
                    row.Selected = true;
                    _grid.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        private PackagingSummary[] SortPackagings(PackagingSummary[] items)
        {
            Func<PackagingSummary, object> selector;
            switch ((_sortColumn ?? string.Empty).ToLowerInvariant())
            {
                case "descricao": selector = item => item.Description ?? string.Empty; break;
                case "brc": selector = item => item.IsBrcEnabled; break;
                case "status": selector = item => item.Status ?? string.Empty; break;
                case "versao": selector = item => item.Version; break;
                default: selector = item => item.Code ?? string.Empty; break;
            }

            return (_sortAscending ? items.OrderBy(selector) : items.OrderByDescending(selector)).ToArray();
        }

        private void OnGridColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            var column = _grid.Columns[e.ColumnIndex].Name;
            _sortAscending = string.Equals(_sortColumn, column, StringComparison.OrdinalIgnoreCase) ? !_sortAscending : true;
            _sortColumn = column;
            RefreshGrid();
        }

        private void OnGridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_grid.Columns[e.ColumnIndex].Name == "brc" && e.Value != null)
            {
                e.Value = Convert.ToBoolean(e.Value) ? "Sim" : "Nao";
                e.FormattingApplied = true;
            }
        }

        private void OnGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_grid.Rows[e.RowIndex].DataBoundItem is PackagingSummary item))
            {
                return;
            }

            var row = _grid.Rows[e.RowIndex];
            row.DefaultCellStyle.ForeColor = string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase) ? Color.Gray : Color.Black;
            row.DefaultCellStyle.BackColor = !string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase) && item.StockBalance > 0
                ? Color.FromArgb(232, 245, 233)
                : Color.White;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) CreatePackaging();
            else if (e.KeyCode == Keys.F3) UpdatePackaging();
            else if (e.KeyCode == Keys.F4) CloseForm();
            else if (e.KeyCode == Keys.F5) ResetForm();
            else if (e.KeyCode == Keys.F6) InactivatePackaging();
        }

        private void CloseForm()
        {
            DialogResult = _hasChanges ? DialogResult.OK : DialogResult.Cancel;
            Close();
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

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label { AutoSize = true, Text = text, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93), Margin = new Padding(0, 8, 0, 4) };
        }

        private static Label CreateStatCaption(string text, Color color, int leftMargin)
        {
            return new Label { AutoSize = true, Text = text, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = color, Margin = new Padding(leftMargin, 8, 2, 0) };
        }

        private static Label CreateStatValue(Color color)
        {
            return new Label { AutoSize = true, Text = "0", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = color, Margin = new Padding(0, 8, 12, 0) };
        }
    }
}
