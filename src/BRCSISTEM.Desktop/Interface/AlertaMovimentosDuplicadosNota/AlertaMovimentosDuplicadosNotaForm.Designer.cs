using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.AlertaMovimentosDuplicadosNota
{
    public sealed partial class AlertaMovimentosDuplicadosNotaForm
    {
        private IContainer components = null;

        private TableLayoutPanel _root;

        private Panel _headerPanel;
        private Label _titleLabel;
        private Label _subtitleLabel;

        private GroupBox _actionsGroup;
        private FlowLayoutPanel _actionsFlow;
        private Button _consultButton;
        private Button _clearButton;
        private Button _removeSelectedButton;
        private Button _removeAllButton;
        private Button _closeButton;
        private Label _infoLabel;

        private GroupBox _nfGroup;
        private DataGridView _gridNf;
        private DataGridViewTextBoxColumn _colNumero;
        private DataGridViewTextBoxColumn _colFornecedor;
        private DataGridViewTextBoxColumn _colVersao;
        private DataGridViewTextBoxColumn _colUsuarioNotaNf;
        private DataGridViewTextBoxColumn _colMovAtivos;
        private DataGridViewTextBoxColumn _colQtdPropostas;
        private DataGridViewTextBoxColumn _colIdsPropostos;
        private DataGridViewTextBoxColumn _colUsuariosMov;

        private GroupBox _detGroup;
        private DataGridView _gridDet;
        private DataGridViewTextBoxColumn _colDetId;
        private DataGridViewTextBoxColumn _colDetMotivo;
        private DataGridViewTextBoxColumn _colDetUsuarioMov;
        private DataGridViewTextBoxColumn _colDetUsuarioNota;
        private DataGridViewTextBoxColumn _colDetMaterial;
        private DataGridViewTextBoxColumn _colDetLote;
        private DataGridViewTextBoxColumn _colDetAlmox;
        private DataGridViewTextBoxColumn _colDetQuantidade;
        private DataGridViewTextBoxColumn _colDetDataMov;
        private DataGridViewTextBoxColumn _colDetDtCriacao;
        private DataGridViewTextBoxColumn _colDetIdRef;

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
            System.Windows.Forms.DataGridViewCellStyle styleCenter = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle styleRight = new System.Windows.Forms.DataGridViewCellStyle();
            this._root = new System.Windows.Forms.TableLayoutPanel();
            this._headerPanel = new System.Windows.Forms.Panel();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this._titleLabel = new System.Windows.Forms.Label();
            this._actionsGroup = new System.Windows.Forms.GroupBox();
            this._actionsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._consultButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this._removeSelectedButton = new System.Windows.Forms.Button();
            this._removeAllButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this._infoLabel = new System.Windows.Forms.Label();
            this._nfGroup = new System.Windows.Forms.GroupBox();
            this._gridNf = new System.Windows.Forms.DataGridView();
            this._colNumero = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colFornecedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colVersao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colUsuarioNotaNf = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colMovAtivos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colQtdPropostas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colIdsPropostos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colUsuariosMov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._detGroup = new System.Windows.Forms.GroupBox();
            this._gridDet = new System.Windows.Forms.DataGridView();
            this._colDetId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetMotivo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetUsuarioMov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetUsuarioNota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetMaterial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetLote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetAlmox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetQuantidade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetDataMov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetDtCriacao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDetIdRef = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._root.SuspendLayout();
            this._headerPanel.SuspendLayout();
            this._actionsGroup.SuspendLayout();
            this._actionsFlow.SuspendLayout();
            this._nfGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridNf)).BeginInit();
            this._detGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridDet)).BeginInit();
            this.SuspendLayout();
            //
            // estilos compartilhados de celula
            //
            styleLeft.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            styleCenter.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            styleRight.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            //
            // _root
            //
            this._root.ColumnCount = 1;
            this._root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.Controls.Add(this._headerPanel, 0, 0);
            this._root.Controls.Add(this._actionsGroup, 0, 1);
            this._root.Controls.Add(this._nfGroup, 0, 2);
            this._root.Controls.Add(this._detGroup, 0, 3);
            this._root.Dock = System.Windows.Forms.DockStyle.Fill;
            this._root.Location = new System.Drawing.Point(0, 0);
            this._root.Name = "_root";
            this._root.Padding = new System.Windows.Forms.Padding(12);
            this._root.RowCount = 4;
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this._root.Size = new System.Drawing.Size(1400, 820);
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
            this._subtitleLabel.Text = "Diagnostico cauteloso: exibe somente IDs propostos para inativacao. Revise antes de remover.";
            //
            // _titleLabel
            //
            this._titleLabel.AutoSize = true;
            this._titleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Alerta: NFs com Movimentos Duplicados/Inconsistentes";
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
            this._actionsFlow.Controls.Add(this._clearButton);
            this._actionsFlow.Controls.Add(this._removeSelectedButton);
            this._actionsFlow.Controls.Add(this._removeAllButton);
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
            // _clearButton
            //
            this._clearButton.AutoSize = true;
            this._clearButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._clearButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._clearButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._clearButton.Name = "_clearButton";
            this._clearButton.TabIndex = 1;
            this._clearButton.Text = "Limpar (F6)";
            this._clearButton.UseVisualStyleBackColor = true;
            this._clearButton.Click += new System.EventHandler(this.OnLimparClick);
            //
            // _removeSelectedButton
            //
            this._removeSelectedButton.AutoSize = true;
            this._removeSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._removeSelectedButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._removeSelectedButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._removeSelectedButton.Name = "_removeSelectedButton";
            this._removeSelectedButton.TabIndex = 2;
            this._removeSelectedButton.Text = "Remover Duplicados da NF Selecionada";
            this._removeSelectedButton.UseVisualStyleBackColor = true;
            this._removeSelectedButton.Click += new System.EventHandler(this.OnRemoverSelecionadaClick);
            //
            // _removeAllButton
            //
            this._removeAllButton.AutoSize = true;
            this._removeAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._removeAllButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._removeAllButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._removeAllButton.Name = "_removeAllButton";
            this._removeAllButton.TabIndex = 3;
            this._removeAllButton.Text = "Remover Todos os Duplicados Propostos";
            this._removeAllButton.UseVisualStyleBackColor = true;
            this._removeAllButton.Click += new System.EventHandler(this.OnRemoverTodosClick);
            //
            // _closeButton
            //
            this._closeButton.AutoSize = true;
            this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._closeButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._closeButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._closeButton.Name = "_closeButton";
            this._closeButton.TabIndex = 4;
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
            this._infoLabel.TabIndex = 5;
            this._infoLabel.Text = "";
            //
            // _nfGroup
            //
            this._nfGroup.Controls.Add(this._gridNf);
            this._nfGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._nfGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._nfGroup.Name = "_nfGroup";
            this._nfGroup.TabIndex = 2;
            this._nfGroup.TabStop = false;
            this._nfGroup.Text = "NFs com Propostas de Limpeza";
            //
            // _gridNf
            //
            this._gridNf.AllowUserToAddRows = false;
            this._gridNf.AllowUserToDeleteRows = false;
            this._gridNf.AutoGenerateColumns = false;
            this._gridNf.BackgroundColor = System.Drawing.Color.White;
            this._gridNf.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._gridNf.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this._colNumero,
                this._colFornecedor,
                this._colVersao,
                this._colUsuarioNotaNf,
                this._colMovAtivos,
                this._colQtdPropostas,
                this._colIdsPropostos,
                this._colUsuariosMov});
            this._gridNf.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridNf.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._gridNf.MultiSelect = false;
            this._gridNf.Name = "_gridNf";
            this._gridNf.ReadOnly = true;
            this._gridNf.RowHeadersVisible = false;
            this._gridNf.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._gridNf.TabIndex = 0;
            this._gridNf.SelectionChanged += new System.EventHandler(this.OnGridNfSelectionChanged);
            //
            // _colNumero
            //
            this._colNumero.DefaultCellStyle = styleCenter;
            this._colNumero.HeaderText = "NF";
            this._colNumero.Name = "numero";
            this._colNumero.ReadOnly = true;
            this._colNumero.Width = 110;
            //
            // _colFornecedor
            //
            this._colFornecedor.DefaultCellStyle = styleCenter;
            this._colFornecedor.HeaderText = "FORNECEDOR";
            this._colFornecedor.Name = "fornecedor";
            this._colFornecedor.ReadOnly = true;
            this._colFornecedor.Width = 110;
            //
            // _colVersao
            //
            this._colVersao.DefaultCellStyle = styleCenter;
            this._colVersao.HeaderText = "VERSAO";
            this._colVersao.Name = "versao";
            this._colVersao.ReadOnly = true;
            this._colVersao.Width = 70;
            //
            // _colUsuarioNotaNf
            //
            this._colUsuarioNotaNf.DefaultCellStyle = styleLeft;
            this._colUsuarioNotaNf.HeaderText = "USUARIO NOTA";
            this._colUsuarioNotaNf.Name = "usuario_nota";
            this._colUsuarioNotaNf.ReadOnly = true;
            this._colUsuarioNotaNf.Width = 120;
            //
            // _colMovAtivos
            //
            this._colMovAtivos.DefaultCellStyle = styleCenter;
            this._colMovAtivos.HeaderText = "MOV. ATIVOS";
            this._colMovAtivos.Name = "mov_ativos";
            this._colMovAtivos.ReadOnly = true;
            this._colMovAtivos.Width = 90;
            //
            // _colQtdPropostas
            //
            this._colQtdPropostas.DefaultCellStyle = styleCenter;
            this._colQtdPropostas.HeaderText = "QTD PROPOSTAS";
            this._colQtdPropostas.Name = "qtd_propostas";
            this._colQtdPropostas.ReadOnly = true;
            this._colQtdPropostas.Width = 95;
            //
            // _colIdsPropostos
            //
            this._colIdsPropostos.DefaultCellStyle = styleLeft;
            this._colIdsPropostos.HeaderText = "IDS PROPOSTOS";
            this._colIdsPropostos.Name = "ids_propostos";
            this._colIdsPropostos.ReadOnly = true;
            this._colIdsPropostos.Width = 360;
            //
            // _colUsuariosMov
            //
            this._colUsuariosMov.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._colUsuariosMov.DefaultCellStyle = styleLeft;
            this._colUsuariosMov.HeaderText = "USUARIOS MOV. ATIVOS";
            this._colUsuariosMov.Name = "usuarios_mov";
            this._colUsuariosMov.ReadOnly = true;
            //
            // _detGroup
            //
            this._detGroup.Controls.Add(this._gridDet);
            this._detGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._detGroup.Name = "_detGroup";
            this._detGroup.TabIndex = 3;
            this._detGroup.TabStop = false;
            this._detGroup.Text = "Detalhes dos IDs Propostos (NF selecionada)";
            //
            // _gridDet
            //
            this._gridDet.AllowUserToAddRows = false;
            this._gridDet.AllowUserToDeleteRows = false;
            this._gridDet.AutoGenerateColumns = false;
            this._gridDet.BackgroundColor = System.Drawing.Color.White;
            this._gridDet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._gridDet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this._colDetId,
                this._colDetMotivo,
                this._colDetUsuarioMov,
                this._colDetUsuarioNota,
                this._colDetMaterial,
                this._colDetLote,
                this._colDetAlmox,
                this._colDetQuantidade,
                this._colDetDataMov,
                this._colDetDtCriacao,
                this._colDetIdRef});
            this._gridDet.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridDet.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._gridDet.MultiSelect = false;
            this._gridDet.Name = "_gridDet";
            this._gridDet.ReadOnly = true;
            this._gridDet.RowHeadersVisible = false;
            this._gridDet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._gridDet.TabIndex = 0;
            //
            // _colDetId
            //
            this._colDetId.DefaultCellStyle = styleCenter;
            this._colDetId.HeaderText = "ID";
            this._colDetId.Name = "id";
            this._colDetId.ReadOnly = true;
            this._colDetId.Width = 70;
            //
            // _colDetMotivo
            //
            this._colDetMotivo.DefaultCellStyle = styleLeft;
            this._colDetMotivo.HeaderText = "MOTIVO";
            this._colDetMotivo.Name = "motivo";
            this._colDetMotivo.ReadOnly = true;
            this._colDetMotivo.Width = 320;
            //
            // _colDetUsuarioMov
            //
            this._colDetUsuarioMov.DefaultCellStyle = styleLeft;
            this._colDetUsuarioMov.HeaderText = "USUARIO MOVIMENTO";
            this._colDetUsuarioMov.Name = "usuario_mov";
            this._colDetUsuarioMov.ReadOnly = true;
            this._colDetUsuarioMov.Width = 130;
            //
            // _colDetUsuarioNota
            //
            this._colDetUsuarioNota.DefaultCellStyle = styleLeft;
            this._colDetUsuarioNota.HeaderText = "USUARIO NOTA";
            this._colDetUsuarioNota.Name = "usuario_nota";
            this._colDetUsuarioNota.ReadOnly = true;
            this._colDetUsuarioNota.Width = 120;
            //
            // _colDetMaterial
            //
            this._colDetMaterial.DefaultCellStyle = styleCenter;
            this._colDetMaterial.HeaderText = "MATERIAL";
            this._colDetMaterial.Name = "material";
            this._colDetMaterial.ReadOnly = true;
            this._colDetMaterial.Width = 95;
            //
            // _colDetLote
            //
            this._colDetLote.DefaultCellStyle = styleLeft;
            this._colDetLote.HeaderText = "LOTE";
            this._colDetLote.Name = "lote";
            this._colDetLote.ReadOnly = true;
            this._colDetLote.Width = 120;
            //
            // _colDetAlmox
            //
            this._colDetAlmox.DefaultCellStyle = styleCenter;
            this._colDetAlmox.HeaderText = "ALMOX";
            this._colDetAlmox.Name = "almox";
            this._colDetAlmox.ReadOnly = true;
            this._colDetAlmox.Width = 80;
            //
            // _colDetQuantidade
            //
            this._colDetQuantidade.DefaultCellStyle = styleRight;
            this._colDetQuantidade.HeaderText = "QUANTIDADE";
            this._colDetQuantidade.Name = "quantidade";
            this._colDetQuantidade.ReadOnly = true;
            this._colDetQuantidade.Width = 100;
            //
            // _colDetDataMov
            //
            this._colDetDataMov.DefaultCellStyle = styleCenter;
            this._colDetDataMov.HeaderText = "DATA MOVIMENTO";
            this._colDetDataMov.Name = "data_mov";
            this._colDetDataMov.ReadOnly = true;
            this._colDetDataMov.Width = 130;
            //
            // _colDetDtCriacao
            //
            this._colDetDtCriacao.DefaultCellStyle = styleCenter;
            this._colDetDtCriacao.HeaderText = "DT/HR CRIACAO";
            this._colDetDtCriacao.Name = "dt_criacao";
            this._colDetDtCriacao.ReadOnly = true;
            this._colDetDtCriacao.Width = 135;
            //
            // _colDetIdRef
            //
            this._colDetIdRef.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._colDetIdRef.DefaultCellStyle = styleCenter;
            this._colDetIdRef.HeaderText = "ID REFERENCIA";
            this._colDetIdRef.Name = "id_ref";
            this._colDetIdRef.ReadOnly = true;
            //
            // AlertaMovimentosDuplicadosNotaForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1400, 820);
            this.Controls.Add(this._root);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1200, 680);
            this.Name = "AlertaMovimentosDuplicadosNotaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Alerta: NFs com Movimentos Duplicados";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this._root.ResumeLayout(false);
            this._root.PerformLayout();
            this._headerPanel.ResumeLayout(false);
            this._headerPanel.PerformLayout();
            this._actionsGroup.ResumeLayout(false);
            this._actionsFlow.ResumeLayout(false);
            this._actionsFlow.PerformLayout();
            this._nfGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._gridNf)).EndInit();
            this._detGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._gridDet)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion
    }
}
