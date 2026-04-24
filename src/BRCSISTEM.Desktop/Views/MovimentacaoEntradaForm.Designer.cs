using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class MovimentacaoEntradaForm
    {
        private IContainer components = null;

        private TableLayoutPanel _rootLayout;

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
        private Button _btnWarehouseLookup;
        private Label _emissionDateLabel;
        private TextBox _emissionDateTextBox;
        private Label _receiptDateLabel;
        private TextBox _receiptDateTimeTextBox;
        private Label _statusLabel;

        private GroupBox _itemGroup;
        private TableLayoutPanel _itemLayout;
        private TableLayoutPanel _itemMainLine;

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

        private GroupBox _gridGroup;
        private DataGridView _itemsGrid;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovimentacaoEntradaForm));
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._headerGroup = new System.Windows.Forms.GroupBox();
            this._headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this._headerLine1 = new System.Windows.Forms.TableLayoutPanel();
            this._numberLabel = new System.Windows.Forms.Label();
            this._numberTextBox = new System.Windows.Forms.TextBox();
            this._btnNumberLookup = new System.Windows.Forms.Button();
            this._supplierLabel = new System.Windows.Forms.Label();
            this._supplierComboBox = new System.Windows.Forms.ComboBox();
            this._btnSupplierRefresh = new System.Windows.Forms.Button();
            this._btnSupplierLookup = new System.Windows.Forms.Button();
            this._btnSupplierNew = new System.Windows.Forms.Button();
            this._warehouseLabel = new System.Windows.Forms.Label();
            this._warehouseComboBox = new System.Windows.Forms.ComboBox();
            this._btnWarehouseLookup = new System.Windows.Forms.Button();
            this._headerLine2 = new System.Windows.Forms.TableLayoutPanel();
            this._emissionDateLabel = new System.Windows.Forms.Label();
            this._emissionDateTextBox = new System.Windows.Forms.TextBox();
            this._receiptDateLabel = new System.Windows.Forms.Label();
            this._receiptDateTimeTextBox = new System.Windows.Forms.TextBox();
            this._statusLabel = new System.Windows.Forms.Label();
            this._itemGroup = new System.Windows.Forms.GroupBox();
            this._itemLayout = new System.Windows.Forms.TableLayoutPanel();
            this._itemMainLine = new System.Windows.Forms.TableLayoutPanel();
            this._materialLabel = new System.Windows.Forms.Label();
            this._materialComboBox = new System.Windows.Forms.ComboBox();
            this._btnMaterialRefresh = new System.Windows.Forms.Button();
            this._btnMaterialLookup = new System.Windows.Forms.Button();
            this._btnMaterialNew = new System.Windows.Forms.Button();
            this._lotLabel = new System.Windows.Forms.Label();
            this._lotComboBox = new System.Windows.Forms.ComboBox();
            this._btnLotRefresh = new System.Windows.Forms.Button();
            this._btnLotLookup = new System.Windows.Forms.Button();
            this._btnLotNew = new System.Windows.Forms.Button();
            this._quantityLabel = new System.Windows.Forms.Label();
            this._quantityTextBox = new System.Windows.Forms.TextBox();
            this._actionsLabel = new System.Windows.Forms.Label();
            this._btnItemAdd = new System.Windows.Forms.Button();
            this._btnItemEdit = new System.Windows.Forms.Button();
            this._btnItemRemove = new System.Windows.Forms.Button();
            this._btnItemClear = new System.Windows.Forms.Button();
            this._gridGroup = new System.Windows.Forms.GroupBox();
            this._itemsGrid = new System.Windows.Forms.DataGridView();
            this._footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this._footerLeftPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._saveButton = new System.Windows.Forms.Button();
            this._updateButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._itemCountLabel = new System.Windows.Forms.Label();
            this._footerRightPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._closeButton = new System.Windows.Forms.Button();
            this._brcLabel = new System.Windows.Forms.Label();
            this._rootLayout.SuspendLayout();
            this._headerGroup.SuspendLayout();
            this._headerLayout.SuspendLayout();
            this._headerLine1.SuspendLayout();
            this._headerLine2.SuspendLayout();
            this._itemGroup.SuspendLayout();
            this._itemLayout.SuspendLayout();
            this._itemMainLine.SuspendLayout();
            this._gridGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._itemsGrid)).BeginInit();
            this._footerLayout.SuspendLayout();
            this._footerLeftPanel.SuspendLayout();
            this._footerRightPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerGroup, 0, 0);
            this._rootLayout.Controls.Add(this._itemGroup, 0, 1);
            this._rootLayout.Controls.Add(this._gridGroup, 0, 2);
            this._rootLayout.Controls.Add(this._footerLayout, 0, 3);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(9);
            this._rootLayout.RowCount = 4;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this._rootLayout.Size = new System.Drawing.Size(1097, 624);
            this._rootLayout.TabIndex = 0;
            // 
            // _headerGroup
            // 
            this._headerGroup.Controls.Add(this._headerLayout);
            this._headerGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._headerGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this._headerGroup.Location = new System.Drawing.Point(9, 9);
            this._headerGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this._headerGroup.Name = "_headerGroup";
            this._headerGroup.Padding = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this._headerGroup.Size = new System.Drawing.Size(1079, 111);
            this._headerGroup.TabIndex = 0;
            this._headerGroup.TabStop = false;
            this._headerGroup.Text = "Dados da Nota";
            // 
            // _headerLayout
            // 
            this._headerLayout.ColumnCount = 1;
            this._headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLayout.Controls.Add(this._headerLine1, 0, 0);
            this._headerLayout.Controls.Add(this._headerLine2, 0, 1);
            this._headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._headerLayout.Location = new System.Drawing.Point(10, 27);
            this._headerLayout.Name = "_headerLayout";
            this._headerLayout.RowCount = 2;
            this._headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this._headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this._headerLayout.Size = new System.Drawing.Size(1059, 75);
            this._headerLayout.TabIndex = 0;
            // 
            // _headerLine1
            // 
            this._headerLine1.ColumnCount = 13;
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 103F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._headerLine1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this._headerLine1.Controls.Add(this._numberLabel, 0, 0);
            this._headerLine1.Controls.Add(this._numberTextBox, 1, 0);
            this._headerLine1.Controls.Add(this._btnNumberLookup, 2, 0);
            this._headerLine1.Controls.Add(this._supplierLabel, 4, 0);
            this._headerLine1.Controls.Add(this._supplierComboBox, 5, 0);
            this._headerLine1.Controls.Add(this._btnSupplierRefresh, 6, 0);
            this._headerLine1.Controls.Add(this._btnSupplierLookup, 7, 0);
            this._headerLine1.Controls.Add(this._btnSupplierNew, 8, 0);
            this._headerLine1.Controls.Add(this._warehouseLabel, 10, 0);
            this._headerLine1.Controls.Add(this._warehouseComboBox, 11, 0);
            this._headerLine1.Controls.Add(this._btnWarehouseLookup, 12, 0);
            this._headerLine1.Dock = System.Windows.Forms.DockStyle.Fill;
            this._headerLine1.Location = new System.Drawing.Point(3, 3);
            this._headerLine1.Name = "_headerLine1";
            this._headerLine1.RowCount = 1;
            this._headerLine1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine1.Size = new System.Drawing.Size(1053, 28);
            this._headerLine1.TabIndex = 0;
            // 
            // _numberLabel
            // 
            this._numberLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._numberLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._numberLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._numberLabel.Location = new System.Drawing.Point(3, 0);
            this._numberLabel.Name = "_numberLabel";
            this._numberLabel.Size = new System.Drawing.Size(56, 28);
            this._numberLabel.TabIndex = 0;
            this._numberLabel.Text = "No Nota:";
            this._numberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _numberTextBox
            // 
            this._numberTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._numberTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._numberTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._numberTextBox.Location = new System.Drawing.Point(65, 3);
            this._numberTextBox.MinimumSize = new System.Drawing.Size(2, 23);
            this._numberTextBox.Name = "_numberTextBox";
            this._numberTextBox.Size = new System.Drawing.Size(97, 22);
            this._numberTextBox.TabIndex = 1;
            // 
            // _btnNumberLookup
            // 
            this._btnNumberLookup.AccessibleName = "Buscar nota";
            this._btnNumberLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnNumberLookup.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnNumberLookup.Location = new System.Drawing.Point(167, 2);
            this._btnNumberLookup.Margin = new System.Windows.Forms.Padding(2);
            this._btnNumberLookup.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnNumberLookup.Name = "_btnNumberLookup";
            this._btnNumberLookup.Size = new System.Drawing.Size(26, 24);
            this._btnNumberLookup.TabIndex = 2;
            this._btnNumberLookup.UseVisualStyleBackColor = true;
            // 
            // _supplierLabel
            // 
            this._supplierLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._supplierLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._supplierLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._supplierLabel.Location = new System.Drawing.Point(210, 0);
            this._supplierLabel.Name = "_supplierLabel";
            this._supplierLabel.Size = new System.Drawing.Size(69, 28);
            this._supplierLabel.TabIndex = 3;
            this._supplierLabel.Text = "Fornecedor:";
            this._supplierLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _supplierComboBox
            // 
            this._supplierComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._supplierComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._supplierComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._supplierComboBox.Location = new System.Drawing.Point(285, 3);
            this._supplierComboBox.Name = "_supplierComboBox";
            this._supplierComboBox.Size = new System.Drawing.Size(267, 21);
            this._supplierComboBox.TabIndex = 4;
            // 
            // _btnSupplierRefresh
            // 
            this._btnSupplierRefresh.AccessibleName = "Atualizar";
            this._btnSupplierRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnSupplierRefresh.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnSupplierRefresh.Location = new System.Drawing.Point(557, 2);
            this._btnSupplierRefresh.Margin = new System.Windows.Forms.Padding(2);
            this._btnSupplierRefresh.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnSupplierRefresh.Name = "_btnSupplierRefresh";
            this._btnSupplierRefresh.Size = new System.Drawing.Size(26, 24);
            this._btnSupplierRefresh.TabIndex = 5;
            this._btnSupplierRefresh.UseVisualStyleBackColor = true;
            // 
            // _btnSupplierLookup
            // 
            this._btnSupplierLookup.AccessibleName = "Buscar";
            this._btnSupplierLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnSupplierLookup.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnSupplierLookup.Location = new System.Drawing.Point(587, 2);
            this._btnSupplierLookup.Margin = new System.Windows.Forms.Padding(2);
            this._btnSupplierLookup.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnSupplierLookup.Name = "_btnSupplierLookup";
            this._btnSupplierLookup.Size = new System.Drawing.Size(26, 24);
            this._btnSupplierLookup.TabIndex = 6;
            this._btnSupplierLookup.UseVisualStyleBackColor = true;
            // 
            // _btnSupplierNew
            // 
            this._btnSupplierNew.AccessibleName = "Novo fornecedor";
            this._btnSupplierNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnSupplierNew.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnSupplierNew.Location = new System.Drawing.Point(617, 2);
            this._btnSupplierNew.Margin = new System.Windows.Forms.Padding(2);
            this._btnSupplierNew.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnSupplierNew.Name = "_btnSupplierNew";
            this._btnSupplierNew.Size = new System.Drawing.Size(26, 24);
            this._btnSupplierNew.TabIndex = 7;
            this._btnSupplierNew.UseVisualStyleBackColor = true;
            // 
            // _warehouseLabel
            // 
            this._warehouseLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._warehouseLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._warehouseLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._warehouseLabel.Location = new System.Drawing.Point(660, 0);
            this._warehouseLabel.Name = "_warehouseLabel";
            this._warehouseLabel.Size = new System.Drawing.Size(87, 28);
            this._warehouseLabel.TabIndex = 8;
            this._warehouseLabel.Text = "Almoxarifado:";
            this._warehouseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _warehouseComboBox
            // 
            this._warehouseComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._warehouseComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._warehouseComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._warehouseComboBox.Location = new System.Drawing.Point(753, 3);
            this._warehouseComboBox.Name = "_warehouseComboBox";
            this._warehouseComboBox.Size = new System.Drawing.Size(267, 21);
            this._warehouseComboBox.TabIndex = 9;
            // 
            // _btnWarehouseLookup
            // 
            this._btnWarehouseLookup.AccessibleName = "Buscar";
            this._btnWarehouseLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnWarehouseLookup.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnWarehouseLookup.Location = new System.Drawing.Point(1025, 2);
            this._btnWarehouseLookup.Margin = new System.Windows.Forms.Padding(2);
            this._btnWarehouseLookup.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnWarehouseLookup.Name = "_btnWarehouseLookup";
            this._btnWarehouseLookup.Size = new System.Drawing.Size(26, 24);
            this._btnWarehouseLookup.TabIndex = 11;
            this._btnWarehouseLookup.UseVisualStyleBackColor = true;
            // 
            // _headerLine2
            // 
            this._headerLine2.ColumnCount = 6;
            this._headerLine2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this._headerLine2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this._headerLine2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._headerLine2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 142F));
            this._headerLine2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this._headerLine2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine2.Controls.Add(this._emissionDateLabel, 0, 0);
            this._headerLine2.Controls.Add(this._emissionDateTextBox, 1, 0);
            this._headerLine2.Controls.Add(this._receiptDateLabel, 3, 0);
            this._headerLine2.Controls.Add(this._receiptDateTimeTextBox, 4, 0);
            this._headerLine2.Controls.Add(this._statusLabel, 5, 0);
            this._headerLine2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._headerLine2.Location = new System.Drawing.Point(3, 37);
            this._headerLine2.Name = "_headerLine2";
            this._headerLine2.RowCount = 1;
            this._headerLine2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine2.Size = new System.Drawing.Size(1053, 35);
            this._headerLine2.TabIndex = 1;
            // 
            // _emissionDateLabel
            // 
            this._emissionDateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._emissionDateLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._emissionDateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._emissionDateLabel.Location = new System.Drawing.Point(3, 0);
            this._emissionDateLabel.Name = "_emissionDateLabel";
            this._emissionDateLabel.Size = new System.Drawing.Size(84, 35);
            this._emissionDateLabel.TabIndex = 0;
            this._emissionDateLabel.Text = "Data Emissao:";
            this._emissionDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _emissionDateTextBox
            // 
            this._emissionDateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._emissionDateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._emissionDateTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._emissionDateTextBox.Location = new System.Drawing.Point(93, 6);
            this._emissionDateTextBox.MinimumSize = new System.Drawing.Size(2, 23);
            this._emissionDateTextBox.Name = "_emissionDateTextBox";
            this._emissionDateTextBox.Size = new System.Drawing.Size(104, 22);
            this._emissionDateTextBox.TabIndex = 1;
            // 
            // _receiptDateLabel
            // 
            this._receiptDateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._receiptDateLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._receiptDateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._receiptDateLabel.Location = new System.Drawing.Point(223, 0);
            this._receiptDateLabel.Name = "_receiptDateLabel";
            this._receiptDateLabel.Size = new System.Drawing.Size(136, 35);
            this._receiptDateLabel.TabIndex = 2;
            this._receiptDateLabel.Text = "Data/Hora Recebimento:";
            this._receiptDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _receiptDateTimeTextBox
            // 
            this._receiptDateTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._receiptDateTimeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._receiptDateTimeTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._receiptDateTimeTextBox.Location = new System.Drawing.Point(365, 6);
            this._receiptDateTimeTextBox.MinimumSize = new System.Drawing.Size(2, 23);
            this._receiptDateTimeTextBox.Name = "_receiptDateTimeTextBox";
            this._receiptDateTimeTextBox.Size = new System.Drawing.Size(144, 22);
            this._receiptDateTimeTextBox.TabIndex = 3;
            // 
            // _statusLabel
            // 
            this._statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusLabel.Location = new System.Drawing.Point(515, 0);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(535, 35);
            this._statusLabel.TabIndex = 4;
            this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _itemGroup
            // 
            this._itemGroup.Controls.Add(this._itemLayout);
            this._itemGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._itemGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this._itemGroup.Location = new System.Drawing.Point(9, 129);
            this._itemGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this._itemGroup.Name = "_itemGroup";
            this._itemGroup.Padding = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this._itemGroup.Size = new System.Drawing.Size(1079, 77);
            this._itemGroup.TabIndex = 1;
            this._itemGroup.TabStop = false;
            this._itemGroup.Text = "Adicionar / Editar Item";
            // 
            // _itemLayout
            // 
            this._itemLayout.ColumnCount = 1;
            this._itemLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._itemLayout.Controls.Add(this._itemMainLine, 0, 0);
            this._itemLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._itemLayout.Location = new System.Drawing.Point(10, 27);
            this._itemLayout.Name = "_itemLayout";
            this._itemLayout.RowCount = 1;
            this._itemLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._itemLayout.Size = new System.Drawing.Size(1059, 41);
            this._itemLayout.TabIndex = 0;
            // 
            // _itemMainLine
            // 
            this._itemMainLine.ColumnCount = 20;
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._itemMainLine.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this._itemMainLine.Controls.Add(this._materialLabel, 0, 0);
            this._itemMainLine.Controls.Add(this._materialComboBox, 1, 0);
            this._itemMainLine.Controls.Add(this._btnMaterialRefresh, 2, 0);
            this._itemMainLine.Controls.Add(this._btnMaterialLookup, 3, 0);
            this._itemMainLine.Controls.Add(this._btnMaterialNew, 4, 0);
            this._itemMainLine.Controls.Add(this._lotLabel, 6, 0);
            this._itemMainLine.Controls.Add(this._lotComboBox, 7, 0);
            this._itemMainLine.Controls.Add(this._btnLotRefresh, 8, 0);
            this._itemMainLine.Controls.Add(this._btnLotLookup, 9, 0);
            this._itemMainLine.Controls.Add(this._btnLotNew, 10, 0);
            this._itemMainLine.Controls.Add(this._quantityLabel, 12, 0);
            this._itemMainLine.Controls.Add(this._quantityTextBox, 13, 0);
            this._itemMainLine.Controls.Add(this._actionsLabel, 15, 0);
            this._itemMainLine.Controls.Add(this._btnItemAdd, 16, 0);
            this._itemMainLine.Controls.Add(this._btnItemEdit, 17, 0);
            this._itemMainLine.Controls.Add(this._btnItemRemove, 18, 0);
            this._itemMainLine.Controls.Add(this._btnItemClear, 19, 0);
            this._itemMainLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this._itemMainLine.Location = new System.Drawing.Point(3, 3);
            this._itemMainLine.Name = "_itemMainLine";
            this._itemMainLine.RowCount = 1;
            this._itemMainLine.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._itemMainLine.Size = new System.Drawing.Size(1053, 35);
            this._itemMainLine.TabIndex = 0;
            // 
            // _materialLabel
            // 
            this._materialLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._materialLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._materialLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._materialLabel.Location = new System.Drawing.Point(3, 0);
            this._materialLabel.Name = "_materialLabel";
            this._materialLabel.Size = new System.Drawing.Size(58, 35);
            this._materialLabel.TabIndex = 0;
            this._materialLabel.Text = "Material:";
            this._materialLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _materialComboBox
            // 
            this._materialComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._materialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._materialComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._materialComboBox.Location = new System.Drawing.Point(67, 7);
            this._materialComboBox.Name = "_materialComboBox";
            this._materialComboBox.Size = new System.Drawing.Size(187, 21);
            this._materialComboBox.TabIndex = 1;
            // 
            // _btnMaterialRefresh
            // 
            this._btnMaterialRefresh.AccessibleName = "Atualizar";
            this._btnMaterialRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnMaterialRefresh.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnMaterialRefresh.Location = new System.Drawing.Point(259, 5);
            this._btnMaterialRefresh.Margin = new System.Windows.Forms.Padding(2);
            this._btnMaterialRefresh.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnMaterialRefresh.Name = "_btnMaterialRefresh";
            this._btnMaterialRefresh.Size = new System.Drawing.Size(26, 24);
            this._btnMaterialRefresh.TabIndex = 2;
            this._btnMaterialRefresh.UseVisualStyleBackColor = true;
            // 
            // _btnMaterialLookup
            // 
            this._btnMaterialLookup.AccessibleName = "Buscar";
            this._btnMaterialLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnMaterialLookup.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnMaterialLookup.Location = new System.Drawing.Point(289, 5);
            this._btnMaterialLookup.Margin = new System.Windows.Forms.Padding(2);
            this._btnMaterialLookup.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnMaterialLookup.Name = "_btnMaterialLookup";
            this._btnMaterialLookup.Size = new System.Drawing.Size(26, 24);
            this._btnMaterialLookup.TabIndex = 3;
            this._btnMaterialLookup.UseVisualStyleBackColor = true;
            // 
            // _btnMaterialNew
            // 
            this._btnMaterialNew.AccessibleName = "Nova embalagem";
            this._btnMaterialNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnMaterialNew.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnMaterialNew.Location = new System.Drawing.Point(319, 5);
            this._btnMaterialNew.Margin = new System.Windows.Forms.Padding(2);
            this._btnMaterialNew.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnMaterialNew.Name = "_btnMaterialNew";
            this._btnMaterialNew.Size = new System.Drawing.Size(26, 24);
            this._btnMaterialNew.TabIndex = 4;
            this._btnMaterialNew.UseVisualStyleBackColor = true;
            // 
            // _lotLabel
            // 
            this._lotLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lotLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._lotLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._lotLabel.Location = new System.Drawing.Point(362, 0);
            this._lotLabel.Name = "_lotLabel";
            this._lotLabel.Size = new System.Drawing.Size(33, 35);
            this._lotLabel.TabIndex = 5;
            this._lotLabel.Text = "Lote:";
            this._lotLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lotComboBox
            // 
            this._lotComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._lotComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._lotComboBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._lotComboBox.Location = new System.Drawing.Point(401, 7);
            this._lotComboBox.Name = "_lotComboBox";
            this._lotComboBox.Size = new System.Drawing.Size(187, 21);
            this._lotComboBox.TabIndex = 6;
            // 
            // _btnLotRefresh
            // 
            this._btnLotRefresh.AccessibleName = "Atualizar";
            this._btnLotRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnLotRefresh.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnLotRefresh.Location = new System.Drawing.Point(593, 5);
            this._btnLotRefresh.Margin = new System.Windows.Forms.Padding(2);
            this._btnLotRefresh.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnLotRefresh.Name = "_btnLotRefresh";
            this._btnLotRefresh.Size = new System.Drawing.Size(29, 24);
            this._btnLotRefresh.TabIndex = 7;
            this._btnLotRefresh.UseVisualStyleBackColor = true;
            // 
            // _btnLotLookup
            // 
            this._btnLotLookup.AccessibleName = "Buscar";
            this._btnLotLookup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnLotLookup.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnLotLookup.Location = new System.Drawing.Point(626, 5);
            this._btnLotLookup.Margin = new System.Windows.Forms.Padding(2);
            this._btnLotLookup.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnLotLookup.Name = "_btnLotLookup";
            this._btnLotLookup.Size = new System.Drawing.Size(29, 24);
            this._btnLotLookup.TabIndex = 8;
            this._btnLotLookup.UseVisualStyleBackColor = true;
            // 
            // _btnLotNew
            // 
            this._btnLotNew.AccessibleName = "Novo lote";
            this._btnLotNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnLotNew.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnLotNew.Location = new System.Drawing.Point(659, 5);
            this._btnLotNew.Margin = new System.Windows.Forms.Padding(2);
            this._btnLotNew.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnLotNew.Name = "_btnLotNew";
            this._btnLotNew.Size = new System.Drawing.Size(27, 24);
            this._btnLotNew.TabIndex = 9;
            this._btnLotNew.UseVisualStyleBackColor = true;
            // 
            // _quantityLabel
            // 
            this._quantityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._quantityLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._quantityLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._quantityLabel.Location = new System.Drawing.Point(709, 0);
            this._quantityLabel.Name = "_quantityLabel";
            this._quantityLabel.Size = new System.Drawing.Size(72, 35);
            this._quantityLabel.TabIndex = 10;
            this._quantityLabel.Text = "Quantidade:";
            this._quantityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _quantityTextBox
            // 
            this._quantityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._quantityTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._quantityTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._quantityTextBox.Location = new System.Drawing.Point(787, 6);
            this._quantityTextBox.MinimumSize = new System.Drawing.Size(2, 23);
            this._quantityTextBox.Name = "_quantityTextBox";
            this._quantityTextBox.Size = new System.Drawing.Size(67, 22);
            this._quantityTextBox.TabIndex = 11;
            this._quantityTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _actionsLabel
            // 
            this._actionsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._actionsLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._actionsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._actionsLabel.Location = new System.Drawing.Point(877, 0);
            this._actionsLabel.Name = "_actionsLabel";
            this._actionsLabel.Size = new System.Drawing.Size(50, 35);
            this._actionsLabel.TabIndex = 12;
            this._actionsLabel.Text = "Acoes:";
            this._actionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _btnItemAdd
            // 
            this._btnItemAdd.AccessibleName = "Adicionar";
            this._btnItemAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnItemAdd.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnItemAdd.Location = new System.Drawing.Point(932, 5);
            this._btnItemAdd.Margin = new System.Windows.Forms.Padding(2);
            this._btnItemAdd.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnItemAdd.Name = "_btnItemAdd";
            this._btnItemAdd.Size = new System.Drawing.Size(27, 24);
            this._btnItemAdd.TabIndex = 13;
            this._btnItemAdd.UseVisualStyleBackColor = true;
            // 
            // _btnItemEdit
            // 
            this._btnItemEdit.AccessibleName = "Editar";
            this._btnItemEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnItemEdit.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnItemEdit.Location = new System.Drawing.Point(963, 5);
            this._btnItemEdit.Margin = new System.Windows.Forms.Padding(2);
            this._btnItemEdit.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnItemEdit.Name = "_btnItemEdit";
            this._btnItemEdit.Size = new System.Drawing.Size(26, 24);
            this._btnItemEdit.TabIndex = 14;
            this._btnItemEdit.UseVisualStyleBackColor = true;
            // 
            // _btnItemRemove
            // 
            this._btnItemRemove.AccessibleName = "Remover";
            this._btnItemRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnItemRemove.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnItemRemove.Location = new System.Drawing.Point(993, 5);
            this._btnItemRemove.Margin = new System.Windows.Forms.Padding(2);
            this._btnItemRemove.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnItemRemove.Name = "_btnItemRemove";
            this._btnItemRemove.Size = new System.Drawing.Size(26, 24);
            this._btnItemRemove.TabIndex = 15;
            this._btnItemRemove.UseVisualStyleBackColor = true;
            // 
            // _btnItemClear
            // 
            this._btnItemClear.AccessibleName = "Limpar item";
            this._btnItemClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._btnItemClear.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnItemClear.Location = new System.Drawing.Point(1023, 5);
            this._btnItemClear.Margin = new System.Windows.Forms.Padding(2);
            this._btnItemClear.MinimumSize = new System.Drawing.Size(26, 24);
            this._btnItemClear.Name = "_btnItemClear";
            this._btnItemClear.Size = new System.Drawing.Size(28, 24);
            this._btnItemClear.TabIndex = 16;
            this._btnItemClear.UseVisualStyleBackColor = true;
            // 
            // _gridGroup
            // 
            this._gridGroup.Controls.Add(this._itemsGrid);
            this._gridGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this._gridGroup.Location = new System.Drawing.Point(9, 215);
            this._gridGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this._gridGroup.Name = "_gridGroup";
            this._gridGroup.Padding = new System.Windows.Forms.Padding(5);
            this._gridGroup.Size = new System.Drawing.Size(1079, 341);
            this._gridGroup.TabIndex = 2;
            this._gridGroup.TabStop = false;
            this._gridGroup.Text = "Itens lancados";
            // 
            // _itemsGrid
            // 
            this._itemsGrid.AllowUserToAddRows = false;
            this._itemsGrid.AllowUserToDeleteRows = false;
            this._itemsGrid.AllowUserToResizeRows = false;
            this._itemsGrid.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._itemsGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._itemsGrid.ColumnHeadersHeight = 26;
            this._itemsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._itemsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._itemsGrid.EnableHeadersVisualStyles = false;
            this._itemsGrid.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._itemsGrid.Location = new System.Drawing.Point(5, 23);
            this._itemsGrid.Margin = new System.Windows.Forms.Padding(0);
            this._itemsGrid.MultiSelect = false;
            this._itemsGrid.Name = "_itemsGrid";
            this._itemsGrid.ReadOnly = true;
            this._itemsGrid.RowHeadersVisible = false;
            this._itemsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._itemsGrid.Size = new System.Drawing.Size(1069, 313);
            this._itemsGrid.TabIndex = 0;
            // 
            // _footerLayout
            // 
            this._footerLayout.ColumnCount = 3;
            this._footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._footerLayout.Controls.Add(this._footerLeftPanel, 0, 0);
            this._footerLayout.Controls.Add(this._itemCountLabel, 1, 0);
            this._footerLayout.Controls.Add(this._footerRightPanel, 2, 0);
            this._footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerLayout.Location = new System.Drawing.Point(12, 568);
            this._footerLayout.Name = "_footerLayout";
            this._footerLayout.Padding = new System.Windows.Forms.Padding(0, 9, 0, 0);
            this._footerLayout.RowCount = 1;
            this._footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._footerLayout.Size = new System.Drawing.Size(1073, 44);
            this._footerLayout.TabIndex = 3;
            // 
            // _footerLeftPanel
            // 
            this._footerLeftPanel.AutoSize = true;
            this._footerLeftPanel.Controls.Add(this._saveButton);
            this._footerLeftPanel.Controls.Add(this._updateButton);
            this._footerLeftPanel.Controls.Add(this._clearButton);
            this._footerLeftPanel.Controls.Add(this._cancelButton);
            this._footerLeftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerLeftPanel.Location = new System.Drawing.Point(0, 9);
            this._footerLeftPanel.Margin = new System.Windows.Forms.Padding(0);
            this._footerLeftPanel.Name = "_footerLeftPanel";
            this._footerLeftPanel.Size = new System.Drawing.Size(605, 35);
            this._footerLeftPanel.TabIndex = 0;
            this._footerLeftPanel.WrapContents = false;
            // 
            // _saveButton
            // 
            this._saveButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._saveButton.Location = new System.Drawing.Point(5, 2);
            this._saveButton.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this._saveButton.Size = new System.Drawing.Size(150, 30);
            this._saveButton.TabIndex = 0;
            this._saveButton.Text = "Salvar Nota (F2)";
            this._saveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // _updateButton
            // 
            this._updateButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._updateButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._updateButton.Location = new System.Drawing.Point(165, 2);
            this._updateButton.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this._updateButton.Name = "_updateButton";
            this._updateButton.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this._updateButton.Size = new System.Drawing.Size(130, 30);
            this._updateButton.TabIndex = 1;
            this._updateButton.Text = "Alterar (F3)";
            this._updateButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // _clearButton
            // 
            this._clearButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._clearButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._clearButton.Location = new System.Drawing.Point(305, 2);
            this._clearButton.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this._clearButton.Size = new System.Drawing.Size(120, 30);
            this._clearButton.TabIndex = 2;
            this._clearButton.Text = "Limpar (F5)";
            this._clearButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // _cancelButton
            // 
            this._cancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._cancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._cancelButton.Location = new System.Drawing.Point(435, 2);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this._cancelButton.Size = new System.Drawing.Size(165, 30);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancelar Nota (F6)";
            this._cancelButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // _itemCountLabel
            // 
            this._itemCountLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._itemCountLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._itemCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._itemCountLabel.Location = new System.Drawing.Point(608, 9);
            this._itemCountLabel.Name = "_itemCountLabel";
            this._itemCountLabel.Size = new System.Drawing.Size(342, 35);
            this._itemCountLabel.TabIndex = 1;
            this._itemCountLabel.Text = "Itens na nota: 0";
            this._itemCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _footerRightPanel
            // 
            this._footerRightPanel.AutoSize = true;
            this._footerRightPanel.Controls.Add(this._closeButton);
            this._footerRightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerRightPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this._footerRightPanel.Location = new System.Drawing.Point(953, 9);
            this._footerRightPanel.Margin = new System.Windows.Forms.Padding(0);
            this._footerRightPanel.Name = "_footerRightPanel";
            this._footerRightPanel.Size = new System.Drawing.Size(120, 35);
            this._footerRightPanel.TabIndex = 2;
            this._footerRightPanel.WrapContents = false;
            // 
            // _closeButton
            // 
            this._closeButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._closeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._closeButton.Location = new System.Drawing.Point(5, 2);
            this._closeButton.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this._closeButton.Size = new System.Drawing.Size(110, 30);
            this._closeButton.TabIndex = 0;
            this._closeButton.Text = "Fechar (F4)";
            this._closeButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            // 
            // _brcLabel
            // 
            this._brcLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._brcLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._brcLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._brcLabel.Location = new System.Drawing.Point(0, 0);
            this._brcLabel.Margin = new System.Windows.Forms.Padding(0);
            this._brcLabel.Name = "_brcLabel";
            this._brcLabel.Size = new System.Drawing.Size(1, 1);
            this._brcLabel.TabIndex = 17;
            this._brcLabel.Text = "BRC: -";
            this._brcLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._brcLabel.Visible = false;
            // 
            // MovimentacaoEntradaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1097, 624);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1014, 612);
            this.Name = "MovimentacaoEntradaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Entrada de Estoque (Nota Fiscal de Material)";
            this._rootLayout.ResumeLayout(false);
            this._headerGroup.ResumeLayout(false);
            this._headerLayout.ResumeLayout(false);
            this._headerLine1.ResumeLayout(false);
            this._headerLine1.PerformLayout();
            this._headerLine2.ResumeLayout(false);
            this._headerLine2.PerformLayout();
            this._itemGroup.ResumeLayout(false);
            this._itemLayout.ResumeLayout(false);
            this._itemMainLine.ResumeLayout(false);
            this._itemMainLine.PerformLayout();
            this._gridGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._itemsGrid)).EndInit();
            this._footerLayout.ResumeLayout(false);
            this._footerLayout.PerformLayout();
            this._footerLeftPanel.ResumeLayout(false);
            this._footerRightPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
