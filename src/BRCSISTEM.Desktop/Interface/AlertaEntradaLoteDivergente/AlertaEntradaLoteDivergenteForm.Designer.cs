using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.AlertaEntradaLoteDivergente
{
    public sealed partial class AlertaEntradaLoteDivergenteForm
    {
        private IContainer components = null;

        private TableLayoutPanel _root;

        private Panel _headerPanel;
        private Label _titleLabel;
        private Label _subtitleLabel;

        private GroupBox _actionsGroup;
        private FlowLayoutPanel _actionsFlow;
        private Button _consultButton;
        private Button _fixButton;
        private Button _closeButton;
        private Label _infoLabel;

        private GroupBox _resultsGroup;
        private DataGridView _grid;
        private DataGridViewTextBoxColumn _colDocumento;
        private DataGridViewTextBoxColumn _colMaterial;
        private DataGridViewTextBoxColumn _colLoteMovimento;
        private DataGridViewTextBoxColumn _colLoteNota;
        private DataGridViewTextBoxColumn _colAlmoxarifado;
        private DataGridViewTextBoxColumn _colFornecedor;
        private DataGridViewTextBoxColumn _colQtdMov;
        private DataGridViewTextBoxColumn _colQtdNota;
        private DataGridViewTextBoxColumn _colUsuarioMov;
        private DataGridViewTextBoxColumn _colUsuarioNota;
        private DataGridViewTextBoxColumn _colCriadoEm;

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
            System.Windows.Forms.DataGridViewCellStyle styleLeft = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle styleRight = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle styleCenter = new System.Windows.Forms.DataGridViewCellStyle();
            this._root = new System.Windows.Forms.TableLayoutPanel();
            this._headerPanel = new System.Windows.Forms.Panel();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this._titleLabel = new System.Windows.Forms.Label();
            this._actionsGroup = new System.Windows.Forms.GroupBox();
            this._actionsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._consultButton = new System.Windows.Forms.Button();
            this._fixButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this._infoLabel = new System.Windows.Forms.Label();
            this._resultsGroup = new System.Windows.Forms.GroupBox();
            this._grid = new System.Windows.Forms.DataGridView();
            this._colDocumento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colMaterial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colLoteMovimento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colLoteNota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colAlmoxarifado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colFornecedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colQtdMov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colQtdNota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colUsuarioMov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colUsuarioNota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colCriadoEm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._root.SuspendLayout();
            this._headerPanel.SuspendLayout();
            this._actionsGroup.SuspendLayout();
            this._actionsFlow.SuspendLayout();
            this._resultsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.SuspendLayout();
            //
            // _root
            //
            this._root.ColumnCount = 1;
            this._root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.Controls.Add(this._headerPanel, 0, 0);
            this._root.Controls.Add(this._actionsGroup, 0, 1);
            this._root.Controls.Add(this._resultsGroup, 0, 2);
            this._root.Dock = System.Windows.Forms.DockStyle.Fill;
            this._root.Location = new System.Drawing.Point(0, 0);
            this._root.Name = "_root";
            this._root.Padding = new System.Windows.Forms.Padding(12);
            this._root.RowCount = 3;
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.Size = new System.Drawing.Size(1400, 760);
            this._root.TabIndex = 0;
            //
            // _headerPanel
            //
            this._headerPanel.AutoSize = true;
            this._headerPanel.Controls.Add(this._subtitleLabel);
            this._headerPanel.Controls.Add(this._titleLabel);
            this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this._headerPanel.TabIndex = 0;
            //
            // _subtitleLabel
            //
            this._subtitleLabel.AutoSize = true;
            this._subtitleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this._subtitleLabel.Name = "_subtitleLabel";
            this._subtitleLabel.TabIndex = 1;
            this._subtitleLabel.Text = "Movimentos de entrada (NOTA) cujo lote registrado nao consta nos itens da nota fiscal";
            //
            // _titleLabel
            //
            this._titleLabel.AutoSize = true;
            this._titleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Alerta: Entradas com Lote Divergente";
            //
            // _actionsGroup
            //
            this._actionsGroup.Controls.Add(this._actionsFlow);
            this._actionsGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._actionsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._actionsGroup.Height = 62;
            this._actionsGroup.Name = "_actionsGroup";
            this._actionsGroup.TabIndex = 1;
            this._actionsGroup.TabStop = false;
            this._actionsGroup.Text = "Acoes";
            //
            // _actionsFlow
            //
            this._actionsFlow.Controls.Add(this._consultButton);
            this._actionsFlow.Controls.Add(this._fixButton);
            this._actionsFlow.Controls.Add(this._closeButton);
            this._actionsFlow.Controls.Add(this._infoLabel);
            this._actionsFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._actionsFlow.Name = "_actionsFlow";
            this._actionsFlow.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this._actionsFlow.TabIndex = 0;
            this._actionsFlow.WrapContents = false;
            //
            // _consultButton
            //
            this._consultButton.AutoSize = true;
            this._consultButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._consultButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._consultButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._consultButton.Name = "_consultButton";
            this._consultButton.TabIndex = 0;
            this._consultButton.Text = "Consultar (F5)";
            this._consultButton.UseVisualStyleBackColor = true;
            this._consultButton.Click += new System.EventHandler(this.OnConsultarClick);
            //
            // _fixButton
            //
            this._fixButton.AutoSize = true;
            this._fixButton.Enabled = false;
            this._fixButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._fixButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._fixButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._fixButton.Name = "_fixButton";
            this._fixButton.TabIndex = 1;
            this._fixButton.Text = "Inativar Selecionado (F7)";
            this._fixButton.UseVisualStyleBackColor = true;
            this._fixButton.Click += new System.EventHandler(this.OnFixClick);
            //
            // _closeButton
            //
            this._closeButton.AutoSize = true;
            this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._closeButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._closeButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._closeButton.Name = "_closeButton";
            this._closeButton.TabIndex = 2;
            this._closeButton.Text = "Fechar (F4)";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this.OnFecharClick);
            //
            // _infoLabel
            //
            this._infoLabel.AutoSize = true;
            this._infoLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._infoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._infoLabel.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this._infoLabel.Name = "_infoLabel";
            this._infoLabel.TabIndex = 3;
            this._infoLabel.Text = "";
            //
            // _resultsGroup
            //
            this._resultsGroup.Controls.Add(this._grid);
            this._resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._resultsGroup.Name = "_resultsGroup";
            this._resultsGroup.TabIndex = 2;
            this._resultsGroup.TabStop = false;
            this._resultsGroup.Text = "Resultados";
            //
            // _grid
            //
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AutoGenerateColumns = false;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this._colDocumento,
                this._colMaterial,
                this._colLoteMovimento,
                this._colLoteNota,
                this._colAlmoxarifado,
                this._colFornecedor,
                this._colQtdMov,
                this._colQtdNota,
                this._colUsuarioMov,
                this._colUsuarioNota,
                this._colCriadoEm});
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.TabIndex = 0;
            this._grid.SelectionChanged += new System.EventHandler(this.OnGridSelectionChanged);
            //
            // estilos de celula
            //
            styleLeft.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            styleRight.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            styleCenter.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            //
            // _colDocumento
            //
            this._colDocumento.DefaultCellStyle = styleLeft;
            this._colDocumento.HeaderText = "DOCUMENTO";
            this._colDocumento.Name = "documento";
            this._colDocumento.ReadOnly = true;
            this._colDocumento.Width = 130;
            //
            // _colMaterial
            //
            this._colMaterial.DefaultCellStyle = styleLeft;
            this._colMaterial.HeaderText = "MATERIAL";
            this._colMaterial.Name = "material";
            this._colMaterial.ReadOnly = true;
            this._colMaterial.Width = 230;
            //
            // _colLoteMovimento
            //
            this._colLoteMovimento.DefaultCellStyle = styleLeft;
            this._colLoteMovimento.HeaderText = "LOTE MOVIMENTO";
            this._colLoteMovimento.Name = "lote_movimento";
            this._colLoteMovimento.ReadOnly = true;
            this._colLoteMovimento.Width = 150;
            //
            // _colLoteNota
            //
            this._colLoteNota.DefaultCellStyle = styleLeft;
            this._colLoteNota.HeaderText = "LOTE NOTA FISCAL";
            this._colLoteNota.Name = "lote_nota";
            this._colLoteNota.ReadOnly = true;
            this._colLoteNota.Width = 150;
            //
            // _colAlmoxarifado
            //
            this._colAlmoxarifado.DefaultCellStyle = styleLeft;
            this._colAlmoxarifado.HeaderText = "ALMOXARIFADO";
            this._colAlmoxarifado.Name = "almoxarifado";
            this._colAlmoxarifado.ReadOnly = true;
            this._colAlmoxarifado.Width = 140;
            //
            // _colFornecedor
            //
            this._colFornecedor.DefaultCellStyle = styleLeft;
            this._colFornecedor.HeaderText = "FORNECEDOR";
            this._colFornecedor.Name = "fornecedor";
            this._colFornecedor.ReadOnly = true;
            this._colFornecedor.Width = 140;
            //
            // _colQtdMov
            //
            this._colQtdMov.DefaultCellStyle = styleRight;
            this._colQtdMov.HeaderText = "QTD. MOV.";
            this._colQtdMov.Name = "qtd_mov";
            this._colQtdMov.ReadOnly = true;
            this._colQtdMov.Width = 85;
            //
            // _colQtdNota
            //
            this._colQtdNota.DefaultCellStyle = styleRight;
            this._colQtdNota.HeaderText = "QTD. NOTA";
            this._colQtdNota.Name = "qtd_nota";
            this._colQtdNota.ReadOnly = true;
            this._colQtdNota.Width = 85;
            //
            // _colUsuarioMov
            //
            this._colUsuarioMov.DefaultCellStyle = styleLeft;
            this._colUsuarioMov.HeaderText = "USUARIO MOV.";
            this._colUsuarioMov.Name = "usuario_mov";
            this._colUsuarioMov.ReadOnly = true;
            this._colUsuarioMov.Width = 130;
            //
            // _colUsuarioNota
            //
            this._colUsuarioNota.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._colUsuarioNota.DefaultCellStyle = styleLeft;
            this._colUsuarioNota.HeaderText = "USUARIO NOTA";
            this._colUsuarioNota.Name = "usuario_nota";
            this._colUsuarioNota.ReadOnly = true;
            //
            // _colCriadoEm
            //
            this._colCriadoEm.DefaultCellStyle = styleCenter;
            this._colCriadoEm.HeaderText = "CRIADO EM";
            this._colCriadoEm.Name = "criado_em";
            this._colCriadoEm.ReadOnly = true;
            this._colCriadoEm.Width = 120;
            //
            // AlertaEntradaLoteDivergenteForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1400, 760);
            this.Controls.Add(this._root);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1100, 600);
            this.Name = "AlertaEntradaLoteDivergenteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Alerta: Entradas com Lote Divergente";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this._root.ResumeLayout(false);
            this._root.PerformLayout();
            this._headerPanel.ResumeLayout(false);
            this._headerPanel.PerformLayout();
            this._actionsGroup.ResumeLayout(false);
            this._actionsFlow.ResumeLayout(false);
            this._actionsFlow.PerformLayout();
            this._resultsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion
    }
}
