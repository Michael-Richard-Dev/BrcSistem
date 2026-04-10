using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    internal sealed class LotDuplicateConfirmationForm : Form
    {
        public LotDuplicateConfirmationForm(string lotName, LotSummary[] duplicates)
        {
            InitializeComponent(lotName, duplicates ?? new LotSummary[0]);
        }

        private void InitializeComponent(string lotName, LotSummary[] duplicates)
        {
            Text = "Verificacao de Lotes Duplicados";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(820, 480);
            MinimumSize = new Size(760, 420);
            BackColor = Color.White;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var warningLabel = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Text = "Ja existem lotes ativos com o nome '" + lotName + "'.\nConfira os registros encontrados antes de continuar.",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.Firebrick,
            };

            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Lotes Existentes com Este Nome", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var grid = new DataGridView
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
                DataSource = duplicates,
            };
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "CODIGO + LOTE", DataPropertyName = nameof(LotSummary.Code), Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "FORNECEDOR", DataPropertyName = nameof(LotSummary.SupplierDisplay), Width = 220, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "MATERIAL", DataPropertyName = nameof(LotSummary.MaterialDisplay), Width = 220, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "VALIDADE", DataPropertyName = nameof(LotSummary.ExpirationDate), Width = 110 });
            group.Controls.Add(grid);

            var actions = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true };
            actions.Controls.Add(CreateButton("Continuar", (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            }));
            actions.Controls.Add(CreateButton("Cancelar", (sender, args) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }));

            root.Controls.Add(warningLabel, 0, 0);
            root.Controls.Add(group, 0, 1);
            root.Controls.Add(actions, 0, 2);
            Controls.Add(root);
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
        }
    }
}
