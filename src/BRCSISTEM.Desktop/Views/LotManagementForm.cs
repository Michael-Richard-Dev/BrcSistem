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
    public sealed partial class LotManagementForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly MasterDataController _masterDataController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly string _prefillMaterialCode;
        private readonly string _prefillSupplierCode;

        private AppConfiguration _configuration;
        private LotSummary[] _allLots;
        private LookupOption[] _supplierOptions;
        private LookupOption[] _materialOptions;
        private DataGridView _grid;
        private TextBox _codeTextBox;
        private TextBox _nameTextBox;
        private ComboBox _materialComboBox;
        private ComboBox _supplierComboBox;
        private TextBox _expirationTextBox;
        private ComboBox _statusComboBox;
        private CheckBox _hideInactiveCheckBox;
        private Label _totalLabel;
        private Label _activeLabel;
        private Label _inactiveLabel;
        private Label _statusLabel;
        private string _selectedCode;
        private string _sortColumn = "codigo";
        private bool _sortAscending = true;
        private bool _hasChanges;
        private bool _prefillApplied;
        private bool _isFormattingExpiration;

        public LotManagementForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile,
            string prefillMaterialCode = null,
            string prefillSupplierCode = null)
        {
            _compositionRoot = compositionRoot;
            _masterDataController = compositionRoot.CreateMasterDataController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _prefillMaterialCode = prefillMaterialCode;
            _prefillSupplierCode = prefillSupplierCode;
            _allLots = new LotSummary[0];
            _supplierOptions = new LookupOption[0];
            _materialOptions = new LookupOption[0];

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Cadastro de Lotes";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1180, 720);
            MinimumSize = new Size(1040, 660);
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
            var group = new GroupBox { Dock = DockStyle.Top, Height = 190, Text = "Dados do Lote", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var line1 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 4, AutoSize = true };
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            line1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            line1.Controls.Add(CreateFieldLabel("Codigo (auto):"), 0, 0);
            _codeTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F), ReadOnly = true };
            line1.Controls.Add(_codeTextBox, 1, 0);
            line1.Controls.Add(CreateFieldLabel("Nome do Lote:"), 2, 0);
            _nameTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            line1.Controls.Add(_nameTextBox, 3, 0);

            var line2 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 6, AutoSize = true };
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            line2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 138F));
            line2.Controls.Add(CreateFieldLabel("Material:"), 0, 0);
            _materialComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Dock = DockStyle.Top };
            line2.Controls.Add(_materialComboBox, 1, 0);
            var materialButtons = new FlowLayoutPanel { Dock = DockStyle.Left, AutoSize = true };
            materialButtons.Controls.Add(CreateButton("Buscar", (sender, args) => OpenMaterialLookup()));
            line2.Controls.Add(materialButtons, 2, 0);
            line2.Controls.Add(CreateFieldLabel("Fornecedor:"), 3, 0);
            _supplierComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Dock = DockStyle.Top };
            line2.Controls.Add(_supplierComboBox, 4, 0);
            var supplierButtons = new FlowLayoutPanel { Dock = DockStyle.Left, AutoSize = true };
            supplierButtons.Controls.Add(CreateButton("Buscar", (sender, args) => OpenSupplierLookup()));
            supplierButtons.Controls.Add(CreateButton("Novo", (sender, args) => OpenSupplierManagement()));
            line2.Controls.Add(supplierButtons, 5, 0);

            var line3 = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 4, AutoSize = true };
            line3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            line3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            line3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95F));
            line3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            line3.Controls.Add(CreateFieldLabel("Validade (DD/MM/AAAA):"), 0, 0);
            _expirationTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            _expirationTextBox.TextChanged += OnExpirationTextChanged;
            line3.Controls.Add(_expirationTextBox, 1, 0);
            line3.Controls.Add(CreateFieldLabel("Status:"), 2, 0);
            _statusComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F), Width = 160 };
            _statusComboBox.Items.AddRange(new object[] { "ATIVO", "INATIVO" });
            line3.Controls.Add(_statusComboBox, 3, 0);

            _statusLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(0, 8, 0, 0) };

            root.Controls.Add(line1, 0, 0);
            root.Controls.Add(line2, 0, 1);
            root.Controls.Add(line3, 0, 2);
            root.Controls.Add(_statusLabel, 0, 3);
            group.Controls.Add(root);
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
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "codigo", HeaderText = "CODIGO", DataPropertyName = nameof(LotSummary.Code), Width = 90 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "nome", HeaderText = "NOME", DataPropertyName = nameof(LotSummary.Name), Width = 180, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "material", HeaderText = "MATERIAL", DataPropertyName = nameof(LotSummary.MaterialDisplay), Width = 220, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "fornecedor", HeaderText = "FORNECEDOR", DataPropertyName = nameof(LotSummary.SupplierDisplay), Width = 220, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "validade", HeaderText = "VALIDADE", DataPropertyName = nameof(LotSummary.ExpirationDate), Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(LotSummary.Status), Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "versao", HeaderText = "VERSAO", DataPropertyName = nameof(LotSummary.Version), Width = 90 });
            _grid.SelectionChanged += (sender, args) => PopulateSelectedLot();
            _grid.ColumnHeaderMouseClick += OnGridColumnHeaderMouseClick;
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
            actions.Controls.Add(CreateButton("Adicionar (F2)", (sender, args) => CreateLot()));
            actions.Controls.Add(CreateButton("Alterar (F3)", (sender, args) => UpdateLot()));
            actions.Controls.Add(CreateButton("Inativar (F6)", (sender, args) => InactivateLot()));
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
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                LoadReferenceData();
                LoadLots();
                ResetForm(false);
                ApplyPrefillSelections();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadReferenceData()
        {
            _supplierOptions = _masterDataController.LoadSuppliers(_configuration, _databaseProfile)
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Name ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status })
                .ToArray();

            _materialOptions = _masterDataController.LoadPackagings(_configuration, _databaseProfile)
                .Where(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(item => new LookupOption { Code = item.Code, Description = item.Description, Status = item.Status })
                .ToArray();

            BindCombo(_supplierComboBox, _supplierOptions);
            BindCombo(_materialComboBox, _materialOptions);
        }

        private void LoadLots()
        {
            _allLots = _masterDataController.LoadLots(_configuration, _databaseProfile);
            UpdateStatistics();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var filtered = _allLots.Where(item => !_hideInactiveCheckBox.Checked || !string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase)).ToArray();
            _grid.DataSource = SortLots(filtered);
        }

        private void PopulateSelectedLot()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is LotSummary lot))
            {
                return;
            }

            _selectedCode = lot.Code;
            _codeTextBox.Text = lot.Code ?? string.Empty;
            _nameTextBox.Text = lot.Name ?? string.Empty;
            EnsureOptionPresent(ref _materialOptions, _materialComboBox, lot.MaterialCode, lot.MaterialDescription);
            EnsureOptionPresent(ref _supplierOptions, _supplierComboBox, lot.SupplierCode, lot.SupplierName);
            _expirationTextBox.Text = lot.ExpirationDate ?? string.Empty;
            _statusComboBox.SelectedItem = lot.Status;
            SetStatus("Lote carregado.", false);
        }

        private void CreateLot()
        {
            try
            {
                var request = BuildRequest();
                if (!ValidateLotForCreate(request))
                {
                    return;
                }

                var createdCode = _masterDataController.CreateLot(_configuration, _databaseProfile, request);
                _hasChanges = true;
                LoadLots();
                ResetForm();
                SetStatus("Lote adicionado com sucesso! Codigo: " + createdCode, false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateLot()
        {
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                SetStatus("Selecione um lote.", true);
                return;
            }

            try
            {
                var request = BuildRequest();
                request.Code = _selectedCode;
                if (!ValidateLotForUpdate(request))
                {
                    return;
                }

                _masterDataController.UpdateLot(_configuration, _databaseProfile, request);
                _hasChanges = true;
                LoadLots();
                SelectRow(_selectedCode);
                SetStatus("Lote alterado com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void InactivateLot()
        {
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                SetStatus("Selecione um lote.", true);
                return;
            }

            if (MessageBox.Show(this, "Tem certeza que deseja inativar o lote '" + _selectedCode + "'?", "Confirmar Inativacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _masterDataController.InactivateLot(_configuration, _databaseProfile, _identity.UserName, _selectedCode);
                _hasChanges = true;
                LoadLots();
                ResetForm();
                SetStatus("Lote inativado com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private SaveLotRequest BuildRequest()
        {
            var expiration = NormalizeExpirationInput(_expirationTextBox.Text);
            if (!string.Equals(_expirationTextBox.Text, expiration, StringComparison.Ordinal))
            {
                _expirationTextBox.Text = expiration;
                _expirationTextBox.SelectionStart = _expirationTextBox.Text.Length;
            }

            return new SaveLotRequest
            {
                Code = _codeTextBox.Text,
                Name = _nameTextBox.Text,
                MaterialCode = (_materialComboBox.SelectedItem as LookupOption)?.Code,
                SupplierCode = (_supplierComboBox.SelectedItem as LookupOption)?.Code,
                ExpirationDate = expiration,
                Status = Convert.ToString(_statusComboBox.SelectedItem),
                ActorUserName = _identity.UserName,
            };
        }

        private void UpdateStatistics()
        {
            _totalLabel.Text = _allLots.Length.ToString();
            _activeLabel.Text = _allLots.Count(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
            _inactiveLabel.Text = _allLots.Count(item => !string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
        }

        private void ResetForm(bool clearSelection = true)
        {
            _selectedCode = null;
            _codeTextBox.Text = _masterDataController.GenerateNextLotCode(_configuration, _databaseProfile);
            _nameTextBox.Clear();
            _materialComboBox.SelectedIndex = -1;
            _supplierComboBox.SelectedIndex = -1;
            _expirationTextBox.Clear();
            _statusComboBox.SelectedItem = "ATIVO";
            if (clearSelection)
            {
                _grid.ClearSelection();
            }

            SetStatus("Preencha os dados do lote.", false);
        }

        private void SelectRow(string code)
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.DataBoundItem is LotSummary item && string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase))
                {
                    row.Selected = true;
                    _grid.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        private void OnGridColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            var column = _grid.Columns[e.ColumnIndex].Name;
            _sortAscending = string.Equals(_sortColumn, column, StringComparison.OrdinalIgnoreCase) ? !_sortAscending : true;
            _sortColumn = column;
            RefreshGrid();
        }

        private void OnGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_grid.Rows[e.RowIndex].DataBoundItem is LotSummary item))
            {
                return;
            }

            var row = _grid.Rows[e.RowIndex];
            row.DefaultCellStyle.ForeColor = string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase) ? Color.Gray : Color.Black;
            row.DefaultCellStyle.BackColor = !string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase) && item.StockBalance > 0
                ? Color.FromArgb(232, 245, 233)
                : Color.White;
        }

        private void OnExpirationTextChanged(object sender, EventArgs e)
        {
            if (_isFormattingExpiration)
            {
                return;
            }

            _isFormattingExpiration = true;
            var formatted = NormalizeExpirationInput(_expirationTextBox.Text);
            _expirationTextBox.Text = formatted;
            _expirationTextBox.SelectionStart = formatted.Length;
            _isFormattingExpiration = false;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) CreateLot();
            else if (e.KeyCode == Keys.F3) UpdateLot();
            else if (e.KeyCode == Keys.F4) CloseForm();
            else if (e.KeyCode == Keys.F5) ResetForm();
            else if (e.KeyCode == Keys.F6) InactivateLot();
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
    }
}
