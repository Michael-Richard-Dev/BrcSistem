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

        // Controles visuais persistentes (equivalente ao Designer do VS).
        private MenuStrip mainMenuStrip;
        private TableLayoutPanel tableLayoutBody;
        private Panel panelCentral;
        private TableLayoutPanel tableLayoutSidebar;
        private Panel panelSidebarContent;
        private FlowLayoutPanel _sidebarContentFlow;
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
            this.buttonRefreshSidebar = new Button();
            this.tableLayoutFooter = new TableLayoutPanel();
            this.labelFooterSystem = new Label();
            this.labelFooterUser = new Label();
            this._footerDateLabel = new Label();
            this._footerDateTimer = new Timer(this.components);

            this.tableLayoutBody.SuspendLayout();
            this.tableLayoutSidebar.SuspendLayout();
            this.panelSidebarContent.SuspendLayout();
            this.tableLayoutFooter.SuspendLayout();
            this.SuspendLayout();
            //
            // mainMenuStrip
            //
            this.mainMenuStrip.Dock = DockStyle.Top;
            this.mainMenuStrip.Font = new Font("Segoe UI", 10F);
            this.mainMenuStrip.Location = new Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new Size(1280, 27);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "mainMenuStrip";
            //
            // tableLayoutBody
            //
            this.tableLayoutBody.BackColor = Color.White;
            this.tableLayoutBody.ColumnCount = 2;
            this.tableLayoutBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutBody.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 310F));
            this.tableLayoutBody.Controls.Add(this.panelCentral, 0, 0);
            this.tableLayoutBody.Controls.Add(this.tableLayoutSidebar, 1, 0);
            this.tableLayoutBody.Dock = DockStyle.Fill;
            this.tableLayoutBody.Location = new Point(0, 27);
            this.tableLayoutBody.Margin = new Padding(0);
            this.tableLayoutBody.Name = "tableLayoutBody";
            this.tableLayoutBody.Padding = new Padding(0);
            this.tableLayoutBody.RowCount = 1;
            this.tableLayoutBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutBody.Size = new Size(1280, 665);
            this.tableLayoutBody.TabIndex = 1;
            //
            // panelCentral
            //
            this.panelCentral.BackColor = Color.White;
            this.panelCentral.Dock = DockStyle.Fill;
            this.panelCentral.Location = new Point(0, 0);
            this.panelCentral.Margin = new Padding(0);
            this.panelCentral.Name = "panelCentral";
            this.panelCentral.Size = new Size(970, 665);
            this.panelCentral.TabIndex = 0;
            //
            // tableLayoutSidebar
            //
            this.tableLayoutSidebar.BackColor = Color.FromArgb(248, 249, 250);
            this.tableLayoutSidebar.ColumnCount = 1;
            this.tableLayoutSidebar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutSidebar.Controls.Add(this.panelSidebarContent, 0, 0);
            this.tableLayoutSidebar.Controls.Add(this.buttonRefreshSidebar, 0, 1);
            this.tableLayoutSidebar.Dock = DockStyle.Fill;
            this.tableLayoutSidebar.Location = new Point(970, 0);
            this.tableLayoutSidebar.Margin = new Padding(0);
            this.tableLayoutSidebar.Name = "tableLayoutSidebar";
            this.tableLayoutSidebar.Padding = new Padding(0);
            this.tableLayoutSidebar.RowCount = 2;
            this.tableLayoutSidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutSidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.tableLayoutSidebar.Size = new Size(310, 665);
            this.tableLayoutSidebar.TabIndex = 1;
            //
            // panelSidebarContent
            //
            this.panelSidebarContent.AutoScroll = true;
            this.panelSidebarContent.BackColor = Color.FromArgb(248, 249, 250);
            this.panelSidebarContent.Controls.Add(this._sidebarContentFlow);
            this.panelSidebarContent.Dock = DockStyle.Fill;
            this.panelSidebarContent.Location = new Point(0, 0);
            this.panelSidebarContent.Margin = new Padding(0);
            this.panelSidebarContent.Name = "panelSidebarContent";
            this.panelSidebarContent.Padding = new Padding(0, 4, 0, 4);
            this.panelSidebarContent.Size = new Size(310, 625);
            this.panelSidebarContent.TabIndex = 0;
            //
            // _sidebarContentFlow
            //
            this._sidebarContentFlow.AutoSize = true;
            this._sidebarContentFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._sidebarContentFlow.BackColor = Color.FromArgb(248, 249, 250);
            this._sidebarContentFlow.Dock = DockStyle.Top;
            this._sidebarContentFlow.FlowDirection = FlowDirection.TopDown;
            this._sidebarContentFlow.Location = new Point(0, 4);
            this._sidebarContentFlow.Margin = new Padding(0);
            this._sidebarContentFlow.Name = "_sidebarContentFlow";
            this._sidebarContentFlow.Padding = new Padding(0);
            this._sidebarContentFlow.Size = new Size(310, 0);
            this._sidebarContentFlow.TabIndex = 0;
            this._sidebarContentFlow.WrapContents = false;
            //
            // buttonRefreshSidebar  (direto no tableLayoutSidebar, linha 1 AutoSize, centralizado por Anchor=None)
            //
            this.buttonRefreshSidebar.Anchor = AnchorStyles.None;
            this.buttonRefreshSidebar.AutoSize = true;
            this.buttonRefreshSidebar.BackColor = Color.FromArgb(248, 249, 250);
            this.buttonRefreshSidebar.Cursor = Cursors.Hand;
            this.buttonRefreshSidebar.FlatAppearance.BorderSize = 0;
            this.buttonRefreshSidebar.FlatStyle = FlatStyle.Flat;
            this.buttonRefreshSidebar.Font = new Font("Segoe UI", 9F);
            this.buttonRefreshSidebar.ForeColor = Color.FromArgb(44, 62, 80);
            this.buttonRefreshSidebar.Margin = new Padding(0, 4, 0, 6);
            this.buttonRefreshSidebar.Name = "buttonRefreshSidebar";
            this.buttonRefreshSidebar.TabIndex = 0;
            this.buttonRefreshSidebar.Text = "⟳ Atualizar";
            this.buttonRefreshSidebar.UseVisualStyleBackColor = false;
            //
            // tableLayoutFooter
            //
            this.tableLayoutFooter.BackColor = Color.FromArgb(0, 122, 204);
            this.tableLayoutFooter.ColumnCount = 3;
            this.tableLayoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.tableLayoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.tableLayoutFooter.Controls.Add(this.labelFooterSystem, 0, 0);
            this.tableLayoutFooter.Controls.Add(this.labelFooterUser, 1, 0);
            this.tableLayoutFooter.Controls.Add(this._footerDateLabel, 2, 0);
            this.tableLayoutFooter.Dock = DockStyle.Bottom;
            this.tableLayoutFooter.Location = new Point(0, 692);
            this.tableLayoutFooter.Margin = new Padding(0);
            this.tableLayoutFooter.Name = "tableLayoutFooter";
            this.tableLayoutFooter.Padding = new Padding(10, 0, 10, 0);
            this.tableLayoutFooter.RowCount = 1;
            this.tableLayoutFooter.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutFooter.Size = new Size(1280, 28);
            this.tableLayoutFooter.TabIndex = 2;
            //
            // labelFooterSystem
            //
            this.labelFooterSystem.Anchor = AnchorStyles.Left;
            this.labelFooterSystem.AutoSize = true;
            this.labelFooterSystem.BackColor = Color.FromArgb(0, 122, 204);
            this.labelFooterSystem.Font = new Font("Segoe UI", 9F);
            this.labelFooterSystem.ForeColor = Color.White;
            this.labelFooterSystem.Location = new Point(10, 6);
            this.labelFooterSystem.Margin = new Padding(0);
            this.labelFooterSystem.Name = "labelFooterSystem";
            this.labelFooterSystem.Size = new Size(128, 15);
            this.labelFooterSystem.TabIndex = 0;
            this.labelFooterSystem.Text = "BRCSISTEM - v3.1.20";
            this.labelFooterSystem.TextAlign = ContentAlignment.MiddleLeft;
            //
            // labelFooterUser  (texto real e completo definido em runtime no construtor)
            //
            this.labelFooterUser.BackColor = Color.FromArgb(0, 122, 204);
            this.labelFooterUser.Dock = DockStyle.Fill;
            this.labelFooterUser.Font = new Font("Segoe UI", 9F);
            this.labelFooterUser.ForeColor = Color.White;
            this.labelFooterUser.Location = new Point(138, 0);
            this.labelFooterUser.Margin = new Padding(0);
            this.labelFooterUser.Name = "labelFooterUser";
            this.labelFooterUser.Size = new Size(1016, 28);
            this.labelFooterUser.TabIndex = 1;
            this.labelFooterUser.Text = "Usuario:";
            this.labelFooterUser.TextAlign = ContentAlignment.MiddleCenter;
            //
            // _footerDateLabel
            //
            this._footerDateLabel.Anchor = AnchorStyles.Right;
            this._footerDateLabel.AutoSize = true;
            this._footerDateLabel.BackColor = Color.FromArgb(0, 122, 204);
            this._footerDateLabel.Font = new Font("Segoe UI", 9F);
            this._footerDateLabel.ForeColor = Color.White;
            this._footerDateLabel.Location = new Point(1154, 6);
            this._footerDateLabel.Margin = new Padding(0);
            this._footerDateLabel.Name = "_footerDateLabel";
            this._footerDateLabel.Size = new Size(116, 15);
            this._footerDateLabel.TabIndex = 2;
            this._footerDateLabel.Text = "dd/MM/yyyy";
            this._footerDateLabel.TextAlign = ContentAlignment.MiddleRight;
            //
            // _footerDateTimer
            //
            this._footerDateTimer.Interval = 60000;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1280, 720);
            this.Controls.Add(this.tableLayoutBody);
            this.Controls.Add(this.tableLayoutFooter);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.MinimumSize = new Size(1000, 600);
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "BRCSISTEM v3.1.20 - Principal";
            this.WindowState = FormWindowState.Maximized;

            this.tableLayoutBody.ResumeLayout(false);
            this.tableLayoutSidebar.ResumeLayout(false);
            this.tableLayoutSidebar.PerformLayout();
            this.panelSidebarContent.ResumeLayout(false);
            this.panelSidebarContent.PerformLayout();
            this.tableLayoutFooter.ResumeLayout(false);
            this.tableLayoutFooter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
