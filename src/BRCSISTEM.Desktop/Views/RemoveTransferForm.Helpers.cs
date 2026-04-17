using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class RemoveTransferForm
    {
        private void SearchTransfer()
        {
            var number = (_numberTextBox.Text ?? string.Empty).Trim();
            if (number.Length == 0)
            {
                MessageBox.Show(this, "Informe o numero da transferencia.", "Campo Obrigatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var header = _databaseMaintenanceController.LoadTransferHeader(_configuration, _databaseProfile, number);
                if (header == null)
                {
                    MessageBox.Show(this, "Transferencia " + number + " nao encontrada.", "Nao Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    return;
                }

                _transferHeader = header;
                var items = _databaseMaintenanceController.LoadTransferItems(_configuration, _databaseProfile, number) ?? Array.Empty<DocumentMaintenanceItem>();

                _headerGrid.DataSource = new List<DetailRow>
                {
                    new DetailRow { Field = "Numero", Value = header.DocumentNumber ?? string.Empty },
                    new DetailRow { Field = "Data Movimento", Value = header.Date ?? string.Empty },
                    new DetailRow { Field = "Almoxarifado Origem", Value = header.OriginWarehouse ?? string.Empty },
                    new DetailRow { Field = "Almoxarifado Destino", Value = header.DestinationWarehouse ?? string.Empty },
                    new DetailRow { Field = "Status", Value = header.Status ?? string.Empty },
                };
                _itemsGrid.DataSource = items.ToArray();
                _removeButton.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Erro ao buscar transferencia: " + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfirmRemoval()
        {
            if (_transferHeader == null)
            {
                MessageBox.Show(this, "Busque uma transferencia antes de remover.", "Transferencia Nao Selecionada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateClosingPeriod(_transferHeader.Date))
            {
                return;
            }

            var confirmation = " ATENCAO!\n\nDeseja REMOVER PERMANENTEMENTE a transferencia "
                + _transferHeader.DocumentNumber
                + "?\n\nEsta operacao NAO pode ser desfeita!";

            if (MessageBox.Show(this, confirmation, "Confirmar Remocao", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var result = _databaseMaintenanceController.RemoveTransfer(_configuration, _databaseProfile, _identity.UserName, _transferHeader.DocumentNumber);
                MessageBox.Show(this,
                    "Transferencia " + result.Number + " removida com sucesso!\n\nItens: " + result.RemovedItems + "\nMovimentos: " + result.RemovedMovements,
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                ClearForm();
                _numberTextBox.Focus();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Erro ao remover transferencia: " + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            _transferHeader = null;
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
                SearchTransfer();
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
