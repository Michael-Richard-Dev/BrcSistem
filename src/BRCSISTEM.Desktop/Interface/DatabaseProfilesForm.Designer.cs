using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface
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
            this._profilesGroup = new GroupBox();
            this._profilesListView = new ListView();
            this._colId = new ColumnHeader();
            this._colName = new ColumnHeader();
            this._colDescription = new ColumnHeader();
            this._colKind = new ColumnHeader();
            this._colHost = new ColumnHeader();
            this._colDatabase = new ColumnHeader();
            this._colStatus = new ColumnHeader();
            this._actionBar = new TableLayoutPanel();
            this._configGroup = new GroupBox();
            this._configGroupLayout = new TableLayoutPanel();
            this._configCaptionLabel = new Label();
            this._configButtonsTable = new TableLayoutPanel();
            this._searchButton = new Button();
            this._newButton = new Button();
            this._editButton = new Button();
            this._deleteButton = new Button();
            this._activateButton = new Button();
            this._serverGroup = new GroupBox();
            this._serverGroupLayout = new TableLayoutPanel();
            this._serverCaptionLabel = new Label();
            this._serverButtonsTable = new TableLayoutPanel();
            this._createDatabaseButton = new Button();
            this._dropDatabaseButton = new Button();
            this._auxPanel = new FlowLayoutPanel();
            this._manualButton = new Button();
            this._closeButton = new Button();
            this._statusLabel = new Label();
            this._rootLayout.SuspendLayout();
            this._headerPanel.SuspendLayout();
            this._profilesGroup.SuspendLayout();
            this._actionBar.SuspendLayout();
            this._configGroup.SuspendLayout();
            this._configGroupLayout.SuspendLayout();
            this._configButtonsTable.SuspendLayout();
            this._serverGroup.SuspendLayout();
            this._serverGroupLayout.SuspendLayout();
            this._serverButtonsTable.SuspendLayout();
            this._auxPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // _rootLayout
            //
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerPanel, 0, 0);
            this._rootLayout.Controls.Add(this._profilesGroup, 0, 1);
            this._rootLayout.Controls.Add(this._actionBar, 0, 2);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 3);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Padding = new Padding(15);
            this._rootLayout.RowCount = 4;
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.Name = "_rootLayout";
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
            this._headerPanel.Margin = new Padding(0, 0, 0, 10);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.TabIndex = 0;
            this._headerPanel.WrapContents = false;
            //
            // _headerTitleLabel
            //
            this._headerTitleLabel.AutoSize = true;
            this._headerTitleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this._headerTitleLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._headerTitleLabel.Name = "_headerTitleLabel";
            this._headerTitleLabel.Text = "Gerenciador de Bancos de Dados";
            //
            // _headerSubtitleLabel
            //
            this._headerSubtitleLabel.AutoSize = true;
            this._headerSubtitleLabel.Font = new Font("Segoe UI", 9F);
            this._headerSubtitleLabel.ForeColor = Color.Gray;
            this._headerSubtitleLabel.Margin = new Padding(3, 5, 3, 0);
            this._headerSubtitleLabel.Name = "_headerSubtitleLabel";
            this._headerSubtitleLabel.Text = "Gerencia configuracoes de acesso aos bancos PostgreSQL";
            //
            // _profilesGroup
            //
            this._profilesGroup.Controls.Add(this._profilesListView);
            this._profilesGroup.Dock = DockStyle.Fill;
            this._profilesGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._profilesGroup.Margin = new Padding(0, 0, 0, 10);
            this._profilesGroup.Name = "_profilesGroup";
            this._profilesGroup.Padding = new Padding(10);
            this._profilesGroup.TabIndex = 1;
            this._profilesGroup.TabStop = false;
            this._profilesGroup.Text = " Configuracoes Registradas ";
            //
            // _profilesListView
            //
            this._profilesListView.Columns.AddRange(new ColumnHeader[] {
                this._colId, this._colName, this._colDescription, this._colKind,
                this._colHost, this._colDatabase, this._colStatus});
            this._profilesListView.Dock = DockStyle.Fill;
            this._profilesListView.Font = new Font("Segoe UI", 9F);
            this._profilesListView.FullRowSelect = true;
            this._profilesListView.GridLines = true;
            this._profilesListView.HideSelection = false;
            this._profilesListView.MultiSelect = false;
            this._profilesListView.Name = "_profilesListView";
            this._profilesListView.TabIndex = 0;
            this._profilesListView.UseCompatibleStateImageBehavior = false;
            this._profilesListView.View = View.Details;
            //
            this._colId.Text = "ID"; this._colId.Width = 200;
            this._colName.Text = "NOME"; this._colName.Width = 150;
            this._colDescription.Text = "DESCRICAO"; this._colDescription.Width = 250;
            this._colKind.Text = "TIPO"; this._colKind.Width = 80; this._colKind.TextAlign = HorizontalAlignment.Center;
            this._colHost.Text = "HOST"; this._colHost.Width = 130;
            this._colDatabase.Text = "DATABASE"; this._colDatabase.Width = 130;
            this._colStatus.Text = "STATUS"; this._colStatus.Width = 100; this._colStatus.TextAlign = HorizontalAlignment.Center;
            //
            // _actionBar
            //
            this._actionBar.AutoSize = true;
            this._actionBar.ColumnCount = 4;
            this._actionBar.ColumnStyles.Add(new ColumnStyle());
            this._actionBar.ColumnStyles.Add(new ColumnStyle());
            this._actionBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._actionBar.ColumnStyles.Add(new ColumnStyle());
            this._actionBar.Controls.Add(this._configGroup, 0, 0);
            this._actionBar.Controls.Add(this._serverGroup, 1, 0);
            this._actionBar.Controls.Add(this._auxPanel, 3, 0);
            this._actionBar.Dock = DockStyle.Fill;
            this._actionBar.Margin = new Padding(0);
            this._actionBar.Name = "_actionBar";
            this._actionBar.RowCount = 1;
            this._actionBar.RowStyles.Add(new RowStyle());
            this._actionBar.TabIndex = 2;
            //
            // _configGroup
            //
            this._configGroup.AutoSize = true;
            this._configGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._configGroup.Controls.Add(this._configGroupLayout);
            this._configGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._configGroup.Margin = new Padding(0, 0, 10, 0);
            this._configGroup.Name = "_configGroup";
            this._configGroup.Padding = new Padding(10);
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
            this._configGroupLayout.Controls.Add(this._configButtonsTable, 0, 1);
            this._configGroupLayout.Dock = DockStyle.Fill;
            this._configGroupLayout.Name = "_configGroupLayout";
            this._configGroupLayout.RowCount = 2;
            this._configGroupLayout.RowStyles.Add(new RowStyle());
            this._configGroupLayout.RowStyles.Add(new RowStyle());
            //
            this._configCaptionLabel.AutoSize = true;
            this._configCaptionLabel.Font = new Font("Segoe UI", 8F);
            this._configCaptionLabel.ForeColor = Color.Blue;
            this._configCaptionLabel.Margin = new Padding(3, 0, 3, 5);
            this._configCaptionLabel.Name = "_configCaptionLabel";
            this._configCaptionLabel.Text = "Gerencia quais bancos aparecem na lista";
            //
            // _configButtonsTable
            //
            this._configButtonsTable.AutoSize = true;
            this._configButtonsTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._configButtonsTable.ColumnCount = 2;
            this._configButtonsTable.ColumnStyles.Add(new ColumnStyle());
            this._configButtonsTable.ColumnStyles.Add(new ColumnStyle());
            this._configButtonsTable.Controls.Add(this._searchButton, 0, 0);
            this._configButtonsTable.Controls.Add(this._newButton, 1, 0);
            this._configButtonsTable.Controls.Add(this._editButton, 0, 1);
            this._configButtonsTable.Controls.Add(this._deleteButton, 1, 1);
            this._configButtonsTable.Controls.Add(this._activateButton, 0, 2);
            this._configButtonsTable.Margin = new Padding(0);
            this._configButtonsTable.Name = "_configButtonsTable";
            this._configButtonsTable.RowCount = 3;
            this._configButtonsTable.RowStyles.Add(new RowStyle());
            this._configButtonsTable.RowStyles.Add(new RowStyle());
            this._configButtonsTable.RowStyles.Add(new RowStyle());
            //
            ConfigureConfigButton(this._searchButton, "Buscar e Adicionar (F7)", 0);
            ConfigureConfigButton(this._newButton, "Adicionar Manual (F2)", 1);
            ConfigureConfigButton(this._editButton, "Editar Selecionado (F3)", 2);
            ConfigureConfigButton(this._deleteButton, "Remover da Lista (F6)", 3);
            ConfigureConfigButton(this._activateButton, "Ativar Banco (F8)", 4);
            //
            // _serverGroup
            //
            this._serverGroup.AutoSize = true;
            this._serverGroup.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._serverGroup.Controls.Add(this._serverGroupLayout);
            this._serverGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._serverGroup.Margin = new Padding(0, 0, 10, 0);
            this._serverGroup.Name = "_serverGroup";
            this._serverGroup.Padding = new Padding(10);
            this._serverGroup.TabIndex = 1;
            this._serverGroup.TabStop = false;
            this._serverGroup.Text = " Gerenciar Servidor PostgreSQL ";
            //
            this._serverGroupLayout.AutoSize = true;
            this._serverGroupLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._serverGroupLayout.ColumnCount = 1;
            this._serverGroupLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._serverGroupLayout.Controls.Add(this._serverCaptionLabel, 0, 0);
            this._serverGroupLayout.Controls.Add(this._serverButtonsTable, 0, 1);
            this._serverGroupLayout.Dock = DockStyle.Fill;
            this._serverGroupLayout.Name = "_serverGroupLayout";
            this._serverGroupLayout.RowCount = 2;
            this._serverGroupLayout.RowStyles.Add(new RowStyle());
            this._serverGroupLayout.RowStyles.Add(new RowStyle());
            //
            this._serverCaptionLabel.AutoSize = true;
            this._serverCaptionLabel.Font = new Font("Segoe UI", 8F);
            this._serverCaptionLabel.ForeColor = Color.Red;
            this._serverCaptionLabel.Margin = new Padding(3, 0, 3, 5);
            this._serverCaptionLabel.Name = "_serverCaptionLabel";
            this._serverCaptionLabel.Text = "Cria/exclui bancos no servidor PostgreSQL";
            //
            this._serverButtonsTable.AutoSize = true;
            this._serverButtonsTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._serverButtonsTable.ColumnCount = 2;
            this._serverButtonsTable.ColumnStyles.Add(new ColumnStyle());
            this._serverButtonsTable.ColumnStyles.Add(new ColumnStyle());
            this._serverButtonsTable.Controls.Add(this._createDatabaseButton, 0, 0);
            this._serverButtonsTable.Controls.Add(this._dropDatabaseButton, 1, 0);
            this._serverButtonsTable.Margin = new Padding(0);
            this._serverButtonsTable.Name = "_serverButtonsTable";
            this._serverButtonsTable.RowCount = 1;
            this._serverButtonsTable.RowStyles.Add(new RowStyle());
            //
            ConfigureServerButton(this._createDatabaseButton, "Criar Novo Banco", 0);
            ConfigureServerButton(this._dropDatabaseButton, "Excluir Banco", 1);
            //
            // _auxPanel
            //
            this._auxPanel.AutoSize = true;
            this._auxPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._auxPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this._auxPanel.Controls.Add(this._manualButton);
            this._auxPanel.Controls.Add(this._closeButton);
            this._auxPanel.FlowDirection = FlowDirection.TopDown;
            this._auxPanel.Margin = new Padding(10, 0, 0, 0);
            this._auxPanel.Name = "_auxPanel";
            this._auxPanel.TabIndex = 2;
            this._auxPanel.WrapContents = false;
            //
            this._manualButton.FlatStyle = FlatStyle.System;
            this._manualButton.Margin = new Padding(3, 3, 3, 2);
            this._manualButton.Name = "_manualButton";
            this._manualButton.Size = new Size(110, 27);
            this._manualButton.TabIndex = 0;
            this._manualButton.Text = "Manual";
            this._manualButton.UseVisualStyleBackColor = true;
            //
            this._closeButton.FlatStyle = FlatStyle.System;
            this._closeButton.Margin = new Padding(3, 2, 3, 3);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new Size(110, 27);
            this._closeButton.TabIndex = 1;
            this._closeButton.Text = "Fechar (F4)";
            this._closeButton.UseVisualStyleBackColor = true;
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._statusLabel.ForeColor = Color.SeaGreen;
            this._statusLabel.Margin = new Padding(0, 10, 0, 0);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Text = string.Empty;
            //
            // DatabaseProfilesForm
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ClientSize = new Size(1100, 600);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 9F);
            this.MinimumSize = new Size(1020, 600);
            this.Name = "DatabaseProfilesForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Gerenciador de Bancos";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._headerPanel.ResumeLayout(false);
            this._headerPanel.PerformLayout();
            this._profilesGroup.ResumeLayout(false);
            this._actionBar.ResumeLayout(false);
            this._actionBar.PerformLayout();
            this._configGroup.ResumeLayout(false);
            this._configGroup.PerformLayout();
            this._configGroupLayout.ResumeLayout(false);
            this._configGroupLayout.PerformLayout();
            this._configButtonsTable.ResumeLayout(false);
            this._serverGroup.ResumeLayout(false);
            this._serverGroup.PerformLayout();
            this._serverGroupLayout.ResumeLayout(false);
            this._serverGroupLayout.PerformLayout();
            this._serverButtonsTable.ResumeLayout(false);
            this._auxPanel.ResumeLayout(false);
            this._auxPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private static void ConfigureConfigButton(Button button, string text, int tabIndex)
        {
            button.Dock = DockStyle.Fill;
            button.FlatStyle = FlatStyle.System;
            button.Margin = new Padding(2);
            button.Size = new Size(180, 25);
            button.TabIndex = tabIndex;
            button.Text = text;
            button.UseVisualStyleBackColor = true;
        }

        private static void ConfigureServerButton(Button button, string text, int tabIndex)
        {
            button.Dock = DockStyle.Fill;
            button.FlatStyle = FlatStyle.System;
            button.Margin = new Padding(2);
            button.Size = new Size(160, 25);
            button.TabIndex = tabIndex;
            button.Text = text;
            button.UseVisualStyleBackColor = true;
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private FlowLayoutPanel _headerPanel;
        private Label _headerTitleLabel;
        private Label _headerSubtitleLabel;
        private GroupBox _profilesGroup;
        private ListView _profilesListView;
        private ColumnHeader _colId;
        private ColumnHeader _colName;
        private ColumnHeader _colDescription;
        private ColumnHeader _colKind;
        private ColumnHeader _colHost;
        private ColumnHeader _colDatabase;
        private ColumnHeader _colStatus;
        private TableLayoutPanel _actionBar;
        private GroupBox _configGroup;
        private TableLayoutPanel _configGroupLayout;
        private Label _configCaptionLabel;
        private TableLayoutPanel _configButtonsTable;
        private Button _searchButton;
        private Button _newButton;
        private Button _editButton;
        private Button _deleteButton;
        private Button _activateButton;
        private GroupBox _serverGroup;
        private TableLayoutPanel _serverGroupLayout;
        private Label _serverCaptionLabel;
        private TableLayoutPanel _serverButtonsTable;
        private Button _createDatabaseButton;
        private Button _dropDatabaseButton;
        private FlowLayoutPanel _auxPanel;
        private Button _manualButton;
        private Button _closeButton;
        private Label _statusLabel;
    }
}
