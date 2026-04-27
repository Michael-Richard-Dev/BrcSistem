namespace BRCSISTEM.Desktop.Interface.ConsultaLogsAuditoria
{
    partial class ConsultaLogsAuditoriaForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel _rootLayout;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.GroupBox _filtersGroupBox;
        private System.Windows.Forms.TableLayoutPanel _filtersLayout;
        private System.Windows.Forms.FlowLayoutPanel _filtersLine1Panel;
        private System.Windows.Forms.Label _periodLabel;
        private System.Windows.Forms.ComboBox _periodComboBox;
        private System.Windows.Forms.Label _userLabel;
        private System.Windows.Forms.ComboBox _userComboBox;
        private System.Windows.Forms.Label _actionLabel;
        private System.Windows.Forms.ComboBox _actionComboBox;
        private System.Windows.Forms.FlowLayoutPanel _filtersLine2Panel;
        private System.Windows.Forms.Label _searchLabel;
        private System.Windows.Forms.TextBox _searchTextBox;
        private System.Windows.Forms.Button _searchButton;
        private System.Windows.Forms.Button _clearButton;
        private System.Windows.Forms.Button _closeButton;
        private System.Windows.Forms.GroupBox _resultsGroupBox;
        private System.Windows.Forms.DataGridView _logsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn _idColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _dateTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _userNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _actionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _detailsSummaryColumn;
        private System.Windows.Forms.TableLayoutPanel _pagingLayout;
        private System.Windows.Forms.FlowLayoutPanel _pagingInfoPanel;
        private System.Windows.Forms.Label _infoLabel;
        private System.Windows.Forms.Label _pageLabel;
        private System.Windows.Forms.FlowLayoutPanel _pagingButtonsPanel;
        private System.Windows.Forms.Button _previousButton;
        private System.Windows.Forms.Button _nextButton;
        private System.Windows.Forms.TableLayoutPanel _footerLayout;
        private System.Windows.Forms.Label _statusLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._filtersGroupBox = new System.Windows.Forms.GroupBox();
            this._filtersLayout = new System.Windows.Forms.TableLayoutPanel();
            this._filtersLine1Panel = new System.Windows.Forms.FlowLayoutPanel();
            this._periodLabel = new System.Windows.Forms.Label();
            this._periodComboBox = new System.Windows.Forms.ComboBox();
            this._userLabel = new System.Windows.Forms.Label();
            this._userComboBox = new System.Windows.Forms.ComboBox();
            this._actionLabel = new System.Windows.Forms.Label();
            this._actionComboBox = new System.Windows.Forms.ComboBox();
            this._filtersLine2Panel = new System.Windows.Forms.FlowLayoutPanel();
            this._searchLabel = new System.Windows.Forms.Label();
            this._searchTextBox = new System.Windows.Forms.TextBox();
            this._searchButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this._resultsGroupBox = new System.Windows.Forms.GroupBox();
            this._logsGrid = new System.Windows.Forms.DataGridView();
            this._idColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._dateTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._userNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._actionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._detailsSummaryColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._pagingLayout = new System.Windows.Forms.TableLayoutPanel();
            this._pagingInfoPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._infoLabel = new System.Windows.Forms.Label();
            this._pageLabel = new System.Windows.Forms.Label();
            this._pagingButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._previousButton = new System.Windows.Forms.Button();
            this._nextButton = new System.Windows.Forms.Button();
            this._footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this._statusLabel = new System.Windows.Forms.Label();
            this._rootLayout.SuspendLayout();
            this._filtersGroupBox.SuspendLayout();
            this._filtersLayout.SuspendLayout();
            this._filtersLine1Panel.SuspendLayout();
            this._filtersLine2Panel.SuspendLayout();
            this._resultsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._logsGrid)).BeginInit();
            this._pagingLayout.SuspendLayout();
            this._pagingInfoPanel.SuspendLayout();
            this._pagingButtonsPanel.SuspendLayout();
            this._footerLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._titleLabel, 0, 0);
            this._rootLayout.Controls.Add(this._filtersGroupBox, 0, 1);
            this._rootLayout.Controls.Add(this._resultsGroupBox, 0, 2);
            this._rootLayout.Controls.Add(this._pagingLayout, 0, 3);
            this._rootLayout.Controls.Add(this._footerLayout, 0, 4);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(12);
            this._rootLayout.RowCount = 5;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this._rootLayout.Size = new System.Drawing.Size(1184, 661);
            this._rootLayout.TabIndex = 0;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Location = new System.Drawing.Point(12, 12);
            this._titleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(227, 21);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Consulta de Logs e Auditoria";
            // 
            // _filtersGroupBox
            // 
            this._filtersGroupBox.Controls.Add(this._filtersLayout);
            this._filtersGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filtersGroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._filtersGroupBox.Location = new System.Drawing.Point(15, 46);
            this._filtersGroupBox.Name = "_filtersGroupBox";
            this._filtersGroupBox.Padding = new System.Windows.Forms.Padding(10);
            this._filtersGroupBox.Size = new System.Drawing.Size(1154, 106);
            this._filtersGroupBox.TabIndex = 1;
            this._filtersGroupBox.TabStop = false;
            this._filtersGroupBox.Text = "Filtros de Pesquisa";
            // 
            // _filtersLayout
            // 
            this._filtersLayout.ColumnCount = 1;
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._filtersLayout.Controls.Add(this._filtersLine1Panel, 0, 0);
            this._filtersLayout.Controls.Add(this._filtersLine2Panel, 0, 1);
            this._filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filtersLayout.Location = new System.Drawing.Point(10, 26);
            this._filtersLayout.Name = "_filtersLayout";
            this._filtersLayout.RowCount = 2;
            this._filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._filtersLayout.Size = new System.Drawing.Size(1134, 70);
            this._filtersLayout.TabIndex = 0;
            // 
            // _filtersLine1Panel
            // 
            this._filtersLine1Panel.Controls.Add(this._periodLabel);
            this._filtersLine1Panel.Controls.Add(this._periodComboBox);
            this._filtersLine1Panel.Controls.Add(this._userLabel);
            this._filtersLine1Panel.Controls.Add(this._userComboBox);
            this._filtersLine1Panel.Controls.Add(this._actionLabel);
            this._filtersLine1Panel.Controls.Add(this._actionComboBox);
            this._filtersLine1Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filtersLine1Panel.Location = new System.Drawing.Point(3, 3);
            this._filtersLine1Panel.Name = "_filtersLine1Panel";
            this._filtersLine1Panel.Size = new System.Drawing.Size(1128, 29);
            this._filtersLine1Panel.TabIndex = 0;
            this._filtersLine1Panel.WrapContents = false;
            // 
            // _periodLabel
            // 
            this._periodLabel.AutoSize = true;
            this._periodLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._periodLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._periodLabel.Location = new System.Drawing.Point(0, 7);
            this._periodLabel.Margin = new System.Windows.Forms.Padding(0, 7, 4, 0);
            this._periodLabel.Name = "_periodLabel";
            this._periodLabel.Size = new System.Drawing.Size(49, 13);
            this._periodLabel.TabIndex = 0;
            this._periodLabel.Text = "Periodo:";
            // 
            // _periodComboBox
            // 
            this._periodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._periodComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._periodComboBox.FormattingEnabled = true;
            this._periodComboBox.Items.AddRange(new object[] {
            "Hoje",
            "Ultimos 7 dias",
            "Ultimos 30 dias",
            "Ultimos 90 dias",
            "Todos"});
            this._periodComboBox.Location = new System.Drawing.Point(56, 3);
            this._periodComboBox.Margin = new System.Windows.Forms.Padding(3, 3, 14, 3);
            this._periodComboBox.Name = "_periodComboBox";
            this._periodComboBox.Size = new System.Drawing.Size(160, 21);
            this._periodComboBox.TabIndex = 1;
            this._periodComboBox.SelectedIndexChanged += new System.EventHandler(this.OnFilterChanged);
            // 
            // _userLabel
            // 
            this._userLabel.AutoSize = true;
            this._userLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._userLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._userLabel.Location = new System.Drawing.Point(230, 7);
            this._userLabel.Margin = new System.Windows.Forms.Padding(0, 7, 4, 0);
            this._userLabel.Name = "_userLabel";
            this._userLabel.Size = new System.Drawing.Size(48, 13);
            this._userLabel.TabIndex = 2;
            this._userLabel.Text = "Usuario:";
            // 
            // _userComboBox
            // 
            this._userComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._userComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._userComboBox.FormattingEnabled = true;
            this._userComboBox.Location = new System.Drawing.Point(285, 3);
            this._userComboBox.Margin = new System.Windows.Forms.Padding(3, 3, 14, 3);
            this._userComboBox.Name = "_userComboBox";
            this._userComboBox.Size = new System.Drawing.Size(180, 21);
            this._userComboBox.TabIndex = 3;
            this._userComboBox.SelectedIndexChanged += new System.EventHandler(this.OnFilterChanged);
            // 
            // _actionLabel
            // 
            this._actionLabel.AutoSize = true;
            this._actionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._actionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._actionLabel.Location = new System.Drawing.Point(479, 7);
            this._actionLabel.Margin = new System.Windows.Forms.Padding(0, 7, 4, 0);
            this._actionLabel.Name = "_actionLabel";
            this._actionLabel.Size = new System.Drawing.Size(34, 13);
            this._actionLabel.TabIndex = 4;
            this._actionLabel.Text = "Acao:";
            // 
            // _actionComboBox
            // 
            this._actionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._actionComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._actionComboBox.FormattingEnabled = true;
            this._actionComboBox.Location = new System.Drawing.Point(520, 3);
            this._actionComboBox.Margin = new System.Windows.Forms.Padding(3, 3, 14, 3);
            this._actionComboBox.Name = "_actionComboBox";
            this._actionComboBox.Size = new System.Drawing.Size(220, 21);
            this._actionComboBox.TabIndex = 5;
            this._actionComboBox.SelectedIndexChanged += new System.EventHandler(this.OnFilterChanged);
            // 
            // _filtersLine2Panel
            // 
            this._filtersLine2Panel.Controls.Add(this._searchLabel);
            this._filtersLine2Panel.Controls.Add(this._searchTextBox);
            this._filtersLine2Panel.Controls.Add(this._searchButton);
            this._filtersLine2Panel.Controls.Add(this._clearButton);
            this._filtersLine2Panel.Controls.Add(this._closeButton);
            this._filtersLine2Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filtersLine2Panel.Location = new System.Drawing.Point(3, 38);
            this._filtersLine2Panel.Name = "_filtersLine2Panel";
            this._filtersLine2Panel.Size = new System.Drawing.Size(1128, 29);
            this._filtersLine2Panel.TabIndex = 1;
            this._filtersLine2Panel.WrapContents = false;
            // 
            // _searchLabel
            // 
            this._searchLabel.AutoSize = true;
            this._searchLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._searchLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._searchLabel.Location = new System.Drawing.Point(0, 7);
            this._searchLabel.Margin = new System.Windows.Forms.Padding(0, 7, 4, 0);
            this._searchLabel.Name = "_searchLabel";
            this._searchLabel.Size = new System.Drawing.Size(60, 13);
            this._searchLabel.TabIndex = 0;
            this._searchLabel.Text = "Pesquisar:";
            // 
            // _searchTextBox
            // 
            this._searchTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._searchTextBox.Location = new System.Drawing.Point(67, 3);
            this._searchTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this._searchTextBox.Name = "_searchTextBox";
            this._searchTextBox.Size = new System.Drawing.Size(480, 22);
            this._searchTextBox.TabIndex = 1;
            this._searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnSearchTextBoxKeyDown);
            // 
            // _searchButton
            // 
            this._searchButton.AutoSize = true;
            this._searchButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._searchButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._searchButton.Location = new System.Drawing.Point(560, 3);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(75, 22);
            this._searchButton.TabIndex = 2;
            this._searchButton.Text = "Pesquisar";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this.OnSearchButtonClick);
            // 
            // _clearButton
            // 
            this._clearButton.AutoSize = true;
            this._clearButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._clearButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._clearButton.Location = new System.Drawing.Point(641, 3);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(75, 22);
            this._clearButton.TabIndex = 3;
            this._clearButton.Text = "Limpar";
            this._clearButton.UseVisualStyleBackColor = true;
            this._clearButton.Click += new System.EventHandler(this.OnClearButtonClick);
            // 
            // _closeButton
            // 
            this._closeButton.AutoSize = true;
            this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._closeButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._closeButton.Location = new System.Drawing.Point(722, 3);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(75, 22);
            this._closeButton.TabIndex = 4;
            this._closeButton.Text = "Fechar";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this.OnCloseButtonClick);
            // 
            // _resultsGroupBox
            // 
            this._resultsGroupBox.Controls.Add(this._logsGrid);
            this._resultsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._resultsGroupBox.Location = new System.Drawing.Point(15, 158);
            this._resultsGroupBox.Name = "_resultsGroupBox";
            this._resultsGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this._resultsGroupBox.Size = new System.Drawing.Size(1154, 416);
            this._resultsGroupBox.TabIndex = 2;
            this._resultsGroupBox.TabStop = false;
            this._resultsGroupBox.Text = "Resultados";
            // 
            // _logsGrid
            // 
            this._logsGrid.AllowUserToAddRows = false;
            this._logsGrid.AllowUserToDeleteRows = false;
            this._logsGrid.AutoGenerateColumns = false;
            this._logsGrid.BackgroundColor = System.Drawing.Color.White;
            this._logsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._idColumn,
            this._dateTimeColumn,
            this._userNameColumn,
            this._actionColumn,
            this._detailsSummaryColumn});
            this._logsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logsGrid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._logsGrid.Location = new System.Drawing.Point(8, 24);
            this._logsGrid.MultiSelect = false;
            this._logsGrid.Name = "_logsGrid";
            this._logsGrid.ReadOnly = true;
            this._logsGrid.RowHeadersVisible = false;
            this._logsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._logsGrid.Size = new System.Drawing.Size(1138, 384);
            this._logsGrid.TabIndex = 0;
            this._logsGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnGridCellDoubleClick);
            // 
            // _idColumn
            // 
            this._idColumn.DataPropertyName = "Id";
            this._idColumn.HeaderText = "ID";
            this._idColumn.Name = "_idColumn";
            this._idColumn.ReadOnly = true;
            this._idColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._idColumn.Width = 70;
            // 
            // _dateTimeColumn
            // 
            this._dateTimeColumn.DataPropertyName = "DateTime";
            this._dateTimeColumn.HeaderText = "Data/Hora";
            this._dateTimeColumn.Name = "_dateTimeColumn";
            this._dateTimeColumn.ReadOnly = true;
            this._dateTimeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._dateTimeColumn.Width = 150;
            // 
            // _userNameColumn
            // 
            this._userNameColumn.DataPropertyName = "UserName";
            this._userNameColumn.HeaderText = "Usuario";
            this._userNameColumn.Name = "_userNameColumn";
            this._userNameColumn.ReadOnly = true;
            this._userNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._userNameColumn.Width = 180;
            // 
            // _actionColumn
            // 
            this._actionColumn.DataPropertyName = "Action";
            this._actionColumn.HeaderText = "Acao";
            this._actionColumn.Name = "_actionColumn";
            this._actionColumn.ReadOnly = true;
            this._actionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._actionColumn.Width = 220;
            // 
            // _detailsSummaryColumn
            // 
            this._detailsSummaryColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._detailsSummaryColumn.DataPropertyName = "DetailsSummary";
            this._detailsSummaryColumn.HeaderText = "Detalhes";
            this._detailsSummaryColumn.Name = "_detailsSummaryColumn";
            this._detailsSummaryColumn.ReadOnly = true;
            this._detailsSummaryColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _pagingLayout
            // 
            this._pagingLayout.ColumnCount = 2;
            this._pagingLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._pagingLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._pagingLayout.Controls.Add(this._pagingInfoPanel, 0, 0);
            this._pagingLayout.Controls.Add(this._pagingButtonsPanel, 1, 0);
            this._pagingLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pagingLayout.Location = new System.Drawing.Point(15, 580);
            this._pagingLayout.Name = "_pagingLayout";
            this._pagingLayout.RowCount = 1;
            this._pagingLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._pagingLayout.Size = new System.Drawing.Size(1154, 34);
            this._pagingLayout.TabIndex = 3;
            // 
            // _pagingInfoPanel
            // 
            this._pagingInfoPanel.Controls.Add(this._infoLabel);
            this._pagingInfoPanel.Controls.Add(this._pageLabel);
            this._pagingInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pagingInfoPanel.Location = new System.Drawing.Point(3, 3);
            this._pagingInfoPanel.Name = "_pagingInfoPanel";
            this._pagingInfoPanel.Size = new System.Drawing.Size(979, 28);
            this._pagingInfoPanel.TabIndex = 0;
            // 
            // _infoLabel
            // 
            this._infoLabel.AutoSize = true;
            this._infoLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._infoLabel.ForeColor = System.Drawing.Color.DimGray;
            this._infoLabel.Location = new System.Drawing.Point(0, 7);
            this._infoLabel.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this._infoLabel.Name = "_infoLabel";
            this._infoLabel.Size = new System.Drawing.Size(0, 13);
            this._infoLabel.TabIndex = 0;
            // 
            // _pageLabel
            // 
            this._pageLabel.AutoSize = true;
            this._pageLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._pageLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._pageLabel.Location = new System.Drawing.Point(18, 7);
            this._pageLabel.Margin = new System.Windows.Forms.Padding(18, 7, 0, 0);
            this._pageLabel.Name = "_pageLabel";
            this._pageLabel.Size = new System.Drawing.Size(0, 13);
            this._pageLabel.TabIndex = 1;
            // 
            // _pagingButtonsPanel
            // 
            this._pagingButtonsPanel.AutoSize = true;
            this._pagingButtonsPanel.Controls.Add(this._previousButton);
            this._pagingButtonsPanel.Controls.Add(this._nextButton);
            this._pagingButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pagingButtonsPanel.Location = new System.Drawing.Point(988, 3);
            this._pagingButtonsPanel.Name = "_pagingButtonsPanel";
            this._pagingButtonsPanel.Size = new System.Drawing.Size(163, 28);
            this._pagingButtonsPanel.TabIndex = 1;
            this._pagingButtonsPanel.WrapContents = false;
            // 
            // _previousButton
            // 
            this._previousButton.AutoSize = true;
            this._previousButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._previousButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._previousButton.Location = new System.Drawing.Point(3, 3);
            this._previousButton.Name = "_previousButton";
            this._previousButton.Size = new System.Drawing.Size(75, 22);
            this._previousButton.TabIndex = 0;
            this._previousButton.Text = "Anterior";
            this._previousButton.UseVisualStyleBackColor = true;
            this._previousButton.Click += new System.EventHandler(this.OnPreviousButtonClick);
            // 
            // _nextButton
            // 
            this._nextButton.AutoSize = true;
            this._nextButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._nextButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._nextButton.Location = new System.Drawing.Point(84, 3);
            this._nextButton.Name = "_nextButton";
            this._nextButton.Size = new System.Drawing.Size(75, 22);
            this._nextButton.TabIndex = 1;
            this._nextButton.Text = "Proxima";
            this._nextButton.UseVisualStyleBackColor = true;
            this._nextButton.Click += new System.EventHandler(this.OnNextButtonClick);
            // 
            // _footerLayout
            // 
            this._footerLayout.ColumnCount = 1;
            this._footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._footerLayout.Controls.Add(this._statusLabel, 0, 0);
            this._footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerLayout.Location = new System.Drawing.Point(15, 620);
            this._footerLayout.Name = "_footerLayout";
            this._footerLayout.RowCount = 1;
            this._footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._footerLayout.Size = new System.Drawing.Size(1154, 26);
            this._footerLayout.TabIndex = 4;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusLabel.Location = new System.Drawing.Point(0, 6);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(0, 13);
            this._statusLabel.TabIndex = 0;
            // 
            // ConsultaLogsAuditoriaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1184, 661);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(920, 560);
            this.Name = "ConsultaLogsAuditoriaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Consulta de Logs";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._filtersGroupBox.ResumeLayout(false);
            this._filtersLayout.ResumeLayout(false);
            this._filtersLine1Panel.ResumeLayout(false);
            this._filtersLine1Panel.PerformLayout();
            this._filtersLine2Panel.ResumeLayout(false);
            this._filtersLine2Panel.PerformLayout();
            this._resultsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._logsGrid)).EndInit();
            this._pagingLayout.ResumeLayout(false);
            this._pagingLayout.PerformLayout();
            this._pagingInfoPanel.ResumeLayout(false);
            this._pagingInfoPanel.PerformLayout();
            this._pagingButtonsPanel.ResumeLayout(false);
            this._pagingButtonsPanel.PerformLayout();
            this._footerLayout.ResumeLayout(false);
            this._footerLayout.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
