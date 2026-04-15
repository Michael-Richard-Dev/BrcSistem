using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using BRCSISTEM.Application.Models;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    internal sealed class InventoryCountForm : Form
    {
        private readonly InventoryController _controller;
        private readonly AppConfiguration _configuration;
        private readonly DatabaseProfile _databaseProfile;
        private readonly UserIdentity _identity;
        private readonly string _inventoryNumber;
        private readonly int _pointId;
        private readonly Action _onChanged;

        private InventoryDetail _detail;
        private InventoryItemDetail[] _items;

        private ComboBox _warehouseComboBox;
        private ComboBox _materialComboBox;
        private ComboBox _lotComboBox;
        private TextBox _quantityTextBox;
        private Label _pointLabel;
        private Label _statusLabel;
        private DataGridView _logGrid;
        private Timer _heartbeatTimer;

        public InventoryCountForm(
            InventoryController controller,
            AppConfiguration configuration,
            DatabaseProfile databaseProfile,
            UserIdentity identity,
            string inventoryNumber,
            int pointId,
            Action onChanged)
        {
            _controller = controller;
            _configuration = configuration;
            _databaseProfile = databaseProfile;
            _identity = identity;
            _inventoryNumber = inventoryNumber;
            _pointId = pointId;
            _onChanged = onChanged;
            _items = Array.Empty<InventoryItemDetail>();

            InitializeComponent();
            Load += (sender, args) => LoadData();
            FormClosing += (sender, args) => StopHeartbeat();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Contagem Cega";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(980, 620);
            MinimumSize = new Size(860, 520);
            BackColor = Color.White;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var top = new TableLayoutPanel { Dock = DockStyle.Top, ColumnCount = 1, AutoSize = true };
            _pointLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = Color.FromArgb(27, 54, 93) };
            _statusLabel = new Label { AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.SeaGreen };
            top.Controls.Add(_pointLabel, 0, 0);
            top.Controls.Add(_statusLabel, 0, 1);

            var editor = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true, Padding = new Padding(0, 8, 0, 8) };
            editor.Controls.Add(CreateLabel("Almoxarifado:"));
            _warehouseComboBox = new ComboBox { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _warehouseComboBox.SelectedIndexChanged += (sender, args) => OnWarehouseChanged();
            editor.Controls.Add(_warehouseComboBox);

            editor.Controls.Add(CreateLabel("Material:"));
            _materialComboBox = new ComboBox { Width = 280, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            _materialComboBox.SelectedIndexChanged += (sender, args) => OnMaterialChanged();
            editor.Controls.Add(_materialComboBox);

            editor.Controls.Add(CreateLabel("Lote:"));
            _lotComboBox = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
            editor.Controls.Add(_lotComboBox);

            editor.Controls.Add(CreateLabel("Quantidade:"));
            _quantityTextBox = new TextBox { Width = 120, Font = new Font("Segoe UI", 10F) };
            editor.Controls.Add(_quantityTextBox);

            editor.Controls.Add(CreateButton("Registrar", (sender, args) => RegisterCount()));
            editor.Controls.Add(CreateButton("Atualizar", (sender, args) => RefreshData()));
            editor.Controls.Add(CreateButton("Fechar", (sender, args) => Close()));

            var group = new GroupBox { Dock = DockStyle.Fill, Text = "Ultimas Leituras do Ponto", Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            _logGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
            };
            _logGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = nameof(InventoryCountSummary.Id), Width = 70 });
            _logGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "data", HeaderText = "DATA/HORA", DataPropertyName = nameof(InventoryCountSummary.CountedAtDisplay), Width = 150 });
            _logGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "item", HeaderText = "ITEM", DataPropertyName = nameof(InventoryCountSummary.ItemDisplay), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            _logGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "qtd", HeaderText = "QTD", DataPropertyName = nameof(InventoryCountSummary.QuantityText), Width = 110 });
            _logGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "usuario", HeaderText = "USUARIO", DataPropertyName = nameof(InventoryCountSummary.UserName), Width = 120 });
            group.Controls.Add(_logGrid);

            root.Controls.Add(top, 0, 0);
            root.Controls.Add(editor, 0, 1);
            root.Controls.Add(group, 0, 2);
            Controls.Add(root);
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
                var point = (_detail.Points ?? Array.Empty<InventoryPointSummary>()).FirstOrDefault(item => item.Id == _pointId);
                _pointLabel.Text = point == null
                    ? "Ponto nao localizado."
                    : "Ponto: " + point.PointName + " | IP: " + point.IpAddress + " | Computador: " + point.ComputerName;
                _statusLabel.Text = "Inventario: " + _inventoryNumber + " | Status: " + (_detail.Status ?? string.Empty);
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
                .Where(item => string.Equals(item.WarehouseCode, warehouseCode, StringComparison.OrdinalIgnoreCase)
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
                var point = (_detail.Points ?? Array.Empty<InventoryPointSummary>()).FirstOrDefault(item => item.Id == _pointId);
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
                    CountedAt = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
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
            _heartbeatTimer = new Timer { Interval = 15000 };
            _heartbeatTimer.Tick += (sender, args) =>
            {
                try
                {
                    _controller.TouchPointHeartbeat(_configuration, _databaseProfile, _inventoryNumber, _pointId);
                }
                catch
                {
                }
            };
            _heartbeatTimer.Start();
        }

        private void StopHeartbeat()
        {
            if (_heartbeatTimer == null)
            {
                return;
            }

            _heartbeatTimer.Stop();
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

            if (!decimal.TryParse(value.Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var parsed)
                && !decimal.TryParse(value.Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out parsed))
            {
                throw new InvalidOperationException("Quantidade invalida.");
            }

            if (parsed < 0M)
            {
                throw new InvalidOperationException("Quantidade nao pode ser negativa.");
            }

            return parsed;
        }

        private static Label CreateLabel(string text)
        {
            return new Label { AutoSize = true, Text = text, Margin = new Padding(0, 8, 0, 0), Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System };
            button.Click += handler;
            return button;
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
            return comboBox.SelectedItem is LookupOption option ? option.Code : string.Empty;
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
