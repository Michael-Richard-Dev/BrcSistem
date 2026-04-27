using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed class SupplierManagementForm : Form
    {
        private readonly MasterDataController _masterDataController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private SupplierSummary[] _allSuppliers;
        private DataGridView _grid;
        private TextBox _codeTextBox;
        private TextBox _nameTextBox;
        private TextBox _cnpjTextBox;
        private TextBox _cityTextBox;
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
        private bool _isFormattingCnpj;
        private bool _hasChanges;

        public SupplierManagementForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _masterDataController = compositionRoot.CreateMasterDataController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _allSuppliers = new SupplierSummary[0];

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Cadastro de Fornecedores";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1100, 680);
            MinimumSize = new Size(1000, 640);
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
            var group = new GroupBox { Dock = DockStyle.Top, Height = 170, Text = "Dados do Fornecedor", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 4 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));

            layout.Controls.Add(CreateFieldLabel("Codigo:"), 0, 0);
            _codeTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            layout.Controls.Add(_codeTextBox, 1, 0);

            layout.Controls.Add(CreateFieldLabel("Nome:"), 2, 0);
            _nameTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            layout.Controls.Add(_nameTextBox, 3, 0);

            layout.Controls.Add(CreateFieldLabel("CNPJ:"), 0, 1);
            _cnpjTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            _cnpjTextBox.TextChanged += OnCnpjTextChanged;
            layout.Controls.Add(_cnpjTextBox, 1, 1);

            layout.Controls.Add(CreateFieldLabel("Cidade:"), 2, 1);
            _cityTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            layout.Controls.Add(_cityTextBox, 3, 1);

            layout.Controls.Add(CreateFieldLabel("Status:"), 0, 2);
            _statusComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Width = 160 };
            _statusComboBox.Items.AddRange(new object[] { "ATIVO", "INATIVO" });
            layout.Controls.Add(_statusComboBox, 1, 2);

            _brcCheckBox = new CheckBox
            {
                AutoSize = true,
                Text = "Habilitado para BRC",
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(0, 6, 0, 0),
            };
            layout.Controls.Add(_brcCheckBox, 3, 2);

            _statusLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(0, 8, 0, 0) };
            layout.Controls.Add(_statusLabel, 3, 3);

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
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "codigo", HeaderText = "CODIGO", DataPropertyName = nameof(SupplierSummary.Code), Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "nome", HeaderText = "NOME", DataPropertyName = nameof(SupplierSummary.Name), Width = 260, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "cnpj", HeaderText = "CNPJ", DataPropertyName = nameof(SupplierSummary.Cnpj), Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "cidade", HeaderText = "CIDADE", DataPropertyName = nameof(SupplierSummary.City), Width = 140 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "brc", HeaderText = "BRC", DataPropertyName = nameof(SupplierSummary.IsBrcEnabled), Width = 70 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(SupplierSummary.Status), Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "versao", HeaderText = "VERSAO", DataPropertyName = nameof(SupplierSummary.Version), Width = 90 });
            _grid.SelectionChanged += (sender, args) => PopulateSelectedSupplier();
            _grid.ColumnHeaderMouseClick += OnGridColumnHeaderMouseClick;
            _grid.CellFormatting += OnGridCellFormatting;
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
            actions.Controls.Add(CreateButton("Adicionar (F2)", (sender, args) => CreateSupplier()));
            actions.Controls.Add(CreateButton("Alterar (F3)", (sender, args) => UpdateSupplier()));
            actions.Controls.Add(CreateButton("Inativar (F6)", (sender, args) => InactivateSupplier()));
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
            stats.Controls.Add(CreateStatCaption("BRC:", Color.FromArgb(41, 128, 185), 0));
            _brcLabel = CreateStatValue(Color.FromArgb(41, 128, 185));
            stats.Controls.Add(_brcLabel);

            _hideInactiveCheckBox = new CheckBox { AutoSize = true, Text = "Ocultar inativos", Checked = true, Font = new Font("Segoe UI", 9F), Margin = new Padding(12, 6, 0, 0) };
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
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            _allSuppliers = _masterDataController.LoadSuppliers(_configuration, _databaseProfile);
            UpdateStatistics();
            RefreshGrid();
            ResetForm(false);
        }

        private void RefreshGrid()
        {
            var filtered = _allSuppliers.Where(item => !_hideInactiveCheckBox.Checked || !string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase)).ToArray();
            _grid.DataSource = SortSuppliers(filtered).Select(item => new
            {
                item.Code,
                item.Name,
                Cnpj = FormatCnpj(item.Cnpj),
                item.City,
                IsBrcEnabled = item.IsBrcEnabled ? "Sim" : "Nao",
                item.Status,
                item.Version,
            }).ToArray();
        }

        private void PopulateSelectedSupplier()
        {
            if (_grid.CurrentRow == null)
            {
                return;
            }

            _selectedCode = Convert.ToString(_grid.CurrentRow.Cells["codigo"].Value);
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                return;
            }

            var supplier = _allSuppliers.FirstOrDefault(item => string.Equals(item.Code, _selectedCode, StringComparison.OrdinalIgnoreCase));
            if (supplier == null)
            {
                return;
            }

            _codeTextBox.Text = supplier.Code ?? string.Empty;
            _codeTextBox.ReadOnly = true;
            _nameTextBox.Text = supplier.Name ?? string.Empty;
            _cnpjTextBox.Text = FormatCnpj(supplier.Cnpj);
            _cityTextBox.Text = supplier.City ?? string.Empty;
            _statusComboBox.SelectedItem = supplier.Status;
            _brcCheckBox.Checked = supplier.IsBrcEnabled;
            SetStatus("Fornecedor carregado.", false);
        }

        private void CreateSupplier()
        {
            try
            {
                _masterDataController.CreateSupplier(_configuration, _databaseProfile, BuildRequest());
                _hasChanges = true;
                LoadSuppliers();
                ResetForm();
                SetStatus("Fornecedor adicionado com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateSupplier()
        {
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                SetStatus("Selecione um fornecedor.", true);
                return;
            }

            try
            {
                var request = BuildRequest();
                request.Code = _selectedCode;
                _masterDataController.UpdateSupplier(_configuration, _databaseProfile, request);
                _hasChanges = true;
                LoadSuppliers();
                SelectRow(_selectedCode);
                SetStatus("Fornecedor alterado com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void InactivateSupplier()
        {
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                SetStatus("Selecione um fornecedor.", true);
                return;
            }

            if (MessageBox.Show(this, "Tem certeza que deseja inativar o fornecedor '" + _selectedCode + "'?", "Confirmar Inativacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _masterDataController.InactivateSupplier(_configuration, _databaseProfile, _identity.UserName, _selectedCode);
                _hasChanges = true;
                LoadSuppliers();
                ResetForm();
                SetStatus("Fornecedor inativado com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private SaveSupplierRequest BuildRequest()
        {
            return new SaveSupplierRequest
            {
                Code = _codeTextBox.Text,
                Name = _nameTextBox.Text,
                Cnpj = _cnpjTextBox.Text,
                City = _cityTextBox.Text,
                Status = Convert.ToString(_statusComboBox.SelectedItem),
                IsBrcEnabled = _brcCheckBox.Checked,
                ActorUserName = _identity.UserName,
            };
        }

        private void UpdateStatistics()
        {
            _totalLabel.Text = _allSuppliers.Length.ToString();
            _activeLabel.Text = _allSuppliers.Count(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
            _inactiveLabel.Text = _allSuppliers.Count(item => !string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
            _brcLabel.Text = _allSuppliers.Count(item => item.IsBrcEnabled).ToString();
        }

        private void ResetForm(bool clearSelection = true)
        {
            _selectedCode = null;
            _codeTextBox.ReadOnly = false;
            _codeTextBox.Clear();
            _nameTextBox.Clear();
            _cnpjTextBox.Clear();
            _cityTextBox.Clear();
            _statusComboBox.SelectedItem = "ATIVO";
            _brcCheckBox.Checked = false;
            if (clearSelection)
            {
                _grid.ClearSelection();
            }

            SetStatus("Preencha os dados do fornecedor.", false);
        }

        private void SelectRow(string code)
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (string.Equals(Convert.ToString(row.Cells["codigo"].Value), code, StringComparison.OrdinalIgnoreCase))
                {
                    row.Selected = true;
                    _grid.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        private SupplierSummary[] SortSuppliers(SupplierSummary[] items)
        {
            Func<SupplierSummary, object> selector;
            switch ((_sortColumn ?? string.Empty).ToLowerInvariant())
            {
                case "nome": selector = item => item.Name ?? string.Empty; break;
                case "cnpj": selector = item => item.Cnpj ?? string.Empty; break;
                case "cidade": selector = item => item.City ?? string.Empty; break;
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
            if (string.Equals(_grid.Columns[e.ColumnIndex].Name, "status", StringComparison.OrdinalIgnoreCase)
                && string.Equals(Convert.ToString(e.Value), "INATIVO", StringComparison.OrdinalIgnoreCase))
            {
                e.CellStyle.ForeColor = Color.Firebrick;
            }
        }

        private void OnCnpjTextChanged(object sender, EventArgs e)
        {
            if (_isFormattingCnpj) return;
            _isFormattingCnpj = true;
            var formatted = FormatCnpj(_cnpjTextBox.Text);
            _cnpjTextBox.Text = formatted;
            _cnpjTextBox.SelectionStart = formatted.Length;
            _isFormattingCnpj = false;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) CreateSupplier();
            else if (e.KeyCode == Keys.F3) UpdateSupplier();
            else if (e.KeyCode == Keys.F4) CloseForm();
            else if (e.KeyCode == Keys.F5) ResetForm();
            else if (e.KeyCode == Keys.F6) InactivateSupplier();
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

        private static string FormatCnpj(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).Take(14).ToArray());
            if (digits.Length <= 2) return digits;
            if (digits.Length <= 5) return digits.Insert(2, ".");
            if (digits.Length <= 8) return digits.Insert(2, ".").Insert(6, ".");
            if (digits.Length <= 12) return digits.Insert(2, ".").Insert(6, ".").Insert(10, "/");
            return digits.Insert(2, ".").Insert(6, ".").Insert(10, "/").Insert(15, "-");
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
