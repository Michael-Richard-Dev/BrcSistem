using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.Inventario
{
    public sealed partial class InventarioForm
    {
        private IContainer components = null;

        private TableLayoutPanel _rootLayout;
        private GroupBox _headerGroup;
        private TableLayoutPanel _headerLayout;
        private TableLayoutPanel _headerLine1Layout;
        private TableLayoutPanel _headerLine2Layout;
        private TableLayoutPanel _headerLine3Layout;
        private GroupBox _planningGroup;
        private FlowLayoutPanel _planningFlow;
        private SplitContainer _centerSplitContainer;
        private GroupBox _itemsGroup;
        private TableLayoutPanel _rightRootLayout;
        private GroupBox _pointsGroup;
        private FlowLayoutPanel _pointActionsFlow;
        private GroupBox _countsGroup;
        private FlowLayoutPanel _footerFlow;

        private Label _numberLabel;
        private Label _createdLabel;
        private Label _scheduledLabel;
        private Label _openedTitleLabel;
        private Label _closedTitleLabel;
        private Label _finalizedTitleLabel;
        private Label _maxPointsLabel;
        private Label _observationLabel;
        private Label _warehouseLabel;
        private Label _materialLabel;
        private Label _lotLabel;

        private Button _searchButton;
        private Button _newButton;
        private Button _currentButton;
        private Button _warehouseRefreshButton;
        private Button _materialRefreshButton;
        private Button _lotRefreshButton;
        private Button _addItemButton;
        private Button _addAllItemsButton;
        private Button _removeItemButton;
        private Button _clearItemsButton;

        private TextBox _numberTextBox;
        private TextBox _createdTextBox;
        private TextBox _scheduledTextBox;
        private NumericUpDown _maxPointsNumeric;
        private TextBox _observationTextBox;
        private ComboBox _warehouseComboBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private CheckBox _onlyBrcCheckBox;
        private Label _statusValueLabel;
        private Label _openedLabel;
        private Label _closedLabel;
        private Label _finalizedLabel;
        private Label _stockLabel;
        private Label _summaryLabel;
        private DataGridView _itemsGrid;
        private DataGridView _pointsGrid;
        private DataGridView _countsGrid;
        private Button _saveButton;
        private Button _updateButton;
        private Button _startButton;
        private Button _closeButton;
        private Button _reopenButton;
        private Button _finalizeButton;
        private Button _cancelButton;
        private Button _newPointButton;
        private Button _openPointButton;
        private Button _closePointButton;
        private Button _reopenPointButton;
        private Button _deletePointButton;
        private Button _zeroButton;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InventarioForm));
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._headerGroup = new System.Windows.Forms.GroupBox();
            this._headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this._headerLine1Layout = new System.Windows.Forms.TableLayoutPanel();
            this._finalizedLabel = new System.Windows.Forms.Label();
            this._finalizedTitleLabel = new System.Windows.Forms.Label();
            this._closedLabel = new System.Windows.Forms.Label();
            this._closedTitleLabel = new System.Windows.Forms.Label();
            this._openedLabel = new System.Windows.Forms.Label();
            this._openedTitleLabel = new System.Windows.Forms.Label();
            this._newButton = new System.Windows.Forms.Button();
            this._numberLabel = new System.Windows.Forms.Label();
            this._createdLabel = new System.Windows.Forms.Label();
            this._searchButton = new System.Windows.Forms.Button();
            this._numberTextBox = new System.Windows.Forms.TextBox();
            this._statusValueLabel = new System.Windows.Forms.Label();
            this._createdTextBox = new System.Windows.Forms.TextBox();
            this._currentButton = new System.Windows.Forms.Button();
            this._headerLine2Layout = new System.Windows.Forms.TableLayoutPanel();
            this._scheduledLabel = new System.Windows.Forms.Label();
            this._scheduledTextBox = new System.Windows.Forms.TextBox();
            this._maxPointsLabel = new System.Windows.Forms.Label();
            this._maxPointsNumeric = new System.Windows.Forms.NumericUpDown();
            this._headerLine3Layout = new System.Windows.Forms.TableLayoutPanel();
            this._observationLabel = new System.Windows.Forms.Label();
            this._observationTextBox = new System.Windows.Forms.TextBox();
            this._summaryLabel = new System.Windows.Forms.Label();
            this._stockLabel = new System.Windows.Forms.Label();
            this._planningGroup = new System.Windows.Forms.GroupBox();
            this._planningFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._warehouseLabel = new System.Windows.Forms.Label();
            this._warehouseComboBox = new System.Windows.Forms.ComboBox();
            this._warehouseRefreshButton = new System.Windows.Forms.Button();
            this._materialLabel = new System.Windows.Forms.Label();
            this._materialComboBox = new System.Windows.Forms.ComboBox();
            this._materialRefreshButton = new System.Windows.Forms.Button();
            this._lotLabel = new System.Windows.Forms.Label();
            this._lotComboBox = new System.Windows.Forms.ComboBox();
            this._lotRefreshButton = new System.Windows.Forms.Button();
            this._onlyBrcCheckBox = new System.Windows.Forms.CheckBox();
            this._addItemButton = new System.Windows.Forms.Button();
            this._addAllItemsButton = new System.Windows.Forms.Button();
            this._removeItemButton = new System.Windows.Forms.Button();
            this._clearItemsButton = new System.Windows.Forms.Button();
            this._centerSplitContainer = new System.Windows.Forms.SplitContainer();
            this._itemsGroup = new System.Windows.Forms.GroupBox();
            this._itemsGrid = new System.Windows.Forms.DataGridView();
            this._rightRootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._pointsGroup = new System.Windows.Forms.GroupBox();
            this._pointsGrid = new System.Windows.Forms.DataGridView();
            this._pointActionsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._newPointButton = new System.Windows.Forms.Button();
            this._openPointButton = new System.Windows.Forms.Button();
            this._closePointButton = new System.Windows.Forms.Button();
            this._reopenPointButton = new System.Windows.Forms.Button();
            this._deletePointButton = new System.Windows.Forms.Button();
            this._zeroButton = new System.Windows.Forms.Button();
            this._countsGroup = new System.Windows.Forms.GroupBox();
            this._countsGrid = new System.Windows.Forms.DataGridView();
            this._footerFlow = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._finalizeButton = new System.Windows.Forms.Button();
            this._reopenButton = new System.Windows.Forms.Button();
            this._closeButton = new System.Windows.Forms.Button();
            this._startButton = new System.Windows.Forms.Button();
            this._updateButton = new System.Windows.Forms.Button();
            this._saveButton = new System.Windows.Forms.Button();
            this._rootLayout.SuspendLayout();
            this._headerGroup.SuspendLayout();
            this._headerLayout.SuspendLayout();
            this._headerLine1Layout.SuspendLayout();
            this._headerLine2Layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._maxPointsNumeric)).BeginInit();
            this._headerLine3Layout.SuspendLayout();
            this._planningGroup.SuspendLayout();
            this._planningFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._centerSplitContainer)).BeginInit();
            this._centerSplitContainer.Panel1.SuspendLayout();
            this._centerSplitContainer.Panel2.SuspendLayout();
            this._centerSplitContainer.SuspendLayout();
            this._itemsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._itemsGrid)).BeginInit();
            this._rightRootLayout.SuspendLayout();
            this._pointsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pointsGrid)).BeginInit();
            this._pointActionsFlow.SuspendLayout();
            this._countsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._countsGrid)).BeginInit();
            this._footerFlow.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerGroup, 0, 0);
            this._rootLayout.Controls.Add(this._planningGroup, 0, 1);
            this._rootLayout.Controls.Add(this._centerSplitContainer, 0, 2);
            this._rootLayout.Controls.Add(this._footerFlow, 0, 3);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Margin = new System.Windows.Forms.Padding(4);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(16, 15, 16, 15);
            this._rootLayout.RowCount = 4;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.Size = new System.Drawing.Size(1973, 1061);
            this._rootLayout.TabIndex = 0;
            // 
            // _headerGroup
            // 
            this._headerGroup.Controls.Add(this._headerLayout);
            this._headerGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._headerGroup.Location = new System.Drawing.Point(20, 19);
            this._headerGroup.Margin = new System.Windows.Forms.Padding(4);
            this._headerGroup.Name = "_headerGroup";
            this._headerGroup.Padding = new System.Windows.Forms.Padding(4);
            this._headerGroup.Size = new System.Drawing.Size(1933, 228);
            this._headerGroup.TabIndex = 0;
            this._headerGroup.TabStop = false;
            this._headerGroup.Text = "Dados do inventario";
            // 
            // _headerLayout
            // 
            this._headerLayout.ColumnCount = 1;
            this._headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLayout.Controls.Add(this._headerLine1Layout, 0, 0);
            this._headerLayout.Controls.Add(this._headerLine2Layout, 0, 1);
            this._headerLayout.Controls.Add(this._headerLine3Layout, 0, 2);
            this._headerLayout.Controls.Add(this._stockLabel, 0, 3);
            this._headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._headerLayout.Location = new System.Drawing.Point(4, 22);
            this._headerLayout.Margin = new System.Windows.Forms.Padding(4);
            this._headerLayout.Name = "_headerLayout";
            this._headerLayout.Padding = new System.Windows.Forms.Padding(13, 12, 13, 12);
            this._headerLayout.RowCount = 4;
            this._headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._headerLayout.Size = new System.Drawing.Size(1925, 202);
            this._headerLayout.TabIndex = 0;
            // 
            // _headerLine1Layout
            // 
            this._headerLine1Layout.AutoSize = true;
            this._headerLine1Layout.ColumnCount = 14;
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 117F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 193F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 156F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 176F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 152F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 194F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 214F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 131F));
            this._headerLine1Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._headerLine1Layout.Controls.Add(this._finalizedLabel, 13, 0);
            this._headerLine1Layout.Controls.Add(this._finalizedTitleLabel, 12, 0);
            this._headerLine1Layout.Controls.Add(this._closedLabel, 11, 0);
            this._headerLine1Layout.Controls.Add(this._closedTitleLabel, 10, 0);
            this._headerLine1Layout.Controls.Add(this._openedLabel, 9, 0);
            this._headerLine1Layout.Controls.Add(this._openedTitleLabel, 8, 0);
            this._headerLine1Layout.Controls.Add(this._newButton, 3, 0);
            this._headerLine1Layout.Controls.Add(this._numberLabel, 1, 0);
            this._headerLine1Layout.Controls.Add(this._createdLabel, 5, 0);
            this._headerLine1Layout.Controls.Add(this._searchButton, 0, 0);
            this._headerLine1Layout.Controls.Add(this._numberTextBox, 2, 0);
            this._headerLine1Layout.Controls.Add(this._statusValueLabel, 4, 0);
            this._headerLine1Layout.Controls.Add(this._createdTextBox, 6, 0);
            this._headerLine1Layout.Controls.Add(this._currentButton, 7, 0);
            this._headerLine1Layout.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerLine1Layout.Location = new System.Drawing.Point(17, 16);
            this._headerLine1Layout.Margin = new System.Windows.Forms.Padding(4);
            this._headerLine1Layout.Name = "_headerLine1Layout";
            this._headerLine1Layout.RowCount = 1;
            this._headerLine1Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine1Layout.Size = new System.Drawing.Size(1891, 42);
            this._headerLine1Layout.TabIndex = 0;
            // 
            // _finalizedLabel
            // 
            this._finalizedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._finalizedLabel.AutoSize = true;
            this._finalizedLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._finalizedLabel.Location = new System.Drawing.Point(1739, 18);
            this._finalizedLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._finalizedLabel.Name = "_finalizedLabel";
            this._finalizedLabel.Size = new System.Drawing.Size(152, 15);
            this._finalizedLabel.TabIndex = 5;
            this._finalizedLabel.Text = "__/__/____ __:__";
            // 
            // _finalizedTitleLabel
            // 
            this._finalizedTitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._finalizedTitleLabel.AutoSize = true;
            this._finalizedTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._finalizedTitleLabel.Location = new System.Drawing.Point(1563, 18);
            this._finalizedTitleLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._finalizedTitleLabel.Name = "_finalizedTitleLabel";
            this._finalizedTitleLabel.Size = new System.Drawing.Size(176, 15);
            this._finalizedTitleLabel.TabIndex = 4;
            this._finalizedTitleLabel.Text = "Data_Hora Finalizacao";
            // 
            // _closedLabel
            // 
            this._closedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._closedLabel.AutoSize = true;
            this._closedLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._closedLabel.Location = new System.Drawing.Point(1419, 18);
            this._closedLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._closedLabel.Name = "_closedLabel";
            this._closedLabel.Size = new System.Drawing.Size(144, 15);
            this._closedLabel.TabIndex = 3;
            this._closedLabel.Text = "__/__/____ __:__";
            // 
            // _closedTitleLabel
            // 
            this._closedTitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._closedTitleLabel.AutoSize = true;
            this._closedTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._closedTitleLabel.Location = new System.Drawing.Point(1244, 18);
            this._closedTitleLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._closedTitleLabel.Name = "_closedTitleLabel";
            this._closedTitleLabel.Size = new System.Drawing.Size(175, 15);
            this._closedTitleLabel.TabIndex = 2;
            this._closedTitleLabel.Text = "Data_Hora Fechamento";
            // 
            // _openedLabel
            // 
            this._openedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._openedLabel.AutoSize = true;
            this._openedLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._openedLabel.Location = new System.Drawing.Point(1088, 18);
            this._openedLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._openedLabel.Name = "_openedLabel";
            this._openedLabel.Size = new System.Drawing.Size(156, 15);
            this._openedLabel.TabIndex = 1;
            this._openedLabel.Text = "__/__/____ __:__";
            // 
            // _openedTitleLabel
            // 
            this._openedTitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._openedTitleLabel.AutoSize = true;
            this._openedTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._openedTitleLabel.Location = new System.Drawing.Point(932, 18);
            this._openedTitleLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._openedTitleLabel.Name = "_openedTitleLabel";
            this._openedTitleLabel.Size = new System.Drawing.Size(156, 15);
            this._openedTitleLabel.TabIndex = 0;
            this._openedTitleLabel.Text = "Data_Hora Abertura";
            // 
            // _newButton
            // 
            this._newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._newButton.AutoSize = true;
            this._newButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._newButton.Location = new System.Drawing.Point(269, 4);
            this._newButton.Margin = new System.Windows.Forms.Padding(4);
            this._newButton.Name = "_newButton";
            this._newButton.Size = new System.Drawing.Size(57, 34);
            this._newButton.TabIndex = 3;
            this._newButton.Text = "Novo";
            this._newButton.UseVisualStyleBackColor = true;
            this._newButton.Click += new System.EventHandler(this.NewButton_Click);
            // 
            // _numberLabel
            // 
            this._numberLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._numberLabel.AutoSize = true;
            this._numberLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._numberLabel.Location = new System.Drawing.Point(76, 18);
            this._numberLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._numberLabel.Name = "_numberLabel";
            this._numberLabel.Size = new System.Drawing.Size(72, 15);
            this._numberLabel.TabIndex = 0;
            this._numberLabel.Text = "Numero";
            // 
            // _createdLabel
            // 
            this._createdLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._createdLabel.AutoSize = true;
            this._createdLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._createdLabel.Location = new System.Drawing.Point(500, 18);
            this._createdLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._createdLabel.Name = "_createdLabel";
            this._createdLabel.Size = new System.Drawing.Size(152, 15);
            this._createdLabel.TabIndex = 4;
            this._createdLabel.Text = "Data_Hora Criacao";
            // 
            // _searchButton
            // 
            this._searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._searchButton.AutoSize = true;
            this._searchButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._searchButton.Location = new System.Drawing.Point(4, 4);
            this._searchButton.Margin = new System.Windows.Forms.Padding(4);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(68, 34);
            this._searchButton.TabIndex = 2;
            this._searchButton.Text = "Buscar";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // _numberTextBox
            // 
            this._numberTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._numberTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._numberTextBox.Location = new System.Drawing.Point(152, 8);
            this._numberTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._numberTextBox.Name = "_numberTextBox";
            this._numberTextBox.ReadOnly = true;
            this._numberTextBox.Size = new System.Drawing.Size(109, 25);
            this._numberTextBox.TabIndex = 1;
            // 
            // _statusValueLabel
            // 
            this._statusValueLabel.AutoSize = true;
            this._statusValueLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._statusValueLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusValueLabel.Location = new System.Drawing.Point(341, 10);
            this._statusValueLabel.Margin = new System.Windows.Forms.Padding(11, 10, 0, 0);
            this._statusValueLabel.Name = "_statusValueLabel";
            this._statusValueLabel.Size = new System.Drawing.Size(47, 19);
            this._statusValueLabel.TabIndex = 9;
            this._statusValueLabel.Text = "Status";
            // 
            // _createdTextBox
            // 
            this._createdTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._createdTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._createdTextBox.Location = new System.Drawing.Point(656, 8);
            this._createdTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._createdTextBox.Name = "_createdTextBox";
            this._createdTextBox.ReadOnly = true;
            this._createdTextBox.Size = new System.Drawing.Size(185, 25);
            this._createdTextBox.TabIndex = 5;
            // 
            // _currentButton
            // 
            this._currentButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._currentButton.AutoSize = true;
            this._currentButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._currentButton.Location = new System.Drawing.Point(849, 4);
            this._currentButton.Margin = new System.Windows.Forms.Padding(4);
            this._currentButton.Name = "_currentButton";
            this._currentButton.Size = new System.Drawing.Size(79, 34);
            this._currentButton.TabIndex = 8;
            this._currentButton.Text = "Atual";
            this._currentButton.UseVisualStyleBackColor = true;
            this._currentButton.Click += new System.EventHandler(this.CurrentButton_Click);
            // 
            // _headerLine2Layout
            // 
            this._headerLine2Layout.AutoSize = true;
            this._headerLine2Layout.ColumnCount = 4;
            this._headerLine2Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 184F));
            this._headerLine2Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 173F));
            this._headerLine2Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 119F));
            this._headerLine2Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1415F));
            this._headerLine2Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 271F));
            this._headerLine2Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 347F));
            this._headerLine2Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this._headerLine2Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine2Layout.Controls.Add(this._scheduledLabel, 0, 0);
            this._headerLine2Layout.Controls.Add(this._scheduledTextBox, 1, 0);
            this._headerLine2Layout.Controls.Add(this._maxPointsLabel, 2, 0);
            this._headerLine2Layout.Controls.Add(this._maxPointsNumeric, 3, 0);
            this._headerLine2Layout.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerLine2Layout.Location = new System.Drawing.Point(17, 66);
            this._headerLine2Layout.Margin = new System.Windows.Forms.Padding(4);
            this._headerLine2Layout.Name = "_headerLine2Layout";
            this._headerLine2Layout.RowCount = 1;
            this._headerLine2Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine2Layout.Size = new System.Drawing.Size(1891, 33);
            this._headerLine2Layout.TabIndex = 1;
            // 
            // _scheduledLabel
            // 
            this._scheduledLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._scheduledLabel.AutoSize = true;
            this._scheduledLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._scheduledLabel.Location = new System.Drawing.Point(0, 14);
            this._scheduledLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._scheduledLabel.Name = "_scheduledLabel";
            this._scheduledLabel.Size = new System.Drawing.Size(184, 15);
            this._scheduledLabel.TabIndex = 6;
            this._scheduledLabel.Text = "Abertura programada";
            // 
            // _scheduledTextBox
            // 
            this._scheduledTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._scheduledTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._scheduledTextBox.Location = new System.Drawing.Point(188, 4);
            this._scheduledTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._scheduledTextBox.Name = "_scheduledTextBox";
            this._scheduledTextBox.Size = new System.Drawing.Size(165, 25);
            this._scheduledTextBox.TabIndex = 7;
            this._scheduledTextBox.Leave += new System.EventHandler(this.ScheduledTextBox_Leave);
            // 
            // _maxPointsLabel
            // 
            this._maxPointsLabel.AutoSize = true;
            this._maxPointsLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._maxPointsLabel.Location = new System.Drawing.Point(357, 10);
            this._maxPointsLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._maxPointsLabel.Name = "_maxPointsLabel";
            this._maxPointsLabel.Size = new System.Drawing.Size(69, 15);
            this._maxPointsLabel.TabIndex = 6;
            this._maxPointsLabel.Text = "Max pontos";
            // 
            // _maxPointsNumeric
            // 
            this._maxPointsNumeric.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._maxPointsNumeric.Location = new System.Drawing.Point(480, 4);
            this._maxPointsNumeric.Margin = new System.Windows.Forms.Padding(4);
            this._maxPointsNumeric.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this._maxPointsNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._maxPointsNumeric.Name = "_maxPointsNumeric";
            this._maxPointsNumeric.Size = new System.Drawing.Size(93, 25);
            this._maxPointsNumeric.TabIndex = 7;
            this._maxPointsNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _headerLine3Layout
            // 
            this._headerLine3Layout.AutoSize = true;
            this._headerLine3Layout.ColumnCount = 3;
            this._headerLine3Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 183F));
            this._headerLine3Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine3Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1235F));
            this._headerLine3Layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 333F));
            this._headerLine3Layout.Controls.Add(this._observationLabel, 0, 0);
            this._headerLine3Layout.Controls.Add(this._observationTextBox, 1, 0);
            this._headerLine3Layout.Controls.Add(this._summaryLabel, 3, 0);
            this._headerLine3Layout.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerLine3Layout.Location = new System.Drawing.Point(17, 107);
            this._headerLine3Layout.Margin = new System.Windows.Forms.Padding(4);
            this._headerLine3Layout.Name = "_headerLine3Layout";
            this._headerLine3Layout.RowCount = 1;
            this._headerLine3Layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLine3Layout.Size = new System.Drawing.Size(1891, 33);
            this._headerLine3Layout.TabIndex = 2;
            // 
            // _observationLabel
            // 
            this._observationLabel.AutoSize = true;
            this._observationLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._observationLabel.Location = new System.Drawing.Point(0, 10);
            this._observationLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._observationLabel.Name = "_observationLabel";
            this._observationLabel.Size = new System.Drawing.Size(117, 15);
            this._observationLabel.TabIndex = 0;
            this._observationLabel.Text = "Observacao (max 40)";
            // 
            // _observationTextBox
            // 
            this._observationTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._observationTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._observationTextBox.Location = new System.Drawing.Point(187, 4);
            this._observationTextBox.Margin = new System.Windows.Forms.Padding(4);
            this._observationTextBox.MaxLength = 40;
            this._observationTextBox.Name = "_observationTextBox";
            this._observationTextBox.Size = new System.Drawing.Size(465, 25);
            this._observationTextBox.TabIndex = 1;
            // 
            // _summaryLabel
            // 
            this._summaryLabel.AutoSize = true;
            this._summaryLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._summaryLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._summaryLabel.Location = new System.Drawing.Point(672, 10);
            this._summaryLabel.Margin = new System.Windows.Forms.Padding(16, 10, 0, 0);
            this._summaryLabel.Name = "_summaryLabel";
            this._summaryLabel.Size = new System.Drawing.Size(0, 15);
            this._summaryLabel.TabIndex = 2;
            // 
            // _stockLabel
            // 
            this._stockLabel.AutoSize = true;
            this._stockLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._stockLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._stockLabel.Location = new System.Drawing.Point(17, 144);
            this._stockLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._stockLabel.Name = "_stockLabel";
            this._stockLabel.Size = new System.Drawing.Size(0, 15);
            this._stockLabel.TabIndex = 3;
            // 
            // _planningGroup
            // 
            this._planningGroup.Controls.Add(this._planningFlow);
            this._planningGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._planningGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._planningGroup.Location = new System.Drawing.Point(20, 255);
            this._planningGroup.Margin = new System.Windows.Forms.Padding(4);
            this._planningGroup.Name = "_planningGroup";
            this._planningGroup.Padding = new System.Windows.Forms.Padding(4);
            this._planningGroup.Size = new System.Drawing.Size(1933, 148);
            this._planningGroup.TabIndex = 1;
            this._planningGroup.TabStop = false;
            this._planningGroup.Text = "Planejamento de itens";
            // 
            // _planningFlow
            // 
            this._planningFlow.AutoScroll = true;
            this._planningFlow.Controls.Add(this._warehouseLabel);
            this._planningFlow.Controls.Add(this._warehouseComboBox);
            this._planningFlow.Controls.Add(this._warehouseRefreshButton);
            this._planningFlow.Controls.Add(this._materialLabel);
            this._planningFlow.Controls.Add(this._materialComboBox);
            this._planningFlow.Controls.Add(this._materialRefreshButton);
            this._planningFlow.Controls.Add(this._lotLabel);
            this._planningFlow.Controls.Add(this._lotComboBox);
            this._planningFlow.Controls.Add(this._lotRefreshButton);
            this._planningFlow.Controls.Add(this._onlyBrcCheckBox);
            this._planningFlow.Controls.Add(this._addItemButton);
            this._planningFlow.Controls.Add(this._addAllItemsButton);
            this._planningFlow.Controls.Add(this._clearItemsButton);
            this._planningFlow.Controls.Add(this._removeItemButton);
            this._planningFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._planningFlow.Location = new System.Drawing.Point(4, 22);
            this._planningFlow.Margin = new System.Windows.Forms.Padding(4);
            this._planningFlow.Name = "_planningFlow";
            this._planningFlow.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this._planningFlow.Size = new System.Drawing.Size(1925, 122);
            this._planningFlow.TabIndex = 0;
            // 
            // _warehouseLabel
            // 
            this._warehouseLabel.AutoSize = true;
            this._warehouseLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._warehouseLabel.Location = new System.Drawing.Point(15, 20);
            this._warehouseLabel.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
            this._warehouseLabel.Name = "_warehouseLabel";
            this._warehouseLabel.Size = new System.Drawing.Size(41, 15);
            this._warehouseLabel.TabIndex = 0;
            this._warehouseLabel.Text = "Almox";
            // 
            // _warehouseComboBox
            // 
            this._warehouseComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._warehouseComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._warehouseComboBox.FormattingEnabled = true;
            this._warehouseComboBox.Location = new System.Drawing.Point(64, 14);
            this._warehouseComboBox.Margin = new System.Windows.Forms.Padding(4);
            this._warehouseComboBox.Name = "_warehouseComboBox";
            this._warehouseComboBox.Size = new System.Drawing.Size(151, 25);
            this._warehouseComboBox.TabIndex = 1;
            this._warehouseComboBox.SelectedIndexChanged += new System.EventHandler(this.WarehouseComboBox_SelectedIndexChanged);
            // 
            // _warehouseRefreshButton
            // 
            this._warehouseRefreshButton.AutoSize = true;
            this._warehouseRefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._warehouseRefreshButton.Location = new System.Drawing.Point(223, 14);
            this._warehouseRefreshButton.Margin = new System.Windows.Forms.Padding(4);
            this._warehouseRefreshButton.Name = "_warehouseRefreshButton";
            this._warehouseRefreshButton.Size = new System.Drawing.Size(60, 34);
            this._warehouseRefreshButton.TabIndex = 2;
            this._warehouseRefreshButton.Text = "Atu";
            this._warehouseRefreshButton.UseVisualStyleBackColor = true;
            this._warehouseRefreshButton.Click += new System.EventHandler(this.WarehouseRefreshButton_Click);
            // 
            // _materialLabel
            // 
            this._materialLabel.AutoSize = true;
            this._materialLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._materialLabel.Location = new System.Drawing.Point(291, 20);
            this._materialLabel.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
            this._materialLabel.Name = "_materialLabel";
            this._materialLabel.Size = new System.Drawing.Size(50, 15);
            this._materialLabel.TabIndex = 3;
            this._materialLabel.Text = "Material";
            // 
            // _materialComboBox
            // 
            this._materialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._materialComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._materialComboBox.FormattingEnabled = true;
            this._materialComboBox.Location = new System.Drawing.Point(349, 14);
            this._materialComboBox.Margin = new System.Windows.Forms.Padding(4);
            this._materialComboBox.Name = "_materialComboBox";
            this._materialComboBox.Size = new System.Drawing.Size(192, 25);
            this._materialComboBox.TabIndex = 4;
            this._materialComboBox.SelectedIndexChanged += new System.EventHandler(this.MaterialComboBox_SelectedIndexChanged);
            // 
            // _materialRefreshButton
            // 
            this._materialRefreshButton.AutoSize = true;
            this._materialRefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._materialRefreshButton.Location = new System.Drawing.Point(549, 14);
            this._materialRefreshButton.Margin = new System.Windows.Forms.Padding(4);
            this._materialRefreshButton.Name = "_materialRefreshButton";
            this._materialRefreshButton.Size = new System.Drawing.Size(60, 34);
            this._materialRefreshButton.TabIndex = 5;
            this._materialRefreshButton.Text = "Atu";
            this._materialRefreshButton.UseVisualStyleBackColor = true;
            this._materialRefreshButton.Click += new System.EventHandler(this.MaterialRefreshButton_Click);
            // 
            // _lotLabel
            // 
            this._lotLabel.AutoSize = true;
            this._lotLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lotLabel.Location = new System.Drawing.Point(617, 20);
            this._lotLabel.Margin = new System.Windows.Forms.Padding(4, 10, 4, 0);
            this._lotLabel.Name = "_lotLabel";
            this._lotLabel.Size = new System.Drawing.Size(30, 15);
            this._lotLabel.TabIndex = 6;
            this._lotLabel.Text = "Lote";
            // 
            // _lotComboBox
            // 
            this._lotComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._lotComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._lotComboBox.FormattingEnabled = true;
            this._lotComboBox.Location = new System.Drawing.Point(655, 14);
            this._lotComboBox.Margin = new System.Windows.Forms.Padding(4);
            this._lotComboBox.Name = "_lotComboBox";
            this._lotComboBox.Size = new System.Drawing.Size(186, 25);
            this._lotComboBox.TabIndex = 7;
            this._lotComboBox.SelectedIndexChanged += new System.EventHandler(this.LotComboBox_SelectedIndexChanged);
            // 
            // _lotRefreshButton
            // 
            this._lotRefreshButton.AutoSize = true;
            this._lotRefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._lotRefreshButton.Location = new System.Drawing.Point(849, 14);
            this._lotRefreshButton.Margin = new System.Windows.Forms.Padding(4);
            this._lotRefreshButton.Name = "_lotRefreshButton";
            this._lotRefreshButton.Size = new System.Drawing.Size(60, 34);
            this._lotRefreshButton.TabIndex = 8;
            this._lotRefreshButton.Text = "Atu";
            this._lotRefreshButton.UseVisualStyleBackColor = true;
            this._lotRefreshButton.Click += new System.EventHandler(this.LotRefreshButton_Click);
            // 
            // _onlyBrcCheckBox
            // 
            this._onlyBrcCheckBox.AutoSize = true;
            this._onlyBrcCheckBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._onlyBrcCheckBox.Location = new System.Drawing.Point(937, 22);
            this._onlyBrcCheckBox.Margin = new System.Windows.Forms.Padding(24, 12, 0, 0);
            this._onlyBrcCheckBox.Name = "_onlyBrcCheckBox";
            this._onlyBrcCheckBox.Size = new System.Drawing.Size(126, 19);
            this._onlyBrcCheckBox.TabIndex = 9;
            this._onlyBrcCheckBox.Text = "Somente itens BRC";
            this._onlyBrcCheckBox.UseVisualStyleBackColor = true;
            this._onlyBrcCheckBox.CheckedChanged += new System.EventHandler(this.OnlyBrcCheckBox_CheckedChanged);
            // 
            // _addItemButton
            // 
            this._addItemButton.AutoSize = true;
            this._addItemButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._addItemButton.Location = new System.Drawing.Point(1067, 14);
            this._addItemButton.Margin = new System.Windows.Forms.Padding(4);
            this._addItemButton.Name = "_addItemButton";
            this._addItemButton.Size = new System.Drawing.Size(148, 34);
            this._addItemButton.TabIndex = 10;
            this._addItemButton.Text = "Adicionar item";
            this._addItemButton.UseVisualStyleBackColor = true;
            this._addItemButton.Click += new System.EventHandler(this.AddItemButton_Click);
            // 
            // _addAllItemsButton
            // 
            this._addAllItemsButton.AutoSize = true;
            this._addAllItemsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._addAllItemsButton.Location = new System.Drawing.Point(1223, 14);
            this._addAllItemsButton.Margin = new System.Windows.Forms.Padding(4);
            this._addAllItemsButton.Name = "_addAllItemsButton";
            this._addAllItemsButton.Size = new System.Drawing.Size(239, 34);
            this._addAllItemsButton.TabIndex = 11;
            this._addAllItemsButton.Text = "Adicionar todos do almox";
            this._addAllItemsButton.UseVisualStyleBackColor = true;
            this._addAllItemsButton.Click += new System.EventHandler(this.AddAllItemsButton_Click);
            // 
            // _removeItemButton
            // 
            this._removeItemButton.AutoSize = true;
            this._removeItemButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._removeItemButton.Location = new System.Drawing.Point(1609, 14);
            this._removeItemButton.Margin = new System.Windows.Forms.Padding(4);
            this._removeItemButton.Name = "_removeItemButton";
            this._removeItemButton.Size = new System.Drawing.Size(144, 34);
            this._removeItemButton.TabIndex = 12;
            this._removeItemButton.Text = "Remover item";
            this._removeItemButton.UseVisualStyleBackColor = true;
            this._removeItemButton.Click += new System.EventHandler(this.RemoveItemButton_Click);
            // 
            // _clearItemsButton
            // 
            this._clearItemsButton.AutoSize = true;
            this._clearItemsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._clearItemsButton.Location = new System.Drawing.Point(1470, 14);
            this._clearItemsButton.Margin = new System.Windows.Forms.Padding(4);
            this._clearItemsButton.Name = "_clearItemsButton";
            this._clearItemsButton.Size = new System.Drawing.Size(131, 34);
            this._clearItemsButton.TabIndex = 13;
            this._clearItemsButton.Text = "Limpar itens";
            this._clearItemsButton.UseVisualStyleBackColor = true;
            this._clearItemsButton.Click += new System.EventHandler(this.ClearItemsButton_Click);
            // 
            // _centerSplitContainer
            // 
            this._centerSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._centerSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._centerSplitContainer.Location = new System.Drawing.Point(20, 411);
            this._centerSplitContainer.Margin = new System.Windows.Forms.Padding(4);
            this._centerSplitContainer.Name = "_centerSplitContainer";
            // 
            // _centerSplitContainer.Panel1
            // 
            this._centerSplitContainer.Panel1.Controls.Add(this._itemsGroup);
            // 
            // _centerSplitContainer.Panel2
            // 
            this._centerSplitContainer.Panel2.Controls.Add(this._rightRootLayout);
            this._centerSplitContainer.Size = new System.Drawing.Size(1933, 574);
            this._centerSplitContainer.SplitterDistance = 1073;
            this._centerSplitContainer.SplitterWidth = 5;
            this._centerSplitContainer.TabIndex = 2;
            // 
            // _itemsGroup
            // 
            this._itemsGroup.Controls.Add(this._itemsGrid);
            this._itemsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._itemsGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._itemsGroup.Location = new System.Drawing.Point(0, 0);
            this._itemsGroup.Margin = new System.Windows.Forms.Padding(4);
            this._itemsGroup.Name = "_itemsGroup";
            this._itemsGroup.Padding = new System.Windows.Forms.Padding(4);
            this._itemsGroup.Size = new System.Drawing.Size(1071, 572);
            this._itemsGroup.TabIndex = 0;
            this._itemsGroup.TabStop = false;
            this._itemsGroup.Text = "Itens planejados";
            // 
            // _itemsGrid
            // 
            this._itemsGrid.AllowUserToAddRows = false;
            this._itemsGrid.AllowUserToDeleteRows = false;
            this._itemsGrid.BackgroundColor = System.Drawing.Color.White;
            this._itemsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._itemsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._itemsGrid.Location = new System.Drawing.Point(4, 22);
            this._itemsGrid.Margin = new System.Windows.Forms.Padding(4);
            this._itemsGrid.MultiSelect = false;
            this._itemsGrid.Name = "_itemsGrid";
            this._itemsGrid.ReadOnly = true;
            this._itemsGrid.RowHeadersVisible = false;
            this._itemsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._itemsGrid.Size = new System.Drawing.Size(1063, 546);
            this._itemsGrid.TabIndex = 0;
            // 
            // _rightRootLayout
            // 
            this._rightRootLayout.ColumnCount = 1;
            this._rightRootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rightRootLayout.Controls.Add(this._pointsGroup, 0, 0);
            this._rightRootLayout.Controls.Add(this._pointActionsFlow, 0, 1);
            this._rightRootLayout.Controls.Add(this._countsGroup, 0, 2);
            this._rightRootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rightRootLayout.Location = new System.Drawing.Point(0, 0);
            this._rightRootLayout.Margin = new System.Windows.Forms.Padding(4);
            this._rightRootLayout.Name = "_rightRootLayout";
            this._rightRootLayout.RowCount = 3;
            this._rightRootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 271F));
            this._rightRootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this._rightRootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rightRootLayout.Size = new System.Drawing.Size(853, 572);
            this._rightRootLayout.TabIndex = 0;
            // 
            // _pointsGroup
            // 
            this._pointsGroup.Controls.Add(this._pointsGrid);
            this._pointsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pointsGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._pointsGroup.Location = new System.Drawing.Point(4, 4);
            this._pointsGroup.Margin = new System.Windows.Forms.Padding(4);
            this._pointsGroup.Name = "_pointsGroup";
            this._pointsGroup.Padding = new System.Windows.Forms.Padding(4);
            this._pointsGroup.Size = new System.Drawing.Size(845, 263);
            this._pointsGroup.TabIndex = 0;
            this._pointsGroup.TabStop = false;
            this._pointsGroup.Text = "Pontos e leituras";
            // 
            // _pointsGrid
            // 
            this._pointsGrid.AllowUserToAddRows = false;
            this._pointsGrid.AllowUserToDeleteRows = false;
            this._pointsGrid.BackgroundColor = System.Drawing.Color.White;
            this._pointsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._pointsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pointsGrid.Location = new System.Drawing.Point(4, 22);
            this._pointsGrid.Margin = new System.Windows.Forms.Padding(4);
            this._pointsGrid.MultiSelect = false;
            this._pointsGrid.Name = "_pointsGrid";
            this._pointsGrid.ReadOnly = true;
            this._pointsGrid.RowHeadersVisible = false;
            this._pointsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._pointsGrid.Size = new System.Drawing.Size(837, 237);
            this._pointsGrid.TabIndex = 0;
            this._pointsGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.PointsGrid_CellDoubleClick);
            // 
            // _pointActionsFlow
            // 
            this._pointActionsFlow.Controls.Add(this._newPointButton);
            this._pointActionsFlow.Controls.Add(this._openPointButton);
            this._pointActionsFlow.Controls.Add(this._closePointButton);
            this._pointActionsFlow.Controls.Add(this._reopenPointButton);
            this._pointActionsFlow.Controls.Add(this._deletePointButton);
            this._pointActionsFlow.Controls.Add(this._zeroButton);
            this._pointActionsFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pointActionsFlow.Location = new System.Drawing.Point(4, 275);
            this._pointActionsFlow.Margin = new System.Windows.Forms.Padding(4);
            this._pointActionsFlow.Name = "_pointActionsFlow";
            this._pointActionsFlow.Padding = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this._pointActionsFlow.Size = new System.Drawing.Size(845, 127);
            this._pointActionsFlow.TabIndex = 1;
            // 
            // _newPointButton
            // 
            this._newPointButton.AutoSize = true;
            this._newPointButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._newPointButton.Location = new System.Drawing.Point(4, 14);
            this._newPointButton.Margin = new System.Windows.Forms.Padding(4);
            this._newPointButton.Name = "_newPointButton";
            this._newPointButton.Size = new System.Drawing.Size(173, 31);
            this._newPointButton.TabIndex = 0;
            this._newPointButton.Text = "Novo ponto leitura";
            this._newPointButton.UseVisualStyleBackColor = true;
            this._newPointButton.Click += new System.EventHandler(this.NewPointButton_Click);
            // 
            // _openPointButton
            // 
            this._openPointButton.AutoSize = true;
            this._openPointButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._openPointButton.Location = new System.Drawing.Point(185, 14);
            this._openPointButton.Margin = new System.Windows.Forms.Padding(4);
            this._openPointButton.Name = "_openPointButton";
            this._openPointButton.Size = new System.Drawing.Size(200, 31);
            this._openPointButton.TabIndex = 1;
            this._openPointButton.Text = "Abrir tela ponto leitura";
            this._openPointButton.UseVisualStyleBackColor = true;
            this._openPointButton.Click += new System.EventHandler(this.OpenPointButton_Click);
            // 
            // _closePointButton
            // 
            this._closePointButton.AutoSize = true;
            this._closePointButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._closePointButton.Location = new System.Drawing.Point(393, 14);
            this._closePointButton.Margin = new System.Windows.Forms.Padding(4);
            this._closePointButton.Name = "_closePointButton";
            this._closePointButton.Size = new System.Drawing.Size(185, 31);
            this._closePointButton.TabIndex = 2;
            this._closePointButton.Text = "Fechar ponto leitura";
            this._closePointButton.UseVisualStyleBackColor = true;
            this._closePointButton.Click += new System.EventHandler(this.ClosePointButton_Click);
            // 
            // _reopenPointButton
            // 
            this._reopenPointButton.AutoSize = true;
            this._reopenPointButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._reopenPointButton.Location = new System.Drawing.Point(586, 14);
            this._reopenPointButton.Margin = new System.Windows.Forms.Padding(4);
            this._reopenPointButton.Name = "_reopenPointButton";
            this._reopenPointButton.Size = new System.Drawing.Size(189, 31);
            this._reopenPointButton.TabIndex = 3;
            this._reopenPointButton.Text = "Reabrir ponto leitura";
            this._reopenPointButton.UseVisualStyleBackColor = true;
            this._reopenPointButton.Click += new System.EventHandler(this.ReopenPointButton_Click);
            // 
            // _deletePointButton
            // 
            this._deletePointButton.AutoSize = true;
            this._deletePointButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._deletePointButton.Location = new System.Drawing.Point(4, 53);
            this._deletePointButton.Margin = new System.Windows.Forms.Padding(4);
            this._deletePointButton.Name = "_deletePointButton";
            this._deletePointButton.Size = new System.Drawing.Size(181, 31);
            this._deletePointButton.TabIndex = 4;
            this._deletePointButton.Text = "Excluir ponto leitura";
            this._deletePointButton.UseVisualStyleBackColor = true;
            this._deletePointButton.Click += new System.EventHandler(this.DeletePointButton_Click);
            // 
            // _zeroButton
            // 
            this._zeroButton.AutoSize = true;
            this._zeroButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._zeroButton.Location = new System.Drawing.Point(193, 53);
            this._zeroButton.Margin = new System.Windows.Forms.Padding(4);
            this._zeroButton.Name = "_zeroButton";
            this._zeroButton.Size = new System.Drawing.Size(244, 31);
            this._zeroButton.TabIndex = 5;
            this._zeroButton.Text = "Lancar zero sem contagem";
            this._zeroButton.UseVisualStyleBackColor = true;
            this._zeroButton.Click += new System.EventHandler(this.ZeroButton_Click);
            // 
            // _countsGroup
            // 
            this._countsGroup.Controls.Add(this._countsGrid);
            this._countsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._countsGroup.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._countsGroup.Location = new System.Drawing.Point(4, 410);
            this._countsGroup.Margin = new System.Windows.Forms.Padding(4);
            this._countsGroup.Name = "_countsGroup";
            this._countsGroup.Padding = new System.Windows.Forms.Padding(4);
            this._countsGroup.Size = new System.Drawing.Size(845, 158);
            this._countsGroup.TabIndex = 2;
            this._countsGroup.TabStop = false;
            this._countsGroup.Text = "Leituras";
            // 
            // _countsGrid
            // 
            this._countsGrid.AllowUserToAddRows = false;
            this._countsGrid.AllowUserToDeleteRows = false;
            this._countsGrid.BackgroundColor = System.Drawing.Color.White;
            this._countsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._countsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._countsGrid.Location = new System.Drawing.Point(4, 22);
            this._countsGrid.Margin = new System.Windows.Forms.Padding(4);
            this._countsGrid.MultiSelect = false;
            this._countsGrid.Name = "_countsGrid";
            this._countsGrid.ReadOnly = true;
            this._countsGrid.RowHeadersVisible = false;
            this._countsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._countsGrid.Size = new System.Drawing.Size(837, 132);
            this._countsGrid.TabIndex = 0;
            // 
            // _footerFlow
            // 
            this._footerFlow.AutoSize = true;
            this._footerFlow.Controls.Add(this._cancelButton);
            this._footerFlow.Controls.Add(this._finalizeButton);
            this._footerFlow.Controls.Add(this._reopenButton);
            this._footerFlow.Controls.Add(this._closeButton);
            this._footerFlow.Controls.Add(this._startButton);
            this._footerFlow.Controls.Add(this._updateButton);
            this._footerFlow.Controls.Add(this._saveButton);
            this._footerFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._footerFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this._footerFlow.Location = new System.Drawing.Point(20, 993);
            this._footerFlow.Margin = new System.Windows.Forms.Padding(4);
            this._footerFlow.Name = "_footerFlow";
            this._footerFlow.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._footerFlow.Size = new System.Drawing.Size(1933, 49);
            this._footerFlow.TabIndex = 3;
            // 
            // _cancelButton
            // 
            this._cancelButton.AutoSize = true;
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cancelButton.Location = new System.Drawing.Point(1794, 14);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(135, 31);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancelar (F6)";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // _finalizeButton
            // 
            this._finalizeButton.AutoSize = true;
            this._finalizeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._finalizeButton.Location = new System.Drawing.Point(1609, 14);
            this._finalizeButton.Margin = new System.Windows.Forms.Padding(4);
            this._finalizeButton.Name = "_finalizeButton";
            this._finalizeButton.Size = new System.Drawing.Size(177, 31);
            this._finalizeButton.TabIndex = 1;
            this._finalizeButton.Text = "Encerrar Inventario";
            this._finalizeButton.UseVisualStyleBackColor = true;
            this._finalizeButton.Click += new System.EventHandler(this.FinalizeButton_Click);
            // 
            // _reopenButton
            // 
            this._reopenButton.AutoSize = true;
            this._reopenButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._reopenButton.Location = new System.Drawing.Point(1424, 14);
            this._reopenButton.Margin = new System.Windows.Forms.Padding(4);
            this._reopenButton.Name = "_reopenButton";
            this._reopenButton.Size = new System.Drawing.Size(177, 31);
            this._reopenButton.TabIndex = 2;
            this._reopenButton.Text = "Reabrir p/ recontar";
            this._reopenButton.UseVisualStyleBackColor = true;
            this._reopenButton.Click += new System.EventHandler(this.ReopenButton_Click);
            // 
            // _closeButton
            // 
            this._closeButton.AutoSize = true;
            this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._closeButton.Location = new System.Drawing.Point(1297, 14);
            this._closeButton.Margin = new System.Windows.Forms.Padding(4);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(119, 31);
            this._closeButton.TabIndex = 3;
            this._closeButton.Text = "Fechar (F4)";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // _startButton
            // 
            this._startButton.AutoSize = true;
            this._startButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._startButton.Location = new System.Drawing.Point(1084, 14);
            this._startButton.Margin = new System.Windows.Forms.Padding(4);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(205, 31);
            this._startButton.TabIndex = 4;
            this._startButton.Text = "Iniciar o inventario (F3)";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // _updateButton
            // 
            this._updateButton.AutoSize = true;
            this._updateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._updateButton.Location = new System.Drawing.Point(996, 14);
            this._updateButton.Margin = new System.Windows.Forms.Padding(4);
            this._updateButton.Name = "_updateButton";
            this._updateButton.Size = new System.Drawing.Size(80, 31);
            this._updateButton.TabIndex = 5;
            this._updateButton.Text = "Alterar";
            this._updateButton.UseVisualStyleBackColor = true;
            this._updateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // _saveButton
            // 
            this._saveButton.AutoSize = true;
            this._saveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._saveButton.Location = new System.Drawing.Point(871, 14);
            this._saveButton.Margin = new System.Windows.Forms.Padding(4);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(117, 31);
            this._saveButton.TabIndex = 6;
            this._saveButton.Text = "Gravar (F2)";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // InventarioForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1973, 1061);
            this.Controls.Add(this._rootLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1648, 926);
            this.Name = "InventarioForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Inventario de Estoque";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnFormKeyDown);
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._headerGroup.ResumeLayout(false);
            this._headerLayout.ResumeLayout(false);
            this._headerLayout.PerformLayout();
            this._headerLine1Layout.ResumeLayout(false);
            this._headerLine1Layout.PerformLayout();
            this._headerLine2Layout.ResumeLayout(false);
            this._headerLine2Layout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._maxPointsNumeric)).EndInit();
            this._headerLine3Layout.ResumeLayout(false);
            this._headerLine3Layout.PerformLayout();
            this._planningGroup.ResumeLayout(false);
            this._planningFlow.ResumeLayout(false);
            this._planningFlow.PerformLayout();
            this._centerSplitContainer.Panel1.ResumeLayout(false);
            this._centerSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._centerSplitContainer)).EndInit();
            this._centerSplitContainer.ResumeLayout(false);
            this._itemsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._itemsGrid)).EndInit();
            this._rightRootLayout.ResumeLayout(false);
            this._pointsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pointsGrid)).EndInit();
            this._pointActionsFlow.ResumeLayout(false);
            this._pointActionsFlow.PerformLayout();
            this._countsGroup.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._countsGrid)).EndInit();
            this._footerFlow.ResumeLayout(false);
            this._footerFlow.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
