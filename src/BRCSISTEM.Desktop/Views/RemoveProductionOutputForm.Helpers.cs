using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class RemoveProductionOutputForm
    {
        private void SearchOutput()
        {
            var number = (_numberTextBox.Text ?? string.Empty).Trim();
            if (number.Length == 0)
            {
                MessageBox.Show(this, "Informe o numero da saida.", "Campo Obrigatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var header = _databaseMaintenanceController.LoadProductionOutputHeader(_configuration, _databaseProfile, number);
                if (header == null)
                {
                    MessageBox.Show(this, "Saida " + number + " nao encontrada.", "Nao Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    return;
                }

                _outputHeader = header;
                var items = _databaseMaintenanceController.LoadProductionOutputItems(_configuration, _databaseProfile, number) ?? Array.Empty<DocumentMaintenanceItem>();

                _headerGrid.DataSource = new List<DetailRow>
                {
                    new DetailRow { Field = "Tipo", Value = "Saida de Producao" },
                    new DetailRow { Field = "Numero", Value = header.DocumentNumber ?? string.Empty },
                    new DetailRow { Field = "Finalidade", Value = header.Purpose ?? string.Empty },
                    new DetailRow { Field = "Data Movimento", Value = header.Date ?? string.Empty },
                    new DetailRow { Field = "Status", Value = header.Status ?? string.Empty },
                };
                _itemsGrid.DataSource = items.ToArray();
                _removeButton.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Erro ao buscar saida: " + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfirmRemoval()
        {
            if (_outputHeader == null)
            {
                MessageBox.Show(this, "Busque uma saida antes de remover.", "Saida Nao Selecionada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateClosingPeriod(_outputHeader.Date))
            {
                return;
            }

            var confirmation = " ATENCAO!\n\nDeseja REMOVER PERMANENTEMENTE a saida "
                + _outputHeader.DocumentNumber
                + "?\n\nEsta operacao NAO pode ser desfeita!";

            if (MessageBox.Show(this, confirmation, "Confirmar Remocao", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var result = _databaseMaintenanceController.RemoveProductionOutput(_configuration, _databaseProfile, _identity.UserName, _outputHeader.DocumentNumber);
                MessageBox.Show(this,
                    "Saida " + result.Number + " removida com sucesso!\n\nItens: " + result.RemovedItems + "\nMovimentos: " + result.RemovedMovements,
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                ClearForm();
                _numberTextBox.Focus();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Erro ao remover saida: " + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateClosingPeriod(string movementDate)
        {
            try
            {
                var parameters = _databaseMaintenanceController.LoadSystemParameters(_configuration, _databaseProfile) ?? Array.Empty<BRCSISTEM.Domain.Models.SystemParameter>();
                var closing = parameters.FirstOrDefault(p => string.Equals(p.Key, "fechamento_contabil", StringComparison.OrdinalIgnoreCase));
                var closingText = (closing?.Value ?? string.Empty).Trim();
                if (closingText.Length == 0)
                {
                    return true;
                }

                DateTime movement;
                if (!TryParseBrazilianDate(movementDate, out movement))
                {
                    return true;
                }

                DateTime closingDate;
                if (!DateTime.TryParseExact(closingText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out closingDate))
                {
                    return true;
                }

                if (movement.Date <= closingDate.Date)
                {
                    MessageBox.Show(this,
                        "A data de movimento (" + movementDate + ") esta no periodo de fechamento contabil.\n\nFechamento ate: " + closingText,
                        "Periodo Bloqueado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
            catch
            {
                return true;
            }
        }

        private void ClearForm()
        {
            _numberTextBox.Text = string.Empty;
            _outputHeader = null;
            _headerGrid.DataSource = Array.Empty<DetailRow>();
            _itemsGrid.DataSource = Array.Empty<DocumentMaintenanceItem>();
            _removeButton.Enabled = false;
        }

        private void OnNumberKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SearchOutput();
            }
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                ClearForm();
            }
        }

        private static bool TryParseBrazilianDate(string value, out DateTime parsed)
        {
            var formats = new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" };
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
        }
    }
}
