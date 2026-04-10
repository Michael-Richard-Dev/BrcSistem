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
    public sealed class ProductManagementForm : Form
    {
        private readonly MasterDataController _masterDataController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private ProductSummary[] _allProducts;
        private DataGridView _grid;
        private TextBox _codeTextBox;
        private TextBox _descriptionTextBox;
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

        public ProductManagementForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _masterDataController = compositionRoot.CreateMasterDataController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _allProducts = new ProductSummary[0];

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Cadastro de Produtos";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1020, 650);
            MinimumSize = new Size(920, 600);
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
            var group = new GroupBox { Dock = DockStyle.Top, Height = 145, Text = "Dados do Produto", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "codigo", HeaderText = "CODIGO", DataPropertyName = nameof(ProductSummary.Code), Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "descricao", HeaderText = "DESCRICAO", DataPropertyName = nameof(ProductSummary.Description), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(ProductSummary.Status), Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "versao", HeaderText = "VERSAO", DataPropertyName = nameof(ProductSummary.Version), Width = 90 });
            _grid.SelectionChanged += (sender, args) => PopulateSelectedProduct();
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
            actions.Controls.Add(CreateButton("Adicionar (F2)", (sender, args) => CreateProduct()));
            actions.Controls.Add(CreateButton("Alterar (F3)", (sender, args) => UpdateProduct()));
            actions.Controls.Add(CreateButton("Inativar (F6)", (sender, args) => InactivateProduct()));
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
            LoadProducts();
        }

        private void LoadProducts()
        {
            _allProducts = _masterDataController.LoadProducts(_configuration, _databaseProfile);
            UpdateStatistics();
            RefreshGrid();
            ResetForm(false);
        }

        private void RefreshGrid()
        {
            var filtered = _allProducts.Where(item => !_hideInactiveCheckBox.Checked || !string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase)).ToArray();
            _grid.DataSource = SortProducts(filtered);
        }

        private void PopulateSelectedProduct()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is ProductSummary product))
            {
                return;
            }

            _selectedCode = product.Code;
            _codeTextBox.Text = product.Code ?? string.Empty;
            _codeTextBox.ReadOnly = true;
            _descriptionTextBox.Text = product.Description ?? string.Empty;
            _statusComboBox.SelectedItem = product.Status;
            SetStatus("Produto carregado.", false);
        }

        private void CreateProduct()
        {
            try
            {
                _masterDataController.CreateProduct(_configuration, _databaseProfile, BuildRequest());
                _hasChanges = true;
                LoadProducts();
                ResetForm();
                SetStatus("Produto adicionado com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateProduct()
        {
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                SetStatus("Selecione um produto.", true);
                return;
            }

            try
            {
                var request = BuildRequest();
                request.Code = _selectedCode;
                _masterDataController.UpdateProduct(_configuration, _databaseProfile, request);
                _hasChanges = true;
                LoadProducts();
                SelectRow(_selectedCode);
                SetStatus("Produto alterado com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void InactivateProduct()
        {
            if (string.IsNullOrWhiteSpace(_selectedCode))
            {
                SetStatus("Selecione um produto.", true);
                return;
            }

            if (MessageBox.Show(this, "Tem certeza que deseja inativar o produto '" + _selectedCode + "'?", "Confirmar Inativacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _masterDataController.InactivateProduct(_configuration, _databaseProfile, _identity.UserName, _selectedCode);
                _hasChanges = true;
                LoadProducts();
                ResetForm();
                SetStatus("Produto inativado com sucesso!", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private SaveProductRequest BuildRequest()
        {
            return new SaveProductRequest
            {
                Code = _codeTextBox.Text,
                Description = _descriptionTextBox.Text,
                Status = Convert.ToString(_statusComboBox.SelectedItem),
                ActorUserName = _identity.UserName,
            };
        }

        private void UpdateStatistics()
        {
            _totalLabel.Text = _allProducts.Length.ToString();
            _activeLabel.Text = _allProducts.Count(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
            _inactiveLabel.Text = _allProducts.Count(item => !string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
        }

        private void ResetForm(bool clearSelection = true)
        {
            _selectedCode = null;
            _codeTextBox.ReadOnly = false;
            _codeTextBox.Clear();
            _descriptionTextBox.Clear();
            _statusComboBox.SelectedItem = "ATIVO";
            if (clearSelection)
            {
                _grid.ClearSelection();
            }

            SetStatus("Preencha os dados do produto.", false);
        }

        private void SelectRow(string code)
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.DataBoundItem is ProductSummary item && string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase))
                {
                    row.Selected = true;
                    _grid.CurrentCell = row.Cells[0];
                    return;
                }
            }
        }

        private ProductSummary[] SortProducts(ProductSummary[] items)
        {
            Func<ProductSummary, object> selector;
            switch ((_sortColumn ?? string.Empty).ToLowerInvariant())
            {
                case "descricao": selector = item => item.Description ?? string.Empty; break;
                case "status": selector = item => item.Status ?? string.Empty; break;
                case "versao": selector = item => item.Version; break;
                default: selector = item => ParseNumericCode(item.Code); break;
            }

            return (_sortAscending ? items.OrderBy(selector) : items.OrderByDescending(selector))
                .ThenBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();
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
            if (!(_grid.Rows[e.RowIndex].DataBoundItem is ProductSummary item))
            {
                return;
            }

            _grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = string.Equals(item.Status, "INATIVO", StringComparison.OrdinalIgnoreCase) ? Color.Gray : Color.Black;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2) CreateProduct();
            else if (e.KeyCode == Keys.F3) UpdateProduct();
            else if (e.KeyCode == Keys.F4) CloseForm();
            else if (e.KeyCode == Keys.F5) ResetForm();
            else if (e.KeyCode == Keys.F6) InactivateProduct();
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

        private static int ParseNumericCode(string code)
        {
            int parsed;
            return int.TryParse(code, out parsed) ? parsed : int.MaxValue;
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
