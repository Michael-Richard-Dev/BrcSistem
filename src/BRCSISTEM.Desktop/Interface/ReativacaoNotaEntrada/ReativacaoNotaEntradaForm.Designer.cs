using System.Drawing;
using System.Windows.Forms;

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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReativacaoNotaEntradaForm));
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._headerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this._searchGroupBox = new System.Windows.Forms.GroupBox();
            this._searchPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._numberLabel = new System.Windows.Forms.Label();
            this._numberTextBox = new System.Windows.Forms.TextBox();
            this._supplierLabel = new System.Windows.Forms.Label();
            this._supplierTextBox = new System.Windows.Forms.TextBox();
            this._searchButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this._loadAllButton = new System.Windows.Forms.Button();
            this._gridGroupBox = new System.Windows.Forms.GroupBox();
            this._grid = new System.Windows.Forms.DataGridView();
            this._footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this._statusLabel = new System.Windows.Forms.Label();
            this._footerButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._reactivateButton = new System.Windows.Forms.Button();
            this._rootLayout.SuspendLayout();
            this._headerPanel.SuspendLayout();
            this._searchGroupBox.SuspendLayout();
            this._searchPanel.SuspendLayout();
            this._gridGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this._footerLayout.SuspendLayout();
            this._footerButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerPanel, 0, 0);
            this._rootLayout.Controls.Add(this._searchGroupBox, 0, 1);
            this._rootLayout.Controls.Add(this._gridGroupBox, 0, 2);
            this._rootLayout.Controls.Add(this._footerLayout, 0, 3);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(10);
            this._rootLayout.RowCount = 4;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.Size = new System.Drawing.Size(784, 461);
            this._rootLayout.TabIndex = 0;
            // 
            // _headerPanel
            // 
            this._headerPanel.AutoSize = true;
            this._headerPanel.Controls.Add(this._titleLabel);
            this._headerPanel.Controls.Add(this._subtitleLabel);
            this._headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._headerPanel.Location = new System.Drawing.Point(10, 10);
            this._headerPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Size = new System.Drawing.Size(764, 33);
            this._headerPanel.TabIndex = 0;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Location = new System.Drawing.Point(0, 4);
            this._titleLabel.Margin = new System.Windows.Forms.Padding(0, 4, 10, 4);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(227, 25);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Reativar Nota de Entrada";
            // 
            // _subtitleLabel
            // 
            this._subtitleLabel.AutoSize = true;
            this._subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._subtitleLabel.Location = new System.Drawing.Point(237, 8);
            this._subtitleLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this._subtitleLabel.Name = "_subtitleLabel";
            this._subtitleLabel.Size = new System.Drawing.Size(226, 15);
            this._subtitleLabel.TabIndex = 1;
            this._subtitleLabel.Text = "Voltar notas canceladas para status ATIVO";
            // 
            // _searchGroupBox
            // 
            this._searchGroupBox.AutoSize = true;
            this._searchGroupBox.Controls.Add(this._searchPanel);
            this._searchGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._searchGroupBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._searchGroupBox.Location = new System.Drawing.Point(10, 53);
            this._searchGroupBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this._searchGroupBox.Name = "_searchGroupBox";
            this._searchGroupBox.Size = new System.Drawing.Size(764, 75);
            this._searchGroupBox.TabIndex = 1;
            this._searchGroupBox.TabStop = false;
            this._searchGroupBox.Text = "Buscar Notas Canceladas";
            // 
            // _searchPanel
            // 
            this._searchPanel.AutoSize = true;
            this._searchPanel.Controls.Add(this._numberLabel);
            this._searchPanel.Controls.Add(this._numberTextBox);
            this._searchPanel.Controls.Add(this._supplierLabel);
            this._searchPanel.Controls.Add(this._supplierTextBox);
            this._searchPanel.Controls.Add(this._searchButton);
            this._searchPanel.Controls.Add(this._clearButton);
            this._searchPanel.Controls.Add(this._loadAllButton);
            this._searchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._searchPanel.Location = new System.Drawing.Point(3, 21);
            this._searchPanel.Name = "_searchPanel";
            this._searchPanel.Padding = new System.Windows.Forms.Padding(10);
            this._searchPanel.Size = new System.Drawing.Size(758, 51);
            this._searchPanel.TabIndex = 0;
            // 
            // _numberLabel
            // 
            this._numberLabel.AutoSize = true;
            this._numberLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._numberLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._numberLabel.Location = new System.Drawing.Point(10, 18);
            this._numberLabel.Margin = new System.Windows.Forms.Padding(0, 8, 6, 4);
            this._numberLabel.Name = "_numberLabel";
            this._numberLabel.Size = new System.Drawing.Size(59, 17);
            this._numberLabel.TabIndex = 0;
            this._numberLabel.Text = "Numero:";
            // 
            // _numberTextBox
            // 
            this._numberTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._numberTextBox.Location = new System.Drawing.Point(78, 13);
            this._numberTextBox.Name = "_numberTextBox";
            this._numberTextBox.Size = new System.Drawing.Size(110, 25);
            this._numberTextBox.TabIndex = 1;
            // 
            // _supplierLabel
            // 
            this._supplierLabel.AutoSize = true;
            this._supplierLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._supplierLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._supplierLabel.Location = new System.Drawing.Point(191, 18);
            this._supplierLabel.Margin = new System.Windows.Forms.Padding(0, 8, 6, 4);
            this._supplierLabel.Name = "_supplierLabel";
            this._supplierLabel.Size = new System.Drawing.Size(78, 17);
            this._supplierLabel.TabIndex = 2;
            this._supplierLabel.Text = "Fornecedor:";
            // 
            // _supplierTextBox
            // 
            this._supplierTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._supplierTextBox.Location = new System.Drawing.Point(278, 13);
            this._supplierTextBox.Name = "_supplierTextBox";
            this._supplierTextBox.Size = new System.Drawing.Size(110, 25);
            this._supplierTextBox.TabIndex = 3;
            // 
            // _searchButton
            // 
            this._searchButton.AutoSize = true;
            this._searchButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._searchButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._searchButton.Location = new System.Drawing.Point(397, 13);
            this._searchButton.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(75, 24);
            this._searchButton.TabIndex = 4;
            this._searchButton.Text = "Pesquisar";
            this._searchButton.UseVisualStyleBackColor = true;
            // 
            // _clearButton
            // 
            this._clearButton.AutoSize = true;
            this._clearButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._clearButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._clearButton.Location = new System.Drawing.Point(478, 13);
            this._clearButton.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(75, 24);
            this._clearButton.TabIndex = 5;
            this._clearButton.Text = "Limpar";
            this._clearButton.UseVisualStyleBackColor = true;
            // 
            // _loadAllButton
            // 
            this._loadAllButton.AutoSize = true;
            this._loadAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._loadAllButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._loadAllButton.Location = new System.Drawing.Point(559, 13);
            this._loadAllButton.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this._loadAllButton.Name = "_loadAllButton";
            this._loadAllButton.Size = new System.Drawing.Size(113, 24);
            this._loadAllButton.TabIndex = 6;
            this._loadAllButton.Text = "Todos Cancelados";
            this._loadAllButton.UseVisualStyleBackColor = true;
            // 
            // _gridGroupBox
            // 
            this._gridGroupBox.Controls.Add(this._grid);
            this._gridGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridGroupBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._gridGroupBox.Location = new System.Drawing.Point(10, 136);
            this._gridGroupBox.Margin = new System.Windows.Forms.Padding(0);
            this._gridGroupBox.Name = "_gridGroupBox";
            this._gridGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this._gridGroupBox.Size = new System.Drawing.Size(764, 275);
            this._gridGroupBox.TabIndex = 2;
            this._gridGroupBox.TabStop = false;
            this._gridGroupBox.Text = "Notas Canceladas";
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AllowUserToResizeRows = false;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Location = new System.Drawing.Point(8, 26);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(748, 241);
            this._grid.TabIndex = 0;
            // 
            // _footerLayout
            // 
            this._footerLayout.AutoSize = true;
            this._footerLayout.ColumnCount = 2;
            this._footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._footerLayout.Controls.Add(this._statusLabel, 0, 0);
            this._footerLayout.Controls.Add(this._footerButtonsPanel, 1, 0);
            this._footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerLayout.Location = new System.Drawing.Point(10, 421);
            this._footerLayout.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._footerLayout.Name = "_footerLayout";
            this._footerLayout.RowCount = 1;
            this._footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._footerLayout.Size = new System.Drawing.Size(764, 30);
            this._footerLayout.TabIndex = 3;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this._statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusLabel.Location = new System.Drawing.Point(0, 8);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(631, 22);
            this._statusLabel.TabIndex = 0;
            // 
            // _footerButtonsPanel
            // 
            this._footerButtonsPanel.AutoSize = true;
            this._footerButtonsPanel.Controls.Add(this._reactivateButton);
            this._footerButtonsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this._footerButtonsPanel.Location = new System.Drawing.Point(631, 0);
            this._footerButtonsPanel.Margin = new System.Windows.Forms.Padding(0);
            this._footerButtonsPanel.Name = "_footerButtonsPanel";
            this._footerButtonsPanel.Size = new System.Drawing.Size(133, 30);
            this._footerButtonsPanel.TabIndex = 1;
            this._footerButtonsPanel.WrapContents = false;
            // 
            // _reactivateButton
            // 
            this._reactivateButton.AutoSize = true;
            this._reactivateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._reactivateButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._reactivateButton.Location = new System.Drawing.Point(6, 3);
            this._reactivateButton.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
            this._reactivateButton.Name = "_reactivateButton";
            this._reactivateButton.Size = new System.Drawing.Size(127, 24);
            this._reactivateButton.TabIndex = 0;
            this._reactivateButton.Text = "Reativar Selecionada";
            this._reactivateButton.UseVisualStyleBackColor = true;
            // 
            // ReativacaoNotaEntradaForm
            // 
            this.AcceptButton = this._searchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 500);
            this.Name = "ReativacaoNotaEntradaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Reativar Nota de Entrada";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._headerPanel.ResumeLayout(false);
            this._headerPanel.PerformLayout();
            this._searchGroupBox.ResumeLayout(false);
            this._searchGroupBox.PerformLayout();
            this._searchPanel.ResumeLayout(false);
            this._searchPanel.PerformLayout();
            this._gridGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this._footerLayout.ResumeLayout(false);
            this._footerLayout.PerformLayout();
            this._footerButtonsPanel.ResumeLayout(false);
            this._footerButtonsPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
