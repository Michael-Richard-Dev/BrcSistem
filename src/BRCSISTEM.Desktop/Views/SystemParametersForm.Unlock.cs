using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class SystemParametersForm
    {
        private ComboBox     _unlockTableCombo;
        private TextBox      _unlockNumberTextBox;
        private TextBox      _unlockSupplierTextBox;
        private DataGridView _unlockGrid;

        private sealed class LockedRow
        {
            public string Table         { get; set; }
            public string Key           { get; set; }
            public string DocumentNumber{ get; set; }
            public string Supplier      { get; set; }
            public string LockedBy      { get; set; }
            public string LockedAt      { get; set; }
        }

        private void BuildUnlockTab(TabPage page)
        {
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Info
            var info = new GroupBox
            {
                Dock    = DockStyle.Top,
                Text    = "Informacoes",
                Font    = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(10),
                AutoSize = true,
                Height  = 150,
            };
            info.Controls.Add(new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F),
                Text = "BLOQUEIOS EM REGISTROS\r\n\r\n"
                     + "Quando um usuario abre um movimento para edicao (nota, transferencia, saida producao ou requisicao),\r\n"
                     + "o registro fica bloqueado. Se a aplicacao fechar sem salvar/cancelar, o bloqueio pode permanecer\r\n"
                     + "no banco e travar novas operacoes.\r\n\r\n"
                     + "Esta ferramenta permite ao ADMINISTRADOR remover bloqueios orfaos e gerar auditoria automatica.",
            });
            root.Controls.Add(info, 0, 0);

            // Filtros
            var filters = new GroupBox
            {
                Dock    = DockStyle.Top,
                Text    = "Filtrar Registros Bloqueados (Opcional)",
                Font    = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(12),
                AutoSize = true,
            };
            var filtersRow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true };

            filtersRow.Controls.Add(CreateFieldLabel("Tipo:"));
            _unlockTableCombo = new ComboBox
            {
                Width         = 160,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10F),
                Margin        = new Padding(0, 6, 12, 4),
            };
            _unlockTableCombo.Items.AddRange(new object[] { "notas", "transferencias", "saidas_producao", "requisicoes" });
            _unlockTableCombo.SelectedItem = "transferencias";
            _unlockTableCombo.SelectedIndexChanged += (sender, args) => LoadLockedRecords(false);
            filtersRow.Controls.Add(_unlockTableCombo);

            filtersRow.Controls.Add(CreateFieldLabel("Numero:"));
            _unlockNumberTextBox = new TextBox { Width = 160, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 6, 12, 4) };
            filtersRow.Controls.Add(_unlockNumberTextBox);

            filtersRow.Controls.Add(CreateFieldLabel("Fornecedor (apenas notas):"));
            _unlockSupplierTextBox = new TextBox { Width = 180, Font = new Font("Segoe UI", 10F), Margin = new Padding(0, 6, 12, 4) };
            filtersRow.Controls.Add(_unlockSupplierTextBox);

            filtersRow.Controls.Add(CreateButton("Aplicar Filtro", (sender, args) => LoadLockedRecords(true)));
            filters.Controls.Add(filtersRow);
            root.Controls.Add(filters, 0, 1);

            // Grid
            var result = new GroupBox
            {
                Dock    = DockStyle.Fill,
                Text    = "Registros Bloqueados Encontrados",
                Font    = new Font("Segoe UI", 10F, FontStyle.Bold),
                Padding = new Padding(10),
            };
            _unlockGrid = new DataGridView
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
            _unlockGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "table", HeaderText = "Tabela", DataPropertyName = "Table", Width = 130, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _unlockGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "key", HeaderText = "Chave", DataPropertyName = "Key", Width = 240, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _unlockGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "by", HeaderText = "Bloqueado Por", DataPropertyName = "LockedBy", Width = 160, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _unlockGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "at", HeaderText = "Bloqueado Em", DataPropertyName = "LockedAt",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            result.Controls.Add(_unlockGrid);
            root.Controls.Add(result, 0, 2);

            // Botoes
            var actions = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 6, 0, 0) };
            actions.Controls.Add(CreateButton("Desbloquear Selecionado", (sender, args) => UnlockSelected()));
            actions.Controls.Add(CreateButton("Desbloquear TODOS Encontrados", (sender, args) => UnlockAll()));
            actions.Controls.Add(CreateButton("Limpar Pesquisa", (sender, args) => ClearUnlockSearch()));
            root.Controls.Add(actions, 0, 3);

            page.Controls.Add(root);
        }

        private string GetCurrentUnlockTable()
        {
            var tbl = (_unlockTableCombo.SelectedItem as string ?? string.Empty).Trim().ToLowerInvariant();
            if (tbl == "notas" || tbl == "transferencias" || tbl == "saidas_producao" || tbl == "requisicoes")
            {
                return tbl;
            }
            return "transferencias";
        }

        private void LoadLockedRecords(bool showMessage)
        {
            try
            {
                var table    = GetCurrentUnlockTable();
                var number   = (_unlockNumberTextBox.Text   ?? string.Empty).Trim();
                var supplier = (_unlockSupplierTextBox.Text ?? string.Empty).Trim();
                if (!string.Equals(table, "notas", StringComparison.OrdinalIgnoreCase))
                {
                    supplier = string.Empty;
                }

                var data = _databaseMaintenanceController.LoadLockedRecords(_configuration, _databaseProfile, table, number, supplier)
                    ?? (IReadOnlyCollection<OpenMovementLockSummary>)new OpenMovementLockSummary[0];

                var rows = data.Select(lk => new LockedRow
                {
                    Table          = table,
                    DocumentNumber = lk.DocumentNumber ?? string.Empty,
                    Supplier       = lk.Supplier ?? string.Empty,
                    Key            = BuildUnlockKey(table, lk.DocumentNumber, lk.Supplier),
                    LockedBy       = lk.UserName ?? string.Empty,
                    LockedAt       = lk.LockedAt ?? string.Empty,
                }).ToList();

                _unlockGrid.DataSource = rows;
                if (_unlockGrid.Rows.Count > 0) _unlockGrid.ClearSelection();

                if (showMessage)
                {
                    if (rows.Count == 0)
                    {
                        MessageBox.Show(this, "Nenhum registro bloqueado encontrado.", "Resultado",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (!string.IsNullOrEmpty(number) || !string.IsNullOrEmpty(supplier))
                    {
                        MessageBox.Show(this, rows.Count + " registro(s) bloqueado(s) encontrado(s).", "Resultado",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private static string BuildUnlockKey(string table, string number, string supplier)
        {
            var n = (number ?? string.Empty).Trim();
            if (string.Equals(table, "notas", StringComparison.OrdinalIgnoreCase))
            {
                return n + " - " + (supplier ?? string.Empty).Trim();
            }
            return n;
        }

        private void UnlockSelected()
        {
            if (_unlockGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Selecione um registro para desbloquear.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _unlockGrid.SelectedRows[0].DataBoundItem as LockedRow;
            if (row == null)
            {
                return;
            }

            var message = "Desbloquear este registro?\r\n\r\n"
                        + "Tabela: " + row.Table + "\r\n"
                        + "Chave: " + row.Key + "\r\n"
                        + "Bloqueado por: " + row.LockedBy + "\r\n"
                        + "Desde: " + row.LockedAt + "\r\n\r\n"
                        + "Acao sera registrada em auditoria.";
            if (MessageBox.Show(this, message, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _databaseMaintenanceController.UnlockRecord(_configuration, _databaseProfile, _identity.UserName,
                    row.Table, row.DocumentNumber, row.Supplier);
                MessageBox.Show(this, "Registro desbloqueado com sucesso.", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadLockedRecords(false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UnlockAll()
        {
            var rows = new List<LockedRow>();
            foreach (DataGridViewRow gridRow in _unlockGrid.Rows)
            {
                var r = gridRow.DataBoundItem as LockedRow;
                if (r != null)
                {
                    rows.Add(r);
                }
            }

            if (rows.Count == 0)
            {
                MessageBox.Show(this, "Nenhum registro para desbloquear.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var message = "Desbloquear TODOS os " + rows.Count + " registro(s) encontrado(s)?\r\n\r\n"
                        + "Esta acao e irreversivel e sera registrada em auditoria.";
            if (MessageBox.Show(this, message, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            int success = 0;
            int failures = 0;
            foreach (var row in rows)
            {
                try
                {
                    _databaseMaintenanceController.UnlockRecord(_configuration, _databaseProfile, _identity.UserName,
                        row.Table, row.DocumentNumber, row.Supplier);
                    success++;
                }
                catch
                {
                    failures++;
                }
            }

            MessageBox.Show(this,
                "Desbloqueados: " + success + "\r\nErros: " + failures,
                "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadLockedRecords(false);
        }

        private void ClearUnlockSearch()
        {
            _unlockNumberTextBox.Text   = string.Empty;
            _unlockSupplierTextBox.Text = string.Empty;
            _unlockTableCombo.SelectedItem = "transferencias";
            _unlockGrid.DataSource = new List<LockedRow>();
        }
    }
}
