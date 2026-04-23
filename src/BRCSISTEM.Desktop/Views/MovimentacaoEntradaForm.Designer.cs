using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class MovimentacaoEntradaForm
    {
        private IContainer components = null;

        // Root / header
        private TableLayoutPanel _rootLayout;
        private Label _titleLabel;

        // Dados da Nota
        private GroupBox _headerGroup;
        private TableLayoutPanel _headerLayout;
        private TableLayoutPanel _headerLine1;
        private TableLayoutPanel _headerLine2;

        private Label _numberLabel;
        private TextBox _numberTextBox;
        private Button _btnNumberLookup;

        private Label _supplierLabel;
        private ComboBox _supplierComboBox;
        private Button _btnSupplierRefresh;
        private Button _btnSupplierLookup;
        private Button _btnSupplierNew;

        private Label _warehouseLabel;
        private ComboBox _warehouseComboBox;
        private Button _btnWarehouseRefresh;
        private Button _btnWarehouseLookup;

        private Label _emissionDateLabel;
        private TextBox _emissionDateTextBox;
        private Label _receiptDateLabel;
        private TextBox _receiptDateTimeTextBox;

        private Label _statusLabel;

        // Adicionar / Editar Item
        private GroupBox _itemGroup;
        private TableLayoutPanel _itemLayout;

        private Label _materialLabel;
        private ComboBox _materialComboBox;
        private Button _btnMaterialRefresh;
        private Button _btnMaterialLookup;
        private Button _btnMaterialNew;

        private Label _lotLabel;
        private ComboBox _lotComboBox;
        private Button _btnLotRefresh;
        private Button _btnLotLookup;
        private Button _btnLotNew;

        private Label _quantityLabel;
        private TextBox _quantityTextBox;

        private Label _actionsLabel;
        private Button _btnItemAdd;
        private Button _btnItemEdit;
        private Button _btnItemRemove;
        private Button _btnItemClear;
        private Label _brcLabel;

        // Itens lancados
        private GroupBox _gridGroup;
        private DataGridView _itemsGrid;

        // Footer
        private TableLayoutPanel _footerLayout;
        private FlowLayoutPanel _footerLeftPanel;
        private Button _saveButton;
        private Button _updateButton;
        private Button _clearButton;
        private Button _cancelButton;
        private Label _itemCountLabel;
        private FlowLayoutPanel _footerRightPanel;
        private Button _closeButton;

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
            this.components = new Container();

            this._rootLayout = new TableLayoutPanel();
            this._titleLabel = new Label();

            this._headerGroup = new GroupBox();
            this._headerLayout = new TableLayoutPanel();
            this._headerLine1 = new TableLayoutPanel();
            this._headerLine2 = new TableLayoutPanel();

            this._numberLabel = new Label();
            this._numberTextBox = new TextBox();
            this._btnNumberLookup = new Button();

            this._supplierLabel = new Label();
            this._supplierComboBox = new ComboBox();
            this._btnSupplierRefresh = new Button();
            this._btnSupplierLookup = new Button();
            this._btnSupplierNew = new Button();

            this._warehouseLabel = new Label();
            this._warehouseComboBox = new ComboBox();
            this._btnWarehouseRefresh = new Button();
            this._btnWarehouseLookup = new Button();

            this._emissionDateLabel = new Label();
            this._emissionDateTextBox = new TextBox();
            this._receiptDateLabel = new Label();
            this._receiptDateTimeTextBox = new TextBox();

            this._statusLabel = new Label();

            this._itemGroup = new GroupBox();
            this._itemLayout = new TableLayoutPanel();

            this._materialLabel = new Label();
            this._materialComboBox = new ComboBox();
            this._btnMaterialRefresh = new Button();
            this._btnMaterialLookup = new Button();
            this._btnMaterialNew = new Button();

            this._lotLabel = new Label();
            this._lotComboBox = new ComboBox();
            this._btnLotRefresh = new Button();
            this._btnLotLookup = new Button();
            this._btnLotNew = new Button();

            this._quantityLabel = new Label();
            this._quantityTextBox = new TextBox();

            this._actionsLabel = new Label();
            this._btnItemAdd = new Button();
            this._btnItemEdit = new Button();
            this._btnItemRemove = new Button();
            this._btnItemClear = new Button();
            this._brcLabel = new Label();

            this._gridGroup = new GroupBox();
            this._itemsGrid = new DataGridView();

            this._footerLayout = new TableLayoutPanel();
            this._footerLeftPanel = new FlowLayoutPanel();
            this._saveButton = new Button();
            this._updateButton = new Button();
            this._clearButton = new Button();
            this._cancelButton = new Button();
            this._itemCountLabel = new Label();
            this._footerRightPanel = new FlowLayoutPanel();
            this._closeButton = new Button();

            this._rootLayout.SuspendLayout();
            this._headerGroup.SuspendLayout();
            this._headerLayout.SuspendLayout();
            this._headerLine1.SuspendLayout();
            this._headerLine2.SuspendLayout();
            this._itemGroup.SuspendLayout();
            this._itemLayout.SuspendLayout();
            this._gridGroup.SuspendLayout();
            ((ISupportInitialize)(this._itemsGrid)).BeginInit();
            this._footerLayout.SuspendLayout();
            this._footerLeftPanel.SuspendLayout();
            this._footerRightPanel.SuspendLayout();
            this.SuspendLayout();

            //
            // _rootLayout
            //
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Padding = new Padding(10);
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.RowCount = 5;
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130F));
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            this._rootLayout.Controls.Add(this._titleLabel, 0, 0);
            this._rootLayout.Controls.Add(this._headerGroup, 0, 1);
            this._rootLayout.Controls.Add(this._itemGroup, 0, 2);
            this._rootLayout.Controls.Add(this._gridGroup, 0, 3);
            this._rootLayout.Controls.Add(this._footerLayout, 0, 4);

            //
            // _titleLabel
            //
            this._titleLabel.Dock = DockStyle.Fill;
            this._titleLabel.Text = "Entrada de Estoque (Nota Fiscal de Material)";
            this._titleLabel.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            this._titleLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            this._titleLabel.AutoSize = false;
            this._titleLabel.Margin = new Padding(0, 0, 0, 6);

            //
            // _headerGroup
            //
            this._headerGroup.Dock = DockStyle.Fill;
            this._headerGroup.Text = "Dados da Nota";
            this._headerGroup.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this._headerGroup.Padding = new Padding(10, 6, 10, 8);
            this._headerGroup.Margin = new Padding(0, 0, 0, 10);
            this._headerGroup.Controls.Add(this._headerLayout);

            //
            // _headerLayout
            //
            this._headerLayout.Dock = DockStyle.Fill;
            this._headerLayout.ColumnCount = 1;
            this._headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._headerLayout.RowCount = 3;
            this._headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            this._headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            this._headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._headerLayout.Controls.Add(this._headerLine1, 0, 0);
            this._headerLayout.Controls.Add(this._headerLine2, 0, 1);
            this._headerLayout.Controls.Add(this._statusLabel, 0, 2);

            //
            // _headerLine1
            //
            this._headerLine1.Dock = DockStyle.Fill;
            this._headerLine1.ColumnCount = 13;
            this._headerLine1.RowCount = 1;
            this._headerLine1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 66F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 94F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._headerLine1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8F));

            this._headerLine1.Controls.Add(this._numberLabel, 0, 0);
            this._headerLine1.Controls.Add(this._numberTextBox, 1, 0);
            this._headerLine1.Controls.Add(this._btnNumberLookup, 2, 0);
            this._headerLine1.Controls.Add(this._supplierLabel, 3, 0);
            this._headerLine1.Controls.Add(this._supplierComboBox, 4, 0);
            this._headerLine1.Controls.Add(this._btnSupplierRefresh, 5, 0);
            this._headerLine1.Controls.Add(this._btnSupplierLookup, 6, 0);
            this._headerLine1.Controls.Add(this._btnSupplierNew, 7, 0);
            this._headerLine1.Controls.Add(this._warehouseLabel, 8, 0);
            this._headerLine1.Controls.Add(this._warehouseComboBox, 9, 0);
            this._headerLine1.Controls.Add(this._btnWarehouseRefresh, 10, 0);
            this._headerLine1.Controls.Add(this._btnWarehouseLookup, 11, 0);

            ConfigureFieldLabel(this._numberLabel, "No Nota:");
            ConfigureCellTextBox(this._numberTextBox);
            this._numberTextBox.TextChanged += this.OnNumberTextChanged;
            ConfigureIconButton(this._btnNumberLookup, "\uD83D\uDD0D", "Buscar nota");
            this._btnNumberLookup.Click += this.OnBtnNumberLookupClick;

            ConfigureFieldLabel(this._supplierLabel, "Fornecedor:");
            ConfigureCellCombo(this._supplierComboBox);
            this._supplierComboBox.SelectedIndexChanged += this.OnSupplierComboChanged;
            ConfigureIconButton(this._btnSupplierRefresh, "\u27F3", "Atualizar");
            this._btnSupplierRefresh.Click += this.OnBtnSupplierRefreshClick;
            ConfigureIconButton(this._btnSupplierLookup, "\uD83D\uDD0D", "Buscar");
            this._btnSupplierLookup.Click += this.OnBtnSupplierLookupClick;
            ConfigureIconButton(this._btnSupplierNew, "+", "Novo fornecedor");
            this._btnSupplierNew.Click += this.OnBtnSupplierNewClick;

            ConfigureFieldLabel(this._warehouseLabel, "Almoxarifado:");
            ConfigureCellCombo(this._warehouseComboBox);
            ConfigureIconButton(this._btnWarehouseRefresh, "\u27F3", "Atualizar");
            this._btnWarehouseRefresh.Click += this.OnBtnWarehouseRefreshClick;
            ConfigureIconButton(this._btnWarehouseLookup, "\uD83D\uDD0D", "Buscar");
            this._btnWarehouseLookup.Click += this.OnBtnWarehouseLookupClick;

            //
            // _headerLine2
            //
            this._headerLine2.Dock = DockStyle.Fill;
            this._headerLine2.ColumnCount = 5;
            this._headerLine2.RowCount = 1;
            this._headerLine2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._headerLine2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 102F));
            this._headerLine2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            this._headerLine2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 188F));
            this._headerLine2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            this._headerLine2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            this._headerLine2.Controls.Add(this._emissionDateLabel, 0, 0);
            this._headerLine2.Controls.Add(this._emissionDateTextBox, 1, 0);
            this._headerLine2.Controls.Add(this._receiptDateLabel, 2, 0);
            this._headerLine2.Controls.Add(this._receiptDateTimeTextBox, 3, 0);

            ConfigureFieldLabel(this._emissionDateLabel, "Data Emissao:");
            ConfigureCellTextBox(this._emissionDateTextBox);
            this._emissionDateTextBox.Leave += this.OnEmissionDateLeave;
            ConfigureFieldLabel(this._receiptDateLabel, "Data/Hora Recebimento:");
            this._receiptDateLabel.Margin = new Padding(15, 0, 3, 0);
            ConfigureCellTextBox(this._receiptDateTimeTextBox);
            this._receiptDateTimeTextBox.Leave += this.OnReceiptDateLeave;

            //
            // _statusLabel
            //
            this._statusLabel.Dock = DockStyle.Fill;
            this._statusLabel.AutoSize = false;
            this._statusLabel.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            this._statusLabel.ForeColor = Color.SeaGreen;
            this._statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            this._statusLabel.Margin = new Padding(3, 2, 3, 0);

            //
            // _itemGroup
            //
            this._itemGroup.Dock = DockStyle.Fill;
            this._itemGroup.Text = "Adicionar / Editar Item";
            this._itemGroup.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this._itemGroup.Padding = new Padding(10, 6, 10, 8);
            this._itemGroup.Margin = new Padding(0, 0, 0, 10);
            this._itemGroup.Controls.Add(this._itemLayout);

            //
            // _itemLayout
            //
            this._itemLayout.Dock = DockStyle.Fill;
            this._itemLayout.ColumnCount = 18;
            this._itemLayout.RowCount = 1;
            this._itemLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 86F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            this._itemLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));

            this._itemLayout.Controls.Add(this._materialLabel, 0, 0);
            this._itemLayout.Controls.Add(this._materialComboBox, 1, 0);
            this._itemLayout.Controls.Add(this._btnMaterialRefresh, 2, 0);
            this._itemLayout.Controls.Add(this._btnMaterialLookup, 3, 0);
            this._itemLayout.Controls.Add(this._btnMaterialNew, 4, 0);
            this._itemLayout.Controls.Add(this._lotLabel, 5, 0);
            this._itemLayout.Controls.Add(this._lotComboBox, 6, 0);
            this._itemLayout.Controls.Add(this._btnLotRefresh, 7, 0);
            this._itemLayout.Controls.Add(this._btnLotLookup, 8, 0);
            this._itemLayout.Controls.Add(this._btnLotNew, 9, 0);
            this._itemLayout.Controls.Add(this._quantityLabel, 10, 0);
            this._itemLayout.Controls.Add(this._quantityTextBox, 11, 0);
            this._itemLayout.Controls.Add(this._actionsLabel, 12, 0);
            this._itemLayout.Controls.Add(this._btnItemAdd, 13, 0);
            this._itemLayout.Controls.Add(this._btnItemEdit, 14, 0);
            this._itemLayout.Controls.Add(this._btnItemRemove, 15, 0);
            this._itemLayout.Controls.Add(this._btnItemClear, 16, 0);
            this._itemLayout.Controls.Add(this._brcLabel, 17, 0);

            ConfigureFieldLabel(this._materialLabel, "Material:");
            ConfigureCellCombo(this._materialComboBox);
            this._materialComboBox.SelectedIndexChanged += this.OnMaterialComboChanged;
            ConfigureIconButton(this._btnMaterialRefresh, "\u27F3", "Atualizar");
            this._btnMaterialRefresh.Click += this.OnBtnMaterialRefreshClick;
            ConfigureIconButton(this._btnMaterialLookup, "\uD83D\uDD0D", "Buscar");
            this._btnMaterialLookup.Click += this.OnBtnMaterialLookupClick;
            ConfigureIconButton(this._btnMaterialNew, "+", "Nova embalagem");
            this._btnMaterialNew.Click += this.OnBtnMaterialNewClick;

            ConfigureFieldLabel(this._lotLabel, "Lote:");
            ConfigureCellCombo(this._lotComboBox);
            ConfigureIconButton(this._btnLotRefresh, "\u27F3", "Atualizar");
            this._btnLotRefresh.Click += this.OnBtnLotRefreshClick;
            ConfigureIconButton(this._btnLotLookup, "\uD83D\uDD0D", "Buscar");
            this._btnLotLookup.Click += this.OnBtnLotLookupClick;
            ConfigureIconButton(this._btnLotNew, "+", "Novo lote");
            this._btnLotNew.Click += this.OnBtnLotNewClick;

            ConfigureFieldLabel(this._quantityLabel, "Quantidade:");
            ConfigureCellTextBox(this._quantityTextBox);
            this._quantityTextBox.TextAlign = HorizontalAlignment.Right;

            this._actionsLabel.Dock = DockStyle.Fill;
            this._actionsLabel.AutoSize = false;
            this._actionsLabel.Text = "Acoes:";
            this._actionsLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            this._actionsLabel.ForeColor = Color.FromArgb(102, 102, 102);
            this._actionsLabel.TextAlign = ContentAlignment.MiddleRight;
            this._actionsLabel.Margin = new Padding(12, 0, 5, 0);

            ConfigureIconButton(this._btnItemAdd, "\u2795", "Adicionar");
            this._btnItemAdd.Click += this.OnBtnItemAddClick;
            ConfigureIconButton(this._btnItemEdit, "\u270E", "Editar");
            this._btnItemEdit.Click += this.OnBtnItemEditClick;
            ConfigureIconButton(this._btnItemRemove, "\uD83D\uDDD1", "Remover");
            this._btnItemRemove.Click += this.OnBtnItemRemoveClick;
            ConfigureIconButton(this._btnItemClear, "\uD83E\uDDF9", "Limpar item");
            this._btnItemClear.Click += this.OnBtnItemClearClick;

            //
            // _brcLabel
            //
            this._brcLabel.Dock = DockStyle.Fill;
            this._brcLabel.AutoSize = false;
            this._brcLabel.Text = "BRC: -";
            this._brcLabel.Font = new Font("Segoe UI", 8.75F, FontStyle.Bold);
            this._brcLabel.ForeColor = Color.FromArgb(102, 102, 102);
            this._brcLabel.TextAlign = ContentAlignment.MiddleLeft;
            this._brcLabel.Margin = new Padding(15, 0, 3, 0);

            //
            // _gridGroup
            //
            this._gridGroup.Dock = DockStyle.Fill;
            this._gridGroup.Text = "Itens lancados";
            this._gridGroup.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            this._gridGroup.Padding = new Padding(6, 6, 6, 6);
            this._gridGroup.Margin = new Padding(0, 0, 0, 10);
            this._gridGroup.Controls.Add(this._itemsGrid);

            //
            // _itemsGrid
            //
            this._itemsGrid.Dock = DockStyle.Fill;
            this._itemsGrid.ReadOnly = true;
            this._itemsGrid.AutoGenerateColumns = false;
            this._itemsGrid.AllowUserToAddRows = false;
            this._itemsGrid.AllowUserToDeleteRows = false;
            this._itemsGrid.AllowUserToResizeRows = false;
            this._itemsGrid.MultiSelect = false;
            this._itemsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._itemsGrid.RowHeadersVisible = false;
            this._itemsGrid.BackgroundColor = Color.White;
            this._itemsGrid.BorderStyle = BorderStyle.FixedSingle;
            this._itemsGrid.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular);
            this._itemsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            this._itemsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            this._itemsGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this._itemsGrid.ColumnHeadersHeight = 26;
            this._itemsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._itemsGrid.EnableHeadersVisualStyles = false;
            this._itemsGrid.RowTemplate.Height = 22;
            this._itemsGrid.Margin = new Padding(0);

            var colItem = new DataGridViewTextBoxColumn
            {
                Name = "item",
                HeaderText = "ITEM",
                DataPropertyName = nameof(InboundReceiptItemRow.ItemNumber),
                Width = 60,
            };
            colItem.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            var colFornecedor = new DataGridViewTextBoxColumn
            {
                Name = "fornecedor",
                HeaderText = "FORNECEDOR",
                DataPropertyName = nameof(InboundReceiptItemRow.SupplierDisplay),
                Width = 220,
            };
            colFornecedor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            var colMaterial = new DataGridViewTextBoxColumn
            {
                Name = "material",
                HeaderText = "MATERIAL",
                DataPropertyName = nameof(InboundReceiptItemRow.MaterialDisplay),
                Width = 240,
            };
            colMaterial.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            var colLote = new DataGridViewTextBoxColumn
            {
                Name = "lote",
                HeaderText = "LOTE",
                DataPropertyName = nameof(InboundReceiptItemRow.LotDisplay),
                Width = 220,
            };
            colLote.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            var colQtd = new DataGridViewTextBoxColumn
            {
                Name = "quantidade",
                HeaderText = "QUANTIDADE",
                DataPropertyName = nameof(InboundReceiptItemRow.QuantityText),
                Width = 120,
            };
            colQtd.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var colStatus = new DataGridViewTextBoxColumn
            {
                Name = "status",
                HeaderText = "STATUS",
                DataPropertyName = nameof(InboundReceiptItemRow.Status),
                Width = 100,
            };
            colStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            this._itemsGrid.Columns.AddRange(new DataGridViewColumn[] { colItem, colFornecedor, colMaterial, colLote, colQtd, colStatus });
            this._itemsGrid.CellDoubleClick += this.OnItemsGridCellDoubleClick;

            //
            // _footerLayout
            //
            this._footerLayout.Dock = DockStyle.Fill;
            this._footerLayout.ColumnCount = 3;
            this._footerLayout.RowCount = 1;
            this._footerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this._footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._footerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this._footerLayout.Padding = new Padding(0, 8, 0, 0);
            this._footerLayout.Controls.Add(this._footerLeftPanel, 0, 0);
            this._footerLayout.Controls.Add(this._itemCountLabel, 1, 0);
            this._footerLayout.Controls.Add(this._footerRightPanel, 2, 0);

            //
            // _footerLeftPanel
            //
            this._footerLeftPanel.Dock = DockStyle.Fill;
            this._footerLeftPanel.AutoSize = true;
            this._footerLeftPanel.FlowDirection = FlowDirection.LeftToRight;
            this._footerLeftPanel.WrapContents = false;
            this._footerLeftPanel.Margin = new Padding(0);
            this._footerLeftPanel.Padding = new Padding(0);
            this._footerLeftPanel.Controls.Add(this._saveButton);
            this._footerLeftPanel.Controls.Add(this._updateButton);
            this._footerLeftPanel.Controls.Add(this._clearButton);
            this._footerLeftPanel.Controls.Add(this._cancelButton);

            ConfigureActionButton(this._saveButton, "Salvar Nota (F2)", 140);
            this._saveButton.Click += this.OnBtnSaveClick;
            ConfigureActionButton(this._updateButton, "Alterar (F3)", 110);
            this._updateButton.Click += this.OnBtnUpdateClick;
            ConfigureActionButton(this._clearButton, "Limpar (F5)", 110);
            this._clearButton.Click += this.OnBtnClearClick;
            ConfigureActionButton(this._cancelButton, "Cancelar Nota (F6)", 160);
            this._cancelButton.Click += this.OnBtnCancelClick;

            //
            // _itemCountLabel
            //
            this._itemCountLabel.Dock = DockStyle.Fill;
            this._itemCountLabel.AutoSize = false;
            this._itemCountLabel.Text = "Itens na nota: 0";
            this._itemCountLabel.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            this._itemCountLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._itemCountLabel.TextAlign = ContentAlignment.MiddleCenter;

            //
            // _footerRightPanel
            //
            this._footerRightPanel.Dock = DockStyle.Fill;
            this._footerRightPanel.AutoSize = true;
            this._footerRightPanel.FlowDirection = FlowDirection.RightToLeft;
            this._footerRightPanel.WrapContents = false;
            this._footerRightPanel.Margin = new Padding(0);
            this._footerRightPanel.Padding = new Padding(0);
            this._footerRightPanel.Controls.Add(this._closeButton);

            ConfigureActionButton(this._closeButton, "Fechar (F4)", 110);
            this._closeButton.Click += this.OnBtnCloseClick;

            //
            // Form
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Text = "BRCSISTEM - Entrada de Estoque (Nota Fiscal de Material)";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(1280, 720);
            this.MinimumSize = new Size(1180, 700);
            this.BackColor = Color.White;
            this.KeyPreview = true;
            this.KeyDown += this.OnFormKeyDown;
            this.FormClosing += this.OnFormClosingHandler;
            this.Load += this.OnFormLoad;
            this.Controls.Add(this._rootLayout);

            this._rootLayout.ResumeLayout(false);
            this._headerGroup.ResumeLayout(false);
            this._headerLayout.ResumeLayout(false);
            this._headerLine1.ResumeLayout(false);
            this._headerLine1.PerformLayout();
            this._headerLine2.ResumeLayout(false);
            this._headerLine2.PerformLayout();
            this._itemGroup.ResumeLayout(false);
            this._itemLayout.ResumeLayout(false);
            this._itemLayout.PerformLayout();
            this._gridGroup.ResumeLayout(false);
            ((ISupportInitialize)(this._itemsGrid)).EndInit();
            this._footerLayout.ResumeLayout(false);
            this._footerLayout.PerformLayout();
            this._footerLeftPanel.ResumeLayout(false);
            this._footerLeftPanel.PerformLayout();
            this._footerRightPanel.ResumeLayout(false);
            this._footerRightPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private static void ConfigureFieldLabel(Label label, string text)
        {
            label.Dock = DockStyle.Fill;
            label.AutoSize = false;
            label.Text = text;
            label.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(27, 54, 93);
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Margin = new Padding(3, 0, 3, 0);
        }

        private static void ConfigureCellTextBox(TextBox textBox)
        {
            textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            textBox.Font = new Font("Segoe UI", 9.75F);
            textBox.Margin = new Padding(5, 4, 5, 4);
        }

        private static void ConfigureCellCombo(ComboBox comboBox)
        {
            comboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Font = new Font("Segoe UI", 9.75F);
            comboBox.Margin = new Padding(5, 4, 5, 4);
            comboBox.FlatStyle = FlatStyle.Standard;
        }

        private static void ConfigureIconButton(Button button, string text, string toolTip)
        {
            button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            button.Height = 25;
            button.Text = text;
            button.Font = new Font("Segoe UI Symbol", 9F, FontStyle.Regular);
            button.FlatStyle = FlatStyle.System;
            button.Margin = new Padding(1, 4, 1, 4);
            button.UseVisualStyleBackColor = true;
            if (!string.IsNullOrEmpty(toolTip))
            {
                button.AccessibleName = toolTip;
            }
        }

        private static void ConfigureActionButton(Button button, string text, int width)
        {
            button.Text = text;
            button.AutoSize = false;
            button.Height = 30;
            button.Width = width;
            button.FlatStyle = FlatStyle.System;
            button.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular);
            button.Margin = new Padding(5, 2, 5, 2);
        }

        #endregion
    }
}
