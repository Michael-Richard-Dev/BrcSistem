using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class ProductionOutputDateChangeForm
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
                LoadProductionOutputs();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadProductionOutputs()
        {
            try
            {
                var selectedNumber = GetSelectedNumber();
                _productionOutputs = _databaseMaintenanceController.LoadActiveProductionOutputs(_configuration, _databaseProfile).ToArray();

                _productionOutputComboBox.BeginUpdate();
                _productionOutputComboBox.DataSource = null;
                _productionOutputComboBox.DisplayMember = nameof(DocumentDateEntry.ProductionOutputDisplayLabel);
                _productionOutputComboBox.ValueMember = nameof(DocumentDateEntry.DocumentNumber);
                _productionOutputComboBox.DataSource = _productionOutputs;
                _productionOutputComboBox.EndUpdate();

                if (!string.IsNullOrWhiteSpace(selectedNumber))
                {
                    for (var index = 0; index < _productionOutputs.Length; index++)
                    {
                        if (string.Equals(_productionOutputs[index].DocumentNumber, selectedNumber, StringComparison.OrdinalIgnoreCase))
                        {
                            _productionOutputComboBox.SelectedIndex = index;
                            UpdateDetails();
                            return;
                        }
                    }
                }

                if (_productionOutputs.Length > 0)
                {
                    _productionOutputComboBox.SelectedIndex = 0;
                    UpdateDetails();
                    SetStatus("Saidas de producao carregadas com sucesso.", false);
                }
                else
                {
                    _productionOutputComboBox.SelectedIndex = -1;
                    ClearDetails();
                    SetStatus("Nenhuma saida de producao ativa encontrada.", false);
                    MessageBox.Show(this, "Nenhuma saida de producao ativa encontrada!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                new DetailRow { Field = "No Saida", Value = selected.DocumentNumber ?? string.Empty },
                new DetailRow { Field = "Data/Hora Atual", Value = FormatIsoToBrazilian(selected.Date) },
                new DetailRow { Field = "Status", Value = selected.Status ?? string.Empty },
                new DetailRow { Field = "Turno", Value = string.IsNullOrWhiteSpace(selected.Shift) ? "-" : selected.Shift },
                new DetailRow { Field = "Finalidade", Value = string.IsNullOrWhiteSpace(selected.Purpose) ? "-" : selected.Purpose },
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
            _productionOutputComboBox.SelectedIndex = -1;
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
                MessageBox.Show(this, "Selecione uma saida de producao!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _productionOutputComboBox.Focus();
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
                    "Deseja alterar a saida " + selected.DocumentNumber + " para:\n\n" + newDateBr + "?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var isoDate = parsedDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                var result = _databaseMaintenanceController.ChangeProductionOutputDate(
                    _configuration,
                    _databaseProfile,
                    _identity.UserName,
                    selected.DocumentNumber,
                    isoDate);

                MessageBox.Show(
                    this,
                    "Saida " + selected.DocumentNumber + " alterada com sucesso!\n\n"
                    + "Tabelas atualizadas:\n"
                    + " - Saidas: " + result.HeaderRowsUpdated + " linha(s)\n"
                    + " - Movimentos: " + result.MovementRowsUpdated + " linha(s)",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                SetStatus("Data da saida alterada com sucesso.", false);
                LoadProductionOutputs();
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
            return _productionOutputComboBox.SelectedItem as DocumentDateEntry;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                LoadProductionOutputs();
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
            return DateTime.TryParseExact(rawValue.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed.ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"))
                : rawValue;
        }
    }
}
