using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class NoteDateChangeForm
    {
        private sealed class DetailRow
        {
            public string Field { get; set; }

            public string Value { get; set; }
        }

        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                // Python: self.data_e.insert(0, datetime.now().strftime("%d/%m/%Y %H:%M"))
                _newDateTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                LoadNotes();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadNotes()
        {
            try
            {
                var selectedNumber = GetSelectedNumber();
                _notes = _databaseMaintenanceController
                    .LoadActiveNotes(_configuration, _databaseProfile)
                    .ToArray();

                _noteComboBox.BeginUpdate();
                _noteComboBox.DataSource    = null;
                _noteComboBox.DisplayMember = nameof(DocumentDateEntry.NoteDisplayLabel);
                _noteComboBox.ValueMember   = nameof(DocumentDateEntry.DocumentNumber);
                _noteComboBox.DataSource    = _notes;
                _noteComboBox.EndUpdate();

                if (!string.IsNullOrWhiteSpace(selectedNumber))
                {
                    for (var index = 0; index < _notes.Length; index++)
                    {
                        if (string.Equals(_notes[index].DocumentNumber, selectedNumber, StringComparison.OrdinalIgnoreCase))
                        {
                            _noteComboBox.SelectedIndex = index;
                            UpdateDetails();
                            return;
                        }
                    }
                }

                if (_notes.Length > 0)
                {
                    _noteComboBox.SelectedIndex = 0;
                    UpdateDetails();
                    SetStatus("Notas de entrada carregadas com sucesso.", false);
                }
                else
                {
                    _noteComboBox.SelectedIndex = -1;
                    ClearDetails();
                    SetStatus("Nenhuma nota de entrada ativa encontrada.", false);
                    MessageBox.Show(this, "Nenhuma nota de entrada ativa encontrada!", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void UpdateDetails()
        {
            var selected = GetSelectedEntry();
            if (selected == null)
            {
                ClearDetails();
                return;
            }

            // Espelha views/bd_alterar_data_entrada.py::_exibir_dados_nota.
            var rows = new List<DetailRow>
            {
                new DetailRow { Field = "No Nota Fiscal",      Value = selected.DocumentNumber ?? string.Empty },
                new DetailRow { Field = "Data/Hora Movimento", Value = FormatIsoToBrazilian(selected.Date) },
                new DetailRow { Field = "Status",              Value = selected.Status ?? string.Empty },
                new DetailRow { Field = "Fornecedor",          Value = FormatCodeName(selected.Supplier, selected.SupplierName) },
                new DetailRow { Field = "Almoxarifado",        Value = FormatCodeName(selected.Warehouse, selected.WarehouseName) },
                new DetailRow { Field = "Total de Itens",      Value = selected.ItemCount.ToString(CultureInfo.InvariantCulture) },
            };

            _detailsGrid.DataSource = rows;
            if (_detailsGrid.Rows.Count > 0)
            {
                _detailsGrid.ClearSelection();
            }
        }

        private void ClearDetails()
        {
            _detailsGrid.DataSource = Array.Empty<DetailRow>();
        }

        private void ClearForm()
        {
            // Python _limpar: set combo=""; reset data; limpar tree.
            _noteComboBox.SelectedIndex = -1;
            _newDateTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            ClearDetails();
            SetStatus(string.Empty, false);
        }

        private void ChangeDate()
        {
            var selected  = GetSelectedEntry();
            var newDateBr = (_newDateTextBox.Text ?? string.Empty).Trim();

            if (selected == null)
            {
                MessageBox.Show(this, "Selecione uma nota de entrada!", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _noteComboBox.Focus();
                return;
            }

            if (newDateBr.Length == 0)
            {
                MessageBox.Show(this, "Informe a nova data/hora!", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _newDateTextBox.Focus();
                return;
            }

            // Equivalente a utils.funcoes.validar_datahora_br + conversao ISO.
            DateTime parsedDate;
            if (!DateTime.TryParseExact(newDateBr, "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                MessageBox.Show(this, "Data/hora invalida. Use o formato DD/MM/YYYY HH:MM",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _newDateTextBox.Focus();
                return;
            }

            if (MessageBox.Show(
                    this,
                    "Deseja alterar a nota " + selected.DocumentNumber + " para:\n\n" + newDateBr + "?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var isoDate = parsedDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var result = _databaseMaintenanceController.ChangeNoteDate(
                    _configuration,
                    _databaseProfile,
                    _identity.UserName,
                    selected.DocumentNumber,
                    selected.Supplier,
                    isoDate);

                MessageBox.Show(
                    this,
                    "Nota " + selected.DocumentNumber + " alterada com sucesso!\n\n"
                    + "Tabelas atualizadas:\n"
                    + " - Notas: " + result.HeaderRowsUpdated + " linha(s)\n"
                    + " - Movimentos: " + result.MovementRowsUpdated + " linha(s)",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                SetStatus("Data da nota alterada com sucesso.", false);
                // Python _alterar chama self._carregar_notas() ao final.
                LoadNotes();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private string GetSelectedNumber()
        {
            var entry = GetSelectedEntry();
            return entry != null ? entry.DocumentNumber ?? string.Empty : string.Empty;
        }

        private DocumentDateEntry GetSelectedEntry()
        {
            return _noteComboBox.SelectedItem as DocumentDateEntry;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                LoadNotes();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private static string FormatIsoToBrazilian(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return "-";
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "yyyy-MM-dd" };
            return DateTime.TryParseExact(rawValue.Trim(), formats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))
                : rawValue;
        }

        private static string FormatCodeName(string code, string name)
        {
            var c = code ?? string.Empty;
            var n = string.IsNullOrWhiteSpace(name) ? "-" : name;
            return c + " - " + n;
        }
    }
}
