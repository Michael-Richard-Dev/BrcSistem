using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Desktop.Interface.AlteracaoSenha;
using BRCSISTEM.Desktop.Interface.ConsultaLogsAuditoria;
using BRCSISTEM.Desktop.Interface.PerfisBancoDados;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    /// <summary>
    /// Porte fiel da tela principal do sistema Python (views/tela_principal.py):
    /// menubar no topo, area central vazia, sidebar direito (310px, bg #F8F9FA)
    /// e rodape azul (#007ACC) com nome do sistema, usuario e data.
    /// A parte visual fica em MainForm.Designer.cs (padrao Windows Forms Designer).
    /// </summary>
    public sealed partial class MainForm : Form
    {
        // Espelha utils/theme.py e config/constantes.py do projeto Python.
        private const string AppName = "BRCSISTEM";
        private const string AppVersion = "v3.1.20";
        private static readonly Color ColorFooterBg = Color.FromArgb(0, 122, 204);
        private static readonly Color ColorSidebarBg = Color.FromArgb(248, 249, 250);
        private static readonly Color ColorSeparator = Color.FromArgb(213, 216, 220);
        private static readonly Color ColorTextDark = Color.FromArgb(44, 62, 80);

        private readonly CompositionRoot _compositionRoot;
        private readonly MainController _mainController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly AppConfiguration _configuration;
        private bool _runtimeChromeInitialized;

        public MainForm()
        {
            InitializeComponent();
            ApplyBaseVisualTexts();

            if (IsInDesignMode())
            {
                ApplyDesignTimeVisualTexts();
            }
        }

        public MainForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
            : this()
        {
            _compositionRoot = compositionRoot;
            _mainController = compositionRoot.CreateMainController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _configuration = compositionRoot.CreateConfigurationController().LoadConfiguration();

            ApplyRuntimeVisualTexts();
            WireRuntimeEvents();
            EnsureRuntimeChrome();
        }

        private static bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void ApplyBaseVisualTexts()
        {
            labelFooterSystem.Text = AppName + " - " + AppVersion;
            _footerDateLabel.Text = DateTime.Now.ToString("dd/MM/yyyy");
            Text = AppName + " " + AppVersion + " - Principal";
        }

        private void ApplyDesignTimeVisualTexts()
        {
            labelFooterUser.Text = "Usuario: Design (Administrador)";
        }

        private void ApplyRuntimeVisualTexts()
        {
            labelFooterUser.Text = "Usuario: " + _identity.UserName + " (" + _identity.UserType + ")";
            FooterDateTimer_Tick(this, EventArgs.Empty);
        }

        private void WireRuntimeEvents()
        {
            buttonRefreshSidebar.Click -= ButtonRefreshSidebar_Click;
            buttonRefreshSidebar.Click += ButtonRefreshSidebar_Click;

            Shown -= MainForm_Shown;
            Shown += MainForm_Shown;

            _footerDateTimer.Tick -= FooterDateTimer_Tick;
            _footerDateTimer.Tick += FooterDateTimer_Tick;
            _footerDateTimer.Interval = 60000;
            _footerDateTimer.Start();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            EnsureRuntimeChrome();
        }

        private void EnsureRuntimeChrome()
        {
            if (_runtimeChromeInitialized)
            {
                RefreshSidebar();
                return;
            }

            BuildMenus();
            RefreshSidebar();
            _runtimeChromeInitialized = true;
        }

        // ── Eventos dos controles visuais ────────────────────────────────────
        private void ButtonRefreshSidebar_Click(object sender, EventArgs e)
        {
            RefreshSidebar();
        }

        private void FooterDateTimer_Tick(object sender, EventArgs e)
        {
            if (_footerDateLabel != null && !_footerDateLabel.IsDisposed)
            {
                _footerDateLabel.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        // ── Sidebar: conteudo dinamico (indicadores) ─────────────────────────
        private Control BuildSidebarSection(string title)
        {
            var holder = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = ColorSidebarBg,
                Margin = new Padding(0),
                Padding = new Padding(10, 10, 10, 5),
            };
            holder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            holder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            holder.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label
            {
                Text = title,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTextDark,
                BackColor = ColorSidebarBg,
                Margin = new Padding(0, 0, 0, 3),
            };

            var separator = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = ColorSeparator,
                Margin = new Padding(0, 0, 0, 5),
            };

            holder.Controls.Add(label, 0, 0);
            holder.Controls.Add(separator, 0, 1);
            return holder;
        }

        private void RefreshSidebar()
        {
            if (_sidebarContentFlow == null || _sidebarContentFlow.IsDisposed || _mainController == null)
            {
                return;
            }

            _sidebarContentFlow.SuspendLayout();
            try
            {
                ResetSidebar();

                var snapshot = _mainController.LoadSidebarSnapshot(_configuration, _databaseProfile);
                BuildFifoSection(snapshot);
                BuildCadastrosSection(snapshot);
                BuildVolumeEstoqueSection(snapshot);
                BuildAuditoriaSection(snapshot);
                BuildUsuariosSection(snapshot);
            }
            catch (Exception ex)
            {
                ResetSidebar();
                AddSidebarControl(BuildSidebarSection("Indicadores"));
                AddSidebarControl(CreateSidebarText("Erro: " + ShortenText(ex.Message, 80), Color.FromArgb(231, 76, 60), false, new Padding(10, 0, 10, 6)));
            }
            finally
            {
                _sidebarContentFlow.ResumeLayout(true);
            }
        }

        private void ResetSidebar()
        {
            _sidebarContentFlow.Controls.Clear();
            _sidebarContentFlow.RowStyles.Clear();
            _sidebarContentFlow.RowCount = 0;
        }

        private void AddSidebarControl(Control control)
        {
            if (control == null)
            {
                return;
            }

            var rowIndex = _sidebarContentFlow.RowCount;
            _sidebarContentFlow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _sidebarContentFlow.Controls.Add(control, 0, rowIndex);
            _sidebarContentFlow.RowCount = rowIndex + 1;
        }

        private void BuildFifoSection(MainSidebarSnapshot snapshot)
        {
            AddSidebarControl(BuildSidebarSection(" FIFO - Vencimento 60 dias"));
            if (snapshot.FifoEntries == null || snapshot.FifoEntries.Length == 0)
            {
                AddSidebarControl(CreateSidebarText(" Sem lotes criticos", Color.FromArgb(39, 174, 96), false, new Padding(10, 0, 10, 4)));
                return;
            }

            AddSidebarControl(CreateTableHeader(new[] { "Lote", "Material", "Val", "Saldo" }, new[] { 39F, 29F, 14F, 18F }));
            foreach (var entry in snapshot.FifoEntries)
            {
                var lot = entry.LotCode ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(entry.LotName))
                {
                    lot += " - " + ShortenText(entry.LotName, 15);
                }

                AddSidebarControl(CreateTableRow(
                    new[]
                    {
                        ShortenText(lot, 11),
                        ShortenText(entry.Material, 9),
                        ShortenText(entry.ExpirationDate, 9),
                        FormatInteger(entry.Balance),
                    },
                    new[] { 39F, 29F, 14F, 18F }));
            }
        }

        private void BuildCadastrosSection(MainSidebarSnapshot snapshot)
        {
            AddSidebarControl(BuildSidebarSection("Cadastros"));
            AddSidebarControl(CreateTableHeader(new[] { "Tabela", "Ativo", "Inativo", "Total", "BRC" }, new[] { 34F, 16.5F, 16.5F, 16.5F, 16.5F }));
            foreach (var row in snapshot.CadastroRows ?? new MainSidebarCadastroRow[0])
            {
                AddSidebarControl(CreateTableRow(
                    new[]
                    {
                        ShortenText(row.TableName, 11),
                        row.ActiveCount.ToString(CultureInfo.InvariantCulture),
                        row.InactiveCount.ToString(CultureInfo.InvariantCulture),
                        row.TotalCount.ToString(CultureInfo.InvariantCulture),
                        row.BrcCount.HasValue ? row.BrcCount.Value.ToString(CultureInfo.InvariantCulture) : "-",
                    },
                    new[] { 34F, 16.5F, 16.5F, 16.5F, 16.5F }));
            }
        }

        private void BuildVolumeEstoqueSection(MainSidebarSnapshot snapshot)
        {
            AddSidebarControl(BuildSidebarSection("Volume Estoque"));
            if (snapshot.VolumeRows == null || snapshot.VolumeRows.Length == 0)
            {
                AddSidebarControl(CreateSidebarText("Sem estoque", Color.FromArgb(93, 109, 126), false, new Padding(10, 0, 10, 4)));
                return;
            }

            foreach (var row in snapshot.VolumeRows)
            {
                AddSidebarControl(CreateValueRow(
                    ShortenText(row.WarehouseDisplay, 34),
                    FormatInteger(row.Volume),
                    ColorTextDark,
                    ColorSidebarBg,
                    false));
            }
        }

        private void BuildAuditoriaSection(MainSidebarSnapshot snapshot)
        {
            AddSidebarControl(BuildSidebarSection(" Auditoria"));
            AddSidebarControl(CreateTableHeader(new[] { "Auditoria", "Qtd" }, new[] { 74F, 26F }));
            foreach (var row in snapshot.AuditRows ?? new MainSidebarAuditRow[0])
            {
                var hasAlert = row.Count > 0;
                var backColor = hasAlert ? Color.FromArgb(255, 243, 205) : ColorSidebarBg;
                var foreColor = hasAlert ? Color.FromArgb(138, 109, 59) : Color.FromArgb(39, 174, 96);

                AddSidebarControl(CreateValueRow(
                    ShortenText(row.Label, 24),
                    row.Count.ToString(CultureInfo.InvariantCulture),
                    foreColor,
                    backColor,
                    true));
            }
        }

        private void BuildUsuariosSection(MainSidebarSnapshot snapshot)
        {
            AddSidebarControl(BuildSidebarSection("Usuarios e Acessos"));
            AddSidebarControl(CreateValueRow("Total ativos:", snapshot.ActiveUsersCount.ToString(CultureInfo.InvariantCulture), ColorTextDark, ColorSidebarBg, false));

            if (snapshot.RecentAccesses == null || snapshot.RecentAccesses.Length == 0)
            {
                AddSidebarControl(CreateSidebarText("Sem registros de acesso", Color.FromArgb(93, 109, 126), false, new Padding(10, 1, 10, 4)));
                return;
            }

            AddSidebarControl(CreateSidebarText("Ultimos acessos por usuario:", Color.FromArgb(93, 109, 126), false, new Padding(10, 1, 10, 2)));
            foreach (var access in snapshot.RecentAccesses)
            {
                AddSidebarControl(CreateValueRow(
                    "  " + ShortenText(access.UserName, 32),
                    access.LastAccessText ?? "-",
                    ColorTextDark,
                    ColorSidebarBg,
                    false));
            }
        }

        private Control CreateSidebarText(string text, Color color, bool bold, Padding margin)
        {
            var label = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(286, 0),
                Dock = DockStyle.Top,
                Text = text,
                Font = new Font("Segoe UI", 8F, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color,
                BackColor = ColorSidebarBg,
                Margin = margin,
                Padding = new Padding(0),
            };

            return label;
        }

        private Control CreateValueRow(string labelText, string valueText, Color valueColor, Color backColor, bool boldValue)
        {
            var row = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                BackColor = backColor,
                Margin = new Padding(10, 1, 10, 1),
                Padding = new Padding(0),
                Dock = DockStyle.Top,
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));

            row.Controls.Add(new Label
            {
                AutoSize = true,
                Text = labelText,
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(93, 109, 126),
                BackColor = backColor,
                Margin = new Padding(0),
                Anchor = AnchorStyles.Left,
            }, 0, 0);

            row.Controls.Add(new Label
            {
                AutoSize = true,
                Text = valueText,
                Font = new Font("Segoe UI", 8F, boldValue ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = valueColor,
                BackColor = backColor,
                Margin = new Padding(0),
                Anchor = AnchorStyles.Right,
            }, 1, 0);

            return row;
        }

        private Control CreateTableHeader(string[] values, float[] widths)
        {
            return CreateTableRow(values, widths, true);
        }

        private Control CreateTableRow(string[] values, float[] widths)
        {
            return CreateTableRow(values, widths, false);
        }

        private Control CreateTableRow(string[] values, float[] widths, bool header)
        {
            var row = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = values.Length,
                BackColor = ColorSidebarBg,
                Margin = header ? new Padding(10, 5, 10, 2) : new Padding(10, 0, 10, 0),
                Padding = new Padding(0),
                Dock = DockStyle.Top,
            };

            for (var i = 0; i < values.Length; i++)
            {
                row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, widths[i]));
                row.Controls.Add(new Label
                {
                    AutoSize = true,
                    Text = values[i],
                    Font = new Font("Segoe UI", header ? 7F : 7.5F, header ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = header ? ColorTextDark : Color.FromArgb(93, 109, 126),
                    BackColor = ColorSidebarBg,
                    Margin = new Padding(0),
                    Anchor = i == 0 ? AnchorStyles.Left : AnchorStyles.None,
                    TextAlign = i == 0 ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter,
                }, i, 0);
            }

            return row;
        }

        private static string ShortenText(string value, int maxLength)
        {
            var text = value ?? string.Empty;
            if (text.Length <= maxLength)
            {
                return text;
            }

            if (maxLength <= 3)
            {
                return text.Substring(0, maxLength);
            }

            return text.Substring(0, maxLength - 3) + "...";
        }

        private static string FormatInteger(decimal value)
        {
            return value.ToString("#,0", CultureInfo.InvariantCulture).Replace(",", ".");
        }

        // ── Menus (populados em runtime a partir do catalogo) ────────────────
        private void BuildMenus()
        {
            mainMenuStrip.SuspendLayout();
            mainMenuStrip.Items.Clear();

            var modules = _mainController.LoadModules(_identity) ?? Array.Empty<ModuleDefinition>();
            var grouped = modules
                .GroupBy(module => module.Group ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            AddGroupMenu(grouped, "Cadastros");
            AddGroupMenu(grouped, "Movimentacoes");
            AddGroupMenu(grouped, "Inventario");
            AddGroupMenu(grouped, "Consultas e Relatorios");
            AddGroupMenu(grouped, "Auditoria");
            AddDatabaseMenu(grouped);
            AddParametersMenu(grouped);
            AddSystemMenu();

            mainMenuStrip.Visible = true;
            mainMenuStrip.ResumeLayout(true);
            mainMenuStrip.PerformLayout();
        }

        private void AddGroupMenu(Dictionary<string, List<ModuleDefinition>> grouped, string groupName)
        {
            List<ModuleDefinition> items;
            if (!grouped.TryGetValue(groupName, out items) || items.Count == 0)
            {
                return;
            }

            var groupMenu = new ToolStripMenuItem(groupName);
            foreach (var module in items)
            {
                groupMenu.DropDownItems.Add(BuildModuleMenuItem(module));
            }

            mainMenuStrip.Items.Add(groupMenu);
        }

        private void AddDatabaseMenu(Dictionary<string, List<ModuleDefinition>> grouped)
        {
            List<ModuleDefinition> items;
            if (!grouped.TryGetValue("Banco de Dados", out items) || items.Count == 0)
            {
                return;
            }

            var bdMenu = new ToolStripMenuItem("Banco de Dados");

            var removerKeys = new[] { "bd_remover_nota", "bd_remover_transferencia", "bd_remover_saida", "bd_remover_requisicao" };
            AddSubCascade(bdMenu, "Retirar Informacoes", items, removerKeys);

            var reativarKeys = new[] { "bd_reativar_nota_entrada" };
            AddSubCascade(bdMenu, "Reativar", items, reativarKeys);

            var alterarKeys = new[] { "bd_alterar_data_transferencia", "bd_alterar_data_entrada", "bd_alterar_data_saida_producao" };
            AddSubCascade(bdMenu, "Alterar Data", items, alterarKeys);

            var logItem = items.FirstOrDefault(m => string.Equals(m.Key, "bd_consulta_logs", StringComparison.OrdinalIgnoreCase));
            if (logItem != null)
            {
                if (bdMenu.DropDownItems.Count > 0)
                {
                    bdMenu.DropDownItems.Add(new ToolStripSeparator());
                }

                bdMenu.DropDownItems.Add(BuildModuleMenuItem(logItem));
            }

            var handled = new HashSet<string>(removerKeys.Concat(reativarKeys).Concat(alterarKeys).Concat(new[] { "bd_consulta_logs" }), StringComparer.OrdinalIgnoreCase);
            foreach (var extra in items.Where(m => !handled.Contains(m.Key)))
            {
                bdMenu.DropDownItems.Add(BuildModuleMenuItem(extra));
            }

            if (bdMenu.DropDownItems.Count > 0)
            {
                mainMenuStrip.Items.Add(bdMenu);
            }
        }

        private void AddSubCascade(ToolStripMenuItem parent, string title, List<ModuleDefinition> all, string[] orderedKeys)
        {
            var sub = new ToolStripMenuItem(title);
            foreach (var key in orderedKeys)
            {
                var mod = all.FirstOrDefault(m => string.Equals(m.Key, key, StringComparison.OrdinalIgnoreCase));
                if (mod != null)
                {
                    sub.DropDownItems.Add(BuildModuleMenuItem(mod));
                }
            }

            if (sub.DropDownItems.Count > 0)
            {
                parent.DropDownItems.Add(sub);
            }
        }

        private void AddParametersMenu(Dictionary<string, List<ModuleDefinition>> grouped)
        {
            List<ModuleDefinition> items;
            if (!grouped.TryGetValue("Parametros", out items) || items.Count == 0)
            {
                return;
            }

            var menu = new ToolStripMenuItem("Parametros");
            TryAddByKey(menu, items, "cadastro_usuario");
            TryAddByKey(menu, items, "tipo_usuario");

            var gerenciar = items.FirstOrDefault(m => string.Equals(m.Key, "gerenciar_acessos", StringComparison.OrdinalIgnoreCase));
            if (gerenciar != null)
            {
                if (menu.DropDownItems.Count > 0)
                {
                    menu.DropDownItems.Add(new ToolStripSeparator());
                }

                menu.DropDownItems.Add(BuildModuleMenuItem(gerenciar));
            }

            if (menu.DropDownItems.Count > 0)
            {
                menu.DropDownItems.Add(new ToolStripSeparator());
            }

            TryAddByKey(menu, items, "parametros");
            TryAddByKey(menu, items, "parametro_sincronizar_movimentos_estoque");

            var handled = new HashSet<string>(new[] { "cadastro_usuario", "tipo_usuario", "gerenciar_acessos", "parametros", "parametro_sincronizar_movimentos_estoque" }, StringComparer.OrdinalIgnoreCase);
            foreach (var extra in items.Where(m => !handled.Contains(m.Key)))
            {
                menu.DropDownItems.Add(BuildModuleMenuItem(extra));
            }

            if (menu.DropDownItems.Count > 0)
            {
                mainMenuStrip.Items.Add(menu);
            }
        }

        private void TryAddByKey(ToolStripMenuItem parent, List<ModuleDefinition> all, string key)
        {
            var mod = all.FirstOrDefault(m => string.Equals(m.Key, key, StringComparison.OrdinalIgnoreCase));
            if (mod != null)
            {
                parent.DropDownItems.Add(BuildModuleMenuItem(mod));
            }
        }

        private ToolStripMenuItem BuildModuleMenuItem(ModuleDefinition module)
        {
            var item = new ToolStripMenuItem(module.Title);
            item.Click += (sender, args) => OpenModule(module);
            return item;
        }

        private void AddSystemMenu()
        {
            var systemMenu = new ToolStripMenuItem("Sistema");

            var manageProfilesItem = new ToolStripMenuItem("Gerenciar Bancos");
            manageProfilesItem.Click += (sender, args) =>
            {
                using (var dialog = new PerfisBancoDadosForm(_compositionRoot))
                {
                    dialog.ShowDialog(this);
                }
            };

            var changePasswordItem = new ToolStripMenuItem("Trocar Minha Senha");
            changePasswordItem.Click += (sender, args) =>
            {
                var configuration = _compositionRoot.CreateConfigurationController().LoadConfiguration();
                using (var dialog = new AlteracaoSenhaForm(_compositionRoot, configuration, _databaseProfile, _identity.UserName, false))
                {
                    dialog.ShowDialog(this);
                }
            };

            var versionItem = new ToolStripMenuItem("Versao do Sistema: " + AppVersion) { Enabled = false };
            var exitItem = new ToolStripMenuItem("Sair");
            exitItem.Click += (sender, args) => Close();

            systemMenu.DropDownItems.Add(manageProfilesItem);
            systemMenu.DropDownItems.Add(changePasswordItem);
            systemMenu.DropDownItems.Add(new ToolStripSeparator());
            systemMenu.DropDownItems.Add(versionItem);
            systemMenu.DropDownItems.Add(new ToolStripSeparator());
            systemMenu.DropDownItems.Add(exitItem);

            mainMenuStrip.Items.Add(systemMenu);
        }

        // ── Dispatch inalterado ──────────────────────────────────────────────
        private void OpenModule(ModuleDefinition module)
        {
            _mainController.RegisterModuleOpen(_identity, module);
            if (string.Equals(module.Key, "cadastro_usuario", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new UserManagementForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "tipo_usuario", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new UserTypeManagementForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "parametros", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new SystemParametersForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "gerenciar_acessos", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new GerenciamentoSolicitacoesAcessoForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "parametro_sincronizar_movimentos_estoque", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new StockMovementSynchronizationForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "cadastro_fornecedor", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new SupplierManagementForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "cadastro_embalagem", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new PackagingManagementForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "cadastro_produto", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new ProductManagementForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "cadastro_lote", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new LotManagementForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "cadastro_almoxarifado", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new WarehouseManagementForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "movimentacao_entrada", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new MovimentacaoEntradaForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "movimentacao_saida_producao", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new ProductionOutputForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "movimentacao_transferencia", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new StockTransferForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "movimentacao_requisicao", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new MaterialRequisitionForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "movimentacao_inventario", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new InventoryForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "conta_corrente_estoque", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new StockLedgerForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "resumo_sintetico", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new StockSummaryForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "consulta_nota_entrada", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new ConsultaNotaEntradaForm(_compositionRoot.CreateInboundReceiptController(), _configuration, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "relatorio_entrada_pdf", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new InboundReceiptPdfReportForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "relatorio_producao_saida_pdf", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new ProductionOutputPdfReportForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "relatorio_transferencia_pdf", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new StockTransferPdfReportForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "relatorio_inventario_pdf", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new InventoryPdfReportForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "relatorio_movimentacao_estoque", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new StockMovementReportForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "alerta_lote_descricao_duplicada_material", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new DuplicateLotAlertForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "alerta_estoque_negativo_antes_entrada", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new NegativeStockAlertForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "alerta_entrada_lote_divergente", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new DivergentLotEntryAlertForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_inconsistencias_lote_material", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new LotMaterialInconsistencyForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "alerta_movimentos_duplicados_nota", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new DuplicateNoteMovementsAlertForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_alterar_data_saida_producao", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new ProductionOutputDateChangeForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_alterar_data_entrada", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new NoteDateChangeForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_alterar_data_transferencia", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new TransferDateChangeForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_consulta_logs", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new ConsultaLogsAuditoriaForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_reativar_nota_entrada", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new InboundReceiptReactivationForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_remover_nota", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new RemoveNoteForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_remover_requisicao", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new RemoveRequisitionForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_remover_transferencia", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new RemoveTransferForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else if (string.Equals(module.Key, "bd_remover_saida", StringComparison.OrdinalIgnoreCase))
            {
                using (var dialog = new RemoveProductionOutputForm(_compositionRoot, _identity, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }
            else
            {
                using (var dialog = new ModulePlaceholderForm(module, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }

            RefreshSidebar();
        }
    }
}
