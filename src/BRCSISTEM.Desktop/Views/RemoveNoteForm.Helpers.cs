using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class RemoveNoteForm
    {
        private void SearchNote()
        {
            var number = (_numberTextBox.Text ?? string.Empty).Trim();
            var supplier = (_supplierTextBox.Text ?? string.Empty).Trim();

            if (number.Length == 0 || supplier.Length == 0)
            {
                MessageBox.Show(this, "Informe numero e fornecedor.", "Campos Obrigatorios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var header = _databaseMaintenanceController.LoadNoteHeader(_configuration, _databaseProfile, number, supplier);
                if (header == null)
                {
                    MessageBox.Show(this, "Nota " + number + " do fornecedor " + supplier + " nao encontrada.", "Nao Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    return;
                }

                _noteHeader = header;
                var items = _databaseMaintenanceController.LoadNoteItems(_configuration, _databaseProfile, number, supplier) ?? Array.Empty<DocumentMaintenanceItem>();

                _headerGrid.DataSource = new List<DetailRow>
                {
                    new DetailRow { Field = "Numero", Value = header.DocumentNumber ?? string.Empty },
                    new DetailRow { Field = "Fornecedor", Value = header.Supplier ?? string.Empty },
                    new DetailRow { Field = "Almoxarifado", Value = header.Warehouse ?? string.Empty },
                    new DetailRow { Field = "Data Emissao", Value = header.EmissionDate ?? string.Empty },
                    new DetailRow { Field = "Data Movimento", Value = header.Date ?? string.Empty },
                    new DetailRow { Field = "Status", Value = header.Status ?? string.Empty },
                    new DetailRow { Field = "Usuario", Value = string.IsNullOrWhiteSpace(header.UserName) ? "N/A" : header.UserName },
                };
                _itemsGrid.DataSource = items.ToArray();
                _removeButton.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Erro ao buscar nota: " + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfirmRemoval()
        {
            if (_noteHeader == null)
            {
                MessageBox.Show(this, "Busque uma nota antes de remover.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ValidateClosingPeriod(_noteHeader.Date);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Periodo Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show(this, "Remover nota " + _noteHeader.DocumentNumber + "?", "Confirmar Remocao", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _databaseMaintenanceController.RemoveNote(_configuration, _databaseProfile, _identity.UserName, _noteHeader.DocumentNumber, _noteHeader.Supplier);
                MessageBox.Show(this, "Nota " + _noteHeader.DocumentNumber + " removida com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, "Erro ao remover nota: " + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateClosingPeriod(string movementDate)
        {
            var parameters = _databaseMaintenanceController.LoadSystemParameters(_configuration, _databaseProfile) ?? Array.Empty<BRCSISTEM.Domain.Models.SystemParameter>();
            var closing = parameters.FirstOrDefault(p =>
                string.Equals(p.Key, "fechamento_contabil", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(p.Key, "data_fechamento", StringComparison.OrdinalIgnoreCase));

            var closingText = (closing?.Value ?? string.Empty).Trim();
            if (closingText.Length == 0)
            {
                return;
            }

            DateTime movement;
            if (!TryParseBrazilianDate(movementDate, out movement))
            {
                return;
            }

            DateTime closingDate;
            if (!TryParseBrazilianDate(closingText, out closingDate))
            {
                return;
            }

            if (movement.Date <= closingDate.Date)
            {
                throw new InvalidOperationException("Data em periodo de fechamento contabil.");
            }
        }

        private void ClearForm()
        {
            _numberTextBox.Text = string.Empty;
            _supplierTextBox.Text = string.Empty;
            _noteHeader = null;
            _headerGrid.DataSource = Array.Empty<DetailRow>();
            _itemsGrid.DataSource = Array.Empty<DocumentMaintenanceItem>();
            _removeButton.Enabled = false;
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
