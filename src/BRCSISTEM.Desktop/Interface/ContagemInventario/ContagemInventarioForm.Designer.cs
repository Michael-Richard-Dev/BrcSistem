namespace BRCSISTEM.Desktop.Interface.ContagemInventario
{
    partial class ContagemInventarioForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel _rootTableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel _topTableLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel _editorFlowLayoutPanel;
        private System.Windows.Forms.GroupBox _logGroupBox;

        private System.Windows.Forms.Label _pointLabel;
        private System.Windows.Forms.Label _statusLabel;
        private System.Windows.Forms.Label _warehouseLabel;
        private System.Windows.Forms.Label _materialLabel;
        private System.Windows.Forms.Label _lotLabel;
        private System.Windows.Forms.Label _quantityLabel;

        private System.Windows.Forms.ComboBox _warehouseComboBox;
        private System.Windows.Forms.ComboBox _materialComboBox;
        private System.Windows.Forms.ComboBox _lotComboBox;
        private System.Windows.Forms.TextBox _quantityTextBox;

        private System.Windows.Forms.Button _registerButton;
        private System.Windows.Forms.Button _refreshButton;
        private System.Windows.Forms.Button _closeButton;

        private System.Windows.Forms.DataGridView _logGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn _idColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _dateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _itemColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _quantityColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _userColumn;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            _rootTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            _topTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            _editorFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            _logGroupBox = new System.Windows.Forms.GroupBox();

            _pointLabel = new System.Windows.Forms.Label();
            _statusLabel = new System.Windows.Forms.Label();
            _warehouseLabel = new System.Windows.Forms.Label();
            _materialLabel = new System.Windows.Forms.Label();
            _lotLabel = new System.Windows.Forms.Label();
            _quantityLabel = new System.Windows.Forms.Label();

            _warehouseComboBox = new System.Windows.Forms.ComboBox();
            _materialComboBox = new System.Windows.Forms.ComboBox();
            _lotComboBox = new System.Windows.Forms.ComboBox();
            _quantityTextBox = new System.Windows.Forms.TextBox();

            _registerButton = new System.Windows.Forms.Button();
            _refreshButton = new System.Windows.Forms.Button();
            _closeButton = new System.Windows.Forms.Button();

            _logGrid = new System.Windows.Forms.DataGridView();
            _idColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            _dateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            _itemColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            _quantityColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            _userColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();

            _rootTableLayoutPanel.SuspendLayout();
            _topTableLayoutPanel.SuspendLayout();
            _editorFlowLayoutPanel.SuspendLayout();
            _logGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_logGrid)).BeginInit();
            SuspendLayout();

            // 
            // _rootTableLayoutPanel
            // 
            _rootTableLayoutPanel.ColumnCount = 1;
            _rootTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _rootTableLayoutPanel.Controls.Add(_topTableLayoutPanel, 0, 0);
            _rootTableLayoutPanel.Controls.Add(_editorFlowLayoutPanel, 0, 1);
            _rootTableLayoutPanel.Controls.Add(_logGroupBox, 0, 2);
            _rootTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _rootTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            _rootTableLayoutPanel.Name = "_rootTableLayoutPanel";
            _rootTableLayoutPanel.Padding = new System.Windows.Forms.Padding(12);
            _rootTableLayoutPanel.RowCount = 3;
            _rootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _rootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _rootTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _rootTableLayoutPanel.Size = new System.Drawing.Size(964, 581);
            _rootTableLayoutPanel.TabIndex = 0;

            // 
            // _topTableLayoutPanel
            // 
            _topTableLayoutPanel.AutoSize = true;
            _topTableLayoutPanel.ColumnCount = 1;
            _topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            _topTableLayoutPanel.Controls.Add(_pointLabel, 0, 0);
            _topTableLayoutPanel.Controls.Add(_statusLabel, 0, 1);
            _topTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            _topTableLayoutPanel.Location = new System.Drawing.Point(15, 15);
            _topTableLayoutPanel.Name = "_topTableLayoutPanel";
            _topTableLayoutPanel.RowCount = 2;
            _topTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _topTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            _topTableLayoutPanel.Size = new System.Drawing.Size(934, 43);
            _topTableLayoutPanel.TabIndex = 0;

            // 
            // _pointLabel
            // 
            _pointLabel.AutoSize = true;
            _pointLabel.Dock = System.Windows.Forms.DockStyle.Top;
            _pointLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            _pointLabel.ForeColor = System.Drawing.Color.FromArgb(27, 54, 93);
            _pointLabel.Location = new System.Drawing.Point(3, 0);
            _pointLabel.Name = "_pointLabel";
            _pointLabel.Size = new System.Drawing.Size(928, 23);
            _pointLabel.TabIndex = 0;
            _pointLabel.Text = "Ponto:";

            // 
            // _statusLabel
            // 
            _statusLabel.AutoSize = true;
            _statusLabel.Dock = System.Windows.Forms.DockStyle.Top;
            _statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            _statusLabel.Location = new System.Drawing.Point(3, 23);
            _statusLabel.Name = "_statusLabel";
            _statusLabel.Size = new System.Drawing.Size(928, 20);
            _statusLabel.TabIndex = 1;
            _statusLabel.Text = "Inventario:";

            // 
            // _editorFlowLayoutPanel
            // 
            _editorFlowLayoutPanel.AutoSize = true;
            _editorFlowLayoutPanel.Controls.Add(_warehouseLabel);
            _editorFlowLayoutPanel.Controls.Add(_warehouseComboBox);
            _editorFlowLayoutPanel.Controls.Add(_materialLabel);
            _editorFlowLayoutPanel.Controls.Add(_materialComboBox);
            _editorFlowLayoutPanel.Controls.Add(_lotLabel);
            _editorFlowLayoutPanel.Controls.Add(_lotComboBox);
            _editorFlowLayoutPanel.Controls.Add(_quantityLabel);
            _editorFlowLayoutPanel.Controls.Add(_quantityTextBox);
            _editorFlowLayoutPanel.Controls.Add(_registerButton);
            _editorFlowLayoutPanel.Controls.Add(_refreshButton);
            _editorFlowLayoutPanel.Controls.Add(_closeButton);
            _editorFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            _editorFlowLayoutPanel.Location = new System.Drawing.Point(15, 64);
            _editorFlowLayoutPanel.Name = "_editorFlowLayoutPanel";
            _editorFlowLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 8);
            _editorFlowLayoutPanel.Size = new System.Drawing.Size(934, 50);
            _editorFlowLayoutPanel.TabIndex = 1;
            _editorFlowLayoutPanel.WrapContents = true;

            // 
            // _warehouseLabel
            // 
            _warehouseLabel.AutoSize = true;
            _warehouseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _warehouseLabel.Location = new System.Drawing.Point(0, 16);
            _warehouseLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            _warehouseLabel.Name = "_warehouseLabel";
            _warehouseLabel.Size = new System.Drawing.Size(105, 20);
            _warehouseLabel.TabIndex = 0;
            _warehouseLabel.Text = "Almoxarifado:";

            // 
            // _warehouseComboBox
            // 
            _warehouseComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _warehouseComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            _warehouseComboBox.FormattingEnabled = true;
            _warehouseComboBox.Location = new System.Drawing.Point(108, 11);
            _warehouseComboBox.Name = "_warehouseComboBox";
            _warehouseComboBox.Size = new System.Drawing.Size(180, 31);
            _warehouseComboBox.TabIndex = 1;

            // 
            // _materialLabel
            // 
            _materialLabel.AutoSize = true;
            _materialLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _materialLabel.Location = new System.Drawing.Point(291, 16);
            _materialLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            _materialLabel.Name = "_materialLabel";
            _materialLabel.Size = new System.Drawing.Size(70, 20);
            _materialLabel.TabIndex = 2;
            _materialLabel.Text = "Material:";

            // 
            // _materialComboBox
            // 
            _materialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _materialComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            _materialComboBox.FormattingEnabled = true;
            _materialComboBox.Location = new System.Drawing.Point(364, 11);
            _materialComboBox.Name = "_materialComboBox";
            _materialComboBox.Size = new System.Drawing.Size(280, 31);
            _materialComboBox.TabIndex = 3;

            // 
            // _lotLabel
            // 
            _lotLabel.AutoSize = true;
            _lotLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _lotLabel.Location = new System.Drawing.Point(647, 16);
            _lotLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            _lotLabel.Name = "_lotLabel";
            _lotLabel.Size = new System.Drawing.Size(42, 20);
            _lotLabel.TabIndex = 4;
            _lotLabel.Text = "Lote:";

            // 
            // _lotComboBox
            // 
            _lotComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            _lotComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            _lotComboBox.FormattingEnabled = true;
            _lotComboBox.Location = new System.Drawing.Point(692, 11);
            _lotComboBox.Name = "_lotComboBox";
            _lotComboBox.Size = new System.Drawing.Size(220, 31);
            _lotComboBox.TabIndex = 5;

            // 
            // _quantityLabel
            // 
            _quantityLabel.AutoSize = true;
            _quantityLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            _quantityLabel.Location = new System.Drawing.Point(0, 58);
            _quantityLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            _quantityLabel.Name = "_quantityLabel";
            _quantityLabel.Size = new System.Drawing.Size(94, 20);
            _quantityLabel.TabIndex = 6;
            _quantityLabel.Text = "Quantidade:";

            // 
            // _quantityTextBox
            // 
            _quantityTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            _quantityTextBox.Location = new System.Drawing.Point(97, 53);
            _quantityTextBox.Name = "_quantityTextBox";
            _quantityTextBox.Size = new System.Drawing.Size(120, 30);
            _quantityTextBox.TabIndex = 7;

            // 
            // _registerButton
            // 
            _registerButton.AutoSize = true;
            _registerButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            _registerButton.Location = new System.Drawing.Point(223, 53);
            _registerButton.Name = "_registerButton";
            _registerButton.Size = new System.Drawing.Size(90, 30);
            _registerButton.TabIndex = 8;
            _registerButton.Text = "Registrar";
            _registerButton.UseVisualStyleBackColor = true;

            // 
            // _refreshButton
            // 
            _refreshButton.AutoSize = true;
            _refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            _refreshButton.Location = new System.Drawing.Point(319, 53);
            _refreshButton.Name = "_refreshButton";
            _refreshButton.Size = new System.Drawing.Size(82, 30);
            _refreshButton.TabIndex = 9;
            _refreshButton.Text = "Atualizar";
            _refreshButton.UseVisualStyleBackColor = true;

            // 
            // _closeButton
            // 
            _closeButton.AutoSize = true;
            _closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            _closeButton.Location = new System.Drawing.Point(407, 53);
            _closeButton.Name = "_closeButton";
            _closeButton.Size = new System.Drawing.Size(68, 30);
            _closeButton.TabIndex = 10;
            _closeButton.Text = "Fechar";
            _closeButton.UseVisualStyleBackColor = true;

            // 
            // _logGroupBox
            // 
            _logGroupBox.Controls.Add(_logGrid);
            _logGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            _logGroupBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            _logGroupBox.Location = new System.Drawing.Point(15, 120);
            _logGroupBox.Name = "_logGroupBox";
            _logGroupBox.Size = new System.Drawing.Size(934, 446);
            _logGroupBox.TabIndex = 2;
            _logGroupBox.TabStop = false;
            _logGroupBox.Text = "Ultimas Leituras do Ponto";

            // 
            // _logGrid
            // 
            _logGrid.AllowUserToAddRows = false;
            _logGrid.AllowUserToDeleteRows = false;
            _logGrid.AutoGenerateColumns = false;
            _logGrid.BackgroundColor = System.Drawing.Color.White;
            _logGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _logGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
            {
                _idColumn,
                _dateColumn,
                _itemColumn,
                _quantityColumn,
                _userColumn
            });
            _logGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            _logGrid.Location = new System.Drawing.Point(3, 26);
            _logGrid.MultiSelect = false;
            _logGrid.Name = "_logGrid";
            _logGrid.ReadOnly = true;
            _logGrid.RowHeadersVisible = false;
            _logGrid.RowHeadersWidth = 51;
            _logGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _logGrid.Size = new System.Drawing.Size(928, 417);
            _logGrid.TabIndex = 0;

            // 
            // _idColumn
            // 
            _idColumn.DataPropertyName = "Id";
            _idColumn.HeaderText = "ID";
            _idColumn.MinimumWidth = 6;
            _idColumn.Name = "_idColumn";
            _idColumn.ReadOnly = true;
            _idColumn.Width = 70;

            // 
            // _dateColumn
            // 
            _dateColumn.DataPropertyName = "CountedAtDisplay";
            _dateColumn.HeaderText = "DATA/HORA";
            _dateColumn.MinimumWidth = 6;
            _dateColumn.Name = "_dateColumn";
            _dateColumn.ReadOnly = true;
            _dateColumn.Width = 150;

            // 
            // _itemColumn
            // 
            _itemColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            _itemColumn.DataPropertyName = "ItemDisplay";
            _itemColumn.HeaderText = "ITEM";
            _itemColumn.MinimumWidth = 6;
            _itemColumn.Name = "_itemColumn";
            _itemColumn.ReadOnly = true;

            // 
            // _quantityColumn
            // 
            _quantityColumn.DataPropertyName = "QuantityText";
            _quantityColumn.HeaderText = "QTD";
            _quantityColumn.MinimumWidth = 6;
            _quantityColumn.Name = "_quantityColumn";
            _quantityColumn.ReadOnly = true;
            _quantityColumn.Width = 110;

            // 
            // _userColumn
            // 
            _userColumn.DataPropertyName = "UserName";
            _userColumn.HeaderText = "USUARIO";
            _userColumn.MinimumWidth = 6;
            _userColumn.Name = "_userColumn";
            _userColumn.ReadOnly = true;
            _userColumn.Width = 120;

            // 
            // ContagemInventarioForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(964, 581);
            Controls.Add(_rootTableLayoutPanel);
            MinimumSize = new System.Drawing.Size(860, 520);
            Name = "ContagemInventarioForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "BRCSISTEM - Contagem Cega";

            _rootTableLayoutPanel.ResumeLayout(false);
            _rootTableLayoutPanel.PerformLayout();
            _topTableLayoutPanel.ResumeLayout(false);
            _topTableLayoutPanel.PerformLayout();
            _editorFlowLayoutPanel.ResumeLayout(false);
            _editorFlowLayoutPanel.PerformLayout();
            _logGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_logGrid)).EndInit();
            ResumeLayout(false);
        }
    }
}