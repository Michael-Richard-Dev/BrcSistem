using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace BRCSISTEM.Desktop.Interface.ContagemInventario
{
    internal sealed partial class ContagemInventarioForm : Form
    {
        private readonly InventoryController _controller = null!;
        private readonly AppConfiguration _configuration = null!;
        private readonly DatabaseProfile _databaseProfile = null!;
        private readonly UserIdentity _identity = null!;
        private readonly string _inventoryNumber = string.Empty;
        private readonly int _pointId;
        private readonly Action _onChanged = null!;

        private InventoryDetail _detail;
        private InventoryItemDetail[] _items;
        private Timer _heartbeatTimer;

        public ContagemInventarioForm()
        {
            _items = Array.Empty<InventoryItemDetail>();

            InitializeComponent();
        }

        public ContagemInventarioForm(
            InventoryController controller,
            AppConfiguration configuration,
            DatabaseProfile databaseProfile,
            UserIdentity identity,
            string inventoryNumber,
            int pointId,
            Action onChanged)
            : this()
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _databaseProfile = databaseProfile ?? throw new ArgumentNullException(nameof(databaseProfile));
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _inventoryNumber = inventoryNumber ?? string.Empty;
            _pointId = pointId;
            _onChanged = onChanged;

            RegisterControlEvents();
        }

        private void RegisterControlEvents()
        {
            Load += ContagemInventarioForm_Load;
            FormClosing += ContagemInventarioForm_FormClosing;

            _warehouseComboBox.SelectedIndexChanged += WarehouseComboBox_SelectedIndexChanged;
            _materialComboBox.SelectedIndexChanged += MaterialComboBox_SelectedIndexChanged;

            _registerButton.Click += RegisterButton_Click;
            _refreshButton.Click += RefreshButton_Click;
            _closeButton.Click += CloseButton_Click;
        }

        private void ContagemInventarioForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void ContagemInventarioForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopHeartbeat();
        }

        private void WarehouseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnWarehouseChanged();
        }

        private void MaterialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnMaterialChanged();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            RegisterCount();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadData()
        {
            RefreshData();
            StartHeartbeat();
        }

        private void RefreshData()
        {
            try
            {
                _detail = _controller.LoadInventory(_configuration, _databaseProfile, _inventoryNumber);
                _items = (_detail.Items ?? Array.Empty<InventoryItemDetail>()).ToArray();

                BindWarehouseCombo();
                RefreshLogGrid();

                var point = (_detail.Points ?? Array.Empty<InventoryPointSummary>())
                    .FirstOrDefault(item => item.Id == _pointId);

                _pointLabel.Text = point == null
                    ? "Ponto nao localizado."
                    : "Ponto: " + point.PointName + " | IP: " + point.IpAddress + " | Computador: " + point.ComputerName;

                _statusLabel.Text = "Inventario: " + _inventoryNumber + " | Status: " + (_detail.Status ?? string.Empty);
                _statusLabel.ForeColor = Color.SeaGreen;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BindWarehouseCombo()
        {
            var currentWarehouse = GetSelectedCode(_warehouseComboBox);

            var warehouseOptions = _items
                .Select(item => item.WarehouseCode ?? string.Empty)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
                .Select(value => new LookupOption { Code = value, Description = value })
                .ToArray();

            BindCombo(_warehouseComboBox, warehouseOptions, currentWarehouse);
            OnWarehouseChanged();
        }

        private void OnWarehouseChanged()
        {
            var currentMaterial = GetSelectedCode(_materialComboBox);
            var warehouseCode = GetSelectedCode(_warehouseComboBox);

            var materialOptions = _items
                .Where(item => string.Equals(item.WarehouseCode, warehouseCode, StringComparison.OrdinalIgnoreCase))
                .Select(item => new LookupOption { Code = item.MaterialCode, Description = item.MaterialDescription })
                .GroupBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            BindCombo(_materialComboBox, materialOptions, currentMaterial);
            OnMaterialChanged();
        }

        private void OnMaterialChanged()
        {
            var currentLot = GetSelectedCode(_lotComboBox);
            var warehouseCode = GetSelectedCode(_warehouseComboBox);
            var materialCode = GetSelectedCode(_materialComboBox);

            var lotOptions = _items
                .Where(item =>
                    string.Equals(item.WarehouseCode, warehouseCode, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(item.MaterialCode, materialCode, StringComparison.OrdinalIgnoreCase))
                .Select(item => new LookupOption { Code = item.LotCode, Description = item.LotName })
                .GroupBy(item => item.Code ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(item => item.Description ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            BindCombo(_lotComboBox, lotOptions, currentLot);
        }

        private void RegisterCount()
        {
            try
            {
                var quantity = ParseQuantity(_quantityTextBox.Text);

                var point = (_detail.Points ?? Array.Empty<InventoryPointSummary>())
                    .FirstOrDefault(item => item.Id == _pointId);

                if (point == null || !string.Equals(point.Status, "ABERTO", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("O ponto precisa estar ABERTO para registrar leitura.");
                }

                var host = Dns.GetHostName();

                var request = new RegisterInventoryCountRequest
                {
                    InventoryNumber = _inventoryNumber,
                    PointId = _pointId,
                    WarehouseCode = GetSelectedCode(_warehouseComboBox),
                    MaterialCode = GetSelectedCode(_materialComboBox),
                    LotCode = GetSelectedCode(_lotComboBox),
                    Quantity = quantity,
                    ActorUserName = _identity.UserName,
                    IpAddress = ResolveIpAddress(),
                    ComputerName = host,
                    CountedAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                };

                _controller.RegisterCount(_configuration, _databaseProfile, request);

                _quantityTextBox.Text = string.Empty;

                RefreshData();

                _onChanged?.Invoke();

                SetStatus("Leitura registrada com sucesso.", false);
            }
            catch (Exception exception)
            {
                SetStatus(exception.Message, true);
                MessageBox.Show(this, exception.Message, "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshLogGrid()
        {
            var items = (_detail?.Counts ?? Array.Empty<InventoryCountSummary>())
                .Where(item => item.PointId == _pointId)
                .ToArray();

            _logGrid.DataSource = items;
        }

        private void StartHeartbeat()
        {
            StopHeartbeat();

            _heartbeatTimer = new Timer
            {
                Interval = 15000
            };

            _heartbeatTimer.Tick += HeartbeatTimer_Tick;
            _heartbeatTimer.Start();
        }

        private void HeartbeatTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _controller.TouchPointHeartbeat(_configuration, _databaseProfile, _inventoryNumber, _pointId);
            }
            catch
            {
            }
        }

        private void StopHeartbeat()
        {
            if (_heartbeatTimer == null)
            {
                return;
            }

            _heartbeatTimer.Stop();
            _heartbeatTimer.Tick -= HeartbeatTimer_Tick;
            _heartbeatTimer.Dispose();
            _heartbeatTimer = null;
        }

        private void SetStatus(string message, bool isError)
        {
            _statusLabel.Text = message;
            _statusLabel.ForeColor = isError ? Color.Firebrick : Color.SeaGreen;
        }

        private static string ResolveIpAddress()
        {
            try
            {
                var addresses = Dns.GetHostAddresses(Dns.GetHostName());

                foreach (var address in addresses)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }
            }
            catch
            {
            }

            return "127.0.0.1";
        }

        private static decimal ParseQuantity(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("Informe a quantidade contada.");
            }

            if (!decimal.TryParse(
                    value.Trim(),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.GetCultureInfo("pt-BR"),
                    out var parsed)
                && !decimal.TryParse(
                    value.Trim(),
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out parsed))
            {
                throw new InvalidOperationException("Quantidade invalida.");
            }

            if (parsed < 0M)
            {
                throw new InvalidOperationException("Quantidade nao pode ser negativa.");
            }

            return parsed;
        }

        private static void BindCombo(ComboBox comboBox, LookupOption[] options, string selectedCode)
        {
            comboBox.DataSource = null;
            comboBox.DisplayMember = nameof(LookupOption.Display);
            comboBox.ValueMember = nameof(LookupOption.Code);
            comboBox.DataSource = options ?? Array.Empty<LookupOption>();

            if (comboBox.Items.Count > 0)
            {
                SelectOptionByCode(comboBox, selectedCode);

                if (comboBox.SelectedIndex < 0)
                {
                    comboBox.SelectedIndex = 0;
                }
            }
        }

        private static void SelectOptionByCode(ComboBox comboBox, string selectedCode)
        {
            if (string.IsNullOrWhiteSpace(selectedCode))
            {
                return;
            }

            for (var index = 0; index < comboBox.Items.Count; index++)
            {
                if (comboBox.Items[index] is LookupOption option
                    && string.Equals(option.Code, selectedCode, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = index;
                    return;
                }
            }
        }

        private static string GetSelectedCode(ComboBox comboBox)
        {
            return comboBox.SelectedItem is LookupOption option
                ? option.Code
                : string.Empty;
        }

        private sealed class LookupOption
        {
            public string Code { get; set; }

            public string Description { get; set; }

            public string Display
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(Code))
                    {
                        return Description ?? string.Empty;
                    }

                    return string.IsNullOrWhiteSpace(Description)
                        ? Code
                        : Code + " - " + Description;
                }
            }
        }
    }
}