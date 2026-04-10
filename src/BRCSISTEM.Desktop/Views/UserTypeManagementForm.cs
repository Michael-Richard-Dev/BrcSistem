using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class UserTypeManagementForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly AdministrationController _administrationController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private DataGridView _typesGrid;
        private TextBox _nameTextBox;
        private TextBox _descriptionTextBox;
        private Label _statusLabel;
        private readonly Dictionary<string, CheckBox> _permissionCheckBoxes;
        private bool _editingMode;
        private string _selectedTypeName;
        private string _originalTypeName;
        private string _originalDescription;
        private bool _hasChanges;

        public UserTypeManagementForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _administrationController = compositionRoot.CreateAdministrationController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _permissionCheckBoxes = new Dictionary<string, CheckBox>(StringComparer.OrdinalIgnoreCase);

            InitializeComponent();
            Load += (sender, args) => LoadData();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Tipos de Usuario";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1320, 860);
            MinimumSize = new Size(1240, 760);
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

            var titlePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                AutoSize = true,
            };
            titlePanel.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Gestao de Tipos de Usuario",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 0, 0, 4),
            }, 0, 0);
            titlePanel.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Cadastro e manutencao de perfis de acesso do sistema",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(73, 80, 87),
                Margin = new Padding(0, 0, 0, 8),
            }, 0, 1);

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));

            content.Controls.Add(BuildTypesListPanel(), 0, 0);
            content.Controls.Add(BuildEditorPanel(), 1, 0);

            var footer = BuildFooter();

            root.Controls.Add(titlePanel, 0, 0);
            root.Controls.Add(content, 0, 1);
            root.Controls.Add(footer, 0, 2);
            Controls.Add(root);
        }

        private Control BuildTypesListPanel()
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Tipos de Usuario Cadastrados",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            _typesGrid = new DataGridView
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
            _typesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = nameof(UserTypeSummary.Name), Width = 140 });
            _typesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "nome", HeaderText = "Nome do Tipo", DataPropertyName = nameof(UserTypeSummary.Name), Width = 180 });
            _typesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "descricao", HeaderText = "Descricao", DataPropertyName = nameof(UserTypeSummary.Description), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _typesGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "permissoes", HeaderText = "Permissoes", Width = 110 });
            _typesGrid.SelectionChanged += (sender, args) => LoadSelectedType();
            _typesGrid.CellDoubleClick += (sender, args) => LoadSelectedType();

            group.Controls.Add(_typesGrid);
            return group;
        }

        private Control BuildEditorPanel()
        {
            var group = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Dados do Tipo de Usuario",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                Padding = new Padding(10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var fields = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                AutoSize = true,
            };
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            fields.Controls.Add(CreateFieldLabel("Nome do Tipo:"), 0, 0);
            _nameTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            fields.Controls.Add(_nameTextBox, 1, 0);

            fields.Controls.Add(CreateFieldLabel("Descricao:"), 0, 1);
            _descriptionTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 10F) };
            fields.Controls.Add(_descriptionTextBox, 1, 1);

            var permissionHeader = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                AutoSize = true,
                Margin = new Padding(0, 12, 0, 8),
            };
            permissionHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            permissionHeader.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            permissionHeader.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Permissoes de Acesso as Telas",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
            }, 0, 0);
            var permissionButtons = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Right,
            };
            var selectAllButton = new Button { Text = "Selecionar Tudo", AutoSize = true, FlatStyle = FlatStyle.System };
            selectAllButton.Click += (sender, args) => SetAllPermissions(true);
            var clearAllButton = new Button { Text = "Desmarcar Tudo", AutoSize = true, FlatStyle = FlatStyle.System };
            clearAllButton.Click += (sender, args) => SetAllPermissions(false);
            permissionButtons.Controls.Add(selectAllButton);
            permissionButtons.Controls.Add(clearAllButton);
            permissionHeader.Controls.Add(permissionButtons, 1, 0);

            _statusLabel = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.SeaGreen,
                Margin = new Padding(0, 0, 0, 6),
            };

            var permissionsHost = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
            };
            BuildPermissionGroups(permissionsHost);

            root.Controls.Add(fields, 0, 0);
            root.Controls.Add(permissionHeader, 0, 1);
            root.Controls.Add(_statusLabel, 0, 2);
            root.Controls.Add(permissionsHost, 0, 3);

            group.Controls.Add(root);
            return group;
        }

        private Control BuildFooter()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                AutoSize = true,
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var leftButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            var newButton = new Button { Text = "Novo (F3)", AutoSize = true, FlatStyle = FlatStyle.System };
            newButton.Click += (sender, args) => ResetForm();
            var saveButton = new Button { Text = "Salvar (F2)", AutoSize = true, FlatStyle = FlatStyle.System };
            saveButton.Click += (sender, args) => SaveType();
            var clearButton = new Button { Text = "Limpar (F5)", AutoSize = true, FlatStyle = FlatStyle.System };
            clearButton.Click += (sender, args) => ResetForm();
            var deleteButton = new Button { Text = "Excluir (Del)", AutoSize = true, FlatStyle = FlatStyle.System };
            deleteButton.Click += (sender, args) => DeleteType();
            leftButtons.Controls.Add(newButton);
            leftButtons.Controls.Add(saveButton);
            leftButtons.Controls.Add(clearButton);
            leftButtons.Controls.Add(deleteButton);

            var rightButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            var usersButton = new Button { Text = "Ver Usuarios", AutoSize = true, FlatStyle = FlatStyle.System };
            usersButton.Click += (sender, args) => OpenUsersByType();
            var closeButton = new Button { Text = "Fechar (F4)", AutoSize = true, FlatStyle = FlatStyle.System };
            closeButton.Click += (sender, args) =>
            {
                DialogResult = _hasChanges ? DialogResult.OK : DialogResult.Cancel;
                Close();
            };
            rightButtons.Controls.Add(usersButton);
            rightButtons.Controls.Add(closeButton);

            root.Controls.Add(leftButtons, 0, 0);
            root.Controls.Add(rightButtons, 1, 0);

            KeyPreview = true;
            KeyDown += OnFormKeyDown;
            return root;
        }

        private void BuildPermissionGroups(Control host)
        {
            var categories = _administrationController.LoadPermissionCategories();
            foreach (var category in categories)
            {
                var categoryPanel = new TableLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Width = 640,
                    Margin = new Padding(0, 0, 0, 10),
                    ColumnCount = 1,
                };

                var header = new TableLayoutPanel
                {
                    AutoSize = true,
                    ColumnCount = 2,
                    Dock = DockStyle.Top,
                };
                header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                header.Controls.Add(new Label
                {
                    AutoSize = true,
                    Text = category.Name + " (" + category.Permissions.Length + " itens)",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(54, 79, 107),
                    Margin = new Padding(0, 0, 0, 6),
                }, 0, 0);

                var groupButtons = new FlowLayoutPanel
                {
                    AutoSize = true,
                    FlowDirection = FlowDirection.LeftToRight,
                    Dock = DockStyle.Right,
                };
                var selectGroupButton = new Button { Text = "Selecionar Grupo", AutoSize = true, FlatStyle = FlatStyle.System };
                selectGroupButton.Click += (sender, args) => SetCategoryPermissions(category, true);
                var clearGroupButton = new Button { Text = "Desmarcar Grupo", AutoSize = true, FlatStyle = FlatStyle.System };
                clearGroupButton.Click += (sender, args) => SetCategoryPermissions(category, false);
                groupButtons.Controls.Add(selectGroupButton);
                groupButtons.Controls.Add(clearGroupButton);
                header.Controls.Add(groupButtons, 1, 0);

                var grid = new TableLayoutPanel
                {
                    AutoSize = true,
                    ColumnCount = 2,
                    Dock = DockStyle.Top,
                };
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

                for (var index = 0; index < category.Permissions.Length; index++)
                {
                    var permission = category.Permissions[index];
                    var checkBox = new CheckBox
                    {
                        AutoSize = true,
                        Text = permission.Title,
                        Tag = permission.Key,
                        Font = new Font("Segoe UI", 9F),
                        Margin = new Padding(3, 2, 12, 2),
                    };
                    _permissionCheckBoxes[permission.Key] = checkBox;
                    grid.Controls.Add(checkBox, index % 2, index / 2);
                }

                categoryPanel.Controls.Add(header, 0, 0);
                categoryPanel.Controls.Add(grid, 0, 1);
                host.Controls.Add(categoryPanel);
            }
        }

        private void LoadData()
        {
            _configuration = _configurationController.LoadConfiguration();
            LoadTypesGrid();
            ResetForm();
        }

        private void LoadTypesGrid()
        {
            var items = _administrationController.LoadUserTypes(_configuration, _databaseProfile)
                .Select(item => new
                {
                    item.Name,
                    item.Description,
                    PermissionCountText = item.PermissionCount + "/" + item.TotalPermissionCount
                })
                .ToArray();

            _typesGrid.Rows.Clear();
            foreach (var item in items)
            {
                _typesGrid.Rows.Add(item.Name, item.Name, item.Description, item.PermissionCountText);
            }
        }

        private void LoadSelectedType()
        {
            if (_typesGrid.CurrentRow == null)
            {
                return;
            }

            var typeName = Convert.ToString(_typesGrid.CurrentRow.Cells[0].Value);
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return;
            }

            var detail = _administrationController.LoadUserType(_configuration, _databaseProfile, typeName);
            if (detail == null)
            {
                SetStatus("Registro nao encontrado. A lista sera recarregada.", true);
                LoadTypesGrid();
                ResetForm();
                return;
            }

            _editingMode = true;
            _selectedTypeName = detail.Name;
            _originalTypeName = detail.Name;
            _originalDescription = detail.Description ?? string.Empty;

            _nameTextBox.Text = detail.Name ?? string.Empty;
            _descriptionTextBox.Text = detail.Description ?? string.Empty;
            SetAllPermissions(false);

            foreach (var key in detail.PermissionKeys ?? Array.Empty<string>())
            {
                if (_permissionCheckBoxes.TryGetValue(key, out var checkBox))
                {
                    checkBox.Checked = true;
                }
            }

            SetStatus("Tipo carregado para edicao.", false);
        }

        private void SaveType()
        {
            try
            {
                var request = new SaveUserTypeRequest
                {
                    OriginalName = _editingMode ? _selectedTypeName : null,
                    Name = _nameTextBox.Text,
                    Description = _descriptionTextBox.Text,
                    PermissionKeys = _permissionCheckBoxes.Where(item => item.Value.Checked).Select(item => item.Key).ToArray(),
                    ActorUserName = _identity.UserName,
                };

                if (_editingMode && RequiresImpactConfirmation())
                {
                    var affectedUsers = _administrationController.CountActiveUsersForType(_configuration, _databaseProfile, _selectedTypeName);
                    if (affectedUsers > 0)
                    {
                        var message =
                            "O tipo de usuario sera alterado.\n\n" +
                            "Isso criara uma nova versao de " + affectedUsers + " usuario(s) ativo(s)\n" +
                            "para manter o historico completo.\n\n" +
                            "Deseja continuar?";
                        if (MessageBox.Show(this, message, "Confirmar Alteracao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            return;
                        }
                    }
                }

                var result = _administrationController.SaveUserType(_configuration, _databaseProfile, request);
                _hasChanges = true;
                LoadTypesGrid();
                ResetForm();

                var successMessage = result.IsNewRecord
                    ? "Tipo '" + result.SavedName + "' cadastrado com sucesso!"
                    : "Tipo '" + result.SavedName + "' atualizado com sucesso!";
                if (!result.IsNewRecord && result.UpdatedUsersCount > 0)
                {
                    successMessage += "\n\n" + result.UpdatedUsersCount + " usuario(s) atualizado(s) automaticamente.";
                }

                SetStatus(successMessage, false);
                MessageBox.Show(this, successMessage, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
                MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteType()
        {
            if (!_editingMode || string.IsNullOrWhiteSpace(_selectedTypeName))
            {
                SetStatus("Selecione um tipo para excluir.", true);
                return;
            }

            var message =
                "Deseja realmente excluir o tipo '" + _selectedTypeName + "'?\n\n" +
                "ATENCAO: Esta acao nao pode ser desfeita!";
            if (MessageBox.Show(this, message, "Confirmar Exclusao", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _administrationController.DeleteUserType(_configuration, _databaseProfile, _identity.UserName, _selectedTypeName);
                _hasChanges = true;
                LoadTypesGrid();
                ResetForm();
                SetStatus("Tipo excluido com sucesso.", false);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
                MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenUsersByType()
        {
            if (!_editingMode || string.IsNullOrWhiteSpace(_selectedTypeName))
            {
                SetStatus("Selecione um tipo de usuario para consultar.", true);
                return;
            }

            var users = _administrationController.LoadUsersByType(_configuration, _databaseProfile, _selectedTypeName);
            if (users.Length == 0)
            {
                MessageBox.Show(this, "Nenhum usuario encontrado do tipo '" + _selectedTypeName + "'.", "Consulta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new UsersByTypeForm(_selectedTypeName, users))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedUserName))
                {
                    using (var userForm = new UserManagementForm(_compositionRoot, _identity, _databaseProfile, dialog.SelectedUserName))
                    {
                        userForm.ShowDialog(this);
                    }
                }
            }
        }

        private void ResetForm()
        {
            _editingMode = false;
            _selectedTypeName = null;
            _originalTypeName = null;
            _originalDescription = string.Empty;
            _nameTextBox.Clear();
            _descriptionTextBox.Clear();
            SetAllPermissions(false);
            _typesGrid.ClearSelection();
            SetStatus("Preencha os dados do tipo de usuario.", false);
        }

        private bool RequiresImpactConfirmation()
        {
            var currentName = (_nameTextBox.Text ?? string.Empty).Trim();
            var currentDescription = (_descriptionTextBox.Text ?? string.Empty).Trim();
            return !string.Equals(currentName, _originalTypeName ?? string.Empty, StringComparison.Ordinal)
                || !string.Equals(currentDescription, _originalDescription ?? string.Empty, StringComparison.Ordinal);
        }

        private void SetAllPermissions(bool selected)
        {
            foreach (var checkBox in _permissionCheckBoxes.Values)
            {
                checkBox.Checked = selected;
            }
        }

        private void SetCategoryPermissions(PermissionCategory category, bool selected)
        {
            foreach (var permission in category.Permissions)
            {
                if (_permissionCheckBoxes.TryGetValue(permission.Key, out var checkBox))
                {
                    checkBox.Checked = selected;
                }
            }
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
                SaveType();
            }
            else if (e.KeyCode == Keys.F3)
            {
                ResetForm();
            }
            else if (e.KeyCode == Keys.F4)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                ResetForm();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteType();
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
                Margin = new Padding(0, 6, 0, 4),
            };
        }
    }
}
