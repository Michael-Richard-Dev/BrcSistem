using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.NavegadorServidoresBancoDados
{
    public sealed partial class NavegadorServidoresBancoDadosForm : Form
    {
        private readonly ConfigurationController _configurationController;
        private readonly AppConfiguration _configuration;

        public string ResultMessage { get; private set; }

        public NavegadorServidoresBancoDadosForm(CompositionRoot compositionRoot, AppConfiguration configuration)
        {
            if (compositionRoot == null)
            {
                throw new ArgumentNullException(nameof(compositionRoot));
            }

            _configurationController = compositionRoot.CreateConfigurationController();
            _configuration = configuration ?? new AppConfiguration();

            InitializeComponent();
            WireEvents();
            LoadDefaults();
        }

        private void WireEvents()
        {
            _searchButton.Click += SearchButton_Click;
            _addSelectedButton.Click += AddSelectedButton_Click;
            _cancelButton.Click += CancelButton_Click;
        }

        private void LoadDefaults()
        {
            var profile = _configuration.GetActiveProfile() ?? _configuration.GetOrderedProfiles().FirstOrDefault();
            _hostTextBox.Text = profile?.Host ?? "localhost";
            _portTextBox.Text = profile?.Port > 0 ? profile.Port.ToString() : "5432";
            _userTextBox.Text = string.IsNullOrWhiteSpace(profile?.User) ? "postgres" : profile.User;
            _passwordTextBox.Text = profile?.Password ?? string.Empty;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            try
            {
                var host = _hostTextBox.Text.Trim();
                DatabaseServerSupport.ValidateHost(host);
                var port = DatabaseServerSupport.ParsePort(_portTextBox.Text);
                var user = _userTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(user))
                {
                    throw new InvalidOperationException("Informe o usuario PostgreSQL.");
                }

                SetStatus("Conectando ao servidor e listando bancos...", false);
                System.Windows.Forms.Application.DoEvents();

                var databases = DatabaseServerSupport.ListDatabases(host, port, user, _passwordTextBox.Text);
                _resultsCheckedListBox.Items.Clear();
                foreach (var database in databases)
                {
                    _resultsCheckedListBox.Items.Add(database, false);
                }

                _addSelectedButton.Enabled = databases.Count > 0;
                SetStatus(databases.Count == 0
                    ? "Nenhum banco de usuario foi encontrado no servidor."
                    : databases.Count + " banco(s) encontrado(s).", false);
            }
            catch (Exception exception)
            {
                _addSelectedButton.Enabled = false;
                SetStatus(exception.Message, true);
            }
        }

        private void AddSelectedButton_Click(object sender, EventArgs e)
        {
            if (_resultsCheckedListBox.CheckedItems.Count == 0)
            {
                SetStatus("Selecione pelo menos um banco para adicionar.", true);
                return;
            }

            try
            {
                var host = _hostTextBox.Text.Trim();
                var port = DatabaseServerSupport.ParsePort(_portTextBox.Text);
                var user = _userTextBox.Text.Trim();
                var password = _passwordTextBox.Text;
                var added = 0;

                foreach (var item in _resultsCheckedListBox.CheckedItems)
                {
                    var databaseName = Convert.ToString(item);
                    var profile = new DatabaseProfile
                    {
                        Id = DatabaseServerSupport.BuildUniqueProfileId(_configuration, databaseName),
                        Name = databaseName,
                        Description = "Descoberto em " + host,
                        Host = host,
                        Port = port,
                        Database = databaseName,
                        User = user,
                        Password = password,
                        Kind = DatabaseServerSupport.IsLocalHost(host) ? "local" : "rede",
                    };

                    _configuration.DatabaseProfiles[profile.Id] = profile;
                    added++;
                }

                if (string.IsNullOrWhiteSpace(_configuration.ActiveDatabaseId) && _configuration.DatabaseProfiles.Count > 0)
                {
                    _configuration.ActiveDatabaseId = _configuration.DatabaseProfiles.Keys.First();
                }

                _configuration.IsConfigured = _configuration.DatabaseProfiles.Count > 0;
                _configurationController.SaveConfiguration(_configuration);
                ResultMessage = added + " banco(s) adicionado(s) com sucesso.";
                DialogResult = DialogResult.OK;
                Close();
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

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }
    }
}
