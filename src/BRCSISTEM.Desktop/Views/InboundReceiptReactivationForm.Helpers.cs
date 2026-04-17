using System;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class InboundReceiptReactivationForm
    {
        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                BindEntries(Array.Empty<InboundReceiptReactivationEntry>());
                SetStatus("Use os filtros ou clique em 'Todos Cancelados' para consultar notas canceladas.", false);
            }
            catch (Exception exception)
            {
                ShowError("Erro ao carregar tela", exception);
            }
        }

        private void SearchCancelledReceipts()
        {
            try
            {
                var number = DigitsOnly(_numberTextBox.Text);
                var supplier = DigitsOnly(_supplierTextBox.Text);
                var results = _databaseMaintenanceController
                    .SearchCancelledInboundReceipts(_configuration, _databaseProfile, number, supplier, 0)
                    .ToArray();

                BindEntries(results);

                if (results.Length == 0)
                {
                    SetStatus("Nenhuma nota cancelada encontrada com os criterios informados.", false);
                    MessageBox.Show(this, "Nenhuma nota cancelada encontrada com os criterios informados.", "Nenhum resultado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetStatus("Pesquisa concluida com sucesso.", false);
                MessageBox.Show(this, "Encontradas " + results.Length + " nota(s) cancelada(s).", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                ShowError("Erro ao pesquisar notas", exception);
            }
        }

        private void LoadAllCancelledReceipts()
        {
            try
            {
                _numberTextBox.Clear();
                _supplierTextBox.Clear();

                var results = _databaseMaintenanceController
                    .SearchCancelledInboundReceipts(_configuration, _databaseProfile, string.Empty, string.Empty, 100)
                    .ToArray();

                BindEntries(results);

                if (results.Length == 0)
                {
                    SetStatus("Nenhuma nota cancelada encontrada no banco de dados.", false);
                    MessageBox.Show(this, "Nenhuma nota cancelada encontrada no banco de dados.", "Sem resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SetStatus("Notas canceladas carregadas com sucesso.", false);
                MessageBox.Show(this, "Total de " + results.Length + " nota(s) cancelada(s) encontrada(s).", "Resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                ShowError("Erro ao listar notas", exception);
            }
        }

        private void ReactivateSelectedReceipt()
        {
            var selected = GetSelectedEntry();
            if (selected == null)
            {
                MessageBox.Show(this, "Selecione uma nota para reativar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmation = MessageBox.Show(
                this,
                "Realmente deseja reativar a nota " + selected.Number + " do fornecedor " + selected.Supplier + "?\n\n"
                + "Esta acao ira:\n"
                + "- Voltar status para ATIVO\n"
                + "- Reativar todos os itens\n"
                + "- Reativar todos os movimentos",
                "Confirmar Reativacao",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _databaseMaintenanceController.ReactivateInboundReceipt(
                    _configuration,
                    _databaseProfile,
                    _identity.UserName,
                    selected.Number,
                    selected.Supplier,
                    selected.Version);

                MessageBox.Show(
                    this,
                    "Nota " + selected.Number + " reativada com sucesso!\n\n"
                    + "- Status alterado de CANCELADA para ATIVO\n"
                    + "- Todos os itens reativados\n"
                    + "- Todos os movimentos reativados\n\n"
                    + "Logs registrados em auditoria.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                ClearFilters();
                LoadAllCancelledReceipts();
            }
            catch (Exception exception)
            {
                ShowError("Erro ao Reativar", exception);
            }
        }

        private void ClearFilters()
        {
            _numberTextBox.Clear();
            _supplierTextBox.Clear();
            BindEntries(Array.Empty<InboundReceiptReactivationEntry>());
            SetStatus(string.Empty, false);
        }

        private void BindEntries(InboundReceiptReactivationEntry[] entries)
        {
            _entries = entries ?? Array.Empty<InboundReceiptReactivationEntry>();
            _grid.DataSource = null;
            _grid.DataSource = _entries;
            if (_grid.Rows.Count > 0)
            {
                _grid.ClearSelection();
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private InboundReceiptReactivationEntry GetSelectedEntry()
        {
            if (_grid.CurrentRow == null || _grid.CurrentRow.Index < 0 || _grid.CurrentRow.Index >= _entries.Length)
            {
                return null;
            }

            return _grid.CurrentRow.DataBoundItem as InboundReceiptReactivationEntry;
        }

        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SearchCancelledReceipts();
            }
        }

        private static string DigitsOnly(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return new string(value.Where(char.IsDigit).ToArray());
        }
    }
}
