namespace BRCSISTEM.Desktop.Interface
{
    partial class ConsultaNotaEntradaForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel _rootLayout;
        private System.Windows.Forms.GroupBox _filterGroupBox;
        private System.Windows.Forms.TableLayoutPanel _filterLayout;
        private System.Windows.Forms.Label _filterLabel;
        private System.Windows.Forms.TextBox _filterTextBox;
        private System.Windows.Forms.GroupBox _resultsGroupBox;
        private System.Windows.Forms.DataGridView _grid;
        private System.Windows.Forms.TableLayoutPanel _footerLayout;
        private System.Windows.Forms.Label _footerInfoLabel;
        private System.Windows.Forms.FlowLayoutPanel _footerButtonsPanel;
        private System.Windows.Forms.Button _selectButton;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsultaNotaEntradaForm));
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._filterGroupBox = new System.Windows.Forms.GroupBox();
            this._filterLayout = new System.Windows.Forms.TableLayoutPanel();
            this._filterLabel = new System.Windows.Forms.Label();
            this._filterTextBox = new System.Windows.Forms.TextBox();
            this._resultsGroupBox = new System.Windows.Forms.GroupBox();
            this._grid = new System.Windows.Forms.DataGridView();
            this._footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this._footerInfoLabel = new System.Windows.Forms.Label();
            this._footerButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._selectButton = new System.Windows.Forms.Button();
            this._rootLayout.SuspendLayout();
            this._filterGroupBox.SuspendLayout();
            this._filterLayout.SuspendLayout();
            this._resultsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this._footerLayout.SuspendLayout();
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
            this._filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this._filterLayout.Controls.Add(this._selectButton, 3, 0);
            this._filterLayout.Controls.Add(this._filterLabel, 0, 0);
            this._filterLayout.Controls.Add(this._filterTextBox, 1, 0);
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
            this._filterLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._filterLabel.Location = new System.Drawing.Point(3, 12);
            this._filterLabel.Margin = new System.Windows.Forms.Padding(3, 0, 8, 0);
            this._filterLabel.Name = "_filterLabel";
            this._filterLabel.Size = new System.Drawing.Size(173, 13);
            this._filterLabel.TabIndex = 0;
            this._filterLabel.Text = "Nota de Entrada ou Fornecedor:";
            // 
            // _filterTextBox
            // 
            this._filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._filterTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._filterTextBox.Location = new System.Drawing.Point(184, 8);
            this._filterTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this._filterTextBox.Name = "_filterTextBox";
            this._filterTextBox.Size = new System.Drawing.Size(647, 22);
            this._filterTextBox.TabIndex = 1;
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
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._grid.ColumnHeadersHeight = 28;
            this._grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
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
            this._footerInfoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerInfoLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._footerInfoLabel.ForeColor = System.Drawing.Color.DimGray;
            this._footerInfoLabel.Location = new System.Drawing.Point(3, 0);
            this._footerInfoLabel.Name = "_footerInfoLabel";
            this._footerInfoLabel.Size = new System.Drawing.Size(942, 46);
            this._footerInfoLabel.TabIndex = 0;
            this._footerInfoLabel.Text = "As notas ativas podem ser abertas para consulta, alteracao e cancelamento. Notas " +
    "canceladas abrem somente para leitura.";
            this._footerInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _footerButtonsPanel
            // 
            this._footerButtonsPanel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._footerButtonsPanel.AutoSize = true;
            this._footerButtonsPanel.Location = new System.Drawing.Point(952, 23);
            this._footerButtonsPanel.Name = "_footerButtonsPanel";
            this._footerButtonsPanel.Size = new System.Drawing.Size(0, 0);
            this._footerButtonsPanel.TabIndex = 1;
            this._footerButtonsPanel.WrapContents = false;
            // 
            // _selectButton
            // 
            this._selectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._selectButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._selectButton.Location = new System.Drawing.Point(842, 6);
            this._selectButton.Name = "_selectButton";
            this._selectButton.Size = new System.Drawing.Size(89, 26);
            this._selectButton.TabIndex = 0;
            this._selectButton.Text = "Selecionar";
            this._selectButton.UseVisualStyleBackColor = true;
            // 
            // ConsultaNotaEntradaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
            this.ResumeLayout(false);

        }
    }
}
