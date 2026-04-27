using System;
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
    public sealed partial class StockTransferPdfReportForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                ApplyDefaultPeriod();
                LoadWarehouses();
                LoadMaterials();
                QueryEntries();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ApplyDefaultPeriod()
        {
            _startDateTextBox.Text = new DateTime(2020, 1, 1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            _endDateTextBox.Text = DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private void LoadWarehouses()
        {
            _warehouseOptions = IncludeAllOption(ToWarehouseLookupOptions(_stockTransferReportController.LoadWarehousesForUser(_configuration, _databaseProfile, _identity.UserName)));
            BindCombo(_originWarehouseComboBox, _warehouseOptions, GetSelectedCode(_originWarehouseComboBox));
            BindCombo(_destinationWarehouseComboBox, _warehouseOptions, GetSelectedCode(_destinationWarehouseComboBox));
        }

        private void LoadMaterials()
        {
            _materialOptions = IncludeAllOption(ToMaterialLookupOptions(_stockTransferReportController.LoadMaterials(_configuration, _databaseProfile)));
            BindCombo(_materialComboBox, _materialOptions, GetSelectedCode(_materialComboBox));
        }

        private void QueryEntries()
        {
            try
            {
                _rows = _stockTransferReportController.SearchEntries(_configuration, _databaseProfile, BuildQuery());
                _grid.DataSource = _rows;
                if (_grid.Rows.Count > 0)
                {
                    _grid.ClearSelection();
                    _grid.Rows[0].Selected = true;
                    _grid.CurrentCell = _grid.Rows[0].Cells[0];
                }

                UpdateSummary();
                SetStatus(_rows.Length == 0 ? "Nenhum item encontrado para os filtros informados." : "Consulta concluida com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateSummary()
        {
            var transferCount = _rows.Select(item => item.Number ?? string.Empty).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            var totalQuantity = _rows.Sum(item => item == null ? 0M : item.Quantity);
            _summaryLabel.Text = _rows.Length + " item(ns) encontrado(s) em " + transferCount + " transferencia(s) | Quantidade total: "
                + totalQuantity.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
        }

        private void ClearFilters()
        {
            _startDateTextBox.Clear();
            _endDateTextBox.Clear();
            _transferNumberTextBox.Clear();
            _excludeCanceledCheckBox.Checked = false;
            SelectOptionByCode(_originWarehouseComboBox, _warehouseOptions, string.Empty);
            SelectOptionByCode(_destinationWarehouseComboBox, _warehouseOptions, string.Empty);
            SelectOptionByCode(_materialComboBox, _materialOptions, string.Empty);
            QueryEntries();
        }

        private void OpenWarehouseLookup(ComboBox targetComboBox, string title)
        {
            using (var dialog = new AlmoxarifadoSelecaoForm(_warehouseOptions.Where(item => !string.IsNullOrWhiteSpace(item.Code)).ToArray(), title))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(targetComboBox, _warehouseOptions, dialog.SelectedOption.Code);
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

        private void ExportCsv()
        {
            if (_rows.Length == 0)
            {
                MessageBox.Show(this, "Nao ha dados para exportar.", "Relatorio de Transferencias", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio CSV";
                    dialog.Filter = "CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "csv";
                    dialog.FileName = "relatorio_transferencias_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine("Transferencia;Item;Data Transferencia;Almox Origem;Almox Destino;Material;Lote;Quantidade;Status");
                        foreach (var row in _rows)
                        {
                            writer.WriteLine(string.Join(";", new[]
                            {
                                EscapeCsv(row.Number),
                                EscapeCsv(row.ItemNumber.ToString(CultureInfo.InvariantCulture)),
                                EscapeCsv(row.MovementDateDisplay),
                                EscapeCsv(row.OriginWarehouseDisplay),
                                EscapeCsv(row.DestinationWarehouseDisplay),
                                EscapeCsv(row.MaterialDisplay),
                                EscapeCsv(row.LotDisplay),
                                EscapeCsv(row.QuantityText),
                                EscapeCsv(row.Status),
                            }));
                        }
                    }

                    _stockTransferReportController.RegisterCsvExport(_configuration, _databaseProfile, _identity.UserName, BuildQuery(), _rows.Length);
                    SetStatus("Arquivo CSV exportado com sucesso.", false);
                    MessageBox.Show(this, "Dados exportados para:\n" + dialog.FileName, "Relatorio de Transferencias", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ExportPdf()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is StockTransferReportEntry selectedRow))
            {
                MessageBox.Show(this, "Selecione uma transferencia na lista para gerar o relatorio.", "Relatorio de Transferencias", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var document = _stockTransferReportController.LoadDocument(_configuration, _databaseProfile, selectedRow.Number);
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio PDF";
                    dialog.Filter = "PDF (*.pdf)|*.pdf|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "pdf";
                    dialog.FileName = "relatorio_transferencia_" + (selectedRow.Number ?? "sem_numero") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    StockTransferPdfReportPdfExporter.Export(dialog.FileName, BuildFilterLines(), document, _identity.DisplayName);
                    _stockTransferReportController.RegisterPdfExport(_configuration, _databaseProfile, _identity.UserName, document.Number, document.Items.Length);
                    SetStatus("PDF gerado com sucesso.", false);

                    if (MessageBox.Show(this, "PDF salvo com sucesso.\n\nDeseja abrir agora?", "Relatorio de Transferencias", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                "Data inicio: " + (_startDateTextBox.Text ?? string.Empty),
                "Data fim: " + (_endDateTextBox.Text ?? string.Empty),
                "Transferencia: " + (_transferNumberTextBox.Text ?? string.Empty),
                "Almox origem: " + GetSelectedDisplayText(_originWarehouseComboBox),
                "Almox destino: " + GetSelectedDisplayText(_destinationWarehouseComboBox),
                "Material: " + GetSelectedDisplayText(_materialComboBox),
                "Excluir canceladas: " + (_excludeCanceledCheckBox.Checked ? "SIM" : "NAO"),
            };
        }

        private void OnGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_grid.Rows[e.RowIndex].DataBoundItem is StockTransferReportEntry row))
            {
                return;
            }

            var style = _grid.Rows[e.RowIndex].DefaultCellStyle;
            if (string.Equals((row.Status ?? string.Empty).Trim(), "CANCELADO", StringComparison.OrdinalIgnoreCase)
                || string.Equals((row.Status ?? string.Empty).Trim(), "CANCELADA", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = Color.FromArgb(255, 232, 232);
                style.ForeColor = Color.Firebrick;
                style.SelectionBackColor = Color.FromArgb(255, 210, 210);
                style.SelectionForeColor = Color.Firebrick;
                return;
            }

            style.BackColor = e.RowIndex % 2 == 0 ? Color.White : Color.FromArgb(245, 248, 250);
            style.ForeColor = Color.Black;
            style.SelectionBackColor = Color.FromArgb(220, 235, 247);
            style.SelectionForeColor = Color.Black;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.E)
            {
                e.Handled = true;
                ExportCsv();
                return;
            }

            if (e.Control && e.KeyCode == Keys.P)
            {
                e.Handled = true;
                ExportPdf();
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

        private void NormalizeTransferNumberInput()
        {
            var normalized = (_transferNumberTextBox.Text ?? string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
            if (!string.Equals(_transferNumberTextBox.Text, normalized, StringComparison.Ordinal))
            {
                var caret = normalized.Length;
                _transferNumberTextBox.Text = normalized;
                _transferNumberTextBox.SelectionStart = caret;
            }
        }

        private StockTransferReportQuery BuildQuery()
        {
            return new StockTransferReportQuery
            {
                StartDate = _startDateTextBox.Text,
                EndDate = _endDateTextBox.Text,
                TransferNumber = _transferNumberTextBox.Text,
                OriginWarehouseCode = GetSelectedCode(_originWarehouseComboBox),
                DestinationWarehouseCode = GetSelectedCode(_destinationWarehouseComboBox),
                MaterialCode = GetSelectedCode(_materialComboBox),
                UserName = _identity.UserName,
                ExcludeCanceled = _excludeCanceledCheckBox.Checked,
            };
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

        private static LookupOption[] IncludeAllOption(LookupOption[] options)
        {
            return new[] { new LookupOption { Code = string.Empty, Description = "TODOS", Status = string.Empty } }
                .Concat(options ?? new LookupOption[0])
                .ToArray();
        }

        private static LookupOption[] ToWarehouseLookupOptions(WarehouseSummary[] items)
        {
            return (items ?? new WarehouseSummary[0]).Select(item => new LookupOption
            {
                Code = item.Code,
                Description = string.IsNullOrWhiteSpace(item.Name) ? (item.Code ?? string.Empty) : item.Code + " - " + item.Name,
                Status = item.Status,
            }).ToArray();
        }

        private static LookupOption[] ToMaterialLookupOptions(PackagingSummary[] items)
        {
            return (items ?? new PackagingSummary[0]).Select(item => new LookupOption
            {
                Code = item.Code,
                Description = string.IsNullOrWhiteSpace(item.Description) ? (item.Code ?? string.Empty) : item.Code + " - " + item.Description,
                Status = item.Status,
            }).ToArray();
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
                comboBox.SelectedIndex = comboBox.Items.Count > 0 ? 0 : -1;
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
