using System;
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
    public sealed class LoginForm : Form
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
        private ComboBox _profilesComboBox;
        private TextBox _userNameTextBox;
        private TextBox _passwordTextBox;
        private Label _statusLabel;
        private Button _loginButton;

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
            Text = AppName + " " + AppVersion + " - Acesso ao Sistema";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(720, 450);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = CinzaClaro;

            var leftPanel = BuildLeftPanel();
            var rightPanel = BuildRightPanel();

            Controls.Add(rightPanel);
            Controls.Add(leftPanel);
        }

        private Panel BuildLeftPanel()
        {
            var left = new Panel
            {
                Dock = DockStyle.Left,
                Width = 300,
                BackColor = AzulPrincipal,
            };

            var content = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = AzulPrincipal,
            };

            var logoImage = TryLoadLogo();
            if (logoImage != null)
            {
                content.Controls.Add(new PictureBox
                {
                    Image = logoImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(80, 80),
                    BackColor = AzulPrincipal,
                    Margin = new Padding(0, 0, 0, 15),
                    Anchor = AnchorStyles.None,
                });
            }

            content.Controls.Add(new Label
            {
                AutoSize = true,
                Text = AppName,
                ForeColor = Branco,
                BackColor = AzulPrincipal,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                Anchor = AnchorStyles.None,
                Margin = new Padding(0, 0, 0, 5),
            });

            content.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Sistema de Controle de Estoque",
                ForeColor = AzulClaro,
                BackColor = AzulPrincipal,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                Anchor = AnchorStyles.None,
                Margin = new Padding(0, 0, 0, 3),
            });

            content.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Versao v3.1.20",
                ForeColor = AzulClaro,
                BackColor = AzulPrincipal,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Anchor = AnchorStyles.None,
                Margin = new Padding(0, 0, 0, 15),
            });

            content.Controls.Add(new Panel
            {
                Height = 2,
                Width = 150,
                BackColor = Dourado,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0, 0, 0, 12),
            });

            content.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "- Gestao Inteligente de Estoque" + Environment.NewLine +
                       "- Controle Total de Movimentacoes" + Environment.NewLine +
                       "- Relatorios Detalhados" + Environment.NewLine +
                       "- Acesso Seguro e Auditado",
                ForeColor = AzulClaro,
                BackColor = AzulPrincipal,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0),
            });

            var centerHost = new Panel { Dock = DockStyle.Fill, BackColor = AzulPrincipal };
            centerHost.Controls.Add(content);
            centerHost.Resize += (s, e) =>
            {
                content.Left = Math.Max(0, (centerHost.Width - content.Width) / 2);
                content.Top = Math.Max(0, (centerHost.Height - content.Height) / 2);
            };
            left.Controls.Add(centerHost);
            left.Resize += (s, e) =>
            {
                content.Left = Math.Max(0, (centerHost.Width - content.Width) / 2);
                content.Top = Math.Max(0, (centerHost.Height - content.Height) / 2);
            };
            return left;
        }

        private Panel BuildRightPanel()
        {
            var right = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Branco,
            };

            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = CinzaClaro,
            };
            footer.Controls.Add(new Label
            {
                Text = " 2025 " + AppName + " - Todos os direitos reservados ",
                ForeColor = CinzaEscuro,
                BackColor = CinzaClaro,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
            });

            var form = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Branco,
                Padding = new Padding(30, 15, 30, 15),
            };
            const int fieldWidth = 260;

            form.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Bem-vindo!",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = AzulPrincipal,
                BackColor = Branco,
                Margin = new Padding(0, 0, 0, 3),
            });
            form.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Faca login para acessar o sistema",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = CinzaEscuro,
                BackColor = Branco,
                Margin = new Padding(0, 0, 0, 25),
            });

            var bancoHeader = new Panel
            {
                Width = fieldWidth,
                Height = 26,
                BackColor = Branco,
                Margin = new Padding(0, 0, 0, 2),
            };

            var iconsPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Branco,
                Margin = new Padding(0),
                Dock = DockStyle.Right,
            };
            var btnCache = BuildIconButton("\uD83E\uDDF9");
            btnCache.Click += (s, e) => LoadConfiguration();
            var btnConfig = BuildIconButton("\u2699");
            btnConfig.Click += OpenProfilesManager;
            iconsPanel.Controls.Add(btnCache);
            iconsPanel.Controls.Add(btnConfig);
            bancoHeader.Controls.Add(iconsPanel);

            bancoHeader.Controls.Add(new Label
            {
                AutoSize = false,
                Dock = DockStyle.Left,
                Width = fieldWidth - 70,
                Text = "Banco de Dados",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = AzulPrincipal,
                BackColor = Branco,
                TextAlign = ContentAlignment.MiddleLeft,
            });

            form.Controls.Add(bancoHeader);

            _profilesComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = fieldWidth,
                Font = new Font("Segoe UI", 9F),
                FlatStyle = FlatStyle.Flat,
                BackColor = CinzaClaro,
                FormattingEnabled = true,
                Margin = new Padding(0, 0, 0, 14),
            };
            _profilesComboBox.Format += OnProfileFormat;
            form.Controls.Add(_profilesComboBox);

            form.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Usuario",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = AzulPrincipal,
                BackColor = Branco,
                Margin = new Padding(0, 0, 0, 2),
            });
            _userNameTextBox = BuildFieldTextBox(fieldWidth);
            _userNameTextBox.Margin = new Padding(0, 0, 0, 12);
            _userNameTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    _passwordTextBox.Focus();
                    e.SuppressKeyPress = true;
                }
            };
            form.Controls.Add(_userNameTextBox);

            form.Controls.Add(new Label
            {
                AutoSize = true,
                Text = "Senha",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = AzulPrincipal,
                BackColor = Branco,
                Margin = new Padding(0, 0, 0, 2),
            });
            _passwordTextBox = BuildFieldTextBox(fieldWidth);
            _passwordTextBox.UseSystemPasswordChar = true;
            _passwordTextBox.Margin = new Padding(0, 0, 0, 8);
            _passwordTextBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TryLogin();
                    e.SuppressKeyPress = true;
                }
            };
            form.Controls.Add(_passwordTextBox);

            _statusLabel = new Label
            {
                AutoSize = false,
                Width = fieldWidth,
                Height = 22,
                Text = string.Empty,
                ForeColor = VermelhoErro,
                BackColor = Branco,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 8),
            };
            form.Controls.Add(_statusLabel);

            _loginButton = new Button
            {
                Text = "ACESSAR SISTEMA",
                Width = fieldWidth,
                Height = 34,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                BackColor = AzulPrincipal,
                ForeColor = Branco,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 6),
            };
            _loginButton.FlatAppearance.BorderSize = 0;
            _loginButton.FlatAppearance.MouseOverBackColor = AzulSecundario;
            _loginButton.Click += (s, e) => TryLogin();
            form.Controls.Add(_loginButton);

            var secondaryButtons = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Width = fieldWidth,
                Height = 30,
                BackColor = Branco,
                Margin = new Padding(0, 0, 0, 10),
            };
            secondaryButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            secondaryButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            var btnSolicitar = BuildSecondaryButton("Solicitar Acesso");
            btnSolicitar.Dock = DockStyle.Fill;
            btnSolicitar.Margin = new Padding(0, 0, 5, 0);
            btnSolicitar.Click += (s, e) =>
                MessageBox.Show(this,
                    "Solicitacao de acesso indisponivel nesta versao. Contate o administrador.",
                    "Solicitar Acesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            var btnFechar = BuildSecondaryButton("Fechar");
            btnFechar.Dock = DockStyle.Fill;
            btnFechar.Margin = new Padding(5, 0, 0, 0);
            btnFechar.FlatAppearance.MouseOverBackColor = VermelhoErro;
            btnFechar.Click += (s, e) => Close();

            secondaryButtons.Controls.Add(btnSolicitar, 0, 0);
            secondaryButtons.Controls.Add(btnFechar, 1, 0);
            form.Controls.Add(secondaryButtons);

            var center = new Panel { Dock = DockStyle.Fill, BackColor = Branco };
            center.Controls.Add(form);
            center.Resize += (s, e) =>
            {
                form.Left = Math.Max(0, (center.Width - form.Width) / 2);
                form.Top = Math.Max(0, (center.Height - form.Height) / 2);
            };

            right.Controls.Add(center);
            right.Controls.Add(footer);
            return right;
        }

        private static Button BuildIconButton(string glyph)
        {
            var btn = new Button
            {
                Text = glyph,
                Font = new Font("Segoe UI Symbol", 10F),
                Width = 26,
                Height = 24,
                BackColor = Branco,
                ForeColor = CinzaEscuro,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(2, 0, 2, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                TabStop = false,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = CinzaClaro;
            return btn;
        }

        private static TextBox BuildFieldTextBox(int width)
        {
            return new TextBox
            {
                Width = width,
                Font = new Font("Segoe UI", 10F),
                BackColor = CinzaClaro,
                ForeColor = CinzaEscuro,
                BorderStyle = BorderStyle.FixedSingle,
            };
        }

        private static Button BuildSecondaryButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                BackColor = CinzaMedio,
                ForeColor = AzulPrincipal,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 28,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = CinzaEscuro;
            return btn;
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
            LoadConfiguration();
            _userNameTextBox.Focus();
        }

        private void LoadConfiguration()
        {
            _configuration = _configurationController.LoadConfiguration();
            var profiles = _configuration.GetOrderedProfiles().ToArray();
            _profilesComboBox.DataSource = profiles;
            _profilesComboBox.DisplayMember = null;
            _profilesComboBox.ValueMember = nameof(DatabaseProfile.Id);

            if (profiles.Length > 0)
            {
                var activeIndex = Array.FindIndex(profiles, profile => string.Equals(profile.Id, _configuration.ActiveDatabaseId, StringComparison.OrdinalIgnoreCase));
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
