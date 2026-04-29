using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;
using BRCSISTEM.Desktop.Interface.ContagemInventario;

namespace BRCSISTEM.Desktop.Interface.Inventario
{
    public sealed partial class InventarioForm : Form
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
        private readonly Dictionary<int, ContagemInventarioForm> _countWindows;

        private InventoryCountSummary[] _currentCounts;
        private bool _isRefreshingReferences;
        private bool _isPersisted;
        private bool _isReadOnly;
        private string _currentStatus;
        private string _lockedNumber;
        private int _temporaryPointSequence;

        public InventarioForm()
        {
            _warehouseOptions = Array.Empty<LookupOption>();
            _materialOptions = Array.Empty<LookupOption>();
            _lotOptions = Array.Empty<LookupOption>();
            _draftItems = new List<InventoryItemDetail>();
            _draftPoints = new List<InventoryPointSummary>();
            _countWindows = new Dictionary<int, ContagemInventarioForm>();
            _currentCounts = Array.Empty<InventoryCountSummary>();
            _temporaryPointSequence = -1;

            InitializeComponent();
        }

        public InventarioForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
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

        private void SearchButton_Click(object sender, EventArgs e)
        {
            OpenInventoryLookup();
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            ClearForm(confirm: false, releaseLock: true, regenerateNumber: true);
        }

        private void CurrentButton_Click(object sender, EventArgs e)
        {
            _createdTextBox.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        private void ScheduledTextBox_Leave(object sender, EventArgs e)
        {
            _scheduledTextBox.Text = NormalizeDateTimeInput(_scheduledTextBox.Text);
        }

        private void WarehouseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnWarehouseSelectionChanged();
        }

        private void WarehouseRefreshButton_Click(object sender, EventArgs e)
        {
            ReloadWarehouses();
        }

        private void MaterialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnMaterialSelectionChanged();
        }

        private void MaterialRefreshButton_Click(object sender, EventArgs e)
        {
            ReloadMaterials();
        }

        private void LotComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStockIndicator();
        }

        private void LotRefreshButton_Click(object sender, EventArgs e)
        {
            ReloadLots();
        }

        private void OnlyBrcCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ReloadMaterials();
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            AddItem();
        }

        private void AddAllItemsButton_Click(object sender, EventArgs e)
        {
            AddAllFromWarehouse();
        }

        private void RemoveItemButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedItem();
        }

        private void ClearItemsButton_Click(object sender, EventArgs e)
        {
            ClearItems();
        }

        private void PointsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenSelectedPoint();
        }

        private void NewPointButton_Click(object sender, EventArgs e)
        {
            CreatePoint();
        }

        private void OpenPointButton_Click(object sender, EventArgs e)
        {
            OpenSelectedPoint();
        }

        private void ClosePointButton_Click(object sender, EventArgs e)
        {
            CloseSelectedPoint();
        }

        private void ReopenPointButton_Click(object sender, EventArgs e)
        {
            ReopenSelectedPoint();
        }

        private void DeletePointButton_Click(object sender, EventArgs e)
        {
            DeleteSelectedPoint();
        }

        private void ZeroButton_Click(object sender, EventArgs e)
        {
            ApplyZeroCounts();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CancelInventory();
        }

        private void FinalizeButton_Click(object sender, EventArgs e)
        {
            FinalizeInventory();
        }

        private void ReopenButton_Click(object sender, EventArgs e)
        {
            ReopenInventory();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            CloseInventory();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartInventory();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            UpdateInventory();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveInventory();
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(0, 8, 0, 0),
                Text = text,
            };
        }
    }
}
