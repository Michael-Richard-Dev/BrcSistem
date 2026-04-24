namespace BRCSISTEM.Desktop.Views
{
    partial class ConsultaNotaEntradaForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel _rootLayout;
        private System.Windows.Forms.GroupBox _filterGroupBox;
        private System.Windows.Forms.TableLayoutPanel _filterLayout;
        private System.Windows.Forms.Label _filterLabel;
        private System.Windows.Forms.TextBox _filterTextBox;
        private System.Windows.Forms.Button _searchButton;
        private System.Windows.Forms.GroupBox _resultsGroupBox;
        private System.Windows.Forms.DataGridView _grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn _numberColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _supplierColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _warehouseColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _movementDateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _statusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _lockedByColumn;
        private System.Windows.Forms.TableLayoutPanel _footerLayout;
        private System.Windows.Forms.Label _footerInfoLabel;
        private System.Windows.Forms.FlowLayoutPanel _footerButtonsPanel;
        private System.Windows.Forms.Button _selectButton;
        private System.Windows.Forms.Button _closeButton;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._filterGroupBox = new System.Windows.Forms.GroupBox();
            this._filterLayout = new System.Windows.Forms.TableLayoutPanel();
            this._filterLabel = new System.Windows.Forms.Label();
            this._filterTextBox = new System.Windows.Forms.TextBox();
            this._searchButton = new System.Windows.Forms.Button();
            this._resultsGroupBox = new System.Windows.Forms.GroupBox();
            this._grid = new System.Windows.Forms.DataGridView();
            this._numberColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._supplierColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._warehouseColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._movementDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._statusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._lockedByColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this._footerInfoLabel = new System.Windows.Forms.Label();
            this._footerButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._selectButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this._rootLayout.SuspendLayout();
            this._filterGroupBox.SuspendLayout();
            this._filterLayout.SuspendLayout();
            this._resultsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this._footerLayout.SuspendLayout();
            this._footerButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._filterGroupBox, 0, 0);
            this._rootLayout.Controls.Add(this._resultsGroupBox, 0, 1);
            this._rootLayout.Controls.Add(this._footerLayout, 0, 2);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(12);
            this._rootLayout.RowCount = 3;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this._rootLayout.Size = new System.Drawing.Size(984, 561);
            this._rootLayout.TabIndex = 0;
            // 
            // _filterGroupBox
            // 
            this._filterGroupBox.Controls.Add(this._filterLayout);
            this._filterGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filterGroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._filterGroupBox.Location = new System.Drawing.Point(15, 15);
            this._filterGroupBox.Name = "_filterGroupBox";
            this._filterGroupBox.Padding = new System.Windows.Forms.Padding(10, 8, 10, 10);
            this._filterGroupBox.Size = new System.Drawing.Size(954, 72);
            this._filterGroupBox.TabIndex = 0;
            this._filterGroupBox.TabStop = false;
            this._filterGroupBox.Text = "Pesquisa";
            // 
            // _filterLayout
            // 
            this._filterLayout.ColumnCount = 3;
            this._filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._filterLayout.Controls.Add(this._filterLabel, 0, 0);
            this._filterLayout.Controls.Add(this._filterTextBox, 1, 0);
            this._filterLayout.Controls.Add(this._searchButton, 2, 0);
            this._filterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filterLayout.Location = new System.Drawing.Point(10, 24);
            this._filterLayout.Name = "_filterLayout";
            this._filterLayout.RowCount = 1;
            this._filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._filterLayout.Size = new System.Drawing.Size(934, 38);
            this._filterLayout.TabIndex = 0;
            // 
            // _filterLabel
            // 
            this._filterLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._filterLabel.AutoSize = true;
            this._filterLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._filterLabel.Location = new System.Drawing.Point(3, 12);
            this._filterLabel.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            this._filterLabel.Name = "_filterLabel";
            this._filterLabel.Size = new System.Drawing.Size(151, 13);
            this._filterLabel.TabIndex = 0;
            this._filterLabel.Text = "Pesquisar nota ou fornecedor:";
            // 
            // _filterTextBox
            // 
            this._filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._filterTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._filterTextBox.Location = new System.Drawing.Point(165, 8);
            this._filterTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this._filterTextBox.Name = "_filterTextBox";
            this._filterTextBox.Size = new System.Drawing.Size(676, 22);
            this._filterTextBox.TabIndex = 1;
            // 
            // _searchButton
            // 
            this._searchButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._searchButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._searchButton.Location = new System.Drawing.Point(849, 5);
            this._searchButton.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(82, 28);
            this._searchButton.TabIndex = 2;
            this._searchButton.Text = "Pesquisar";
            this._searchButton.UseVisualStyleBackColor = true;
            // 
            // _resultsGroupBox
            // 
            this._resultsGroupBox.Controls.Add(this._grid);
            this._resultsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._resultsGroupBox.Location = new System.Drawing.Point(15, 93);
            this._resultsGroupBox.Name = "_resultsGroupBox";
            this._resultsGroupBox.Padding = new System.Windows.Forms.Padding(10, 8, 10, 10);
            this._resultsGroupBox.Size = new System.Drawing.Size(954, 401);
            this._resultsGroupBox.TabIndex = 1;
            this._resultsGroupBox.TabStop = false;
            this._resultsGroupBox.Text = "Notas encontradas";
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AllowUserToResizeRows = false;
            this._grid.AutoGenerateColumns = false;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._grid.ColumnHeadersHeight = 28;
            this._grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._numberColumn,
            this._supplierColumn,
            this._warehouseColumn,
            this._movementDateColumn,
            this._statusColumn,
            this._lockedByColumn});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._grid.DefaultCellStyle = dataGridViewCellStyle2;
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._grid.Location = new System.Drawing.Point(10, 24);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.RowTemplate.Height = 24;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(934, 367);
            this._grid.TabIndex = 0;
            // 
            // _numberColumn
            // 
            this._numberColumn.DataPropertyName = "Number";
            this._numberColumn.HeaderText = "NOTA";
            this._numberColumn.Name = "_numberColumn";
            this._numberColumn.ReadOnly = true;
            this._numberColumn.Width = 90;
            // 
            // _supplierColumn
            // 
            this._supplierColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._supplierColumn.DataPropertyName = "SupplierDisplay";
            this._supplierColumn.HeaderText = "FORNECEDOR";
            this._supplierColumn.Name = "_supplierColumn";
            this._supplierColumn.ReadOnly = true;
            // 
            // _warehouseColumn
            // 
            this._warehouseColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._warehouseColumn.DataPropertyName = "WarehouseDisplay";
            this._warehouseColumn.HeaderText = "ALMOXARIFADO";
            this._warehouseColumn.Name = "_warehouseColumn";
            this._warehouseColumn.ReadOnly = true;
            // 
            // _movementDateColumn
            // 
            this._movementDateColumn.DataPropertyName = "MovementDateTimeDisplay";
            this._movementDateColumn.HeaderText = "RECEBIMENTO";
            this._movementDateColumn.Name = "_movementDateColumn";
            this._movementDateColumn.ReadOnly = true;
            this._movementDateColumn.Width = 150;
            // 
            // _statusColumn
            // 
            this._statusColumn.DataPropertyName = "Status";
            this._statusColumn.HeaderText = "STATUS";
            this._statusColumn.Name = "_statusColumn";
            this._statusColumn.ReadOnly = true;
            this._statusColumn.Width = 110;
            // 
            // _lockedByColumn
            // 
            this._lockedByColumn.DataPropertyName = "LockedBy";
            this._lockedByColumn.HeaderText = "BLOQUEADO POR";
            this._lockedByColumn.Name = "_lockedByColumn";
            this._lockedByColumn.ReadOnly = true;
            this._lockedByColumn.Width = 140;
            // 
            // _footerLayout
            // 
            this._footerLayout.ColumnCount = 2;
            this._footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._footerLayout.Controls.Add(this._footerInfoLabel, 0, 0);
            this._footerLayout.Controls.Add(this._footerButtonsPanel, 1, 0);
            this._footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerLayout.Location = new System.Drawing.Point(15, 500);
            this._footerLayout.Name = "_footerLayout";
            this._footerLayout.RowCount = 1;
            this._footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._footerLayout.Size = new System.Drawing.Size(954, 46);
            this._footerLayout.TabIndex = 2;
            // 
            // _footerInfoLabel
            // 
            this._footerInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._footerInfoLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._footerInfoLabel.ForeColor = System.Drawing.Color.DimGray;
            this._footerInfoLabel.Location = new System.Drawing.Point(3, 3);
            this._footerInfoLabel.Name = "_footerInfoLabel";
            this._footerInfoLabel.Size = new System.Drawing.Size(761, 40);
            this._footerInfoLabel.TabIndex = 0;
            this._footerInfoLabel.Text = "As notas ativas podem ser abertas para consulta, alteracao e cancelamento. Notas canceladas abrem somente para leitura.";
            this._footerInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _footerButtonsPanel
            // 
            this._footerButtonsPanel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._footerButtonsPanel.AutoSize = true;
            this._footerButtonsPanel.Controls.Add(this._selectButton);
            this._footerButtonsPanel.Controls.Add(this._closeButton);
            this._footerButtonsPanel.Location = new System.Drawing.Point(770, 7);
            this._footerButtonsPanel.Name = "_footerButtonsPanel";
            this._footerButtonsPanel.Size = new System.Drawing.Size(181, 32);
            this._footerButtonsPanel.TabIndex = 1;
            this._footerButtonsPanel.WrapContents = false;
            // 
            // _selectButton
            // 
            this._selectButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._selectButton.Location = new System.Drawing.Point(3, 3);
            this._selectButton.Name = "_selectButton";
            this._selectButton.Size = new System.Drawing.Size(92, 26);
            this._selectButton.TabIndex = 0;
            this._selectButton.Text = "Selecionar";
            this._selectButton.UseVisualStyleBackColor = true;
            // 
            // _closeButton
            // 
            this._closeButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._closeButton.Location = new System.Drawing.Point(101, 3);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(77, 26);
            this._closeButton.TabIndex = 1;
            this._closeButton.Text = "Fechar";
            this._closeButton.UseVisualStyleBackColor = true;
            // 
            // ConsultaNotaEntradaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.MinimumSize = new System.Drawing.Size(860, 460);
            this.Name = "ConsultaNotaEntradaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Consultar Notas";
            this._rootLayout.ResumeLayout(false);
            this._filterGroupBox.ResumeLayout(false);
            this._filterLayout.ResumeLayout(false);
            this._filterLayout.PerformLayout();
            this._resultsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this._footerLayout.ResumeLayout(false);
            this._footerLayout.PerformLayout();
            this._footerButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
