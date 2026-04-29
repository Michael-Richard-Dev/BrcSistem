using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.RelatorioPdfNotaEntrada
{
    public sealed partial class RelatorioPdfNotaEntradaForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                ApplyDefaultPeriod();
                LoadSuppliers();
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

        private void LoadSuppliers()
        {
            _supplierOptions = IncludeAllOption(ToLookupOptions(_inboundReceiptReportController.LoadSuppliers(_configuration, _databaseProfile)));
            BindCombo(_supplierComboBox, _supplierOptions, GetSelectedCode(_supplierComboBox));
        }

        private void QueryEntries()
        {
            try
            {
                _rows = _inboundReceiptReportController.SearchEntries(_configuration, _databaseProfile, BuildQuery());
                _grid.DataSource = _rows;
                if (_grid.Rows.Count > 0)
                {
                    _grid.ClearSelection();
                    _grid.Rows[0].Selected = true;
                    _grid.CurrentCell = _grid.Rows[0].Cells[0];
                }
                UpdateSummary();
                SetStatus(_rows.Length == 0 ? "Nenhum material encontrado para os filtros informados." : "Consulta concluida com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateSummary()
        {
            _summaryLabel.Text = _rows.Length + " material(is) encontrado(s)";
        }

        private void ClearFilters()
        {
            _startDateTextBox.Clear();
            _endDateTextBox.Clear();
            _receiptNumberTextBox.Clear();
            _excludeCanceledCheckBox.Checked = false;
            SelectOptionByCode(_supplierComboBox, _supplierOptions, string.Empty);
            QueryEntries();
        }

        private void OpenSupplierLookup()
        {
            using (var dialog = new FornecedorSelecaoForm(_supplierOptions.Where(item => !string.IsNullOrWhiteSpace(item.Code)).ToArray()))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_supplierComboBox, _supplierOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void ExportCsv()
        {
            if (_rows.Length == 0)
            {
                MessageBox.Show(this, "Nao ha dados para exportar.", "Relatorio de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio CSV";
                    dialog.Filter = "CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "csv";
                    dialog.FileName = "relatorio_entrada_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine("Nota Fiscal;Fornecedor;Codigo;Material;Quantidade;Lote;Data Entrada;Status");
                        foreach (var row in _rows)
                        {
                            writer.WriteLine(string.Join(";", new[]
                            {
                                EscapeCsv(row.Number),
                                EscapeCsv(row.SupplierDisplay),
                                EscapeCsv(row.MaterialCodeDisplay),
                                EscapeCsv(row.MaterialNameDisplay),
                                EscapeCsv(row.QuantityText),
                                EscapeCsv(row.LotDisplay),
                                EscapeCsv(row.ReceiptDateDisplay),
                                EscapeCsv(row.Status),
                            }));
                        }
                    }

                    _inboundReceiptReportController.RegisterCsvExport(_configuration, _databaseProfile, _identity.UserName, BuildQuery(), _rows.Length);
                    SetStatus("Arquivo CSV exportado com sucesso.", false);
                    MessageBox.Show(this, "Dados exportados para:\n" + dialog.FileName, "Relatorio de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ExportPdf()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is InboundReceiptReportEntry selectedRow))
            {
                MessageBox.Show(this, "Selecione uma nota fiscal na lista para gerar o relatorio.", "Relatorio de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var document = _inboundReceiptReportController.LoadDocument(_configuration, _databaseProfile, selectedRow.Number, selectedRow.SupplierCode);
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio PDF";
                    dialog.Filter = "PDF (*.pdf)|*.pdf|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "pdf";
                    dialog.FileName = "relatorio_entrada_nota_" + (selectedRow.Number ?? "sem_numero") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    ExportadorPdfRelatorioNotaEntrada.Export(dialog.FileName, BuildFilterLines(), document, _identity.DisplayName);
                    _inboundReceiptReportController.RegisterPdfExport(_configuration, _databaseProfile, _identity.UserName, document.Number, document.SupplierCode, document.Items.Length);
                    SetStatus("PDF gerado com sucesso.", false);

                    if (MessageBox.Show(this, "PDF salvo com sucesso.\n\nDeseja abrir agora?", "Relatorio de Entrada", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                "Nota fiscal: " + (_receiptNumberTextBox.Text ?? string.Empty),
                "Fornecedor: " + GetSelectedDisplayText(_supplierComboBox),
                "Excluir canceladas: " + (_excludeCanceledCheckBox.Checked ? "SIM" : "NAO"),
            };
        }

        private void OnGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_grid.Rows[e.RowIndex].DataBoundItem is InboundReceiptReportEntry row))
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

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                QueryEntries();
            }
        }

        private void NormalizeReceiptNumberInput()
        {
            var digits = new string((_receiptNumberTextBox.Text ?? string.Empty).Where(char.IsDigit).ToArray());
            if (!string.Equals(_receiptNumberTextBox.Text, digits, StringComparison.Ordinal))
            {
                var caret = digits.Length;
                _receiptNumberTextBox.Text = digits;
                _receiptNumberTextBox.SelectionStart = caret;
            }
        }

        private InboundReceiptReportQuery BuildQuery()
        {
            return new InboundReceiptReportQuery
            {
                StartDate = _startDateTextBox.Text,
                EndDate = _endDateTextBox.Text,
                ReceiptNumber = _receiptNumberTextBox.Text,
                SupplierCode = GetSelectedCode(_supplierComboBox),
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

        private static LookupOption[] ToLookupOptions(SupplierSummary[] items)
        {
            return (items ?? new SupplierSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
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

        private void ShowError(Exception exception)
        {
            SetStatus(exception.Message, true);
            MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }
    }
}
