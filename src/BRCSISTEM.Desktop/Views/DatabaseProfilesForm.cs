using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class DatabaseProfilesForm : Form
    {
        private readonly ConfigurationController _configurationController;

        private AppConfiguration _configuration;
        private bool _hasChanges;
        private ListBox _profilesListBox;
        private TextBox _nameTextBox;
        private TextBox _descriptionTextBox;
        private TextBox _hostTextBox;
        private NumericUpDown _portNumericUpDown;
        private TextBox _databaseTextBox;
        private TextBox _userTextBox;
        private TextBox _passwordTextBox;
        private Label _statusLabel;

        public DatabaseProfilesForm(CompositionRoot compositionRoot)
        {
            _configurationController = compositionRoot.CreateConfigurationController();
            InitializeComponent();
            Load += (sender, args) => LoadConfiguration();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Gerenciar Bancos";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(900, 560);
            MinimumSize = new Size(900, 560);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(12),
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var leftPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
            };
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Perfis configurados",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 0, 0, 8),
            }, 0, 0);

            _profilesListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
            };
            _profilesListBox.SelectedIndexChanged += (sender, args) =>
            {
                if (_profilesListBox.SelectedItem is DatabaseProfile profile)
                {
                    PopulateForm(profile);
                }
            };
            leftPanel.Controls.Add(_profilesListBox, 0, 1);

            var listButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            var newButton = new Button { Text = "Novo", AutoSize = true, FlatStyle = FlatStyle.System };
            newButton.Click += (sender, args) => ClearForm();
            var deleteButton = new Button { Text = "Excluir", AutoSize = true, FlatStyle = FlatStyle.System };
            deleteButton.Click += DeleteSelectedProfile;
            var activateButton = new Button { Text = "Definir Ativo", AutoSize = true, FlatStyle = FlatStyle.System };
            activateButton.Click += ActivateSelectedProfile;
            listButtons.Controls.Add(newButton);
            listButtons.Controls.Add(deleteButton);
            listButtons.Controls.Add(activateButton);
            leftPanel.Controls.Add(listButtons, 0, 2);

            var rightPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                AutoScroll = true,
            };
            rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            AddField(rightPanel, 0, "Nome", out _nameTextBox);
            AddField(rightPanel, 1, "Descricao", out _descriptionTextBox);
            AddField(rightPanel, 2, "Host", out _hostTextBox);

            rightPanel.Controls.Add(CreateFieldLabel("Porta"), 0, 3);
            _portNumericUpDown = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 65535,
                Value = 5432,
                Width = 140,
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Top,
            };
            rightPanel.Controls.Add(_portNumericUpDown, 1, 3);

            AddField(rightPanel, 4, "Database", out _databaseTextBox);
            AddField(rightPanel, 5, "Usuario", out _userTextBox);
            AddField(rightPanel, 6, "Senha", out _passwordTextBox);
            _passwordTextBox.UseSystemPasswordChar = true;

            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.Firebrick,
                Margin = new Padding(0, 16, 0, 12),
            };
            rightPanel.Controls.Add(_statusLabel, 0, 7);
            rightPanel.SetColumnSpan(_statusLabel, 2);

            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0),
            };
            var saveButton = new Button { Text = "Salvar", AutoSize = true, FlatStyle = FlatStyle.System };
            saveButton.Click += SaveProfile;
            var testButton = new Button { Text = "Testar Conexao", AutoSize = true, FlatStyle = FlatStyle.System };
            testButton.Click += TestConnection;
            var closeButton = new Button { Text = "Fechar", AutoSize = true, FlatStyle = FlatStyle.System };
            closeButton.Click += (sender, args) =>
            {
                DialogResult = _hasChanges ? DialogResult.OK : DialogResult.Cancel;
                Close();
            };
            actions.Controls.Add(saveButton);
            actions.Controls.Add(testButton);
            actions.Controls.Add(closeButton);
            rightPanel.Controls.Add(actions, 0, 8);
            rightPanel.SetColumnSpan(actions, 2);

            root.Controls.Add(leftPanel, 0, 0);
            root.Controls.Add(rightPanel, 1, 0);
            Controls.Add(root);
        }

        private void LoadConfiguration()
        {
            _configuration = _configurationController.LoadConfiguration();
            RefreshProfileList();
            if (_profilesListBox.Items.Count == 0)
            {
                ClearForm();
            }
        }

        private void RefreshProfileList()
        {
            var profiles = _configuration.GetOrderedProfiles().ToArray();
            _profilesListBox.DataSource = profiles;
            _profilesListBox.DisplayMember = nameof(DatabaseProfile.DisplayName);
            if (profiles.Length > 0)
            {
                var selected = profiles.FirstOrDefault(profile => string.Equals(profile.Id, _configuration.ActiveDatabaseId, StringComparison.OrdinalIgnoreCase));
                _profilesListBox.SelectedItem = selected ?? profiles[0];
            }
        }

        private void PopulateForm(DatabaseProfile profile)
        {
            _nameTextBox.Text = profile.Name;
            _descriptionTextBox.Text = profile.Description;
            _hostTextBox.Text = profile.Host;
            _portNumericUpDown.Value = profile.Port <= 0 ? 5432 : profile.Port;
            _databaseTextBox.Text = profile.Database;
            _userTextBox.Text = profile.User;
            _passwordTextBox.Text = profile.Password;
            SetStatus("Perfil carregado.", false);
        }

        private void ClearForm()
        {
            _profilesListBox.ClearSelected();
            _nameTextBox.Text = string.Empty;
            _descriptionTextBox.Text = string.Empty;
            _hostTextBox.Text = string.Empty;
            _portNumericUpDown.Value = 5432;
            _databaseTextBox.Text = string.Empty;
            _userTextBox.Text = string.Empty;
            _passwordTextBox.Text = string.Empty;
            SetStatus("Preencha os dados do novo banco.", false);
        }

        private void SaveProfile(object sender, EventArgs e)
        {
            try
            {
                var selectedId = (_profilesListBox.SelectedItem as DatabaseProfile)?.Id;
                var profile = BuildProfileFromForm(selectedId);
                var profileId = _configuration.EnsureProfileId(profile);
                _configuration.DatabaseProfiles[profileId] = profile;
                if (string.IsNullOrWhiteSpace(_configuration.ActiveDatabaseId))
                {
                    _configuration.ActiveDatabaseId = profileId;
                }

                _configuration.IsConfigured = true;
                _configurationController.SaveConfiguration(_configuration);
                _hasChanges = true;
                LoadConfiguration();
                _profilesListBox.SelectedItem = _configuration.GetProfile(profileId);
                SetStatus("Perfil salvo com sucesso.", false);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
            }
        }

        private void TestConnection(object sender, EventArgs e)
        {
            try
            {
                var previewProfile = BuildProfileFromForm((_profilesListBox.SelectedItem as DatabaseProfile)?.Id);
                var result = _configurationController.TestConnection(_configuration, previewProfile);
                SetStatus(result.Message, !result.Success);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
            }
        }

        private void DeleteSelectedProfile(object sender, EventArgs e)
        {
            if (!(_profilesListBox.SelectedItem is DatabaseProfile profile))
            {
                SetStatus("Selecione um perfil para excluir.", true);
                return;
            }

            if (MessageBox.Show(this, $"Excluir o perfil '{profile.DisplayName}'?", "Confirmar Exclusao", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            _configuration.DatabaseProfiles.Remove(profile.Id);
            if (string.Equals(_configuration.ActiveDatabaseId, profile.Id, StringComparison.OrdinalIgnoreCase))
            {
                _configuration.ActiveDatabaseId = _configuration.DatabaseProfiles.Keys.FirstOrDefault();
            }

            _configuration.IsConfigured = _configuration.DatabaseProfiles.Count > 0;
            _configurationController.SaveConfiguration(_configuration);
            _hasChanges = true;
            LoadConfiguration();
            SetStatus("Perfil removido.", false);
        }

        private void ActivateSelectedProfile(object sender, EventArgs e)
        {
            if (!(_profilesListBox.SelectedItem is DatabaseProfile profile))
            {
                SetStatus("Selecione um perfil para ativar.", true);
                return;
            }

            _configuration.ActiveDatabaseId = profile.Id;
            _configurationController.SaveConfiguration(_configuration);
            _hasChanges = true;
            RefreshProfileList();
            SetStatus("Perfil ativo atualizado.", false);
        }

        private DatabaseProfile BuildProfileFromForm(string existingId)
        {
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                throw new InvalidOperationException("Informe o nome do perfil.");
            }

            if (string.IsNullOrWhiteSpace(_hostTextBox.Text) || string.IsNullOrWhiteSpace(_databaseTextBox.Text) || string.IsNullOrWhiteSpace(_userTextBox.Text))
            {
                throw new InvalidOperationException("Host, database e usuario sao obrigatorios.");
            }

            return new DatabaseProfile
            {
                Id = existingId,
                Name = _nameTextBox.Text.Trim(),
                Description = _descriptionTextBox.Text.Trim(),
                Host = _hostTextBox.Text.Trim(),
                Port = Convert.ToInt32(_portNumericUpDown.Value),
                Database = _databaseTextBox.Text.Trim(),
                User = _userTextBox.Text.Trim(),
                Password = _passwordTextBox.Text,
                Kind = IsLocalHost(_hostTextBox.Text) ? "local" : "rede",
            };
        }

        private static bool IsLocalHost(string host)
        {
            var normalized = (host ?? string.Empty).Trim().ToLowerInvariant();
            return normalized == "localhost" || normalized == "127.0.0.1" || normalized == "::1";
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }

        private static void AddField(TableLayoutPanel panel, int row, string label, out TextBox textBox)
        {
            panel.Controls.Add(CreateFieldLabel(label), 0, row);
            textBox = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10F),
            };
            panel.Controls.Add(textBox, 1, row);
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
