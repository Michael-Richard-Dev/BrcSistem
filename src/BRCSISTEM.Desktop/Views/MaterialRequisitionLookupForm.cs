using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    internal sealed class MaterialRequisitionLookupForm : Form
    {
        private readonly MaterialRequisitionController _controller;
        private readonly AppConfiguration _configuration;
        private readonly DatabaseProfile _databaseProfile;

        private TextBox _filterTextBox;
        private DataGridView _grid;

        public MaterialRequisitionLookupForm(MaterialRequisitionController controller, AppConfiguration configuration, DatabaseProfile databaseProfile)
        {
            _controller = controller;
            _configuration = configuration;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            Load += (sender, args) => RefreshGrid();
        }

        public MaterialRequisitionSummary SelectedRequisition { get; private set; }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Consultar Requisicoes";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1120, 600);
            MinimumSize = new Size(980, 520);
            BackColor = Color.White;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var filterPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            filterPanel.Controls.Add(new Label { AutoSize = true, Text = "Pesquisar numero, almoxarifado ou materiais:", Margin = new Padding(0, 8, 0, 0), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) });
            _filterTextBox = new TextBox { Width = 360, Font = new Font("Segoe UI", 10F) };
            _filterTextBox.TextChanged += (sender, args) => RefreshGrid();
            filterPanel.Controls.Add(_filterTextBox);
            filterPanel.Controls.Add(CreateButton("Usar", (sender, args) => ConfirmSelection()));
            filterPanel.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Requisicoes", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
            };
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "numero", HeaderText = "REQ.", DataPropertyName = nameof(MaterialRequisitionSummary.Number), Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "data", HeaderText = "DATA/HORA", DataPropertyName = nameof(MaterialRequisitionSummary.MovementDateTimeDisplay), Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "almox", HeaderText = "ALMOXARIFADO", DataPropertyName = nameof(MaterialRequisitionSummary.WarehouseDisplay), Width = 240 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "itens", HeaderText = "ITENS", DataPropertyName = nameof(MaterialRequisitionSummary.ItemCount), Width = 70 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "materiais", HeaderText = "MATERIAIS", DataPropertyName = nameof(MaterialRequisitionSummary.MaterialsPreview), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(MaterialRequisitionSummary.Status), Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "bloqueio", HeaderText = "BLOQUEADO POR", DataPropertyName = nameof(MaterialRequisitionSummary.LockedBy), Width = 140 });
            _grid.CellDoubleClick += (sender, args) => ConfirmSelection();
            _grid.KeyDown += OnGridKeyDown;
            group.Controls.Add(_grid);

            var footer = new Label
            {
                AutoSize = true,
                Text = "Requisicoes ativas podem ser abertas para consulta, alteracao e cancelamento. Requisicoes canceladas abrem somente para leitura.",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.DimGray,
            };

            root.Controls.Add(filterPanel, 0, 0);
            root.Controls.Add(group, 0, 1);
            root.Controls.Add(footer, 0, 2);
            Controls.Add(root);
        }

        private void RefreshGrid()
        {
            var items = _controller.SearchRequisitions(_configuration, _databaseProfile, _filterTextBox.Text);
            _grid.DataSource = items;
            if (_grid.Rows.Count > 0)
            {
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private void ConfirmSelection()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is MaterialRequisitionSummary item))
            {
                MessageBox.Show(this, "Selecione uma requisicao.", "Informacao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedRequisition = item;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void OnGridKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                ConfirmSelection();
            }
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
        }
    }
}
