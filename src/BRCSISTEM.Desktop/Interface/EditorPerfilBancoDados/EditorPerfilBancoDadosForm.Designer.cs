using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.EditorPerfilBancoDados
{
    public sealed partial class EditorPerfilBancoDadosForm
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
            this._rootLayout = new TableLayoutPanel();
            this._headerLabel = new Label();
            this._formGroup = new GroupBox();
            this._formLayout = new TableLayoutPanel();
            this._sectionIdLabel = new Label();
            this._nameLabel = new Label();
            this._nameTextBox = new TextBox();
            this._nameHintLabel = new Label();
            this._descriptionLabel = new Label();
            this._descriptionTextBox = new TextBox();
            this._descriptionHintLabel = new Label();
            this._separator1 = new Label();
            this._sectionServerLabel = new Label();
            this._hostLabel = new Label();
            this._hostTextBox = new TextBox();
            this._hostHintLabel = new Label();
            this._portLabel = new Label();
            this._portNumericUpDown = new NumericUpDown();
            this._portHintLabel = new Label();
            this._separator2 = new Label();
            this._sectionDbLabel = new Label();
            this._databaseLabel = new Label();
            this._databaseTextBox = new TextBox();
            this._databaseHintLabel = new Label();
            this._separator3 = new Label();
            this._sectionAuthLabel = new Label();
            this._userLabel = new Label();
            this._userTextBox = new TextBox();
            this._userHintLabel = new Label();
            this._passwordLabel = new Label();
            this._passwordTextBox = new TextBox();
            this._passwordHintLabel = new Label();
            this._statusLabel = new Label();
            this._buttonsPanel = new TableLayoutPanel();
            this._leftButtonsPanel = new FlowLayoutPanel();
            this._saveButton = new Button();
            this._testButton = new Button();
            this._cancelButton = new Button();
            this._rootLayout.SuspendLayout();
            this._formGroup.SuspendLayout();
            this._formLayout.SuspendLayout();
            ((ISupportInitialize)(this._portNumericUpDown)).BeginInit();
            this._buttonsPanel.SuspendLayout();
            this._leftButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerLabel, 0, 0);
            this._rootLayout.Controls.Add(this._formGroup, 0, 1);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 2);
            this._rootLayout.Controls.Add(this._buttonsPanel, 0, 3);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Location = new Point(0, 0);
            this._rootLayout.Margin = new Padding(0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new Padding(20);
            this._rootLayout.RowCount = 4;
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.Size = new Size(550, 700);
            this._rootLayout.TabIndex = 0;
            // 
            // _headerLabel
            // 
            this._headerLabel.AutoSize = true;
            this._headerLabel.Dock = DockStyle.Top;
            this._headerLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._headerLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._headerLabel.Location = new Point(20, 20);
            this._headerLabel.Margin = new Padding(0, 0, 0, 15);
            this._headerLabel.Name = "_headerLabel";
            this._headerLabel.Size = new Size(510, 21);
            this._headerLabel.TabIndex = 0;
            this._headerLabel.Text = "Nova Configuracao";
            // 
            // _formGroup
            // 
            this._formGroup.Controls.Add(this._formLayout);
            this._formGroup.Dock = DockStyle.Fill;
            this._formGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._formGroup.Location = new Point(20, 56);
            this._formGroup.Margin = new Padding(0);
            this._formGroup.Name = "_formGroup";
            this._formGroup.Padding = new Padding(10);
            this._formGroup.Size = new Size(510, 561);
            this._formGroup.TabIndex = 1;
            this._formGroup.TabStop = false;
            this._formGroup.Text = " Configuracoes ";
            // 
            // _formLayout
            // 
            this._formLayout.ColumnCount = 2;
            this._formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._formLayout.Controls.Add(this._sectionIdLabel, 0, 0);
            this._formLayout.Controls.Add(this._nameLabel, 0, 1);
            this._formLayout.Controls.Add(this._nameTextBox, 1, 1);
            this._formLayout.Controls.Add(this._nameHintLabel, 1, 2);
            this._formLayout.Controls.Add(this._descriptionLabel, 0, 3);
            this._formLayout.Controls.Add(this._descriptionTextBox, 1, 3);
            this._formLayout.Controls.Add(this._descriptionHintLabel, 1, 4);
            this._formLayout.Controls.Add(this._separator1, 0, 5);
            this._formLayout.Controls.Add(this._sectionServerLabel, 0, 6);
            this._formLayout.Controls.Add(this._hostLabel, 0, 7);
            this._formLayout.Controls.Add(this._hostTextBox, 1, 7);
            this._formLayout.Controls.Add(this._hostHintLabel, 1, 8);
            this._formLayout.Controls.Add(this._portLabel, 0, 9);
            this._formLayout.Controls.Add(this._portNumericUpDown, 1, 9);
            this._formLayout.Controls.Add(this._portHintLabel, 1, 10);
            this._formLayout.Controls.Add(this._separator2, 0, 11);
            this._formLayout.Controls.Add(this._sectionDbLabel, 0, 12);
            this._formLayout.Controls.Add(this._databaseLabel, 0, 13);
            this._formLayout.Controls.Add(this._databaseTextBox, 1, 13);
            this._formLayout.Controls.Add(this._databaseHintLabel, 1, 14);
            this._formLayout.Controls.Add(this._separator3, 0, 15);
            this._formLayout.Controls.Add(this._sectionAuthLabel, 0, 16);
            this._formLayout.Controls.Add(this._userLabel, 0, 17);
            this._formLayout.Controls.Add(this._userTextBox, 1, 17);
            this._formLayout.Controls.Add(this._userHintLabel, 1, 18);
            this._formLayout.Controls.Add(this._passwordLabel, 0, 19);
            this._formLayout.Controls.Add(this._passwordTextBox, 1, 19);
            this._formLayout.Controls.Add(this._passwordHintLabel, 1, 20);
            this._formLayout.Dock = DockStyle.Fill;
            this._formLayout.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._formLayout.Location = new Point(10, 26);
            this._formLayout.Margin = new Padding(0);
            this._formLayout.Name = "_formLayout";
            this._formLayout.RowCount = 21;
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.Size = new Size(490, 525);
            this._formLayout.TabIndex = 0;
            this._formLayout.SetColumnSpan(this._sectionIdLabel, 2);
            this._formLayout.SetColumnSpan(this._separator1, 2);
            this._formLayout.SetColumnSpan(this._sectionServerLabel, 2);
            this._formLayout.SetColumnSpan(this._separator2, 2);
            this._formLayout.SetColumnSpan(this._sectionDbLabel, 2);
            this._formLayout.SetColumnSpan(this._separator3, 2);
            this._formLayout.SetColumnSpan(this._sectionAuthLabel, 2);
            // 
            // _sectionIdLabel
            // 
            this._sectionIdLabel.AutoSize = true;
            this._sectionIdLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._sectionIdLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._sectionIdLabel.Location = new Point(0, 5);
            this._sectionIdLabel.Margin = new Padding(0, 5, 0, 3);
            this._sectionIdLabel.Name = "_sectionIdLabel";
            this._sectionIdLabel.Size = new Size(76, 13);
            this._sectionIdLabel.TabIndex = 0;
            this._sectionIdLabel.Text = "Identificacao";
            // 
            // _nameLabel
            // 
            this._nameLabel.AutoSize = true;
            this._nameLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._nameLabel.Location = new Point(0, 29);
            this._nameLabel.Margin = new Padding(0, 8, 10, 0);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new Size(39, 13);
            this._nameLabel.TabIndex = 1;
            this._nameLabel.Text = "Nome:";
            // 
            // _nameTextBox
            // 
            this._nameTextBox.Dock = DockStyle.Top;
            this._nameTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._nameTextBox.Location = new Point(110, 26);
            this._nameTextBox.Margin = new Padding(0, 5, 0, 0);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new Size(380, 22);
            this._nameTextBox.TabIndex = 2;
            // 
            // _nameHintLabel
            // 
            this._nameHintLabel.AutoSize = true;
            this._nameHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._nameHintLabel.ForeColor = Color.Gray;
            this._nameHintLabel.Location = new Point(110, 48);
            this._nameHintLabel.Margin = new Padding(0, 0, 0, 5);
            this._nameHintLabel.Name = "_nameHintLabel";
            this._nameHintLabel.Size = new Size(64, 13);
            this._nameHintLabel.TabIndex = 3;
            this._nameHintLabel.Text = "Nome unico";
            // 
            // _descriptionLabel
            // 
            this._descriptionLabel.AutoSize = true;
            this._descriptionLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._descriptionLabel.Location = new Point(0, 74);
            this._descriptionLabel.Margin = new Padding(0, 8, 10, 0);
            this._descriptionLabel.Name = "_descriptionLabel";
            this._descriptionLabel.Size = new Size(58, 13);
            this._descriptionLabel.TabIndex = 4;
            this._descriptionLabel.Text = "Descricao:";
            // 
            // _descriptionTextBox
            // 
            this._descriptionTextBox.Dock = DockStyle.Top;
            this._descriptionTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._descriptionTextBox.Location = new Point(110, 71);
            this._descriptionTextBox.Margin = new Padding(0, 5, 0, 0);
            this._descriptionTextBox.Name = "_descriptionTextBox";
            this._descriptionTextBox.Size = new Size(380, 22);
            this._descriptionTextBox.TabIndex = 5;
            // 
            // _descriptionHintLabel
            // 
            this._descriptionHintLabel.AutoSize = true;
            this._descriptionHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._descriptionHintLabel.ForeColor = Color.Gray;
            this._descriptionHintLabel.Location = new Point(110, 93);
            this._descriptionHintLabel.Margin = new Padding(0, 0, 0, 5);
            this._descriptionHintLabel.Name = "_descriptionHintLabel";
            this._descriptionHintLabel.Size = new Size(105, 13);
            this._descriptionHintLabel.TabIndex = 6;
            this._descriptionHintLabel.Text = "Ex: Producao, Testes";
            // 
            // _separator1
            // 
            this._separator1.AutoSize = false;
            this._separator1.BackColor = Color.LightGray;
            this._separator1.Dock = DockStyle.Top;
            this._separator1.Location = new Point(0, 119);
            this._separator1.Margin = new Padding(0, 8, 0, 8);
            this._separator1.Name = "_separator1";
            this._separator1.Size = new Size(490, 1);
            this._separator1.TabIndex = 7;
            // 
            // _sectionServerLabel
            // 
            this._sectionServerLabel.AutoSize = true;
            this._sectionServerLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._sectionServerLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._sectionServerLabel.Location = new Point(0, 136);
            this._sectionServerLabel.Margin = new Padding(0, 5, 0, 3);
            this._sectionServerLabel.Name = "_sectionServerLabel";
            this._sectionServerLabel.Size = new Size(53, 13);
            this._sectionServerLabel.TabIndex = 8;
            this._sectionServerLabel.Text = "Servidor";
            // 
            // _hostLabel
            // 
            this._hostLabel.AutoSize = true;
            this._hostLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._hostLabel.Location = new Point(0, 160);
            this._hostLabel.Margin = new Padding(0, 8, 10, 0);
            this._hostLabel.Name = "_hostLabel";
            this._hostLabel.Size = new Size(34, 13);
            this._hostLabel.TabIndex = 9;
            this._hostLabel.Text = "Host:";
            // 
            // _hostTextBox
            // 
            this._hostTextBox.Dock = DockStyle.Top;
            this._hostTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._hostTextBox.Location = new Point(110, 157);
            this._hostTextBox.Margin = new Padding(0, 5, 0, 0);
            this._hostTextBox.Name = "_hostTextBox";
            this._hostTextBox.Size = new Size(380, 22);
            this._hostTextBox.TabIndex = 10;
            // 
            // _hostHintLabel
            // 
            this._hostHintLabel.AutoSize = true;
            this._hostHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._hostHintLabel.ForeColor = Color.Gray;
            this._hostHintLabel.Location = new Point(110, 179);
            this._hostHintLabel.Margin = new Padding(0, 0, 0, 5);
            this._hostHintLabel.Name = "_hostHintLabel";
            this._hostHintLabel.Size = new Size(76, 13);
            this._hostHintLabel.TabIndex = 11;
            this._hostHintLabel.Text = "localhost ou IP";
            // 
            // _portLabel
            // 
            this._portLabel.AutoSize = true;
            this._portLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._portLabel.Location = new Point(0, 205);
            this._portLabel.Margin = new Padding(0, 8, 10, 0);
            this._portLabel.Name = "_portLabel";
            this._portLabel.Size = new Size(37, 13);
            this._portLabel.TabIndex = 12;
            this._portLabel.Text = "Porta:";
            // 
            // _portNumericUpDown
            // 
            this._portNumericUpDown.Anchor = AnchorStyles.Left;
            this._portNumericUpDown.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._portNumericUpDown.Location = new Point(110, 202);
            this._portNumericUpDown.Margin = new Padding(0, 5, 0, 5);
            this._portNumericUpDown.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this._portNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this._portNumericUpDown.Name = "_portNumericUpDown";
            this._portNumericUpDown.Size = new Size(130, 22);
            this._portNumericUpDown.TabIndex = 13;
            this._portNumericUpDown.Value = new decimal(new int[] { 5432, 0, 0, 0 });
            // 
            // _portHintLabel
            // 
            this._portHintLabel.AutoSize = true;
            this._portHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._portHintLabel.ForeColor = Color.Gray;
            this._portHintLabel.Location = new Point(110, 229);
            this._portHintLabel.Margin = new Padding(0, 0, 0, 5);
            this._portHintLabel.Name = "_portHintLabel";
            this._portHintLabel.Size = new Size(71, 13);
            this._portHintLabel.TabIndex = 14;
            this._portHintLabel.Text = "Padrao: 5432";
            // 
            // _separator2
            // 
            this._separator2.AutoSize = false;
            this._separator2.BackColor = Color.LightGray;
            this._separator2.Dock = DockStyle.Top;
            this._separator2.Location = new Point(0, 255);
            this._separator2.Margin = new Padding(0, 8, 0, 8);
            this._separator2.Name = "_separator2";
            this._separator2.Size = new Size(490, 1);
            this._separator2.TabIndex = 15;
            // 
            // _sectionDbLabel
            // 
            this._sectionDbLabel.AutoSize = true;
            this._sectionDbLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._sectionDbLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._sectionDbLabel.Location = new Point(0, 272);
            this._sectionDbLabel.Margin = new Padding(0, 5, 0, 3);
            this._sectionDbLabel.Name = "_sectionDbLabel";
            this._sectionDbLabel.Size = new Size(91, 13);
            this._sectionDbLabel.TabIndex = 16;
            this._sectionDbLabel.Text = "Banco de Dados";
            // 
            // _databaseLabel
            // 
            this._databaseLabel.AutoSize = true;
            this._databaseLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._databaseLabel.Location = new Point(0, 296);
            this._databaseLabel.Margin = new Padding(0, 8, 10, 0);
            this._databaseLabel.Name = "_databaseLabel";
            this._databaseLabel.Size = new Size(55, 13);
            this._databaseLabel.TabIndex = 17;
            this._databaseLabel.Text = "Database:";
            // 
            // _databaseTextBox
            // 
            this._databaseTextBox.Dock = DockStyle.Top;
            this._databaseTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._databaseTextBox.Location = new Point(110, 293);
            this._databaseTextBox.Margin = new Padding(0, 5, 0, 0);
            this._databaseTextBox.Name = "_databaseTextBox";
            this._databaseTextBox.Size = new Size(380, 22);
            this._databaseTextBox.TabIndex = 18;
            // 
            // _databaseHintLabel
            // 
            this._databaseHintLabel.AutoSize = true;
            this._databaseHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._databaseHintLabel.ForeColor = Color.Gray;
            this._databaseHintLabel.Location = new Point(110, 315);
            this._databaseHintLabel.Margin = new Padding(0, 0, 0, 5);
            this._databaseHintLabel.Name = "_databaseHintLabel";
            this._databaseHintLabel.Size = new Size(81, 13);
            this._databaseHintLabel.TabIndex = 19;
            this._databaseHintLabel.Text = "Nome do banco";
            // 
            // _separator3
            // 
            this._separator3.AutoSize = false;
            this._separator3.BackColor = Color.LightGray;
            this._separator3.Dock = DockStyle.Top;
            this._separator3.Location = new Point(0, 341);
            this._separator3.Margin = new Padding(0, 8, 0, 8);
            this._separator3.Name = "_separator3";
            this._separator3.Size = new Size(490, 1);
            this._separator3.TabIndex = 20;
            // 
            // _sectionAuthLabel
            // 
            this._sectionAuthLabel.AutoSize = true;
            this._sectionAuthLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._sectionAuthLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._sectionAuthLabel.Location = new Point(0, 358);
            this._sectionAuthLabel.Margin = new Padding(0, 5, 0, 3);
            this._sectionAuthLabel.Name = "_sectionAuthLabel";
            this._sectionAuthLabel.Size = new Size(74, 13);
            this._sectionAuthLabel.TabIndex = 21;
            this._sectionAuthLabel.Text = "Autenticacao";
            // 
            // _userLabel
            // 
            this._userLabel.AutoSize = true;
            this._userLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._userLabel.Location = new Point(0, 382);
            this._userLabel.Margin = new Padding(0, 8, 10, 0);
            this._userLabel.Name = "_userLabel";
            this._userLabel.Size = new Size(47, 13);
            this._userLabel.TabIndex = 22;
            this._userLabel.Text = "Usuario:";
            // 
            // _userTextBox
            // 
            this._userTextBox.Dock = DockStyle.Top;
            this._userTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._userTextBox.Location = new Point(110, 379);
            this._userTextBox.Margin = new Padding(0, 5, 0, 0);
            this._userTextBox.Name = "_userTextBox";
            this._userTextBox.Size = new Size(380, 22);
            this._userTextBox.TabIndex = 23;
            // 
            // _userHintLabel
            // 
            this._userHintLabel.AutoSize = true;
            this._userHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._userHintLabel.ForeColor = Color.Gray;
            this._userHintLabel.Location = new Point(110, 401);
            this._userHintLabel.Margin = new Padding(0, 0, 0, 5);
            this._userHintLabel.Name = "_userHintLabel";
            this._userHintLabel.Size = new Size(94, 13);
            this._userHintLabel.TabIndex = 24;
            this._userHintLabel.Text = "Usuario PostgreSQL";
            // 
            // _passwordLabel
            // 
            this._passwordLabel.AutoSize = true;
            this._passwordLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._passwordLabel.Location = new Point(0, 427);
            this._passwordLabel.Margin = new Padding(0, 8, 10, 0);
            this._passwordLabel.Name = "_passwordLabel";
            this._passwordLabel.Size = new Size(41, 13);
            this._passwordLabel.TabIndex = 25;
            this._passwordLabel.Text = "Senha:";
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.Dock = DockStyle.Top;
            this._passwordTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._passwordTextBox.Location = new Point(110, 424);
            this._passwordTextBox.Margin = new Padding(0, 5, 0, 0);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new Size(380, 22);
            this._passwordTextBox.TabIndex = 26;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _passwordHintLabel
            // 
            this._passwordHintLabel.AutoSize = true;
            this._passwordHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._passwordHintLabel.ForeColor = Color.Gray;
            this._passwordHintLabel.Location = new Point(110, 446);
            this._passwordHintLabel.Margin = new Padding(0, 0, 0, 5);
            this._passwordHintLabel.Name = "_passwordHintLabel";
            this._passwordHintLabel.Size = new Size(91, 13);
            this._passwordHintLabel.TabIndex = 27;
            this._passwordHintLabel.Text = "Senha do usuario";
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Dock = DockStyle.Top;
            this._statusLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._statusLabel.ForeColor = Color.SeaGreen;
            this._statusLabel.Location = new Point(20, 627);
            this._statusLabel.Margin = new Padding(0, 10, 0, 5);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new Size(510, 13);
            this._statusLabel.TabIndex = 2;
            // 
            // _buttonsPanel
            // 
            this._buttonsPanel.ColumnCount = 2;
            this._buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._buttonsPanel.ColumnStyles.Add(new ColumnStyle());
            this._buttonsPanel.Controls.Add(this._leftButtonsPanel, 0, 0);
            this._buttonsPanel.Controls.Add(this._cancelButton, 1, 0);
            this._buttonsPanel.Dock = DockStyle.Fill;
            this._buttonsPanel.Location = new Point(20, 650);
            this._buttonsPanel.Margin = new Padding(0, 5, 0, 0);
            this._buttonsPanel.Name = "_buttonsPanel";
            this._buttonsPanel.RowCount = 1;
            this._buttonsPanel.RowStyles.Add(new RowStyle());
            this._buttonsPanel.Size = new Size(510, 30);
            this._buttonsPanel.TabIndex = 3;
            // 
            // _leftButtonsPanel
            // 
            this._leftButtonsPanel.AutoSize = true;
            this._leftButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._leftButtonsPanel.Controls.Add(this._saveButton);
            this._leftButtonsPanel.Controls.Add(this._testButton);
            this._leftButtonsPanel.Dock = DockStyle.Left;
            this._leftButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
            this._leftButtonsPanel.Location = new Point(0, 0);
            this._leftButtonsPanel.Margin = new Padding(0);
            this._leftButtonsPanel.Name = "_leftButtonsPanel";
            this._leftButtonsPanel.Size = new Size(240, 30);
            this._leftButtonsPanel.TabIndex = 0;
            // 
            // _saveButton
            // 
            this._saveButton.FlatStyle = FlatStyle.System;
            this._saveButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._saveButton.Location = new Point(0, 0);
            this._saveButton.Margin = new Padding(0, 0, 5, 0);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new Size(100, 28);
            this._saveButton.TabIndex = 0;
            this._saveButton.Text = "Salvar";
            this._saveButton.UseVisualStyleBackColor = true;
            // 
            // _testButton
            // 
            this._testButton.FlatStyle = FlatStyle.System;
            this._testButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._testButton.Location = new Point(110, 0);
            this._testButton.Margin = new Padding(5, 0, 0, 0);
            this._testButton.Name = "_testButton";
            this._testButton.Size = new Size(130, 28);
            this._testButton.TabIndex = 1;
            this._testButton.Text = "Testar Conexao";
            this._testButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = AnchorStyles.Right;
            this._cancelButton.FlatStyle = FlatStyle.System;
            this._cancelButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._cancelButton.Location = new Point(410, 1);
            this._cancelButton.Margin = new Padding(0);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(100, 28);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancelar";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // EditorPerfilBancoDadosForm
            // 
            this.AcceptButton = this._saveButton;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new Size(550, 700);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorPerfilBancoDadosForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Novo Banco";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._formGroup.ResumeLayout(false);
            this._formLayout.ResumeLayout(false);
            this._formLayout.PerformLayout();
            ((ISupportInitialize)(this._portNumericUpDown)).EndInit();
            this._buttonsPanel.ResumeLayout(false);
            this._buttonsPanel.PerformLayout();
            this._leftButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private Label _headerLabel;
        private GroupBox _formGroup;
        private TableLayoutPanel _formLayout;
        private Label _sectionIdLabel;
        private Label _nameLabel;
        private TextBox _nameTextBox;
        private Label _nameHintLabel;
        private Label _descriptionLabel;
        private TextBox _descriptionTextBox;
        private Label _descriptionHintLabel;
        private Label _separator1;
        private Label _sectionServerLabel;
        private Label _hostLabel;
        private TextBox _hostTextBox;
        private Label _hostHintLabel;
        private Label _portLabel;
        private NumericUpDown _portNumericUpDown;
        private Label _portHintLabel;
        private Label _separator2;
        private Label _sectionDbLabel;
        private Label _databaseLabel;
        private TextBox _databaseTextBox;
        private Label _databaseHintLabel;
        private Label _separator3;
        private Label _sectionAuthLabel;
        private Label _userLabel;
        private TextBox _userTextBox;
        private Label _userHintLabel;
        private Label _passwordLabel;
        private TextBox _passwordTextBox;
        private Label _passwordHintLabel;
        private Label _statusLabel;
        private TableLayoutPanel _buttonsPanel;
        private FlowLayoutPanel _leftButtonsPanel;
        private Button _saveButton;
        private Button _testButton;
        private Button _cancelButton;
    }
}
