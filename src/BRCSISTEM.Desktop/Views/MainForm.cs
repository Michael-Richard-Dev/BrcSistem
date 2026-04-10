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
