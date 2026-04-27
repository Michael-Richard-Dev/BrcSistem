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
    public sealed class UserManagementForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly AdministrationController _administrationController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly string _initialUserName;

        private AppConfiguration _configuration;
        private UserSummary[] _allUsers;
        private DataGridView _usersGrid;
        private TextBox _userNameTextBox;
        private TextBox _displayNameTextBox;
        private TextBox _passwordTextBox;
        private ComboBox _userTypeComboBox;
        private ComboBox _statusComboBox;
        private CheckBox _hideInactiveCheckBox;
        private Label _totalLabel;
        private Label _activeLabel;
        private Label _inactiveLabel;
        private Label _statusLabel;
        private string _selectedUserName;
        private string _sortColumn = nameof(UserSummary.UserName);
        private bool _sortAscending = true;
        private bool _hasChanges;

        public UserManagementForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile, string initialUserName = null)
        {
            _compositionRoot = compositionRoot;
            _administrationController = compositionRoot.CreateAdministrationController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _initialUserName = initialUserName;
            _allUsers = new UserSummary[0];

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Cadastro de Usuarios";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(980, 620);
            MinimumSize = new Size(960, 620);
            BackColor = Color.White;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                Padding = new Padding(12),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildFormPanel(), 0, 0);
            root.Controls.Add(BuildGridPanel(), 0, 1);
            root.Controls.Add(BuildFooterPanel(), 0, 2);

            Controls.Add(root);
            KeyPreview = true;
            KeyDown += OnFormKeyDown;
        }

        private Control BuildFormPanel()
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Top,
                Text = "Dados do Usuario",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Height = 160,
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                Padding = new Padding(10),
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));

            layout.Controls.Add(CreateFieldLabel("Usuario:"), 0, 0);
            _userNameTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            layout.Controls.Add(_userNameTextBox, 1, 0);

            layout.Controls.Add(CreateFieldLabel("Nome:"), 2, 0);
            _displayNameTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            layout.Controls.Add(_displayNameTextBox, 3, 0);

            layout.Controls.Add(CreateFieldLabel("Senha:"), 0, 1);
            var passwordPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                AutoSize = true,
            };
            _passwordTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F), UseSystemPasswordChar = true };
            passwordPanel.Controls.Add(_passwordTextBox, 0, 0);
            passwordPanel.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "(deixe vazio para manter atual)",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                Margin = new Padding(3, 3, 0, 0),
            }, 0, 1);
            layout.Controls.Add(passwordPanel, 1, 1);

            layout.Controls.Add(CreateFieldLabel("Tipo:"), 2, 1);
            var typePanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            _userTypeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 220,
                Font = new Font("Segoe UI", 10F),
            };
            var manageTypesButton = new Button { Text = "...", Width = 36, FlatStyle = FlatStyle.System };
            manageTypesButton.Click += (sender, args) => OpenUserTypeManagement();
            var refreshTypesButton = new Button { Text = "Atualizar", AutoSize = true, FlatStyle = FlatStyle.System };
            refreshTypesButton.Click += (sender, args) => LoadUserTypes(true);
            typePanel.Controls.Add(_userTypeComboBox);
            typePanel.Controls.Add(manageTypesButton);
            typePanel.Controls.Add(refreshTypesButton);
            layout.Controls.Add(typePanel, 3, 1);

            layout.Controls.Add(CreateFieldLabel("Status:"), 0, 2);
            _statusComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 160,
                Font = new Font("Segoe UI", 10F),
            };
            _statusComboBox.Items.AddRange(new object[] { "ATIVO", "INATIVO" });
            layout.Controls.Add(_statusComboBox, 1, 2);

            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
                Margin = new Padding(0, 6, 0, 0),
            };
            layout.Controls.Add(_statusLabel, 3, 2);

            group.Controls.Add(layout);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Usuarios Cadastrados",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            _usersGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
            };
            _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "usuario", HeaderText = "USUARIO", DataPropertyName = nameof(UserSummary.UserName), Width = 150 });
            _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "nome", HeaderText = "NOME", DataPropertyName = nameof(UserSummary.DisplayName), Width = 260, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "tipo", HeaderText = "TIPO", DataPropertyName = nameof(UserSummary.UserType), Width = 180 });
            _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(UserSummary.Status), Width = 120 });
            _usersGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "versao", HeaderText = "VERSAO", DataPropertyName = nameof(UserSummary.Version), Width = 90 });
            _usersGrid.SelectionChanged += (sender, args) => PopulateSelectedUser();
            _usersGrid.ColumnHeaderMouseClick += OnGridColumnHeaderMouseClick;

            group.Controls.Add(_usersGrid);
            return group;
        }

        private Control BuildFooterPanel()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                AutoSize = true,
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var actionButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            var addButton = new Button { Text = "Adicionar (F2)", AutoSize = true, FlatStyle = FlatStyle.System };
            addButton.Click += (sender, args) => CreateUser();
            var updateButton = new Button { Text = "Alterar (F3)", AutoSize = true, FlatStyle = FlatStyle.System };
            updateButton.Click += (sender, args) => UpdateUser();
            var inactivateButton = new Button { Text = "Inativar (F6)", AutoSize = true, FlatStyle = FlatStyle.System };
            inactivateButton.Click += (sender, args) => InactivateUser();
            var clearButton = new Button { Text = "Limpar (F5)", AutoSize = true, FlatStyle = FlatStyle.System };
            clearButton.Click += (sender, args) => ResetForm();
            actionButtons.Controls.Add(addButton);
            actionButtons.Controls.Add(updateButton);
            actionButtons.Controls.Add(inactivateButton);
            actionButtons.Controls.Add(clearButton);

            var statsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false,
            };
            statsPanel.Controls.Add(new Label { AutoSize = true, Text = "Total:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(52, 73, 94), Margin = new Padding(18, 8, 2, 0) });
            _totalLabel = new Label { AutoSize = true, Text = "0", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(52, 73, 94), Margin = new Padding(0, 8, 12, 0) };
            statsPanel.Controls.Add(_totalLabel);
            statsPanel.Controls.Add(new Label { AutoSize = true, Text = "Ativos:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(0, 8, 2, 0) });
            _activeLabel = new Label { AutoSize = true, Text = "0", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen, Margin = new Padding(0, 8, 12, 0) };
            statsPanel.Controls.Add(_activeLabel);
            statsPanel.Controls.Add(new Label { AutoSize = true, Text = "Inativos:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.Firebrick, Margin = new Padding(0, 8, 2, 0) });
            _inactiveLabel = new Label { AutoSize = true, Text = "0", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.Firebrick, Margin = new Padding(0, 8, 18, 0) };
            statsPanel.Controls.Add(_inactiveLabel);

            _hideInactiveCheckBox = new CheckBox
            {
                AutoSize = true,
                Text = "Ocultar inativos",
                Font = new Font("Segoe UI", 9F),
                Checked = true,
                Margin = new Padding(0, 6, 0, 0),
            };
            _hideInactiveCheckBox.CheckedChanged += (sender, args) => RefreshGrid();
            statsPanel.Controls.Add(_hideInactiveCheckBox);

            var rightButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            var closeButton = new Button { Text = "Fechar (F4)", AutoSize = true, FlatStyle = FlatStyle.System };
            closeButton.Click += (sender, args) =>
            {
                DialogResult = _hasChanges ? DialogResult.OK : DialogResult.Cancel;
                Close();
            };
            rightButtons.Controls.Add(closeButton);

            root.Controls.Add(actionButtons, 0, 0);
            root.Controls.Add(statsPanel, 1, 0);
            root.Controls.Add(rightButtons, 2, 0);
            return root;
        }

        private void LoadData()
        {
            _configuration = _configurationController.LoadConfiguration();
            LoadUserTypes(false);
            LoadUsers();
        }

        private void LoadUsers()
        {
            _allUsers = _administrationController.LoadUsers(_configuration, _databaseProfile);
            UpdateStatistics();
            RefreshGrid();
            ResetForm(false);
            SelectInitialUserIfAny();
        }

        private void LoadUserTypes(bool showStatus)
        {
            var selectedType = Convert.ToString(_userTypeComboBox.SelectedItem);
            var types = _administrationController.LoadUserTypeNames(_configuration, _databaseProfile);
            _userTypeComboBox.DataSource = types;
            if (!string.IsNullOrWhiteSpace(selectedType) && types.Contains(selectedType, StringComparer.OrdinalIgnoreCase))
            {
                _userTypeComboBox.SelectedItem = types.First(item => string.Equals(item, selectedType, StringComparison.OrdinalIgnoreCase));
            }

            if (showStatus)
            {
                SetStatus("Lista de tipos atualizada!", false);
            }
        }

        private void RefreshGrid()
        {
            var filtered = _allUsers
                .Where(user => !_hideInactiveCheckBox.Checked || !string.Equals(user.Status, "INATIVO", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            filtered = SortUsers(filtered);
            _usersGrid.DataSource = filtered;
        }

        private void PopulateSelectedUser()
        {
            if (_usersGrid.CurrentRow == null || !(_usersGrid.CurrentRow.DataBoundItem is UserSummary user))
            {
                return;
            }

            _selectedUserName = user.UserName;
            _userNameTextBox.Text = user.UserName ?? string.Empty;
            _userNameTextBox.ReadOnly = true;
            _displayNameTextBox.Text = user.DisplayName ?? string.Empty;
            _passwordTextBox.Clear();
            _userTypeComboBox.SelectedItem = user.UserType;
            _statusComboBox.SelectedItem = user.Status;
            SetStatus("Usuario carregado.", false);
        }

        private void CreateUser()
        {
            try
            {
                var request = BuildRequest();
                _administrationController.CreateUser(_configuration, _databaseProfile, request);
                _hasChanges = true;
                LoadUsers();
                ResetForm();
                SetStatus("Usuario adicionado com sucesso!", false);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
                MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUser()
        {
            if (string.IsNullOrWhiteSpace(_selectedUserName))
            {
                SetStatus("Selecione um usuario.", true);
                return;
            }

            try
            {
                var request = BuildRequest();
                request.OriginalUserName = _selectedUserName;
                _administrationController.UpdateUser(_configuration, _databaseProfile, request);
                _hasChanges = true;
                LoadUsers();
                SelectUser(_selectedUserName);
                SetStatus("Usuario alterado com sucesso!", false);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
                MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InactivateUser()
        {
            if (string.IsNullOrWhiteSpace(_selectedUserName))
            {
                SetStatus("Selecione um usuario.", true);
                return;
            }

            try
            {
                _administrationController.InactivateUser(_configuration, _databaseProfile, _identity.UserName, _selectedUserName);
                _hasChanges = true;
                LoadUsers();
                ResetForm();
                SetStatus("Usuario inativado com sucesso!", false);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
                MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenUserTypeManagement()
        {
            using (var dialog = new UserTypeManagementForm(_compositionRoot, _identity, _databaseProfile))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _configuration = _configurationController.LoadConfiguration();
                    LoadUserTypes(false);
                }
            }
        }

        private SaveUserRequest BuildRequest()
        {
            return new SaveUserRequest
            {
                UserName = _userNameTextBox.Text,
                DisplayName = _displayNameTextBox.Text,
                Password = _passwordTextBox.Text,
                UserType = Convert.ToString(_userTypeComboBox.SelectedItem),
                Status = Convert.ToString(_statusComboBox.SelectedItem),
                ActorUserName = _identity.UserName,
            };
        }

        private void UpdateStatistics()
        {
            _totalLabel.Text = _allUsers.Length.ToString();
            _activeLabel.Text = _allUsers.Count(user => string.Equals(user.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
            _inactiveLabel.Text = _allUsers.Count(user => !string.Equals(user.Status, "ATIVO", StringComparison.OrdinalIgnoreCase)).ToString();
        }

        private void ResetForm(bool clearSelection = true)
        {
            _selectedUserName = null;
            _userNameTextBox.ReadOnly = false;
            _userNameTextBox.Clear();
            _displayNameTextBox.Clear();
            _passwordTextBox.Clear();
            _statusComboBox.SelectedItem = "ATIVO";
            if (_userTypeComboBox.Items.Count > 0 && _userTypeComboBox.SelectedIndex < 0)
            {
                _userTypeComboBox.SelectedIndex = 0;
            }

            if (clearSelection)
            {
                _usersGrid.ClearSelection();
            }

            SetStatus("Preencha os dados do usuario.", false);
        }

        private void SelectInitialUserIfAny()
        {
            if (!string.IsNullOrWhiteSpace(_initialUserName))
            {
                SelectUser(_initialUserName);
            }
        }

        private void SelectUser(string userName)
        {
            foreach (DataGridViewRow row in _usersGrid.Rows)
            {
                if (row.DataBoundItem is UserSummary user && string.Equals(user.UserName, userName, StringComparison.OrdinalIgnoreCase))
                {
                    row.Selected = true;
                    _usersGrid.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        private UserSummary[] SortUsers(UserSummary[] users)
        {
            Func<UserSummary, object> keySelector;
            switch ((_sortColumn ?? string.Empty).ToLowerInvariant())
            {
                case "nome":
                    keySelector = user => user.DisplayName ?? string.Empty;
                    break;
                case "tipo":
                    keySelector = user => user.UserType ?? string.Empty;
                    break;
                case "status":
                    keySelector = user => user.Status ?? string.Empty;
                    break;
                case "versao":
                    keySelector = user => user.Version;
                    break;
                default:
                    keySelector = user => user.UserName ?? string.Empty;
                    break;
            }

            return (_sortAscending ? users.OrderBy(keySelector) : users.OrderByDescending(keySelector)).ToArray();
        }

        private void OnGridColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var column = _usersGrid.Columns[e.ColumnIndex].Name;
            if (string.Equals(_sortColumn, column, StringComparison.OrdinalIgnoreCase))
            {
                _sortAscending = !_sortAscending;
            }
            else
            {
                _sortColumn = column;
                _sortAscending = true;
            }

            RefreshGrid();
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                CreateUser();
            }
            else if (e.KeyCode == Keys.F3)
            {
                UpdateUser();
            }
            else if (e.KeyCode == Keys.F4)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                ResetForm();
            }
            else if (e.KeyCode == Keys.F6)
            {
                InactivateUser();
            }
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
    }
}
