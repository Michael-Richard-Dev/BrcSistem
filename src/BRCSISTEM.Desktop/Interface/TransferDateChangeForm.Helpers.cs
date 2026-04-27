using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class TransferDateChangeForm
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
                _newDateTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                LoadTransfers();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadTransfers()
        {
            try
            {
                var selectedNumber = GetSelectedNumber();
                _transfers = _databaseMaintenanceController.LoadActiveTransfers(_configuration, _databaseProfile).ToArray();

                _transferComboBox.BeginUpdate();
                _transferComboBox.DataSource = null;
                _transferComboBox.DisplayMember = nameof(DocumentDateEntry.TransferDisplayLabel);
                _transferComboBox.ValueMember = nameof(DocumentDateEntry.DocumentNumber);
                _transferComboBox.DataSource = _transfers;
                _transferComboBox.EndUpdate();

                if (!string.IsNullOrWhiteSpace(selectedNumber))
                {
                    for (var index = 0; index < _transfers.Length; index++)
                    {
                        if (string.Equals(_transfers[index].DocumentNumber, selectedNumber, StringComparison.OrdinalIgnoreCase))
                        {
                            _transferComboBox.SelectedIndex = index;
                            UpdateDetails();
                            return;
                        }
                    }
                }

                if (_transfers.Length > 0)
                {
                    _transferComboBox.SelectedIndex = 0;
                    UpdateDetails();
                    SetStatus("Transferencias carregadas com sucesso.", false);
                }
                else
                {
                    _transferComboBox.SelectedIndex = -1;
                    ClearDetails();
                    SetStatus("Nenhuma transferencia ativa encontrada.", false);
                    MessageBox.Show(this, "Nenhuma transferencia ativa encontrada!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            var rows = new List<DetailRow>
            {
                new DetailRow { Field = "No Transferencia", Value = selected.DocumentNumber ?? string.Empty },
                new DetailRow { Field = "Data/Hora Atual", Value = FormatIsoToBrazilian(selected.Date) },
                new DetailRow { Field = "Status", Value = selected.Status ?? string.Empty },
                new DetailRow { Field = "Almox Origem", Value = FormatWarehouse(selected.OriginWarehouse, selected.OriginWarehouseName) },
                new DetailRow { Field = "Almox Destino", Value = FormatWarehouse(selected.DestinationWarehouse, selected.DestinationWarehouseName) },
                new DetailRow { Field = "Total de Itens", Value = selected.ItemCount.ToString(CultureInfo.InvariantCulture) },
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
            _transferComboBox.SelectedIndex = -1;
            _newDateTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            ClearDetails();
            SetStatus(string.Empty, false);
        }

        private void ChangeDate()
        {
            var selected = GetSelectedEntry();
            var newDateBr = (_newDateTextBox.Text ?? string.Empty).Trim();

            if (selected == null)
            {
                MessageBox.Show(this, "Selecione uma transferencia!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _transferComboBox.Focus();
                return;
            }

            if (newDateBr.Length == 0)
            {
                MessageBox.Show(this, "Informe a nova data/hora!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _newDateTextBox.Focus();
                return;
            }

            DateTime parsedDate;
            if (!DateTime.TryParseExact(newDateBr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                MessageBox.Show(this, "Data/hora invalida. Use o formato DD/MM/YYYY HH:MM", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _newDateTextBox.Focus();
                return;
            }

            if (MessageBox.Show(
                    this,
                    "Deseja alterar a transferencia " + selected.DocumentNumber + " para:\n\n" + newDateBr + "?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var isoDate = parsedDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var result = _databaseMaintenanceController.ChangeTransferDate(
                    _configuration,
                    _databaseProfile,
                    _identity.UserName,
                    selected.DocumentNumber,
                    isoDate);

                MessageBox.Show(
                    this,
                    "Transferencia " + selected.DocumentNumber + " alterada com sucesso!\n\n"
                    + "Tabelas atualizadas:\n"
                    + " - Transferencias: " + result.HeaderRowsUpdated + " linha(s)\n"
                    + " - Movimentos: " + result.MovementRowsUpdated + " linha(s)",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                SetStatus("Data da transferencia alterada com sucesso.", false);
                LoadTransfers();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private string GetSelectedNumber()
        {
            return GetSelectedEntry()?.DocumentNumber ?? string.Empty;
        }

        private DocumentDateEntry GetSelectedEntry()
        {
            return _transferComboBox.SelectedItem as DocumentDateEntry;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                LoadTransfers();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private static string FormatWarehouse(string code, string name)
        {
            var normalizedCode = string.IsNullOrWhiteSpace(code) ? "-" : code.Trim();
            var normalizedName = string.IsNullOrWhiteSpace(name) ? "-" : name.Trim();
            return normalizedCode + " - " + normalizedName;
        }

        private static string FormatIsoToBrazilian(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return "-";
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "dd/MM/yyyy HH:mm", "yyyy-MM-dd" };
            return DateTime.TryParseExact(rawValue.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))
                : rawValue;
        }
    }
}
