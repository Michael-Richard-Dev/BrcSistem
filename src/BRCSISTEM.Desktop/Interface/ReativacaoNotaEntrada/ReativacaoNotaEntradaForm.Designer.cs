using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.ReativacaoNotaEntrada
{
    public sealed partial class ReativacaoNotaEntradaForm
    {
        private TableLayoutPanel _rootLayout;
        private FlowLayoutPanel _headerPanel;
        private Label _titleLabel;
        private Label _subtitleLabel;
        private GroupBox _searchGroupBox;
        private FlowLayoutPanel _searchPanel;
        private Label _numberLabel;
        private TextBox _numberTextBox;
        private Label _supplierLabel;
        private TextBox _supplierTextBox;
        private Button _searchButton;
        private Button _clearButton;
        private Button _loadAllButton;
        private GroupBox _gridGroupBox;
        private DataGridView _grid;
        private DataGridViewTextBoxColumn _numberColumn;
        private DataGridViewTextBoxColumn _supplierColumn;
        private DataGridViewTextBoxColumn _warehouseColumn;
        private DataGridViewTextBoxColumn _versionColumn;
        private DataGridViewTextBoxColumn _emissionDateColumn;
        private DataGridViewTextBoxColumn _statusColumn;
        private TableLayoutPanel _footerLayout;
        private Label _statusLabel;
        private FlowLayoutPanel _footerButtonsPanel;
        private Button _reactivateButton;
        private Button _closeButton;

        private void InitializeComponent()
        {
            _rootLayout = new TableLayoutPanel();
            _headerPanel = new FlowLayoutPanel();
            _titleLabel = new Label();
            _subtitleLabel = new Label();
            _searchGroupBox = new GroupBox();
            _searchPanel = new FlowLayoutPanel();
            _numberLabel = new Label();
            _numberTextBox = new TextBox();
            _supplierLabel = new Label();
            _supplierTextBox = new TextBox();
            _searchButton = new Button();
            _clearButton = new Button();
            _loadAllButton = new Button();
            _gridGroupBox = new GroupBox();
            _grid = new DataGridView();
            _numberColumn = new DataGridViewTextBoxColumn();
            _supplierColumn = new DataGridViewTextBoxColumn();
            _warehouseColumn = new DataGridViewTextBoxColumn();
            _versionColumn = new DataGridViewTextBoxColumn();
            _emissionDateColumn = new DataGridViewTextBoxColumn();
            _statusColumn = new DataGridViewTextBoxColumn();
            _footerLayout = new TableLayoutPanel();
            _statusLabel = new Label();
            _footerButtonsPanel = new FlowLayoutPanel();
            _reactivateButton = new Button();
            _closeButton = new Button();
            _rootLayout.SuspendLayout();
            _headerPanel.SuspendLayout();
            _searchGroupBox.SuspendLayout();
            _searchPanel.SuspendLayout();
            _gridGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_grid).BeginInit();
            _footerLayout.SuspendLayout();
            _footerButtonsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _rootLayout
            // 
            _rootLayout.ColumnCount = 1;
            _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _rootLayout.Controls.Add(_headerPanel, 0, 0);
            _rootLayout.Controls.Add(_searchGroupBox, 0, 1);
            _rootLayout.Controls.Add(_gridGroupBox, 0, 2);
            _rootLayout.Controls.Add(_footerLayout, 0, 3);
            _rootLayout.Dock = DockStyle.Fill;
            _rootLayout.Location = new Point(0, 0);
            _rootLayout.Name = "_rootLayout";
            _rootLayout.Padding = new Padding(10);
            _rootLayout.RowCount = 4;
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _rootLayout.Size = new Size(800, 500);
            _rootLayout.TabIndex = 0;
            // 
            // _headerPanel
            // 
            _headerPanel.AutoSize = true;
            _headerPanel.Controls.Add(_titleLabel);
            _headerPanel.Controls.Add(_subtitleLabel);
            _headerPanel.Dock = DockStyle.Fill;
            _headerPanel.Location = new Point(10, 10);
            _headerPanel.Margin = new Padding(0, 0, 0, 10);
            _headerPanel.Name = "_headerPanel";
            _headerPanel.Size = new Size(780, 34);
            _headerPanel.TabIndex = 0;
            _headerPanel.WrapContents = true;
            // 
            // _titleLabel
            // 
            _titleLabel.AutoSize = true;
            _titleLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            _titleLabel.ForeColor = Color.FromArgb(27, 54, 93);
            _titleLabel.Location = new Point(0, 4);
            _titleLabel.Margin = new Padding(0, 4, 10, 4);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new Size(226, 25);
            _titleLabel.TabIndex = 0;
            _titleLabel.Text = "Reativar Nota de Entrada";
            // 
            // _subtitleLabel
            // 
            _subtitleLabel.AutoSize = true;
            _subtitleLabel.Font = new Font("Segoe UI", 9F);
            _subtitleLabel.ForeColor = Color.FromArgb(102, 102, 102);
            _subtitleLabel.Location = new Point(236, 8);
            _subtitleLabel.Margin = new Padding(0, 8, 0, 4);
            _subtitleLabel.Name = "_subtitleLabel";
            _subtitleLabel.Size = new Size(223, 15);
            _subtitleLabel.TabIndex = 1;
            _subtitleLabel.Text = "Voltar notas canceladas para status ATIVO";
            // 
            // _searchGroupBox
            // 
            _searchGroupBox.AutoSize = true;
            _searchGroupBox.Controls.Add(_searchPanel);
            _searchGroupBox.Dock = DockStyle.Top;
            _searchGroupBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _searchGroupBox.Location = new Point(10, 54);
            _searchGroupBox.Margin = new Padding(0, 0, 0, 8);
            _searchGroupBox.Name = "_searchGroupBox";
            _searchGroupBox.Size = new Size(780, 69);
            _searchGroupBox.TabIndex = 1;
            _searchGroupBox.TabStop = false;
            _searchGroupBox.Text = "Buscar Notas Canceladas";
            // 
            // _searchPanel
            // 
            _searchPanel.AutoSize = true;
            _searchPanel.Controls.Add(_numberLabel);
            _searchPanel.Controls.Add(_numberTextBox);
            _searchPanel.Controls.Add(_supplierLabel);
            _searchPanel.Controls.Add(_supplierTextBox);
            _searchPanel.Controls.Add(_searchButton);
            _searchPanel.Controls.Add(_clearButton);
            _searchPanel.Controls.Add(_loadAllButton);
            _searchPanel.Dock = DockStyle.Fill;
            _searchPanel.Location = new Point(3, 21);
            _searchPanel.Name = "_searchPanel";
            _searchPanel.Padding = new Padding(10);
            _searchPanel.Size = new Size(774, 45);
            _searchPanel.TabIndex = 0;
            _searchPanel.WrapContents = true;
            // 
            // _numberLabel
            // 
            _numberLabel.AutoSize = true;
            _numberLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _numberLabel.ForeColor = Color.FromArgb(27, 54, 93);
            _numberLabel.Location = new Point(10, 18);
            _numberLabel.Margin = new Padding(0, 8, 6, 4);
            _numberLabel.Name = "_numberLabel";
            _numberLabel.Size = new Size(62, 17);
            _numberLabel.TabIndex = 0;
            _numberLabel.Text = "Numero:";
            // 
            // _numberTextBox
            // 
            _numberTextBox.Font = new Font("Segoe UI", 10F);
            _numberTextBox.Location = new Point(78, 13);
            _numberTextBox.Name = "_numberTextBox";
            _numberTextBox.Size = new Size(110, 25);
            _numberTextBox.TabIndex = 1;
            _numberTextBox.KeyDown += OnSearchKeyDown;
            // 
            // _supplierLabel
            // 
            _supplierLabel.AutoSize = true;
            _supplierLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _supplierLabel.ForeColor = Color.FromArgb(27, 54, 93);
            _supplierLabel.Location = new Point(191, 18);
            _supplierLabel.Margin = new Padding(0, 8, 6, 4);
            _supplierLabel.Name = "_supplierLabel";
            _supplierLabel.Size = new Size(78, 17);
            _supplierLabel.TabIndex = 2;
            _supplierLabel.Text = "Fornecedor:";
            // 
            // _supplierTextBox
            // 
            _supplierTextBox.Font = new Font("Segoe UI", 10F);
            _supplierTextBox.Location = new Point(275, 13);
            _supplierTextBox.Name = "_supplierTextBox";
            _supplierTextBox.Size = new Size(110, 25);
            _supplierTextBox.TabIndex = 3;
            _supplierTextBox.KeyDown += OnSearchKeyDown;
            // 
            // _searchButton
            // 
            _searchButton.AutoSize = true;
            _searchButton.FlatStyle = FlatStyle.System;
            _searchButton.Font = new Font("Segoe UI", 8.25F);
            _searchButton.Location = new Point(391, 13);
            _searchButton.Margin = new Padding(6, 3, 0, 3);
            _searchButton.Name = "_searchButton";
            _searchButton.Size = new Size(75, 24);
            _searchButton.TabIndex = 4;
            _searchButton.Text = "Pesquisar";
            _searchButton.UseVisualStyleBackColor = true;
            _searchButton.Click += OnSearchButtonClick;
            // 
            // _clearButton
            // 
            _clearButton.AutoSize = true;
            _clearButton.FlatStyle = FlatStyle.System;
            _clearButton.Font = new Font("Segoe UI", 8.25F);
            _clearButton.Location = new Point(472, 13);
            _clearButton.Margin = new Padding(6, 3, 0, 3);
            _clearButton.Name = "_clearButton";
            _clearButton.Size = new Size(75, 24);
            _clearButton.TabIndex = 5;
            _clearButton.Text = "Limpar";
            _clearButton.UseVisualStyleBackColor = true;
            _clearButton.Click += OnClearButtonClick;
            // 
            // _loadAllButton
            // 
            _loadAllButton.AutoSize = true;
            _loadAllButton.FlatStyle = FlatStyle.System;
            _loadAllButton.Font = new Font("Segoe UI", 8.25F);
            _loadAllButton.Location = new Point(553, 13);
            _loadAllButton.Margin = new Padding(6, 3, 0, 3);
            _loadAllButton.Name = "_loadAllButton";
            _loadAllButton.Size = new Size(112, 24);
            _loadAllButton.TabIndex = 6;
            _loadAllButton.Text = "Todos Cancelados";
            _loadAllButton.UseVisualStyleBackColor = true;
            _loadAllButton.Click += OnLoadAllButtonClick;
            // 
            // _gridGroupBox
            // 
            _gridGroupBox.Controls.Add(_grid);
            _gridGroupBox.Dock = DockStyle.Fill;
            _gridGroupBox.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _gridGroupBox.Location = new Point(10, 131);
            _gridGroupBox.Margin = new Padding(0);
            _gridGroupBox.Name = "_gridGroupBox";
            _gridGroupBox.Padding = new Padding(8);
            _gridGroupBox.Size = new Size(780, 319);
            _gridGroupBox.TabIndex = 2;
            _gridGroupBox.TabStop = false;
            _gridGroupBox.Text = "Notas Canceladas";
            // 
            // _grid
            // 
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AllowUserToResizeRows = false;
            _grid.AutoGenerateColumns = false;
            _grid.BackgroundColor = Color.White;
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _grid.Columns.AddRange(new DataGridViewColumn[] {
            _numberColumn,
            _supplierColumn,
            _warehouseColumn,
            _versionColumn,
            _emissionDateColumn,
            _statusColumn});
            _grid.Dock = DockStyle.Fill;
            _grid.Location = new Point(8, 26);
            _grid.MultiSelect = false;
            _grid.Name = "_grid";
            _grid.ReadOnly = true;
            _grid.RowHeadersVisible = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.Size = new Size(764, 285);
            _grid.TabIndex = 0;
            _grid.CellDoubleClick += OnGridCellDoubleClick;
            // 
            // _numberColumn
            // 
            _numberColumn.DataPropertyName = nameof(InboundReceiptReactivationEntry.Number);
            _numberColumn.HeaderText = "Numero";
            _numberColumn.Name = "_numberColumn";
            _numberColumn.ReadOnly = true;
            _numberColumn.Width = 90;
            // 
            // _supplierColumn
            // 
            _supplierColumn.DataPropertyName = nameof(InboundReceiptReactivationEntry.Supplier);
            _supplierColumn.HeaderText = "Fornecedor";
            _supplierColumn.Name = "_supplierColumn";
            _supplierColumn.ReadOnly = true;
            _supplierColumn.Width = 110;
            // 
            // _warehouseColumn
            // 
            _warehouseColumn.DataPropertyName = nameof(InboundReceiptReactivationEntry.Warehouse);
            _warehouseColumn.HeaderText = "Almoxarifado";
            _warehouseColumn.Name = "_warehouseColumn";
            _warehouseColumn.ReadOnly = true;
            _warehouseColumn.Width = 130;
            // 
            // _versionColumn
            // 
            _versionColumn.DataPropertyName = nameof(InboundReceiptReactivationEntry.Version);
            _versionColumn.HeaderText = "Versao";
            _versionColumn.Name = "_versionColumn";
            _versionColumn.ReadOnly = true;
            _versionColumn.Width = 70;
            // 
            // _emissionDateColumn
            // 
            _emissionDateColumn.DataPropertyName = nameof(InboundReceiptReactivationEntry.EmissionDate);
            _emissionDateColumn.HeaderText = "Data Emissao";
            _emissionDateColumn.Name = "_emissionDateColumn";
            _emissionDateColumn.ReadOnly = true;
            _emissionDateColumn.Width = 120;
            // 
            // _statusColumn
            // 
            _statusColumn.DataPropertyName = nameof(InboundReceiptReactivationEntry.Status);
            _statusColumn.HeaderText = "Status";
            _statusColumn.Name = "_statusColumn";
            _statusColumn.ReadOnly = true;
            _statusColumn.Width = 90;
            // 
            // _footerLayout
            // 
            _footerLayout.AutoSize = true;
            _footerLayout.ColumnCount = 2;
            _footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _footerLayout.Controls.Add(_statusLabel, 0, 0);
            _footerLayout.Controls.Add(_footerButtonsPanel, 1, 0);
            _footerLayout.Dock = DockStyle.Fill;
            _footerLayout.Location = new Point(10, 460);
            _footerLayout.Margin = new Padding(0, 10, 0, 0);
            _footerLayout.Name = "_footerLayout";
            _footerLayout.RowCount = 1;
            _footerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _footerLayout.Size = new Size(780, 30);
            _footerLayout.TabIndex = 3;
            // 
            // _statusLabel
            // 
            _statusLabel.AutoSize = true;
            _statusLabel.Dock = DockStyle.Fill;
            _statusLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            _statusLabel.ForeColor = Color.SeaGreen;
            _statusLabel.Location = new Point(0, 8);
            _statusLabel.Margin = new Padding(0, 8, 0, 0);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new Size(607, 22);
            _statusLabel.TabIndex = 0;
            // 
            // _footerButtonsPanel
            // 
            _footerButtonsPanel.AutoSize = true;
            _footerButtonsPanel.Controls.Add(_reactivateButton);
            _footerButtonsPanel.Controls.Add(_closeButton);
            _footerButtonsPanel.Dock = DockStyle.Right;
            _footerButtonsPanel.Location = new Point(607, 0);
            _footerButtonsPanel.Margin = new Padding(0);
            _footerButtonsPanel.Name = "_footerButtonsPanel";
            _footerButtonsPanel.Size = new Size(173, 30);
            _footerButtonsPanel.TabIndex = 1;
            _footerButtonsPanel.WrapContents = false;
            // 
            // _reactivateButton
            // 
            _reactivateButton.AutoSize = true;
            _reactivateButton.FlatStyle = FlatStyle.System;
            _reactivateButton.Font = new Font("Segoe UI", 8.25F);
            _reactivateButton.Location = new Point(6, 3);
            _reactivateButton.Margin = new Padding(6, 3, 0, 3);
            _reactivateButton.Name = "_reactivateButton";
            _reactivateButton.Size = new Size(111, 24);
            _reactivateButton.TabIndex = 0;
            _reactivateButton.Text = "Reativar Selecionada";
            _reactivateButton.UseVisualStyleBackColor = true;
            _reactivateButton.Click += OnReactivateButtonClick;
            // 
            // _closeButton
            // 
            _closeButton.AutoSize = true;
            _closeButton.DialogResult = DialogResult.Cancel;
            _closeButton.FlatStyle = FlatStyle.System;
            _closeButton.Font = new Font("Segoe UI", 8.25F);
            _closeButton.Location = new Point(123, 3);
            _closeButton.Margin = new Padding(6, 3, 0, 3);
            _closeButton.Name = "_closeButton";
            _closeButton.Size = new Size(50, 24);
            _closeButton.TabIndex = 1;
            _closeButton.Text = "Fechar";
            _closeButton.UseVisualStyleBackColor = true;
            _closeButton.Click += OnCloseButtonClick;
            // 
            // ReativacaoNotaEntradaForm
            // 
            AcceptButton = _searchButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = _closeButton;
            ClientSize = new Size(800, 500);
            Controls.Add(_rootLayout);
            Font = new Font("Segoe UI", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MaximumSize = new Size(800, 500);
            MinimizeBox = false;
            MinimumSize = new Size(800, 500);
            Name = "ReativacaoNotaEntradaForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "BRCSISTEM - Reativar Nota de Entrada";
            _rootLayout.ResumeLayout(false);
            _rootLayout.PerformLayout();
            _headerPanel.ResumeLayout(false);
            _headerPanel.PerformLayout();
            _searchGroupBox.ResumeLayout(false);
            _searchGroupBox.PerformLayout();
            _searchPanel.ResumeLayout(false);
            _searchPanel.PerformLayout();
            _gridGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_grid).EndInit();
            _footerLayout.ResumeLayout(false);
            _footerLayout.PerformLayout();
            _footerButtonsPanel.ResumeLayout(false);
            _footerButtonsPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}
