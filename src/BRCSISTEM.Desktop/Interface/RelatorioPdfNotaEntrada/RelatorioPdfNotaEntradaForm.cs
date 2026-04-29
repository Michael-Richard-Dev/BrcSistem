using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Desktop.Interface;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.RelatorioPdfNotaEntrada
{
    /// <summary>
    /// Tela de auditoria de relatorio de entrada nota por nota, com exportacao
    /// em PDF e CSV. Toda a logica funcional (consulta, filtros, geracao,
    /// exportacao e carga de dados) permanece neste arquivo; o layout visual
    /// fica em RelatorioPdfNotaEntradaForm.Designer.cs.
    /// </summary>
    public sealed partial class RelatorioPdfNotaEntradaForm : Form
    {
        private readonly InboundReceiptReportController _inboundReceiptReportController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly bool _isDesignerInstance;

        private AppConfiguration _configuration;
        private LookupOption[] _supplierOptions;
        private InboundReceiptReportEntry[] _rows;

        public RelatorioPdfNotaEntradaForm()
            : this(null, null, null, true)
        {
        }

        public RelatorioPdfNotaEntradaForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
            : this(compositionRoot, identity, databaseProfile, false)
        {
        }

        private RelatorioPdfNotaEntradaForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile, bool designerCtor)
        {
            _isDesignerInstance = designerCtor;
            _supplierOptions = new LookupOption[0];
            _rows = new InboundReceiptReportEntry[0];

            if (!designerCtor)
            {
                _inboundReceiptReportController = compositionRoot.CreateInboundReceiptReportController();
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

        // Eventos de tela associados no Designer.

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;
            LoadData();
        }

        private void OnStartDateLeave(object sender, EventArgs e)
        {
            _startDateTextBox.Text = NormalizeDateInput(_startDateTextBox.Text);
        }

        private void OnEndDateLeave(object sender, EventArgs e)
        {
            _endDateTextBox.Text = NormalizeDateInput(_endDateTextBox.Text);
        }

        private void OnReceiptNumberTextChanged(object sender, EventArgs e)
        {
            NormalizeReceiptNumberInput();
        }

        private void OnReceiptNumberKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                if (!IsDesignModeActive) QueryEntries();
            }
        }

        private void OnBuscarFornecedorClick(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;
            OpenSupplierLookup();
        }

        private void OnFiltrarClick(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;
            QueryEntries();
        }

        private void OnLimparFiltrosClick(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;
            ClearFilters();
        }

        private void OnGerarPdfClick(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;
            ExportPdf();
        }

        private void OnGerarCsvClick(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;
            ExportCsv();
        }

        private void OnFecharClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
