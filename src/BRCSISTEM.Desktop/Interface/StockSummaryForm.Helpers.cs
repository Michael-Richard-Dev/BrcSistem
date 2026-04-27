using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class StockSummaryForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                ApplyReferenceDate(DateTime.Today);
                LoadReferences();
                QueryEntries();
                SetStatus("Resumo sintetico carregado com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadReferences()
        {
            _isRefreshingFilters = true;
            try
            {
                _warehouseOptions = IncludeBlank(ToLookupOptions(_stockSummaryController.LoadWarehouses(_configuration, _databaseProfile)));
                BindCombo(_warehouseComboBox, _warehouseOptions, GetSelectedCode(_warehouseComboBox));

                _materialOptions = IncludeBlank(ToLookupOptions(_stockSummaryController.LoadMaterials(_configuration, _databaseProfile)));
                BindCombo(_materialComboBox, _materialOptions, GetSelectedCode(_materialComboBox));

                _lotOptions = IncludeBlank(ToLookupOptions(_stockSummaryController.LoadLots(_configuration, _databaseProfile)));
                BindCombo(_lotComboBox, _lotOptions, GetSelectedCode(_lotComboBox));
            }
            finally
            {
                _isRefreshingFilters = false;
            }
        }

        private void QueryEntries()
        {
            try
            {
                _filterRefreshTimer.Stop();
                _entries = _stockSummaryController.LoadEntries(_configuration, _databaseProfile, BuildQuery());
                _displayRows = BuildDisplayRows(_entries);
                BindGrid();
                UpdateSummary();
                SetStatus(_entries.Length == 0 ? "Nenhum registro encontrado para os filtros informados." : "Consulta concluida com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void BindGrid()
        {
            _grid.DataSource = _displayRows;
            if (_grid.Rows.Count > 0)
            {
                _grid.ClearSelection();
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private void UpdateSummary()
        {
            var totalQuantity = _entries.Sum(item => item.Quantity);
            _summaryLabel.Text = "Total: " + totalQuantity.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) + " unidades";
            _infoLabel.Text = _entries.Length + " registro(s) | Data: " + (_referenceDateTextBox.Text ?? string.Empty);
        }

        private StockSummaryDisplayRow[] BuildDisplayRows(StockSummaryEntry[] entries)
        {
            var rows = new List<StockSummaryDisplayRow>();
            var source = entries ?? new StockSummaryEntry[0];

            foreach (var warehouseGroup in source
                .GroupBy(item => new { item.WarehouseCode, Display = item.WarehouseDisplay })
                .OrderBy(group => group.Key.Display ?? string.Empty, StringComparer.OrdinalIgnoreCase))
            {
                rows.Add(new StockSummaryDisplayRow
                {
                    HierarchyText = warehouseGroup.Key.Display,
                    Quantity = warehouseGroup.Sum(item => item.Quantity),
                    ExpirationDateDisplay = string.Empty,
                    RowKind = StockSummaryRowKind.WarehouseHeader,
                    WarehouseCode = warehouseGroup.Key.WarehouseCode,
                });

                foreach (var materialGroup in warehouseGroup
                    .GroupBy(item => new { item.MaterialCode, Display = item.MaterialDisplay })
                    .OrderBy(group => group.Key.Display ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                {
                    rows.Add(new StockSummaryDisplayRow
                    {
                        HierarchyText = "  " + materialGroup.Key.Display,
                        Quantity = materialGroup.Sum(item => item.Quantity),
                        ExpirationDateDisplay = string.Empty,
                        RowKind = StockSummaryRowKind.MaterialHeader,
                        WarehouseCode = warehouseGroup.Key.WarehouseCode,
                        MaterialCode = materialGroup.Key.MaterialCode,
                    });

                    foreach (var lotGroup in materialGroup
                        .GroupBy(item => new
                        {
                            item.LotCode,
                            Display = item.LotDisplay,
                            Expiration = item.ExpirationDateDisplay,
                        })
                        .OrderBy(group => group.Key.Display ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(group => ParseDate(group.Key.Expiration)))
                    {
                        rows.Add(new StockSummaryDisplayRow
                        {
                            HierarchyText = "    " + lotGroup.Key.Display,
                            Quantity = lotGroup.Sum(item => item.Quantity),
                            ExpirationDateDisplay = lotGroup.Key.Expiration,
                            RowKind = StockSummaryRowKind.LotItem,
                            WarehouseCode = warehouseGroup.Key.WarehouseCode,
                            MaterialCode = materialGroup.Key.MaterialCode,
                            LotCode = lotGroup.Key.LotCode,
                        });
                    }
                }
            }

            return rows.ToArray();
        }

        private void ApplyQuickDate(int daysAgo)
        {
            ApplyReferenceDate(DateTime.Today.AddDays(-daysAgo));
            QueryEntries();
        }

        private void ApplyReferenceDate(DateTime date)
        {
            _referenceDateTextBox.Text = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private void ClearFilters()
        {
            _filterRefreshTimer.Stop();
            ApplyReferenceDate(DateTime.Today);
            _isRefreshingFilters = true;
            try
            {
                SelectFirstOption(_warehouseComboBox);
                SelectFirstOption(_materialComboBox);
                SelectFirstOption(_lotComboBox);
            }
            finally
            {
                _isRefreshingFilters = false;
            }

            QueryEntries();
        }

        private void ScheduleRefresh()
        {
            if (_isRefreshingFilters || _configuration == null)
            {
                return;
            }

            _filterRefreshTimer.Stop();
            _filterRefreshTimer.Start();
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
            if (_entries.Length == 0)
            {
                MessageBox.Show(this, "Nao ha dados para exportar.", "Resumo Sintetico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Resumo Sintetico";
                    dialog.Filter = "CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "csv";
                    dialog.FileName = "resumo_sintetico_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine("Almoxarifado;Material;Lote;Data Validade;Quantidade");
                        foreach (var entry in _entries)
                        {
                            writer.WriteLine(string.Join(";", new[]
                            {
                                EscapeCsv(entry.WarehouseDisplay),
                                EscapeCsv(entry.MaterialDisplay),
                                EscapeCsv(entry.LotDisplay),
                                EscapeCsv(entry.ExpirationDateDisplay),
                                EscapeCsv(entry.QuantityText),
                            }));
                        }
                    }

                    _stockSummaryController.RegisterCsvExport(_configuration, _databaseProfile, _identity.UserName, BuildQuery(), _entries.Length);
                    SetStatus("Arquivo CSV exportado com sucesso.", false);
                    MessageBox.Show(this, "Dados exportados para:\n" + dialog.FileName, "Resumo Sintetico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ExportPdf()
        {
            if (_displayRows.Length == 0)
            {
                MessageBox.Show(this, "Nao ha dados para gerar o PDF.", "Resumo Sintetico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Resumo Sintetico em PDF";
                    dialog.Filter = "PDF (*.pdf)|*.pdf|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "pdf";
                    dialog.FileName = "resumo_sintetico_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    StockSummaryPdfExporter.Export(dialog.FileName, BuildFilterLines(), _displayRows, _entries.Sum(item => item.Quantity), _entries.Length);
                    _stockSummaryController.RegisterPdfExport(_configuration, _databaseProfile, _identity.UserName, BuildQuery(), _entries.Length);
                    SetStatus("PDF gerado com sucesso.", false);

                    if (MessageBox.Show(this, "PDF salvo com sucesso.\n\nDeseja abrir agora?", "Resumo Sintetico", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                "Data de referencia: " + (_referenceDateTextBox.Text ?? string.Empty),
                "Almoxarifado: " + GetSelectedDisplayText(_warehouseComboBox),
                "Material: " + GetSelectedDisplayText(_materialComboBox),
                "Lote: " + GetSelectedDisplayText(_lotComboBox),
                "Total de registros: " + _entries.Length,
            };
        }

        private void OnGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_grid.Rows[e.RowIndex].DataBoundItem is StockSummaryDisplayRow row))
            {
                return;
            }

            var style = _grid.Rows[e.RowIndex].DefaultCellStyle;
            style.ForeColor = Color.Black;
            style.SelectionForeColor = Color.Black;

            if (row.RowKind == StockSummaryRowKind.WarehouseHeader)
            {
                style.Font = _warehouseRowFont;
                style.BackColor = Color.FromArgb(10, 77, 140);
                style.ForeColor = Color.White;
                style.SelectionBackColor = style.BackColor;
                style.SelectionForeColor = style.ForeColor;
                return;
            }

            if (row.RowKind == StockSummaryRowKind.MaterialHeader)
            {
                style.Font = _materialRowFont;
                style.BackColor = Color.FromArgb(27, 54, 93);
                style.ForeColor = Color.White;
                style.SelectionBackColor = style.BackColor;
                style.SelectionForeColor = style.ForeColor;
                return;
            }

            style.Font = _grid.Font;
            style.BackColor = e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(245, 248, 250);
            style.SelectionBackColor = Color.FromArgb(220, 235, 247);
            style.SelectionForeColor = Color.Black;
        }

        private void OnGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            OpenLedgerForSelectedRow();
        }

        private void OpenLedgerForSelectedRow()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is StockSummaryDisplayRow row))
            {
                return;
            }

            using (var dialog = new StockLedgerForm(_compositionRoot, _identity, _databaseProfile, BuildLedgerQuery(row)))
            {
                dialog.ShowDialog(this);
            }
        }

        private StockLedgerQuery BuildLedgerQuery(StockSummaryDisplayRow row)
        {
            return new StockLedgerQuery
            {
                StartDate = string.Empty,
                EndDate = _referenceDateTextBox.Text,
                SupplierCode = string.Empty,
                MaterialCode = row.MaterialCode ?? string.Empty,
                LotCode = row.LotCode ?? string.Empty,
                WarehouseCode = row.WarehouseCode ?? string.Empty,
                MovementType = string.Empty,
                IncludeInactive = false,
            };
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.E)
            {
                e.Handled = true;
                ExportCsv();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                QueryEntries();
            }
        }

        private StockSummaryQuery BuildQuery()
        {
            return new StockSummaryQuery
            {
                ReferenceDate = _referenceDateTextBox.Text,
                WarehouseCode = GetSelectedCode(_warehouseComboBox),
                MaterialCode = GetSelectedCode(_materialComboBox),
                LotCode = GetSelectedCode(_lotComboBox),
            };
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

        private static void BindCombo(ComboBox comboBox, LookupOption[] options, string selectedCode)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            comboBox.Items.AddRange((options ?? new LookupOption[0]).Cast<object>().ToArray());
            comboBox.EndUpdate();
            SelectOptionByCode(comboBox, options ?? new LookupOption[0], selectedCode);
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
