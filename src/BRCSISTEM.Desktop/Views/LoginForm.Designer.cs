using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class LoginForm
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(LoginForm));
            this._leftPanel = new Panel();
            this._leftContentLayout = new TableLayoutPanel();
            this._logoPictureBox = new PictureBox();
            this._brandingTitleLabel = new Label();
            this._brandingSubtitleLabel = new Label();
            this._brandingVersionLabel = new Label();
            this._brandingSeparator = new Panel();
            this._brandingBulletsLabel = new Label();
            this._rightPanel = new Panel();
            this._rightContentPanel = new Panel();
            this._formContainer = new TableLayoutPanel();
            this._welcomeLabel = new Label();
            this._welcomeSubtitleLabel = new Label();
            this._bancoHeaderPanel = new Panel();
            this._bancoLabel = new Label();
            this._cacheIconButton = new Button();
            this._configIconButton = new Button();
            this._profilesComboBox = new ComboBox();
            this._userLabel = new Label();
            this._userHostPanel = new Panel();
            this._userNameTextBox = new TextBox();
            this._passwordLabel = new Label();
            this._passwordHostPanel = new Panel();
            this._passwordTextBox = new TextBox();
            this._statusLabel = new Label();
            this._loginButton = new Button();
            this._secondaryButtonsLayout = new TableLayoutPanel();
            this._requestAccessButton = new Button();
            this._closeButton = new Button();
            this._footerPanel = new Panel();
            this._footerLabel = new Label();
            this._leftPanel.SuspendLayout();
            this._leftContentLayout.SuspendLayout();
            ((ISupportInitialize)(this._logoPictureBox)).BeginInit();
            this._rightPanel.SuspendLayout();
            this._rightContentPanel.SuspendLayout();
            this._formContainer.SuspendLayout();
            this._bancoHeaderPanel.SuspendLayout();
            this._userHostPanel.SuspendLayout();
            this._passwordHostPanel.SuspendLayout();
            this._secondaryButtonsLayout.SuspendLayout();
            this._footerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _leftPanel
            // 
            this._leftPanel.BackColor = Color.FromArgb(27, 54, 93);
            this._leftPanel.Controls.Add(this._leftContentLayout);
            this._leftPanel.Dock = DockStyle.Left;
            this._leftPanel.Location = new Point(0, 0);
            this._leftPanel.Name = "_leftPanel";
            this._leftPanel.Size = new Size(360, 540);
            this._leftPanel.TabIndex = 1;
            // 
            // _leftContentLayout
            // 
            this._leftContentLayout.AutoSize = true;
            this._leftContentLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._leftContentLayout.BackColor = Color.FromArgb(27, 54, 93);
            this._leftContentLayout.ColumnCount = 1;
            this._leftContentLayout.ColumnStyles.Add(new ColumnStyle());
            this._leftContentLayout.Controls.Add(this._logoPictureBox, 0, 0);
            this._leftContentLayout.Controls.Add(this._brandingTitleLabel, 0, 1);
            this._leftContentLayout.Controls.Add(this._brandingSubtitleLabel, 0, 2);
            this._leftContentLayout.Controls.Add(this._brandingVersionLabel, 0, 3);
            this._leftContentLayout.Controls.Add(this._brandingSeparator, 0, 4);
            this._leftContentLayout.Controls.Add(this._brandingBulletsLabel, 0, 5);
            this._leftContentLayout.Location = new Point(47, 82);
            this._leftContentLayout.Margin = new Padding(0);
            this._leftContentLayout.Name = "_leftContentLayout";
            this._leftContentLayout.RowCount = 6;
            this._leftContentLayout.RowStyles.Add(new RowStyle());
            this._leftContentLayout.RowStyles.Add(new RowStyle());
            this._leftContentLayout.RowStyles.Add(new RowStyle());
            this._leftContentLayout.RowStyles.Add(new RowStyle());
            this._leftContentLayout.RowStyles.Add(new RowStyle());
            this._leftContentLayout.RowStyles.Add(new RowStyle());
            this._leftContentLayout.Size = new Size(230, 303);
            this._leftContentLayout.TabIndex = 0;
            // 
            // _logoPictureBox
            // 
            this._logoPictureBox.Anchor = AnchorStyles.None;
            this._logoPictureBox.BackColor = Color.FromArgb(27, 54, 93);
            this._logoPictureBox.Image = ((Image)(resources.GetObject("_logoPictureBox.Image")));
            this._logoPictureBox.Location = new Point(67, 0);
            this._logoPictureBox.Margin = new Padding(0, 0, 0, 15);
            this._logoPictureBox.Name = "_logoPictureBox";
            this._logoPictureBox.Size = new Size(96, 96);
            this._logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this._logoPictureBox.TabIndex = 0;
            this._logoPictureBox.TabStop = false;
            // 
            // _brandingTitleLabel
            // 
            this._brandingTitleLabel.Anchor = AnchorStyles.None;
            this._brandingTitleLabel.AutoSize = true;
            this._brandingTitleLabel.BackColor = Color.FromArgb(27, 54, 93);
            this._brandingTitleLabel.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            this._brandingTitleLabel.ForeColor = Color.White;
            this._brandingTitleLabel.Location = new Point(23, 111);
            this._brandingTitleLabel.Margin = new Padding(0, 0, 0, 5);
            this._brandingTitleLabel.Name = "_brandingTitleLabel";
            this._brandingTitleLabel.Size = new Size(183, 41);
            this._brandingTitleLabel.TabIndex = 1;
            this._brandingTitleLabel.Text = "BRCSISTEM";
            this._brandingTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _brandingSubtitleLabel
            // 
            this._brandingSubtitleLabel.Anchor = AnchorStyles.None;
            this._brandingSubtitleLabel.AutoSize = true;
            this._brandingSubtitleLabel.BackColor = Color.FromArgb(27, 54, 93);
            this._brandingSubtitleLabel.Font = new Font("Segoe UI", 12F);
            this._brandingSubtitleLabel.ForeColor = Color.FromArgb(74, 144, 194);
            this._brandingSubtitleLabel.Location = new Point(0, 157);
            this._brandingSubtitleLabel.Margin = new Padding(0, 0, 0, 3);
            this._brandingSubtitleLabel.Name = "_brandingSubtitleLabel";
            this._brandingSubtitleLabel.Size = new Size(230, 21);
            this._brandingSubtitleLabel.TabIndex = 2;
            this._brandingSubtitleLabel.Text = "Sistema de Controle de Estoque";
            this._brandingSubtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _brandingVersionLabel
            // 
            this._brandingVersionLabel.Anchor = AnchorStyles.None;
            this._brandingVersionLabel.AutoSize = true;
            this._brandingVersionLabel.BackColor = Color.FromArgb(27, 54, 93);
            this._brandingVersionLabel.Font = new Font("Segoe UI", 9.5F);
            this._brandingVersionLabel.ForeColor = Color.FromArgb(74, 144, 194);
            this._brandingVersionLabel.Location = new Point(69, 181);
            this._brandingVersionLabel.Margin = new Padding(0, 0, 0, 15);
            this._brandingVersionLabel.Name = "_brandingVersionLabel";
            this._brandingVersionLabel.Size = new Size(92, 17);
            this._brandingVersionLabel.TabIndex = 3;
            this._brandingVersionLabel.Text = "Versao v3.1.20";
            this._brandingVersionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _brandingSeparator
            // 
            this._brandingSeparator.Anchor = AnchorStyles.None;
            this._brandingSeparator.BackColor = Color.FromArgb(204, 173, 0);
            this._brandingSeparator.Location = new Point(30, 213);
            this._brandingSeparator.Margin = new Padding(0, 0, 0, 12);
            this._brandingSeparator.Name = "_brandingSeparator";
            this._brandingSeparator.Size = new Size(170, 2);
            this._brandingSeparator.TabIndex = 4;
            // 
            // _brandingBulletsLabel
            // 
            this._brandingBulletsLabel.Anchor = AnchorStyles.None;
            this._brandingBulletsLabel.AutoSize = true;
            this._brandingBulletsLabel.BackColor = Color.FromArgb(27, 54, 93);
            this._brandingBulletsLabel.Font = new Font("Segoe UI", 10F);
            this._brandingBulletsLabel.ForeColor = Color.FromArgb(74, 144, 194);
            this._brandingBulletsLabel.Location = new Point(2, 227);
            this._brandingBulletsLabel.Margin = new Padding(0);
            this._brandingBulletsLabel.Name = "_brandingBulletsLabel";
            this._brandingBulletsLabel.Size = new Size(225, 76);
            this._brandingBulletsLabel.TabIndex = 5;
            this._brandingBulletsLabel.Text = "- Gestao Inteligente de Estoque\r\n- Controle Total de Movimentacoes\r\n- Relatorios Detalhados\r\n- Acesso Seguro e Auditado";
            this._brandingBulletsLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _rightPanel
            // 
            this._rightPanel.BackColor = Color.White;
            this._rightPanel.Controls.Add(this._rightContentPanel);
            this._rightPanel.Controls.Add(this._footerPanel);
            this._rightPanel.Dock = DockStyle.Fill;
            this._rightPanel.Location = new Point(360, 0);
            this._rightPanel.Name = "_rightPanel";
            this._rightPanel.Size = new Size(460, 540);
            this._rightPanel.TabIndex = 0;
            // 
            // _rightContentPanel
            // 
            this._rightContentPanel.BackColor = Color.White;
            this._rightContentPanel.Controls.Add(this._formContainer);
            this._rightContentPanel.Dock = DockStyle.Fill;
            this._rightContentPanel.Location = new Point(0, 0);
            this._rightContentPanel.Name = "_rightContentPanel";
            this._rightContentPanel.Size = new Size(460, 506);
            this._rightContentPanel.TabIndex = 0;
            // 
            // _formContainer
            // 
            this._formContainer.Anchor = AnchorStyles.None;
            this._formContainer.AutoSize = true;
            this._formContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._formContainer.BackColor = Color.White;
            this._formContainer.ColumnCount = 1;
            this._formContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280F));
            this._formContainer.Controls.Add(this._welcomeLabel, 0, 0);
            this._formContainer.Controls.Add(this._welcomeSubtitleLabel, 0, 1);
            this._formContainer.Controls.Add(this._bancoHeaderPanel, 0, 2);
            this._formContainer.Controls.Add(this._profilesComboBox, 0, 3);
            this._formContainer.Controls.Add(this._userLabel, 0, 4);
            this._formContainer.Controls.Add(this._userHostPanel, 0, 5);
            this._formContainer.Controls.Add(this._passwordLabel, 0, 6);
            this._formContainer.Controls.Add(this._passwordHostPanel, 0, 7);
            this._formContainer.Controls.Add(this._statusLabel, 0, 8);
            this._formContainer.Controls.Add(this._loginButton, 0, 9);
            this._formContainer.Controls.Add(this._secondaryButtonsLayout, 0, 10);
            this._formContainer.Location = new Point(90, 111);
            this._formContainer.Margin = new Padding(0);
            this._formContainer.Name = "_formContainer";
            this._formContainer.Padding = new Padding(0);
            this._formContainer.RowCount = 11;
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.RowStyles.Add(new RowStyle());
            this._formContainer.Size = new Size(280, 320);
            this._formContainer.TabIndex = 0;
            // 
            // _welcomeLabel
            // 
            this._welcomeLabel.Anchor = AnchorStyles.Left;
            this._welcomeLabel.AutoSize = true;
            this._welcomeLabel.BackColor = Color.White;
            this._welcomeLabel.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            this._welcomeLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._welcomeLabel.Location = new Point(0, 0);
            this._welcomeLabel.Margin = new Padding(0, 0, 0, 4);
            this._welcomeLabel.Name = "_welcomeLabel";
            this._welcomeLabel.Size = new Size(167, 37);
            this._welcomeLabel.TabIndex = 0;
            this._welcomeLabel.Text = "Bem-vindo!";
            // 
            // _welcomeSubtitleLabel
            // 
            this._welcomeSubtitleLabel.Anchor = AnchorStyles.Left;
            this._welcomeSubtitleLabel.AutoSize = true;
            this._welcomeSubtitleLabel.BackColor = Color.White;
            this._welcomeSubtitleLabel.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            this._welcomeSubtitleLabel.ForeColor = Color.FromArgb(73, 80, 87);
            this._welcomeSubtitleLabel.Location = new Point(0, 41);
            this._welcomeSubtitleLabel.Margin = new Padding(0, 0, 0, 20);
            this._welcomeSubtitleLabel.Name = "_welcomeSubtitleLabel";
            this._welcomeSubtitleLabel.Size = new Size(211, 19);
            this._welcomeSubtitleLabel.TabIndex = 1;
            this._welcomeSubtitleLabel.Text = "Faca login para acessar o sistema";
            // 
            // _bancoHeaderPanel
            // 
            this._bancoHeaderPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this._bancoHeaderPanel.BackColor = Color.White;
            this._bancoHeaderPanel.Controls.Add(this._bancoLabel);
            this._bancoHeaderPanel.Controls.Add(this._cacheIconButton);
            this._bancoHeaderPanel.Controls.Add(this._configIconButton);
            this._bancoHeaderPanel.Location = new Point(0, 80);
            this._bancoHeaderPanel.Margin = new Padding(0, 0, 0, 3);
            this._bancoHeaderPanel.Name = "_bancoHeaderPanel";
            this._bancoHeaderPanel.Size = new Size(280, 30);
            this._bancoHeaderPanel.TabIndex = 2;
            // 
            // _bancoLabel
            // 
            this._bancoLabel.BackColor = Color.White;
            this._bancoLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._bancoLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._bancoLabel.Location = new Point(0, 6);
            this._bancoLabel.Name = "_bancoLabel";
            this._bancoLabel.Size = new Size(212, 20);
            this._bancoLabel.TabIndex = 0;
            this._bancoLabel.Text = "Banco de Dados";
            this._bancoLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _cacheIconButton
            // 
            this._cacheIconButton.BackColor = Color.White;
            this._cacheIconButton.Cursor = Cursors.Hand;
            this._cacheIconButton.FlatAppearance.BorderSize = 0;
            this._cacheIconButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 249, 250);
            this._cacheIconButton.FlatStyle = FlatStyle.Flat;
            this._cacheIconButton.ForeColor = Color.FromArgb(73, 80, 87);
            this._cacheIconButton.ImageAlign = ContentAlignment.MiddleCenter;
            this._cacheIconButton.Location = new Point(216, 2);
            this._cacheIconButton.Name = "_cacheIconButton";
            this._cacheIconButton.Size = new Size(26, 26);
            this._cacheIconButton.TabIndex = 1;
            this._cacheIconButton.TabStop = false;
            this._cacheIconButton.Text = "";
            this._cacheIconButton.UseVisualStyleBackColor = false;
            // 
            // _configIconButton
            // 
            this._configIconButton.BackColor = Color.White;
            this._configIconButton.Cursor = Cursors.Hand;
            this._configIconButton.FlatAppearance.BorderSize = 0;
            this._configIconButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(248, 249, 250);
            this._configIconButton.FlatStyle = FlatStyle.Flat;
            this._configIconButton.ForeColor = Color.FromArgb(73, 80, 87);
            this._configIconButton.ImageAlign = ContentAlignment.MiddleCenter;
            this._configIconButton.Location = new Point(248, 2);
            this._configIconButton.Name = "_configIconButton";
            this._configIconButton.Size = new Size(26, 26);
            this._configIconButton.TabIndex = 2;
            this._configIconButton.TabStop = false;
            this._configIconButton.Text = "";
            this._configIconButton.UseVisualStyleBackColor = false;
            // 
            // _profilesComboBox
            // 
            this._profilesComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this._profilesComboBox.BackColor = Color.FromArgb(248, 249, 250);
            this._profilesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this._profilesComboBox.FlatStyle = FlatStyle.Flat;
            this._profilesComboBox.Font = new Font("Segoe UI", 9F);
            this._profilesComboBox.FormattingEnabled = true;
            this._profilesComboBox.Location = new Point(0, 113);
            this._profilesComboBox.Margin = new Padding(0, 0, 0, 16);
            this._profilesComboBox.Name = "_profilesComboBox";
            this._profilesComboBox.Size = new Size(280, 23);
            this._profilesComboBox.TabIndex = 3;
            // 
            // _userLabel
            // 
            this._userLabel.Anchor = AnchorStyles.Left;
            this._userLabel.AutoSize = true;
            this._userLabel.BackColor = Color.White;
            this._userLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._userLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._userLabel.Location = new Point(0, 154);
            this._userLabel.Margin = new Padding(0, 0, 0, 2);
            this._userLabel.Name = "_userLabel";
            this._userLabel.Size = new Size(49, 15);
            this._userLabel.TabIndex = 4;
            this._userLabel.Text = "Usuario";
            // 
            // _userHostPanel
            // 
            this._userHostPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this._userHostPanel.BackColor = Color.FromArgb(248, 249, 250);
            this._userHostPanel.BorderStyle = BorderStyle.FixedSingle;
            this._userHostPanel.Controls.Add(this._userNameTextBox);
            this._userHostPanel.Location = new Point(0, 171);
            this._userHostPanel.Margin = new Padding(0, 0, 0, 10);
            this._userHostPanel.Name = "_userHostPanel";
            this._userHostPanel.Padding = new Padding(10, 8, 10, 6);
            this._userHostPanel.Size = new Size(280, 38);
            this._userHostPanel.TabIndex = 5;
            // 
            // _userNameTextBox
            // 
            this._userNameTextBox.BackColor = Color.FromArgb(248, 249, 250);
            this._userNameTextBox.BorderStyle = BorderStyle.None;
            this._userNameTextBox.Dock = DockStyle.Fill;
            this._userNameTextBox.Font = new Font("Segoe UI", 10F);
            this._userNameTextBox.ForeColor = Color.FromArgb(73, 80, 87);
            this._userNameTextBox.Location = new Point(10, 8);
            this._userNameTextBox.Name = "_userNameTextBox";
            this._userNameTextBox.Size = new Size(258, 18);
            this._userNameTextBox.TabIndex = 0;
            // 
            // _passwordLabel
            // 
            this._passwordLabel.Anchor = AnchorStyles.Left;
            this._passwordLabel.AutoSize = true;
            this._passwordLabel.BackColor = Color.White;
            this._passwordLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._passwordLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._passwordLabel.Location = new Point(0, 221);
            this._passwordLabel.Margin = new Padding(0, 0, 0, 2);
            this._passwordLabel.Name = "_passwordLabel";
            this._passwordLabel.Size = new Size(41, 15);
            this._passwordLabel.TabIndex = 6;
            this._passwordLabel.Text = "Senha";
            // 
            // _passwordHostPanel
            // 
            this._passwordHostPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this._passwordHostPanel.BackColor = Color.FromArgb(248, 249, 250);
            this._passwordHostPanel.BorderStyle = BorderStyle.FixedSingle;
            this._passwordHostPanel.Controls.Add(this._passwordTextBox);
            this._passwordHostPanel.Location = new Point(0, 238);
            this._passwordHostPanel.Margin = new Padding(0, 0, 0, 10);
            this._passwordHostPanel.Name = "_passwordHostPanel";
            this._passwordHostPanel.Padding = new Padding(10, 8, 10, 6);
            this._passwordHostPanel.Size = new Size(280, 38);
            this._passwordHostPanel.TabIndex = 7;
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.BackColor = Color.FromArgb(248, 249, 250);
            this._passwordTextBox.BorderStyle = BorderStyle.None;
            this._passwordTextBox.Dock = DockStyle.Fill;
            this._passwordTextBox.Font = new Font("Segoe UI", 10F);
            this._passwordTextBox.ForeColor = Color.FromArgb(73, 80, 87);
            this._passwordTextBox.Location = new Point(10, 8);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new Size(258, 18);
            this._passwordTextBox.TabIndex = 0;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _statusLabel
            // 
            this._statusLabel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this._statusLabel.BackColor = Color.White;
            this._statusLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            this._statusLabel.ForeColor = Color.FromArgb(220, 53, 69);
            this._statusLabel.Location = new Point(0, 286);
            this._statusLabel.Margin = new Padding(0, 0, 0, 10);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new Size(280, 22);
            this._statusLabel.TabIndex = 8;
            this._statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _loginButton
            // 
            this._loginButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this._loginButton.BackColor = Color.FromArgb(27, 54, 93);
            this._loginButton.Cursor = Cursors.Hand;
            this._loginButton.FlatAppearance.BorderSize = 0;
            this._loginButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(46, 89, 132);
            this._loginButton.FlatStyle = FlatStyle.Flat;
            this._loginButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this._loginButton.ForeColor = Color.White;
            this._loginButton.Location = new Point(0, 318);
            this._loginButton.Margin = new Padding(0, 0, 0, 8);
            this._loginButton.Name = "_loginButton";
            this._loginButton.Size = new Size(280, 38);
            this._loginButton.TabIndex = 9;
            this._loginButton.Text = "ACESSAR SISTEMA";
            this._loginButton.UseVisualStyleBackColor = false;
            // 
            // _secondaryButtonsLayout
            // 
            this._secondaryButtonsLayout.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this._secondaryButtonsLayout.BackColor = Color.White;
            this._secondaryButtonsLayout.ColumnCount = 2;
            this._secondaryButtonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this._secondaryButtonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this._secondaryButtonsLayout.Controls.Add(this._requestAccessButton, 0, 0);
            this._secondaryButtonsLayout.Controls.Add(this._closeButton, 1, 0);
            this._secondaryButtonsLayout.Location = new Point(0, 364);
            this._secondaryButtonsLayout.Margin = new Padding(0, 0, 0, 12);
            this._secondaryButtonsLayout.Name = "_secondaryButtonsLayout";
            this._secondaryButtonsLayout.RowCount = 1;
            this._secondaryButtonsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._secondaryButtonsLayout.Size = new Size(280, 36);
            this._secondaryButtonsLayout.TabIndex = 10;
            // 
            // _requestAccessButton
            // 
            this._requestAccessButton.BackColor = Color.FromArgb(233, 236, 239);
            this._requestAccessButton.Cursor = Cursors.Hand;
            this._requestAccessButton.Dock = DockStyle.Fill;
            this._requestAccessButton.FlatAppearance.BorderSize = 0;
            this._requestAccessButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(73, 80, 87);
            this._requestAccessButton.FlatStyle = FlatStyle.Flat;
            this._requestAccessButton.Font = new Font("Segoe UI", 9F);
            this._requestAccessButton.ForeColor = Color.FromArgb(27, 54, 93);
            this._requestAccessButton.Location = new Point(0, 0);
            this._requestAccessButton.Margin = new Padding(0, 0, 5, 0);
            this._requestAccessButton.Name = "_requestAccessButton";
            this._requestAccessButton.Size = new Size(135, 36);
            this._requestAccessButton.TabIndex = 0;
            this._requestAccessButton.Text = "Solicitar Acesso";
            this._requestAccessButton.UseVisualStyleBackColor = false;
            // 
            // _closeButton
            // 
            this._closeButton.BackColor = Color.FromArgb(233, 236, 239);
            this._closeButton.Cursor = Cursors.Hand;
            this._closeButton.Dock = DockStyle.Fill;
            this._closeButton.FlatAppearance.BorderSize = 0;
            this._closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 53, 69);
            this._closeButton.FlatStyle = FlatStyle.Flat;
            this._closeButton.Font = new Font("Segoe UI", 9F);
            this._closeButton.ForeColor = Color.FromArgb(27, 54, 93);
            this._closeButton.Location = new Point(145, 0);
            this._closeButton.Margin = new Padding(5, 0, 0, 0);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new Size(135, 36);
            this._closeButton.TabIndex = 1;
            this._closeButton.Text = "Fechar";
            this._closeButton.UseVisualStyleBackColor = false;
            // 
            // _footerPanel
            // 
            this._footerPanel.BackColor = Color.FromArgb(248, 249, 250);
            this._footerPanel.Controls.Add(this._footerLabel);
            this._footerPanel.Dock = DockStyle.Bottom;
            this._footerPanel.Location = new Point(0, 506);
            this._footerPanel.Name = "_footerPanel";
            this._footerPanel.Size = new Size(460, 34);
            this._footerPanel.TabIndex = 1;
            // 
            // _footerLabel
            // 
            this._footerLabel.BackColor = Color.FromArgb(248, 249, 250);
            this._footerLabel.Dock = DockStyle.Fill;
            this._footerLabel.Font = new Font("Segoe UI", 9F);
            this._footerLabel.ForeColor = Color.FromArgb(73, 80, 87);
            this._footerLabel.Location = new Point(0, 0);
            this._footerLabel.Name = "_footerLabel";
            this._footerLabel.Size = new Size(460, 34);
            this._footerLabel.TabIndex = 0;
            this._footerLabel.Text = " 2025 BRCSISTEM - Todos os direitos reservados ";
            this._footerLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.ClientSize = new Size(820, 540);
            this.Controls.Add(this._rightPanel);
            this.Controls.Add(this._leftPanel);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "BRCSISTEM v3.1.20 - Acesso ao Sistema";
            this._leftPanel.ResumeLayout(false);
            this._leftPanel.PerformLayout();
            this._leftContentLayout.ResumeLayout(false);
            this._leftContentLayout.PerformLayout();
            ((ISupportInitialize)(this._logoPictureBox)).EndInit();
            this._rightPanel.ResumeLayout(false);
            this._rightContentPanel.ResumeLayout(false);
            this._rightContentPanel.PerformLayout();
            this._formContainer.ResumeLayout(false);
            this._formContainer.PerformLayout();
            this._bancoHeaderPanel.ResumeLayout(false);
            this._userHostPanel.ResumeLayout(false);
            this._userHostPanel.PerformLayout();
            this._passwordHostPanel.ResumeLayout(false);
            this._passwordHostPanel.PerformLayout();
            this._secondaryButtonsLayout.ResumeLayout(false);
            this._footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Panel _leftPanel;
        private TableLayoutPanel _leftContentLayout;
        private PictureBox _logoPictureBox;
        private Label _brandingTitleLabel;
        private Label _brandingSubtitleLabel;
        private Label _brandingVersionLabel;
        private Panel _brandingSeparator;
        private Label _brandingBulletsLabel;
        private Panel _rightPanel;
        private Panel _rightContentPanel;
        private TableLayoutPanel _formContainer;
        private Label _welcomeLabel;
        private Label _welcomeSubtitleLabel;
        private Panel _bancoHeaderPanel;
        private Label _bancoLabel;
        private Button _cacheIconButton;
        private Button _configIconButton;
        private ComboBox _profilesComboBox;
        private Label _userLabel;
        private Panel _userHostPanel;
        private TextBox _userNameTextBox;
        private Label _passwordLabel;
        private Panel _passwordHostPanel;
        private TextBox _passwordTextBox;
        private Label _statusLabel;
        private Button _loginButton;
        private TableLayoutPanel _secondaryButtonsLayout;
        private Button _requestAccessButton;
        private Button _closeButton;
        private Panel _footerPanel;
        private Label _footerLabel;
    }
}