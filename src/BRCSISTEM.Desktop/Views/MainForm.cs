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
    /// Porte fiel da tela principal do sistema Python (views/tela_principal.py):
    /// menubar no topo, area central vazia, sidebar direito (310px, bg #F8F9FA)
    /// e rodape azul (#007ACC) com nome do sistema, usuario e data.
    /// </summary>
    public sealed class MainForm : Form
    {
        // Espelha utils/theme.py e config/constantes.py do projeto Python.
        private const string AppName     = "BRCSISTEM";
        private const string AppVersion  = "v3.1.20";
        private static readonly Color ColorFooterBg  = Color.FromArgb(0, 122, 204);   // FOOTER_BG
        private static readonly Color ColorSidebarBg = Color.FromArgb(248, 249, 250); // sidebar_indicadores
        private static readonly Color ColorSeparator = Color.FromArgb(213, 216, 220); // separador fino dos titulos
        private static readonly Color ColorTextDark  = Color.FromArgb(44, 62, 80);    // #2c3e50

        private readonly CompositionRoot _compositionRoot;
        private readonly MainController _mainController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly AppConfiguration _configuration;

        private FlowLayoutPanel _sidebarContentFlow;
        private Label _footerDateLabel;
        private Timer _footerDateTimer;

        public MainForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _mainController = compositionRoot.CreateMainController();
            _identity = identity;
            _databaseProfile = databaseProfile;
            _configuration = compositionRoot.CreateConfigurationController().LoadConfiguration();

            InitializeComponent();
            BuildMenus();
            RefreshSidebar();
        }

        private void InitializeComponent()
        {
            // Python: master.title(f"{APP_NAME} {APP_VERSION} - Principal")
            Text = AppName + " " + AppVersion + " - Principal";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1000, 600);
            BackColor = Color.White;

            // Ordem de docking (WinForms empilha do ultimo para o primeiro):
            //   1) rodape (Bottom)    <- adicionado por ultimo, fica mais interno? Nao: Dock Bottom eh baseado em ordem inversa de add.
            //   Para garantir "rodape colado ao fundo + central acima", adicionamos rodape antes e central depois com Dock=Fill.
            Controls.Add(BuildBody());
            Controls.Add(BuildFooter());
        }

        // ── Corpo principal: area central vazia + sidebar direito ────────────
        private Control BuildBody()
        {
            // Python: frame_principal (bg=RIGHT_BG) > container_main > (frame_central | sidebar)
            var body = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 2,
                RowCount    = 1,
                BackColor   = Color.White,
                Margin      = new Padding(0),
                Padding     = new Padding(0),
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310F));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Area central (Python: frame_central vazio, reservado para conteudo futuro)
            var central = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White,
            };
            body.Controls.Add(central, 0, 0);

            // Sidebar direito (Python: SidebarIndicadores width=310, bg #F8F9FA)
            body.Controls.Add(BuildSidebar(), 1, 0);
            return body;
        }

        private Control BuildSidebar()
        {
            var sidebar = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 1,
                RowCount    = 2,
                BackColor   = ColorSidebarBg,
                Margin      = new Padding(0),
                Padding     = new Padding(0),
            };
            sidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Conteudo com scroll equivalente ao SidebarIndicadores do Python.
            var content = new Panel
            {
                Dock         = DockStyle.Fill,
                BackColor    = ColorSidebarBg,
                AutoScroll   = true,
                Padding      = new Padding(0, 4, 0, 4),
            };

            _sidebarContentFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = ColorSidebarBg,
                Margin = new Padding(0),
                Padding = new Padding(0),
            };
            content.Controls.Add(_sidebarContentFlow);

            sidebar.Controls.Add(content, 0, 0);

            // Rodape do sidebar com botao "Atualizar" (Python: rodape + btn_atualizar)
            var sidebarFooter = new Panel
            {
                Dock      = DockStyle.Fill,
                Height    = 30,
                BackColor = ColorSidebarBg,
                Padding   = new Padding(0, 4, 0, 6),
            };
            var refreshButton = new Button
            {
                Text      = "\u27f3 Atualizar",
                Font      = new Font("Segoe UI", 9F),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorSidebarBg,
                ForeColor = ColorTextDark,
                AutoSize  = true,
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Top,
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += (sender, args) => RefreshSidebar();
            // Centraliza horizontalmente
            sidebarFooter.Resize += (sender, args) =>
            {
                refreshButton.Left = Math.Max(0, (sidebarFooter.ClientSize.Width - refreshButton.Width) / 2);
                refreshButton.Top  = 4;
            };
            sidebarFooter.Controls.Add(refreshButton);
            sidebar.Controls.Add(sidebarFooter, 0, 1);

            return sidebar;
        }

        private Control BuildSidebarSection(string title)
        {
            // Python (_titulo): Label bold + Frame 1px #d5d8dc como separador horizontal
            var holder = new TableLayoutPanel
            {
                Dock         = DockStyle.Top,
                AutoSize     = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount  = 1,
                RowCount     = 2,
                BackColor    = ColorSidebarBg,
                Margin       = new Padding(0),
                Padding      = new Padding(10, 10, 10, 5),
            };
            holder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            holder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            holder.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label
            {
                Text      = title,
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = ColorTextDark,
                BackColor = ColorSidebarBg,
                Margin    = new Padding(0, 0, 0, 3),
            };
            var separator = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 1,
                BackColor = ColorSeparator,
                Margin    = new Padding(0, 0, 0, 5),
            };
            holder.Controls.Add(label,     0, 0);
            holder.Controls.Add(separator, 0, 1);
            return holder;
        }

        private void RefreshSidebar()
        {
            if (_sidebarContentFlow == null || _sidebarContentFlow.IsDisposed)
            {
                return;
            }

            _sidebarContentFlow.SuspendLayout();
            try
            {
                _sidebarContentFlow.Controls.Clear();

                var snapshot = _mainController.LoadSidebarSnapshot(_configuration, _databaseProfile);
                BuildFifoSection(snapshot);
                BuildCadastrosSection(snapshot);
                BuildVolumeEstoqueSection(snapshot);
                BuildAuditoriaSection(snapshot);
                BuildUsuariosSection(snapshot);
            }
            catch (Exception ex)
            {
                _sidebarContentFlow.Controls.Clear();
                _sidebarContentFlow.Controls.Add(BuildSidebarSection("Indicadores"));
                _sidebarContentFlow.Controls.Add(CreateSidebarText("Erro: " + ShortenText(ex.Message, 80), Color.FromArgb(231, 76, 60), false, new Padding(10, 0, 10, 6)));
            }
            finally
            {
                _sidebarContentFlow.ResumeLayout(true);
            }
        }

        private void BuildFifoSection(MainSidebarSnapshot snapshot)
        {
            _sidebarContentFlow.Controls.Add(BuildSidebarSection(" FIFO - Vencimento 60 dias"));
            if (snapshot.FifoEntries == null || snapshot.FifoEntries.Length == 0)
            {
                _sidebarContentFlow.Controls.Add(CreateSidebarText(" Sem lotes criticos", Color.FromArgb(39, 174, 96), false, new Padding(10, 0, 10, 4)));
                return;
            }

            _sidebarContentFlow.Controls.Add(CreateTableHeader(new[] { "Lote", "Material", "Val", "Saldo" }, new[] { 39F, 29F, 14F, 18F }));
            foreach (var entry in snapshot.FifoEntries)
            {
                var lot = entry.LotCode ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(entry.LotName))
                {
                    lot += " - " + ShortenText(entry.LotName, 15);
                }

                _sidebarContentFlow.Controls.Add(CreateTableRow(
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
            _sidebarContentFlow.Controls.Add(BuildSidebarSection("Cadastros"));
            _sidebarContentFlow.Controls.Add(CreateTableHeader(new[] { "Tabela", "Ativo", "Inativo", "Total", "BRC" }, new[] { 34F, 16.5F, 16.5F, 16.5F, 16.5F }));
            foreach (var row in snapshot.CadastroRows ?? new MainSidebarCadastroRow[0])
            {
                _sidebarContentFlow.Controls.Add(CreateTableRow(
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
            _sidebarContentFlow.Controls.Add(BuildSidebarSection("Volume Estoque"));
            if (snapshot.VolumeRows == null || snapshot.VolumeRows.Length == 0)
            {
                _sidebarContentFlow.Controls.Add(CreateSidebarText("Sem estoque", Color.FromArgb(93, 109, 126), false, new Padding(10, 0, 10, 4)));
                return;
            }

            foreach (var row in snapshot.VolumeRows)
            {
                _sidebarContentFlow.Controls.Add(CreateValueRow(
                    ShortenText(row.WarehouseDisplay, 34),
                    FormatInteger(row.Volume),
                    ColorTextDark,
                    ColorSidebarBg,
                    false));
            }
        }

        private void BuildAuditoriaSection(MainSidebarSnapshot snapshot)
        {
            _sidebarContentFlow.Controls.Add(BuildSidebarSection(" Auditoria"));
            _sidebarContentFlow.Controls.Add(CreateTableHeader(new[] { "Auditoria", "Qtd" }, new[] { 74F, 26F }));
            foreach (var row in snapshot.AuditRows ?? new MainSidebarAuditRow[0])
            {
                var hasAlert = row.Count > 0;
                var backColor = hasAlert ? Color.FromArgb(255, 243, 205) : ColorSidebarBg;
                var foreColor = hasAlert ? Color.FromArgb(138, 109, 59) : Color.FromArgb(39, 174, 96);
                _sidebarContentFlow.Controls.Add(CreateValueRow(
                    ShortenText(row.Label, 24),
                    row.Count.ToString(CultureInfo.InvariantCulture),
                    foreColor,
                    backColor,
                    true));
            }
        }

        private void BuildUsuariosSection(MainSidebarSnapshot snapshot)
        {
            _sidebarContentFlow.Controls.Add(BuildSidebarSection("Usuarios e Acessos"));
            _sidebarContentFlow.Controls.Add(CreateValueRow("Total ativos:", snapshot.ActiveUsersCount.ToString(CultureInfo.InvariantCulture), ColorTextDark, ColorSidebarBg, false));

            if (snapshot.RecentAccesses == null || snapshot.RecentAccesses.Length == 0)
            {
                _sidebarContentFlow.Controls.Add(CreateSidebarText("Sem registros de acesso", Color.FromArgb(93, 109, 126), false, new Padding(10, 1, 10, 4)));
                return;
            }

            _sidebarContentFlow.Controls.Add(CreateSidebarText("Ultimos acessos por usuario:", Color.FromArgb(93, 109, 126), false, new Padding(10, 1, 10, 2)));
            foreach (var access in snapshot.RecentAccesses)
            {
                _sidebarContentFlow.Controls.Add(CreateValueRow(
                    "  " + ShortenText(access.UserName, 32),
                    access.LastAccessText ?? "-",
                    ColorTextDark,
                    ColorSidebarBg,
                    false));
            }
        }

        private Control CreateSidebarText(string text, Color color, bool bold, Padding margin)
        {
            return new Label
            {
                AutoSize = true,
                MaximumSize = new Size(280, 0),
                Text = text,
                Font = new Font("Segoe UI", 8F, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = color,
                BackColor = ColorSidebarBg,
                Margin = margin,
                Padding = new Padding(0),
            };
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
                Width = 270,
                MinimumSize = new Size(270, 0),
                MaximumSize = new Size(270, 200),
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
                Width = 270,
                MinimumSize = new Size(270, 0),
                MaximumSize = new Size(270, 200),
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

        // ── Rodape azul (Python: utils/ui.adicionar_rodape) ──────────────────
        private Control BuildFooter()
        {
            var footer = new TableLayoutPanel
            {
                Dock        = DockStyle.Bottom,
                Height      = 28,
                ColumnCount = 3,
                RowCount    = 1,
                BackColor   = ColorFooterBg,
                Margin      = new Padding(0),
                Padding     = new Padding(10, 0, 10, 0),
            };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            footer.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var systemLabel = new Label
            {
                Text      = AppName + " - " + AppVersion,
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                BackColor = ColorFooterBg,
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor    = AnchorStyles.Left,
                Margin    = new Padding(0, 0, 0, 0),
            };
            var userText = "Usuario: " + _identity.UserName + " (" + _identity.UserType + ")";
            var userLabel = new Label
            {
                Text      = userText,
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                BackColor = ColorFooterBg,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            _footerDateLabel = new Label
            {
                Text      = DateTime.Now.ToString("dd/MM/yyyy"),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                BackColor = ColorFooterBg,
                TextAlign = ContentAlignment.MiddleRight,
                Anchor    = AnchorStyles.Right,
            };

            footer.Controls.Add(systemLabel,      0, 0);
            footer.Controls.Add(userLabel,        1, 0);
            footer.Controls.Add(_footerDateLabel, 2, 0);

            // Timer leve para manter a data atualizada (Python faz o mesmo via after())
            _footerDateTimer = new Timer { Interval = 60 * 1000 };
            _footerDateTimer.Tick += (sender, args) =>
            {
                if (_footerDateLabel != null && !_footerDateLabel.IsDisposed)
                {
                    _footerDateLabel.Text = DateTime.Now.ToString("dd/MM/yyyy");
                }
            };
            _footerDateTimer.Start();
            FormClosed += (sender, args) =>
            {
                if (_footerDateTimer != null)
                {
                    _footerDateTimer.Stop();
                    _footerDateTimer.Dispose();
                    _footerDateTimer = null;
                }
            };

            return footer;
        }

        // ── Menus ────────────────────────────────────────────────────────────
        private void BuildMenus()
        {
            var menuStrip = new MenuStrip { Font = new Font("Segoe UI", 10F) };

            var modules = _mainController.LoadModules(_identity) ?? Array.Empty<ModuleDefinition>();
            var grouped = modules
                .GroupBy(module => module.Group ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            // Ordem de cascades espelhando views/tela_principal.py
            AddGroupMenu(menuStrip, grouped, "Cadastros");
            AddGroupMenu(menuStrip, grouped, "Movimentacoes");
            AddGroupMenu(menuStrip, grouped, "Inventario");
            AddGroupMenu(menuStrip, grouped, "Consultas e Relatorios");
            AddGroupMenu(menuStrip, grouped, "Auditoria");
            AddDatabaseMenu(menuStrip, grouped);
            AddParametersMenu(menuStrip, grouped);
            AddSystemMenu(menuStrip);

            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);
        }

        private void AddGroupMenu(MenuStrip menuStrip, Dictionary<string, List<ModuleDefinition>> grouped, string groupName)
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
            menuStrip.Items.Add(groupMenu);
        }

        private void AddDatabaseMenu(MenuStrip menuStrip, Dictionary<string, List<ModuleDefinition>> grouped)
        {
            // Python: submenus Retirar Informacoes / Reativar / Alterar Data + Consultar Logs
            List<ModuleDefinition> items;
            if (!grouped.TryGetValue("Banco de Dados", out items) || items.Count == 0)
            {
                return;
            }

            var bdMenu = new ToolStripMenuItem("Banco de Dados");

            // Retirar Informacoes
            var removerKeys = new[] { "bd_remover_nota", "bd_remover_transferencia", "bd_remover_saida", "bd_remover_requisicao" };
            AddSubCascade(bdMenu, "Retirar Informacoes", items, removerKeys);

            // Reativar
            var reativarKeys = new[] { "bd_reativar_nota_entrada" };
            AddSubCascade(bdMenu, "Reativar", items, reativarKeys);

            // Alterar Data
            var alterarKeys = new[] { "bd_alterar_data_transferencia", "bd_alterar_data_entrada", "bd_alterar_data_saida_producao" };
            AddSubCascade(bdMenu, "Alterar Data", items, alterarKeys);

            // Separador + Consultar Logs e Auditoria
            var logItem = items.FirstOrDefault(m => string.Equals(m.Key, "bd_consulta_logs", StringComparison.OrdinalIgnoreCase));
            if (logItem != null)
            {
                if (bdMenu.DropDownItems.Count > 0)
                {
                    bdMenu.DropDownItems.Add(new ToolStripSeparator());
                }
                bdMenu.DropDownItems.Add(BuildModuleMenuItem(logItem));
            }

            // Itens extras do catalogo que nao se encaixam em nenhum submenu conhecido
            var handled = new HashSet<string>(removerKeys.Concat(reativarKeys).Concat(alterarKeys).Concat(new[] { "bd_consulta_logs" }), StringComparer.OrdinalIgnoreCase);
            foreach (var extra in items.Where(m => !handled.Contains(m.Key)))
            {
                bdMenu.DropDownItems.Add(BuildModuleMenuItem(extra));
            }

            if (bdMenu.DropDownItems.Count > 0)
            {
                menuStrip.Items.Add(bdMenu);
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

        private void AddParametersMenu(MenuStrip menuStrip, Dictionary<string, List<ModuleDefinition>> grouped)
        {
            // Python:
            //   Cadastro Usuarios | Tipos Usuario | [sep + Solicitacoes Acesso se admin]
            //   sep | Parametros do Sistema | Sincronizar Movimentos x Estoque
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

            // Separador entre bloco de usuarios e bloco de parametros
            if (menu.DropDownItems.Count > 0)
            {
                menu.DropDownItems.Add(new ToolStripSeparator());
            }
            TryAddByKey(menu, items, "parametros");
            TryAddByKey(menu, items, "parametro_sincronizar_movimentos_estoque");

            // Itens extras eventualmente adicionados ao catalogo
            var handled = new HashSet<string>(new[] { "cadastro_usuario", "tipo_usuario", "gerenciar_acessos", "parametros", "parametro_sincronizar_movimentos_estoque" }, StringComparer.OrdinalIgnoreCase);
            foreach (var extra in items.Where(m => !handled.Contains(m.Key)))
            {
                menu.DropDownItems.Add(BuildModuleMenuItem(extra));
            }

            if (menu.DropDownItems.Count > 0)
            {
                menuStrip.Items.Add(menu);
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

        private void AddSystemMenu(MenuStrip menuStrip)
        {
            // Python (Sistema): Trocar Senha | sep | Versao do Sistema: v3.1.20 | sep | Sair
            // Mantemos "Gerenciar Bancos" (especifico do C# multi-profile, sem equivalente Python).
            var systemMenu = new ToolStripMenuItem("Sistema");

            var manageProfilesItem = new ToolStripMenuItem("Gerenciar Bancos");
            manageProfilesItem.Click += (sender, args) =>
            {
                using (var dialog = new DatabaseProfilesForm(_compositionRoot))
                {
                    dialog.ShowDialog(this);
                }
            };

            var changePasswordItem = new ToolStripMenuItem("Trocar Minha Senha");
            changePasswordItem.Click += (sender, args) =>
            {
                var configuration = _compositionRoot.CreateConfigurationController().LoadConfiguration();
                using (var dialog = new ChangePasswordForm(_compositionRoot, configuration, _databaseProfile, _identity.UserName, false))
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

            menuStrip.Items.Add(systemMenu);
        }

        // ── Dispatch inalterado (apenas funcao existente) ────────────────────
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
                using (var dialog = new AccessRequestManagementForm(_compositionRoot, _identity, _databaseProfile))
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
                using (var dialog = new InboundReceiptForm(_compositionRoot, _identity, _databaseProfile))
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
                using (var dialog = new AuditLogQueryForm(_compositionRoot, _identity, _databaseProfile))
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
