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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertaEntradaLoteDivergenteForm));
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
            this.documento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.material = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lote_movimento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lote_nota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.almoxarifado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fornecedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qtd_mov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qtd_nota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usuario_mov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usuario_nota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.criado_em = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle());
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
            this._headerPanel.Location = new System.Drawing.Point(15, 15);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this._headerPanel.Size = new System.Drawing.Size(1370, 44);
            this._headerPanel.TabIndex = 0;
            // 
            // _subtitleLabel
            // 
            this._subtitleLabel.AutoSize = true;
            this._subtitleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this._subtitleLabel.Location = new System.Drawing.Point(0, 25);
            this._subtitleLabel.Name = "_subtitleLabel";
            this._subtitleLabel.Size = new System.Drawing.Size(452, 13);
            this._subtitleLabel.TabIndex = 1;
            this._subtitleLabel.Text = "Movimentos de entrada (NOTA) cujo lote registrado nao consta nos itens da nota fi" +
    "scal";
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Location = new System.Drawing.Point(0, 0);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(343, 25);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Alerta: Entradas com Lote Divergente";
            // 
            // _actionsGroup
            // 
            this._actionsGroup.Controls.Add(this._actionsFlow);
            this._actionsGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._actionsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._actionsGroup.Location = new System.Drawing.Point(15, 65);
            this._actionsGroup.Name = "_actionsGroup";
            this._actionsGroup.Size = new System.Drawing.Size(1370, 62);
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
            this._actionsFlow.Location = new System.Drawing.Point(3, 21);
            this._actionsFlow.Name = "_actionsFlow";
            this._actionsFlow.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this._actionsFlow.Size = new System.Drawing.Size(1364, 38);
            this._actionsFlow.TabIndex = 0;
            this._actionsFlow.WrapContents = false;
            // 
            // _consultButton
            // 
            this._consultButton.AutoSize = true;
            this._consultButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._consultButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._consultButton.Location = new System.Drawing.Point(8, 4);
            this._consultButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._consultButton.Name = "_consultButton";
            this._consultButton.Size = new System.Drawing.Size(92, 23);
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
            this._fixButton.Location = new System.Drawing.Point(106, 4);
            this._fixButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._fixButton.Name = "_fixButton";
            this._fixButton.Size = new System.Drawing.Size(146, 23);
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
            this._closeButton.Location = new System.Drawing.Point(258, 4);
            this._closeButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(76, 23);
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
            this._infoLabel.Location = new System.Drawing.Point(352, 12);
            this._infoLabel.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this._infoLabel.Name = "_infoLabel";
            this._infoLabel.Size = new System.Drawing.Size(0, 13);
            this._infoLabel.TabIndex = 3;
            // 
            // _resultsGroup
            // 
            this._resultsGroup.Controls.Add(this._grid);
            this._resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._resultsGroup.Location = new System.Drawing.Point(15, 133);
            this._resultsGroup.Name = "_resultsGroup";
            this._resultsGroup.Size = new System.Drawing.Size(1370, 612);
            this._resultsGroup.TabIndex = 2;
            this._resultsGroup.TabStop = false;
            this._resultsGroup.Text = "Resultados";
            // 
            // _grid
            // 
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.documento,
            this.material,
            this.lote_movimento,
            this.lote_nota,
            this.almoxarifado,
            this.fornecedor,
            this.qtd_mov,
            this.qtd_nota,
            this.usuario_mov,
            this.usuario_nota,
            this.criado_em});
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._grid.Location = new System.Drawing.Point(3, 21);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.Size = new System.Drawing.Size(1364, 588);
            this._grid.TabIndex = 0;
            this._grid.SelectionChanged += new System.EventHandler(this.OnGridSelectionChanged);
            // 
            // documento
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.documento.DefaultCellStyle = dataGridViewCellStyle1;
            this.documento.HeaderText = "DOCUMENTO";
            this.documento.Name = "documento";
            this.documento.ReadOnly = true;
            this.documento.Width = 130;
            // 
            // material
            // 
            this.material.DefaultCellStyle = dataGridViewCellStyle1;
            this.material.HeaderText = "MATERIAL";
            this.material.Name = "material";
            this.material.ReadOnly = true;
            this.material.Width = 230;
            // 
            // lote_movimento
            // 
            this.lote_movimento.DefaultCellStyle = dataGridViewCellStyle1;
            this.lote_movimento.HeaderText = "LOTE MOVIMENTO";
            this.lote_movimento.Name = "lote_movimento";
            this.lote_movimento.ReadOnly = true;
            this.lote_movimento.Width = 150;
            // 
            // lote_nota
            // 
            this.lote_nota.DefaultCellStyle = dataGridViewCellStyle1;
            this.lote_nota.HeaderText = "LOTE NOTA FISCAL";
            this.lote_nota.Name = "lote_nota";
            this.lote_nota.ReadOnly = true;
            this.lote_nota.Width = 150;
            // 
            // almoxarifado
            // 
            this.almoxarifado.DefaultCellStyle = dataGridViewCellStyle1;
            this.almoxarifado.HeaderText = "ALMOXARIFADO";
            this.almoxarifado.Name = "almoxarifado";
            this.almoxarifado.ReadOnly = true;
            this.almoxarifado.Width = 140;
            // 
            // fornecedor
            // 
            this.fornecedor.DefaultCellStyle = dataGridViewCellStyle1;
            this.fornecedor.HeaderText = "FORNECEDOR";
            this.fornecedor.Name = "fornecedor";
            this.fornecedor.ReadOnly = true;
            this.fornecedor.Width = 140;
            // 
            // qtd_mov
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.qtd_mov.DefaultCellStyle = dataGridViewCellStyle2;
            this.qtd_mov.HeaderText = "QTD. MOV.";
            this.qtd_mov.Name = "qtd_mov";
            this.qtd_mov.ReadOnly = true;
            this.qtd_mov.Width = 85;
            // 
            // qtd_nota
            // 
            this.qtd_nota.DefaultCellStyle = dataGridViewCellStyle2;
            this.qtd_nota.HeaderText = "QTD. NOTA";
            this.qtd_nota.Name = "qtd_nota";
            this.qtd_nota.ReadOnly = true;
            this.qtd_nota.Width = 85;
            // 
            // usuario_mov
            // 
            this.usuario_mov.DefaultCellStyle = dataGridViewCellStyle1;
            this.usuario_mov.HeaderText = "USUARIO MOV.";
            this.usuario_mov.Name = "usuario_mov";
            this.usuario_mov.ReadOnly = true;
            this.usuario_mov.Width = 130;
            // 
            // usuario_nota
            // 
            this.usuario_nota.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.usuario_nota.DefaultCellStyle = dataGridViewCellStyle1;
            this.usuario_nota.HeaderText = "USUARIO NOTA";
            this.usuario_nota.Name = "usuario_nota";
            this.usuario_nota.ReadOnly = true;
            // 
            // criado_em
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.criado_em.DefaultCellStyle = dataGridViewCellStyle3;
            this.criado_em.HeaderText = "CRIADO EM";
            this.criado_em.Name = "criado_em";
            this.criado_em.ReadOnly = true;
            this.criado_em.Width = 120;
            // 
            // AlertaEntradaLoteDivergenteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1400, 760);
            this.Controls.Add(this._root);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1100, 600);
            this.Name = "AlertaEntradaLoteDivergenteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Alerta: Entradas com Lote Divergente";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
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

        private DataGridViewTextBoxColumn documento;
        private DataGridViewTextBoxColumn material;
        private DataGridViewTextBoxColumn lote_movimento;
        private DataGridViewTextBoxColumn lote_nota;
        private DataGridViewTextBoxColumn almoxarifado;
        private DataGridViewTextBoxColumn fornecedor;
        private DataGridViewTextBoxColumn qtd_mov;
        private DataGridViewTextBoxColumn qtd_nota;
        private DataGridViewTextBoxColumn usuario_mov;
        private DataGridViewTextBoxColumn usuario_nota;
        private DataGridViewTextBoxColumn criado_em;
    }
}
