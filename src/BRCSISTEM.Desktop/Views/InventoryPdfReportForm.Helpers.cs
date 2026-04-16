using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class InventoryPdfReportForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                LoadInventories();
                if (_inventoryOptions.Length > 0)
                {
                    QueryDocument();
                }
                else
                {
                    ClearDocument();
                    SetStatus("Nenhum inventario encontrado para consulta.", false);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadInventories()
        {
            try
            {
                var selectedNumber = GetSelectedInventoryNumber();
                _inventoryOptions = _inventoryReportController.LoadInventories(_configuration, _databaseProfile);
                _inventoryComboBox.BeginUpdate();
                _inventoryComboBox.DataSource = null;
                _inventoryComboBox.DisplayMember = nameof(InventoryReportEntry.DisplayText);
                _inventoryComboBox.ValueMember = nameof(InventoryReportEntry.Number);
                _inventoryComboBox.DataSource = _inventoryOptions;
                _inventoryComboBox.EndUpdate();

                if (!string.IsNullOrWhiteSpace(selectedNumber))
                {
                    for (var index = 0; index < _inventoryOptions.Length; index++)
                    {
                        if (string.Equals(_inventoryOptions[index].Number, selectedNumber, StringComparison.OrdinalIgnoreCase))
                        {
                            _inventoryComboBox.SelectedIndex = index;
                            return;
                        }
                    }
                }

                _inventoryComboBox.SelectedIndex = _inventoryOptions.Length > 0 ? 0 : -1;
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void QueryDocument()
        {
            var number = GetSelectedInventoryNumber();
            if (string.IsNullOrWhiteSpace(number))
            {
                MessageBox.Show(this, "Selecione um inventario.", "Relatorio de Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _currentDocument = _inventoryReportController.LoadDocument(_configuration, _databaseProfile, number);
                _itemsGrid.DataSource = _currentDocument.Items;
                _movementsGrid.DataSource = _currentDocument.Movements;
                if (_itemsGrid.Rows.Count > 0)
                {
                    _itemsGrid.ClearSelection();
                }

                if (_movementsGrid.Rows.Count > 0)
                {
                    _movementsGrid.ClearSelection();
                }

                UpdateSummary();
                SetStatus("Consulta concluida com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ClearDocument()
        {
            _currentDocument = null;
            _itemsGrid.DataSource = Array.Empty<InventoryReportItem>();
            _movementsGrid.DataSource = Array.Empty<InventoryReportMovement>();
            _statusSummaryLabel.Text = "Status: -";
            _creatorSummaryLabel.Text = "Criador: -";
            _datesSummaryLabel.Text = "Abertura/Fechamento: -";
            _totalsSummaryLabel.Text = "Itens: 0 | Divergentes: 0 | Entradas: 0,00 | Saidas: 0,00";
            _observationSummaryLabel.Text = "Observacao: -";
        }

        private void UpdateSummary()
        {
            var document = _currentDocument ?? new InventoryReportDocument();
            _statusSummaryLabel.Text = "Status: " + SafeText(document.Status);
            _creatorSummaryLabel.Text = "Criador: " + SafeText(document.CreatedBy);
            _datesSummaryLabel.Text = "Abertura: " + SafeText(document.OpenedAtDisplay)
                + " | Fechamento: " + SafeText(document.ClosedAtDisplay)
                + " | Finalizacao: " + SafeText(document.FinalizedAtDisplay);
            _totalsSummaryLabel.Text = "Itens: " + (document.Items ?? Array.Empty<InventoryReportItem>()).Length
                + " | Divergentes: " + document.DivergentItemCount
                + " | Entradas: " + document.TotalEntryAdjustmentsText
                + " | Saidas: " + document.TotalOutputAdjustmentsText;
            _observationSummaryLabel.Text = "Observacao: " + SafeText(document.Observation)
                + " | Motivo Cancelamento: " + SafeText(document.CancellationReason);
        }

        private void ExportCsv()
        {
            if (!EnsureCurrentDocumentLoaded())
            {
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio CSV";
                    dialog.Filter = "CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "csv";
                    dialog.FileName = "relatorio_inventario_" + (_currentDocument.Number ?? "sem_numero") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine("Inventario;" + EscapeCsv(_currentDocument.Number));
                        writer.WriteLine("Status;" + EscapeCsv(_currentDocument.Status));
                        writer.WriteLine("Criador;" + EscapeCsv(_currentDocument.CreatedBy));
                        writer.WriteLine("Abertura;" + EscapeCsv(_currentDocument.OpenedAtDisplay));
                        writer.WriteLine("Fechamento;" + EscapeCsv(_currentDocument.ClosedAtDisplay));
                        writer.WriteLine("Finalizacao;" + EscapeCsv(_currentDocument.FinalizedAtDisplay));
                        writer.WriteLine("Observacao;" + EscapeCsv(_currentDocument.Observation));
                        writer.WriteLine("Motivo Cancelamento;" + EscapeCsv(_currentDocument.CancellationReason));
                        writer.WriteLine();
                        writer.WriteLine("Itens");
                        writer.WriteLine("Almox;Material;Lote;Saldo Sistema;Qtd Contada;Divergencia;Tipo Ajuste;Validade;Status");
                        foreach (var item in _currentDocument.Items ?? Array.Empty<InventoryReportItem>())
                        {
                            writer.WriteLine(string.Join(";", new[]
                            {
                                EscapeCsv(item.WarehouseCode),
                                EscapeCsv(item.MaterialDisplay),
                                EscapeCsv(item.LotDisplay),
                                EscapeCsv(item.SystemBalanceText),
                                EscapeCsv(item.CountedQuantityText),
                                EscapeCsv(item.AdjustmentQuantityText),
                                EscapeCsv(item.AdjustmentType),
                                EscapeCsv(item.ExpirationDateDisplay),
                                EscapeCsv(item.Status),
                            }));
                        }

                        writer.WriteLine();
                        writer.WriteLine("Movimentos de Ajuste");
                        writer.WriteLine("Data/Hora;Item;Tipo;Almox;Material;Lote;Quantidade;Fornecedor;Usuario;Status");
                        foreach (var movement in _currentDocument.Movements ?? Array.Empty<InventoryReportMovement>())
                        {
                            writer.WriteLine(string.Join(";", new[]
                            {
                                EscapeCsv(movement.MovementDateDisplay),
                                EscapeCsv(movement.ItemNumberText),
                                EscapeCsv(movement.Type),
                                EscapeCsv(movement.WarehouseCode),
                                EscapeCsv(movement.MaterialCode),
                                EscapeCsv(movement.LotCode),
                                EscapeCsv(movement.QuantityText),
                                EscapeCsv(movement.SupplierCode),
                                EscapeCsv(movement.UserName),
                                EscapeCsv(movement.Status),
                            }));
                        }
                    }

                    _inventoryReportController.RegisterCsvExport(
                        _configuration,
                        _databaseProfile,
                        _identity.UserName,
                        _currentDocument.Number,
                        (_currentDocument.Items ?? Array.Empty<InventoryReportItem>()).Length,
                        (_currentDocument.Movements ?? Array.Empty<InventoryReportMovement>()).Length);
                    SetStatus("Arquivo CSV exportado com sucesso.", false);
                    MessageBox.Show(this, "Dados exportados para:\n" + dialog.FileName, "Relatorio de Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ExportPdf()
        {
            if (!EnsureCurrentDocumentLoaded())
            {
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio PDF";
                    dialog.Filter = "PDF (*.pdf)|*.pdf|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "pdf";
                    dialog.FileName = "relatorio_inventario_" + (_currentDocument.Number ?? "sem_numero") + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    InventoryPdfReportPdfExporter.Export(dialog.FileName, BuildFilterLines(), _currentDocument, _identity.DisplayName);
                    _inventoryReportController.RegisterPdfExport(
                        _configuration,
                        _databaseProfile,
                        _identity.UserName,
                        _currentDocument.Number,
                        _currentDocument.DivergentItemCount,
                        (_currentDocument.Movements ?? Array.Empty<InventoryReportMovement>()).Length);
                    SetStatus("PDF gerado com sucesso.", false);

                    if (MessageBox.Show(this, "PDF salvo com sucesso.\n\nDeseja abrir agora?", "Relatorio de Inventario", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
            var entry = _inventoryComboBox.SelectedItem as InventoryReportEntry;
            return new[]
            {
                "Inventario: " + SafeText(entry != null ? entry.Number : string.Empty),
                "Status: " + SafeText(_currentDocument != null ? _currentDocument.Status : string.Empty),
                "Referencia: " + SafeText(entry != null ? entry.ReferenceDateDisplay : string.Empty),
            };
        }

        private bool EnsureCurrentDocumentLoaded()
        {
            var selectedNumber = GetSelectedInventoryNumber();
            if (string.IsNullOrWhiteSpace(selectedNumber))
            {
                MessageBox.Show(this, "Selecione um inventario.", "Relatorio de Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (_currentDocument == null || !string.Equals(_currentDocument.Number, selectedNumber, StringComparison.OrdinalIgnoreCase))
            {
                QueryDocument();
            }

            return _currentDocument != null;
        }

        private string GetSelectedInventoryNumber()
        {
            return (_inventoryComboBox.SelectedItem as InventoryReportEntry)?.Number ?? string.Empty;
        }

        private void OnItemsGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_itemsGrid.Rows[e.RowIndex].DataBoundItem is InventoryReportItem item))
            {
                return;
            }

            var style = _itemsGrid.Rows[e.RowIndex].DefaultCellStyle;
            if (item.IsDivergent)
            {
                style.BackColor = Color.FromArgb(255, 236, 236);
                style.ForeColor = Color.Firebrick;
                style.SelectionBackColor = Color.FromArgb(255, 214, 214);
                style.SelectionForeColor = Color.Firebrick;
                return;
            }

            style.BackColor = Color.FromArgb(238, 248, 238);
            style.ForeColor = Color.DarkGreen;
            style.SelectionBackColor = Color.FromArgb(214, 236, 214);
            style.SelectionForeColor = Color.DarkGreen;
        }

        private void OnMovementsGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_movementsGrid.Rows[e.RowIndex].DataBoundItem is InventoryReportMovement movement))
            {
                return;
            }

            var style = _movementsGrid.Rows[e.RowIndex].DefaultCellStyle;
            if (string.Equals((movement.Status ?? string.Empty).Trim(), "INATIVO", StringComparison.OrdinalIgnoreCase)
                || string.Equals((movement.Status ?? string.Empty).Trim(), "CANCELADO", StringComparison.OrdinalIgnoreCase)
                || string.Equals((movement.Status ?? string.Empty).Trim(), "CANCELADA", StringComparison.OrdinalIgnoreCase))
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

            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                LoadInventories();
                QueryDocument();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private static string EscapeCsv(string value)
        {
            var normalized = (value ?? string.Empty).Replace("\"", "\"\"");
            return "\"" + normalized + "\"";
        }

        private static string SafeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }
    }
}
