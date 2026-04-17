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
    /// <summary>
    /// Porte fiel de views/alerta_movimentos_duplicados_nota.py.
    /// Mestre-detalhe em memoria: a consulta retorna Groups com Details embedados
    /// (cache local = _diagCache do Python) e a grade inferior e repopulada a
    /// partir do Group selecionado.
    /// </summary>
    public sealed class DuplicateNoteMovementsAlertForm : Form
    {
        private readonly DatabaseMaintenanceController _maintenanceController;
        private readonly ConfigurationController       _configurationController;
        private readonly UserIdentity    _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;

        // Cache em memoria (igual ao self._diag_cache do Python)
        private List<DuplicateNoteMovementGroup> _diagCache = new List<DuplicateNoteMovementGroup>();

        // Grids
        private DataGridView _gridNf;
        private DataGridView _gridDet;
        private Label        _infoLabel;

        public DuplicateNoteMovementsAlertForm(
            CompositionRoot compositionRoot,
            UserIdentity    identity,
            DatabaseProfile databaseProfile)
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
            Text          = "BRCSISTEM - Alerta: NFs com Movimentos Duplicados";
            StartPosition = FormStartPosition.CenterParent;
            Size          = new Size(1400, 820);
            MinimumSize   = new Size(1200, 680);
            BackColor     = Color.White;
            KeyPreview    = true;
            KeyDown      += OnFormKeyDown;

            var root = new TableLayoutPanel
            {
                Dock     = DockStyle.Fill,
                Padding  = new Padding(12),
                RowCount = 4,
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // cabecalho
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // acoes
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));   // grade mestre
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));   // grade detalhe

            root.Controls.Add(BuildHeaderPanel(),  0, 0);
            root.Controls.Add(BuildActionsPanel(), 0, 1);
            root.Controls.Add(BuildNfGridPanel(),  0, 2);
            root.Controls.Add(BuildDetGridPanel(), 0, 3);

            Controls.Add(root);
        }

        private Control BuildHeaderPanel()
        {
            var panel = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 6) };

            var subtitle = new Label
            {
                AutoSize  = true,
                Text      = "Diagnostico cauteloso: exibe somente IDs propostos para inativacao. Revise antes de remover.",
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock      = DockStyle.Top,
            };
            var title = new Label
            {
                AutoSize  = true,
                Text      = "Alerta: NFs com Movimentos Duplicados/Inconsistentes",
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

            flow.Controls.Add(CreateButton("Consultar (F5)",                         (s, e) => RunQuery()));
            flow.Controls.Add(CreateButton("Limpar (F6)",                            (s, e) => ClearAll()));
            flow.Controls.Add(CreateButton("Remover Duplicados da NF Selecionada",   (s, e) => RemoveSelectedNote()));
            flow.Controls.Add(CreateButton("Remover Todos os Duplicados Propostos",  (s, e) => RemoveAllProposed()));
            flow.Controls.Add(CreateButton("Fechar (F4)",                            (s, e) => Close()));

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

        private Control BuildNfGridPanel()
        {
            var group = new GroupBox
            {
                Text = "NFs com Propostas de Limpeza",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };
            _gridNf = CreateGrid();
            // colunas espelham cols_nf do Python
            _gridNf.Columns.Add(MakeCol("numero",        "NF",                    110, DataGridViewContentAlignment.MiddleCenter));
            _gridNf.Columns.Add(MakeCol("fornecedor",    "FORNECEDOR",            110, DataGridViewContentAlignment.MiddleCenter));
            _gridNf.Columns.Add(MakeCol("versao",        "VERSAO",                 70, DataGridViewContentAlignment.MiddleCenter));
            _gridNf.Columns.Add(MakeCol("usuario_nota",  "USUARIO NOTA",          120));
            _gridNf.Columns.Add(MakeCol("mov_ativos",    "MOV. ATIVOS",            90, DataGridViewContentAlignment.MiddleCenter));
            _gridNf.Columns.Add(MakeCol("qtd_propostas", "QTD PROPOSTAS",          95, DataGridViewContentAlignment.MiddleCenter));
            _gridNf.Columns.Add(MakeCol("ids_propostos", "IDS PROPOSTOS",         360));
            _gridNf.Columns.Add(MakeCol("usuarios_mov",  "USUARIOS MOV. ATIVOS",  260, fill: true));

            _gridNf.SelectionChanged += (s, e) => PopulateDetailFromSelection();

            group.Controls.Add(_gridNf);
            return group;
        }

        private Control BuildDetGridPanel()
        {
            var group = new GroupBox
            {
                Text = "Detalhes dos IDs Propostos (NF selecionada)",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };
            _gridDet = CreateGrid();
            _gridDet.Columns.Add(MakeCol("id",          "ID",                 70,  DataGridViewContentAlignment.MiddleCenter));
            _gridDet.Columns.Add(MakeCol("motivo",      "MOTIVO",             320));
            _gridDet.Columns.Add(MakeCol("usuario_mov", "USUARIO MOVIMENTO",  130));
            _gridDet.Columns.Add(MakeCol("usuario_nota","USUARIO NOTA",       120));
            _gridDet.Columns.Add(MakeCol("material",    "MATERIAL",            95, DataGridViewContentAlignment.MiddleCenter));
            _gridDet.Columns.Add(MakeCol("lote",        "LOTE",               120));
            _gridDet.Columns.Add(MakeCol("almox",       "ALMOX",               80, DataGridViewContentAlignment.MiddleCenter));
            _gridDet.Columns.Add(MakeCol("quantidade",  "QUANTIDADE",         100, DataGridViewContentAlignment.MiddleRight));
            _gridDet.Columns.Add(MakeCol("data_mov",    "DATA MOVIMENTO",     130, DataGridViewContentAlignment.MiddleCenter));
            _gridDet.Columns.Add(MakeCol("dt_criacao",  "DT/HR CRIACAO",      135, DataGridViewContentAlignment.MiddleCenter));
            _gridDet.Columns.Add(MakeCol("id_ref",      "ID REFERENCIA",       95, DataGridViewContentAlignment.MiddleCenter, fill: true));

            group.Controls.Add(_gridDet);
            return group;
        }

        // ── Inicializacao ─────────────────────────────────────────────────────

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
                ClearAll();
                _infoLabel.Text      = "Consultando...";
                _infoLabel.ForeColor = Color.FromArgb(100, 100, 100);
                System.Windows.Forms.Application.DoEvents();

                var groups = _maintenanceController
                    .DiagnoseDuplicateNoteMovements(_configuration, _databaseProfile);

                _diagCache = groups.ToList();
                PopulateNfGrid();

                var totalNf  = _diagCache.Count;
                var totalIds = _diagCache.Sum(g => g.DuplicateMovementIds?.Count ?? 0);
                _infoLabel.Text      = $"NFs: {totalNf} | IDs propostos: {totalIds}";
                _infoLabel.ForeColor = totalNf > 0 ? Color.FromArgb(180, 60, 0) : Color.SeaGreen;

                // Seleciona primeira linha (igual Python)
                if (_gridNf.Rows.Count > 0)
                {
                    _gridNf.ClearSelection();
                    _gridNf.Rows[0].Selected = true;
                    _gridNf.CurrentCell = _gridNf.Rows[0].Cells[0];
                }
            }
            catch (Exception ex)
            {
                _infoLabel.Text = string.Empty;
                ShowError("Erro ao consultar duplicidades", ex);
            }
        }

        private void PopulateNfGrid()
        {
            _gridNf.Rows.Clear();
            foreach (var g in _diagCache)
            {
                var ids   = g.DuplicateMovementIds ?? new List<long>();
                var idsTxt = string.Join(", ", ids);

                var idx = _gridNf.Rows.Add(
                    g.NoteNumber          ?? string.Empty,
                    g.Supplier            ?? string.Empty,
                    g.NoteVersion         ?? string.Empty,
                    g.NoteUser            ?? string.Empty,
                    g.TotalActiveMovements,
                    ids.Count,
                    idsTxt,
                    g.ActiveMovementUsers ?? string.Empty);

                _gridNf.Rows[idx].Tag = g;
                _gridNf.Rows[idx].Cells["qtd_propostas"].Style.ForeColor = Color.FromArgb(180, 60, 0);
                _gridNf.Rows[idx].Cells["qtd_propostas"].Style.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            }
        }

        private void PopulateDetailFromSelection()
        {
            _gridDet.Rows.Clear();
            var g = GetSelectedGroup();
            if (g == null) return;

            foreach (var d in g.Details ?? new List<DuplicateNoteMovementDetail>())
            {
                _gridDet.Rows.Add(
                    d.MovementId,
                    d.ReasonLabel     ?? d.Reason ?? string.Empty,
                    d.MovementUser    ?? string.Empty,
                    d.NoteUser        ?? string.Empty,
                    d.Material        ?? string.Empty,
                    d.Lot             ?? string.Empty,
                    d.Warehouse       ?? string.Empty,
                    FormatQuantity(d.Quantity),
                    FormatDateTime(d.MovementDate),
                    FormatDateTime(d.CreatedAt),
                    d.KeepReferenceId.HasValue ? d.KeepReferenceId.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
            }
        }

        private DuplicateNoteMovementGroup GetSelectedGroup()
        {
            if (_gridNf.CurrentRow == null) return null;
            return _gridNf.CurrentRow.Tag as DuplicateNoteMovementGroup;
        }

        // ── Limpar ────────────────────────────────────────────────────────────

        private void ClearAll()
        {
            _diagCache = new List<DuplicateNoteMovementGroup>();
            _gridNf.Rows.Clear();
            _gridDet.Rows.Clear();
            _infoLabel.Text = string.Empty;
        }

        // ── Inativar NF selecionada ───────────────────────────────────────────

        private void RemoveSelectedNote()
        {
            var g = GetSelectedGroup();
            if (g == null)
            {
                MessageBox.Show(this,
                    "Selecione uma NF para remover os duplicados.",
                    "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ids = (g.DuplicateMovementIds ?? new List<long>()).ToArray();
            if (ids.Length == 0)
            {
                MessageBox.Show(this,
                    "Nao ha IDs propostos para esta NF.",
                    "Sem dados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idsTxt = "[" + string.Join(", ", ids) + "]";
            var confirm = MessageBox.Show(this,
                $"NF {g.NoteNumber} / Fornecedor {g.Supplier}\n\n" +
                $"IDs propostos para inativar: {idsTxt}\n\n" +
                "Deseja continuar?",
                "Confirmar limpeza",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            ExecuteInactivation(ids, g.NoteNumber);
        }

        // ── Inativar todos os propostos ───────────────────────────────────────

        private void RemoveAllProposed()
        {
            var ids = _diagCache
                .SelectMany(g => g.DuplicateMovementIds ?? Enumerable.Empty<long>())
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            if (ids.Length == 0)
            {
                MessageBox.Show(this,
                    "Nao ha IDs propostos para remover.",
                    "Sem dados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var idsTxt = "[" + string.Join(", ", ids) + "]";
            var confirm = MessageBox.Show(this,
                $"Total de IDs propostos: {ids.Length}\n\n" +
                $"IDs: {idsTxt}\n\n" +
                "Deseja inativar todos?",
                "Confirmar limpeza geral",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes) return;

            ExecuteInactivation(ids, noteNumber: null);
        }

        private void ExecuteInactivation(long[] ids, string noteNumber)
        {
            try
            {
                var result = _maintenanceController.InactivateDuplicateNoteMovements(
                    _configuration,
                    _databaseProfile,
                    _identity.UserName,
                    ids,
                    noteNumber ?? string.Empty);

                MessageBox.Show(this,
                    $"IDs solicitados: [{string.Join(", ", result.RequestedIds)}]\n" +
                    $"IDs encontrados: [{string.Join(", ", result.FoundIds)}]\n" +
                    $"IDs inativados: [{string.Join(", ", result.InactivatedIds)}]\n" +
                    $"Total inativado: {result.TotalInactivated}",
                    "Limpeza concluida",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RunQuery(); // recarrega (igual Python)
            }
            catch (Exception ex)
            {
                ShowError("Erro ao inativar duplicados", ex);
            }
        }

        // ── Teclas ────────────────────────────────────────────────────────────

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if      (e.KeyCode == Keys.F4) Close();
            else if (e.KeyCode == Keys.F5) RunQuery();
            else if (e.KeyCode == Keys.F6) ClearAll();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static DataGridView CreateGrid()
        {
            return new DataGridView
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
                AutoSizeMode     = fill ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = align },
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

        /// <summary>
        /// Equivalente a AlertaMovimentosDuplicadosNota._fmt_data:
        /// tenta varios formatos ISO/BR e retorna dd/MM/yyyy HH:mm.
        /// </summary>
        private static string FormatDateTime(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            var txt = raw.Trim();
            if (txt.Length > 19) txt = txt.Substring(0, 19);

            string[] fmts =
            {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm",
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy HH:mm",
            };
            if (DateTime.TryParseExact(txt, fmts, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
            return raw;
        }

        private static string FormatQuantity(decimal value)
        {
            return value.ToString("N3", CultureInfo.GetCultureInfo("pt-BR"));
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
