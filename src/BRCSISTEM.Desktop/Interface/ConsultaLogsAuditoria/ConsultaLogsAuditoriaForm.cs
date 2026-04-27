using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface.ConsultaLogsAuditoria
{
    public sealed partial class ConsultaLogsAuditoriaForm : Form
    {
        private readonly DatabaseMaintenanceController _databaseMaintenanceController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly bool _isDesignerInstance;

        private AppConfiguration _configuration;

        private int _currentPage;
        private int _pageSize;
        private int _totalRecords;
        private AuditLogEntry[] _currentEntries;

        public ConsultaLogsAuditoriaForm()
        {
            _isDesignerInstance = true;
            _currentPage = 1;
            _pageSize = 50;
            _currentEntries = Array.Empty<AuditLogEntry>();

            InitializeComponent();
        }

        public ConsultaLogsAuditoriaForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            if (compositionRoot == null) throw new ArgumentNullException(nameof(compositionRoot));

            _databaseMaintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _databaseProfile = databaseProfile ?? throw new ArgumentNullException(nameof(databaseProfile));
            _currentPage = 1;
            _pageSize = 50;
            _currentEntries = Array.Empty<AuditLogEntry>();

            InitializeComponent();
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
            if (!IsDesignModeActive)
            {
                LoadData();
            }
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            if (!IsDesignModeActive && _configuration != null)
            {
                SearchFromFirstPage();
            }
        }

        private void OnSearchButtonClick(object sender, EventArgs e)
        {
            SearchFromFirstPage();
        }

        private void OnClearButtonClick(object sender, EventArgs e)
        {
            ClearFilters();
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnPreviousButtonClick(object sender, EventArgs e)
        {
            GoToPreviousPage();
        }

        private void OnNextButtonClick(object sender, EventArgs e)
        {
            GoToNextPage();
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }

        private void ShowError(Exception exception)
        {
            SetStatus(exception.Message, true);
            MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
