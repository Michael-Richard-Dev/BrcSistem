using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    partial class MainForm
    {
        /// <summary>
        /// Variavel de designer necessaria.
        /// </summary>
        private IContainer components = null;

        // ── Controles visuais persistentes (equivalente ao Designer do VS) ───
        private MenuStrip mainMenuStrip;
        private TableLayoutPanel tableLayoutBody;
        private Panel panelCentral;
        private TableLayoutPanel tableLayoutSidebar;
        private Panel panelSidebarContent;
        private FlowLayoutPanel _sidebarContentFlow;
        private Panel panelSidebarFooter;
        private Button buttonRefreshSidebar;
        private TableLayoutPanel tableLayoutFooter;
        private Label labelFooterSystem;
        private Label labelFooterUser;
        private Label _footerDateLabel;
        private Timer _footerDateTimer;

        /// <summary>
        /// Limpar os recursos utilizados.
        /// </summary>
        /// <param name="disposing">true se for preciso liberar recursos gerenciados; caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_footerDateTimer != null)
                {
                    _footerDateTimer.Stop();
                    _footerDateTimer.Dispose();
                    _footerDateTimer = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Metodo necessario para suporte do Designer - nao modificar
        /// o conteudo deste metodo com o editor de codigo.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            this.mainMenuStrip = new MenuStrip();
            this.tableLayoutBody = new TableLayoutPanel();
            this.panelCentral = new Panel();
            this.tableLayoutSidebar = new TableLayoutPanel();
            this.panelSidebarContent = new Panel();
            this._sidebarContentFlow = new FlowLayoutPanel();
            this.panelSidebarFooter = new Panel();
            this.buttonRefreshSidebar = new Button();
            this.tableLayoutFooter = new TableLayoutPanel();
            this.labelFooterSystem = new Label();
            this.labelFooterUser = new Label();
            this._footerDateLabel = new Label();
            this._footerDateTimer = new Timer(this.components);

            this.tableLayoutBody.SuspendLayout();
            this.tableLayoutSidebar.SuspendLayout();
            this.panelSidebarContent.SuspendLayout();
            this.panelSidebarFooter.SuspendLayout();
            this.tableLayoutFooter.SuspendLayout();
            this.SuspendLayout();

            //
            // mainMenuStrip
            //
            this.mainMenuStrip.Font = new Font("Segoe UI", 10F);
            this.mainMenuStrip.Name = "mainMenuStrip";
            //
            // tableLayoutBody (area central + sidebar direita)
            //
            this.tableLayoutBody.Dock = DockStyle.Fill;
            this.tableLayoutBody.ColumnCount = 2;
            this.tableLayoutBody.RowCount = 1;
            this.tableLayoutBody.BackColor = Color.White;
            this.tableLayoutBody.Margin = new Padding(0);
            this.tableLayoutBody.Padding = new Padding(0);
            this.tableLayoutBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutBody.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310F));
            this.tableLayoutBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutBody.Name = "tableLayoutBody";
            //
            // panelCentral
            //
            this.panelCentral.Dock = DockStyle.Fill;
            this.panelCentral.BackColor = Color.White;
            this.panelCentral.Name = "panelCentral";
            this.tableLayoutBody.Controls.Add(this.panelCentral, 0, 0);
            //
            // tableLayoutSidebar
            //
            this.tableLayoutSidebar.Dock = DockStyle.Fill;
            this.tableLayoutSidebar.ColumnCount = 1;
            this.tableLayoutSidebar.RowCount = 2;
            this.tableLayoutSidebar.BackColor = ColorSidebarBg;
            this.tableLayoutSidebar.Margin = new Padding(0);
            this.tableLayoutSidebar.Padding = new Padding(0);
            this.tableLayoutSidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.tableLayoutSidebar.Name = "tableLayoutSidebar";
            this.tableLayoutBody.Controls.Add(this.tableLayoutSidebar, 1, 0);
            //
            // panelSidebarContent
            //
            this.panelSidebarContent.Dock = DockStyle.Fill;
            this.panelSidebarContent.BackColor = ColorSidebarBg;
            this.panelSidebarContent.AutoScroll = true;
            this.panelSidebarContent.Padding = new Padding(0, 4, 0, 4);
            this.panelSidebarContent.Name = "panelSidebarContent";
            this.tableLayoutSidebar.Controls.Add(this.panelSidebarContent, 0, 0);
            //
            // _sidebarContentFlow
            //
            this._sidebarContentFlow.Dock = DockStyle.Top;
            this._sidebarContentFlow.AutoSize = true;
            this._sidebarContentFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._sidebarContentFlow.FlowDirection = FlowDirection.TopDown;
            this._sidebarContentFlow.WrapContents = false;
            this._sidebarContentFlow.BackColor = ColorSidebarBg;
            this._sidebarContentFlow.Margin = new Padding(0);
            this._sidebarContentFlow.Padding = new Padding(0);
            this._sidebarContentFlow.Name = "_sidebarContentFlow";
            this.panelSidebarContent.Controls.Add(this._sidebarContentFlow);
            //
            // panelSidebarFooter
            //
            this.panelSidebarFooter.Dock = DockStyle.Fill;
            this.panelSidebarFooter.Height = 30;
            this.panelSidebarFooter.BackColor = ColorSidebarBg;
            this.panelSidebarFooter.Padding = new Padding(0, 4, 0, 6);
            this.panelSidebarFooter.Name = "panelSidebarFooter";
            this.tableLayoutSidebar.Controls.Add(this.panelSidebarFooter, 0, 1);
            //
            // buttonRefreshSidebar
            //
            this.buttonRefreshSidebar.Text = "\u27f3 Atualizar";
            this.buttonRefreshSidebar.Font = new Font("Segoe UI", 9F);
            this.buttonRefreshSidebar.FlatStyle = FlatStyle.Flat;
            this.buttonRefreshSidebar.BackColor = ColorSidebarBg;
            this.buttonRefreshSidebar.ForeColor = ColorTextDark;
            this.buttonRefreshSidebar.AutoSize = true;
            this.buttonRefreshSidebar.Cursor = Cursors.Hand;
            this.buttonRefreshSidebar.Anchor = AnchorStyles.Top;
            this.buttonRefreshSidebar.FlatAppearance.BorderSize = 0;
            this.buttonRefreshSidebar.Name = "buttonRefreshSidebar";
            this.panelSidebarFooter.Controls.Add(this.buttonRefreshSidebar);
            //
            // tableLayoutFooter (rodape azul)
            //
            this.tableLayoutFooter.Dock = DockStyle.Bottom;
            this.tableLayoutFooter.Height = 28;
            this.tableLayoutFooter.ColumnCount = 3;
            this.tableLayoutFooter.RowCount = 1;
            this.tableLayoutFooter.BackColor = ColorFooterBg;
            this.tableLayoutFooter.Margin = new Padding(0);
            this.tableLayoutFooter.Padding = new Padding(10, 0, 10, 0);
            this.tableLayoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.tableLayoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.tableLayoutFooter.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutFooter.Name = "tableLayoutFooter";
            //
            // labelFooterSystem
            //
            this.labelFooterSystem.Text = AppName + " - " + AppVersion;
            this.labelFooterSystem.AutoSize = true;
            this.labelFooterSystem.Font = new Font("Segoe UI", 9F);
            this.labelFooterSystem.ForeColor = Color.White;
            this.labelFooterSystem.BackColor = ColorFooterBg;
            this.labelFooterSystem.TextAlign = ContentAlignment.MiddleLeft;
            this.labelFooterSystem.Anchor = AnchorStyles.Left;
            this.labelFooterSystem.Margin = new Padding(0);
            this.labelFooterSystem.Name = "labelFooterSystem";
            this.tableLayoutFooter.Controls.Add(this.labelFooterSystem, 0, 0);
            //
            // labelFooterUser (Text definido em runtime no construtor a partir de _identity)
            //
            this.labelFooterUser.AutoSize = false;
            this.labelFooterUser.Dock = DockStyle.Fill;
            this.labelFooterUser.Font = new Font("Segoe UI", 9F);
            this.labelFooterUser.ForeColor = Color.White;
            this.labelFooterUser.BackColor = ColorFooterBg;
            this.labelFooterUser.TextAlign = ContentAlignment.MiddleCenter;
            this.labelFooterUser.Name = "labelFooterUser";
            this.tableLayoutFooter.Controls.Add(this.labelFooterUser, 1, 0);
            //
            // _footerDateLabel
            //
            this._footerDateLabel.Text = System.DateTime.Now.ToString("dd/MM/yyyy");
            this._footerDateLabel.AutoSize = true;
            this._footerDateLabel.Font = new Font("Segoe UI", 9F);
            this._footerDateLabel.ForeColor = Color.White;
            this._footerDateLabel.BackColor = ColorFooterBg;
            this._footerDateLabel.TextAlign = ContentAlignment.MiddleRight;
            this._footerDateLabel.Anchor = AnchorStyles.Right;
            this._footerDateLabel.Name = "_footerDateLabel";
            this.tableLayoutFooter.Controls.Add(this._footerDateLabel, 2, 0);
            //
            // _footerDateTimer
            //
            this._footerDateTimer.Interval = 60 * 1000;
            //
            // MainForm
            //
            this.BackColor = Color.White;
            this.MinimumSize = new Size(1000, 600);
            this.WindowState = FormWindowState.Maximized;
            this.Text = AppName + " " + AppVersion + " - Principal";
            this.Name = "MainForm";
            this.Controls.Add(this.tableLayoutBody);
            this.Controls.Add(this.tableLayoutFooter);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;

            this.panelSidebarFooter.ResumeLayout(false);
            this.panelSidebarContent.ResumeLayout(false);
            this.panelSidebarContent.PerformLayout();
            this.tableLayoutSidebar.ResumeLayout(false);
            this.tableLayoutBody.ResumeLayout(false);
            this.tableLayoutFooter.ResumeLayout(false);
            this.tableLayoutFooter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
