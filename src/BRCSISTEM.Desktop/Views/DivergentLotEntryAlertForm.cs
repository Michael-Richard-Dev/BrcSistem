using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class DivergentLotEntryAlertForm : Form
    {
        private readonly DatabaseMaintenanceController _maintenanceController;
        private readonly ConfigurationController       _configurationController;
        private readonly UserIdentity    _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;

        // Barra de ações
        private Button _fixButton;
        private Label  _infoLabel;

        // Grid
        private DataGridView _grid;

        public DivergentLotEntryAlertForm(
            CompositionRoot  compositionRoot,
            UserIdentity     identity,
            DatabaseProfile  databaseProfile)
        {
            _maintenanceController   = compositionRoot.CreateDatabaseMaintenanceController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity        = identity;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            Load += (s, e) => LoadForm();
        }

        // ── Layout ────────────────────────────────────────────────────────────

        private void InitializeComponent()
        {
            Text          = "BRCSISTEM - Alerta: Entradas com Lote Divergente";
            StartPosition = FormStartPosition.CenterParent;
            Size          = new Size(1400, 760);
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
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // cabeçalho
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // barra de ações
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // grid

            root.Controls.Add(BuildHeaderPanel(),  0, 0);
            root.Controls.Add(BuildActionsPanel(), 0, 1);
            root.Controls.Add(BuildGridPanel(),    0, 2);

            Controls.Add(root);
        }

        private Control BuildHeaderPanel()
        {
            var panel = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 6) };

            var subtitle = new Label
            {
                AutoSize  = true,
                Text      = "Movimentos de entrada (NOTA) cujo lote registrado nao consta nos itens da nota fiscal",
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock      = DockStyle.Top,
            };
            var title = new Label
            {
                AutoSize  = true,
                Text      = "Alerta: Entradas com Lote Divergente",
                Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Dock      = DockStyle.Top,
            };

            panel.Controls.Add(subtitle);
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildActionsPanel()
        {
            var group = new GroupBox
            {
                Text   = "Acoes",
                Dock   = DockStyle.Top,
                Height = 62,
                Font   = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            var flow = new FlowLayoutPanel
            {
                Dock         = DockStyle.Fill,
                Padding      = new Padding(8, 4, 8, 4),
                WrapContents = false,
            };

            flow.Controls.Add(CreateButton("Consultar (F5)", (s, e) => RunQuery()));

            _fixButton = CreateButton("Inativar Selecionado (F7)", (s, e) => FixSelected());
            _fixButton.Enabled = false;
            flow.Controls.Add(_fixButton);

            flow.Controls.Add(CreateButton("Fechar (F4)", (s, e) => Close()));

            _infoLabel = new Label
            {
                AutoSize  = true,
                Text      = string.Empty,
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin    = new Padding(12, 8, 0, 0),
            };
            flow.Controls.Add(_infoLabel);

            group.Controls.Add(flow);
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

            _grid.Columns.Add(MakeCol("documento",     "DOCUMENTO",        130));
            _grid.Columns.Add(MakeCol("material",      "MATERIAL",         230));
            _grid.Columns.Add(MakeCol("lote_movimento","LOTE MOVIMENTO",   150));
            _grid.Columns.Add(MakeCol("lote_nota",     "LOTE NOTA FISCAL", 150));
            _grid.Columns.Add(MakeCol("almoxarifado",  "ALMOXARIFADO",     140));
            _grid.Columns.Add(MakeCol("fornecedor",    "FORNECEDOR",       140));
            _grid.Columns.Add(MakeCol("qtd_mov",       "QTD. MOV.",         85, DataGridViewContentAlignment.MiddleRight));
            _grid.Columns.Add(MakeCol("qtd_nota",      "QTD. NOTA",         85, DataGridViewContentAlignment.MiddleRight));
            _grid.Columns.Add(MakeCol("usuario_mov",   "USUARIO MOV.",      130));
            _grid.Columns.Add(MakeCol("usuario_nota",  "USUARIO NOTA",      130, fill: true));
            _grid.Columns.Add(MakeCol("criado_em",     "CRIADO EM",         120, DataGridViewContentAlignment.MiddleCenter));

            _grid.SelectionChanged += OnGridSelectionChanged;

            group.Controls.Add(_grid);
            return group;
        }

        // ── Carga inicial ─────────────────────────────────────────────────────

        private void LoadForm()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
            }
            catch (Exception ex)
            {
                ShowError("Erro ao carregar tela", ex);
            }
        }

        // ── Consulta ──────────────────────────────────────────────────────────

        private void RunQuery()
        {
            try
            {
                _grid.Rows.Clear();
                _fixButton.Enabled   = false;
                _infoLabel.Text      = "Consultando...";
                _infoLabel.ForeColor = Color.FromArgb(100, 100, 100);
                System.Windows.Forms.Application.DoEvents();

                var entries = _maintenanceController.DiagnoseDivergentLotEntries(
                    _configuration,
                    _databaseProfile);

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

        private void PopulateGrid(IReadOnlyCollection<DivergentLotEntry> entries)
        {
            _grid.Rows.Clear();
            foreach (var e in entries)
            {
                var materialDisplay = FormatCodeName(e.Material, e.MaterialName);
                var dataBr          = FormatDateTime(e.CreatedAt);
                var qtdMovFmt       = FormatDecimal(e.Quantity);
                var qtdNotaFmt      = FormatDecimal(e.QuantityInNoteItem);

                var idx = _grid.Rows.Add(
                    e.DocumentNumber  ?? string.Empty,
                    materialDisplay,
                    e.LotInMovement   ?? string.Empty,
                    e.LotInNoteItem   ?? string.Empty,
                    e.Warehouse       ?? string.Empty,
                    e.Supplier        ?? string.Empty,
                    qtdMovFmt,
                    qtdNotaFmt,
                    e.MovementUser    ?? string.Empty,
                    e.NoteUser        ?? string.Empty,
                    dataBr);

                // guarda o MovementId na Tag da linha para uso na ação de inativar
                _grid.Rows[idx].Tag = e.MovementId;

                // destaca a coluna "LOTE MOVIMENTO" em laranja para evidenciar a divergência
                _grid.Rows[idx].Cells["lote_movimento"].Style.ForeColor  = Color.FromArgb(180, 60, 0);
                _grid.Rows[idx].Cells["lote_movimento"].Style.Font       = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
        }

        // ── Inativar selecionado ───────────────────────────────────────────────

        private void FixSelected()
        {
            if (_grid.CurrentRow == null) return;

            var movementId = (long)_grid.CurrentRow.Tag;
            var docNumber  = _grid.CurrentRow.Cells["documento"].Value as string ?? string.Empty;
            var material   = _grid.CurrentRow.Cells["material"].Value  as string ?? string.Empty;
            var loteOrf    = _grid.CurrentRow.Cells["lote_movimento"].Value as string ?? string.Empty;

            var confirm = MessageBox.Show(
                this,
                $"Deseja inativar o movimento de entrada com lote divergente?\n\n" +
                $"  Documento  : {docNumber}\n" +
                $"  Material   : {material}\n" +
                $"  Lote (mov.): {loteOrf}\n" +
                $"  ID         : {movementId}\n\n" +
                "Esta operacao nao pode ser desfeita.",
                "Confirmar Inativacao",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _maintenanceController.FixDivergentLotEntry(
                    _configuration,
                    _databaseProfile,
                    _identity.UserName,
                    movementId);

                MessageBox.Show(
                    this,
                    "Movimento inativado com sucesso.",
                    "Operacao Concluida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RunQuery(); // recarrega a grade
            }
            catch (Exception ex)
            {
                ShowError("Erro ao inativar movimento", ex);
            }
        }

        // ── Seleção na grade ──────────────────────────────────────────────────

        private void OnGridSelectionChanged(object sender, EventArgs e)
        {
            _fixButton.Enabled = _grid.CurrentRow != null && _grid.CurrentRow.Tag is long;
        }

        // ── Teclas ────────────────────────────────────────────────────────────

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if      (e.KeyCode == Keys.F4) Close();
            else if (e.KeyCode == Keys.F5) RunQuery();
            else if (e.KeyCode == Keys.F7) FixSelected();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string FormatCodeName(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;
            return string.IsNullOrWhiteSpace(name) ? code : $"{code} - {name}";
        }

        /// <summary>Converte datetime ISO para BR dd/MM/yyyy HH:mm (mostra data+hora da criação).</summary>
        private static string FormatDateTime(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            // tenta datetime completo primeiro
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            return raw;
        }

        private static string FormatDecimal(decimal value)
        {
            return value.ToString("N3", CultureInfo.CurrentCulture);
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            int    width,
            DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleLeft,
            bool   fill = false)
        {
            return new DataGridViewTextBoxColumn
            {
                Name             = name,
                HeaderText       = header,
                Width            = width,
                AutoSizeMode     = fill
                    ? DataGridViewAutoSizeColumnMode.Fill
                    : DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = align },
            };
        }
    }
}
