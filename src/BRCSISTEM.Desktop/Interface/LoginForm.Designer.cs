using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this._leftPanel = new System.Windows.Forms.Panel();
            this._leftContentLayout = new System.Windows.Forms.TableLayoutPanel();
            this._logoPictureBox = new System.Windows.Forms.PictureBox();
            this._brandingTitleLabel = new System.Windows.Forms.Label();
            this._brandingSubtitleLabel = new System.Windows.Forms.Label();
            this._brandingVersionLabel = new System.Windows.Forms.Label();
            this._brandingSeparator = new System.Windows.Forms.Panel();
            this._brandingBulletsLabel = new System.Windows.Forms.Label();
            this._rightPanel = new System.Windows.Forms.Panel();
            this._rightContentPanel = new System.Windows.Forms.Panel();
            this._formContainer = new System.Windows.Forms.TableLayoutPanel();
            this._welcomeLabel = new System.Windows.Forms.Label();
            this._welcomeSubtitleLabel = new System.Windows.Forms.Label();
            this._bancoHeaderPanel = new System.Windows.Forms.Panel();
            this._bancoLabel = new System.Windows.Forms.Label();
            this._cacheIconButton = new System.Windows.Forms.Button();
            this._configIconButton = new System.Windows.Forms.Button();
            this._profilesComboBox = new System.Windows.Forms.ComboBox();
            this._userLabel = new System.Windows.Forms.Label();
            this._userHostPanel = new System.Windows.Forms.Panel();
            this._userNameTextBox = new System.Windows.Forms.TextBox();
            this._passwordLabel = new System.Windows.Forms.Label();
            this._passwordHostPanel = new System.Windows.Forms.Panel();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this._statusLabel = new System.Windows.Forms.Label();
            this._loginButton = new System.Windows.Forms.Button();
            this._secondaryButtonsLayout = new System.Windows.Forms.TableLayoutPanel();
            this._requestAccessButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this._footerPanel = new System.Windows.Forms.Panel();
            this._footerLabel = new System.Windows.Forms.Label();
            this._leftPanel.SuspendLayout();
            this._leftContentLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._logoPictureBox)).BeginInit();
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
            this._leftPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._leftPanel.Controls.Add(this._leftContentLayout);
            this._leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this._leftPanel.Location = new System.Drawing.Point(0, 0);
            this._leftPanel.Name = "_leftPanel";
            this._leftPanel.Size = new System.Drawing.Size(360, 540);
            this._leftPanel.TabIndex = 1;
            // 
            // _leftContentLayout
            // 
            this._leftContentLayout.AutoSize = true;
            this._leftContentLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._leftContentLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._leftContentLayout.ColumnCount = 1;
            this._leftContentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._leftContentLayout.Controls.Add(this._logoPictureBox, 0, 0);
            this._leftContentLayout.Controls.Add(this._brandingTitleLabel, 0, 1);
            this._leftContentLayout.Controls.Add(this._brandingSubtitleLabel, 0, 2);
            this._leftContentLayout.Controls.Add(this._brandingVersionLabel, 0, 3);
            this._leftContentLayout.Controls.Add(this._brandingSeparator, 0, 4);
            this._leftContentLayout.Controls.Add(this._brandingBulletsLabel, 0, 5);
            this._leftContentLayout.Location = new System.Drawing.Point(47, 82);
            this._leftContentLayout.Margin = new System.Windows.Forms.Padding(0);
            this._leftContentLayout.Name = "_leftContentLayout";
            this._leftContentLayout.RowCount = 6;
            this._leftContentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._leftContentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._leftContentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._leftContentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._leftContentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._leftContentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._leftContentLayout.Size = new System.Drawing.Size(230, 303);
            this._leftContentLayout.TabIndex = 0;
            // 
            // _logoPictureBox
            // 
            this._logoPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._logoPictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._logoPictureBox.Image = global::BRCSISTEM.Desktop.Properties.Resources.logo;
            this._logoPictureBox.Location = new System.Drawing.Point(67, 0);
            this._logoPictureBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this._logoPictureBox.Name = "_logoPictureBox";
            this._logoPictureBox.Size = new System.Drawing.Size(96, 96);
            this._logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._logoPictureBox.TabIndex = 0;
            this._logoPictureBox.TabStop = false;
            // 
            // _brandingTitleLabel
            // 
            this._brandingTitleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._brandingTitleLabel.AutoSize = true;
            this._brandingTitleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._brandingTitleLabel.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this._brandingTitleLabel.ForeColor = System.Drawing.Color.White;
            this._brandingTitleLabel.Location = new System.Drawing.Point(23, 111);
            this._brandingTitleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this._brandingTitleLabel.Name = "_brandingTitleLabel";
            this._brandingTitleLabel.Size = new System.Drawing.Size(183, 41);
            this._brandingTitleLabel.TabIndex = 1;
            this._brandingTitleLabel.Text = "BRCSISTEM";
            this._brandingTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _brandingSubtitleLabel
            // 
            this._brandingSubtitleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._brandingSubtitleLabel.AutoSize = true;
            this._brandingSubtitleLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._brandingSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            this._brandingSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(144)))), ((int)(((byte)(194)))));
            this._brandingSubtitleLabel.Location = new System.Drawing.Point(0, 157);
            this._brandingSubtitleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._brandingSubtitleLabel.Name = "_brandingSubtitleLabel";
            this._brandingSubtitleLabel.Size = new System.Drawing.Size(230, 21);
            this._brandingSubtitleLabel.TabIndex = 2;
            this._brandingSubtitleLabel.Text = "Sistema de Controle de Estoque";
            this._brandingSubtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _brandingVersionLabel
            // 
            this._brandingVersionLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._brandingVersionLabel.AutoSize = true;
            this._brandingVersionLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._brandingVersionLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this._brandingVersionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(144)))), ((int)(((byte)(194)))));
            this._brandingVersionLabel.Location = new System.Drawing.Point(69, 181);
            this._brandingVersionLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this._brandingVersionLabel.Name = "_brandingVersionLabel";
            this._brandingVersionLabel.Size = new System.Drawing.Size(92, 17);
            this._brandingVersionLabel.TabIndex = 3;
            this._brandingVersionLabel.Text = "Versao v3.1.20";
            this._brandingVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _brandingSeparator
            // 
            this._brandingSeparator.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._brandingSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(173)))), ((int)(((byte)(0)))));
            this._brandingSeparator.Location = new System.Drawing.Point(30, 213);
            this._brandingSeparator.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this._brandingSeparator.Name = "_brandingSeparator";
            this._brandingSeparator.Size = new System.Drawing.Size(170, 2);
            this._brandingSeparator.TabIndex = 4;
            // 
            // _brandingBulletsLabel
            // 
            this._brandingBulletsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._brandingBulletsLabel.AutoSize = true;
            this._brandingBulletsLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._brandingBulletsLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._brandingBulletsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(144)))), ((int)(((byte)(194)))));
            this._brandingBulletsLabel.Location = new System.Drawing.Point(2, 227);
            this._brandingBulletsLabel.Margin = new System.Windows.Forms.Padding(0);
            this._brandingBulletsLabel.Name = "_brandingBulletsLabel";
            this._brandingBulletsLabel.Size = new System.Drawing.Size(225, 76);
            this._brandingBulletsLabel.TabIndex = 5;
            this._brandingBulletsLabel.Text = "- Gestao Inteligente de Estoque\r\n- Controle Total de Movimentacoes\r\n- Relatorios " +
    "Detalhados\r\n- Acesso Seguro e Auditado";
            this._brandingBulletsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _rightPanel
            // 
            this._rightPanel.BackColor = System.Drawing.Color.White;
            this._rightPanel.Controls.Add(this._rightContentPanel);
            this._rightPanel.Controls.Add(this._footerPanel);
            this._rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rightPanel.Location = new System.Drawing.Point(360, 0);
            this._rightPanel.Name = "_rightPanel";
            this._rightPanel.Size = new System.Drawing.Size(460, 540);
            this._rightPanel.TabIndex = 0;
            // 
            // _rightContentPanel
            // 
            this._rightContentPanel.BackColor = System.Drawing.Color.White;
            this._rightContentPanel.Controls.Add(this._formContainer);
            this._rightContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rightContentPanel.Location = new System.Drawing.Point(0, 0);
            this._rightContentPanel.Name = "_rightContentPanel";
            this._rightContentPanel.Size = new System.Drawing.Size(460, 506);
            this._rightContentPanel.TabIndex = 0;
            // 
            // _formContainer
            // 
            this._formContainer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._formContainer.AutoSize = true;
            this._formContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._formContainer.BackColor = System.Drawing.Color.White;
            this._formContainer.ColumnCount = 1;
            this._formContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
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
            this._formContainer.Location = new System.Drawing.Point(90, 111);
            this._formContainer.Margin = new System.Windows.Forms.Padding(0);
            this._formContainer.Name = "_formContainer";
            this._formContainer.RowCount = 11;
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._formContainer.Size = new System.Drawing.Size(280, 408);
            this._formContainer.TabIndex = 0;
            // 
            // _welcomeLabel
            // 
            this._welcomeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._welcomeLabel.AutoSize = true;
            this._welcomeLabel.BackColor = System.Drawing.Color.White;
            this._welcomeLabel.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this._welcomeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._welcomeLabel.Location = new System.Drawing.Point(0, 0);
            this._welcomeLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this._welcomeLabel.Name = "_welcomeLabel";
            this._welcomeLabel.Size = new System.Drawing.Size(167, 37);
            this._welcomeLabel.TabIndex = 0;
            this._welcomeLabel.Text = "Bem-vindo!";
            // 
            // _welcomeSubtitleLabel
            // 
            this._welcomeSubtitleLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._welcomeSubtitleLabel.AutoSize = true;
            this._welcomeSubtitleLabel.BackColor = System.Drawing.Color.White;
            this._welcomeSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this._welcomeSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this._welcomeSubtitleLabel.Location = new System.Drawing.Point(0, 41);
            this._welcomeSubtitleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this._welcomeSubtitleLabel.Name = "_welcomeSubtitleLabel";
            this._welcomeSubtitleLabel.Size = new System.Drawing.Size(211, 19);
            this._welcomeSubtitleLabel.TabIndex = 1;
            this._welcomeSubtitleLabel.Text = "Faca login para acessar o sistema";
            // 
            // _bancoHeaderPanel
            // 
            this._bancoHeaderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._bancoHeaderPanel.BackColor = System.Drawing.Color.White;
            this._bancoHeaderPanel.Controls.Add(this._bancoLabel);
            this._bancoHeaderPanel.Controls.Add(this._cacheIconButton);
            this._bancoHeaderPanel.Controls.Add(this._configIconButton);
            this._bancoHeaderPanel.Location = new System.Drawing.Point(0, 80);
            this._bancoHeaderPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._bancoHeaderPanel.Name = "_bancoHeaderPanel";
            this._bancoHeaderPanel.Size = new System.Drawing.Size(280, 30);
            this._bancoHeaderPanel.TabIndex = 2;
            // 
            // _bancoLabel
            // 
            this._bancoLabel.BackColor = System.Drawing.Color.White;
            this._bancoLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._bancoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._bancoLabel.Location = new System.Drawing.Point(0, 6);
            this._bancoLabel.Name = "_bancoLabel";
            this._bancoLabel.Size = new System.Drawing.Size(212, 20);
            this._bancoLabel.TabIndex = 0;
            this._bancoLabel.Text = "Banco de Dados";
            this._bancoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _cacheIconButton
            // 
            this._cacheIconButton.BackColor = System.Drawing.Color.White;
            this._cacheIconButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._cacheIconButton.FlatAppearance.BorderSize = 0;
            this._cacheIconButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._cacheIconButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._cacheIconButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this._cacheIconButton.Location = new System.Drawing.Point(216, 2);
            this._cacheIconButton.Name = "_cacheIconButton";
            this._cacheIconButton.Size = new System.Drawing.Size(26, 26);
            this._cacheIconButton.TabIndex = 1;
            this._cacheIconButton.TabStop = false;
            this._cacheIconButton.UseVisualStyleBackColor = false;
            // 
            // _configIconButton
            // 
            this._configIconButton.BackColor = System.Drawing.Color.White;
            this._configIconButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._configIconButton.FlatAppearance.BorderSize = 0;
            this._configIconButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._configIconButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._configIconButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this._configIconButton.Location = new System.Drawing.Point(248, 2);
            this._configIconButton.Name = "_configIconButton";
            this._configIconButton.Size = new System.Drawing.Size(26, 26);
            this._configIconButton.TabIndex = 2;
            this._configIconButton.TabStop = false;
            this._configIconButton.UseVisualStyleBackColor = false;
            // 
            // _profilesComboBox
            // 
            this._profilesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._profilesComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._profilesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._profilesComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._profilesComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._profilesComboBox.FormattingEnabled = true;
            this._profilesComboBox.Location = new System.Drawing.Point(0, 113);
            this._profilesComboBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this._profilesComboBox.Name = "_profilesComboBox";
            this._profilesComboBox.Size = new System.Drawing.Size(280, 23);
            this._profilesComboBox.TabIndex = 3;
            // 
            // _userLabel
            // 
            this._userLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._userLabel.AutoSize = true;
            this._userLabel.BackColor = System.Drawing.Color.White;
            this._userLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._userLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._userLabel.Location = new System.Drawing.Point(0, 152);
            this._userLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this._userLabel.Name = "_userLabel";
            this._userLabel.Size = new System.Drawing.Size(49, 15);
            this._userLabel.TabIndex = 4;
            this._userLabel.Text = "Usuario";
            // 
            // _userHostPanel
            // 
            this._userHostPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._userHostPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._userHostPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._userHostPanel.Controls.Add(this._userNameTextBox);
            this._userHostPanel.Location = new System.Drawing.Point(0, 169);
            this._userHostPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._userHostPanel.Name = "_userHostPanel";
            this._userHostPanel.Padding = new System.Windows.Forms.Padding(10, 8, 10, 6);
            this._userHostPanel.Size = new System.Drawing.Size(280, 38);
            this._userHostPanel.TabIndex = 5;
            // 
            // _userNameTextBox
            // 
            this._userNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._userNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._userNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._userNameTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._userNameTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this._userNameTextBox.Location = new System.Drawing.Point(10, 8);
            this._userNameTextBox.Name = "_userNameTextBox";
            this._userNameTextBox.Size = new System.Drawing.Size(258, 18);
            this._userNameTextBox.TabIndex = 0;
            // 
            // _passwordLabel
            // 
            this._passwordLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._passwordLabel.AutoSize = true;
            this._passwordLabel.BackColor = System.Drawing.Color.White;
            this._passwordLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._passwordLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._passwordLabel.Location = new System.Drawing.Point(0, 217);
            this._passwordLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this._passwordLabel.Name = "_passwordLabel";
            this._passwordLabel.Size = new System.Drawing.Size(41, 15);
            this._passwordLabel.TabIndex = 6;
            this._passwordLabel.Text = "Senha";
            // 
            // _passwordHostPanel
            // 
            this._passwordHostPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._passwordHostPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._passwordHostPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._passwordHostPanel.Controls.Add(this._passwordTextBox);
            this._passwordHostPanel.Location = new System.Drawing.Point(0, 234);
            this._passwordHostPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._passwordHostPanel.Name = "_passwordHostPanel";
            this._passwordHostPanel.Padding = new System.Windows.Forms.Padding(10, 8, 10, 6);
            this._passwordHostPanel.Size = new System.Drawing.Size(280, 38);
            this._passwordHostPanel.TabIndex = 7;
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._passwordTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._passwordTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._passwordTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this._passwordTextBox.Location = new System.Drawing.Point(10, 8);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new System.Drawing.Size(258, 18);
            this._passwordTextBox.TabIndex = 0;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _statusLabel
            // 
            this._statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._statusLabel.BackColor = System.Drawing.Color.White;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this._statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this._statusLabel.Location = new System.Drawing.Point(0, 282);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(280, 22);
            this._statusLabel.TabIndex = 8;
            this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _loginButton
            // 
            this._loginButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._loginButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._loginButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._loginButton.FlatAppearance.BorderSize = 0;
            this._loginButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(89)))), ((int)(((byte)(132)))));
            this._loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._loginButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._loginButton.ForeColor = System.Drawing.Color.White;
            this._loginButton.Location = new System.Drawing.Point(0, 314);
            this._loginButton.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this._loginButton.Name = "_loginButton";
            this._loginButton.Size = new System.Drawing.Size(280, 38);
            this._loginButton.TabIndex = 9;
            this._loginButton.Text = "ACESSAR SISTEMA";
            this._loginButton.UseVisualStyleBackColor = false;
            // 
            // _secondaryButtonsLayout
            // 
            this._secondaryButtonsLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._secondaryButtonsLayout.BackColor = System.Drawing.Color.White;
            this._secondaryButtonsLayout.ColumnCount = 2;
            this._secondaryButtonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._secondaryButtonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._secondaryButtonsLayout.Controls.Add(this._requestAccessButton, 0, 0);
            this._secondaryButtonsLayout.Controls.Add(this._closeButton, 1, 0);
            this._secondaryButtonsLayout.Location = new System.Drawing.Point(0, 360);
            this._secondaryButtonsLayout.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this._secondaryButtonsLayout.Name = "_secondaryButtonsLayout";
            this._secondaryButtonsLayout.RowCount = 1;
            this._secondaryButtonsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._secondaryButtonsLayout.Size = new System.Drawing.Size(280, 36);
            this._secondaryButtonsLayout.TabIndex = 10;
            // 
            // _requestAccessButton
            // 
            this._requestAccessButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this._requestAccessButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._requestAccessButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._requestAccessButton.FlatAppearance.BorderSize = 0;
            this._requestAccessButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this._requestAccessButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._requestAccessButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._requestAccessButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._requestAccessButton.Location = new System.Drawing.Point(0, 0);
            this._requestAccessButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this._requestAccessButton.Name = "_requestAccessButton";
            this._requestAccessButton.Size = new System.Drawing.Size(135, 36);
            this._requestAccessButton.TabIndex = 0;
            this._requestAccessButton.Text = "Solicitar Acesso";
            this._requestAccessButton.UseVisualStyleBackColor = false;
            // 
            // _closeButton
            // 
            this._closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
            this._closeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._closeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._closeButton.FlatAppearance.BorderSize = 0;
            this._closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._closeButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._closeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._closeButton.Location = new System.Drawing.Point(145, 0);
            this._closeButton.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(135, 36);
            this._closeButton.TabIndex = 1;
            this._closeButton.Text = "Fechar";
            this._closeButton.UseVisualStyleBackColor = false;
            // 
            // _footerPanel
            // 
            this._footerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._footerPanel.Controls.Add(this._footerLabel);
            this._footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._footerPanel.Location = new System.Drawing.Point(0, 506);
            this._footerPanel.Name = "_footerPanel";
            this._footerPanel.Size = new System.Drawing.Size(460, 34);
            this._footerPanel.TabIndex = 1;
            // 
            // _footerLabel
            // 
            this._footerLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._footerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._footerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(80)))), ((int)(((byte)(87)))));
            this._footerLabel.Location = new System.Drawing.Point(0, 0);
            this._footerLabel.Name = "_footerLabel";
            this._footerLabel.Size = new System.Drawing.Size(460, 34);
            this._footerLabel.TabIndex = 0;
            this._footerLabel.Text = " 2025 BRCSISTEM - Todos os direitos reservados ";
            this._footerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(820, 540);
            this.Controls.Add(this._rightPanel);
            this.Controls.Add(this._leftPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BRCSISTEM v3.1.20 - Acesso ao Sistema";
            this._leftPanel.ResumeLayout(false);
            this._leftPanel.PerformLayout();
            this._leftContentLayout.ResumeLayout(false);
            this._leftContentLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._logoPictureBox)).EndInit();
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
