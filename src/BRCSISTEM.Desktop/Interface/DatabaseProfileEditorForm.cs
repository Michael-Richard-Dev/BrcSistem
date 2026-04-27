using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class DatabaseProfileEditorForm : Form
    {
        private readonly ConfigurationController _configurationController;
        private readonly AppConfiguration _configuration;
        private readonly DatabaseProfile _originalProfile;

        public string SavedProfileId { get; private set; }

        public DatabaseProfileEditorForm(CompositionRoot compositionRoot, AppConfiguration configuration, DatabaseProfile profile)
        {
            if (compositionRoot == null)
            {
                throw new ArgumentNullException(nameof(compositionRoot));
            }

            _configurationController = compositionRoot.CreateConfigurationController();
            _configuration = configuration ?? new AppConfiguration();
            _originalProfile = profile == null ? null : profile.Clone();

            InitializeComponent();
            WireEvents();
            LoadProfile();
        }

        private void WireEvents()
        {
            _saveButton.Click += SaveButton_Click;
            _testButton.Click += TestButton_Click;
            _cancelButton.Click += CancelButton_Click;
        }

        private void LoadProfile()
        {
            var isEditing = _originalProfile != null;
            Text = isEditing ? "Editar Banco" : "Novo Banco";
            _headerLabel.Text = isEditing ? "Editar Configuracao" : "Nova Configuracao";

            var profile = _originalProfile ?? new DatabaseProfile
            {
                Host = "localhost",
                Port = 5432,
                User = "postgres",
            };

            _nameTextBox.Text = profile.Name ?? string.Empty;
            _descriptionTextBox.Text = profile.Description ?? string.Empty;
            _hostTextBox.Text = string.IsNullOrWhiteSpace(profile.Host) ? "localhost" : profile.Host;
            _portNumericUpDown.Value = profile.Port <= 0 ? 5432 : profile.Port;
            _databaseTextBox.Text = profile.Database ?? string.Empty;
            _userTextBox.Text = string.IsNullOrWhiteSpace(profile.User) ? "postgres" : profile.User;
            _passwordTextBox.Text = profile.Password ?? string.Empty;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var profile = BuildProfileFromForm();
                var profileId = _configuration.EnsureProfileId(profile);
                _configuration.DatabaseProfiles[profileId] = profile;

                if (_originalProfile != null
                    && !string.IsNullOrWhiteSpace(_originalProfile.Id)
                    && !string.Equals(_originalProfile.Id, profileId, StringComparison.OrdinalIgnoreCase))
                {
                    _configuration.DatabaseProfiles.Remove(_originalProfile.Id);
                    if (string.Equals(_configuration.ActiveDatabaseId, _originalProfile.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        _configuration.ActiveDatabaseId = profileId;
                    }
                }

                if (string.IsNullOrWhiteSpace(_configuration.ActiveDatabaseId))
                {
                    _configuration.ActiveDatabaseId = profileId;
                }

                _configuration.IsConfigured = true;
                _configurationController.SaveConfiguration(_configuration);
                SavedProfileId = profileId;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
            }
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            try
            {
                var previewProfile = BuildProfileFromForm();
                var result = _configurationController.TestConnection(_configuration, previewProfile);
                SetStatus(result.Message, !result.Success);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private DatabaseProfile BuildProfileFromForm()
        {
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                throw new InvalidOperationException("Informe o nome do perfil.");
            }

            if (string.IsNullOrWhiteSpace(_hostTextBox.Text))
            {
                throw new InvalidOperationException("Informe o host do servidor.");
            }

            if (string.IsNullOrWhiteSpace(_databaseTextBox.Text))
            {
                throw new InvalidOperationException("Informe o nome do database.");
            }

            if (string.IsNullOrWhiteSpace(_userTextBox.Text))
            {
                throw new InvalidOperationException("Informe o usuario PostgreSQL.");
            }

            var host = _hostTextBox.Text.Trim();
            return new DatabaseProfile
            {
                Id = _originalProfile?.Id,
                Name = _nameTextBox.Text.Trim(),
                Description = _descriptionTextBox.Text.Trim(),
                Host = host,
                Port = Decimal.ToInt32(_portNumericUpDown.Value),
                Database = _databaseTextBox.Text.Trim(),
                User = _userTextBox.Text.Trim(),
                Password = _passwordTextBox.Text,
                Kind = IsLocalHost(host) ? "local" : "rede",
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
