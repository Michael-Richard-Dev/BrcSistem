using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.PerfisBancoDados
{
    public sealed partial class PerfisBancoDadosForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerfisBancoDadosForm));
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._headerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._headerSubtitleLabel = new System.Windows.Forms.Label();
            this._profilesGroup = new System.Windows.Forms.GroupBox();
            this._profilesListView = new System.Windows.Forms.ListView();
            this._colId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colKind = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colHost = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colDatabase = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._colStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._actionBar = new System.Windows.Forms.TableLayoutPanel();
            this._configGroup = new System.Windows.Forms.GroupBox();
            this._configGroupLayout = new System.Windows.Forms.TableLayoutPanel();
            this._configCaptionLabel = new System.Windows.Forms.Label();
            this._configButtonsTable = new System.Windows.Forms.TableLayoutPanel();
            this._searchButton = new System.Windows.Forms.Button();
            this._newButton = new System.Windows.Forms.Button();
            this._editButton = new System.Windows.Forms.Button();
            this._deleteButton = new System.Windows.Forms.Button();
            this._activateButton = new System.Windows.Forms.Button();
            this._serverGroup = new System.Windows.Forms.GroupBox();
            this._serverGroupLayout = new System.Windows.Forms.TableLayoutPanel();
            this._serverCaptionLabel = new System.Windows.Forms.Label();
            this._serverButtonsTable = new System.Windows.Forms.TableLayoutPanel();
            this._createDatabaseButton = new System.Windows.Forms.Button();
            this._dropDatabaseButton = new System.Windows.Forms.Button();
            this._auxPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._manualButton = new System.Windows.Forms.Button();
            this._statusLabel = new System.Windows.Forms.Label();
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
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerPanel, 0, 0);
            this._rootLayout.Controls.Add(this._profilesGroup, 0, 1);
            this._rootLayout.Controls.Add(this._actionBar, 0, 2);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 3);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(15);
            this._rootLayout.RowCount = 4;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.Size = new System.Drawing.Size(1100, 600);
            this._rootLayout.TabIndex = 0;
            // 
            // _headerPanel
            // 
            this._headerPanel.AutoSize = true;
            this._headerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._headerPanel.Controls.Add(this._headerSubtitleLabel);
            this._headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._headerPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._headerPanel.Location = new System.Drawing.Point(15, 15);
            this._headerPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Size = new System.Drawing.Size(1070, 18);
            this._headerPanel.TabIndex = 0;
            this._headerPanel.WrapContents = false;
            // 
            // _headerSubtitleLabel
            // 
            this._headerSubtitleLabel.AutoSize = true;
            this._headerSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._headerSubtitleLabel.ForeColor = System.Drawing.Color.Gray;
            this._headerSubtitleLabel.Location = new System.Drawing.Point(3, 5);
            this._headerSubtitleLabel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this._headerSubtitleLabel.Name = "_headerSubtitleLabel";
            this._headerSubtitleLabel.Size = new System.Drawing.Size(303, 13);
            this._headerSubtitleLabel.TabIndex = 1;
            this._headerSubtitleLabel.Text = "Gerencia configuracoes de acesso aos bancos PostgreSQL";
            // 
            // _profilesGroup
            // 
            this._profilesGroup.Controls.Add(this._profilesListView);
            this._profilesGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._profilesGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._profilesGroup.Location = new System.Drawing.Point(15, 43);
            this._profilesGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._profilesGroup.Name = "_profilesGroup";
            this._profilesGroup.Padding = new System.Windows.Forms.Padding(10);
            this._profilesGroup.Size = new System.Drawing.Size(1070, 368);
            this._profilesGroup.TabIndex = 1;
            this._profilesGroup.TabStop = false;
            this._profilesGroup.Text = " Configuracoes Registradas ";
            // 
            // _profilesListView
            // 
            this._profilesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._colId,
            this._colName,
            this._colDescription,
            this._colKind,
            this._colHost,
            this._colDatabase,
            this._colStatus});
            this._profilesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._profilesListView.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._profilesListView.FullRowSelect = true;
            this._profilesListView.GridLines = true;
            this._profilesListView.HideSelection = false;
            this._profilesListView.Location = new System.Drawing.Point(10, 26);
            this._profilesListView.MultiSelect = false;
            this._profilesListView.Name = "_profilesListView";
            this._profilesListView.Size = new System.Drawing.Size(1050, 332);
            this._profilesListView.TabIndex = 0;
            this._profilesListView.UseCompatibleStateImageBehavior = false;
            this._profilesListView.View = System.Windows.Forms.View.Details;
            // 
            // _colId
            // 
            this._colId.Text = "ID";
            this._colId.Width = 200;
            // 
            // _colName
            // 
            this._colName.Text = "NOME";
            this._colName.Width = 150;
            // 
            // _colDescription
            // 
            this._colDescription.Text = "DESCRICAO";
            this._colDescription.Width = 250;
            // 
            // _colKind
            // 
            this._colKind.Text = "TIPO";
            this._colKind.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._colKind.Width = 80;
            // 
            // _colHost
            // 
            this._colHost.Text = "HOST";
            this._colHost.Width = 130;
            // 
            // _colDatabase
            // 
            this._colDatabase.Text = "DATABASE";
            this._colDatabase.Width = 130;
            // 
            // _colStatus
            // 
            this._colStatus.Text = "STATUS";
            this._colStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._colStatus.Width = 100;
            // 
            // _actionBar
            // 
            this._actionBar.AutoSize = true;
            this._actionBar.ColumnCount = 4;
            this._actionBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._actionBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._actionBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._actionBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._actionBar.Controls.Add(this._configGroup, 0, 0);
            this._actionBar.Controls.Add(this._serverGroup, 1, 0);
            this._actionBar.Controls.Add(this._auxPanel, 3, 0);
            this._actionBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this._actionBar.Location = new System.Drawing.Point(15, 421);
            this._actionBar.Margin = new System.Windows.Forms.Padding(0);
            this._actionBar.Name = "_actionBar";
            this._actionBar.RowCount = 1;
            this._actionBar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._actionBar.Size = new System.Drawing.Size(1070, 141);
            this._actionBar.TabIndex = 2;
            // 
            // _configGroup
            // 
            this._configGroup.AutoSize = true;
            this._configGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._configGroup.Controls.Add(this._configGroupLayout);
            this._configGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._configGroup.Location = new System.Drawing.Point(0, 0);
            this._configGroup.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this._configGroup.Name = "_configGroup";
            this._configGroup.Padding = new System.Windows.Forms.Padding(10);
            this._configGroup.Size = new System.Drawing.Size(388, 141);
            this._configGroup.TabIndex = 0;
            this._configGroup.TabStop = false;
            this._configGroup.Text = " Gerenciar Configuracoes ";
            // 
            // _configGroupLayout
            // 
            this._configGroupLayout.AutoSize = true;
            this._configGroupLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._configGroupLayout.ColumnCount = 1;
            this._configGroupLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._configGroupLayout.Controls.Add(this._configCaptionLabel, 0, 0);
            this._configGroupLayout.Controls.Add(this._configButtonsTable, 0, 1);
            this._configGroupLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._configGroupLayout.Location = new System.Drawing.Point(10, 26);
            this._configGroupLayout.Name = "_configGroupLayout";
            this._configGroupLayout.RowCount = 2;
            this._configGroupLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._configGroupLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._configGroupLayout.Size = new System.Drawing.Size(368, 105);
            this._configGroupLayout.TabIndex = 0;
            // 
            // _configCaptionLabel
            // 
            this._configCaptionLabel.AutoSize = true;
            this._configCaptionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._configCaptionLabel.ForeColor = System.Drawing.Color.Blue;
            this._configCaptionLabel.Location = new System.Drawing.Point(3, 0);
            this._configCaptionLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this._configCaptionLabel.Name = "_configCaptionLabel";
            this._configCaptionLabel.Size = new System.Drawing.Size(215, 13);
            this._configCaptionLabel.TabIndex = 0;
            this._configCaptionLabel.Text = "Gerencia quais bancos aparecem na lista";
            // 
            // _configButtonsTable
            // 
            this._configButtonsTable.AutoSize = true;
            this._configButtonsTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._configButtonsTable.ColumnCount = 2;
            this._configButtonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._configButtonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._configButtonsTable.Controls.Add(this._searchButton, 0, 0);
            this._configButtonsTable.Controls.Add(this._newButton, 1, 0);
            this._configButtonsTable.Controls.Add(this._editButton, 0, 1);
            this._configButtonsTable.Controls.Add(this._deleteButton, 1, 1);
            this._configButtonsTable.Controls.Add(this._activateButton, 0, 2);
            this._configButtonsTable.Location = new System.Drawing.Point(0, 18);
            this._configButtonsTable.Margin = new System.Windows.Forms.Padding(0);
            this._configButtonsTable.Name = "_configButtonsTable";
            this._configButtonsTable.RowCount = 3;
            this._configButtonsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._configButtonsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._configButtonsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._configButtonsTable.Size = new System.Drawing.Size(368, 87);
            this._configButtonsTable.TabIndex = 1;
            // 
            // _searchButton
            // 
            this._searchButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._searchButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._searchButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._searchButton.Location = new System.Drawing.Point(2, 2);
            this._searchButton.Margin = new System.Windows.Forms.Padding(2);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(180, 25);
            this._searchButton.TabIndex = 0;
            this._searchButton.Text = "Buscar e Adicionar (F7)";
            this._searchButton.UseVisualStyleBackColor = true;
            // 
            // _newButton
            // 
            this._newButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._newButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._newButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._newButton.Location = new System.Drawing.Point(186, 2);
            this._newButton.Margin = new System.Windows.Forms.Padding(2);
            this._newButton.Name = "_newButton";
            this._newButton.Size = new System.Drawing.Size(180, 25);
            this._newButton.TabIndex = 1;
            this._newButton.Text = "Adicionar Manual (F2)";
            this._newButton.UseVisualStyleBackColor = true;
            // 
            // _editButton
            // 
            this._editButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._editButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._editButton.Location = new System.Drawing.Point(2, 31);
            this._editButton.Margin = new System.Windows.Forms.Padding(2);
            this._editButton.Name = "_editButton";
            this._editButton.Size = new System.Drawing.Size(180, 25);
            this._editButton.TabIndex = 2;
            this._editButton.Text = "Editar Selecionado (F3)";
            this._editButton.UseVisualStyleBackColor = true;
            // 
            // _deleteButton
            // 
            this._deleteButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._deleteButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._deleteButton.Location = new System.Drawing.Point(186, 31);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(2);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(180, 25);
            this._deleteButton.TabIndex = 3;
            this._deleteButton.Text = "Remover da Lista (F6)";
            this._deleteButton.UseVisualStyleBackColor = true;
            // 
            // _activateButton
            // 
            this._activateButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._activateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._activateButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._activateButton.Location = new System.Drawing.Point(2, 60);
            this._activateButton.Margin = new System.Windows.Forms.Padding(2);
            this._activateButton.Name = "_activateButton";
            this._activateButton.Size = new System.Drawing.Size(180, 25);
            this._activateButton.TabIndex = 4;
            this._activateButton.Text = "Ativar Banco (F8)";
            this._activateButton.UseVisualStyleBackColor = true;
            // 
            // _serverGroup
            // 
            this._serverGroup.AutoSize = true;
            this._serverGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._serverGroup.Controls.Add(this._serverGroupLayout);
            this._serverGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._serverGroup.Location = new System.Drawing.Point(398, 0);
            this._serverGroup.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this._serverGroup.Name = "_serverGroup";
            this._serverGroup.Padding = new System.Windows.Forms.Padding(10);
            this._serverGroup.Size = new System.Drawing.Size(348, 83);
            this._serverGroup.TabIndex = 1;
            this._serverGroup.TabStop = false;
            this._serverGroup.Text = " Gerenciar Servidor PostgreSQL ";
            // 
            // _serverGroupLayout
            // 
            this._serverGroupLayout.AutoSize = true;
            this._serverGroupLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._serverGroupLayout.ColumnCount = 1;
            this._serverGroupLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._serverGroupLayout.Controls.Add(this._serverCaptionLabel, 0, 0);
            this._serverGroupLayout.Controls.Add(this._serverButtonsTable, 0, 1);
            this._serverGroupLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._serverGroupLayout.Location = new System.Drawing.Point(10, 26);
            this._serverGroupLayout.Name = "_serverGroupLayout";
            this._serverGroupLayout.RowCount = 2;
            this._serverGroupLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverGroupLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverGroupLayout.Size = new System.Drawing.Size(328, 47);
            this._serverGroupLayout.TabIndex = 0;
            // 
            // _serverCaptionLabel
            // 
            this._serverCaptionLabel.AutoSize = true;
            this._serverCaptionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._serverCaptionLabel.ForeColor = System.Drawing.Color.Red;
            this._serverCaptionLabel.Location = new System.Drawing.Point(3, 0);
            this._serverCaptionLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this._serverCaptionLabel.Name = "_serverCaptionLabel";
            this._serverCaptionLabel.Size = new System.Drawing.Size(222, 13);
            this._serverCaptionLabel.TabIndex = 0;
            this._serverCaptionLabel.Text = "Cria/exclui bancos no servidor PostgreSQL";
            // 
            // _serverButtonsTable
            // 
            this._serverButtonsTable.AutoSize = true;
            this._serverButtonsTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._serverButtonsTable.ColumnCount = 2;
            this._serverButtonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._serverButtonsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._serverButtonsTable.Controls.Add(this._createDatabaseButton, 0, 0);
            this._serverButtonsTable.Controls.Add(this._dropDatabaseButton, 1, 0);
            this._serverButtonsTable.Location = new System.Drawing.Point(0, 18);
            this._serverButtonsTable.Margin = new System.Windows.Forms.Padding(0);
            this._serverButtonsTable.Name = "_serverButtonsTable";
            this._serverButtonsTable.RowCount = 1;
            this._serverButtonsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverButtonsTable.Size = new System.Drawing.Size(328, 29);
            this._serverButtonsTable.TabIndex = 1;
            // 
            // _createDatabaseButton
            // 
            this._createDatabaseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._createDatabaseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._createDatabaseButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._createDatabaseButton.Location = new System.Drawing.Point(2, 2);
            this._createDatabaseButton.Margin = new System.Windows.Forms.Padding(2);
            this._createDatabaseButton.Name = "_createDatabaseButton";
            this._createDatabaseButton.Size = new System.Drawing.Size(160, 25);
            this._createDatabaseButton.TabIndex = 0;
            this._createDatabaseButton.Text = "Criar Novo Banco";
            this._createDatabaseButton.UseVisualStyleBackColor = true;
            // 
            // _dropDatabaseButton
            // 
            this._dropDatabaseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dropDatabaseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._dropDatabaseButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._dropDatabaseButton.Location = new System.Drawing.Point(166, 2);
            this._dropDatabaseButton.Margin = new System.Windows.Forms.Padding(2);
            this._dropDatabaseButton.Name = "_dropDatabaseButton";
            this._dropDatabaseButton.Size = new System.Drawing.Size(160, 25);
            this._dropDatabaseButton.TabIndex = 1;
            this._dropDatabaseButton.Text = "Excluir Banco";
            this._dropDatabaseButton.UseVisualStyleBackColor = true;
            // 
            // _auxPanel
            // 
            this._auxPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._auxPanel.AutoSize = true;
            this._auxPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._auxPanel.Controls.Add(this._manualButton);
            this._auxPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._auxPanel.Location = new System.Drawing.Point(954, 0);
            this._auxPanel.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this._auxPanel.Name = "_auxPanel";
            this._auxPanel.Size = new System.Drawing.Size(116, 32);
            this._auxPanel.TabIndex = 2;
            this._auxPanel.WrapContents = false;
            // 
            // _manualButton
            // 
            this._manualButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._manualButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._manualButton.Location = new System.Drawing.Point(3, 3);
            this._manualButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this._manualButton.Name = "_manualButton";
            this._manualButton.Size = new System.Drawing.Size(110, 27);
            this._manualButton.TabIndex = 0;
            this._manualButton.Text = "Manual";
            this._manualButton.UseVisualStyleBackColor = true;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusLabel.Location = new System.Drawing.Point(15, 572);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(0, 13);
            this._statusLabel.TabIndex = 3;
            // 
            // PerfisBancoDadosForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(1100, 600);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1020, 600);
            this.Name = "PerfisBancoDadosForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private FlowLayoutPanel _headerPanel;
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
        private Label _statusLabel;
    }
}
