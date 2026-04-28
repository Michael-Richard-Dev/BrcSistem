using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.AlertaEntradaLoteDivergente
{
    /// <summary>
    /// Tela de alerta para movimentos de entrada (NOTA) cujo lote registrado
    /// nao coincide com o lote dos itens da nota fiscal. Permite consultar
    /// e inativar individualmente o movimento divergente.
    /// </summary>
    public sealed partial class AlertaEntradaLoteDivergenteForm : Form
    {
        private readonly DatabaseMaintenanceController _maintenanceController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly bool _isDesignerInstance;

        private AppConfiguration _configuration;

        public AlertaEntradaLoteDivergenteForm()
            : this(null, null, null, true)
        {
        }

        public AlertaEntradaLoteDivergenteForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile)
            : this(compositionRoot, identity, databaseProfile, false)
        {
        }

        private AlertaEntradaLoteDivergenteForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile,
            bool designerCtor)
        {
            _isDesignerInstance = designerCtor;

            if (!designerCtor)
            {
                _maintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
                _configurationController = compositionRoot.CreateConfigurationController();
                _identity = identity;
                _databaseProfile = databaseProfile;
            }

            InitializeComponent();
        }

        private bool IsDesignModeActive
        {
            get
            {
                if (_isDesignerInstance) return true;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;
                if (DesignMode) return true;
                return Site != null && Site.DesignMode;
            }
        }

        // ── Eventos de tela ───────────────────────────────────────────────────

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;

            try
            {
                _configuration = _configurationController.LoadConfiguration();
            }
            catch (Exception ex)
            {
                ShowError("Erro ao carregar tela", ex);
            }
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4) Close();
            else if (e.KeyCode == Keys.F5) RunQuery();
            else if (e.KeyCode == Keys.F7) FixSelected();
        }

        private void OnConsultarClick(object sender, EventArgs e)
        {
            RunQuery();
        }

        private void OnFixClick(object sender, EventArgs e)
        {
            FixSelected();
        }

        private void OnFecharClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnGridSelectionChanged(object sender, EventArgs e)
        {
            _fixButton.Enabled = _grid.CurrentRow != null && _grid.CurrentRow.Tag is long;
        }

        // ── Consulta ──────────────────────────────────────────────────────────

        private void RunQuery()
        {
            if (IsDesignModeActive) return;

            try
            {
                _grid.Rows.Clear();
                _fixButton.Enabled = false;
                _infoLabel.Text = "Consultando...";
                _infoLabel.ForeColor = Color.FromArgb(100, 100, 100);
                System.Windows.Forms.Application.DoEvents();

                var entries = _maintenanceController.DiagnoseDivergentLotEntries(
                    _configuration,
                    _databaseProfile);

                PopulateGrid(entries);

                _infoLabel.Text = "Registros: " + entries.Count;
                _infoLabel.ForeColor = entries.Count > 0
                    ? Color.FromArgb(180, 60, 0)
                    : Color.SeaGreen;
            }
            catch (Exception ex)
            {
                _infoLabel.Text = string.Empty;
                ShowError("Erro ao consultar alertas", ex);
            }
        }

        private void PopulateGrid(IReadOnlyCollection<DivergentLotEntry> entries)
        {
            _grid.Rows.Clear();
            foreach (var entry in entries)
            {
                var materialDisplay = FormatCodeName(entry.Material, entry.MaterialName);
                var dataBr = FormatDateTime(entry.CreatedAt);
                var qtdMovFmt = FormatDecimal(entry.Quantity);
                var qtdNotaFmt = FormatDecimal(entry.QuantityInNoteItem);

                var idx = _grid.Rows.Add(
                    entry.DocumentNumber ?? string.Empty,
                    materialDisplay,
                    entry.LotInMovement ?? string.Empty,
                    entry.LotInNoteItem ?? string.Empty,
                    entry.Warehouse ?? string.Empty,
                    entry.Supplier ?? string.Empty,
                    qtdMovFmt,
                    qtdNotaFmt,
                    entry.MovementUser ?? string.Empty,
                    entry.NoteUser ?? string.Empty,
                    dataBr);

                // guarda o MovementId na Tag da linha para uso na acao de inativar
                _grid.Rows[idx].Tag = entry.MovementId;

                // destaca a coluna "LOTE MOVIMENTO" em laranja para evidenciar a divergencia
                _grid.Rows[idx].Cells["lote_movimento"].Style.ForeColor = Color.FromArgb(180, 60, 0);
                _grid.Rows[idx].Cells["lote_movimento"].Style.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            }
        }

        // ── Inativar selecionado ──────────────────────────────────────────────

        private void FixSelected()
        {
            if (IsDesignModeActive) return;
            if (_grid.CurrentRow == null) return;

            var movementId = (long)_grid.CurrentRow.Tag;
            var docNumber = _grid.CurrentRow.Cells["documento"].Value as string ?? string.Empty;
            var material = _grid.CurrentRow.Cells["material"].Value as string ?? string.Empty;
            var loteOrf = _grid.CurrentRow.Cells["lote_movimento"].Value as string ?? string.Empty;

            var confirm = MessageBox.Show(
                this,
                "Deseja inativar o movimento de entrada com lote divergente?\n\n" +
                "  Documento  : " + docNumber + "\n" +
                "  Material   : " + material + "\n" +
                "  Lote (mov.): " + loteOrf + "\n" +
                "  ID         : " + movementId + "\n\n" +
                "Esta operacao nao pode ser desfeita.",
                "Confirmar Inativacao",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _maintenanceController.FixDivergentLotEntry(
                    _configuration,
                    _databaseProfile,
                    _identity.UserName,
                    movementId);

                MessageBox.Show(
                    this,
                    "Movimento inativado com sucesso.",
                    "Operacao Concluida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RunQuery(); // recarrega a grade
            }
            catch (Exception ex)
            {
                ShowError("Erro ao inativar movimento", ex);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string FormatCodeName(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            return string.IsNullOrWhiteSpace(name) ? code : code + " - " + name;
        }

        /// <summary>Converte datetime ISO para BR dd/MM/yyyy HH:mm (mostra data+hora da criacao).</summary>
        private static string FormatDateTime(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            }
            return raw;
        }

        private static string FormatDecimal(decimal value)
        {
            return value.ToString("N3", CultureInfo.CurrentCulture);
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
