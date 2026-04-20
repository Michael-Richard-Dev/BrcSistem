using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class DatabaseProfilesForm : Form
    {
        private readonly ConfigurationController _configurationController;

        private AppConfiguration _configuration;
        private bool _hasChanges;

        public DatabaseProfilesForm(CompositionRoot compositionRoot)
        {
            _configurationController = compositionRoot.CreateConfigurationController();
            InitializeComponent();
            WireEvents();
        }

        private void WireEvents()
        {
            Load += DatabaseProfilesForm_Load;
            _profilesListBox.SelectedIndexChanged += ProfilesListBox_SelectedIndexChanged;
            _newButton.Click += NewButton_Click;
            _deleteButton.Click += DeleteSelectedProfile;
            _activateButton.Click += ActivateSelectedProfile;
            _saveButton.Click += SaveProfile;
            _testButton.Click += TestConnection;
            _closeButton.Click += CloseButton_Click;
        }

        private void DatabaseProfilesForm_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

        private void ProfilesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_profilesListBox.SelectedItem is DatabaseProfile profile)
            {
                PopulateForm(profile);
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = _hasChanges ? DialogResult.OK : DialogResult.Cancel;
            Close();
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
    }
}
