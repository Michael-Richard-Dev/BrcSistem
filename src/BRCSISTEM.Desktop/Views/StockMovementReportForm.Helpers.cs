using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class StockMovementReportForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                ApplyDefaultPeriod();
                LoadReferences();
                _grid.DataSource = new StockMovementReportRow[0];
                UpdateSummary();
                SetStatus("Use os filtros e clique em 'Filtrar' (ou F5) para consultar os dados.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadReferences()
        {
            _warehouseOptions = IncludeBlank(ToLookupOptions(_stockMovementReportController.LoadWarehouses(_configuration, _databaseProfile)));
            BindCombo(_warehouseComboBox, _warehouseOptions);

            _materialOptions = IncludeBlank(ToLookupOptions(_stockMovementReportController.LoadMaterials(_configuration, _databaseProfile)));
            BindCombo(_materialComboBox, _materialOptions);

            _lotOptions = IncludeBlank(ToLookupOptions(_stockMovementReportController.LoadLots(_configuration, _databaseProfile)));
            BindCombo(_lotComboBox, _lotOptions);
        }

        private void QueryRows()
        {
            try
            {
                _rows = _stockMovementReportController.LoadRows(_configuration, _databaseProfile, BuildQuery());
                _sortColumn = string.Empty;
                _sortAscending = true;
                BindGrid();
                UpdateSummary();
                SetStatus(_rows.Length == 0 ? "Nenhum registro encontrado para os filtros informados." : "Consulta concluida com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void BindGrid()
        {
            _grid.DataSource = BuildDisplayRows();
            if (_grid.Rows.Count > 0)
            {
                _grid.ClearSelection();
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private StockMovementReportRow[] BuildDisplayRows()
        {
            var orderedRows = SortRows(_rows);
            if (orderedRows.Length == 0)
            {
                return orderedRows;
            }

            return orderedRows
                .Concat(new[] { StockMovementReportRow.CreateTotal(_rows) })
                .ToArray();
        }

        private void UpdateSummary()
        {
            if (_rows.Length == 0)
            {
                _summaryLabel.Text = "Total de registros: 0";
                return;
            }

            var total = StockMovementReportRow.CreateTotal(_rows);
            _summaryLabel.Text = "Total de registros: " + _rows.Length
                + " | Saldo Inicial: " + total.OpeningBalanceText
                + " | Entradas: " + total.EntriesText
                + " | Saldo Final: " + total.FinalBalanceText;
        }

        private void ClearFilters()
        {
            ApplyDefaultPeriod();
            _onlyMovementOrBalanceCheckBox.Checked = false;
            SelectFirstOption(_warehouseComboBox);
            SelectFirstOption(_materialComboBox);
            SelectFirstOption(_lotComboBox);
            QueryRows();
        }

        private void OpenWarehouseLookup()
        {
            using (var dialog = new AlmoxarifadoSelecaoForm(_warehouseOptions.Where(item => !string.IsNullOrWhiteSpace(item.Code)).ToArray()))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_warehouseComboBox, _warehouseOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenMaterialLookup()
        {
            using (var dialog = new MaterialSelecaoForm(_materialOptions.Where(item => !string.IsNullOrWhiteSpace(item.Code)).ToArray()))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_materialComboBox, _materialOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenLotLookup()
        {
            using (var dialog = new LoteSelecaoForm(_lotOptions.Where(item => !string.IsNullOrWhiteSpace(item.Code)).ToArray()))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_lotComboBox, _lotOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void ExportCsv()
        {
            if (_rows.Length == 0)
            {
                MessageBox.Show(this, "Nao ha dados para exportar.", "Relatorio de Movimentacao de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio CSV";
                    dialog.Filter = "CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "csv";
                    dialog.FileName = "relatorio_movimentacao_estoque_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine("Almoxarifado;Lote;Material;Validade;Saldo Inicial;Entrada (+);Transf (+);Transf (-);Saida Prod (-);Requisicao (-);Inventario (+/-);Saldo Final (=)");
                        foreach (var row in _rows)
                        {
                            writer.WriteLine(string.Join(";", new[]
                            {
                                EscapeCsv(row.WarehouseDisplay),
                                EscapeCsv(row.LotDisplay),
                                EscapeCsv(row.MaterialDisplay),
                                EscapeCsv(row.ExpirationDateDisplay),
                                EscapeCsv(row.OpeningBalanceText),
                                EscapeCsv(row.EntriesText),
                                EscapeCsv(row.TransferInText),
                                EscapeCsv(row.TransferOutText),
                                EscapeCsv(row.ProductionOutputText),
                                EscapeCsv(row.RequisitionText),
                                EscapeCsv(row.InventoryText),
                                EscapeCsv(row.FinalBalanceText),
                            }));
                        }
                    }

                    _stockMovementReportController.RegisterCsvExport(_configuration, _databaseProfile, _identity.UserName, BuildQuery(), _rows.Length);
                    SetStatus("Arquivo CSV exportado com sucesso.", false);
                    MessageBox.Show(this, "Dados exportados para:\n" + dialog.FileName, "Relatorio de Movimentacao de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ExportPdf()
        {
            if (_rows.Length == 0)
            {
                MessageBox.Show(this, "Nao ha dados para gerar o PDF.", "Relatorio de Movimentacao de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio PDF";
                    dialog.Filter = "PDF (*.pdf)|*.pdf|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "pdf";
                    dialog.FileName = "relatorio_movimentacao_estoque_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    StockMovementReportPdfExporter.Export(dialog.FileName, BuildFilterLines(), _rows);
                    _stockMovementReportController.RegisterPdfExport(_configuration, _databaseProfile, _identity.UserName, BuildQuery(), _rows.Length);
                    SetStatus("PDF gerado com sucesso.", false);

                    if (MessageBox.Show(this, "PDF salvo com sucesso.\n\nDeseja abrir agora?", "Relatorio de Movimentacao de Estoque", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(dialog.FileName);
                    }
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private string[] BuildFilterLines()
        {
            return new[]
            {
                "Periodo: " + (_startDateTextBox.Text ?? string.Empty) + " ate " + (_endDateTextBox.Text ?? string.Empty),
                "Almoxarifado: " + GetSelectedDisplayText(_warehouseComboBox),
                "Material: " + GetSelectedDisplayText(_materialComboBox),
                "Lote: " + GetSelectedDisplayText(_lotComboBox),
                "Somente com movimentacao ou saldo > 0: " + (_onlyMovementOrBalanceCheckBox.Checked ? "SIM" : "NAO"),
                "Total de registros: " + _rows.Length,
            };
        }

        private void OnGridColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var column = _grid.Columns[e.ColumnIndex].Name;
            _sortAscending = string.Equals(_sortColumn, column, StringComparison.OrdinalIgnoreCase) ? !_sortAscending : true;
            _sortColumn = column;
            BindGrid();
        }

        private void OnGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_grid.Rows[e.RowIndex].DataBoundItem is StockMovementReportRow row))
            {
                return;
            }

            var style = _grid.Rows[e.RowIndex].DefaultCellStyle;
            style.Font = row.IsTotalRow ? _totalRowFont : _grid.Font;

            if (row.IsTotalRow)
            {
                style.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
                style.ForeColor = System.Drawing.Color.Black;
                return;
            }

            style.ForeColor = System.Drawing.Color.Black;
            if (string.Equals(row.ExpirationRiskCategory, "CRITICO", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = System.Drawing.Color.FromArgb(204, 0, 0);
                style.ForeColor = System.Drawing.Color.White;
            }
            else if (string.Equals(row.ExpirationRiskCategory, "ALERTA", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = System.Drawing.Color.FromArgb(255, 215, 0);
            }
            else if (string.Equals(row.ExpirationRiskCategory, "ATENCAO", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = System.Drawing.Color.FromArgb(135, 206, 235);
            }
            else
            {
                style.BackColor = e.RowIndex % 2 == 0 ? System.Drawing.Color.White : System.Drawing.Color.FromArgb(245, 248, 250);
            }
        }

        private void ApplyDefaultPeriod()
        {
            var today = DateTime.Now;
            var start = new DateTime(today.Year, today.Month, 1);
            _startDateTextBox.Text = start.ToString("dd/MM/yyyy");
            _endDateTextBox.Text = today.ToString("dd/MM/yyyy");
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                QueryRows();
            }
        }

        private StockMovementReportQuery BuildQuery()
        {
            return new StockMovementReportQuery
            {
                StartDate = _startDateTextBox.Text,
                EndDate = _endDateTextBox.Text,
                WarehouseCode = GetSelectedCode(_warehouseComboBox),
                MaterialCode = GetSelectedCode(_materialComboBox),
                LotCode = GetSelectedCode(_lotComboBox),
                OnlyRowsWithMovementOrPositiveBalance = _onlyMovementOrBalanceCheckBox.Checked,
            };
        }

        private StockMovementReportRow[] SortRows(StockMovementReportRow[] items)
        {
            var source = items ?? new StockMovementReportRow[0];
            if (string.IsNullOrWhiteSpace(_sortColumn))
            {
                return source.ToArray();
            }

            switch ((_sortColumn ?? string.Empty).ToLowerInvariant())
            {
                case "almoxarifado": return ApplySort(source, item => item.WarehouseDisplay ?? string.Empty, item => item.MaterialDisplay ?? string.Empty);
                case "lote": return ApplySort(source, item => item.LotDisplay ?? string.Empty, item => item.MaterialDisplay ?? string.Empty);
                case "material": return ApplySort(source, item => item.MaterialDisplay ?? string.Empty, item => item.LotDisplay ?? string.Empty);
                case "validade": return ApplySort(source, item => ParseDate(item.ExpirationDate), item => item.MaterialDisplay ?? string.Empty);
                case "saldoinicial": return ApplySort(source, item => item.OpeningBalance, item => item.MaterialDisplay ?? string.Empty);
                case "entradas": return ApplySort(source, item => item.Entries, item => item.MaterialDisplay ?? string.Empty);
                case "transfentrada": return ApplySort(source, item => item.TransferIn, item => item.MaterialDisplay ?? string.Empty);
                case "transfsaida": return ApplySort(source, item => item.TransferOut, item => item.MaterialDisplay ?? string.Empty);
                case "saidaproducao": return ApplySort(source, item => item.ProductionOutput, item => item.MaterialDisplay ?? string.Empty);
                case "requisicao": return ApplySort(source, item => item.Requisition, item => item.MaterialDisplay ?? string.Empty);
                case "inventario": return ApplySort(source, item => item.Inventory, item => item.MaterialDisplay ?? string.Empty);
                case "saldofinal": return ApplySort(source, item => item.FinalBalance, item => item.MaterialDisplay ?? string.Empty);
                default: return source.ToArray();
            }
        }

        private StockMovementReportRow[] ApplySort<TKey, TThen>(StockMovementReportRow[] items, Func<StockMovementReportRow, TKey> keySelector, Func<StockMovementReportRow, TThen> thenBy)
        {
            return _sortAscending
                ? items.OrderBy(keySelector).ThenBy(thenBy).ToArray()
                : items.OrderByDescending(keySelector).ThenByDescending(thenBy).ToArray();
        }

        private static DateTime ParseDate(string value)
        {
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy" };
            DateTime parsed;
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : DateTime.MinValue;
        }

        private static string NormalizeDateInput(string value)
        {
            var digits = new string((value ?? string.Empty).Where(char.IsDigit).Take(8).ToArray());
            if (digits.Length <= 2)
            {
                return digits;
            }

            if (digits.Length <= 4)
            {
                return digits.Substring(0, 2) + "/" + digits.Substring(2);
            }

            return digits.Substring(0, 2) + "/" + digits.Substring(2, 2) + "/" + digits.Substring(4);
        }

        private static LookupOption[] IncludeBlank(LookupOption[] options)
        {
            return new[] { new LookupOption { Code = string.Empty, Description = string.Empty, Status = string.Empty } }
                .Concat(options ?? new LookupOption[0])
                .ToArray();
        }

        private static LookupOption[] ToLookupOptions(WarehouseSummary[] items)
        {
            return (items ?? new WarehouseSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
        }

        private static LookupOption[] ToLookupOptions(PackagingSummary[] items)
        {
            return (items ?? new PackagingSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Description, Status = item.Status }).ToArray();
        }

        private static LookupOption[] ToLookupOptions(LotSummary[] items)
        {
            return (items ?? new LotSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
        }

        private static void BindCombo(ComboBox comboBox, LookupOption[] options)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            comboBox.Items.AddRange((options ?? new LookupOption[0]).Cast<object>().ToArray());
            comboBox.EndUpdate();
            SelectFirstOption(comboBox);
        }

        private static void SelectOptionByCode(ComboBox comboBox, LookupOption[] options, string selectedCode)
        {
            if (string.IsNullOrWhiteSpace(selectedCode))
            {
                SelectFirstOption(comboBox);
                return;
            }

            for (var index = 0; index < (options ?? new LookupOption[0]).Length; index++)
            {
                if (string.Equals(options[index].Code, selectedCode, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
            }

            SelectFirstOption(comboBox);
        }

        private static void SelectFirstOption(ComboBox comboBox)
        {
            comboBox.SelectedIndex = comboBox.Items.Count > 0 ? 0 : -1;
        }

        private static string GetSelectedCode(ComboBox comboBox)
        {
            return comboBox.SelectedItem is LookupOption option ? option.Code : string.Empty;
        }

        private static string GetSelectedDisplayText(ComboBox comboBox)
        {
            if (!(comboBox.SelectedItem is LookupOption option) || string.IsNullOrWhiteSpace(option.Code))
            {
                return "TODOS";
            }

            return option.DisplayText;
        }

        private static string EscapeCsv(string value)
        {
            var normalized = (value ?? string.Empty).Replace("\"", "\"\"");
            return "\"" + normalized + "\"";
        }
    }
}
