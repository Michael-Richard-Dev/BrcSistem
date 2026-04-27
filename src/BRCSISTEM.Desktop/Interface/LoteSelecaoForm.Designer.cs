using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface
{
    internal sealed partial class LoteSelecaoForm
    {
        private IContainer components = null;

        private TableLayoutPanel _rootLayout;
        private TableLayoutPanel _filterLayout;
        private Label _filterLabel;
        private TextBox _filterTextBox;
        private Button _confirmButton;
        private GroupBox _resultsGroup;
        private DataGridView _grid;
        private DataGridViewTextBoxColumn _colCodigo;
        private DataGridViewTextBoxColumn _colNome;
        private DataGridViewTextBoxColumn _colStatus;
        private Label _footerLabel;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoteSelecaoForm));
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._filterLayout = new System.Windows.Forms.TableLayoutPanel();
            this._filterLabel = new System.Windows.Forms.Label();
            this._filterTextBox = new System.Windows.Forms.TextBox();
            this._confirmButton = new System.Windows.Forms.Button();
            this._resultsGroup = new System.Windows.Forms.GroupBox();
            this._grid = new System.Windows.Forms.DataGridView();
            this._footerLabel = new System.Windows.Forms.Label();
            this._rootLayout.SuspendLayout();
            this._filterLayout.SuspendLayout();
            this._resultsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._filterLayout, 0, 0);
            this._rootLayout.Controls.Add(this._resultsGroup, 0, 1);
            this._rootLayout.Controls.Add(this._footerLabel, 0, 2);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(12);
            this._rootLayout.RowCount = 3;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this._rootLayout.Size = new System.Drawing.Size(760, 500);
            this._rootLayout.TabIndex = 0;
            // 
            // _filterLayout
            // 
            this._filterLayout.ColumnCount = 4;
            this._filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this._filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this._filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this._filterLayout.Controls.Add(this._filterLabel, 0, 0);
            this._filterLayout.Controls.Add(this._filterTextBox, 1, 0);
            this._filterLayout.Controls.Add(this._confirmButton, 3, 0);
            this._filterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filterLayout.Location = new System.Drawing.Point(12, 12);
            this._filterLayout.Margin = new System.Windows.Forms.Padding(0);
            this._filterLayout.Name = "_filterLayout";
            this._filterLayout.RowCount = 1;
            this._filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._filterLayout.Size = new System.Drawing.Size(736, 36);
            this._filterLayout.TabIndex = 0;
            // 
            // _filterLabel
            // 
            this._filterLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filterLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._filterLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._filterLabel.Location = new System.Drawing.Point(3, 0);
            this._filterLabel.Name = "_filterLabel";
            this._filterLabel.Size = new System.Drawing.Size(49, 36);
            this._filterLabel.TabIndex = 0;
            this._filterLabel.Text = "Buscar:";
            this._filterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _filterTextBox
            // 
            this._filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._filterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._filterTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._filterTextBox.Location = new System.Drawing.Point(58, 6);
            this._filterTextBox.MinimumSize = new System.Drawing.Size(2, 23);
            this._filterTextBox.Name = "_filterTextBox";
            this._filterTextBox.Size = new System.Drawing.Size(553, 23);
            this._filterTextBox.TabIndex = 1;
            this._filterTextBox.TextChanged += new System.EventHandler(this.OnFilterTextChanged);
            // 
            // _confirmButton
            // 
            this._confirmButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._confirmButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._confirmButton.Location = new System.Drawing.Point(629, 3);
            this._confirmButton.Name = "_confirmButton";
            this._confirmButton.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this._confirmButton.Size = new System.Drawing.Size(104, 30);
            this._confirmButton.TabIndex = 2;
            this._confirmButton.Text = "Confirmar";
            this._confirmButton.UseVisualStyleBackColor = true;
            this._confirmButton.Click += new System.EventHandler(this.OnConfirmClick);
            // 
            // _resultsGroup
            // 
            this._resultsGroup.Controls.Add(this._grid);
            this._resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this._resultsGroup.Location = new System.Drawing.Point(12, 51);
            this._resultsGroup.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this._resultsGroup.Name = "_resultsGroup";
            this._resultsGroup.Padding = new System.Windows.Forms.Padding(6);
            this._resultsGroup.Size = new System.Drawing.Size(736, 410);
            this._resultsGroup.TabIndex = 1;
            this._resultsGroup.TabStop = false;
            this._resultsGroup.Text = "Resultados";
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AllowUserToResizeRows = false;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._grid.ColumnHeadersHeight = 26;
            this._grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.EnableHeadersVisualStyles = false;
            this._grid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._grid.Location = new System.Drawing.Point(6, 24);
            this._grid.Margin = new System.Windows.Forms.Padding(0);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(724, 380);
            this._grid.TabIndex = 0;
            this._grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnGridCellDoubleClick);
            this._grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnGridKeyDown);
            // 
            // _footerLabel
            // 
            this._footerLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._footerLabel.ForeColor = System.Drawing.Color.DimGray;
            this._footerLabel.Location = new System.Drawing.Point(15, 467);
            this._footerLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this._footerLabel.Name = "_footerLabel";
            this._footerLabel.Size = new System.Drawing.Size(730, 21);
            this._footerLabel.TabIndex = 2;
            this._footerLabel.Text = "Dica: filtre por codigo ou nome e pressione Enter para confirmar.";
            this._footerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LoteSelecaoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(760, 500);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(680, 420);
            this.Name = "LoteSelecaoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Selecionar Lote";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this._rootLayout.ResumeLayout(false);
            this._filterLayout.ResumeLayout(false);
            this._filterLayout.PerformLayout();
            this._resultsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
