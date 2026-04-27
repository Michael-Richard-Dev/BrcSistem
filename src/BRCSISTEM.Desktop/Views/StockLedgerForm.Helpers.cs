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
    public sealed partial class StockLedgerForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                if (_initialQuery != null)
                {
                    ApplyInitialQuery(_initialQuery);
                    SetStatus("Conta corrente carregada com filtros pre-aplicados.", false);
                    return;
                }

                ApplyDefaultPeriod();
                ReloadAllReferences(null, null, null, null);
                QueryEntries();
                SetStatus("Conta corrente carregada.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ApplyInitialQuery(StockLedgerQuery initialQuery)
        {
            _startDateTextBox.Text = NormalizeDateInput(initialQuery.StartDate);
            _endDateTextBox.Text = NormalizeDateInput(initialQuery.EndDate);
            _includeInactiveCheckBox.Checked = initialQuery.IncludeInactive;
            ReloadAllReferences(initialQuery.SupplierCode, initialQuery.MaterialCode, initialQuery.LotCode, initialQuery.WarehouseCode);
            SetSelectedMovementType(initialQuery.MovementType);
            QueryEntries();
        }

        private void ReloadAllReferences(string supplierCode, string materialCode, string lotCode, string warehouseCode)
        {
            _isRefreshingFilters = true;
            try
            {
                _supplierOptions = IncludeBlank(ToLookupOptions(_stockLedgerController.LoadSuppliers(_configuration, _databaseProfile)));
                BindCombo(_supplierComboBox, _supplierOptions, supplierCode);

                _materialOptions = IncludeBlank(ToLookupOptions(_stockLedgerController.LoadMaterials(_configuration, _databaseProfile, GetSelectedCode(_supplierComboBox))));
                BindCombo(_materialComboBox, _materialOptions, materialCode);

                _warehouseOptions = IncludeBlank(ToLookupOptions(_stockLedgerController.LoadWarehouses(_configuration, _databaseProfile, GetSelectedCode(_supplierComboBox))));
                BindCombo(_warehouseComboBox, _warehouseOptions, warehouseCode);

                _lotOptions = IncludeBlank(ToLookupOptions(_stockLedgerController.LoadLots(_configuration, _databaseProfile, GetSelectedCode(_materialComboBox), GetSelectedCode(_supplierComboBox))));
                BindCombo(_lotComboBox, _lotOptions, lotCode);
            }
            finally
            {
                _isRefreshingFilters = false;
            }
        }

        private void ReloadDependentReferences()
        {
            var selectedMaterial = GetSelectedCode(_materialComboBox);
            var selectedLot = GetSelectedCode(_lotComboBox);
            var selectedWarehouse = GetSelectedCode(_warehouseComboBox);

            _isRefreshingFilters = true;
            try
            {
                _materialOptions = IncludeBlank(ToLookupOptions(_stockLedgerController.LoadMaterials(_configuration, _databaseProfile, GetSelectedCode(_supplierComboBox))));
                BindCombo(_materialComboBox, _materialOptions, selectedMaterial);

                _warehouseOptions = IncludeBlank(ToLookupOptions(_stockLedgerController.LoadWarehouses(_configuration, _databaseProfile, GetSelectedCode(_supplierComboBox))));
                BindCombo(_warehouseComboBox, _warehouseOptions, selectedWarehouse);

                _lotOptions = IncludeBlank(ToLookupOptions(_stockLedgerController.LoadLots(_configuration, _databaseProfile, GetSelectedCode(_materialComboBox), GetSelectedCode(_supplierComboBox))));
                BindCombo(_lotComboBox, _lotOptions, selectedLot);
            }
            finally
            {
                _isRefreshingFilters = false;
            }
        }

        private void ReloadLots()
        {
            var selectedLot = GetSelectedCode(_lotComboBox);
            _isRefreshingFilters = true;
            try
            {
                _lotOptions = IncludeBlank(ToLookupOptions(_stockLedgerController.LoadLots(_configuration, _databaseProfile, GetSelectedCode(_materialComboBox), GetSelectedCode(_supplierComboBox))));
                BindCombo(_lotComboBox, _lotOptions, selectedLot);
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
                _entries = _stockLedgerController.LoadEntries(_configuration, _databaseProfile, BuildQuery());
                BindGrid();
                UpdateSummary();
                SetStatus(_entries.Length == 0 ? "Nenhum movimento encontrado para os filtros informados." : "Consulta concluida com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void BindGrid()
        {
            _grid.DataSource = SortEntries(_entries);
            if (_grid.Rows.Count > 0)
            {
                _grid.ClearSelection();
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private void UpdateSummary()
        {
            var total = _entries.Length;
            var active = _entries.Count(item => string.Equals(item.Status, "ATIVO", StringComparison.OrdinalIgnoreCase));
            var finalBalance = _entries.Length == 0
                ? 0M
                : _entries.OrderBy(item => ParseDate(item.MovementDateTime)).ThenBy(item => item.MovementId).Last().RunningBalance;
            _summaryLabel.Text = "Total: " + total
                + " | Ativos: " + active
                + " | Saldo Final: " + finalBalance.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
        }

        private void ClearFilters()
        {
            _startDateTextBox.Clear();
            _endDateTextBox.Clear();
            _includeInactiveCheckBox.Checked = false;
            _typeComboBox.SelectedIndex = 0;
            ReloadAllReferences(null, null, null, null);
            _entries = new StockLedgerEntry[0];
            _grid.DataSource = _entries;
            UpdateSummary();
            SetStatus("Filtros limpos.", false);
        }

        private void OnSupplierChanged()
        {
            if (_isRefreshingFilters || _configuration == null)
            {
                return;
            }

            ReloadDependentReferences();
        }

        private void OnMaterialChanged()
        {
            if (_isRefreshingFilters || _configuration == null)
            {
                return;
            }

            ReloadLots();
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

        private void OpenMaterialLookup()
        {
            ReloadDependentReferences();
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
            ReloadLots();
            using (var dialog = new LoteSelecaoForm(_lotOptions.Where(item => !string.IsNullOrWhiteSpace(item.Code)).ToArray()))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_lotComboBox, _lotOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void OpenWarehouseLookup()
        {
            ReloadDependentReferences();
            using (var dialog = new AlmoxarifadoSelecaoForm(_warehouseOptions.Where(item => !string.IsNullOrWhiteSpace(item.Code)).ToArray()))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedOption != null)
                {
                    SelectOptionByCode(_warehouseComboBox, _warehouseOptions, dialog.SelectedOption.Code);
                }
            }
        }

        private void ExportCsv()
        {
            if (_entries.Length == 0)
            {
                MessageBox.Show(this, "Nao ha dados para exportar.", "Conta Corrente de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Exportar Conta Corrente de Estoque";
                    dialog.Filter = "CSV (*.csv)|*.csv|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "csv";
                    dialog.FileName = "conta_corrente_estoque_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine("Data/Hora;Documento;Tipo;Material;Lote;Validade;Almoxarifado;Fornecedor;Quantidade;Saldo Acumulado;Data Criacao;Usuario Criacao;Usuario;Status");
                        foreach (var entry in SortEntries(_entries))
                        {
                            writer.WriteLine(string.Join(";", new[]
                            {
                                EscapeCsv(entry.MovementDateTimeDisplay),
                                EscapeCsv(entry.DocumentDisplay),
                                EscapeCsv(entry.DisplayType),
                                EscapeCsv(entry.MaterialDisplay),
                                EscapeCsv(entry.LotDisplay),
                                EscapeCsv(entry.ExpirationDateDisplay),
                                EscapeCsv(entry.WarehouseDisplay),
                                EscapeCsv(entry.SupplierDisplay),
                                EscapeCsv(entry.QuantityText),
                                EscapeCsv(entry.RunningBalanceText),
                                EscapeCsv(entry.CreatedAtDisplay),
                                EscapeCsv(entry.UserName),
                                EscapeCsv(entry.UserName),
                                EscapeCsv(entry.Status),
                            }));
                        }
                    }

                    _stockLedgerController.RegisterCsvExport(_configuration, _databaseProfile, _identity.UserName, BuildQuery(), _entries.Length);
                    SetStatus("Arquivo CSV exportado com sucesso.", false);
                    MessageBox.Show(this, "Dados exportados para:\n" + dialog.FileName, "Conta Corrente de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void ExportPdf()
        {
            if (_entries.Length == 0)
            {
                MessageBox.Show(this, "Nao ha dados para gerar o PDF.", "Conta Corrente de Estoque", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Salvar Relatorio PDF";
                    dialog.Filter = "PDF (*.pdf)|*.pdf|Todos os arquivos (*.*)|*.*";
                    dialog.DefaultExt = "pdf";
                    dialog.FileName = "conta_corrente_estoque_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";
                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    var sortedEntries = SortEntries(_entries);
                    var finalBalance = sortedEntries.Length == 0
                        ? 0M
                        : sortedEntries.OrderBy(item => ParseDate(item.MovementDateTime)).ThenBy(item => item.MovementId).Last().RunningBalance;
                    StockLedgerPdfExporter.Export(dialog.FileName, BuildFilterLines(), sortedEntries, finalBalance);
                    _stockLedgerController.RegisterPdfExport(_configuration, _databaseProfile, _identity.UserName, BuildQuery(), _entries.Length);
                    SetStatus("PDF gerado com sucesso.", false);

                    if (MessageBox.Show(this, "PDF salvo com sucesso.\n\nDeseja abrir agora?", "Conta Corrente de Estoque", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                "Data inicial: " + (_startDateTextBox.Text ?? string.Empty),
                "Data final: " + (_endDateTextBox.Text ?? string.Empty),
                "Fornecedor: " + (_supplierComboBox.SelectedItem is LookupOption supplier ? supplier.DisplayText : "TODOS"),
                "Material: " + (_materialComboBox.SelectedItem is LookupOption material ? material.DisplayText : "TODOS"),
                "Lote: " + (_lotComboBox.SelectedItem is LookupOption lot ? lot.DisplayText : "TODOS"),
                "Almoxarifado: " + (_warehouseComboBox.SelectedItem is LookupOption warehouse ? warehouse.DisplayText : "TODOS"),
                "Tipo: " + ((_typeComboBox.SelectedItem as string) ?? "TODOS"),
                "Mostrar inativos: " + (_includeInactiveCheckBox.Checked ? "SIM" : "NAO"),
            };
        }

        private void OnGridColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var column = _grid.Columns[e.ColumnIndex].Name;
            _sortAscending = string.Equals(_sortColumn, column, StringComparison.OrdinalIgnoreCase) ? !_sortAscending : column != "data";
            _sortColumn = column;
            BindGrid();
        }

        private void OnGridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (!(_grid.Rows[e.RowIndex].DataBoundItem is StockLedgerEntry entry))
            {
                return;
            }

            var style = _grid.Rows[e.RowIndex].DefaultCellStyle;
            style.ForeColor = System.Drawing.Color.Black;
            if (string.Equals(entry.RowCategory, "INATIVO", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
                style.ForeColor = System.Drawing.Color.FromArgb(102, 102, 102);
            }
            else if (string.Equals(entry.RowCategory, "INVENTARIO", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = System.Drawing.Color.FromArgb(234, 246, 255);
            }
            else if (string.Equals(entry.RowCategory, "ENTRADA", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = System.Drawing.Color.FromArgb(232, 245, 232);
            }
            else if (string.Equals(entry.RowCategory, "SAIDA", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = System.Drawing.Color.FromArgb(255, 232, 232);
            }
            else if (string.Equals(entry.RowCategory, "TRANSFERENCIA", StringComparison.OrdinalIgnoreCase))
            {
                style.BackColor = System.Drawing.Color.FromArgb(232, 232, 255);
            }
            else
            {
                style.BackColor = e.RowIndex % 2 == 0 ? System.Drawing.Color.FromArgb(248, 248, 248) : System.Drawing.Color.White;
            }
        }

        private void ApplyDefaultPeriod()
        {
            var end = DateTime.Now;
            var start = end.AddDays(-30);
            _startDateTextBox.Text = start.ToString("dd/MM/yyyy");
            _endDateTextBox.Text = end.ToString("dd/MM/yyyy");
        }

        private void ApplyQuickPeriod(int? days)
        {
            if (days == null)
            {
                _startDateTextBox.Clear();
                _endDateTextBox.Clear();
                return;
            }

            var end = DateTime.Now;
            var start = end.AddDays(-days.Value);
            _startDateTextBox.Text = start.ToString("dd/MM/yyyy");
            _endDateTextBox.Text = end.ToString("dd/MM/yyyy");
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.P)
            {
                e.Handled = true;
                ExportCsv();
                return;
            }

            if (e.KeyCode == Keys.F4)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                ClearFilters();
            }
        }

        private StockLedgerQuery BuildQuery()
        {
            return new StockLedgerQuery
            {
                StartDate = _startDateTextBox.Text,
                EndDate = _endDateTextBox.Text,
                SupplierCode = GetSelectedCode(_supplierComboBox),
                MaterialCode = GetSelectedCode(_materialComboBox),
                LotCode = GetSelectedCode(_lotComboBox),
                WarehouseCode = GetSelectedCode(_warehouseComboBox),
                MovementType = GetSelectedMovementType(),
                IncludeInactive = _includeInactiveCheckBox.Checked,
            };
        }

        private string GetSelectedMovementType()
        {
            var value = (_typeComboBox.SelectedItem as string ?? string.Empty).Trim();
            return string.Equals(value, "TODOS", StringComparison.OrdinalIgnoreCase) ? string.Empty : value;
        }

        private void SetSelectedMovementType(string movementType)
        {
            var normalized = (movementType ?? string.Empty).Trim();
            if (normalized.Length == 0)
            {
                _typeComboBox.SelectedIndex = 0;
                return;
            }

            for (var index = 0; index < _typeComboBox.Items.Count; index++)
            {
                if (string.Equals(Convert.ToString(_typeComboBox.Items[index]), normalized, StringComparison.OrdinalIgnoreCase))
                {
                    _typeComboBox.SelectedIndex = index;
                    return;
                }
            }

            _typeComboBox.SelectedIndex = 0;
        }

        private StockLedgerEntry[] SortEntries(StockLedgerEntry[] items)
        {
            var source = items ?? new StockLedgerEntry[0];
            switch ((_sortColumn ?? string.Empty).ToLowerInvariant())
            {
                case "documento": return ApplySort(source, item => item.DocumentDisplay ?? string.Empty, item => item.MovementId);
                case "tipo": return ApplySort(source, item => item.DisplayType ?? string.Empty, item => item.MovementId);
                case "material": return ApplySort(source, item => item.MaterialDisplay ?? string.Empty, item => item.MovementId);
                case "lote": return ApplySort(source, item => item.LotDisplay ?? string.Empty, item => item.MovementId);
                case "validade": return ApplySort(source, item => ParseDate(item.ExpirationDate), item => item.MovementId);
                case "almox": return ApplySort(source, item => item.WarehouseDisplay ?? string.Empty, item => item.MovementId);
                case "fornecedor": return ApplySort(source, item => item.SupplierDisplay ?? string.Empty, item => item.MovementId);
                case "quantidade": return ApplySort(source, item => item.Quantity, item => item.MovementId);
                case "saldo": return ApplySort(source, item => item.RunningBalance, item => item.MovementId);
                case "criacao": return ApplySort(source, item => ParseDate(item.CreatedAt), item => item.MovementId);
                case "usuariocriacao":
                case "usuario": return ApplySort(source, item => item.UserName ?? string.Empty, item => item.MovementId);
                case "status": return ApplySort(source, item => item.Status ?? string.Empty, item => item.MovementId);
                case "data":
                default: return ApplySort(source, item => ParseDate(item.MovementDateTime), item => item.MovementId);
            }
        }

        private StockLedgerEntry[] ApplySort<TKey, TThen>(StockLedgerEntry[] items, Func<StockLedgerEntry, TKey> keySelector, Func<StockLedgerEntry, TThen> thenBy)
        {
            return _sortAscending
                ? items.OrderBy(keySelector).ThenBy(thenBy).ToArray()
                : items.OrderByDescending(keySelector).ThenByDescending(thenBy).ToArray();
        }

        private static DateTime ParseDate(string value)
        {
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "dd/MM/yyyy", "yyyy-MM-dd" };
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

        private static LookupOption[] ToLookupOptions(SupplierSummary[] items)
        {
            return (items ?? new SupplierSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
        }

        private static LookupOption[] ToLookupOptions(PackagingSummary[] items)
        {
            return (items ?? new PackagingSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Description, Status = item.Status }).ToArray();
        }

        private static LookupOption[] ToLookupOptions(WarehouseSummary[] items)
        {
            return (items ?? new WarehouseSummary[0]).Select(item => new LookupOption { Code = item.Code, Description = item.Name, Status = item.Status }).ToArray();
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
                comboBox.SelectedIndex = 0;
                return;
            }

            for (var index = 0; index < options.Length; index++)
            {
                if (string.Equals(options[index].Code, selectedCode, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
            }

            comboBox.SelectedIndex = 0;
        }

        private static string GetSelectedCode(ComboBox comboBox)
        {
            return comboBox.SelectedItem is LookupOption option ? option.Code : string.Empty;
        }

        private static string EscapeCsv(string value)
        {
            var normalized = (value ?? string.Empty).Replace("\"", "\"\"");
            return "\"" + normalized + "\"";
        }
    }
}
