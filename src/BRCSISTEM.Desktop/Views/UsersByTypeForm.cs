using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class UsersByTypeForm : Form
    {
        private readonly UserSummary[] _users;
        private readonly string _typeName;
        private DataGridView _grid;

        public UsersByTypeForm(string typeName, UserSummary[] users)
        {
            _typeName = typeName;
            _users = users ?? new UserSummary[0];
            InitializeComponent();
            LoadUsers();
        }

        public string SelectedUserName { get; private set; }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Usuarios do Tipo";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(720, 420);
            MinimumSize = new Size(720, 420);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                Padding = new Padding(12),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var header = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Text = "Usuarios do tipo '" + _typeName + "' (" + _users.Length + " encontrado(s))",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 0, 0, 10),
            };

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
            };
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "usuario", HeaderText = "Usuario", DataPropertyName = nameof(UserSummary.UserName), Width = 140 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "nome", HeaderText = "Nome Completo", DataPropertyName = nameof(UserSummary.DisplayName), Width = 340, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "Status", DataPropertyName = nameof(UserSummary.Status), Width = 120 });
            _grid.CellDoubleClick += (sender, args) => ConfirmSelection();

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            var openButton = new Button { Text = "Abrir Cadastro", AutoSize = true, FlatStyle = FlatStyle.System };
            openButton.Click += (sender, args) => ConfirmSelection();
            var closeButton = new Button { Text = "Fechar", AutoSize = true, FlatStyle = FlatStyle.System };
            closeButton.Click += (sender, args) => Close();
            buttons.Controls.Add(openButton);
            buttons.Controls.Add(closeButton);

            root.Controls.Add(header, 0, 0);
            root.Controls.Add(_grid, 0, 1);
            root.Controls.Add(buttons, 0, 2);
            Controls.Add(root);
        }

        private void LoadUsers()
        {
            _grid.DataSource = _users;
        }

        private void ConfirmSelection()
        {
            if (_grid.CurrentRow == null || !(_grid.CurrentRow.DataBoundItem is UserSummary user))
            {
                return;
            }

            SelectedUserName = user.UserName;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
