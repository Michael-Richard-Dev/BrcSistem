using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed partial class RemoveRequisitionForm
    {
        private void SearchRequisition()
        {
            var number = (_numberTextBox.Text ?? string.Empty).Trim();
            if (number.Length == 0)
            {
                MessageBox.Show(this, "Informe o numero da requisicao.", "Campo Obrigatorio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var header = _databaseMaintenanceController.LoadRequisitionHeader(_configuration, _databaseProfile, number);
                if (header == null)
                {
                    MessageBox.Show(this, "Requisicao " + number + " nao encontrada.", "Nao Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    return;
                }

                _requisitionHeader = header;
                var items = _databaseMaintenanceController.LoadRequisitionItems(_configuration, _databaseProfile, number) ?? Array.Empty<DocumentMaintenanceItem>();

                _headerGrid.DataSource = new List<DetailRow>
                {
                    new DetailRow { Field = "Numero", Value = header.DocumentNumber ?? string.Empty },
                    new DetailRow { Field = "Almoxarifado", Value = header.Warehouse ?? string.Empty },
                    new DetailRow { Field = "Data Movimento", Value = header.Date ?? string.Empty },
                    new DetailRow { Field = "Status", Value = header.Status ?? string.Empty },
                    new DetailRow { Field = "Usuario", Value = string.IsNullOrWhiteSpace(header.UserName) ? "N/A" : header.UserName },
                };
                _itemsGrid.DataSource = items.ToArray();
                _removeButton.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Erro ao buscar requisicao: " + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfirmRemoval()
        {
            if (_requisitionHeader == null)
            {
                MessageBox.Show(this, "Busque uma requisicao antes de remover.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateClosingPeriod(_requisitionHeader.Date))
            {
                return;
            }

            if (MessageBox.Show(this, "Remover requisicao " + _requisitionHeader.DocumentNumber + "?", "Confirmar Remocao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var result = _databaseMaintenanceController.RemoveRequisition(_configuration, _databaseProfile, _identity.UserName, _requisitionHeader.DocumentNumber);
                MessageBox.Show(this, "Requisicao " + result.Number + " removida com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Erro ao remover requisicao: " + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show(this, "Data em periodo de fechamento contabil.", "Periodo Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            _requisitionHeader = null;
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
                SearchRequisition();
            }
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                ClearForm();
                return;
            }

            if (e.KeyCode == Keys.F4)
            {
                e.Handled = true;
                Close();
            }
        }

        private static bool TryParseBrazilianDate(string value, out DateTime parsed)
        {
            var formats = new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss" };
            return DateTime.TryParseExact((value ?? string.Empty).Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
        }
    }
}
