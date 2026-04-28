using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.AlertaLoteDuplicado
{
    public sealed partial class AlertaLoteDuplicadoForm
    {
        private IContainer components = null;

        private TableLayoutPanel _root;

        private Panel _headerPanel;
        private Label _titleLabel;
        private Label _subtitleLabel;

        private GroupBox _filtersGroup;
        private TableLayoutPanel _filtersLayout;
        private Label _materialFieldLabel;
        private ComboBox _materialComboBox;
        private Label _lotDescriptionFieldLabel;
        private TextBox _lotDescriptionTextBox;
        private Label _lotCodeFieldLabel;
        private TextBox _lotCodeTextBox;
        private FlowLayoutPanel _buttonsFlow;
        private Button _consultButton;
        private Button _clearButton;
        private Button _closeButton;
        private Label _infoLabel;

        private GroupBox _resultsGroup;
        private DataGridView _grid;
        private DataGridViewTextBoxColumn _colMaterial;
        private DataGridViewTextBoxColumn _colDescricaoLote;
        private DataGridViewTextBoxColumn _colCodigoLote;
        private DataGridViewTextBoxColumn _colFornecedor;
        private DataGridViewTextBoxColumn _colValidade;
        private DataGridViewTextBoxColumn _colQtdDuplicados;
        private DataGridViewTextBoxColumn _colGrupoCodigos;

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
            System.Windows.Forms.DataGridViewCellStyle styleCenter = new System.Windows.Forms.DataGridViewCellStyle();
            this._root = new System.Windows.Forms.TableLayoutPanel();
            this._headerPanel = new System.Windows.Forms.Panel();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this._titleLabel = new System.Windows.Forms.Label();
            this._filtersGroup = new System.Windows.Forms.GroupBox();
            this._filtersLayout = new System.Windows.Forms.TableLayoutPanel();
            this._materialFieldLabel = new System.Windows.Forms.Label();
            this._materialComboBox = new System.Windows.Forms.ComboBox();
            this._lotDescriptionFieldLabel = new System.Windows.Forms.Label();
            this._lotDescriptionTextBox = new System.Windows.Forms.TextBox();
            this._lotCodeFieldLabel = new System.Windows.Forms.Label();
            this._lotCodeTextBox = new System.Windows.Forms.TextBox();
            this._buttonsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._consultButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this._infoLabel = new System.Windows.Forms.Label();
            this._resultsGroup = new System.Windows.Forms.GroupBox();
            this._grid = new System.Windows.Forms.DataGridView();
            this._colMaterial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colDescricaoLote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colCodigoLote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colFornecedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colValidade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colQtdDuplicados = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colGrupoCodigos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._root.SuspendLayout();
            this._headerPanel.SuspendLayout();
            this._filtersGroup.SuspendLayout();
            this._filtersLayout.SuspendLayout();
            this._buttonsFlow.SuspendLayout();
            this._resultsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this.SuspendLayout();
            //
            // _root
            //
            this._root.ColumnCount = 1;
            this._root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.Controls.Add(this._headerPanel, 0, 0);
            this._root.Controls.Add(this._filtersGroup, 0, 1);
            this._root.Controls.Add(this._resultsGroup, 0, 2);
            this._root.Dock = System.Windows.Forms.DockStyle.Fill;
            this._root.Location = new System.Drawing.Point(0, 0);
            this._root.Name = "_root";
            this._root.Padding = new System.Windows.Forms.Padding(12);
            this._root.RowCount = 3;
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.Size = new System.Drawing.Size(1280, 760);
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
            this._subtitleLabel.Text = "Mostra lotes ativos com a mesma descricao para o mesmo material";
            //
            // _titleLabel
            //
            this._titleLabel.AutoSize = true;
            this._titleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Alerta: Descricao de Lote Duplicada no Mesmo Material";
            //
            // _filtersGroup
            //
            this._filtersGroup.Controls.Add(this._filtersLayout);
            this._filtersGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._filtersGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._filtersGroup.Height = 92;
            this._filtersGroup.Name = "_filtersGroup";
            this._filtersGroup.TabIndex = 1;
            this._filtersGroup.TabStop = false;
            this._filtersGroup.Text = "Filtros";
            //
            // _filtersLayout
            //
            this._filtersLayout.ColumnCount = 8;
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.Controls.Add(this._materialFieldLabel, 0, 0);
            this._filtersLayout.Controls.Add(this._materialComboBox, 1, 0);
            this._filtersLayout.Controls.Add(this._lotDescriptionFieldLabel, 2, 0);
            this._filtersLayout.Controls.Add(this._lotDescriptionTextBox, 3, 0);
            this._filtersLayout.Controls.Add(this._lotCodeFieldLabel, 4, 0);
            this._filtersLayout.Controls.Add(this._lotCodeTextBox, 5, 0);
            this._filtersLayout.Controls.Add(this._buttonsFlow, 0, 1);
            this._filtersLayout.SetColumnSpan(this._buttonsFlow, 8);
            this._filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filtersLayout.Name = "_filtersLayout";
            this._filtersLayout.Padding = new System.Windows.Forms.Padding(8);
            this._filtersLayout.RowCount = 2;
            this._filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.TabIndex = 0;
            //
            // _materialFieldLabel
            //
            this._materialFieldLabel.AutoSize = true;
            this._materialFieldLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._materialFieldLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._materialFieldLabel.Margin = new System.Windows.Forms.Padding(0, 7, 4, 0);
            this._materialFieldLabel.Name = "_materialFieldLabel";
            this._materialFieldLabel.TabIndex = 0;
            this._materialFieldLabel.Text = "Material:";
            //
            // _materialComboBox
            //
            this._materialComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._materialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._materialComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._materialComboBox.Name = "_materialComboBox";
            this._materialComboBox.TabIndex = 1;
            //
            // _lotDescriptionFieldLabel
            //
            this._lotDescriptionFieldLabel.AutoSize = true;
            this._lotDescriptionFieldLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._lotDescriptionFieldLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._lotDescriptionFieldLabel.Margin = new System.Windows.Forms.Padding(0, 7, 4, 0);
            this._lotDescriptionFieldLabel.Name = "_lotDescriptionFieldLabel";
            this._lotDescriptionFieldLabel.TabIndex = 2;
            this._lotDescriptionFieldLabel.Text = "Descricao do Lote:";
            //
            // _lotDescriptionTextBox
            //
            this._lotDescriptionTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._lotDescriptionTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._lotDescriptionTextBox.Name = "_lotDescriptionTextBox";
            this._lotDescriptionTextBox.TabIndex = 3;
            this._lotDescriptionTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFilterTextBoxKeyDown);
            //
            // _lotCodeFieldLabel
            //
            this._lotCodeFieldLabel.AutoSize = true;
            this._lotCodeFieldLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._lotCodeFieldLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._lotCodeFieldLabel.Margin = new System.Windows.Forms.Padding(0, 7, 4, 0);
            this._lotCodeFieldLabel.Name = "_lotCodeFieldLabel";
            this._lotCodeFieldLabel.TabIndex = 4;
            this._lotCodeFieldLabel.Text = "Codigo do Lote:";
            //
            // _lotCodeTextBox
            //
            this._lotCodeTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._lotCodeTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._lotCodeTextBox.Name = "_lotCodeTextBox";
            this._lotCodeTextBox.TabIndex = 5;
            this._lotCodeTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFilterTextBoxKeyDown);
            //
            // _buttonsFlow
            //
            this._buttonsFlow.AutoSize = true;
            this._buttonsFlow.Controls.Add(this._consultButton);
            this._buttonsFlow.Controls.Add(this._clearButton);
            this._buttonsFlow.Controls.Add(this._closeButton);
            this._buttonsFlow.Controls.Add(this._infoLabel);
            this._buttonsFlow.Name = "_buttonsFlow";
            this._buttonsFlow.TabIndex = 6;
            this._buttonsFlow.WrapContents = false;
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
            this._infoLabel.Margin = new System.Windows.Forms.Padding(12, 6, 0, 0);
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
                this._colMaterial,
                this._colDescricaoLote,
                this._colCodigoLote,
                this._colFornecedor,
                this._colValidade,
                this._colQtdDuplicados,
                this._colGrupoCodigos});
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.TabIndex = 0;
            //
            // estilo de celula centralizado (para QTD DUPLICADOS)
            //
            styleCenter.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            //
            // _colMaterial
            //
            this._colMaterial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._colMaterial.HeaderText = "MATERIAL";
            this._colMaterial.Name = "material";
            this._colMaterial.ReadOnly = true;
            this._colMaterial.Width = 240;
            //
            // _colDescricaoLote
            //
            this._colDescricaoLote.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._colDescricaoLote.HeaderText = "DESCRICAO DO LOTE";
            this._colDescricaoLote.Name = "descricao_lote";
            this._colDescricaoLote.ReadOnly = true;
            this._colDescricaoLote.Width = 220;
            //
            // _colCodigoLote
            //
            this._colCodigoLote.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._colCodigoLote.HeaderText = "CODIGO DO LOTE";
            this._colCodigoLote.Name = "codigo_lote";
            this._colCodigoLote.ReadOnly = true;
            this._colCodigoLote.Width = 110;
            //
            // _colFornecedor
            //
            this._colFornecedor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._colFornecedor.HeaderText = "FORNECEDOR";
            this._colFornecedor.Name = "fornecedor";
            this._colFornecedor.ReadOnly = true;
            this._colFornecedor.Width = 220;
            //
            // _colValidade
            //
            this._colValidade.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._colValidade.HeaderText = "VALIDADE";
            this._colValidade.Name = "validade";
            this._colValidade.ReadOnly = true;
            this._colValidade.Width = 100;
            //
            // _colQtdDuplicados
            //
            this._colQtdDuplicados.DefaultCellStyle = styleCenter;
            this._colQtdDuplicados.HeaderText = "QTD DUPLICADOS";
            this._colQtdDuplicados.Name = "qtd_duplicados";
            this._colQtdDuplicados.ReadOnly = true;
            this._colQtdDuplicados.Width = 110;
            //
            // _colGrupoCodigos
            //
            this._colGrupoCodigos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._colGrupoCodigos.HeaderText = "LOTES DO GRUPO";
            this._colGrupoCodigos.Name = "grupo_codigos";
            this._colGrupoCodigos.ReadOnly = true;
            //
            // AlertaLoteDuplicadoForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1280, 760);
            this.Controls.Add(this._root);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1100, 600);
            this.Name = "AlertaLoteDuplicadoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Alerta: Descricao de Lote Duplicada por Material";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this._root.ResumeLayout(false);
            this._root.PerformLayout();
            this._headerPanel.ResumeLayout(false);
            this._headerPanel.PerformLayout();
            this._filtersGroup.ResumeLayout(false);
            this._filtersLayout.ResumeLayout(false);
            this._filtersLayout.PerformLayout();
            this._buttonsFlow.ResumeLayout(false);
            this._buttonsFlow.PerformLayout();
            this._resultsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion
    }
}
