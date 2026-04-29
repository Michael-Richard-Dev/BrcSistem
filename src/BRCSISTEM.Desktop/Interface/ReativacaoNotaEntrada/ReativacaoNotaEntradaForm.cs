using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.ReativacaoNotaEntrada
{
    public sealed partial class ReativacaoNotaEntradaForm : Form
    {
        private readonly DatabaseMaintenanceController _databaseMaintenanceController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly bool _isDesignerInstance;

        private AppConfiguration _configuration;
        private InboundReceiptReactivationEntry[] _entries;

        public ReativacaoNotaEntradaForm()
            : this(null, null, null, true)
        {
        }

        public ReativacaoNotaEntradaForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
            : this(compositionRoot, identity, databaseProfile, false)
        {
        }

        private ReativacaoNotaEntradaForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile, bool designerCtor)
        {
            if (!designerCtor && compositionRoot == null)
            {
                throw new ArgumentNullException(nameof(compositionRoot));
            }

            _isDesignerInstance = designerCtor;
            _databaseMaintenanceController = compositionRoot == null ? null : compositionRoot.CreateDatabaseMaintenanceController();
            _configurationController = compositionRoot == null ? null : compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _entries = Array.Empty<InboundReceiptReactivationEntry>();

            InitializeComponent();

            if (!IsDesignModeActive)
            {
                Load += OnFormLoad;
            }
        }

        private bool IsDesignModeActive
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                    || _isDesignerInstance
                    || DesignMode
                    || (Site != null && Site.DesignMode);
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            LoadData();
        }

        private void OnSearchButtonClick(object sender, EventArgs e)
        {
            SearchCancelledReceipts();
        }

        private void OnClearButtonClick(object sender, EventArgs e)
        {
            ClearFilters();
        }

        private void OnLoadAllButtonClick(object sender, EventArgs e)
        {
            LoadAllCancelledReceipts();
        }

        private void OnReactivateButtonClick(object sender, EventArgs e)
        {
            ReactivateSelectedReceipt();
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ReactivateSelectedReceipt();
            }
        }

        private void SetStatus(string message, bool isError)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = isError ? Color.Firebrick : Color.SeaGreen;
        }

        private void ShowError(string title, Exception exception)
        {
            SetStatus(exception.Message, true);
            MessageBox.Show(this, title + ":\n" + exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
