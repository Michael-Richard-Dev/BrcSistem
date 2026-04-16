using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class NegativeStockAlertForm : Form
    {
        private readonly DatabaseMaintenanceController _maintenanceController;
        private readonly MasterDataController          _masterDataController;
        private readonly ConfigurationController       _configurationController;
        private readonly UserIdentity    _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;

        // Filtros
        private ComboBox _warehouseComboBox;
        private ComboBox _materialComboBox;
        private TextBox  _lotTextBox;

        // Grid
        private DataGridView _grid;
        private Label        _infoLabel;

        public NegativeStockAlertForm(
            CompositionRoot  compositionRoot,
            UserIdentity     identity,
            DatabaseProfile  databaseProfile)
        {
            _maintenanceController   = compositionRoot.CreateDatabaseMaintenanceController();
            _masterDataController    = compositionRoot.CreateMasterDataController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity        = identity;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            Load += (s, e) => LoadForm();
        }

        // ── Layout ────────────────────────────────────────────────────────────

        private void InitializeComponent()
        {
            Text          = "BRCSISTEM - Alerta: Estoque Negativo antes da Entrada";
            StartPosition = FormStartPosition.CenterParent;
            Size          = new Size(1280, 760);
            MinimumSize   = new Size(1100, 600);
            BackColor     = Color.White;
            KeyPreview    = true;
            KeyDown      += OnFormKeyDown;

            var root = new TableLayoutPanel
            {
                Dock     = DockStyle.Fill,
                Padding  = new Padding(12),
                RowCount = 3,
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // cabeçalho
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // filtros
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // grid

            root.Controls.Add(BuildHeaderPanel(),  0, 0);
            root.Controls.Add(BuildFiltersPanel(), 0, 1);
            root.Controls.Add(BuildGridPanel(),    0, 2);

            Controls.Add(root);
        }

        private Control BuildHeaderPanel()
        {
            var panel = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 6) };

            var subtitle = new Label
            {
                AutoSize  = true,
                Text      = "Movimentos com saldo negativo antes da primeira entrada do lote",
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock      = DockStyle.Top,
            };
            var title = new Label
            {
                AutoSize  = true,
                Text      = "Alerta: Estoque Negativo antes da Entrada",
                Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Dock      = DockStyle.Top,
            };

            panel.Controls.Add(subtitle);
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildFiltersPanel()
        {
            var group = new GroupBox
            {
                Text   = "Filtros",
                Dock   = DockStyle.Top,
                Height = 92,
                Font   = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            var layout = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                Padding     = new Padding(8),
                ColumnCount = 7,
                RowCount    = 2,
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // "Almoxarifado:"
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));  // combo almox
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // "Material:"
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));  // combo material
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // "Lote:"
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));  // txt lote
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // spacer

            // ── linha 0: labels + controles ──
            layout.Controls.Add(CreateFieldLabel("Almoxarifado:"), 0, 0);
            _warehouseComboBox = new ComboBox
            {
                Dock          = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9.5F),
            };
            layout.Controls.Add(_warehouseComboBox, 1, 0);

            layout.Controls.Add(CreateFieldLabel("Material:"), 2, 0);
            _materialComboBox = new ComboBox
            {
                Dock          = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9.5F),
            };
            layout.Controls.Add(_materialComboBox, 3, 0);

            layout.Controls.Add(CreateFieldLabel("Lote:"), 4, 0);
            _lotTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            _lotTextBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) RunQuery(); };
            layout.Controls.Add(_lotTextBox, 5, 0);

            // ── linha 1: botões + info ──
            var buttonsFlow = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
            buttonsFlow.Controls.Add(CreateButton("Consultar (F5)", (s, e) => RunQuery()));
            buttonsFlow.Controls.Add(CreateButton("Limpar (F6)",    (s, e) => ClearFilters()));
            buttonsFlow.Controls.Add(CreateButton("Fechar (F4)",    (s, e) => Close()));

            _infoLabel = new Label
            {
                AutoSize  = true,
                Text      = string.Empty,
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin    = new Padding(12, 6, 0, 0),
            };
            buttonsFlow.Controls.Add(_infoLabel);

            layout.SetColumnSpan(buttonsFlow, 7);
            layout.Controls.Add(buttonsFlow, 0, 1);

            group.Controls.Add(layout);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox
            {
                Text = "Resultados",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            _grid = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                ReadOnly              = true,
                AutoGenerateColumns   = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                MultiSelect           = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible     = false,
                BackgroundColor       = Color.White,
                BorderStyle           = BorderStyle.None,
                Font                  = new Font("Segoe UI", 9F),
            };

            _grid.Columns.Add(MakeCol("almox",      "ALMOXARIFADO",    180));
            _grid.Columns.Add(MakeCol("material",   "MATERIAL",        240));
            _grid.Columns.Add(MakeCol("lote",       "LOTE",            160));
            _grid.Columns.Add(MakeCol("data_mov",   "DATA MOVIMENTO",  130, DataGridViewContentAlignment.MiddleCenter));
            _grid.Columns.Add(MakeCol("tipo",       "TIPO",            150));
            _grid.Columns.Add(MakeCol("documento",  "DOCUMENTO",       160));
            _grid.Columns.Add(MakeCol("quantidade", "QUANTIDADE",       90, DataGridViewContentAlignment.MiddleRight));
            _grid.Columns.Add(MakeCol("saldo",      "SALDO",            90, DataGridViewContentAlignment.MiddleRight));
            _grid.Columns.Add(MakeCol("fornecedor", "FORNECEDOR",      200, fill: true));
            _grid.Columns.Add(MakeCol("validade",   "VALIDADE",        100, DataGridViewContentAlignment.MiddleCenter));

            group.Controls.Add(_grid);
            return group;
        }

        // ── Carga inicial ─────────────────────────────────────────────────────

        private void LoadForm()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                LoadCombos();
            }
            catch (Exception ex)
            {
                ShowError("Erro ao carregar tela", ex);
            }
        }

        private void LoadCombos()
        {
            // ── Almoxarifado ──
            var warehouses = _masterDataController
                .LoadWarehouses(_configuration, _databaseProfile)
                .Where(w => string.Equals(w.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(w => w.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            _warehouseComboBox.Items.Clear();
            _warehouseComboBox.Items.Add(string.Empty);
            foreach (var w in warehouses)
            {
                var label = string.IsNullOrWhiteSpace(w.Name) ? w.Code : $"{w.Code} - {w.Name}";
                _warehouseComboBox.Items.Add(label);
            }
            _warehouseComboBox.SelectedIndex = 0;

            // ── Material ──
            var materials = _masterDataController
                .LoadPackagings(_configuration, _databaseProfile)
                .Where(p => string.Equals(p.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Description, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            _materialComboBox.Items.Clear();
            _materialComboBox.Items.Add(string.Empty);
            foreach (var p in materials)
            {
                var label = string.IsNullOrWhiteSpace(p.Description) ? p.Code : $"{p.Code} - {p.Description}";
                _materialComboBox.Items.Add(label);
            }
            _materialComboBox.SelectedIndex = 0;
        }

        // ── Consulta ──────────────────────────────────────────────────────────

        private void RunQuery()
        {
            try
            {
                _grid.Rows.Clear();
                _infoLabel.Text      = "Consultando...";
                _infoLabel.ForeColor = Color.FromArgb(100, 100, 100);
                System.Windows.Forms.Application.DoEvents();

                var filterWarehouse = ExtractCode(_warehouseComboBox.Text);
                var filterMaterial  = ExtractCode(_materialComboBox.Text);
                var filterLot       = _lotTextBox.Text.Trim();

                var entries = _maintenanceController.DiagnoseNegativeStock(
                    _configuration,
                    _databaseProfile,
                    filterWarehouse,
                    filterMaterial,
                    filterLot);

                PopulateGrid(entries);

                _infoLabel.Text      = $"Registros: {entries.Count}";
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

        private void PopulateGrid(IReadOnlyCollection<NegativeStockEntry> entries)
        {
            _grid.Rows.Clear();
            foreach (var e in entries)
            {
                var almoxDisplay    = FormatCodeName(e.Warehouse,     e.WarehouseName);
                var materialDisplay = FormatCodeName(e.Material,      e.MaterialName);
                var loteDisplay     = FormatCodeName(e.Lot,           e.LotName);
                var docDisplay      = string.IsNullOrWhiteSpace(e.DocumentType)
                                          ? (e.DocumentNumber ?? string.Empty)
                                          : $"{e.DocumentType} {e.DocumentNumber}".Trim();
                var dataBr     = FormatDate(e.MovementDate);
                var validBr    = FormatDate(e.Validity);
                var qtdFmt     = FormatDecimal(e.Quantity);
                var saldoFmt   = FormatDecimal(e.RunningBalance);

                _grid.Rows.Add(
                    almoxDisplay,
                    materialDisplay,
                    loteDisplay,
                    dataBr,
                    e.MovementType     ?? string.Empty,
                    docDisplay,
                    qtdFmt,
                    saldoFmt,
                    e.SupplierName     ?? string.Empty,
                    validBr);
            }
        }

        // ── Limpeza ───────────────────────────────────────────────────────────

        private void ClearFilters()
        {
            _warehouseComboBox.SelectedIndex = 0;
            _materialComboBox.SelectedIndex  = 0;
            _lotTextBox.Clear();
            _grid.Rows.Clear();
            _infoLabel.Text = string.Empty;
        }

        // ── Teclas ────────────────────────────────────────────────────────────

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if      (e.KeyCode == Keys.F4) Close();
            else if (e.KeyCode == Keys.F5) RunQuery();
            else if (e.KeyCode == Keys.F6) ClearFilters();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>Extrai o código (parte antes de " - ") do item selecionado no combo.</summary>
        private static string ExtractCode(string comboText)
        {
            if (string.IsNullOrWhiteSpace(comboText)) return string.Empty;
            var idx = comboText.IndexOf(" - ", StringComparison.Ordinal);
            return idx >= 0 ? comboText.Substring(0, idx).Trim() : comboText.Trim();
        }

        /// <summary>Formata "CODIGO - NOME" ou só "CODIGO" quando o nome está vazio.</summary>
        private static string FormatCodeName(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            return string.IsNullOrWhiteSpace(name) ? code : $"{code} - {name}";
        }

        /// <summary>Converte data ISO/datetime para BR dd/MM/yyyy (pega só os 10 primeiros chars).</summary>
        private static string FormatDate(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            var part = raw.Length >= 10 ? raw.Substring(0, 10) : raw;
            if (DateTime.TryParseExact(part, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return dt.ToString("dd/MM/yyyy");
            }
            return part;
        }

        /// <summary>Formata decimal com separador de milhar e 3 casas decimais (equivalente a formatar_milhar do Python).</summary>
        private static string FormatDecimal(decimal value)
        {
            return value.ToString("N3", CultureInfo.CurrentCulture);
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize  = true,
                Text      = text,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin    = new Padding(0, 7, 4, 0),
            };
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var btn = new Button
            {
                Text      = text,
                AutoSize  = true,
                FlatStyle = FlatStyle.System,
                Margin    = new Padding(0, 0, 6, 0),
            };
            btn.Click += handler;
            return btn;
        }

        private static DataGridViewTextBoxColumn MakeCol(
            string name,
            string header,
            int width,
            DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft,
            bool fill = false)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name             = name,
                HeaderText       = header,
                Width            = width,
                AutoSizeMode     = fill
                    ? DataGridViewAutoSizeColumnMode.Fill
                    : DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = align },
            };
            return col;
        }
    }
}
