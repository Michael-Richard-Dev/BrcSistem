using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class DatabaseProfilesForm
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
            this._headerPanel = new FlowLayoutPanel();
            this._headerTitleLabel = new Label();
            this._headerSubtitleLabel = new Label();
            this._dataArea = new TableLayoutPanel();
            this._profilesGroup = new GroupBox();
            this._profilesListView = new ListView();
            this._colId = new ColumnHeader();
            this._colName = new ColumnHeader();
            this._colDescription = new ColumnHeader();
            this._colKind = new ColumnHeader();
            this._colHost = new ColumnHeader();
            this._colDatabase = new ColumnHeader();
            this._colStatus = new ColumnHeader();
            this._formGroup = new GroupBox();
            this._formLayout = new TableLayoutPanel();
            this._nameLabel = new Label();
            this._nameTextBox = new TextBox();
            this._descriptionLabel = new Label();
            this._descriptionTextBox = new TextBox();
            this._hostLabel = new Label();
            this._hostTextBox = new TextBox();
            this._portLabel = new Label();
            this._portNumericUpDown = new NumericUpDown();
            this._databaseLabel = new Label();
            this._databaseTextBox = new TextBox();
            this._userFormLabel = new Label();
            this._userTextBox = new TextBox();
            this._passwordFormLabel = new Label();
            this._passwordTextBox = new TextBox();
            this._statusLabel = new Label();
            this._formActionsPanel = new FlowLayoutPanel();
            this._saveButton = new Button();
            this._testButton = new Button();
            this._actionBar = new TableLayoutPanel();
            this._configGroup = new GroupBox();
            this._configGroupLayout = new TableLayoutPanel();
            this._configCaptionLabel = new Label();
            this._configButtonsPanel = new FlowLayoutPanel();
            this._newButton = new Button();
            this._deleteButton = new Button();
            this._activateButton = new Button();
            this._closeButton = new Button();
            this._rootLayout.SuspendLayout();
            this._headerPanel.SuspendLayout();
            this._dataArea.SuspendLayout();
            this._profilesGroup.SuspendLayout();
            this._formGroup.SuspendLayout();
            this._formLayout.SuspendLayout();
            ((ISupportInitialize)(this._portNumericUpDown)).BeginInit();
            this._formActionsPanel.SuspendLayout();
            this._actionBar.SuspendLayout();
            this._configGroup.SuspendLayout();
            this._configGroupLayout.SuspendLayout();
            this._configButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // _rootLayout
            //
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerPanel, 0, 0);
            this._rootLayout.Controls.Add(this._dataArea, 0, 1);
            this._rootLayout.Controls.Add(this._actionBar, 0, 2);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Location = new Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new Padding(15);
            this._rootLayout.RowCount = 3;
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.Size = new Size(1100, 600);
            this._rootLayout.TabIndex = 0;
            //
            // _headerPanel
            //
            this._headerPanel.AutoSize = true;
            this._headerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._headerPanel.Controls.Add(this._headerTitleLabel);
            this._headerPanel.Controls.Add(this._headerSubtitleLabel);
            this._headerPanel.Dock = DockStyle.Fill;
            this._headerPanel.FlowDirection = FlowDirection.TopDown;
            this._headerPanel.Location = new Point(18, 18);
            this._headerPanel.Margin = new Padding(0, 0, 0, 10);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Size = new Size(1064, 46);
            this._headerPanel.TabIndex = 0;
            this._headerPanel.WrapContents = false;
            //
            // _headerTitleLabel
            //
            this._headerTitleLabel.AutoSize = true;
            this._headerTitleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this._headerTitleLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._headerTitleLabel.Location = new Point(3, 0);
            this._headerTitleLabel.Margin = new Padding(3, 0, 3, 0);
            this._headerTitleLabel.Name = "_headerTitleLabel";
            this._headerTitleLabel.Size = new Size(292, 25);
            this._headerTitleLabel.TabIndex = 0;
            this._headerTitleLabel.Text = "Gerenciador de Bancos de Dados";
            //
            // _headerSubtitleLabel
            //
            this._headerSubtitleLabel.AutoSize = true;
            this._headerSubtitleLabel.Font = new Font("Segoe UI", 9F);
            this._headerSubtitleLabel.ForeColor = Color.Gray;
            this._headerSubtitleLabel.Location = new Point(3, 25);
            this._headerSubtitleLabel.Margin = new Padding(3, 5, 3, 0);
            this._headerSubtitleLabel.Name = "_headerSubtitleLabel";
            this._headerSubtitleLabel.Size = new Size(340, 15);
            this._headerSubtitleLabel.TabIndex = 1;
            this._headerSubtitleLabel.Text = "Gerencia configuracoes de acesso aos bancos PostgreSQL";
            //
            // _dataArea
            //
            this._dataArea.ColumnCount = 2;
            this._dataArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._dataArea.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420F));
            this._dataArea.Controls.Add(this._profilesGroup, 0, 0);
            this._dataArea.Controls.Add(this._formGroup, 1, 0);
            this._dataArea.Dock = DockStyle.Fill;
            this._dataArea.Location = new Point(15, 74);
            this._dataArea.Margin = new Padding(0, 0, 0, 15);
            this._dataArea.Name = "_dataArea";
            this._dataArea.RowCount = 1;
            this._dataArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._dataArea.Size = new Size(1070, 401);
            this._dataArea.TabIndex = 1;
            //
            // _profilesGroup
            //
            this._profilesGroup.Controls.Add(this._profilesListView);
            this._profilesGroup.Dock = DockStyle.Fill;
            this._profilesGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._profilesGroup.Location = new Point(3, 3);
            this._profilesGroup.Name = "_profilesGroup";
            this._profilesGroup.Padding = new Padding(10);
            this._profilesGroup.Size = new Size(644, 395);
            this._profilesGroup.TabIndex = 0;
            this._profilesGroup.TabStop = false;
            this._profilesGroup.Text = " Configuracoes Registradas ";
            //
            // _profilesListView
            //
            this._profilesListView.Columns.AddRange(new ColumnHeader[] {
                this._colId,
                this._colName,
                this._colDescription,
                this._colKind,
                this._colHost,
                this._colDatabase,
                this._colStatus});
            this._profilesListView.Dock = DockStyle.Fill;
            this._profilesListView.Font = new Font("Segoe UI", 9F);
            this._profilesListView.FullRowSelect = true;
            this._profilesListView.GridLines = true;
            this._profilesListView.HideSelection = false;
            this._profilesListView.Location = new Point(10, 25);
            this._profilesListView.MultiSelect = false;
            this._profilesListView.Name = "_profilesListView";
            this._profilesListView.Size = new Size(624, 360);
            this._profilesListView.TabIndex = 0;
            this._profilesListView.UseCompatibleStateImageBehavior = false;
            this._profilesListView.View = View.Details;
            //
            // _colId
            //
            this._colId.Text = "ID";
            this._colId.Width = 180;
            //
            // _colName
            //
            this._colName.Text = "NOME";
            this._colName.Width = 130;
            //
            // _colDescription
            //
            this._colDescription.Text = "DESCRICAO";
            this._colDescription.Width = 200;
            //
            // _colKind
            //
            this._colKind.Text = "TIPO";
            this._colKind.TextAlign = HorizontalAlignment.Center;
            this._colKind.Width = 70;
            //
            // _colHost
            //
            this._colHost.Text = "HOST";
            this._colHost.Width = 120;
            //
            // _colDatabase
            //
            this._colDatabase.Text = "DATABASE";
            this._colDatabase.Width = 120;
            //
            // _colStatus
            //
            this._colStatus.Text = "STATUS";
            this._colStatus.TextAlign = HorizontalAlignment.Center;
            this._colStatus.Width = 90;
            //
            // _formGroup
            //
            this._formGroup.Controls.Add(this._formLayout);
            this._formGroup.Dock = DockStyle.Fill;
            this._formGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._formGroup.Location = new Point(653, 3);
            this._formGroup.Name = "_formGroup";
            this._formGroup.Padding = new Padding(12);
            this._formGroup.Size = new Size(414, 395);
            this._formGroup.TabIndex = 1;
            this._formGroup.TabStop = false;
            this._formGroup.Text = " Dados do Banco ";
            //
            // _formLayout
            //
            this._formLayout.ColumnCount = 2;
            this._formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._formLayout.Controls.Add(this._nameLabel, 0, 0);
            this._formLayout.Controls.Add(this._nameTextBox, 1, 0);
            this._formLayout.Controls.Add(this._descriptionLabel, 0, 1);
            this._formLayout.Controls.Add(this._descriptionTextBox, 1, 1);
            this._formLayout.Controls.Add(this._hostLabel, 0, 2);
            this._formLayout.Controls.Add(this._hostTextBox, 1, 2);
            this._formLayout.Controls.Add(this._portLabel, 0, 3);
            this._formLayout.Controls.Add(this._portNumericUpDown, 1, 3);
            this._formLayout.Controls.Add(this._databaseLabel, 0, 4);
            this._formLayout.Controls.Add(this._databaseTextBox, 1, 4);
            this._formLayout.Controls.Add(this._userFormLabel, 0, 5);
            this._formLayout.Controls.Add(this._userTextBox, 1, 5);
            this._formLayout.Controls.Add(this._passwordFormLabel, 0, 6);
            this._formLayout.Controls.Add(this._passwordTextBox, 1, 6);
            this._formLayout.Controls.Add(this._statusLabel, 0, 7);
            this._formLayout.SetColumnSpan(this._statusLabel, 2);
            this._formLayout.Controls.Add(this._formActionsPanel, 0, 8);
            this._formLayout.SetColumnSpan(this._formActionsPanel, 2);
            this._formLayout.Dock = DockStyle.Fill;
            this._formLayout.Font = new Font("Segoe UI", 9F);
            this._formLayout.Location = new Point(12, 28);
            this._formLayout.Name = "_formLayout";
            this._formLayout.RowCount = 9;
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.RowStyles.Add(new RowStyle());
            this._formLayout.Size = new Size(390, 355);
            this._formLayout.TabIndex = 0;
            //
            // _nameLabel
            //
            this._nameLabel.AutoSize = true;
            this._nameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._nameLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._nameLabel.Location = new Point(0, 8);
            this._nameLabel.Margin = new Padding(0, 8, 0, 4);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new Size(41, 15);
            this._nameLabel.TabIndex = 0;
            this._nameLabel.Text = "Nome";
            //
            // _nameTextBox
            //
            this._nameTextBox.Dock = DockStyle.Top;
            this._nameTextBox.Font = new Font("Segoe UI", 9F);
            this._nameTextBox.Location = new Point(113, 3);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new Size(274, 23);
            this._nameTextBox.TabIndex = 1;
            //
            // _descriptionLabel
            //
            this._descriptionLabel.AutoSize = true;
            this._descriptionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._descriptionLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._descriptionLabel.Location = new Point(0, 37);
            this._descriptionLabel.Margin = new Padding(0, 8, 0, 4);
            this._descriptionLabel.Name = "_descriptionLabel";
            this._descriptionLabel.Size = new Size(63, 15);
            this._descriptionLabel.TabIndex = 2;
            this._descriptionLabel.Text = "Descricao";
            //
            // _descriptionTextBox
            //
            this._descriptionTextBox.Dock = DockStyle.Top;
            this._descriptionTextBox.Font = new Font("Segoe UI", 9F);
            this._descriptionTextBox.Location = new Point(113, 32);
            this._descriptionTextBox.Name = "_descriptionTextBox";
            this._descriptionTextBox.Size = new Size(274, 23);
            this._descriptionTextBox.TabIndex = 3;
            //
            // _hostLabel
            //
            this._hostLabel.AutoSize = true;
            this._hostLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._hostLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._hostLabel.Location = new Point(0, 66);
            this._hostLabel.Margin = new Padding(0, 8, 0, 4);
            this._hostLabel.Name = "_hostLabel";
            this._hostLabel.Size = new Size(32, 15);
            this._hostLabel.TabIndex = 4;
            this._hostLabel.Text = "Host";
            //
            // _hostTextBox
            //
            this._hostTextBox.Dock = DockStyle.Top;
            this._hostTextBox.Font = new Font("Segoe UI", 9F);
            this._hostTextBox.Location = new Point(113, 61);
            this._hostTextBox.Name = "_hostTextBox";
            this._hostTextBox.Size = new Size(274, 23);
            this._hostTextBox.TabIndex = 5;
            //
            // _portLabel
            //
            this._portLabel.AutoSize = true;
            this._portLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._portLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._portLabel.Location = new Point(0, 95);
            this._portLabel.Margin = new Padding(0, 8, 0, 4);
            this._portLabel.Name = "_portLabel";
            this._portLabel.Size = new Size(37, 15);
            this._portLabel.TabIndex = 6;
            this._portLabel.Text = "Porta";
            //
            // _portNumericUpDown
            //
            this._portNumericUpDown.Anchor = AnchorStyles.Left;
            this._portNumericUpDown.Font = new Font("Segoe UI", 9F);
            this._portNumericUpDown.Location = new Point(113, 90);
            this._portNumericUpDown.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this._portNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this._portNumericUpDown.Name = "_portNumericUpDown";
            this._portNumericUpDown.Size = new Size(130, 23);
            this._portNumericUpDown.TabIndex = 7;
            this._portNumericUpDown.Value = new decimal(new int[] { 5432, 0, 0, 0 });
            //
            // _databaseLabel
            //
            this._databaseLabel.AutoSize = true;
            this._databaseLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._databaseLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._databaseLabel.Location = new Point(0, 124);
            this._databaseLabel.Margin = new Padding(0, 8, 0, 4);
            this._databaseLabel.Name = "_databaseLabel";
            this._databaseLabel.Size = new Size(58, 15);
            this._databaseLabel.TabIndex = 8;
            this._databaseLabel.Text = "Database";
            //
            // _databaseTextBox
            //
            this._databaseTextBox.Dock = DockStyle.Top;
            this._databaseTextBox.Font = new Font("Segoe UI", 9F);
            this._databaseTextBox.Location = new Point(113, 119);
            this._databaseTextBox.Name = "_databaseTextBox";
            this._databaseTextBox.Size = new Size(274, 23);
            this._databaseTextBox.TabIndex = 9;
            //
            // _userFormLabel
            //
            this._userFormLabel.AutoSize = true;
            this._userFormLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._userFormLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._userFormLabel.Location = new Point(0, 153);
            this._userFormLabel.Margin = new Padding(0, 8, 0, 4);
            this._userFormLabel.Name = "_userFormLabel";
            this._userFormLabel.Size = new Size(49, 15);
            this._userFormLabel.TabIndex = 10;
            this._userFormLabel.Text = "Usuario";
            //
            // _userTextBox
            //
            this._userTextBox.Dock = DockStyle.Top;
            this._userTextBox.Font = new Font("Segoe UI", 9F);
            this._userTextBox.Location = new Point(113, 148);
            this._userTextBox.Name = "_userTextBox";
            this._userTextBox.Size = new Size(274, 23);
            this._userTextBox.TabIndex = 11;
            //
            // _passwordFormLabel
            //
            this._passwordFormLabel.AutoSize = true;
            this._passwordFormLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._passwordFormLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._passwordFormLabel.Location = new Point(0, 182);
            this._passwordFormLabel.Margin = new Padding(0, 8, 0, 4);
            this._passwordFormLabel.Name = "_passwordFormLabel";
            this._passwordFormLabel.Size = new Size(42, 15);
            this._passwordFormLabel.TabIndex = 12;
            this._passwordFormLabel.Text = "Senha";
            //
            // _passwordTextBox
            //
            this._passwordTextBox.Dock = DockStyle.Top;
            this._passwordTextBox.Font = new Font("Segoe UI", 9F);
            this._passwordTextBox.Location = new Point(113, 177);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new Size(274, 23);
            this._passwordTextBox.TabIndex = 13;
            this._passwordTextBox.UseSystemPasswordChar = true;
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._statusLabel.ForeColor = Color.Firebrick;
            this._statusLabel.Location = new Point(0, 218);
            this._statusLabel.Margin = new Padding(0, 14, 0, 10);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new Size(0, 15);
            this._statusLabel.TabIndex = 14;
            //
            // _formActionsPanel
            //
            this._formActionsPanel.AutoSize = true;
            this._formActionsPanel.Controls.Add(this._saveButton);
            this._formActionsPanel.Controls.Add(this._testButton);
            this._formActionsPanel.Dock = DockStyle.Fill;
            this._formActionsPanel.FlowDirection = FlowDirection.LeftToRight;
            this._formActionsPanel.Location = new Point(0, 243);
            this._formActionsPanel.Margin = new Padding(0, 0, 0, 0);
            this._formActionsPanel.Name = "_formActionsPanel";
            this._formActionsPanel.Size = new Size(390, 31);
            this._formActionsPanel.TabIndex = 15;
            //
            // _saveButton
            //
            this._saveButton.AutoSize = true;
            this._saveButton.FlatStyle = FlatStyle.System;
            this._saveButton.Location = new Point(3, 3);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new Size(75, 25);
            this._saveButton.TabIndex = 0;
            this._saveButton.Text = "Salvar";
            this._saveButton.UseVisualStyleBackColor = true;
            //
            // _testButton
            //
            this._testButton.AutoSize = true;
            this._testButton.FlatStyle = FlatStyle.System;
            this._testButton.Location = new Point(84, 3);
            this._testButton.Name = "_testButton";
            this._testButton.Size = new Size(110, 25);
            this._testButton.TabIndex = 1;
            this._testButton.Text = "Testar Conexao";
            this._testButton.UseVisualStyleBackColor = true;
            //
            // _actionBar
            //
            this._actionBar.AutoSize = true;
            this._actionBar.ColumnCount = 3;
            this._actionBar.ColumnStyles.Add(new ColumnStyle());
            this._actionBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._actionBar.ColumnStyles.Add(new ColumnStyle());
            this._actionBar.Controls.Add(this._configGroup, 0, 0);
            this._actionBar.Controls.Add(this._closeButton, 2, 0);
            this._actionBar.Dock = DockStyle.Fill;
            this._actionBar.Location = new Point(15, 490);
            this._actionBar.Margin = new Padding(0);
            this._actionBar.Name = "_actionBar";
            this._actionBar.RowCount = 1;
            this._actionBar.RowStyles.Add(new RowStyle());
            this._actionBar.Size = new Size(1070, 95);
            this._actionBar.TabIndex = 2;
            //
            // _configGroup
            //
            this._configGroup.AutoSize = true;
            this._configGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._configGroup.Controls.Add(this._configGroupLayout);
            this._configGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._configGroup.Location = new Point(3, 3);
            this._configGroup.Name = "_configGroup";
            this._configGroup.Padding = new Padding(10);
            this._configGroup.Size = new Size(620, 89);
            this._configGroup.TabIndex = 0;
            this._configGroup.TabStop = false;
            this._configGroup.Text = " Gerenciar Configuracoes ";
            //
            // _configGroupLayout
            //
            this._configGroupLayout.AutoSize = true;
            this._configGroupLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._configGroupLayout.ColumnCount = 1;
            this._configGroupLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._configGroupLayout.Controls.Add(this._configCaptionLabel, 0, 0);
            this._configGroupLayout.Controls.Add(this._configButtonsPanel, 0, 1);
            this._configGroupLayout.Dock = DockStyle.Fill;
            this._configGroupLayout.Location = new Point(10, 25);
            this._configGroupLayout.Name = "_configGroupLayout";
            this._configGroupLayout.RowCount = 2;
            this._configGroupLayout.RowStyles.Add(new RowStyle());
            this._configGroupLayout.RowStyles.Add(new RowStyle());
            this._configGroupLayout.Size = new Size(600, 54);
            this._configGroupLayout.TabIndex = 0;
            //
            // _configCaptionLabel
            //
            this._configCaptionLabel.AutoSize = true;
            this._configCaptionLabel.Font = new Font("Segoe UI", 8F);
            this._configCaptionLabel.ForeColor = Color.Blue;
            this._configCaptionLabel.Location = new Point(3, 0);
            this._configCaptionLabel.Margin = new Padding(3, 0, 3, 5);
            this._configCaptionLabel.Name = "_configCaptionLabel";
            this._configCaptionLabel.Size = new Size(236, 13);
            this._configCaptionLabel.TabIndex = 0;
            this._configCaptionLabel.Text = "Gerencia quais bancos aparecem na lista";
            //
            // _configButtonsPanel
            //
            this._configButtonsPanel.AutoSize = true;
            this._configButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._configButtonsPanel.Controls.Add(this._newButton);
            this._configButtonsPanel.Controls.Add(this._deleteButton);
            this._configButtonsPanel.Controls.Add(this._activateButton);
            this._configButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
            this._configButtonsPanel.Location = new Point(0, 18);
            this._configButtonsPanel.Margin = new Padding(0);
            this._configButtonsPanel.Name = "_configButtonsPanel";
            this._configButtonsPanel.Size = new Size(594, 31);
            this._configButtonsPanel.TabIndex = 1;
            //
            // _newButton
            //
            this._newButton.FlatStyle = FlatStyle.System;
            this._newButton.Location = new Point(3, 3);
            this._newButton.Name = "_newButton";
            this._newButton.Size = new Size(180, 25);
            this._newButton.TabIndex = 0;
            this._newButton.Text = "Adicionar Manual (F2)";
            this._newButton.UseVisualStyleBackColor = true;
            //
            // _deleteButton
            //
            this._deleteButton.FlatStyle = FlatStyle.System;
            this._deleteButton.Location = new Point(189, 3);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new Size(180, 25);
            this._deleteButton.TabIndex = 1;
            this._deleteButton.Text = "Remover da Lista (F6)";
            this._deleteButton.UseVisualStyleBackColor = true;
            //
            // _activateButton
            //
            this._activateButton.FlatStyle = FlatStyle.System;
            this._activateButton.Location = new Point(375, 3);
            this._activateButton.Name = "_activateButton";
            this._activateButton.Size = new Size(180, 25);
            this._activateButton.TabIndex = 2;
            this._activateButton.Text = "Ativar Banco (F8)";
            this._activateButton.UseVisualStyleBackColor = true;
            //
            // _closeButton
            //
            this._closeButton.Anchor = AnchorStyles.Right;
            this._closeButton.FlatStyle = FlatStyle.System;
            this._closeButton.Location = new Point(985, 35);
            this._closeButton.Margin = new Padding(10, 3, 3, 3);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new Size(82, 25);
            this._closeButton.TabIndex = 1;
            this._closeButton.Text = "Fechar (F4)";
            this._closeButton.UseVisualStyleBackColor = true;
            //
            // DatabaseProfilesForm
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ClientSize = new Size(1100, 600);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 9F);
            this.MinimumSize = new Size(1000, 580);
            this.Name = "DatabaseProfilesForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Gerenciador de Bancos";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._headerPanel.ResumeLayout(false);
            this._headerPanel.PerformLayout();
            this._dataArea.ResumeLayout(false);
            this._profilesGroup.ResumeLayout(false);
            this._formGroup.ResumeLayout(false);
            this._formLayout.ResumeLayout(false);
            this._formLayout.PerformLayout();
            ((ISupportInitialize)(this._portNumericUpDown)).EndInit();
            this._formActionsPanel.ResumeLayout(false);
            this._formActionsPanel.PerformLayout();
            this._actionBar.ResumeLayout(false);
            this._actionBar.PerformLayout();
            this._configGroup.ResumeLayout(false);
            this._configGroup.PerformLayout();
            this._configGroupLayout.ResumeLayout(false);
            this._configGroupLayout.PerformLayout();
            this._configButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private FlowLayoutPanel _headerPanel;
        private Label _headerTitleLabel;
        private Label _headerSubtitleLabel;
        private TableLayoutPanel _dataArea;
        private GroupBox _profilesGroup;
        private ListView _profilesListView;
        private ColumnHeader _colId;
        private ColumnHeader _colName;
        private ColumnHeader _colDescription;
        private ColumnHeader _colKind;
        private ColumnHeader _colHost;
        private ColumnHeader _colDatabase;
        private ColumnHeader _colStatus;
        private GroupBox _formGroup;
        private TableLayoutPanel _formLayout;
        private Label _nameLabel;
        private TextBox _nameTextBox;
        private Label _descriptionLabel;
        private TextBox _descriptionTextBox;
        private Label _hostLabel;
        private TextBox _hostTextBox;
        private Label _portLabel;
        private NumericUpDown _portNumericUpDown;
        private Label _databaseLabel;
        private TextBox _databaseTextBox;
        private Label _userFormLabel;
        private TextBox _userTextBox;
        private Label _passwordFormLabel;
        private TextBox _passwordTextBox;
        private Label _statusLabel;
        private FlowLayoutPanel _formActionsPanel;
        private Button _saveButton;
        private Button _testButton;
        private TableLayoutPanel _actionBar;
        private GroupBox _configGroup;
        private TableLayoutPanel _configGroupLayout;
        private Label _configCaptionLabel;
        private FlowLayoutPanel _configButtonsPanel;
        private Button _newButton;
        private Button _deleteButton;
        private Button _activateButton;
        private Button _closeButton;
    }
}
