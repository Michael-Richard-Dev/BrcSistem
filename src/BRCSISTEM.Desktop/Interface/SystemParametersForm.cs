using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    /// <summary>
    /// Porte fiel de views/parametros.py (classe Parametros).
    /// Reproduz as 6 abas da tela original: Geral, Travas de Movimento,
    /// Controle de Acesso, Turnos, Motivos de Requisicao e Desbloqueio de Registros.
    /// </summary>
    public sealed partial class SystemParametersForm : Form
    {
        private readonly DatabaseMaintenanceController _databaseMaintenanceController;
        private readonly AdministrationController      _administrationController;
        private readonly ConfigurationController       _configurationController;
        private readonly UserIdentity    _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;

        private TabControl _tabControl;
        private TabPage    _tabGeneral;
        private TabPage    _tabLocks;
        private TabPage    _tabAccess;
        private TabPage    _tabShifts;
        private TabPage    _tabReasons;
        private TabPage    _tabUnlock;

        public SystemParametersForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _databaseMaintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
            _administrationController      = compositionRoot.CreateAdministrationController();
            _configurationController       = compositionRoot.CreateConfigurationController();
            _identity        = identity;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            Load += (sender, args) => OnInitialLoad();
        }

        private void InitializeComponent()
        {
            Text          = "BRCSISTEM - Parametros do Sistema";
            StartPosition = FormStartPosition.CenterParent;
            Size          = new Size(1200, 800);
            MinimumSize   = new Size(960, 600);
            BackColor     = Color.White;
            KeyPreview    = true;
            KeyDown      += (sender, args) =>
            {
                if (args.KeyCode == Keys.F4 || args.KeyCode == Keys.Escape)
                {
                    args.Handled = true;
                    Close();
                }
            };

            _tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5F),
            };

            _tabGeneral = new TabPage("Geral");
            _tabLocks   = new TabPage("Travas de Movimento");
            _tabAccess  = new TabPage("Controle de Acesso");
            _tabShifts  = new TabPage("Turnos");
            _tabReasons = new TabPage("Motivos Requisicao");
            _tabUnlock  = new TabPage("Desbloqueio de Registros");

            BuildGeneralTab(_tabGeneral);
            BuildLocksTab(_tabLocks);
            BuildAccessTab(_tabAccess);
            BuildShiftsTab(_tabShifts);
            BuildReasonsTab(_tabReasons);
            BuildUnlockTab(_tabUnlock);

            _tabControl.TabPages.Add(_tabGeneral);
            _tabControl.TabPages.Add(_tabLocks);
            _tabControl.TabPages.Add(_tabAccess);
            _tabControl.TabPages.Add(_tabShifts);
            _tabControl.TabPages.Add(_tabReasons);
            _tabControl.TabPages.Add(_tabUnlock);
            _tabControl.SelectedIndexChanged += OnTabChanged;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1, ColumnCount = 1, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.Controls.Add(_tabControl, 0, 0);
            Controls.Add(root);
        }

        private void OnInitialLoad()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                ReloadSystemParameters();
                LoadShiftsGrid();
                LoadReasonsGrid();
                LoadAccessUsers();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void OnTabChanged(object sender, EventArgs e)
        {
            // Python "_on_tab_changed": quando a aba Desbloqueio fica ativa,
            // limpa filtros e carrega TODOS os bloqueados automaticamente.
            if (ReferenceEquals(_tabControl.SelectedTab, _tabUnlock))
            {
                try
                {
                    _unlockNumberTextBox.Text   = string.Empty;
                    _unlockSupplierTextBox.Text = string.Empty;
                    _unlockTableCombo.SelectedItem = "transferencias";
                    LoadLockedRecords(false);
                }
                catch (Exception exception)
                {
                    ShowError(exception);
                }
            }
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize  = true,
                Text      = text,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin    = new Padding(0, 8, 6, 4),
            };
        }

        private static Label CreateHelpLabel(string text)
        {
            return new Label
            {
                AutoSize  = true,
                Text      = text,
                Font      = new Font("Segoe UI", 8F),
                ForeColor = Color.Gray,
                Margin    = new Padding(0, 8, 14, 4),
            };
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(4) };
            button.Click += handler;
            return button;
        }

        private void ShowError(Exception exception)
        {
            MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
