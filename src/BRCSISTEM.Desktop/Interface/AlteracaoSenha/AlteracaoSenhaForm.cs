using System;
using System.ComponentModel;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.AlteracaoSenha
{
    public sealed partial class AlteracaoSenhaForm : Form
    {
        private readonly AuthenticationController _authenticationController;
        private readonly AppConfiguration _configuration;
        private readonly DatabaseProfile _databaseProfile;
        private readonly string _userName;
        private readonly bool _forceReset;
        private readonly bool _isDesignerInstance;

        public AlteracaoSenhaForm()
        {
            _userName = string.Empty;
            _forceReset = false;
            _isDesignerInstance = true;

            InitializeComponent();
        }

        public AlteracaoSenhaForm(CompositionRoot compositionRoot, AppConfiguration configuration, DatabaseProfile databaseProfile, string userName, bool forceReset)
        {
            if (compositionRoot == null) throw new ArgumentNullException(nameof(compositionRoot));

            _authenticationController = compositionRoot.CreateAuthenticationController();
            _configuration = configuration;
            _databaseProfile = databaseProfile;
            _userName = userName;
            _forceReset = forceReset;

            InitializeComponent();
            ApplyRuntimeTexts();
        }

        private bool IsDesignModeActive
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                    || _isDesignerInstance
                    || DesignMode
                    || (Site != null && Site.DesignMode);
            }
        }

        private void ApplyRuntimeTexts()
        {
            _instructionLabel.Text = _forceReset
                ? "Senha padrao detectada. Defina uma nova senha para continuar."
                : "Altere a senha do usuario atual.";
            _instructionLabel.ForeColor = _forceReset
                ? System.Drawing.Color.DarkOrange
                : System.Drawing.Color.FromArgb(27, 54, 93);
            _userNameValueLabel.Text = _userName ?? string.Empty;
            _cancelButton.Text = _forceReset ? "Cancelar" : "Fechar";
        }

        private void OnSaveButtonClick(object sender, EventArgs e)
        {
            SavePassword();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void SavePassword()
        {
            if (IsDesignModeActive)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_newPasswordTextBox.Text) || string.IsNullOrWhiteSpace(_confirmPasswordTextBox.Text))
            {
                SetStatus("Preencha a nova senha e a confirmacao.", true);
                return;
            }

            if (!string.Equals(_newPasswordTextBox.Text, _confirmPasswordTextBox.Text, StringComparison.Ordinal))
            {
                SetStatus("A confirmacao nao confere com a nova senha.", true);
                return;
            }

            var result = _authenticationController.ChangePassword(_configuration, _databaseProfile, _userName, _newPasswordTextBox.Text);
            SetStatus(result.Message, !result.Success);
            if (result.Success)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? System.Drawing.Color.Firebrick : System.Drawing.Color.SeaGreen;
        }
    }
}
