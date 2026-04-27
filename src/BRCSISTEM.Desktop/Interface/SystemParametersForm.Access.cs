using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class SystemParametersForm
    {
        private ComboBox     _accessUserCombo;
        private DataGridView _accessAvailableGrid;
        private DataGridView _accessGrantedGrid;

        private UserSummary[]      _accessUsers      = new UserSummary[0];
        private sealed class AccessUserItem
        {
            public string UserName    { get; set; }
            public string DisplayName { get; set; }
            public string Label       { get { return (DisplayName ?? string.Empty) + " (" + (UserName ?? string.Empty) + ")"; } }
        }

        private sealed class WarehouseRow
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        private void BuildAccessTab(TabPage page)
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Topo: selecao de usuario
            var top = new GroupBox
            {
                Dock    = DockStyle.Top,
                Text    = "Selecionar Usuario",
                Font    = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(10),
                Height  = 70,
            };
            var topRow = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = false };
            topRow.Controls.Add(CreateFieldLabel("Usuario:"));
            _accessUserCombo = new ComboBox
            {
                Width         = 420,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10F),
            };
            _accessUserCombo.SelectedIndexChanged += (sender, args) => OnAccessUserChanged();
            topRow.Controls.Add(_accessUserCombo);
            topRow.Controls.Add(CreateButton("Atualizar", (sender, args) => { LoadAccessUsers(); LoadAccessWarehouses(); }));
            top.Controls.Add(topRow);
            root.Controls.Add(top, 0, 0);

            // Meio: dois grids + botoes
            var middle = new GroupBox
            {
                Dock    = DockStyle.Fill,
                Text    = "Controle de Acesso",
                Font    = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(10),
            };
            var middleLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            middleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            middleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            middleLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));

            var leftGroup = new GroupBox { Dock = DockStyle.Fill, Text = "Almoxarifados Disponiveis", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            _accessAvailableGrid = BuildWarehouseGrid();
            leftGroup.Controls.Add(_accessAvailableGrid);

            var centerButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true, Padding = new Padding(6, 60, 6, 0) };
            centerButtons.Controls.Add(CreateButton("Conceder acesso  >>", (sender, args) => GrantAccess()));
            centerButtons.Controls.Add(CreateButton("<<  Revogar acesso",  (sender, args) => RevokeAccess()));

            var rightGroup = new GroupBox { Dock = DockStyle.Fill, Text = "Almoxarifados com Acesso", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            _accessGrantedGrid = BuildWarehouseGrid();
            rightGroup.Controls.Add(_accessGrantedGrid);

            middleLayout.Controls.Add(leftGroup,     0, 0);
            middleLayout.Controls.Add(centerButtons, 1, 0);
            middleLayout.Controls.Add(rightGroup,    2, 0);
            middle.Controls.Add(middleLayout);
            root.Controls.Add(middle, 0, 1);

            // Rodape: informacoes
            var info = new GroupBox
            {
                Dock    = DockStyle.Bottom,
                Text    = "Informacoes",
                Font    = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Padding = new Padding(10),
                Height  = 120,
            };
            info.Controls.Add(new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                Text = "- Selecione um usuario acima para gerenciar seus acessos aos almoxarifados\r\n"
                     + "- Use os botoes para conceder ou revogar acesso\r\n"
                     + "- IMPORTANTE: Usuarios SEM acessos configurados terao acesso a TODOS os almoxarifados\r\n"
                     + "- Usuarios COM acessos configurados terao acesso APENAS aos almoxarifados listados a direita\r\n"
                     + "- As telas de movimentacao filtrarao automaticamente os almoxarifados disponiveis",
            });
            root.Controls.Add(info, 0, 2);

            page.Controls.Add(root);
        }

        private static DataGridView BuildWarehouseGrid()
        {
            var grid = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                ReadOnly              = true,
                AutoGenerateColumns   = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                MultiSelect           = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible     = false,
                BackgroundColor       = Color.White,
            };
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "code", HeaderText = "Codigo", DataPropertyName = "Code", Width = 90, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "name", HeaderText = "Nome", DataPropertyName = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            return grid;
        }

        private void LoadAccessUsers()
        {
            try
            {
                var rawUsers = _administrationController.LoadUsers(_configuration, _databaseProfile) ?? new UserSummary[0];
                _accessUsers = rawUsers
                    .Where(u => u != null && !string.Equals(u.Status, "Inativo", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(u => u.DisplayName ?? u.UserName, StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                var items = _accessUsers
                    .Select(u => new AccessUserItem { UserName = u.UserName, DisplayName = u.DisplayName ?? u.UserName })
                    .ToArray();

                _accessUserCombo.BeginUpdate();
                _accessUserCombo.DataSource    = null;
                _accessUserCombo.DisplayMember = nameof(AccessUserItem.Label);
                _accessUserCombo.ValueMember   = nameof(AccessUserItem.UserName);
                _accessUserCombo.DataSource    = items;
                _accessUserCombo.SelectedIndex = -1;
                _accessUserCombo.EndUpdate();

                ClearAccessGrids();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadAccessWarehouses()
        {
            // Mantido apenas para compatibilidade com o bootstrap inicial da tela.
            // A carga efetiva dos grids segue o Python e acontece em OnAccessUserChanged
            // via usuario_almoxarifados + almoxarifados ativos.
        }

        private void OnAccessUserChanged()
        {
            var user = _accessUserCombo.SelectedItem as AccessUserItem;
            if (user == null || string.IsNullOrWhiteSpace(user.UserName))
            {
                ClearAccessGrids();
                return;
            }

            try
            {
                var granted = _databaseMaintenanceController.LoadGrantedWarehouseAccess(_configuration, _databaseProfile, user.UserName)
                    ?? (IReadOnlyCollection<WarehouseAccessEntry>)new WarehouseAccessEntry[0];
                var available = _databaseMaintenanceController.LoadAvailableWarehousesForUser(_configuration, _databaseProfile, user.UserName)
                    ?? (IReadOnlyCollection<WarehouseAccessEntry>)new WarehouseAccessEntry[0];

                _accessAvailableGrid.DataSource = available
                    .Select(w => new WarehouseRow { Code = w.WarehouseCode, Name = w.WarehouseName })
                    .ToList();
                _accessGrantedGrid.DataSource = granted
                    .Select(w => new WarehouseRow { Code = w.WarehouseCode, Name = w.WarehouseName })
                    .ToList();
                if (_accessAvailableGrid.Rows.Count > 0) _accessAvailableGrid.ClearSelection();
                if (_accessGrantedGrid.Rows.Count   > 0) _accessGrantedGrid.ClearSelection();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ClearAccessGrids()
        {
            _accessAvailableGrid.DataSource = new List<WarehouseRow>();
            _accessGrantedGrid.DataSource   = new List<WarehouseRow>();
        }

        private void GrantAccess()
        {
            var user = _accessUserCombo.SelectedItem as AccessUserItem;
            if (user == null)
            {
                MessageBox.Show(this, "Selecione um usuario primeiro.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var code = GetSelectedWarehouseCode(_accessAvailableGrid);
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show(this, "Selecione um almoxarifado na lista de disponiveis.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _databaseMaintenanceController.GrantWarehouseAccess(_configuration, _databaseProfile, _identity.UserName, user.UserName, code);
                OnAccessUserChanged();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void RevokeAccess()
        {
            var user = _accessUserCombo.SelectedItem as AccessUserItem;
            if (user == null)
            {
                MessageBox.Show(this, "Selecione um usuario primeiro.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var code = GetSelectedWarehouseCode(_accessGrantedGrid);
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show(this, "Selecione um almoxarifado na lista com acesso.", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _databaseMaintenanceController.RevokeWarehouseAccess(_configuration, _databaseProfile, _identity.UserName, user.UserName, code);
                OnAccessUserChanged();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private static string GetSelectedWarehouseCode(DataGridView grid)
        {
            if (grid == null || grid.SelectedRows.Count == 0)
            {
                return null;
            }
            var row = grid.SelectedRows[0].DataBoundItem as WarehouseRow;
            return row != null ? row.Code : null;
        }
    }
}
