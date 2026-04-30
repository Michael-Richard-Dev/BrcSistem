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
            DataGridViewTextBoxColumn itemsAlmoxColumn;
            DataGridViewTextBoxColumn itemsMaterialColumn;
            DataGridViewTextBoxColumn itemsLotColumn;
            DataGridViewTextBoxColumn itemsSaldoColumn;
            DataGridViewTextBoxColumn itemsEntradaColumn;
            DataGridViewTextBoxColumn itemsSaidaColumn;
            DataGridViewTextBoxColumn itemsFinalColumn;
            DataGridViewTextBoxColumn itemsAjusteColumn;
            DataGridViewTextBoxColumn pointsIdColumn;
            DataGridViewTextBoxColumn pointsNameColumn;
            DataGridViewTextBoxColumn pointsIpColumn;
            DataGridViewTextBoxColumn pointsComputerColumn;
            DataGridViewTextBoxColumn pointsStatusColumn;
            DataGridViewTextBoxColumn countsIdColumn;
            DataGridViewTextBoxColumn countsDateColumn;
            DataGridViewTextBoxColumn countsPointColumn;
            DataGridViewTextBoxColumn countsItemColumn;
            DataGridViewTextBoxColumn countsQuantityColumn;
            DataGridViewTextBoxColumn countsUserColumn;
            this.components = new Container();
            this._rootLayout = new TableLayoutPanel();
            this._headerGroup = new GroupBox();
            this._headerLayout = new TableLayoutPanel();
            this._headerLine1Layout = new TableLayoutPanel();
            this._numberLabel = new Label();
            this._numberTextBox = new TextBox();
            this._searchButton = new Button();
            this._newButton = new Button();
            this._createdLabel = new Label();
            this._createdTextBox = new TextBox();
            this._scheduledLabel = new Label();
            this._scheduledTextBox = new TextBox();
            this._currentButton = new Button();
            this._statusValueLabel = new Label();
            this._headerLine2Layout = new TableLayoutPanel();
            this._openedTitleLabel = new Label();
            this._openedLabel = new Label();
            this._closedTitleLabel = new Label();
            this._closedLabel = new Label();
            this._finalizedTitleLabel = new Label();
            this._finalizedLabel = new Label();
            this._maxPointsLabel = new Label();
            this._maxPointsNumeric = new NumericUpDown();
            this._headerLine3Layout = new TableLayoutPanel();
            this._observationLabel = new Label();
            this._observationTextBox = new TextBox();
            this._summaryLabel = new Label();
            this._stockLabel = new Label();
            this._planningGroup = new GroupBox();
            this._planningFlow = new FlowLayoutPanel();
            this._warehouseLabel = new Label();
            this._warehouseComboBox = new ComboBox();
            this._warehouseRefreshButton = new Button();
            this._materialLabel = new Label();
            this._materialComboBox = new ComboBox();
            this._materialRefreshButton = new Button();
            this._lotLabel = new Label();
            this._lotComboBox = new ComboBox();
            this._lotRefreshButton = new Button();
            this._onlyBrcCheckBox = new CheckBox();
            this._addItemButton = new Button();
            this._addAllItemsButton = new Button();
            this._removeItemButton = new Button();
            this._clearItemsButton = new Button();
            this._centerSplitContainer = new SplitContainer();
            this._itemsGroup = new GroupBox();
            this._itemsGrid = new DataGridView();
            this._rightRootLayout = new TableLayoutPanel();
            this._pointsGroup = new GroupBox();
            this._pointsGrid = new DataGridView();
            this._pointActionsFlow = new FlowLayoutPanel();
            this._newPointButton = new Button();
            this._openPointButton = new Button();
            this._closePointButton = new Button();
            this._reopenPointButton = new Button();
            this._deletePointButton = new Button();
            this._zeroButton = new Button();
            this._countsGroup = new GroupBox();
            this._countsGrid = new DataGridView();
            this._footerFlow = new FlowLayoutPanel();
            this._cancelButton = new Button();
            this._finalizeButton = new Button();
            this._reopenButton = new Button();
            this._closeButton = new Button();
            this._startButton = new Button();
            this._updateButton = new Button();
            this._saveButton = new Button();
            itemsAlmoxColumn = new DataGridViewTextBoxColumn();
            itemsMaterialColumn = new DataGridViewTextBoxColumn();
            itemsLotColumn = new DataGridViewTextBoxColumn();
            itemsSaldoColumn = new DataGridViewTextBoxColumn();
            itemsEntradaColumn = new DataGridViewTextBoxColumn();
            itemsSaidaColumn = new DataGridViewTextBoxColumn();
            itemsFinalColumn = new DataGridViewTextBoxColumn();
            itemsAjusteColumn = new DataGridViewTextBoxColumn();
            pointsIdColumn = new DataGridViewTextBoxColumn();
            pointsNameColumn = new DataGridViewTextBoxColumn();
            pointsIpColumn = new DataGridViewTextBoxColumn();
            pointsComputerColumn = new DataGridViewTextBoxColumn();
            pointsStatusColumn = new DataGridViewTextBoxColumn();
            countsIdColumn = new DataGridViewTextBoxColumn();
            countsDateColumn = new DataGridViewTextBoxColumn();
            countsPointColumn = new DataGridViewTextBoxColumn();
            countsItemColumn = new DataGridViewTextBoxColumn();
            countsQuantityColumn = new DataGridViewTextBoxColumn();
            countsUserColumn = new DataGridViewTextBoxColumn();
            ((ISupportInitialize)(this._maxPointsNumeric)).BeginInit();
            this._rootLayout.SuspendLayout();
            this._headerGroup.SuspendLayout();
            this._headerLayout.SuspendLayout();
            this._headerLine1Layout.SuspendLayout();
            this._headerLine2Layout.SuspendLayout();
            this._headerLine3Layout.SuspendLayout();
            this._planningGroup.SuspendLayout();
            this._planningFlow.SuspendLayout();
            ((ISupportInitialize)(this._centerSplitContainer)).BeginInit();
            this._centerSplitContainer.Panel1.SuspendLayout();
            this._centerSplitContainer.Panel2.SuspendLayout();
            this._centerSplitContainer.SuspendLayout();
            this._itemsGroup.SuspendLayout();
            ((ISupportInitialize)(this._itemsGrid)).BeginInit();
            this._rightRootLayout.SuspendLayout();
            this._pointsGroup.SuspendLayout();
            ((ISupportInitialize)(this._pointsGrid)).BeginInit();
            this._pointActionsFlow.SuspendLayout();
            this._countsGroup.SuspendLayout();
            ((ISupportInitialize)(this._countsGrid)).BeginInit();
            this._footerFlow.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerGroup, 0, 0);
            this._rootLayout.Controls.Add(this._planningGroup, 0, 1);
            this._rootLayout.Controls.Add(this._centerSplitContainer, 0, 2);
            this._rootLayout.Controls.Add(this._footerFlow, 0, 3);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Location = new Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new Padding(12);
            this._rootLayout.RowCount = 4;
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.Size = new Size(1434, 821);
            this._rootLayout.TabIndex = 0;
            // 
            // _headerGroup
            // 
            this._headerGroup.Controls.Add(this._headerLayout);
            this._headerGroup.Dock = DockStyle.Top;
            this._headerGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this._headerGroup.Location = new Point(15, 15);
            this._headerGroup.Name = "_headerGroup";
            this._headerGroup.Size = new Size(1404, 185);
            this._headerGroup.TabIndex = 0;
            this._headerGroup.TabStop = false;
            this._headerGroup.Text = "Dados do inventario";
            // 
            // _headerLayout
            // 
            this._headerLayout.ColumnCount = 1;
            this._headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._headerLayout.Controls.Add(this._headerLine1Layout, 0, 0);
            this._headerLayout.Controls.Add(this._headerLine2Layout, 0, 1);
            this._headerLayout.Controls.Add(this._headerLine3Layout, 0, 2);
            this._headerLayout.Controls.Add(this._stockLabel, 0, 3);
            this._headerLayout.Dock = DockStyle.Fill;
            this._headerLayout.Location = new Point(3, 21);
            this._headerLayout.Name = "_headerLayout";
            this._headerLayout.Padding = new Padding(10);
            this._headerLayout.RowCount = 4;
            this._headerLayout.RowStyles.Add(new RowStyle());
            this._headerLayout.RowStyles.Add(new RowStyle());
            this._headerLayout.RowStyles.Add(new RowStyle());
            this._headerLayout.RowStyles.Add(new RowStyle());
            this._headerLayout.Size = new Size(1398, 161);
            this._headerLayout.TabIndex = 0;
            // 
            // _headerLine1Layout
            // 
            this._headerLine1Layout.AutoSize = true;
            this._headerLine1Layout.ColumnCount = 10;
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            this._headerLine1Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._headerLine1Layout.Controls.Add(this._numberLabel, 0, 0);
            this._headerLine1Layout.Controls.Add(this._numberTextBox, 1, 0);
            this._headerLine1Layout.Controls.Add(this._searchButton, 2, 0);
            this._headerLine1Layout.Controls.Add(this._newButton, 3, 0);
            this._headerLine1Layout.Controls.Add(this._createdLabel, 4, 0);
            this._headerLine1Layout.Controls.Add(this._createdTextBox, 5, 0);
            this._headerLine1Layout.Controls.Add(this._scheduledLabel, 6, 0);
            this._headerLine1Layout.Controls.Add(this._scheduledTextBox, 7, 0);
            this._headerLine1Layout.Controls.Add(this._currentButton, 8, 0);
            this._headerLine1Layout.Controls.Add(this._statusValueLabel, 9, 0);
            this._headerLine1Layout.Dock = DockStyle.Top;
            this._headerLine1Layout.Location = new Point(13, 13);
            this._headerLine1Layout.Name = "_headerLine1Layout";
            this._headerLine1Layout.RowCount = 1;
            this._headerLine1Layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._headerLine1Layout.Size = new Size(1372, 38);
            this._headerLine1Layout.TabIndex = 0;
            // 
            // _numberLabel
            // 
            this._numberLabel.AutoSize = true;
            this._numberLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._numberLabel.Location = new Point(0, 8);
            this._numberLabel.Margin = new Padding(0, 8, 0, 0);
            this._numberLabel.Name = "_numberLabel";
            this._numberLabel.Size = new Size(80, 15);
            this._numberLabel.TabIndex = 0;
            this._numberLabel.Text = "Numero";
            // 
            // _numberTextBox
            // 
            this._numberTextBox.Dock = DockStyle.Top;
            this._numberTextBox.Font = new Font("Segoe UI", 10F);
            this._numberTextBox.Location = new Point(113, 3);
            this._numberTextBox.Name = "_numberTextBox";
            this._numberTextBox.ReadOnly = true;
            this._numberTextBox.Size = new Size(134, 25);
            this._numberTextBox.TabIndex = 1;
            // 
            // _searchButton
            // 
            this._searchButton.AutoSize = true;
            this._searchButton.FlatStyle = FlatStyle.System;
            this._searchButton.Location = new Point(253, 3);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new Size(56, 25);
            this._searchButton.TabIndex = 2;
            this._searchButton.Text = "Buscar";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // _newButton
            // 
            this._newButton.AutoSize = true;
            this._newButton.FlatStyle = FlatStyle.System;
            this._newButton.Location = new Point(333, 3);
            this._newButton.Name = "_newButton";
            this._newButton.Size = new Size(51, 25);
            this._newButton.TabIndex = 3;
            this._newButton.Text = "Novo";
            this._newButton.UseVisualStyleBackColor = true;
            this._newButton.Click += new System.EventHandler(this.NewButton_Click);
            // 
            // _createdLabel
            // 
            this._createdLabel.AutoSize = true;
            this._createdLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._createdLabel.Location = new Point(413, 8);
            this._createdLabel.Margin = new Padding(0, 8, 0, 0);
            this._createdLabel.Name = "_createdLabel";
            this._createdLabel.Size = new Size(49, 15);
            this._createdLabel.TabIndex = 4;
            this._createdLabel.Text = "Data_Hora Criacao";
            // 
            // _createdTextBox
            // 
            this._createdTextBox.Dock = DockStyle.Top;
            this._createdTextBox.Font = new Font("Segoe UI", 10F);
            this._createdTextBox.Location = new Point(528, 3);
            this._createdTextBox.Name = "_createdTextBox";
            this._createdTextBox.ReadOnly = true;
            this._createdTextBox.Size = new Size(144, 25);
            this._createdTextBox.TabIndex = 5;
            // 
            // _scheduledLabel
            // 
            this._scheduledLabel.AutoSize = true;
            this._scheduledLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._scheduledLabel.Location = new Point(678, 8);
            this._scheduledLabel.Margin = new Padding(0, 8, 0, 0);
            this._scheduledLabel.Name = "_scheduledLabel";
            this._scheduledLabel.Size = new Size(68, 15);
            this._scheduledLabel.TabIndex = 6;
            this._scheduledLabel.Text = "Abertura programada";
            // 
            // _scheduledTextBox
            // 
            this._scheduledTextBox.Dock = DockStyle.Top;
            this._scheduledTextBox.Font = new Font("Segoe UI", 10F);
            this._scheduledTextBox.Location = new Point(803, 3);
            this._scheduledTextBox.Name = "_scheduledTextBox";
            this._scheduledTextBox.Size = new Size(104, 25);
            this._scheduledTextBox.TabIndex = 7;
            this._scheduledTextBox.Leave += new System.EventHandler(this.ScheduledTextBox_Leave);
            // 
            // _currentButton
            // 
            this._currentButton.AutoSize = true;
            this._currentButton.FlatStyle = FlatStyle.System;
            this._currentButton.Location = new Point(913, 3);
            this._currentButton.Name = "_currentButton";
            this._currentButton.Size = new Size(49, 25);
            this._currentButton.TabIndex = 8;
            this._currentButton.Text = "Atual";
            this._currentButton.UseVisualStyleBackColor = true;
            this._currentButton.Click += new System.EventHandler(this.CurrentButton_Click);
            // 
            // _statusValueLabel
            // 
            this._statusValueLabel.AutoSize = true;
            this._statusValueLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this._statusValueLabel.ForeColor = Color.SeaGreen;
            this._statusValueLabel.Location = new Point(978, 8);
            this._statusValueLabel.Margin = new Padding(8, 8, 0, 0);
            this._statusValueLabel.Name = "_statusValueLabel";
            this._statusValueLabel.Size = new Size(0, 19);
            this._statusValueLabel.TabIndex = 9;
            // 
            // _headerLine2Layout
            // 
            this._headerLine2Layout.AutoSize = true;
            this._headerLine2Layout.ColumnCount = 8;
            this._headerLine2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._headerLine2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._headerLine2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._headerLine2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._headerLine2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._headerLine2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
            this._headerLine2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this._headerLine2Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._headerLine2Layout.Controls.Add(this._openedTitleLabel, 0, 0);
            this._headerLine2Layout.Controls.Add(this._openedLabel, 1, 0);
            this._headerLine2Layout.Controls.Add(this._closedTitleLabel, 2, 0);
            this._headerLine2Layout.Controls.Add(this._closedLabel, 3, 0);
            this._headerLine2Layout.Controls.Add(this._finalizedTitleLabel, 4, 0);
            this._headerLine2Layout.Controls.Add(this._finalizedLabel, 5, 0);
            this._headerLine2Layout.Controls.Add(this._maxPointsLabel, 6, 0);
            this._headerLine2Layout.Controls.Add(this._maxPointsNumeric, 7, 0);
            this._headerLine2Layout.Dock = DockStyle.Top;
            this._headerLine2Layout.Location = new Point(13, 57);
            this._headerLine2Layout.Name = "_headerLine2Layout";
            this._headerLine2Layout.RowCount = 1;
            this._headerLine2Layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._headerLine2Layout.Size = new Size(1372, 28);
            this._headerLine2Layout.TabIndex = 1;
            // 
            // _openedTitleLabel
            // 
            this._openedTitleLabel.AutoSize = true;
            this._openedTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._openedTitleLabel.Location = new Point(0, 8);
            this._openedTitleLabel.Margin = new Padding(0, 8, 0, 0);
            this._openedTitleLabel.Name = "_openedTitleLabel";
            this._openedTitleLabel.Size = new Size(57, 15);
            this._openedTitleLabel.TabIndex = 0;
            this._openedTitleLabel.Text = "Data_Hora Abertura";
            // 
            // _openedLabel
            // 
            this._openedLabel.AutoSize = true;
            this._openedLabel.Font = new Font("Segoe UI", 9F);
            this._openedLabel.Location = new Point(110, 8);
            this._openedLabel.Margin = new Padding(0, 8, 0, 0);
            this._openedLabel.Name = "_openedLabel";
            this._openedLabel.Size = new Size(0, 15);
            this._openedLabel.TabIndex = 1;
            // 
            // _closedTitleLabel
            // 
            this._closedTitleLabel.AutoSize = true;
            this._closedTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._closedTitleLabel.Location = new Point(220, 8);
            this._closedTitleLabel.Margin = new Padding(0, 8, 0, 0);
            this._closedTitleLabel.Name = "_closedTitleLabel";
            this._closedTitleLabel.Size = new Size(70, 15);
            this._closedTitleLabel.TabIndex = 2;
            this._closedTitleLabel.Text = "Data_Hora Fechamento";
            // 
            // _closedLabel
            // 
            this._closedLabel.AutoSize = true;
            this._closedLabel.Font = new Font("Segoe UI", 9F);
            this._closedLabel.Location = new Point(330, 8);
            this._closedLabel.Margin = new Padding(0, 8, 0, 0);
            this._closedLabel.Name = "_closedLabel";
            this._closedLabel.Size = new Size(0, 15);
            this._closedLabel.TabIndex = 3;
            // 
            // _finalizedTitleLabel
            // 
            this._finalizedTitleLabel.AutoSize = true;
            this._finalizedTitleLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._finalizedTitleLabel.Location = new Point(440, 8);
            this._finalizedTitleLabel.Margin = new Padding(0, 8, 0, 0);
            this._finalizedTitleLabel.Name = "_finalizedTitleLabel";
            this._finalizedTitleLabel.Size = new Size(67, 15);
            this._finalizedTitleLabel.TabIndex = 4;
            this._finalizedTitleLabel.Text = "Data_Hora Finalizacao";
            // 
            // _finalizedLabel
            // 
            this._finalizedLabel.AutoSize = true;
            this._finalizedLabel.Font = new Font("Segoe UI", 9F);
            this._finalizedLabel.Location = new Point(550, 8);
            this._finalizedLabel.Margin = new Padding(0, 8, 0, 0);
            this._finalizedLabel.Name = "_finalizedLabel";
            this._finalizedLabel.Size = new Size(0, 15);
            this._finalizedLabel.TabIndex = 5;
            // 
            // _maxPointsLabel
            // 
            this._maxPointsLabel.AutoSize = true;
            this._maxPointsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._maxPointsLabel.Location = new Point(810, 8);
            this._maxPointsLabel.Margin = new Padding(0, 8, 0, 0);
            this._maxPointsLabel.Name = "_maxPointsLabel";
            this._maxPointsLabel.Size = new Size(67, 15);
            this._maxPointsLabel.TabIndex = 6;
            this._maxPointsLabel.Text = "Max pontos:";
            // 
            // _maxPointsNumeric
            // 
            this._maxPointsNumeric.Font = new Font("Segoe UI", 10F);
            this._maxPointsNumeric.Location = new Point(933, 3);
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
            this._maxPointsNumeric.Size = new Size(70, 25);
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
            this._headerLine3Layout.ColumnCount = 4;
            this._headerLine3Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._headerLine3Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._headerLine3Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            this._headerLine3Layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
            this._headerLine3Layout.Controls.Add(this._observationLabel, 0, 0);
            this._headerLine3Layout.Controls.Add(this._observationTextBox, 1, 0);
            this._headerLine3Layout.Controls.Add(this._summaryLabel, 3, 0);
            this._headerLine3Layout.Dock = DockStyle.Top;
            this._headerLine3Layout.Location = new Point(13, 91);
            this._headerLine3Layout.Name = "_headerLine3Layout";
            this._headerLine3Layout.RowCount = 1;
            this._headerLine3Layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._headerLine3Layout.Size = new Size(1372, 25);
            this._headerLine3Layout.TabIndex = 2;
            // 
            // _observationLabel
            // 
            this._observationLabel.AutoSize = true;
            this._observationLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._observationLabel.Location = new Point(0, 8);
            this._observationLabel.Margin = new Padding(0, 8, 0, 0);
            this._observationLabel.Name = "_observationLabel";
            this._observationLabel.Size = new Size(68, 15);
            this._observationLabel.TabIndex = 0;
            this._observationLabel.Text = "Observacao (max 40)";
            // 
            // _observationTextBox
            // 
            this._observationTextBox.Dock = DockStyle.Top;
            this._observationTextBox.Font = new Font("Segoe UI", 10F);
            this._observationTextBox.Location = new Point(113, 3);
            this._observationTextBox.MaxLength = 40;
            this._observationTextBox.Name = "_observationTextBox";
            this._observationTextBox.Size = new Size(846, 25);
            this._observationTextBox.TabIndex = 1;
            // 
            // _summaryLabel
            // 
            this._summaryLabel.AutoSize = true;
            this._summaryLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._summaryLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._summaryLabel.Location = new Point(1124, 8);
            this._summaryLabel.Margin = new Padding(12, 8, 0, 0);
            this._summaryLabel.Name = "_summaryLabel";
            this._summaryLabel.Size = new Size(0, 15);
            this._summaryLabel.TabIndex = 2;
            // 
            // _stockLabel
            // 
            this._stockLabel.AutoSize = true;
            this._stockLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._stockLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._stockLabel.Location = new Point(13, 119);
            this._stockLabel.Name = "_stockLabel";
            this._stockLabel.Size = new Size(0, 15);
            this._stockLabel.TabIndex = 3;
            // 
            // _planningGroup
            // 
            this._planningGroup.Controls.Add(this._planningFlow);
            this._planningGroup.Dock = DockStyle.Top;
            this._planningGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this._planningGroup.Location = new Point(15, 206);
            this._planningGroup.Name = "_planningGroup";
            this._planningGroup.Size = new Size(1404, 120);
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
            this._planningFlow.Controls.Add(this._removeItemButton);
            this._planningFlow.Controls.Add(this._clearItemsButton);
            this._planningFlow.Dock = DockStyle.Fill;
            this._planningFlow.Location = new Point(3, 21);
            this._planningFlow.Name = "_planningFlow";
            this._planningFlow.Padding = new Padding(8);
            this._planningFlow.Size = new Size(1398, 96);
            this._planningFlow.TabIndex = 0;
            this._planningFlow.WrapContents = true;
            // 
            // _warehouseLabel
            // 
            this._warehouseLabel.AutoSize = true;
            this._warehouseLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._warehouseLabel.Location = new Point(11, 16);
            this._warehouseLabel.Margin = new Padding(3, 8, 3, 0);
            this._warehouseLabel.Name = "_warehouseLabel";
            this._warehouseLabel.Size = new Size(80, 15);
            this._warehouseLabel.TabIndex = 0;
            this._warehouseLabel.Text = "Almoxarifado:";
            // 
            // _warehouseComboBox
            // 
            this._warehouseComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this._warehouseComboBox.Font = new Font("Segoe UI", 10F);
            this._warehouseComboBox.FormattingEnabled = true;
            this._warehouseComboBox.Location = new Point(97, 11);
            this._warehouseComboBox.Name = "_warehouseComboBox";
            this._warehouseComboBox.Size = new Size(220, 25);
            this._warehouseComboBox.TabIndex = 1;
            this._warehouseComboBox.SelectedIndexChanged += new System.EventHandler(this.WarehouseComboBox_SelectedIndexChanged);
            // 
            // _warehouseRefreshButton
            // 
            this._warehouseRefreshButton.AutoSize = true;
            this._warehouseRefreshButton.FlatStyle = FlatStyle.System;
            this._warehouseRefreshButton.Location = new Point(323, 11);
            this._warehouseRefreshButton.Name = "_warehouseRefreshButton";
            this._warehouseRefreshButton.Size = new Size(42, 25);
            this._warehouseRefreshButton.TabIndex = 2;
            this._warehouseRefreshButton.Text = "Atu";
            this._warehouseRefreshButton.UseVisualStyleBackColor = true;
            this._warehouseRefreshButton.Click += new System.EventHandler(this.WarehouseRefreshButton_Click);
            // 
            // _materialLabel
            // 
            this._materialLabel.AutoSize = true;
            this._materialLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._materialLabel.Location = new Point(371, 16);
            this._materialLabel.Margin = new Padding(3, 8, 3, 0);
            this._materialLabel.Name = "_materialLabel";
            this._materialLabel.Size = new Size(53, 15);
            this._materialLabel.TabIndex = 3;
            this._materialLabel.Text = "Material:";
            // 
            // _materialComboBox
            // 
            this._materialComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this._materialComboBox.Font = new Font("Segoe UI", 10F);
            this._materialComboBox.FormattingEnabled = true;
            this._materialComboBox.Location = new Point(430, 11);
            this._materialComboBox.Name = "_materialComboBox";
            this._materialComboBox.Size = new Size(260, 25);
            this._materialComboBox.TabIndex = 4;
            this._materialComboBox.SelectedIndexChanged += new System.EventHandler(this.MaterialComboBox_SelectedIndexChanged);
            // 
            // _materialRefreshButton
            // 
            this._materialRefreshButton.AutoSize = true;
            this._materialRefreshButton.FlatStyle = FlatStyle.System;
            this._materialRefreshButton.Location = new Point(696, 11);
            this._materialRefreshButton.Name = "_materialRefreshButton";
            this._materialRefreshButton.Size = new Size(42, 25);
            this._materialRefreshButton.TabIndex = 5;
            this._materialRefreshButton.Text = "Atu";
            this._materialRefreshButton.UseVisualStyleBackColor = true;
            this._materialRefreshButton.Click += new System.EventHandler(this.MaterialRefreshButton_Click);
            // 
            // _lotLabel
            // 
            this._lotLabel.AutoSize = true;
            this._lotLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._lotLabel.Location = new Point(744, 16);
            this._lotLabel.Margin = new Padding(3, 8, 3, 0);
            this._lotLabel.Name = "_lotLabel";
            this._lotLabel.Size = new Size(32, 15);
            this._lotLabel.TabIndex = 6;
            this._lotLabel.Text = "Lote:";
            // 
            // _lotComboBox
            // 
            this._lotComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this._lotComboBox.Font = new Font("Segoe UI", 10F);
            this._lotComboBox.FormattingEnabled = true;
            this._lotComboBox.Location = new Point(782, 11);
            this._lotComboBox.Name = "_lotComboBox";
            this._lotComboBox.Size = new Size(220, 25);
            this._lotComboBox.TabIndex = 7;
            this._lotComboBox.SelectedIndexChanged += new System.EventHandler(this.LotComboBox_SelectedIndexChanged);
            // 
            // _lotRefreshButton
            // 
            this._lotRefreshButton.AutoSize = true;
            this._lotRefreshButton.FlatStyle = FlatStyle.System;
            this._lotRefreshButton.Location = new Point(1008, 11);
            this._lotRefreshButton.Name = "_lotRefreshButton";
            this._lotRefreshButton.Size = new Size(42, 25);
            this._lotRefreshButton.TabIndex = 8;
            this._lotRefreshButton.Text = "Atu";
            this._lotRefreshButton.UseVisualStyleBackColor = true;
            this._lotRefreshButton.Click += new System.EventHandler(this.LotRefreshButton_Click);
            // 
            // _onlyBrcCheckBox
            // 
            this._onlyBrcCheckBox.AutoSize = true;
            this._onlyBrcCheckBox.Font = new Font("Segoe UI", 9F);
            this._onlyBrcCheckBox.Location = new Point(1068, 13);
            this._onlyBrcCheckBox.Margin = new Padding(18, 10, 0, 0);
            this._onlyBrcCheckBox.Name = "_onlyBrcCheckBox";
            this._onlyBrcCheckBox.Size = new Size(96, 19);
            this._onlyBrcCheckBox.TabIndex = 9;
            this._onlyBrcCheckBox.Text = "Somente itens BRC";
            this._onlyBrcCheckBox.UseVisualStyleBackColor = true;
            this._onlyBrcCheckBox.CheckedChanged += new System.EventHandler(this.OnlyBrcCheckBox_CheckedChanged);
            // 
            // _addItemButton
            // 
            this._addItemButton.AutoSize = true;
            this._addItemButton.FlatStyle = FlatStyle.System;
            this._addItemButton.Location = new Point(11, 42);
            this._addItemButton.Name = "_addItemButton";
            this._addItemButton.Size = new Size(74, 25);
            this._addItemButton.TabIndex = 10;
            this._addItemButton.Text = "Adicionar item";
            this._addItemButton.UseVisualStyleBackColor = true;
            this._addItemButton.Click += new System.EventHandler(this.AddItemButton_Click);
            // 
            // _addAllItemsButton
            // 
            this._addAllItemsButton.AutoSize = true;
            this._addAllItemsButton.FlatStyle = FlatStyle.System;
            this._addAllItemsButton.Location = new Point(91, 42);
            this._addAllItemsButton.Name = "_addAllItemsButton";
            this._addAllItemsButton.Size = new Size(76, 25);
            this._addAllItemsButton.TabIndex = 11;
            this._addAllItemsButton.Text = "Adicionar todos do almox";
            this._addAllItemsButton.UseVisualStyleBackColor = true;
            this._addAllItemsButton.Click += new System.EventHandler(this.AddAllItemsButton_Click);
            // 
            // _removeItemButton
            // 
            this._removeItemButton.AutoSize = true;
            this._removeItemButton.FlatStyle = FlatStyle.System;
            this._removeItemButton.Location = new Point(173, 42);
            this._removeItemButton.Name = "_removeItemButton";
            this._removeItemButton.Size = new Size(70, 25);
            this._removeItemButton.TabIndex = 12;
            this._removeItemButton.Text = "Remover item";
            this._removeItemButton.UseVisualStyleBackColor = true;
            this._removeItemButton.Click += new System.EventHandler(this.RemoveItemButton_Click);
            // 
            // _clearItemsButton
            // 
            this._clearItemsButton.AutoSize = true;
            this._clearItemsButton.FlatStyle = FlatStyle.System;
            this._clearItemsButton.Location = new Point(249, 42);
            this._clearItemsButton.Name = "_clearItemsButton";
            this._clearItemsButton.Size = new Size(57, 25);
            this._clearItemsButton.TabIndex = 13;
            this._clearItemsButton.Text = "Limpar itens";
            this._clearItemsButton.UseVisualStyleBackColor = true;
            this._clearItemsButton.Click += new System.EventHandler(this.ClearItemsButton_Click);
            // 
            // _centerSplitContainer
            // 
            this._centerSplitContainer.BorderStyle = BorderStyle.FixedSingle;
            this._centerSplitContainer.Dock = DockStyle.Fill;
            this._centerSplitContainer.Location = new Point(15, 332);
            this._centerSplitContainer.Name = "_centerSplitContainer";
            this._centerSplitContainer.Orientation = Orientation.Vertical;
            // 
            // _centerSplitContainer.Panel1
            // 
            this._centerSplitContainer.Panel1.Controls.Add(this._itemsGroup);
            // 
            // _centerSplitContainer.Panel2
            // 
            this._centerSplitContainer.Panel2.Controls.Add(this._rightRootLayout);
            this._centerSplitContainer.Size = new Size(1404, 430);
            this._centerSplitContainer.SplitterDistance = 780;
            this._centerSplitContainer.TabIndex = 2;
            // 
            // _itemsGroup
            // 
            this._itemsGroup.Controls.Add(this._itemsGrid);
            this._itemsGroup.Dock = DockStyle.Fill;
            this._itemsGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this._itemsGroup.Location = new Point(0, 0);
            this._itemsGroup.Name = "_itemsGroup";
            this._itemsGroup.Size = new Size(778, 428);
            this._itemsGroup.TabIndex = 0;
            this._itemsGroup.TabStop = false;
            this._itemsGroup.Text = "Itens do Inventario";
            // 
            // _itemsGrid
            // 
            this._itemsGrid.AllowUserToAddRows = false;
            this._itemsGrid.AllowUserToDeleteRows = false;
            this._itemsGrid.AutoGenerateColumns = false;
            this._itemsGrid.BackgroundColor = Color.White;
            this._itemsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._itemsGrid.Columns.AddRange(new DataGridViewColumn[] {
            itemsAlmoxColumn,
            itemsMaterialColumn,
            itemsLotColumn,
            itemsSaldoColumn,
            itemsEntradaColumn,
            itemsSaidaColumn,
            itemsFinalColumn,
            itemsAjusteColumn});
            this._itemsGrid.Dock = DockStyle.Fill;
            this._itemsGrid.Location = new Point(3, 21);
            this._itemsGrid.MultiSelect = false;
            this._itemsGrid.Name = "_itemsGrid";
            this._itemsGrid.ReadOnly = true;
            this._itemsGrid.RowHeadersVisible = false;
            this._itemsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._itemsGrid.Size = new Size(772, 404);
            this._itemsGrid.TabIndex = 0;
            // 
            // items columns
            // 
            itemsAlmoxColumn.DataPropertyName = "WarehouseCode";
            itemsAlmoxColumn.HeaderText = "ALMOX";
            itemsAlmoxColumn.Name = "almox";
            itemsAlmoxColumn.ReadOnly = true;
            itemsAlmoxColumn.Width = 80;
            itemsMaterialColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            itemsMaterialColumn.DataPropertyName = "MaterialDisplay";
            itemsMaterialColumn.HeaderText = "MATERIAL";
            itemsMaterialColumn.Name = "material";
            itemsMaterialColumn.ReadOnly = true;
            itemsLotColumn.DataPropertyName = "LotDisplay";
            itemsLotColumn.HeaderText = "LOTE";
            itemsLotColumn.Name = "lote";
            itemsLotColumn.ReadOnly = true;
            itemsLotColumn.Width = 180;
            itemsSaldoColumn.DataPropertyName = "SystemBalanceText";
            itemsSaldoColumn.HeaderText = "SALDO";
            itemsSaldoColumn.Name = "saldo";
            itemsSaldoColumn.ReadOnly = true;
            itemsSaldoColumn.Width = 95;
            itemsEntradaColumn.DataPropertyName = "InputQuantityText";
            itemsEntradaColumn.HeaderText = "ENTRADA";
            itemsEntradaColumn.Name = "entrada";
            itemsEntradaColumn.ReadOnly = true;
            itemsEntradaColumn.Width = 95;
            itemsSaidaColumn.DataPropertyName = "OutputQuantityText";
            itemsSaidaColumn.HeaderText = "SAIDA";
            itemsSaidaColumn.Name = "saida";
            itemsSaidaColumn.ReadOnly = true;
            itemsSaidaColumn.Width = 95;
            itemsFinalColumn.DataPropertyName = "FinalBalanceText";
            itemsFinalColumn.HeaderText = "SALDO FINAL";
            itemsFinalColumn.Name = "final";
            itemsFinalColumn.ReadOnly = true;
            itemsFinalColumn.Width = 110;
            itemsAjusteColumn.DataPropertyName = "AdjustmentQuantityText";
            itemsAjusteColumn.HeaderText = "DIVERGENCIA";
            itemsAjusteColumn.Name = "ajuste";
            itemsAjusteColumn.ReadOnly = true;
            itemsAjusteColumn.Width = 95;
            // 
            // _rightRootLayout
            // 
            this._rightRootLayout.ColumnCount = 1;
            this._rightRootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rightRootLayout.Controls.Add(this._pointsGroup, 0, 0);
            this._rightRootLayout.Controls.Add(this._pointActionsFlow, 0, 1);
            this._rightRootLayout.Controls.Add(this._countsGroup, 0, 2);
            this._rightRootLayout.Dock = DockStyle.Fill;
            this._rightRootLayout.Location = new Point(0, 0);
            this._rightRootLayout.Name = "_rightRootLayout";
            this._rightRootLayout.RowCount = 3;
            this._rightRootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220F));
            this._rightRootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F));
            this._rightRootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rightRootLayout.Size = new Size(618, 428);
            this._rightRootLayout.TabIndex = 0;
            // 
            // _pointsGroup
            // 
            this._pointsGroup.Controls.Add(this._pointsGrid);
            this._pointsGroup.Dock = DockStyle.Fill;
            this._pointsGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this._pointsGroup.Location = new Point(3, 3);
            this._pointsGroup.Name = "_pointsGroup";
            this._pointsGroup.Size = new Size(612, 214);
            this._pointsGroup.TabIndex = 0;
            this._pointsGroup.TabStop = false;
            this._pointsGroup.Text = "Pontos e leituras";
            // 
            // _pointsGrid
            // 
            this._pointsGrid.AllowUserToAddRows = false;
            this._pointsGrid.AllowUserToDeleteRows = false;
            this._pointsGrid.AutoGenerateColumns = false;
            this._pointsGrid.BackgroundColor = Color.White;
            this._pointsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._pointsGrid.Columns.AddRange(new DataGridViewColumn[] {
            pointsIdColumn,
            pointsNameColumn,
            pointsIpColumn,
            pointsComputerColumn,
            pointsStatusColumn});
            this._pointsGrid.Dock = DockStyle.Fill;
            this._pointsGrid.Location = new Point(3, 21);
            this._pointsGrid.MultiSelect = false;
            this._pointsGrid.Name = "_pointsGrid";
            this._pointsGrid.ReadOnly = true;
            this._pointsGrid.RowHeadersVisible = false;
            this._pointsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._pointsGrid.Size = new Size(606, 190);
            this._pointsGrid.TabIndex = 0;
            this._pointsGrid.CellDoubleClick += new DataGridViewCellEventHandler(this.PointsGrid_CellDoubleClick);
            // 
            // points columns
            // 
            pointsIdColumn.DataPropertyName = "Id";
            pointsIdColumn.HeaderText = "ID";
            pointsIdColumn.Name = "id";
            pointsIdColumn.ReadOnly = true;
            pointsIdColumn.Width = 60;
            pointsNameColumn.DataPropertyName = "PointName";
            pointsNameColumn.HeaderText = "PONTO";
            pointsNameColumn.Name = "nome";
            pointsNameColumn.ReadOnly = true;
            pointsNameColumn.Width = 120;
            pointsIpColumn.DataPropertyName = "IpAddress";
            pointsIpColumn.HeaderText = "IP";
            pointsIpColumn.Name = "ip";
            pointsIpColumn.ReadOnly = true;
            pointsIpColumn.Width = 110;
            pointsComputerColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            pointsComputerColumn.DataPropertyName = "ComputerName";
            pointsComputerColumn.HeaderText = "COMPUTADOR";
            pointsComputerColumn.Name = "pc";
            pointsComputerColumn.ReadOnly = true;
            pointsStatusColumn.DataPropertyName = "Status";
            pointsStatusColumn.HeaderText = "STATUS";
            pointsStatusColumn.Name = "status";
            pointsStatusColumn.ReadOnly = true;
            pointsStatusColumn.Width = 90;
            // 
            // _pointActionsFlow
            // 
            this._pointActionsFlow.Controls.Add(this._newPointButton);
            this._pointActionsFlow.Controls.Add(this._openPointButton);
            this._pointActionsFlow.Controls.Add(this._closePointButton);
            this._pointActionsFlow.Controls.Add(this._reopenPointButton);
            this._pointActionsFlow.Controls.Add(this._deletePointButton);
            this._pointActionsFlow.Controls.Add(this._zeroButton);
            this._pointActionsFlow.Dock = DockStyle.Fill;
            this._pointActionsFlow.Location = new Point(3, 223);
            this._pointActionsFlow.Name = "_pointActionsFlow";
            this._pointActionsFlow.Padding = new Padding(0, 8, 0, 8);
            this._pointActionsFlow.Size = new Size(612, 104);
            this._pointActionsFlow.TabIndex = 1;
            this._pointActionsFlow.WrapContents = true;
            // 
            // _newPointButton
            // 
            this._newPointButton.AutoSize = true;
            this._newPointButton.FlatStyle = FlatStyle.System;
            this._newPointButton.Location = new Point(3, 11);
            this._newPointButton.Name = "_newPointButton";
            this._newPointButton.Size = new Size(81, 25);
            this._newPointButton.TabIndex = 0;
            this._newPointButton.Text = "Novo ponto leitura";
            this._newPointButton.UseVisualStyleBackColor = true;
            this._newPointButton.Click += new System.EventHandler(this.NewPointButton_Click);
            // 
            // _openPointButton
            // 
            this._openPointButton.AutoSize = true;
            this._openPointButton.FlatStyle = FlatStyle.System;
            this._openPointButton.Location = new Point(90, 11);
            this._openPointButton.Name = "_openPointButton";
            this._openPointButton.Size = new Size(82, 25);
            this._openPointButton.TabIndex = 1;
            this._openPointButton.Text = "Abrir tela ponto leitura";
            this._openPointButton.UseVisualStyleBackColor = true;
            this._openPointButton.Click += new System.EventHandler(this.OpenPointButton_Click);
            // 
            // _closePointButton
            // 
            this._closePointButton.AutoSize = true;
            this._closePointButton.FlatStyle = FlatStyle.System;
            this._closePointButton.Location = new Point(178, 11);
            this._closePointButton.Name = "_closePointButton";
            this._closePointButton.Size = new Size(87, 25);
            this._closePointButton.TabIndex = 2;
            this._closePointButton.Text = "Fechar ponto leitura";
            this._closePointButton.UseVisualStyleBackColor = true;
            this._closePointButton.Click += new System.EventHandler(this.ClosePointButton_Click);
            // 
            // _reopenPointButton
            // 
            this._reopenPointButton.AutoSize = true;
            this._reopenPointButton.FlatStyle = FlatStyle.System;
            this._reopenPointButton.Location = new Point(271, 11);
            this._reopenPointButton.Name = "_reopenPointButton";
            this._reopenPointButton.Size = new Size(93, 25);
            this._reopenPointButton.TabIndex = 3;
            this._reopenPointButton.Text = "Reabrir ponto leitura";
            this._reopenPointButton.UseVisualStyleBackColor = true;
            this._reopenPointButton.Click += new System.EventHandler(this.ReopenPointButton_Click);
            // 
            // _deletePointButton
            // 
            this._deletePointButton.AutoSize = true;
            this._deletePointButton.FlatStyle = FlatStyle.System;
            this._deletePointButton.Location = new Point(370, 11);
            this._deletePointButton.Name = "_deletePointButton";
            this._deletePointButton.Size = new Size(85, 25);
            this._deletePointButton.TabIndex = 4;
            this._deletePointButton.Text = "Excluir ponto leitura";
            this._deletePointButton.UseVisualStyleBackColor = true;
            this._deletePointButton.Click += new System.EventHandler(this.DeletePointButton_Click);
            // 
            // _zeroButton
            // 
            this._zeroButton.AutoSize = true;
            this._zeroButton.FlatStyle = FlatStyle.System;
            this._zeroButton.Location = new Point(461, 11);
            this._zeroButton.Name = "_zeroButton";
            this._zeroButton.Size = new Size(82, 25);
            this._zeroButton.TabIndex = 5;
            this._zeroButton.Text = "Lancar zero sem contagem";
            this._zeroButton.UseVisualStyleBackColor = true;
            this._zeroButton.Click += new System.EventHandler(this.ZeroButton_Click);
            // 
            // _countsGroup
            // 
            this._countsGroup.Controls.Add(this._countsGrid);
            this._countsGroup.Dock = DockStyle.Fill;
            this._countsGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this._countsGroup.Location = new Point(3, 333);
            this._countsGroup.Name = "_countsGroup";
            this._countsGroup.Size = new Size(612, 92);
            this._countsGroup.TabIndex = 2;
            this._countsGroup.TabStop = false;
            this._countsGroup.Text = "Ultimas Leituras";
            // 
            // _countsGrid
            // 
            this._countsGrid.AllowUserToAddRows = false;
            this._countsGrid.AllowUserToDeleteRows = false;
            this._countsGrid.AutoGenerateColumns = false;
            this._countsGrid.BackgroundColor = Color.White;
            this._countsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._countsGrid.Columns.AddRange(new DataGridViewColumn[] {
            countsIdColumn,
            countsDateColumn,
            countsPointColumn,
            countsItemColumn,
            countsQuantityColumn,
            countsUserColumn});
            this._countsGrid.Dock = DockStyle.Fill;
            this._countsGrid.Location = new Point(3, 21);
            this._countsGrid.MultiSelect = false;
            this._countsGrid.Name = "_countsGrid";
            this._countsGrid.ReadOnly = true;
            this._countsGrid.RowHeadersVisible = false;
            this._countsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._countsGrid.Size = new Size(606, 68);
            this._countsGrid.TabIndex = 0;
            // 
            // counts columns
            // 
            countsIdColumn.DataPropertyName = "Id";
            countsIdColumn.HeaderText = "ID";
            countsIdColumn.Name = "counts_id";
            countsIdColumn.ReadOnly = true;
            countsIdColumn.Width = 60;
            countsDateColumn.DataPropertyName = "CountedAtDisplay";
            countsDateColumn.HeaderText = "DATA/HORA";
            countsDateColumn.Name = "data";
            countsDateColumn.ReadOnly = true;
            countsDateColumn.Width = 140;
            countsPointColumn.DataPropertyName = "PointId";
            countsPointColumn.HeaderText = "PONTO";
            countsPointColumn.Name = "ponto";
            countsPointColumn.ReadOnly = true;
            countsPointColumn.Width = 70;
            countsItemColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            countsItemColumn.DataPropertyName = "ItemDisplay";
            countsItemColumn.HeaderText = "ITEM";
            countsItemColumn.Name = "item";
            countsItemColumn.ReadOnly = true;
            countsQuantityColumn.DataPropertyName = "QuantityText";
            countsQuantityColumn.HeaderText = "QTD";
            countsQuantityColumn.Name = "qtd";
            countsQuantityColumn.ReadOnly = true;
            countsQuantityColumn.Width = 90;
            countsUserColumn.DataPropertyName = "UserName";
            countsUserColumn.HeaderText = "USUARIO";
            countsUserColumn.Name = "usuario";
            countsUserColumn.ReadOnly = true;
            countsUserColumn.Width = 90;
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
            this._footerFlow.Dock = DockStyle.Fill;
            this._footerFlow.FlowDirection = FlowDirection.RightToLeft;
            this._footerFlow.Location = new Point(15, 768);
            this._footerFlow.Name = "_footerFlow";
            this._footerFlow.Padding = new Padding(0, 8, 0, 0);
            this._footerFlow.Size = new Size(1404, 38);
            this._footerFlow.TabIndex = 3;
            // 
            // footer buttons
            // 
            this._cancelButton.AutoSize = true;
            this._cancelButton.FlatStyle = FlatStyle.System;
            this._cancelButton.Location = new Point(1335, 11);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(66, 25);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancelar (F6)";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            this._finalizeButton.AutoSize = true;
            this._finalizeButton.FlatStyle = FlatStyle.System;
            this._finalizeButton.Location = new Point(1260, 11);
            this._finalizeButton.Name = "_finalizeButton";
            this._finalizeButton.Size = new Size(69, 25);
            this._finalizeButton.TabIndex = 1;
            this._finalizeButton.Text = "Encerrar Inventario";
            this._finalizeButton.UseVisualStyleBackColor = true;
            this._finalizeButton.Click += new System.EventHandler(this.FinalizeButton_Click);
            this._reopenButton.AutoSize = true;
            this._reopenButton.FlatStyle = FlatStyle.System;
            this._reopenButton.Location = new Point(1193, 11);
            this._reopenButton.Name = "_reopenButton";
            this._reopenButton.Size = new Size(61, 25);
            this._reopenButton.TabIndex = 2;
            this._reopenButton.Text = "Reabrir p/ recontar";
            this._reopenButton.UseVisualStyleBackColor = true;
            this._reopenButton.Click += new System.EventHandler(this.ReopenButton_Click);
            this._closeButton.AutoSize = true;
            this._closeButton.FlatStyle = FlatStyle.System;
            this._closeButton.Location = new Point(1137, 11);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new Size(50, 25);
            this._closeButton.TabIndex = 3;
            this._closeButton.Text = "Fechar (F4)";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            this._startButton.AutoSize = true;
            this._startButton.FlatStyle = FlatStyle.System;
            this._startButton.Location = new Point(1084, 11);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new Size(47, 25);
            this._startButton.TabIndex = 4;
            this._startButton.Text = "Iniciar o inventario (F3)";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this.StartButton_Click);
            this._updateButton.AutoSize = true;
            this._updateButton.FlatStyle = FlatStyle.System;
            this._updateButton.Location = new Point(1029, 11);
            this._updateButton.Name = "_updateButton";
            this._updateButton.Size = new Size(49, 25);
            this._updateButton.TabIndex = 5;
            this._updateButton.Text = "Alterar";
            this._updateButton.UseVisualStyleBackColor = true;
            this._updateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            this._saveButton.AutoSize = true;
            this._saveButton.FlatStyle = FlatStyle.System;
            this._saveButton.Location = new Point(973, 11);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new Size(50, 25);
            this._saveButton.TabIndex = 6;
            this._saveButton.Text = "Gravar (F2)";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // InventarioForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1480, 880);
            this.Controls.Add(this._rootLayout);
            this.KeyPreview = true;
            this.MinimumSize = new Size(1240, 760);
            this.Name = "InventarioForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Inventario de Estoque";
            this.KeyDown += new KeyEventHandler(this.OnFormKeyDown);
            ((ISupportInitialize)(this._maxPointsNumeric)).EndInit();
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._headerGroup.ResumeLayout(false);
            this._headerLayout.ResumeLayout(false);
            this._headerLayout.PerformLayout();
            this._headerLine1Layout.ResumeLayout(false);
            this._headerLine1Layout.PerformLayout();
            this._headerLine2Layout.ResumeLayout(false);
            this._headerLine2Layout.PerformLayout();
            this._headerLine3Layout.ResumeLayout(false);
            this._headerLine3Layout.PerformLayout();
            this._planningGroup.ResumeLayout(false);
            this._planningFlow.ResumeLayout(false);
            this._planningFlow.PerformLayout();
            this._centerSplitContainer.Panel1.ResumeLayout(false);
            this._centerSplitContainer.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(this._centerSplitContainer)).EndInit();
            this._centerSplitContainer.ResumeLayout(false);
            this._itemsGroup.ResumeLayout(false);
            ((ISupportInitialize)(this._itemsGrid)).EndInit();
            this._rightRootLayout.ResumeLayout(false);
            this._pointsGroup.ResumeLayout(false);
            ((ISupportInitialize)(this._pointsGrid)).EndInit();
            this._pointActionsFlow.ResumeLayout(false);
            this._pointActionsFlow.PerformLayout();
            this._countsGroup.ResumeLayout(false);
            ((ISupportInitialize)(this._countsGrid)).EndInit();
            this._footerFlow.ResumeLayout(false);
            this._footerFlow.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
