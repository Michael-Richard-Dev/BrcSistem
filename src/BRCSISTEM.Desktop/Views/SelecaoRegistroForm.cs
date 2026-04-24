using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    internal sealed class SelecaoRegistroForm : Form
    {
        private readonly string _descriptionHeader;
        private readonly LookupOption[] _allOptions;

        private TextBox _filterTextBox;
        private DataGridView _grid;

        public SelecaoRegistroForm(string title, string descriptionHeader, LookupOption[] options)
        {
            _descriptionHeader = string.IsNullOrWhiteSpace(descriptionHeader) ? "DESCRICAO" : descriptionHeader.ToUpperInvariant();
            _allOptions = options ?? new LookupOption[0];

            InitializeComponent(title);
            Load += (sender, args) => RefreshGrid();
        }

        public LookupOption SelectedOption { get; private set; }

        private void InitializeComponent(string title)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(760, 500);
            MinimumSize = new Size(680, 420);
            BackColor = Color.White;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var filterPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            filterPanel.Controls.Add(new Label { AutoSize = true, Text = "Buscar:", Margin = new Padding(0, 8, 0, 0), Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) });
            _filterTextBox = new TextBox { Width = 300, Font = new Font("Segoe UI", 10F) };
            _filterTextBox.TextChanged += (sender, args) => RefreshGrid();
            filterPanel.Controls.Add(_filterTextBox);
            filterPanel.Controls.Add(CreateButton("Confirmar", (sender, args) => ConfirmSelection()));
            filterPanel.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Resultados", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
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
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "codigo", HeaderText = "CODIGO", DataPropertyName = nameof(LookupOption.Code), Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "descricao", HeaderText = _descriptionHeader, DataPropertyName = nameof(LookupOption.Description), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "STATUS", DataPropertyName = nameof(LookupOption.Status), Width = 110 });
            _grid.CellDoubleClick += (sender, args) => ConfirmSelection();
            _grid.KeyDown += OnGridKeyDown;
            group.Controls.Add(_grid);

            var footer = new Label
            {
                AutoSize = true,
                Text = "Dica: filtre por codigo ou descricao e pressione Enter para confirmar.",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.DimGray,
                Margin = new Padding(3, 0, 3, 0),
            };

            root.Controls.Add(filterPanel, 0, 0);
            root.Controls.Add(group, 0, 1);
            root.Controls.Add(footer, 0, 2);
            Controls.Add(root);
        }

        private void RefreshGrid()
        {
            var filter = (_filterTextBox.Text ?? string.Empty).Trim();
            var items = string.IsNullOrWhiteSpace(filter)
                ? _allOptions
                : _allOptions.Where(item =>
                    (item.Code ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item.Description ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item.Status ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();

            _grid.DataSource = items;
            if (_grid.Rows.Count > 0)
            {
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private void ConfirmSelection()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is LookupOption option))
            {
                MessageBox.Show(this, "Selecione um registro.", "Informacao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedOption = option;
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
