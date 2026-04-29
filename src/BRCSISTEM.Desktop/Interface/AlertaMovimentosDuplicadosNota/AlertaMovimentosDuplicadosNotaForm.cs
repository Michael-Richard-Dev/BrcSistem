using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.AlertaMovimentosDuplicadosNota
{
    /// <summary>
    /// Tela mestre-detalhe de NFs com movimentos duplicados/inconsistentes.
    /// A consulta retorna Groups com Details embedados (cache local) e a
    /// grade inferior e repopulada a partir do Group selecionado.
    /// </summary>
    public sealed partial class AlertaMovimentosDuplicadosNotaForm : Form
    {
        private readonly DatabaseMaintenanceController _maintenanceController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly bool _isDesignerInstance;

        private AppConfiguration _configuration;

        // Cache em memoria (igual ao self._diag_cache do Python)
        private List<DuplicateNoteMovementGroup> _diagCache = new List<DuplicateNoteMovementGroup>();

        public AlertaMovimentosDuplicadosNotaForm()
            : this(null, null, null, true)
        {
        }

        public AlertaMovimentosDuplicadosNotaForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile)
            : this(compositionRoot, identity, databaseProfile, false)
        {
        }

        private AlertaMovimentosDuplicadosNotaForm(
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
            else if (e.KeyCode == Keys.F6) ClearAll();
        }

        private void OnConsultarClick(object sender, EventArgs e)
        {
            RunQuery();
        }

        private void OnLimparClick(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void OnRemoverSelecionadaClick(object sender, EventArgs e)
        {
            RemoveSelectedNote();
        }

        private void OnRemoverTodosClick(object sender, EventArgs e)
        {
            RemoveAllProposed();
        }

        private void OnFecharClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnGridNfSelectionChanged(object sender, EventArgs e)
        {
            PopulateDetailFromSelection();
        }

        // ── Consulta ──────────────────────────────────────────────────────────

        private void RunQuery()
        {
            if (IsDesignModeActive) return;

            try
            {
                ClearAll();
                _infoLabel.Text = "Consultando...";
                _infoLabel.ForeColor = Color.FromArgb(100, 100, 100);
                System.Windows.Forms.Application.DoEvents();

                var groups = _maintenanceController
                    .DiagnoseDuplicateNoteMovements(_configuration, _databaseProfile);

                _diagCache = groups.ToList();
                PopulateNfGrid();

                var totalNf = _diagCache.Count;
                var totalIds = _diagCache.Sum(g => g.DuplicateMovementIds?.Count ?? 0);
                _infoLabel.Text = "NFs: " + totalNf + " | IDs propostos: " + totalIds;
                _infoLabel.ForeColor = totalNf > 0 ? Color.FromArgb(180, 60, 0) : Color.SeaGreen;

                // Seleciona primeira linha (igual Python)
                if (_gridNf.Rows.Count > 0)
                {
                    _gridNf.ClearSelection();
                    _gridNf.Rows[0].Selected = true;
                    _gridNf.CurrentCell = _gridNf.Rows[0].Cells[0];
                }
            }
            catch (Exception ex)
            {
                _infoLabel.Text = string.Empty;
                ShowError("Erro ao consultar duplicidades", ex);
            }
        }

        private void PopulateNfGrid()
        {
            _gridNf.Rows.Clear();
            foreach (var g in _diagCache)
            {
                var ids = g.DuplicateMovementIds ?? new List<long>();
                var idsTxt = string.Join(", ", ids);

                var idx = _gridNf.Rows.Add(
                    g.NoteNumber ?? string.Empty,
                    g.Supplier ?? string.Empty,
                    g.NoteVersion ?? string.Empty,
                    g.NoteUser ?? string.Empty,
                    g.TotalActiveMovements,
                    ids.Count,
                    idsTxt,
                    g.ActiveMovementUsers ?? string.Empty);

                _gridNf.Rows[idx].Tag = g;
                _gridNf.Rows[idx].Cells["qtd_propostas"].Style.ForeColor = Color.FromArgb(180, 60, 0);
                _gridNf.Rows[idx].Cells["qtd_propostas"].Style.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            }
        }

        private void PopulateDetailFromSelection()
        {
            if (IsDesignModeActive) return;

            _gridDet.Rows.Clear();
            var g = GetSelectedGroup();
            if (g == null) return;

            foreach (var d in g.Details ?? new List<DuplicateNoteMovementDetail>())
            {
                _gridDet.Rows.Add(
                    d.MovementId,
                    d.ReasonLabel ?? d.Reason ?? string.Empty,
                    d.MovementUser ?? string.Empty,
                    d.NoteUser ?? string.Empty,
                    d.Material ?? string.Empty,
                    d.Lot ?? string.Empty,
                    d.Warehouse ?? string.Empty,
                    FormatQuantity(d.Quantity),
                    FormatDateTime(d.MovementDate),
                    FormatDateTime(d.CreatedAt),
                    d.KeepReferenceId.HasValue ? d.KeepReferenceId.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
            }
        }

        private DuplicateNoteMovementGroup GetSelectedGroup()
        {
            if (_gridNf.CurrentRow == null) return null;
            return _gridNf.CurrentRow.Tag as DuplicateNoteMovementGroup;
        }

        // ── Limpar ────────────────────────────────────────────────────────────

        private void ClearAll()
        {
            _diagCache = new List<DuplicateNoteMovementGroup>();
            _gridNf.Rows.Clear();
            _gridDet.Rows.Clear();
            _infoLabel.Text = string.Empty;
        }

        // ── Inativar NF selecionada ───────────────────────────────────────────

        private void RemoveSelectedNote()
        {
            if (IsDesignModeActive) return;

            var g = GetSelectedGroup();
            if (g == null)
            {
                MessageBox.Show(this,
                    "Selecione uma NF para remover os duplicados.",
                    "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ids = (g.DuplicateMovementIds ?? new List<long>()).ToArray();
            if (ids.Length == 0)
            {
                MessageBox.Show(this,
                    "Nao ha IDs propostos para esta NF.",
                    "Sem dados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idsTxt = "[" + string.Join(", ", ids) + "]";
            var confirm = MessageBox.Show(this,
                "NF " + g.NoteNumber + " / Fornecedor " + g.Supplier + "\n\n" +
                "IDs propostos para inativar: " + idsTxt + "\n\n" +
                "Deseja continuar?",
                "Confirmar limpeza",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            ExecuteInactivation(ids, g.NoteNumber);
        }

        // ── Inativar todos os propostos ───────────────────────────────────────

        private void RemoveAllProposed()
        {
            if (IsDesignModeActive) return;

            var ids = _diagCache
                .SelectMany(g => g.DuplicateMovementIds ?? Enumerable.Empty<long>())
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            if (ids.Length == 0)
            {
                MessageBox.Show(this,
                    "Nao ha IDs propostos para remover.",
                    "Sem dados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idsTxt = "[" + string.Join(", ", ids) + "]";
            var confirm = MessageBox.Show(this,
                "Total de IDs propostos: " + ids.Length + "\n\n" +
                "IDs: " + idsTxt + "\n\n" +
                "Deseja inativar todos?",
                "Confirmar limpeza geral",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            ExecuteInactivation(ids, noteNumber: null);
        }

        private void ExecuteInactivation(long[] ids, string noteNumber)
        {
            try
            {
                var result = _maintenanceController.InactivateDuplicateNoteMovements(
                    _configuration,
                    _databaseProfile,
                    _identity.UserName,
                    ids,
                    noteNumber ?? string.Empty);

                MessageBox.Show(this,
                    "IDs solicitados: [" + string.Join(", ", result.RequestedIds) + "]\n" +
                    "IDs encontrados: [" + string.Join(", ", result.FoundIds) + "]\n" +
                    "IDs inativados: [" + string.Join(", ", result.InactivatedIds) + "]\n" +
                    "Total inativado: " + result.TotalInactivated,
                    "Limpeza concluida",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RunQuery(); // recarrega (igual Python)
            }
            catch (Exception ex)
            {
                ShowError("Erro ao inativar duplicados", ex);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Tenta varios formatos ISO/BR e retorna dd/MM/yyyy HH:mm.
        /// </summary>
        private static string FormatDateTime(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            var txt = raw.Trim();
            if (txt.Length > 19) txt = txt.Substring(0, 19);

            string[] fmts =
            {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm",
            };
            if (DateTime.TryParseExact(txt, fmts, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            }
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {
                return dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            }
            return raw;
        }

        private static string FormatQuantity(decimal value)
        {
            return value.ToString("N3", CultureInfo.GetCultureInfo("pt-BR"));
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
