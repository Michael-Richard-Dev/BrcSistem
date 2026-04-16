using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;
using WinFormsApplication = System.Windows.Forms.Application;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class DuplicateLotAlertForm : Form
    {
        private readonly DatabaseMaintenanceController _maintenanceController;
        private readonly MasterDataController _masterDataController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;

        // Filtros
        private ComboBox _materialComboBox;
        private TextBox _lotDescriptionTextBox;
        private TextBox _lotCodeTextBox;

        // Grid
        private DataGridView _grid;
        private Label _infoLabel;

        // Cache das embalagens para manter o par codigo→label
        private PackagingSummary[] _packagings = new PackagingSummary[0];

        public DuplicateLotAlertForm(
            CompositionRoot compositionRoot,
            UserIdentity identity,
            DatabaseProfile databaseProfile)
        {
            _maintenanceController   = compositionRoot.CreateDatabaseMaintenanceController();
            _masterDataController    = compositionRoot.CreateMasterDataController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity        = identity;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            Load += (s, e) => LoadForm();
        }

        // ── Inicialização visual ───────────────────────────────────────────────

        private void InitializeComponent()
        {
            Text            = "BRCSISTEM - Alerta: Descricao de Lote Duplicada por Material";
            StartPosition   = FormStartPosition.CenterParent;
            Size            = new Size(1280, 760);
            MinimumSize     = new Size(1100, 600);
            BackColor       = Color.White;
            KeyPreview      = true;
            KeyDown        += OnFormKeyDown;

            var root = new TableLayoutPanel
            {
                Dock     = DockStyle.Fill,
                Padding  = new Padding(12),
                RowCount = 3,
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // cabeçalho
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // filtros
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // grid

            root.Controls.Add(BuildHeaderPanel(),  0, 0);
            root.Controls.Add(BuildFiltersPanel(), 0, 1);
            root.Controls.Add(BuildGridPanel(),    0, 2);

            Controls.Add(root);
        }

        private Control BuildHeaderPanel()
        {
            var panel = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 6) };

            var title = new Label
            {
                AutoSize  = true,
                Text      = "Alerta: Descricao de Lote Duplicada no Mesmo Material",
                Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Dock      = DockStyle.Top,
            };

            var subtitle = new Label
            {
                AutoSize  = true,
                Text      = "Mostra lotes ativos com a mesma descricao para o mesmo material",
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock      = DockStyle.Top,
            };

            // adicionar em ordem inversa para que Dock.Top empilhe corretamente
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
                ColumnCount = 8,
                RowCount    = 2,
            };

            // Linha 1: rótulos + controles
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));   // "Material:"
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F)); // combo material
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));   // "Descricao do Lote:"
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F)); // txt descrição
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));   // "Codigo do Lote:"
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // txt código
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));   // spacer
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));   // spacer

            // Material
            layout.Controls.Add(CreateFieldLabel("Material:"), 0, 0);
            _materialComboBox = new ComboBox
            {
                Dock          = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9.5F),
            };
            layout.Controls.Add(_materialComboBox, 1, 0);

            // Descrição do lote
            layout.Controls.Add(CreateFieldLabel("Descricao do Lote:"), 2, 0);
            _lotDescriptionTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            _lotDescriptionTextBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) RunQuery(); };
            layout.Controls.Add(_lotDescriptionTextBox, 3, 0);

            // Código do lote
            layout.Controls.Add(CreateFieldLabel("Codigo do Lote:"), 4, 0);
            _lotCodeTextBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F) };
            _lotCodeTextBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) RunQuery(); };
            layout.Controls.Add(_lotCodeTextBox, 5, 0);

            // Linha 2: botões + info
            var buttonsFlow = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
            buttonsFlow.Controls.Add(CreateButton("Consultar (F5)", (s, e) => RunQuery()));
            buttonsFlow.Controls.Add(CreateButton("Limpar (F6)", (s, e) => ClearFilters()));
            buttonsFlow.Controls.Add(CreateButton("Fechar (F4)", (s, e) => Close()));

            _infoLabel = new Label
            {
                AutoSize  = true,
                Text      = string.Empty,
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin    = new Padding(12, 6, 0, 0),
            };
            buttonsFlow.Controls.Add(_infoLabel);

            layout.SetColumnSpan(buttonsFlow, 8);
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

            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "material",      HeaderText = "MATERIAL",         Width = 240, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "descricao_lote", HeaderText = "DESCRICAO DO LOTE", Width = 220, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "codigo_lote",   HeaderText = "CODIGO DO LOTE",   Width = 110, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "fornecedor",    HeaderText = "FORNECEDOR",       Width = 220, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "validade",      HeaderText = "VALIDADE",         Width = 100, AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "qtd_duplicados", HeaderText = "QTD DUPLICADOS",  Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "grupo_codigos", HeaderText = "LOTES DO GRUPO",   Width = 260, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            group.Controls.Add(_grid);
            return group;
        }

        // ── Carga inicial ──────────────────────────────────────────────────────

        private void LoadForm()
        {
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

        private void LoadMaterialCombo()
        {
            _packagings = _masterDataController
                .LoadPackagings(_configuration, _databaseProfile)
                .Where(p => string.Equals(p.Status, "ATIVO", StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Description, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            _materialComboBox.Items.Clear();
            _materialComboBox.Items.Add(string.Empty); // opção "todos"

            foreach (var p in _packagings)
            {
                var label = string.IsNullOrWhiteSpace(p.Description)
                    ? p.Code
                    : $"{p.Code} - {p.Description}";
                _materialComboBox.Items.Add(label);
            }

            _materialComboBox.SelectedIndex = 0;
        }

        // ── Consulta ───────────────────────────────────────────────────────────

        private void RunQuery()
        {
            try
            {
                _grid.Rows.Clear();
                _infoLabel.Text = "Consultando...";
                _infoLabel.ForeColor = Color.FromArgb(100, 100, 100);
                //Application.DoEvents();
                WinFormsApplication.DoEvents();

                var filterMaterial    = ExtractMaterialCode(_materialComboBox.Text);
                var filterLotDesc     = _lotDescriptionTextBox.Text.Trim();
                var filterLotCode     = _lotCodeTextBox.Text.Trim();

                var entries = _maintenanceController.DiagnoseDuplicateLotsByMaterial(
                    _configuration,
                    _databaseProfile,
                    filterMaterial,
                    filterLotDesc,
                    filterLotCode);

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

        private void PopulateGrid(IReadOnlyCollection<DuplicateLotEntry> entries)
        {
            _grid.Rows.Clear();
            foreach (var e in entries)
            {
                var materialTxt   = FormatCodeName(e.Material, e.MaterialName);
                var fornecedorTxt = FormatCodeName(e.Supplier, e.SupplierName);
                var validadeTxt   = FormatDate(e.Validity);

                _grid.Rows.Add(
                    materialTxt,
                    e.LotName    ?? string.Empty,
                    e.LotCode    ?? string.Empty,
                    fornecedorTxt,
                    validadeTxt,
                    e.DuplicateCount,
                    e.GroupCodes ?? string.Empty);
            }
        }

        // ── Limpeza ────────────────────────────────────────────────────────────

        private void ClearFilters()
        {
            _materialComboBox.SelectedIndex = 0;
            _lotDescriptionTextBox.Clear();
            _lotCodeTextBox.Clear();
            _grid.Rows.Clear();
            _infoLabel.Text = string.Empty;
        }

        // ── Teclas de atalho ───────────────────────────────────────────────────

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if      (e.KeyCode == Keys.F4) Close();
            else if (e.KeyCode == Keys.F5) RunQuery();
            else if (e.KeyCode == Keys.F6) ClearFilters();
        }

        // ── Utilitários ────────────────────────────────────────────────────────

        /// <summary>Extrai o código (parte antes do " - ") do texto selecionado no combo.</summary>
        private static string ExtractMaterialCode(string comboText)
        {
            if (string.IsNullOrWhiteSpace(comboText)) return string.Empty;
            var idx = comboText.IndexOf(" - ", StringComparison.Ordinal);
            return idx >= 0 ? comboText.Substring(0, idx).Trim() : comboText.Trim();
        }

        /// <summary>Formata "CODIGO - NOME" ou só "CODIGO" se nome estiver vazio.</summary>
        private static string FormatCodeName(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            return string.IsNullOrWhiteSpace(name) ? code : $"{code} - {name}";
        }

        /// <summary>Converte data ISO (yyyy-MM-dd) para BR (dd/MM/yyyy).</summary>
        private static string FormatDate(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            if (DateTime.TryParseExact(raw.Length >= 10 ? raw.Substring(0, 10) : raw,
                    "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return dt.ToString("dd/MM/yyyy");
            }
            return raw;
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
            var btn = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(0, 0, 6, 0) };
            btn.Click += handler;
            return btn;
        }
    }
}
