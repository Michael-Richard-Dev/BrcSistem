using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class GerenciamentoSolicitacoesAcessoForm
    {
        private IContainer components = null;

        private TableLayoutPanel _root;

        private GroupBox _listArea;
        private DataGridView _grid;
        private DataGridViewTextBoxColumn _colNome;
        private DataGridViewTextBoxColumn _colEmail;
        private DataGridViewTextBoxColumn _colJustificativa;
        private DataGridViewTextBoxColumn _colData;

        private GroupBox _detailArea;
        private TableLayoutPanel _detailLayout;
        private Label _nameLabel;
        private Label _emailLabel;
        private Label _dateLabel;
        private TextBox _justificationText;

        private TableLayoutPanel _buttonBar;
        private Button _approveButton;
        private Button _cancelButton;
        private Label _buttonSpacer;
        private FlowLayoutPanel _rightButtons;
        private Button _refreshButton;

        private Label _statusLabel;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GerenciamentoSolicitacoesAcessoForm));
            this._root = new System.Windows.Forms.TableLayoutPanel();
            this._listArea = new System.Windows.Forms.GroupBox();
            this._grid = new System.Windows.Forms.DataGridView();
            this._detailArea = new System.Windows.Forms.GroupBox();
            this._detailLayout = new System.Windows.Forms.TableLayoutPanel();
            this._nameLabel = new System.Windows.Forms.Label();
            this._emailLabel = new System.Windows.Forms.Label();
            this._dateLabel = new System.Windows.Forms.Label();
            this._justificationText = new System.Windows.Forms.TextBox();
            this._buttonBar = new System.Windows.Forms.TableLayoutPanel();
            this._approveButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._buttonSpacer = new System.Windows.Forms.Label();
            this._rightButtons = new System.Windows.Forms.FlowLayoutPanel();
            this._refreshButton = new System.Windows.Forms.Button();
            this._statusLabel = new System.Windows.Forms.Label();
            this._root.SuspendLayout();
            this._listArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this._detailArea.SuspendLayout();
            this._detailLayout.SuspendLayout();
            this._buttonBar.SuspendLayout();
            this._rightButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // _root
            // 
            this._root.ColumnCount = 1;
            this._root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.Controls.Add(this._listArea, 0, 1);
            this._root.Controls.Add(this._detailArea, 0, 2);
            this._root.Controls.Add(this._buttonBar, 0, 3);
            this._root.Controls.Add(this._statusLabel, 0, 4);
            this._root.Dock = System.Windows.Forms.DockStyle.Fill;
            this._root.Location = new System.Drawing.Point(0, 0);
            this._root.Name = "_root";
            this._root.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this._root.RowCount = 5;
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._root.Size = new System.Drawing.Size(1000, 550);
            this._root.TabIndex = 0;
            // 
            // _listArea
            // 
            this._listArea.Controls.Add(this._grid);
            this._listArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this._listArea.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this._listArea.Location = new System.Drawing.Point(18, 13);
            this._listArea.Name = "_listArea";
            this._listArea.Padding = new System.Windows.Forms.Padding(8);
            this._listArea.Size = new System.Drawing.Size(964, 353);
            this._listArea.TabIndex = 1;
            this._listArea.TabStop = false;
            this._listArea.Text = " Solicitacoes ";
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._grid.Location = new System.Drawing.Point(8, 25);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(948, 320);
            this._grid.TabIndex = 0;
            this._grid.SelectionChanged += new System.EventHandler(this.OnGridSelectionChanged);
            this._grid.DoubleClick += new System.EventHandler(this.OnGridDoubleClick);
            // 
            // _detailArea
            // 
            this._detailArea.Controls.Add(this._detailLayout);
            this._detailArea.Dock = System.Windows.Forms.DockStyle.Top;
            this._detailArea.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._detailArea.Location = new System.Drawing.Point(15, 377);
            this._detailArea.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this._detailArea.Name = "_detailArea";
            this._detailArea.Padding = new System.Windows.Forms.Padding(6);
            this._detailArea.Size = new System.Drawing.Size(970, 110);
            this._detailArea.TabIndex = 2;
            this._detailArea.TabStop = false;
            this._detailArea.Text = " Detalhes ";
            // 
            // _detailLayout
            // 
            this._detailLayout.ColumnCount = 1;
            this._detailLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._detailLayout.Controls.Add(this._nameLabel, 0, 0);
            this._detailLayout.Controls.Add(this._emailLabel, 0, 1);
            this._detailLayout.Controls.Add(this._dateLabel, 0, 2);
            this._detailLayout.Controls.Add(this._justificationText, 0, 3);
            this._detailLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detailLayout.Location = new System.Drawing.Point(6, 22);
            this._detailLayout.Name = "_detailLayout";
            this._detailLayout.RowCount = 4;
            this._detailLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._detailLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._detailLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._detailLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._detailLayout.Size = new System.Drawing.Size(958, 82);
            this._detailLayout.TabIndex = 0;
            // 
            // _nameLabel
            // 
            this._nameLabel.AutoSize = true;
            this._nameLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._nameLabel.Location = new System.Drawing.Point(0, 2);
            this._nameLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new System.Drawing.Size(43, 13);
            this._nameLabel.TabIndex = 0;
            this._nameLabel.Text = "Nome: ";
            // 
            // _emailLabel
            // 
            this._emailLabel.AutoSize = true;
            this._emailLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._emailLabel.Location = new System.Drawing.Point(0, 17);
            this._emailLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this._emailLabel.Name = "_emailLabel";
            this._emailLabel.Size = new System.Drawing.Size(44, 13);
            this._emailLabel.TabIndex = 1;
            this._emailLabel.Text = "E-mail: ";
            // 
            // _dateLabel
            // 
            this._dateLabel.AutoSize = true;
            this._dateLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._dateLabel.Location = new System.Drawing.Point(0, 32);
            this._dateLabel.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this._dateLabel.Name = "_dateLabel";
            this._dateLabel.Size = new System.Drawing.Size(37, 13);
            this._dateLabel.TabIndex = 2;
            this._dateLabel.Text = "Data: ";
            // 
            // _justificationText
            // 
            this._justificationText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._justificationText.Dock = System.Windows.Forms.DockStyle.Fill;
            this._justificationText.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._justificationText.Location = new System.Drawing.Point(3, 48);
            this._justificationText.Multiline = true;
            this._justificationText.Name = "_justificationText";
            this._justificationText.ReadOnly = true;
            this._justificationText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._justificationText.Size = new System.Drawing.Size(952, 31);
            this._justificationText.TabIndex = 3;
            this._justificationText.Text = "Selecione uma solicitacao para ver os detalhes";
            // 
            // _buttonBar
            // 
            this._buttonBar.AutoSize = true;
            this._buttonBar.ColumnCount = 4;
            this._buttonBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._buttonBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonBar.Controls.Add(this._approveButton, 0, 0);
            this._buttonBar.Controls.Add(this._cancelButton, 1, 0);
            this._buttonBar.Controls.Add(this._buttonSpacer, 2, 0);
            this._buttonBar.Controls.Add(this._rightButtons, 3, 0);
            this._buttonBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this._buttonBar.Location = new System.Drawing.Point(15, 492);
            this._buttonBar.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this._buttonBar.Name = "_buttonBar";
            this._buttonBar.RowCount = 1;
            this._buttonBar.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._buttonBar.Size = new System.Drawing.Size(970, 29);
            this._buttonBar.TabIndex = 3;
            // 
            // _approveButton
            // 
            this._approveButton.AutoSize = true;
            this._approveButton.Enabled = false;
            this._approveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._approveButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._approveButton.Location = new System.Drawing.Point(0, 0);
            this._approveButton.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this._approveButton.Name = "_approveButton";
            this._approveButton.Size = new System.Drawing.Size(82, 23);
            this._approveButton.TabIndex = 0;
            this._approveButton.Text = "Aprovar (F1)";
            this._approveButton.UseVisualStyleBackColor = true;
            this._approveButton.Click += new System.EventHandler(this.OnApproveClick);
            // 
            // _cancelButton
            // 
            this._cancelButton.AutoSize = true;
            this._cancelButton.Enabled = false;
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._cancelButton.Location = new System.Drawing.Point(86, 0);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(86, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancelar (F3)";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.OnCancelClick);
            // 
            // _buttonSpacer
            // 
            this._buttonSpacer.Location = new System.Drawing.Point(179, 0);
            this._buttonSpacer.Name = "_buttonSpacer";
            this._buttonSpacer.Size = new System.Drawing.Size(1, 1);
            this._buttonSpacer.TabIndex = 2;
            // 
            // _rightButtons
            // 
            this._rightButtons.AutoSize = true;
            this._rightButtons.Controls.Add(this._refreshButton);
            this._rightButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this._rightButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this._rightButtons.Location = new System.Drawing.Point(884, 3);
            this._rightButtons.Name = "_rightButtons";
            this._rightButtons.Size = new System.Drawing.Size(83, 23);
            this._rightButtons.TabIndex = 3;
            this._rightButtons.WrapContents = false;
            // 
            // _refreshButton
            // 
            this._refreshButton.AutoSize = true;
            this._refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._refreshButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._refreshButton.Location = new System.Drawing.Point(4, 0);
            this._refreshButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._refreshButton.Name = "_refreshButton";
            this._refreshButton.Size = new System.Drawing.Size(75, 23);
            this._refreshButton.TabIndex = 1;
            this._refreshButton.Text = "Atualizar";
            this._refreshButton.UseVisualStyleBackColor = true;
            this._refreshButton.Click += new System.EventHandler(this.OnRefreshClick);
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._statusLabel.ForeColor = System.Drawing.Color.Gray;
            this._statusLabel.Location = new System.Drawing.Point(15, 527);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(42, 13);
            this._statusLabel.TabIndex = 4;
            this._statusLabel.Text = "Pronto";
            // 
            // GerenciamentoSolicitacoesAcessoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1000, 550);
            this.Controls.Add(this._root);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(950, 500);
            this.Name = "GerenciamentoSolicitacoesAcessoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Gerenciar Acessos";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
            this._root.ResumeLayout(false);
            this._root.PerformLayout();
            this._listArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this._detailArea.ResumeLayout(false);
            this._detailLayout.ResumeLayout(false);
            this._detailLayout.PerformLayout();
            this._buttonBar.ResumeLayout(false);
            this._buttonBar.PerformLayout();
            this._rightButtons.ResumeLayout(false);
            this._rightButtons.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
