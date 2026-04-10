using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class LoginForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly ConfigurationController _configurationController;
        private readonly AuthenticationController _authenticationController;

        private AppConfiguration _configuration;
        private ComboBox _profilesComboBox;
        private TextBox _userNameTextBox;
        private TextBox _passwordTextBox;
        private Label _statusLabel;

        public LoginForm(CompositionRoot compositionRoot)
        {
            _compositionRoot = compositionRoot;
            _configurationController = compositionRoot.CreateConfigurationController();
            _authenticationController = compositionRoot.CreateAuthenticationController();

            InitializeComponent();
            Load += OnLoad;
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Acesso ao Sistema";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(760, 460);
            MinimumSize = new Size(760, 460);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;

            var rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
            };
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));

            var brandingPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(27, 54, 93) };
            var brandingTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 120,
                Text = "BRCSISTEM",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                TextAlign = ContentAlignment.BottomCenter,
                Padding = new Padding(10, 70, 10, 10),
            };
            var brandingSubtitle = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Migracao inicial para .NET Framework\n\n- Login e autenticacao\n- Gerenciamento de bancos\n- Menu principal MVC\n- Mapeamento dos modulos Python",
                ForeColor = Color.FromArgb(207, 225, 245),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                TextAlign = ContentAlignment.TopCenter,
                Padding = new Padding(20),
            };
            brandingPanel.Controls.Add(brandingSubtitle);
            brandingPanel.Controls.Add(brandingTitle);

            var formPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(32) };
            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
            };

            contentLayout.Controls.Add(CreateHeaderLabel("Bem-vindo", 20F, FontStyle.Bold, Color.FromArgb(27, 54, 93)));
            contentLayout.Controls.Add(CreateHeaderLabel("Use a configuracao existente do PostgreSQL e faca login para abrir o shell WinForms.", 9.5F, FontStyle.Regular, Color.FromArgb(73, 80, 87)));

            contentLayout.Controls.Add(CreateFieldLabel("Banco de Dados"));
            _profilesComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 360,
                Height = 30,
                Font = new Font("Segoe UI", 10F),
            };
            contentLayout.Controls.Add(_profilesComboBox);

            var profileButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 6, 0, 12),
            };
            var manageProfilesButton = new Button
            {
                Text = "Gerenciar Bancos",
                AutoSize = true,
                FlatStyle = FlatStyle.System,
            };
            manageProfilesButton.Click += OpenProfilesManager;
            var refreshButton = new Button
            {
                Text = "Atualizar",
                AutoSize = true,
                FlatStyle = FlatStyle.System,
            };
            refreshButton.Click += (sender, args) => LoadConfiguration();
            profileButtons.Controls.Add(manageProfilesButton);
            profileButtons.Controls.Add(refreshButton);
            contentLayout.Controls.Add(profileButtons);

            contentLayout.Controls.Add(CreateFieldLabel("Usuario"));
            _userNameTextBox = new TextBox
            {
                Width = 360,
                Font = new Font("Segoe UI", 10F),
            };
            contentLayout.Controls.Add(_userNameTextBox);

            contentLayout.Controls.Add(CreateFieldLabel("Senha"));
            _passwordTextBox = new TextBox
            {
                Width = 360,
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true,
            };
            _passwordTextBox.KeyDown += (sender, args) =>
            {
                if (args.KeyCode == Keys.Enter)
                {
                    TryLogin();
                }
            };
            contentLayout.Controls.Add(_passwordTextBox);

            _statusLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.Firebrick,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(0, 12, 0, 12),
            };
            contentLayout.Controls.Add(_statusLabel);

            var actionButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            var loginButton = new Button
            {
                Text = "Acessar Sistema",
                AutoSize = true,
                FlatStyle = FlatStyle.System,
            };
            loginButton.Click += (sender, args) => TryLogin();
            var closeButton = new Button
            {
                Text = "Fechar",
                AutoSize = true,
                FlatStyle = FlatStyle.System,
            };
            closeButton.Click += (sender, args) => Close();
            actionButtons.Controls.Add(loginButton);
            actionButtons.Controls.Add(closeButton);
            contentLayout.Controls.Add(actionButtons);

            formPanel.Controls.Add(contentLayout);

            rootLayout.Controls.Add(brandingPanel, 0, 0);
            rootLayout.Controls.Add(formPanel, 1, 0);
            Controls.Add(rootLayout);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            LoadConfiguration();
            _userNameTextBox.Focus();
        }

        private void LoadConfiguration()
        {
            _configuration = _configurationController.LoadConfiguration();
            var profiles = _configuration.GetOrderedProfiles().ToArray();
            _profilesComboBox.DataSource = profiles;
            _profilesComboBox.DisplayMember = nameof(DatabaseProfile.DisplayName);
            _profilesComboBox.ValueMember = nameof(DatabaseProfile.Id);

            if (profiles.Length > 0)
            {
                var activeIndex = Array.FindIndex(profiles, profile => string.Equals(profile.Id, _configuration.ActiveDatabaseId, StringComparison.OrdinalIgnoreCase));
                _profilesComboBox.SelectedIndex = activeIndex >= 0 ? activeIndex : 0;
                SetStatus("Base de configuracao carregada.", false);
            }
            else
            {
                SetStatus("Nenhum banco configurado. Use 'Gerenciar Bancos' para adicionar um perfil PostgreSQL.", true);
            }
        }

        private void OpenProfilesManager(object sender, EventArgs e)
        {
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
                result = _authenticationController.Login(_configuration, selectedProfile.Id, _userNameTextBox.Text, _passwordTextBox.Text);
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
                using (var changePasswordForm = new ChangePasswordForm(_compositionRoot, _configuration, result.DatabaseProfile, result.Identity.UserName, true))
                {
                    var dialogResult = changePasswordForm.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        MessageBox.Show(this, "Senha alterada. Faca login novamente com a nova senha.", "Senha Atualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            mainForm.FormClosed += (sender, args) => Close();
            Hide();
            mainForm.Show();
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }

        private static Label CreateHeaderLabel(string text, float size, FontStyle style, Color color)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", size, style),
                ForeColor = color,
                Margin = new Padding(0, 0, 0, 12),
            };
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 0, 0, 4),
            };
        }
    }
}
