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

namespace BRCSISTEM.Desktop.Interface
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
        private EnvironmentOption[] _environmentOptions = Array.Empty<EnvironmentOption>();

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
            _leftPanel.Resize += LeftPanel_Resize;
            _rightContentPanel.Resize += RightContentPanel_Resize;
            _cacheIconButton.Click += CacheIconButton_Click;
            _configIconButton.Click += OpenProfilesManager;
            _loginButton.Click += LoginButton_Click;
            _requestAccessButton.Click += RequestAccessButton_Click;
            _closeButton.Click += CloseButton_Click;
            _profilesComboBox.SelectedIndexChanged += ProfilesComboBox_SelectedIndexChanged;
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
            _bancoLabel.Text = "Ambiente";

            ApplyToolbarImages();
            _logoPictureBox.Image = TryLoadLogo();
            _logoPictureBox.Visible = _logoPictureBox.Image != null;
            _cacheIconButton.Visible = false;
            _configIconButton.Visible = false;

            CenterLeftContent();
            CenterFormContent();
        }

        private void ApplyToolbarImages()
        {
            var broomImage = TryLoadEmbeddedImage("vassoura.png");
            if (broomImage != null)
            {
                _cacheIconButton.Image = broomImage;
            }

            var configImage = TryLoadEmbeddedImage("configuracoes-cog.png");
            if (configImage != null)
            {
                _configIconButton.Image = configImage;
            }
        }

        private Image TryLoadLogo()
        {
            return TryLoadEmbeddedImage("logo.png") ?? TryLoadImageFromOutput("Assets", "logo.png");
        }

        private Image TryLoadEmbeddedImage(string fileName)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var resourceName = asm.GetManifestResourceNames()
                    .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
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
            }
            catch
            {
            }

            return null;
        }

        private static Image TryLoadImageFromOutput(string folder, string fileName)
        {
            try
            {
                var probe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder, fileName);
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

        private void LeftPanel_Resize(object sender, EventArgs e)
        {
            CenterLeftContent();
        }

        private void RightContentPanel_Resize(object sender, EventArgs e)
        {
            CenterFormContent();
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
                ConfigureDesignTimeEnvironmentOptions();
                SetStatus(string.Empty, false);
                return;
            }

            _configuration = _configurationController.LoadConfiguration();
            _environmentOptions = BuildEnvironmentOptions(_configuration);
            _profilesComboBox.DataSource = null;
            _profilesComboBox.DisplayMember = nameof(EnvironmentOption.DisplayName);
            _profilesComboBox.ValueMember = nameof(EnvironmentOption.Key);
            _profilesComboBox.DataSource = _environmentOptions;

            if (_environmentOptions.Length > 0)
            {
                var activeIndex = ResolveSelectedEnvironmentIndex();
                _profilesComboBox.SelectedIndex = activeIndex >= 0 ? activeIndex : 0;
                SetStatus(string.Empty, false);
            }
            else
            {
                SetStatus("Nenhum ambiente valido foi encontrado. Configure Producao e Homologacao antes de continuar.", true);
            }
        }

        private void ConfigureDesignTimeEnvironmentOptions()
        {
            if (_profilesComboBox.DataSource != null)
            {
                return;
            }

            _profilesComboBox.DisplayMember = nameof(EnvironmentOption.DisplayName);
            _profilesComboBox.ValueMember = nameof(EnvironmentOption.Key);
            _profilesComboBox.DataSource = new[]
            {
                new EnvironmentOption("production", "Producao", null),
                new EnvironmentOption("homologation", "Homologacao", null),
            };
            _profilesComboBox.SelectedIndex = 0;
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

            var selectedOption = _profilesComboBox.SelectedItem as EnvironmentOption;
            if (selectedOption == null)
            {
                SetStatus("Selecione um ambiente antes de continuar.", true);
                return;
            }

            if (selectedOption.Profile == null)
            {
                SetStatus("O ambiente selecionado nao esta configurado internamente.", true);
                return;
            }

            ActivateEnvironmentProfile(selectedOption.Profile, persistConfiguration: false);

            LoginResult result;
            try
            {
                result = _authenticationController.Login(
                    _configuration,
                    selectedOption.Profile.Id,
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

        private void CenterLeftContent()
        {
            if (_leftContentLayout == null || _leftContentLayout.IsDisposed)
            {
                return;
            }

            var left = (_leftPanel.ClientSize.Width - _leftContentLayout.Width) / 2;
            var top = (_leftPanel.ClientSize.Height - _leftContentLayout.Height) / 2;
            _leftContentLayout.Location = new Point(Math.Max(0, left), Math.Max(24, top));
        }

        private void CenterFormContent()
        {
            if (_formContainer == null || _formContainer.IsDisposed)
            {
                return;
            }

            var left = (_rightContentPanel.ClientSize.Width - _formContainer.Width) / 2;
            var top = (_rightContentPanel.ClientSize.Height - _formContainer.Height) / 2;
            _formContainer.Location = new Point(Math.Max(24, left), Math.Max(24, top));
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

        private void ProfilesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsDesignModeActive || _configuration == null)
            {
                return;
            }

            var selectedOption = _profilesComboBox.SelectedItem as EnvironmentOption;
            if (selectedOption == null || selectedOption.Profile == null)
            {
                return;
            }

            ActivateEnvironmentProfile(selectedOption.Profile, persistConfiguration: true);
            SetStatus(string.Empty, false);
        }

        private void ActivateEnvironmentProfile(DatabaseProfile profile, bool persistConfiguration)
        {
            if (_configuration == null || profile == null || string.IsNullOrWhiteSpace(profile.Id))
            {
                return;
            }

            if (string.Equals(_configuration.ActiveDatabaseId, profile.Id, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _configuration.ActiveDatabaseId = profile.Id;
            if (persistConfiguration && _configurationController != null)
            {
                _configurationController.SaveConfiguration(_configuration);
            }
        }

        private int ResolveSelectedEnvironmentIndex()
        {
            if (_environmentOptions.Length == 0)
            {
                return -1;
            }

            if (!string.IsNullOrWhiteSpace(_configuration?.ActiveDatabaseId))
            {
                for (var index = 0; index < _environmentOptions.Length; index++)
                {
                    var profile = _environmentOptions[index].Profile;
                    if (profile != null
                        && string.Equals(profile.Id, _configuration.ActiveDatabaseId, StringComparison.OrdinalIgnoreCase))
                    {
                        return index;
                    }
                }
            }

            var productionIndex = Array.FindIndex(_environmentOptions, option =>
                string.Equals(option.Key, "production", StringComparison.OrdinalIgnoreCase) && option.Profile != null);

            return productionIndex >= 0 ? productionIndex : 0;
        }

        private static EnvironmentOption[] BuildEnvironmentOptions(AppConfiguration configuration)
        {
            if (configuration == null)
            {
                return Array.Empty<EnvironmentOption>();
            }

            var profiles = configuration.GetOrderedProfiles().ToArray();
            var productionProfile = SelectPreferredProfile(profiles, production: true, configuration.ActiveDatabaseId);
            var homologationProfile = SelectPreferredProfile(profiles, production: false, configuration.ActiveDatabaseId);

            return new[]
            {
                new EnvironmentOption("production", "Producao", productionProfile),
                new EnvironmentOption("homologation", "Homologacao", homologationProfile),
            };
        }

        private static DatabaseProfile SelectPreferredProfile(DatabaseProfile[] profiles, bool production, string activeDatabaseId)
        {
            var candidates = profiles
                .Where(profile => IsHomologationProfile(profile) != production)
                .ToArray();

            if (candidates.Length == 0)
            {
                return null;
            }

            var activeCandidate = candidates.FirstOrDefault(profile =>
                string.Equals(profile.Id, activeDatabaseId, StringComparison.OrdinalIgnoreCase));
            if (activeCandidate != null)
            {
                return activeCandidate;
            }

            if (production)
            {
                var brcCandidate = candidates.FirstOrDefault(profile =>
                    ContainsAny(profile.Name, "brc", "producao")
                    || ContainsAny(profile.Database, "brc", "producao")
                    || ContainsAny(profile.Description, "producao"));
                if (brcCandidate != null)
                {
                    return brcCandidate;
                }
            }
            else
            {
                var homologCandidate = candidates.FirstOrDefault(IsHomologationProfile);
                if (homologCandidate != null)
                {
                    return homologCandidate;
                }
            }

            return candidates
                .OrderBy(profile => profile.Name ?? profile.Id, StringComparer.CurrentCultureIgnoreCase)
                .First();
        }

        private static bool IsHomologationProfile(DatabaseProfile profile)
        {
            return ContainsAny(profile?.Id, "hml", "homolog", "homologa")
                || ContainsAny(profile?.Name, "hml", "homolog", "homologa")
                || ContainsAny(profile?.Database, "hml", "homolog", "homologa")
                || ContainsAny(profile?.Description, "hml", "homolog", "homologa");
        }

        private static bool ContainsAny(string value, params string[] markers)
        {
            var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();
            return markers.Any(marker => normalized.Contains(marker));
        }

        private sealed class EnvironmentOption
        {
            public EnvironmentOption(string key, string displayName, DatabaseProfile profile)
            {
                Key = key;
                DisplayName = displayName;
                Profile = profile;
            }

            public string Key { get; }

            public string DisplayName { get; }

            public DatabaseProfile Profile { get; }
        }
    }
}
