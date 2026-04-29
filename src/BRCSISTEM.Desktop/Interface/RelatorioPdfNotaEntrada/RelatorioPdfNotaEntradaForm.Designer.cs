using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.RelatorioPdfNotaEntrada
{
    public sealed partial class RelatorioPdfNotaEntradaForm
    {
        private IContainer components = null;

        private TableLayoutPanel _root;

        private GroupBox _filtersGroup;
        private TableLayoutPanel _filtersLayout;
        private Label _titleLabel;
        private FlowLayoutPanel _line1;
        private Label _startDateFieldLabel;
        private TextBox _startDateTextBox;
        private Label _endDateFieldLabel;
        private TextBox _endDateTextBox;
        private Label _receiptNumberFieldLabel;
        private TextBox _receiptNumberTextBox;
        private FlowLayoutPanel _line2;
        private Label _supplierFieldLabel;
        private ComboBox _supplierComboBox;
        private Button _searchSupplierButton;
        private CheckBox _excludeCanceledCheckBox;
        private Button _filterButton;
        private Button _clearButton;
        private Button _generatePdfButton;
        private Button _generateCsvButton;
        private Button _closeButton;

        private GroupBox _gridGroup;
        private DataGridView _grid;
        private DataGridViewTextBoxColumn _colDocumento;
        private DataGridViewTextBoxColumn _colFornecedor;
        private DataGridViewTextBoxColumn _colCodigo;
        private DataGridViewTextBoxColumn _colMaterial;
        private DataGridViewTextBoxColumn _colQuantidade;
        private DataGridViewTextBoxColumn _colLote;
        private DataGridViewTextBoxColumn _colData;
        private DataGridViewTextBoxColumn _colStatus;

        private TableLayoutPanel _footer;
        private Label _summaryLabel;
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
            System.Windows.Forms.DataGridViewCellStyle styleLeft = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle styleCenter = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle styleRight = new System.Windows.Forms.DataGridViewCellStyle();
            this._root = new System.Windows.Forms.TableLayoutPanel();
            this._filtersGroup = new System.Windows.Forms.GroupBox();
            this._filtersLayout = new System.Windows.Forms.TableLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._line1 = new System.Windows.Forms.FlowLayoutPanel();
            this._startDateFieldLabel = new System.Windows.Forms.Label();
            this._startDateTextBox = new System.Windows.Forms.TextBox();
            this._endDateFieldLabel = new System.Windows.Forms.Label();
            this._endDateTextBox = new System.Windows.Forms.TextBox();
            this._receiptNumberFieldLabel = new System.Windows.Forms.Label();
            this._receiptNumberTextBox = new System.Windows.Forms.TextBox();
            this._line2 = new System.Windows.Forms.FlowLayoutPanel();
            this._supplierFieldLabel = new System.Windows.Forms.Label();
            this._supplierComboBox = new System.Windows.Forms.ComboBox();
            this._searchSupplierButton = new System.Windows.Forms.Button();
            this._excludeCanceledCheckBox = new System.Windows.Forms.CheckBox();
            this._filterButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this._generatePdfButton = new System.Windows.Forms.Button();
            this._generateCsvButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this._gridGroup = new System.Windows.Forms.GroupBox();
            this._grid = new System.Windows.Forms.DataGridView();
            this._colDocumento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colFornecedor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colMaterial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colQuantidade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colLote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._footer = new System.Windows.Forms.TableLayoutPanel();
            this._summaryLabel = new System.Windows.Forms.Label();
            this._statusLabel = new System.Windows.Forms.Label();
            this._root.SuspendLayout();
            this._filtersGroup.SuspendLayout();
            this._filtersLayout.SuspendLayout();
            this._line1.SuspendLayout();
            this._line2.SuspendLayout();
            this._gridGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._grid)).BeginInit();
            this._footer.SuspendLayout();
            this.SuspendLayout();
            //
            // estilos compartilhados
            //
            styleLeft.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            styleCenter.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            styleRight.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            //
            // _root
            //
            this._root.ColumnCount = 1;
            this._root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.Controls.Add(this._filtersGroup, 0, 0);
            this._root.Controls.Add(this._gridGroup, 0, 1);
            this._root.Controls.Add(this._footer, 0, 2);
            this._root.Dock = System.Windows.Forms.DockStyle.Fill;
            this._root.Location = new System.Drawing.Point(0, 0);
            this._root.Name = "_root";
            this._root.Padding = new System.Windows.Forms.Padding(12);
            this._root.RowCount = 3;
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._root.Size = new System.Drawing.Size(1320, 780);
            this._root.TabIndex = 0;
            //
            // _filtersGroup
            //
            this._filtersGroup.AutoSize = true;
            this._filtersGroup.Controls.Add(this._filtersLayout);
            this._filtersGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._filtersGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._filtersGroup.Name = "_filtersGroup";
            this._filtersGroup.TabIndex = 0;
            this._filtersGroup.TabStop = false;
            this._filtersGroup.Text = "Filtros";
            //
            // _filtersLayout
            //
            this._filtersLayout.AutoSize = true;
            this._filtersLayout.ColumnCount = 1;
            this._filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._filtersLayout.Controls.Add(this._titleLabel, 0, 0);
            this._filtersLayout.Controls.Add(this._line1, 0, 1);
            this._filtersLayout.Controls.Add(this._line2, 0, 2);
            this._filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._filtersLayout.Name = "_filtersLayout";
            this._filtersLayout.Padding = new System.Windows.Forms.Padding(10);
            this._filtersLayout.RowCount = 3;
            this._filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._filtersLayout.TabIndex = 0;
            //
            // _titleLabel
            //
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._titleLabel.ForeColor = System.Drawing.Color.DimGray;
            this._titleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Relatorio analitico de entrada nota por nota, com exportacao em PDF e CSV para auditoria.";
            //
            // _line1
            //
            this._line1.AutoSize = true;
            this._line1.Controls.Add(this._startDateFieldLabel);
            this._line1.Controls.Add(this._startDateTextBox);
            this._line1.Controls.Add(this._endDateFieldLabel);
            this._line1.Controls.Add(this._endDateTextBox);
            this._line1.Controls.Add(this._receiptNumberFieldLabel);
            this._line1.Controls.Add(this._receiptNumberTextBox);
            this._line1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._line1.Name = "_line1";
            this._line1.TabIndex = 1;
            this._line1.WrapContents = true;
            //
            // _startDateFieldLabel
            //
            this._startDateFieldLabel.AutoSize = true;
            this._startDateFieldLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._startDateFieldLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._startDateFieldLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this._startDateFieldLabel.Name = "_startDateFieldLabel";
            this._startDateFieldLabel.TabIndex = 0;
            this._startDateFieldLabel.Text = "Data Inicio:";
            //
            // _startDateTextBox
            //
            this._startDateTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._startDateTextBox.Name = "_startDateTextBox";
            this._startDateTextBox.TabIndex = 1;
            this._startDateTextBox.Width = 110;
            this._startDateTextBox.Leave += new System.EventHandler(this.OnStartDateLeave);
            //
            // _endDateFieldLabel
            //
            this._endDateFieldLabel.AutoSize = true;
            this._endDateFieldLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._endDateFieldLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._endDateFieldLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this._endDateFieldLabel.Name = "_endDateFieldLabel";
            this._endDateFieldLabel.TabIndex = 2;
            this._endDateFieldLabel.Text = "Data Fim:";
            //
            // _endDateTextBox
            //
            this._endDateTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._endDateTextBox.Name = "_endDateTextBox";
            this._endDateTextBox.TabIndex = 3;
            this._endDateTextBox.Width = 110;
            this._endDateTextBox.Leave += new System.EventHandler(this.OnEndDateLeave);
            //
            // _receiptNumberFieldLabel
            //
            this._receiptNumberFieldLabel.AutoSize = true;
            this._receiptNumberFieldLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._receiptNumberFieldLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._receiptNumberFieldLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this._receiptNumberFieldLabel.Name = "_receiptNumberFieldLabel";
            this._receiptNumberFieldLabel.TabIndex = 4;
            this._receiptNumberFieldLabel.Text = "Nota Fiscal:";
            //
            // _receiptNumberTextBox
            //
            this._receiptNumberTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._receiptNumberTextBox.Name = "_receiptNumberTextBox";
            this._receiptNumberTextBox.TabIndex = 5;
            this._receiptNumberTextBox.Width = 150;
            this._receiptNumberTextBox.TextChanged += new System.EventHandler(this.OnReceiptNumberTextChanged);
            this._receiptNumberTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnReceiptNumberKeyDown);
            //
            // _line2
            //
            this._line2.AutoSize = true;
            this._line2.Controls.Add(this._supplierFieldLabel);
            this._line2.Controls.Add(this._supplierComboBox);
            this._line2.Controls.Add(this._searchSupplierButton);
            this._line2.Controls.Add(this._excludeCanceledCheckBox);
            this._line2.Controls.Add(this._filterButton);
            this._line2.Controls.Add(this._clearButton);
            this._line2.Controls.Add(this._generatePdfButton);
            this._line2.Controls.Add(this._generateCsvButton);
            this._line2.Controls.Add(this._closeButton);
            this._line2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._line2.Name = "_line2";
            this._line2.TabIndex = 2;
            this._line2.WrapContents = true;
            //
            // _supplierFieldLabel
            //
            this._supplierFieldLabel.AutoSize = true;
            this._supplierFieldLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._supplierFieldLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._supplierFieldLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this._supplierFieldLabel.Name = "_supplierFieldLabel";
            this._supplierFieldLabel.TabIndex = 0;
            this._supplierFieldLabel.Text = "Fornecedor:";
            //
            // _supplierComboBox
            //
            this._supplierComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._supplierComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._supplierComboBox.Name = "_supplierComboBox";
            this._supplierComboBox.TabIndex = 1;
            this._supplierComboBox.Width = 360;
            //
            // _searchSupplierButton
            //
            this._searchSupplierButton.AutoSize = true;
            this._searchSupplierButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._searchSupplierButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._searchSupplierButton.Name = "_searchSupplierButton";
            this._searchSupplierButton.TabIndex = 2;
            this._searchSupplierButton.Text = "Buscar";
            this._searchSupplierButton.UseVisualStyleBackColor = true;
            this._searchSupplierButton.Click += new System.EventHandler(this.OnBuscarFornecedorClick);
            //
            // _excludeCanceledCheckBox
            //
            this._excludeCanceledCheckBox.AutoSize = true;
            this._excludeCanceledCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._excludeCanceledCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._excludeCanceledCheckBox.Margin = new System.Windows.Forms.Padding(14, 8, 0, 0);
            this._excludeCanceledCheckBox.Name = "_excludeCanceledCheckBox";
            this._excludeCanceledCheckBox.TabIndex = 3;
            this._excludeCanceledCheckBox.Text = "Excluir Canceladas";
            this._excludeCanceledCheckBox.UseVisualStyleBackColor = true;
            //
            // _filterButton
            //
            this._filterButton.AutoSize = true;
            this._filterButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._filterButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._filterButton.Name = "_filterButton";
            this._filterButton.TabIndex = 4;
            this._filterButton.Text = "Filtrar";
            this._filterButton.UseVisualStyleBackColor = true;
            this._filterButton.Click += new System.EventHandler(this.OnFiltrarClick);
            //
            // _clearButton
            //
            this._clearButton.AutoSize = true;
            this._clearButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._clearButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._clearButton.Name = "_clearButton";
            this._clearButton.TabIndex = 5;
            this._clearButton.Text = "Limpar Filtros";
            this._clearButton.UseVisualStyleBackColor = true;
            this._clearButton.Click += new System.EventHandler(this.OnLimparFiltrosClick);
            //
            // _generatePdfButton
            //
            this._generatePdfButton.AutoSize = true;
            this._generatePdfButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._generatePdfButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._generatePdfButton.Name = "_generatePdfButton";
            this._generatePdfButton.TabIndex = 6;
            this._generatePdfButton.Text = "Gerar PDF";
            this._generatePdfButton.UseVisualStyleBackColor = true;
            this._generatePdfButton.Click += new System.EventHandler(this.OnGerarPdfClick);
            //
            // _generateCsvButton
            //
            this._generateCsvButton.AutoSize = true;
            this._generateCsvButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._generateCsvButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._generateCsvButton.Name = "_generateCsvButton";
            this._generateCsvButton.TabIndex = 7;
            this._generateCsvButton.Text = "Gerar CSV";
            this._generateCsvButton.UseVisualStyleBackColor = true;
            this._generateCsvButton.Click += new System.EventHandler(this.OnGerarCsvClick);
            //
            // _closeButton
            //
            this._closeButton.AutoSize = true;
            this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._closeButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._closeButton.Name = "_closeButton";
            this._closeButton.TabIndex = 8;
            this._closeButton.Text = "Fechar";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this.OnFecharClick);
            //
            // _gridGroup
            //
            this._gridGroup.Controls.Add(this._grid);
            this._gridGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._gridGroup.Name = "_gridGroup";
            this._gridGroup.TabIndex = 1;
            this._gridGroup.TabStop = false;
            this._gridGroup.Text = "Notas Fiscais de Entrada";
            //
            // _grid
            //
            this._grid.AllowUserToAddRows = false;
            this._grid.AllowUserToDeleteRows = false;
            this._grid.AutoGenerateColumns = false;
            this._grid.BackgroundColor = System.Drawing.Color.White;
            this._grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this._colDocumento,
                this._colFornecedor,
                this._colCodigo,
                this._colMaterial,
                this._colQuantidade,
                this._colLote,
                this._colData,
                this._colStatus});
            this._grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._grid.MultiSelect = false;
            this._grid.Name = "_grid";
            this._grid.ReadOnly = true;
            this._grid.RowHeadersVisible = false;
            this._grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._grid.TabIndex = 0;
            this._grid.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.OnGridRowPrePaint);
            //
            // _colDocumento
            //
            this._colDocumento.DataPropertyName = "Number";
            this._colDocumento.DefaultCellStyle = styleLeft;
            this._colDocumento.HeaderText = "NOTA FISCAL";
            this._colDocumento.Name = "documento";
            this._colDocumento.ReadOnly = true;
            this._colDocumento.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colDocumento.Width = 110;
            //
            // _colFornecedor
            //
            this._colFornecedor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._colFornecedor.DataPropertyName = "SupplierDisplay";
            this._colFornecedor.DefaultCellStyle = styleLeft;
            this._colFornecedor.HeaderText = "FORNECEDOR";
            this._colFornecedor.Name = "fornecedor";
            this._colFornecedor.ReadOnly = true;
            this._colFornecedor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // _colCodigo
            //
            this._colCodigo.DataPropertyName = "MaterialCodeDisplay";
            this._colCodigo.DefaultCellStyle = styleCenter;
            this._colCodigo.HeaderText = "CODIGO";
            this._colCodigo.Name = "codigo";
            this._colCodigo.ReadOnly = true;
            this._colCodigo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colCodigo.Width = 90;
            //
            // _colMaterial
            //
            this._colMaterial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._colMaterial.DataPropertyName = "MaterialNameDisplay";
            this._colMaterial.DefaultCellStyle = styleLeft;
            this._colMaterial.HeaderText = "NOME DO MATERIAL";
            this._colMaterial.Name = "material";
            this._colMaterial.ReadOnly = true;
            this._colMaterial.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // _colQuantidade
            //
            this._colQuantidade.DataPropertyName = "QuantityText";
            this._colQuantidade.DefaultCellStyle = styleRight;
            this._colQuantidade.HeaderText = "QUANTIDADE";
            this._colQuantidade.Name = "quantidade";
            this._colQuantidade.ReadOnly = true;
            this._colQuantidade.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colQuantidade.Width = 110;
            //
            // _colLote
            //
            this._colLote.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._colLote.DataPropertyName = "LotDisplay";
            this._colLote.DefaultCellStyle = styleLeft;
            this._colLote.HeaderText = "LOTE";
            this._colLote.Name = "lote";
            this._colLote.ReadOnly = true;
            this._colLote.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //
            // _colData
            //
            this._colData.DataPropertyName = "ReceiptDateDisplay";
            this._colData.DefaultCellStyle = styleCenter;
            this._colData.HeaderText = "DATA ENTRADA";
            this._colData.Name = "data";
            this._colData.ReadOnly = true;
            this._colData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colData.Width = 115;
            //
            // _colStatus
            //
            this._colStatus.DataPropertyName = "Status";
            this._colStatus.DefaultCellStyle = styleCenter;
            this._colStatus.HeaderText = "STATUS";
            this._colStatus.Name = "status";
            this._colStatus.ReadOnly = true;
            this._colStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._colStatus.Width = 110;
            //
            // _footer
            //
            this._footer.AutoSize = true;
            this._footer.ColumnCount = 1;
            this._footer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._footer.Controls.Add(this._summaryLabel, 0, 0);
            this._footer.Controls.Add(this._statusLabel, 0, 1);
            this._footer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footer.Name = "_footer";
            this._footer.RowCount = 2;
            this._footer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._footer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this._footer.TabIndex = 2;
            //
            // _summaryLabel
            //
            this._summaryLabel.AutoSize = true;
            this._summaryLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._summaryLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._summaryLabel.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this._summaryLabel.Name = "_summaryLabel";
            this._summaryLabel.TabIndex = 0;
            this._summaryLabel.Text = "";
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.TabIndex = 1;
            this._statusLabel.Text = "";
            //
            // RelatorioPdfNotaEntradaForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1320, 780);
            this.Controls.Add(this._root);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1100, 680);
            this.Name = "RelatorioPdfNotaEntradaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Relatorio de Entrada - Auditoria";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this._root.ResumeLayout(false);
            this._root.PerformLayout();
            this._filtersGroup.ResumeLayout(false);
            this._filtersGroup.PerformLayout();
            this._filtersLayout.ResumeLayout(false);
            this._filtersLayout.PerformLayout();
            this._line1.ResumeLayout(false);
            this._line1.PerformLayout();
            this._line2.ResumeLayout(false);
            this._line2.PerformLayout();
            this._gridGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._grid)).EndInit();
            this._footer.ResumeLayout(false);
            this._footer.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion
    }
}
