using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class InventoryForm : Form
    {
        private CompositionRoot _compositionRoot;
        private InventoryController _inventoryController;
        private ConfigurationController _configurationController;
        private UserIdentity _identity;
        private DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private InventoryPermissions _permissions;
        private LookupOption[] _warehouseOptions;
        private LookupOption[] _materialOptions;
        private LookupOption[] _lotOptions;
        private readonly List<InventoryItemDetail> _draftItems;
        private readonly List<InventoryPointSummary> _draftPoints;
        private readonly Dictionary<int, InventoryCountForm> _countWindows;

        private InventoryCountSummary[] _currentCounts;
        private bool _isRefreshingReferences;
        private bool _isPersisted;
        private bool _isReadOnly;
        private string _currentStatus;
        private string _lockedNumber;
        private int _temporaryPointSequence;

        public InventoryForm()
        {
            _warehouseOptions = Array.Empty<LookupOption>();
            _materialOptions = Array.Empty<LookupOption>();
            _lotOptions = Array.Empty<LookupOption>();
            _draftItems = new List<InventoryItemDetail>();
            _draftPoints = new List<InventoryPointSummary>();
            _countWindows = new Dictionary<int, InventoryCountForm>();
            _currentCounts = Array.Empty<InventoryCountSummary>();
            _temporaryPointSequence = -1;

            InitializeComponent();
        }

        public InventoryForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
            : this()
        {
            _compositionRoot = compositionRoot;
            _inventoryController = compositionRoot.CreateInventoryController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                Load += OnInventoryFormLoad;
                FormClosing += OnFormClosing;
            }
        }

        private void OnInventoryFormLoad(object sender, EventArgs e)
        {
            Load -= OnInventoryFormLoad;
            LoadData();
        }
    }
}
