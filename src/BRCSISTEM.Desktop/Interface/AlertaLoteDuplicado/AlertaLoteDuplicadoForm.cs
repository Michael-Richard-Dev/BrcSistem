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

namespace BRCSISTEM.Desktop.Interface.AlertaLoteDuplicado
{
    /// <summary>
    /// Tela de alerta para lotes ativos com a mesma descricao dentro do mesmo
    /// material. Permite filtrar por material, descricao do lote ou codigo do
    /// lote e listar os duplicados detectados pelo backend.
    /// </summary>
    public sealed partial class AlertaLoteDuplicadoForm : Form
    {
        private readonly DatabaseMaintenanceController _maintenanceController;
        private readonly MasterDataController _masterDataController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly bool _isDesignerInstance;

        private AppConfiguration _configuration;

        // Cache das embalagens para manter o par codigo->label
        private PackagingSummary[] _packagings = new PackagingSummary[0];

        public AlertaLoteDuplicadoForm()
            : this(null, null, null, true)
        {
        }

        public AlertaLoteDuplicadoForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile)
            : this(compositionRoot, identity, databaseProfile, false)
        {
        }

        private AlertaLoteDuplicadoForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile,
            bool designerCtor)
        {
            _isDesignerInstance = designerCtor;

            if (!designerCtor)
            {
                _maintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
                _masterDataController = compositionRoot.CreateMasterDataController();
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
                LoadMaterialCombo();
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
            else if (e.KeyCode == Keys.F6) ClearFilters();
        }

        private void OnFilterTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                RunQuery();
            }
        }

        private void OnConsultarClick(object sender, EventArgs e)
        {
            RunQuery();
        }

        private void OnLimparClick(object sender, EventArgs e)
        {
            ClearFilters();
        }

        private void OnFecharClick(object sender, EventArgs e)
        {
            Close();
        }

        // ── Carga inicial ─────────────────────────────────────────────────────

        private void LoadMaterialCombo()
        {
            _packagings = _masterDataController
                .LoadPackagings(_configuration, _databaseProfile)
                .Where(p => string.Equals(p.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Description, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            _materialComboBox.Items.Clear();
            _materialComboBox.Items.Add(string.Empty); // opcao "todos"

            foreach (var p in _packagings)
            {
                var label = string.IsNullOrWhiteSpace(p.Description)
                    ? p.Code
                    : p.Code + " - " + p.Description;
                _materialComboBox.Items.Add(label);
            }

            _materialComboBox.SelectedIndex = 0;
        }

        // ── Consulta ──────────────────────────────────────────────────────────

        private void RunQuery()
        {
            if (IsDesignModeActive) return;

            try
            {
                _grid.Rows.Clear();
                _infoLabel.Text = "Consultando...";
                _infoLabel.ForeColor = Color.FromArgb(100, 100, 100);
                System.Windows.Forms.Application.DoEvents();

                var filterMaterial = ExtractMaterialCode(_materialComboBox.Text);
                var filterLotDesc = _lotDescriptionTextBox.Text.Trim();
                var filterLotCode = _lotCodeTextBox.Text.Trim();

                var entries = _maintenanceController.DiagnoseDuplicateLotsByMaterial(
                    _configuration,
                    _databaseProfile,
                    filterMaterial,
                    filterLotDesc,
                    filterLotCode);

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

        private void PopulateGrid(IReadOnlyCollection<DuplicateLotEntry> entries)
        {
            _grid.Rows.Clear();
            foreach (var entry in entries)
            {
                var materialTxt = FormatCodeName(entry.Material, entry.MaterialName);
                var fornecedorTxt = FormatCodeName(entry.Supplier, entry.SupplierName);
                var validadeTxt = FormatDate(entry.Validity);

                _grid.Rows.Add(
                    materialTxt,
                    entry.LotName ?? string.Empty,
                    entry.LotCode ?? string.Empty,
                    fornecedorTxt,
                    validadeTxt,
                    entry.DuplicateCount,
                    entry.GroupCodes ?? string.Empty);
            }
        }

        // ── Limpeza ───────────────────────────────────────────────────────────

        private void ClearFilters()
        {
            if (IsDesignModeActive) return;

            _materialComboBox.SelectedIndex = 0;
            _lotDescriptionTextBox.Clear();
            _lotCodeTextBox.Clear();
            _grid.Rows.Clear();
            _infoLabel.Text = string.Empty;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>Extrai o codigo (parte antes do " - ") do texto selecionado no combo.</summary>
        private static string ExtractMaterialCode(string comboText)
        {
            if (string.IsNullOrWhiteSpace(comboText)) return string.Empty;
            var idx = comboText.IndexOf(" - ", StringComparison.Ordinal);
            return idx >= 0 ? comboText.Substring(0, idx).Trim() : comboText.Trim();
        }

        /// <summary>Formata "CODIGO - NOME" ou so "CODIGO" se nome estiver vazio.</summary>
        private static string FormatCodeName(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            return string.IsNullOrWhiteSpace(name) ? code : code + " - " + name;
        }

        /// <summary>Converte data ISO (yyyy-MM-dd) para BR (dd/MM/yyyy).</summary>
        private static string FormatDate(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            if (DateTime.TryParseExact(
                raw.Length >= 10 ? raw.Substring(0, 10) : raw,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dt))
            {
                return dt.ToString("dd/MM/yyyy");
            }
            return raw;
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
