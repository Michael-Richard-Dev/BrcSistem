using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class LoginForm : Form
    {
        private const string AppName = "BRCSISTEM";
        private const string AppVersion = "v3.1.20";

        private static readonly Color AzulPrincipal = Color.FromArgb(27, 54, 93);
        private static readonly Color AzulSecundario = Color.FromArgb(46, 89, 132);
        private static readonly Color AzulClaro = Color.FromArgb(74, 144, 194);
        private static readonly Color Branco = Color.White;
        private static readonly Color CinzaClaro = Color.FromArgb(248, 249, 250);
        private static readonly Color CinzaMedio = Color.FromArgb(233, 236, 239);
        private static readonly Color CinzaEscuro = Color.FromArgb(73, 80, 87);
        private static readonly Color VermelhoErro = Color.FromArgb(220, 53, 69);
        private static readonly Color Dourado = Color.FromArgb(204, 173, 0);

        private readonly CompositionRoot _compositionRoot;
        private readonly ConfigurationController _configurationController;
        private readonly AuthenticationController _authenticationController;

        private AppConfiguration _configuration;

        public LoginForm()
            : this(null, true)
        {
        }

        public LoginForm(CompositionRoot compositionRoot)
            : this(compositionRoot, false)
        {
        }

        private LoginForm(CompositionRoot compositionRoot, bool designerCtor)
        {
            _compositionRoot = compositionRoot;

            if (!designerCtor)
            {
                if (_compositionRoot == null)
                {
                    throw new ArgumentNullException(nameof(compositionRoot));
                }

                _configurationController = _compositionRoot.CreateConfigurationController();
                _authenticationController = _compositionRoot.CreateAuthenticationController();
            }

            InitializeComponent();
            WireEvents();
            ApplyRuntimeState();
        }

        private bool IsDesignModeActive
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                    || DesignMode
                    || Site != null && Site.DesignMode;
            }
        }

        private void WireEvents()
        {
            Load += OnLoad;
            _cacheIconButton.Click += CacheIconButton_Click;
            _configIconButton.Click += OpenProfilesManager;
            _loginButton.Click += LoginButton_Click;
            _requestAccessButton.Click += RequestAccessButton_Click;
            _closeButton.Click += CloseButton_Click;
            _userNameTextBox.KeyDown += UserNameTextBox_KeyDown;
            _passwordTextBox.KeyDown += PasswordTextBox_KeyDown;
            _userHostPanel.GotFocus += UserHostPanel_GotFocus;
            _passwordHostPanel.GotFocus += PasswordHostPanel_GotFocus;
        }

        private void ApplyRuntimeState()
        {
            _brandingTitleLabel.Text = AppName;
            _brandingVersionLabel.Text = "Versao " + AppVersion;
            _footerLabel.Text = " 2025 " + AppName + " - Todos os direitos reservados ";

            _logoPictureBox.Image = TryLoadLogo();
            _logoPictureBox.Visible = _logoPictureBox.Image != null;
        }

        private Image TryLoadLogo()
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var resourceName = asm.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith("logo.png", StringComparison.OrdinalIgnoreCase));
                if (resourceName != null)
                {
                    using (var stream = asm.GetManifestResourceStream(resourceName))
                    {
                        if (stream == null)
                        {
                            throw new InvalidOperationException("Recurso embutido '" + resourceName + "' nao pode ser lido.");
                        }

                        using (var original = Image.FromStream(stream, useEmbeddedColorManagement: false, validateImageData: true))
                        {
                            return new Bitmap(original);
                        }
                    }
                }

                var probe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "logo.png");
                if (File.Exists(probe))
                {
                    using (var original = Image.FromFile(probe))
                    {
                        return new Bitmap(original);
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (IsDesignModeActive)
            {
                return;
            }

            LoadConfiguration();
            _userNameTextBox.Focus();
        }

        private void CacheIconButton_Click(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            TryLogin();
        }

        private void RequestAccessButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "Solicitacao de acesso indisponivel nesta versao. Contate o administrador.",
                "Solicitar Acesso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UserNameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            _passwordTextBox.Focus();
            e.SuppressKeyPress = true;
        }

        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            TryLogin();
            e.SuppressKeyPress = true;
        }

        private void UserHostPanel_GotFocus(object sender, EventArgs e)
        {
            _userNameTextBox.Focus();
        }

        private void PasswordHostPanel_GotFocus(object sender, EventArgs e)
        {
            _passwordTextBox.Focus();
        }

        private void LoadConfiguration()
        {
            if (IsDesignModeActive || _configurationController == null)
            {
                SetStatus(string.Empty, false);
                return;
            }

            _configuration = _configurationController.LoadConfiguration();
            var profiles = _configuration.GetOrderedProfiles().ToArray();
            _profilesComboBox.DataSource = profiles;
            _profilesComboBox.DisplayMember = null;
            _profilesComboBox.ValueMember = nameof(DatabaseProfile.Id);

            if (profiles.Length > 0)
            {
                var activeIndex = Array.FindIndex(
                    profiles,
                    profile => string.Equals(profile.Id, _configuration.ActiveDatabaseId, StringComparison.OrdinalIgnoreCase));
                _profilesComboBox.SelectedIndex = activeIndex >= 0 ? activeIndex : 0;
                SetStatus(string.Empty, false);
            }
            else
            {
                SetStatus("Nenhum banco configurado. Use o icone de configuracao para adicionar um perfil.", true);
            }
        }

        private void OpenProfilesManager(object sender, EventArgs e)
        {
            if (IsDesignModeActive || _compositionRoot == null)
            {
                return;
            }

            using (var dialog = new DatabaseProfilesForm(_compositionRoot))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    LoadConfiguration();
                }
            }
        }

        private void TryLogin()
        {
            if (IsDesignModeActive || _authenticationController == null || _compositionRoot == null)
            {
                return;
            }

            if (_configuration == null)
            {
                LoadConfiguration();
            }

            if (_profilesComboBox.SelectedItem == null)
            {
                SetStatus("Selecione um banco configurado antes de continuar.", true);
                return;
            }

            var selectedProfile = (DatabaseProfile)_profilesComboBox.SelectedItem;
            LoginResult result;
            try
            {
                result = _authenticationController.Login(
                    _configuration,
                    selectedProfile.Id,
                    _userNameTextBox.Text,
                    _passwordTextBox.Text);
            }
            catch (Exception exception)
            {
                SetStatus("Falha ao autenticar: " + exception.Message, true);
                return;
            }

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                SetStatus(result.ErrorMessage, true);
                return;
            }

            if (result.RequiresPasswordChange)
            {
                using (var changePasswordForm = new ChangePasswordForm(
                    _compositionRoot,
                    _configuration,
                    result.DatabaseProfile,
                    result.Identity.UserName,
                    true))
                {
                    var dialogResult = changePasswordForm.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        MessageBox.Show(
                            this,
                            "Senha alterada. Faca login novamente com a nova senha.",
                            "Senha Atualizada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        _passwordTextBox.Clear();
                        _passwordTextBox.Focus();
                    }
                }

                return;
            }

            if (!result.Success)
            {
                SetStatus("Nao foi possivel concluir o login.", true);
                return;
            }

            var mainForm = new MainForm(_compositionRoot, result.Identity, result.DatabaseProfile);
            mainForm.FormClosed += MainForm_FormClosed;
            Hide();
            mainForm.Show();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }

        private static void OnProfileFormat(object sender, ListControlConvertEventArgs e)
        {
            var profile = e.ListItem as DatabaseProfile;
            if (profile == null)
            {
                return;
            }

            var kind = string.IsNullOrWhiteSpace(profile.Kind) ? "LOCAL" : profile.Kind.Trim().ToUpperInvariant();
            var name = string.IsNullOrWhiteSpace(profile.Name) ? profile.Id : profile.Name.Trim();
            e.Value = "[" + kind + "] " + name;
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? VermelhoErro : Color.SeaGreen;
        }
    }
}
