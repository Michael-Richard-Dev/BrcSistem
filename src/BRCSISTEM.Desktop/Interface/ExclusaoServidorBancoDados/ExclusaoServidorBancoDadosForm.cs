using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.ExclusaoServidorBancoDados
{
    public sealed partial class ExclusaoServidorBancoDadosForm : Form
    {
        private readonly ConfigurationController _configurationController;
        private readonly AppConfiguration _configuration;

        public string ResultMessage { get; private set; }

        public ExclusaoServidorBancoDadosForm()
        {
            InitializeComponent();
        }

        public ExclusaoServidorBancoDadosForm(CompositionRoot compositionRoot, AppConfiguration configuration)
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
            _listButton.Click += ListButton_Click;
            _deleteButton.Click += DeleteButton_Click;
            _cancelButton.Click += CancelButton_Click;
        }

        private void LoadDefaults()
        {
            var profile = _configuration.GetActiveProfile() ?? _configuration.GetOrderedProfiles().FirstOrDefault();
            _hostTextBox.Text = profile?.Host ?? "localhost";
            _portTextBox.Text = profile?.Port > 0 ? profile.Port.ToString() : "5432";
            _adminUserTextBox.Text = string.IsNullOrWhiteSpace(profile?.User) ? "postgres" : profile.User;
            _adminPasswordTextBox.Text = profile?.Password ?? string.Empty;
        }

        private void ListButton_Click(object sender, EventArgs e)
        {
            try
            {
                var host = _hostTextBox.Text.Trim();
                SuporteServidorBancoDados.ValidateHost(host);
                var port = SuporteServidorBancoDados.ParsePort(_portTextBox.Text);
                var user = _adminUserTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(user))
                {
                    throw new InvalidOperationException("Informe o usuario administrador.");
                }

                SetStatus("Conectando ao servidor...", false);
                System.Windows.Forms.Application.DoEvents();

                var databases = SuporteServidorBancoDados.ListDatabases(host, port, user, _adminPasswordTextBox.Text);
                _databasesListBox.Items.Clear();
                foreach (var database in databases)
                {
                    _databasesListBox.Items.Add(database);
                }

                _deleteButton.Enabled = databases.Count > 0;
                SetStatus(databases.Count == 0
                    ? "Nenhum banco de usuario foi encontrado."
                    : databases.Count + " banco(s) disponivel(is) para exclusao.", false);
            }
            catch (Exception exception)
            {
                _deleteButton.Enabled = false;
                SetStatus(exception.Message, true);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (_databasesListBox.SelectedItem == null)
            {
                SetStatus("Selecione um banco para excluir.", true);
                return;
            }

            var databaseName = Convert.ToString(_databasesListBox.SelectedItem);
            var host = _hostTextBox.Text.Trim();
            var port = SuporteServidorBancoDados.ParsePort(_portTextBox.Text);
            var user = _adminUserTextBox.Text.Trim();
            var password = _adminPasswordTextBox.Text;

            var firstConfirmation = MessageBox.Show(
                this,
                "Voce esta prestes a EXCLUIR o banco '" + databaseName + "'.\r\n\r\n" +
                "Esta acao remove tabelas e dados do servidor e nao pode ser desfeita.\r\n\r\n" +
                "Deseja continuar?",
                "Confirmacao Critica 1/3",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (firstConfirmation != DialogResult.Yes)
            {
                return;
            }

            var secondConfirmation = MessageBox.Show(
                this,
                "Segunda confirmacao:\r\n\r\n" +
                "Banco: " + databaseName + "\r\n" +
                "Servidor: " + host + ":" + port + "\r\n\r\n" +
                "Confirme que o backup foi realizado e que voce selecionou o banco correto.",
                "Confirmacao Critica 2/3",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (secondConfirmation != DialogResult.Yes)
            {
                return;
            }

            var typedConfirmation = SuporteServidorBancoDados.ShowTypedConfirmation(
                this,
                "Confirmacao Final 3/3",
                "Digite exatamente o nome do banco para confirmar a exclusao:\r\n\r\n" + databaseName,
                databaseName);

            if (typedConfirmation == DialogResult.Cancel)
            {
                return;
            }

            if (typedConfirmation != DialogResult.OK)
            {
                SetStatus("A exclusao foi cancelada porque o nome informado nao confere.", true);
                return;
            }

            try
            {
                SetStatus("Excluindo banco do servidor...", false);
                System.Windows.Forms.Application.DoEvents();

                SuporteServidorBancoDados.DropDatabase(host, port, user, password, databaseName);
                ResultMessage = "Banco '" + databaseName + "' excluido do servidor.";

                foreach (var profile in _configuration.DatabaseProfiles.Values.Where(p =>
                    string.Equals(p.Database, databaseName, StringComparison.OrdinalIgnoreCase)).ToArray())
                {
                    // Mantemos as configuracoes existentes para o usuario decidir se deseja remove-las.
                }

                _configurationController.SaveConfiguration(_configuration);
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
