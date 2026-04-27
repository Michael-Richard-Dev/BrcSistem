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
        // ── Turnos ─────────────────────────────────────────────────────────────
        private TextBox      _shiftNameTextBox;
        private TextBox      _shiftDescriptionTextBox;
        private DataGridView _shiftsGrid;
        private ShiftSummary[] _shifts = new ShiftSummary[0];
        private int? _selectedShiftId;

        // ── Motivos de Requisicao ──────────────────────────────────────────────
        private TextBox      _reasonNameTextBox;
        private TextBox      _reasonDescriptionTextBox;
        private DataGridView _reasonsGrid;
        private RequisitionReasonSummary[] _reasons = new RequisitionReasonSummary[0];
        private int? _selectedReasonId;

        private sealed class CrudRow
        {
            public int    Id          { get; set; }
            public string Name        { get; set; }
            public string Description { get; set; }
            public string ActiveText  { get; set; }
        }

        // ====================================================================
        //  TURNOS
        // ====================================================================

        private void BuildShiftsTab(TabPage page)
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var top = new GroupBox
            {
                Dock    = DockStyle.Top,
                Text    = "Novo Turno",
                Font    = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(10),
                AutoSize = true,
            };
            var topLayout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 3 };
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            topLayout.Controls.Add(CreateFieldLabel("Nome do Turno:"), 0, 0);
            _shiftNameTextBox = new TextBox { Dock = DockStyle.Top, Width = 320, Font = new Font("Segoe UI", 10F) };
            topLayout.Controls.Add(_shiftNameTextBox, 1, 0);

            topLayout.Controls.Add(CreateFieldLabel("Descricao:"), 0, 1);
            _shiftDescriptionTextBox = new TextBox { Dock = DockStyle.Top, Width = 320, Font = new Font("Segoe UI", 10F) };
            topLayout.Controls.Add(_shiftDescriptionTextBox, 1, 1);

            var shiftButtons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            shiftButtons.Controls.Add(CreateButton("Adicionar", (sender, args) => AddShift()));
            shiftButtons.Controls.Add(CreateButton("Atualizar", (sender, args) => UpdateShift()));
            shiftButtons.Controls.Add(CreateButton("Deletar",   (sender, args) => DeleteShift()));
            shiftButtons.Controls.Add(CreateButton("Limpar",    (sender, args) => ClearShiftForm()));
            topLayout.Controls.Add(shiftButtons, 1, 2);

            top.Controls.Add(topLayout);
            root.Controls.Add(top, 0, 0);

            var list = new GroupBox { Dock = DockStyle.Fill, Text = "Turnos Cadastrados", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(10) };
            _shiftsGrid = BuildCrudGrid();
            _shiftsGrid.SelectionChanged += (sender, args) => OnShiftRowSelected();
            list.Controls.Add(_shiftsGrid);
            root.Controls.Add(list, 0, 1);

            page.Controls.Add(root);
        }

        private void LoadShiftsGrid()
        {
            try
            {
                _shifts = (_databaseMaintenanceController.LoadShifts(_configuration, _databaseProfile) ?? new ShiftSummary[0]).ToArray();
                var rows = _shifts.Select(s => new CrudRow
                {
                    Id          = s.Id,
                    Name        = s.Name ?? string.Empty,
                    Description = s.Description ?? string.Empty,
                    ActiveText  = s.IsActive ? "Ativo" : "Inativo",
                }).ToList();
                _shiftsGrid.DataSource = rows;
                if (_shiftsGrid.Rows.Count > 0) _shiftsGrid.ClearSelection();
                _selectedShiftId = null;
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void OnShiftRowSelected()
        {
            if (_shiftsGrid.SelectedRows.Count == 0)
            {
                return;
            }
            var row = _shiftsGrid.SelectedRows[0].DataBoundItem as CrudRow;
            if (row == null)
            {
                return;
            }
            _selectedShiftId              = row.Id;
            _shiftNameTextBox.Text        = row.Name;
            _shiftDescriptionTextBox.Text = row.Description;
        }

        private void AddShift()
        {
            var name = (_shiftNameTextBox.Text ?? string.Empty).Trim();
            var desc = (_shiftDescriptionTextBox.Text ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                MessageBox.Show(this, "Informe o nome do turno", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _databaseMaintenanceController.AddShift(_configuration, _databaseProfile, _identity.UserName, name, desc);
                MessageBox.Show(this, "Turno '" + name + "' adicionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearShiftForm();
                LoadShiftsGrid();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateShift()
        {
            if (!_selectedShiftId.HasValue)
            {
                MessageBox.Show(this, "Selecione um turno para atualizar", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var name = (_shiftNameTextBox.Text ?? string.Empty).Trim();
            var desc = (_shiftDescriptionTextBox.Text ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                MessageBox.Show(this, "Informe o nome do turno", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _databaseMaintenanceController.UpdateShift(_configuration, _databaseProfile, _identity.UserName, _selectedShiftId.Value, name, desc);
                MessageBox.Show(this, "Turno '" + name + "' atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearShiftForm();
                LoadShiftsGrid();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void DeleteShift()
        {
            if (!_selectedShiftId.HasValue)
            {
                MessageBox.Show(this, "Selecione um turno para deletar", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var name = (_shiftNameTextBox.Text ?? string.Empty).Trim();
            if (MessageBox.Show(this, "Deseja deletar o turno '" + name + "'?", "Confirmacao",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                _databaseMaintenanceController.DeleteShift(_configuration, _databaseProfile, _identity.UserName, _selectedShiftId.Value, name);
                MessageBox.Show(this, "Turno deletado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearShiftForm();
                LoadShiftsGrid();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ClearShiftForm()
        {
            _shiftNameTextBox.Text        = string.Empty;
            _shiftDescriptionTextBox.Text = string.Empty;
            _selectedShiftId              = null;
            if (_shiftsGrid.Rows.Count > 0)
            {
                _shiftsGrid.ClearSelection();
            }
        }

        // ====================================================================
        //  MOTIVOS DE REQUISICAO
        // ====================================================================

        private void BuildReasonsTab(TabPage page)
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var top = new GroupBox
            {
                Dock    = DockStyle.Top,
                Text    = "Novo Motivo",
                Font    = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(10),
                AutoSize = true,
            };
            var topLayout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 3 };
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            topLayout.Controls.Add(CreateFieldLabel("Nome do Motivo:"), 0, 0);
            _reasonNameTextBox = new TextBox { Dock = DockStyle.Top, Width = 320, Font = new Font("Segoe UI", 10F) };
            topLayout.Controls.Add(_reasonNameTextBox, 1, 0);

            topLayout.Controls.Add(CreateFieldLabel("Descricao:"), 0, 1);
            _reasonDescriptionTextBox = new TextBox { Dock = DockStyle.Top, Width = 320, Font = new Font("Segoe UI", 10F) };
            topLayout.Controls.Add(_reasonDescriptionTextBox, 1, 1);

            var reasonButtons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
            reasonButtons.Controls.Add(CreateButton("Adicionar", (sender, args) => AddReason()));
            reasonButtons.Controls.Add(CreateButton("Atualizar", (sender, args) => UpdateReason()));
            reasonButtons.Controls.Add(CreateButton("Deletar",   (sender, args) => DeleteReason()));
            reasonButtons.Controls.Add(CreateButton("Limpar",    (sender, args) => ClearReasonForm()));
            topLayout.Controls.Add(reasonButtons, 1, 2);

            top.Controls.Add(topLayout);
            root.Controls.Add(top, 0, 0);

            var list = new GroupBox { Dock = DockStyle.Fill, Text = "Motivos Cadastrados", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(10) };
            _reasonsGrid = BuildCrudGrid();
            _reasonsGrid.SelectionChanged += (sender, args) => OnReasonRowSelected();
            list.Controls.Add(_reasonsGrid);
            root.Controls.Add(list, 0, 1);

            page.Controls.Add(root);
        }

        private void LoadReasonsGrid()
        {
            try
            {
                _reasons = (_databaseMaintenanceController.LoadRequisitionReasons(_configuration, _databaseProfile) ?? new RequisitionReasonSummary[0]).ToArray();
                var rows = _reasons.Select(r => new CrudRow
                {
                    Id          = r.Id,
                    Name        = r.Name ?? string.Empty,
                    Description = r.Description ?? string.Empty,
                    ActiveText  = r.IsActive ? "Ativo" : "Inativo",
                }).ToList();
                _reasonsGrid.DataSource = rows;
                if (_reasonsGrid.Rows.Count > 0) _reasonsGrid.ClearSelection();
                _selectedReasonId = null;
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void OnReasonRowSelected()
        {
            if (_reasonsGrid.SelectedRows.Count == 0)
            {
                return;
            }
            var row = _reasonsGrid.SelectedRows[0].DataBoundItem as CrudRow;
            if (row == null)
            {
                return;
            }
            _selectedReasonId              = row.Id;
            _reasonNameTextBox.Text        = row.Name;
            _reasonDescriptionTextBox.Text = row.Description;
        }

        private void AddReason()
        {
            var name = (_reasonNameTextBox.Text ?? string.Empty).Trim();
            var desc = (_reasonDescriptionTextBox.Text ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                MessageBox.Show(this, "Informe o nome do motivo", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _databaseMaintenanceController.AddRequisitionReason(_configuration, _databaseProfile, _identity.UserName, name, desc);
                MessageBox.Show(this, "Motivo '" + name + "' adicionado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearReasonForm();
                LoadReasonsGrid();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateReason()
        {
            if (!_selectedReasonId.HasValue)
            {
                MessageBox.Show(this, "Selecione um motivo para atualizar", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var name = (_reasonNameTextBox.Text ?? string.Empty).Trim();
            var desc = (_reasonDescriptionTextBox.Text ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                MessageBox.Show(this, "Informe o nome do motivo", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                _databaseMaintenanceController.UpdateRequisitionReason(_configuration, _databaseProfile, _identity.UserName, _selectedReasonId.Value, name, desc);
                MessageBox.Show(this, "Motivo '" + name + "' atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearReasonForm();
                LoadReasonsGrid();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void DeleteReason()
        {
            if (!_selectedReasonId.HasValue)
            {
                MessageBox.Show(this, "Selecione um motivo para deletar", "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var name = (_reasonNameTextBox.Text ?? string.Empty).Trim();
            if (MessageBox.Show(this, "Deseja deletar o motivo '" + name + "'?", "Confirmacao",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            try
            {
                _databaseMaintenanceController.DeleteRequisitionReason(_configuration, _databaseProfile, _identity.UserName, _selectedReasonId.Value, name);
                MessageBox.Show(this, "Motivo deletado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearReasonForm();
                LoadReasonsGrid();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ClearReasonForm()
        {
            _reasonNameTextBox.Text        = string.Empty;
            _reasonDescriptionTextBox.Text = string.Empty;
            _selectedReasonId              = null;
            if (_reasonsGrid.Rows.Count > 0)
            {
                _reasonsGrid.ClearSelection();
            }
        }

        // ====================================================================
        //  COMPARTILHADO
        // ====================================================================

        private static DataGridView BuildCrudGrid()
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
                Name = "name", HeaderText = "Nome", DataPropertyName = "Name", Width = 180, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "description", HeaderText = "Descricao", DataPropertyName = "Description",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "active", HeaderText = "Ativo", DataPropertyName = "ActiveText", Width = 90, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            return grid;
        }
    }
}
