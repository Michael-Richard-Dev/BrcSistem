using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.CriacaoServidorBancoDados
{
    public sealed partial class CriacaoServidorBancoDadosForm : Form
    {
        private readonly ConfigurationController _configurationController;
        private readonly AppConfiguration _configuration;

        public string ResultMessage { get; private set; }

        public CriacaoServidorBancoDadosForm(CompositionRoot compositionRoot, AppConfiguration configuration)
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
            _createButton.Click += CreateButton_Click;
            _cancelButton.Click += CancelButton_Click;
        }

        private void LoadDefaults()
        {
            var profile = _configuration.GetActiveProfile() ?? _configuration.GetOrderedProfiles().FirstOrDefault();
            _hostTextBox.Text = profile?.Host ?? "localhost";
            _portTextBox.Text = profile?.Port > 0 ? profile.Port.ToString() : "5432";
            _adminUserTextBox.Text = string.IsNullOrWhiteSpace(profile?.User) ? "postgres" : profile.User;
            _adminPasswordTextBox.Text = profile?.Password ?? string.Empty;
            _addToListCheckBox.Checked = true;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            try
            {
                var host = _hostTextBox.Text.Trim();
                DatabaseServerSupport.ValidateHost(host);
                var port = DatabaseServerSupport.ParsePort(_portTextBox.Text);
                var user = _adminUserTextBox.Text.Trim();
                var password = _adminPasswordTextBox.Text;
                var databaseName = _databaseNameTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(user))
                {
                    throw new InvalidOperationException("Informe o usuario administrador.");
                }

                DatabaseServerSupport.ValidateDatabaseName(databaseName);

                var confirmation = MessageBox.Show(
                    this,
                    "Criar banco '" + databaseName + "'?\r\n\r\n" +
                    "Servidor: " + host + "\r\n" +
                    "Porta: " + port + "\r\n\r\n" +
                    "O banco sera criado vazio no PostgreSQL.",
                    "Confirmar Criacao",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmation != DialogResult.Yes)
                {
                    return;
                }

                SetStatus("Criando banco no servidor...", false);
                System.Windows.Forms.Application.DoEvents();

                DatabaseServerSupport.CreateDatabase(host, port, user, password, databaseName);

                if (_addToListCheckBox.Checked)
                {
                    var profile = new DatabaseProfile
                    {
                        Id = DatabaseServerSupport.BuildUniqueProfileId(_configuration, databaseName),
                        Name = databaseName,
                        Description = "Criado em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        Host = host,
                        Port = port,
                        Database = databaseName,
                        User = user,
                        Password = password,
                        Kind = DatabaseServerSupport.IsLocalHost(host) ? "local" : "rede",
                    };

                    _configuration.DatabaseProfiles[profile.Id] = profile;
                    if (string.IsNullOrWhiteSpace(_configuration.ActiveDatabaseId))
                    {
                        _configuration.ActiveDatabaseId = profile.Id;
                    }

                    _configuration.IsConfigured = _configuration.DatabaseProfiles.Count > 0;
                    _configurationController.SaveConfiguration(_configuration);
                    ResultMessage = "Banco criado e adicionado a lista de configuracoes.";
                }
                else
                {
                    ResultMessage = "Banco criado com sucesso. Use 'Buscar e Adicionar' para registra-lo.";
                }

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
