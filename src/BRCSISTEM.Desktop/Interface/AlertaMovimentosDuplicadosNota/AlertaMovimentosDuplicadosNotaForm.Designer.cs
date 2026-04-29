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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlertaMovimentosDuplicadosNotaForm));
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
            this._infoLabel = new System.Windows.Forms.Label();
            this._nfGroup = new System.Windows.Forms.GroupBox();
            this._gridNf = new System.Windows.Forms.DataGridView();
            this.numero = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fornecedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.versao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usuario_nota_nf = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mov_ativos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qtd_propostas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ids_propostos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usuarios_mov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._detGroup = new System.Windows.Forms.GroupBox();
            this._gridDet = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.motivo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usuario_mov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usuario_nota = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.material = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.almox = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.quantidade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.data_mov = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dt_criacao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id_ref = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle());
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
            this._subtitleLabel.Size = new System.Drawing.Size(490, 13);
            this._subtitleLabel.TabIndex = 1;
            this._subtitleLabel.Text = "Diagnostico cauteloso: exibe somente IDs propostos para inativacao. Revise antes " +
    "de remover.";
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Location = new System.Drawing.Point(0, 0);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(506, 25);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Alerta: NFs com Movimentos Duplicados/Inconsistentes";
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
            this._actionsFlow.Controls.Add(this._clearButton);
            this._actionsFlow.Controls.Add(this._removeSelectedButton);
            this._actionsFlow.Controls.Add(this._removeAllButton);
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
            // _clearButton
            // 
            this._clearButton.AutoSize = true;
            this._clearButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._clearButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._clearButton.Location = new System.Drawing.Point(106, 4);
            this._clearButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(76, 23);
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
            this._removeSelectedButton.Location = new System.Drawing.Point(188, 4);
            this._removeSelectedButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._removeSelectedButton.Name = "_removeSelectedButton";
            this._removeSelectedButton.Size = new System.Drawing.Size(224, 23);
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
            this._removeAllButton.Location = new System.Drawing.Point(418, 4);
            this._removeAllButton.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this._removeAllButton.Name = "_removeAllButton";
            this._removeAllButton.Size = new System.Drawing.Size(229, 23);
            this._removeAllButton.TabIndex = 3;
            this._removeAllButton.Text = "Remover Todos os Duplicados Propostos";
            this._removeAllButton.UseVisualStyleBackColor = true;
            this._removeAllButton.Click += new System.EventHandler(this.OnRemoverTodosClick);
            // 
            // _infoLabel
            // 
            this._infoLabel.AutoSize = true;
            this._infoLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._infoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._infoLabel.Location = new System.Drawing.Point(665, 12);
            this._infoLabel.Margin = new System.Windows.Forms.Padding(12, 8, 0, 0);
            this._infoLabel.Name = "_infoLabel";
            this._infoLabel.Size = new System.Drawing.Size(0, 13);
            this._infoLabel.TabIndex = 5;
            // 
            // _nfGroup
            // 
            this._nfGroup.Controls.Add(this._gridNf);
            this._nfGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._nfGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._nfGroup.Location = new System.Drawing.Point(15, 133);
            this._nfGroup.Name = "_nfGroup";
            this._nfGroup.Size = new System.Drawing.Size(1370, 299);
            this._nfGroup.TabIndex = 2;
            this._nfGroup.TabStop = false;
            this._nfGroup.Text = "NFs com Propostas de Limpeza";
            // 
            // _gridNf
            // 
            this._gridNf.AllowUserToAddRows = false;
            this._gridNf.AllowUserToDeleteRows = false;
            this._gridNf.BackgroundColor = System.Drawing.Color.White;
            this._gridNf.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._gridNf.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.numero,
            this.fornecedor,
            this.versao,
            this.usuario_nota_nf,
            this.mov_ativos,
            this.qtd_propostas,
            this.ids_propostos,
            this.usuarios_mov});
            this._gridNf.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridNf.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._gridNf.Location = new System.Drawing.Point(3, 21);
            this._gridNf.MultiSelect = false;
            this._gridNf.Name = "_gridNf";
            this._gridNf.ReadOnly = true;
            this._gridNf.RowHeadersVisible = false;
            this._gridNf.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._gridNf.Size = new System.Drawing.Size(1364, 275);
            this._gridNf.TabIndex = 0;
            this._gridNf.SelectionChanged += new System.EventHandler(this.OnGridNfSelectionChanged);
            // 
            // numero
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.numero.DefaultCellStyle = dataGridViewCellStyle1;
            this.numero.HeaderText = "NF";
            this.numero.Name = "numero";
            this.numero.ReadOnly = true;
            this.numero.Width = 110;
            // 
            // fornecedor
            // 
            this.fornecedor.DefaultCellStyle = dataGridViewCellStyle1;
            this.fornecedor.HeaderText = "FORNECEDOR";
            this.fornecedor.Name = "fornecedor";
            this.fornecedor.ReadOnly = true;
            this.fornecedor.Width = 110;
            // 
            // versao
            // 
            this.versao.DefaultCellStyle = dataGridViewCellStyle1;
            this.versao.HeaderText = "VERSAO";
            this.versao.Name = "versao";
            this.versao.ReadOnly = true;
            this.versao.Width = 70;
            // 
            // usuario_nota_nf
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.usuario_nota_nf.DefaultCellStyle = dataGridViewCellStyle2;
            this.usuario_nota_nf.HeaderText = "USUARIO NOTA";
            this.usuario_nota_nf.Name = "usuario_nota_nf";
            this.usuario_nota_nf.ReadOnly = true;
            this.usuario_nota_nf.Width = 120;
            // 
            // mov_ativos
            // 
            this.mov_ativos.DefaultCellStyle = dataGridViewCellStyle1;
            this.mov_ativos.HeaderText = "MOV. ATIVOS";
            this.mov_ativos.Name = "mov_ativos";
            this.mov_ativos.ReadOnly = true;
            this.mov_ativos.Width = 90;
            // 
            // qtd_propostas
            // 
            this.qtd_propostas.DefaultCellStyle = dataGridViewCellStyle1;
            this.qtd_propostas.HeaderText = "QTD PROPOSTAS";
            this.qtd_propostas.Name = "qtd_propostas";
            this.qtd_propostas.ReadOnly = true;
            this.qtd_propostas.Width = 95;
            // 
            // ids_propostos
            // 
            this.ids_propostos.DefaultCellStyle = dataGridViewCellStyle2;
            this.ids_propostos.HeaderText = "IDS PROPOSTOS";
            this.ids_propostos.Name = "ids_propostos";
            this.ids_propostos.ReadOnly = true;
            this.ids_propostos.Width = 360;
            // 
            // usuarios_mov
            // 
            this.usuarios_mov.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.usuarios_mov.DefaultCellStyle = dataGridViewCellStyle2;
            this.usuarios_mov.HeaderText = "USUARIOS MOV. ATIVOS";
            this.usuarios_mov.Name = "usuarios_mov";
            this.usuarios_mov.ReadOnly = true;
            // 
            // _detGroup
            // 
            this._detGroup.Controls.Add(this._gridDet);
            this._detGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._detGroup.Location = new System.Drawing.Point(15, 438);
            this._detGroup.Name = "_detGroup";
            this._detGroup.Size = new System.Drawing.Size(1370, 367);
            this._detGroup.TabIndex = 3;
            this._detGroup.TabStop = false;
            this._detGroup.Text = "Detalhes dos IDs Propostos (NF selecionada)";
            // 
            // _gridDet
            // 
            this._gridDet.AllowUserToAddRows = false;
            this._gridDet.AllowUserToDeleteRows = false;
            this._gridDet.BackgroundColor = System.Drawing.Color.White;
            this._gridDet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._gridDet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.motivo,
            this.usuario_mov,
            this.usuario_nota,
            this.material,
            this.lote,
            this.almox,
            this.quantidade,
            this.data_mov,
            this.dt_criacao,
            this.id_ref});
            this._gridDet.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridDet.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._gridDet.Location = new System.Drawing.Point(3, 21);
            this._gridDet.MultiSelect = false;
            this._gridDet.Name = "_gridDet";
            this._gridDet.ReadOnly = true;
            this._gridDet.RowHeadersVisible = false;
            this._gridDet.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._gridDet.Size = new System.Drawing.Size(1364, 343);
            this._gridDet.TabIndex = 0;
            // 
            // id
            // 
            this.id.DefaultCellStyle = dataGridViewCellStyle1;
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Width = 70;
            // 
            // motivo
            // 
            this.motivo.DefaultCellStyle = dataGridViewCellStyle2;
            this.motivo.HeaderText = "MOTIVO";
            this.motivo.Name = "motivo";
            this.motivo.ReadOnly = true;
            this.motivo.Width = 320;
            // 
            // usuario_mov
            // 
            this.usuario_mov.DefaultCellStyle = dataGridViewCellStyle2;
            this.usuario_mov.HeaderText = "USUARIO MOVIMENTO";
            this.usuario_mov.Name = "usuario_mov";
            this.usuario_mov.ReadOnly = true;
            this.usuario_mov.Width = 130;
            // 
            // usuario_nota
            // 
            this.usuario_nota.DefaultCellStyle = dataGridViewCellStyle2;
            this.usuario_nota.HeaderText = "USUARIO NOTA";
            this.usuario_nota.Name = "usuario_nota";
            this.usuario_nota.ReadOnly = true;
            this.usuario_nota.Width = 120;
            // 
            // material
            // 
            this.material.DefaultCellStyle = dataGridViewCellStyle1;
            this.material.HeaderText = "MATERIAL";
            this.material.Name = "material";
            this.material.ReadOnly = true;
            this.material.Width = 95;
            // 
            // lote
            // 
            this.lote.DefaultCellStyle = dataGridViewCellStyle2;
            this.lote.HeaderText = "LOTE";
            this.lote.Name = "lote";
            this.lote.ReadOnly = true;
            this.lote.Width = 120;
            // 
            // almox
            // 
            this.almox.DefaultCellStyle = dataGridViewCellStyle1;
            this.almox.HeaderText = "ALMOX";
            this.almox.Name = "almox";
            this.almox.ReadOnly = true;
            this.almox.Width = 80;
            // 
            // quantidade
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.quantidade.DefaultCellStyle = dataGridViewCellStyle3;
            this.quantidade.HeaderText = "QUANTIDADE";
            this.quantidade.Name = "quantidade";
            this.quantidade.ReadOnly = true;
            // 
            // data_mov
            // 
            this.data_mov.DefaultCellStyle = dataGridViewCellStyle1;
            this.data_mov.HeaderText = "DATA MOVIMENTO";
            this.data_mov.Name = "data_mov";
            this.data_mov.ReadOnly = true;
            this.data_mov.Width = 130;
            // 
            // dt_criacao
            // 
            this.dt_criacao.DefaultCellStyle = dataGridViewCellStyle1;
            this.dt_criacao.HeaderText = "DT/HR CRIACAO";
            this.dt_criacao.Name = "dt_criacao";
            this.dt_criacao.ReadOnly = true;
            this.dt_criacao.Width = 135;
            // 
            // id_ref
            // 
            this.id_ref.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.id_ref.DefaultCellStyle = dataGridViewCellStyle1;
            this.id_ref.HeaderText = "ID REFERENCIA";
            this.id_ref.Name = "id_ref";
            this.id_ref.ReadOnly = true;
            // 
            // AlertaMovimentosDuplicadosNotaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1400, 820);
            this.Controls.Add(this._root);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1200, 680);
            this.Name = "AlertaMovimentosDuplicadosNotaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Alerta: NFs com Movimentos Duplicados";
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
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

        private DataGridViewTextBoxColumn numero;
        private DataGridViewTextBoxColumn fornecedor;
        private DataGridViewTextBoxColumn versao;
        private DataGridViewTextBoxColumn usuario_nota_nf;
        private DataGridViewTextBoxColumn mov_ativos;
        private DataGridViewTextBoxColumn qtd_propostas;
        private DataGridViewTextBoxColumn ids_propostos;
        private DataGridViewTextBoxColumn usuarios_mov;
        private DataGridViewTextBoxColumn id;
        private DataGridViewTextBoxColumn motivo;
        private DataGridViewTextBoxColumn usuario_mov;
        private DataGridViewTextBoxColumn usuario_nota;
        private DataGridViewTextBoxColumn material;
        private DataGridViewTextBoxColumn lote;
        private DataGridViewTextBoxColumn almox;
        private DataGridViewTextBoxColumn quantidade;
        private DataGridViewTextBoxColumn data_mov;
        private DataGridViewTextBoxColumn dt_criacao;
        private DataGridViewTextBoxColumn id_ref;
    }
}
