using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class MainForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly MainController _mainController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private ListBox _recentModulesListBox;

        public MainForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _mainController = compositionRoot.CreateMainController();
            _identity = identity;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            BuildMenus();
            LoadRecentModules();
        }

        private void InitializeComponent()
        {
            Text = $"BRCSISTEM - Principal ({_identity.DisplayName})";
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                BackColor = Color.White,
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = "Migracao base do BRCSISTEM para .NET Framework",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(18, 18, 18, 6),
            };

            var summaryLabel = new Label
            {
                AutoSize = true,
                Text = $"Usuario: {_identity.DisplayName} ({_identity.UserType})   |   Banco ativo: {_databaseProfile.DisplayName}\nA navegação principal já foi portada para WinForms/MVC. Cada módulo abre um placeholder apontando para o arquivo Python original que ainda precisa ser migrado.",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(73, 80, 87),
                Margin = new Padding(18, 0, 18, 12),
            };

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Margin = new Padding(18, 0, 18, 18),
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));

            var overviewPanel = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Visao Geral da Migracao",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };
            var overviewText = new Label
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                Text = "Base pronta nesta etapa:\n\n- leitura e escrita do config_db.json\n- conexao PostgreSQL com compatibilidade para Npgsql\n- bootstrap das tabelas de usuarios, tipos, parametros e auditoria\n- autenticacao compatível com SHA256 + fallback MD5\n- troca de senha obrigatoria e manual\n- menu dinâmico com todos os módulos principais mapeados",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            };
            overviewPanel.Controls.Add(overviewText);

            var recentPanel = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = "Ultimos Modulos Abertos",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };
            _recentModulesListBox = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
            };
            recentPanel.Controls.Add(_recentModulesListBox);

            content.Controls.Add(overviewPanel, 0, 0);
            content.Controls.Add(recentPanel, 1, 0);

            root.Controls.Add(titleLabel, 0, 0);
            root.Controls.Add(summaryLabel, 0, 1);
            root.Controls.Add(content, 0, 2);
            Controls.Add(root);
        }

        private void BuildMenus()
        {
            var menuStrip = new MenuStrip();
            var modules = _mainController.LoadModules(_identity);

            foreach (var group in modules.GroupBy(module => module.Group))
            {
                var groupMenu = new ToolStripMenuItem(group.Key);
                foreach (var module in group)
                {
                    var moduleItem = new ToolStripMenuItem(module.Title);
                    moduleItem.Click += (sender, args) => OpenModule(module);
                    groupMenu.DropDownItems.Add(moduleItem);
                }

                menuStrip.Items.Add(groupMenu);
            }

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

            var exitItem = new ToolStripMenuItem("Sair");
            exitItem.Click += (sender, args) => Close();

            systemMenu.DropDownItems.Add(manageProfilesItem);
            systemMenu.DropDownItems.Add(changePasswordItem);
            systemMenu.DropDownItems.Add(new ToolStripSeparator());
            systemMenu.DropDownItems.Add(exitItem);
            menuStrip.Items.Add(systemMenu);

            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);
        }

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
            else
            {
                using (var dialog = new ModulePlaceholderForm(module, _databaseProfile))
                {
                    dialog.ShowDialog(this);
                }
            }

            LoadRecentModules();
        }

        private void LoadRecentModules()
        {
            var session = _mainController.LoadSession(_identity);
            _recentModulesListBox.DataSource = session.OpenModules.Select(item => item.Title ?? item.ModuleKey).ToArray();
        }
    }
}
