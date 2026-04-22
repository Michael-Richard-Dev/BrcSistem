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
        private TableLayoutPanel rootLayout;
        private MenuStrip mainMenuStrip;
        private TableLayoutPanel tableLayoutBody;
        private Panel panelCentral;
        private TableLayoutPanel tableLayoutSidebar;
        private TableLayoutPanel _sidebarContentFlow;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.tableLayoutBody = new System.Windows.Forms.TableLayoutPanel();
            this.panelCentral = new System.Windows.Forms.Panel();
            this.tableLayoutSidebar = new System.Windows.Forms.TableLayoutPanel();
            this._sidebarContentFlow = new System.Windows.Forms.TableLayoutPanel();
            this.panelSidebarFooter = new System.Windows.Forms.Panel();
            this.buttonRefreshSidebar = new System.Windows.Forms.Button();
            this.tableLayoutFooter = new System.Windows.Forms.TableLayoutPanel();
            this.labelFooterSystem = new System.Windows.Forms.Label();
            this.labelFooterUser = new System.Windows.Forms.Label();
            this._footerDateLabel = new System.Windows.Forms.Label();
            this._footerDateTimer = new System.Windows.Forms.Timer(this.components);
            this.rootLayout.SuspendLayout();
            this.tableLayoutBody.SuspendLayout();
            this.tableLayoutSidebar.SuspendLayout();
            this.panelSidebarFooter.SuspendLayout();
            this.tableLayoutFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.BackColor = System.Drawing.Color.White;
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.mainMenuStrip, 0, 0);
            this.rootLayout.Controls.Add(this.tableLayoutBody, 0, 1);
            this.rootLayout.Controls.Add(this.tableLayoutFooter, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootLayout.Size = new System.Drawing.Size(1097, 624);
            this.rootLayout.TabIndex = 0;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainMenuStrip.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.mainMenuStrip.Size = new System.Drawing.Size(1097, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "mainMenuStrip";
            // 
            // tableLayoutBody
            // 
            this.tableLayoutBody.BackColor = System.Drawing.Color.White;
            this.tableLayoutBody.ColumnCount = 2;
            this.tableLayoutBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 266F));
            this.tableLayoutBody.Controls.Add(this.panelCentral, 0, 0);
            this.tableLayoutBody.Controls.Add(this.tableLayoutSidebar, 1, 0);
            this.tableLayoutBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutBody.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutBody.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutBody.Name = "tableLayoutBody";
            this.tableLayoutBody.RowCount = 1;
            this.tableLayoutBody.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutBody.Size = new System.Drawing.Size(1097, 576);
            this.tableLayoutBody.TabIndex = 1;
            // 
            // panelCentral
            // 
            this.panelCentral.BackColor = System.Drawing.Color.White;
            this.panelCentral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCentral.Location = new System.Drawing.Point(0, 0);
            this.panelCentral.Margin = new System.Windows.Forms.Padding(0);
            this.panelCentral.Name = "panelCentral";
            this.panelCentral.Size = new System.Drawing.Size(831, 576);
            this.panelCentral.TabIndex = 0;
            // 
            // tableLayoutSidebar
            // 
            this.tableLayoutSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.tableLayoutSidebar.ColumnCount = 1;
            this.tableLayoutSidebar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutSidebar.Controls.Add(this._sidebarContentFlow, 0, 0);
            this.tableLayoutSidebar.Controls.Add(this.panelSidebarFooter, 0, 1);
            this.tableLayoutSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutSidebar.Location = new System.Drawing.Point(831, 0);
            this.tableLayoutSidebar.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutSidebar.Name = "tableLayoutSidebar";
            this.tableLayoutSidebar.RowCount = 2;
            this.tableLayoutSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutSidebar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutSidebar.Size = new System.Drawing.Size(266, 576);
            this.tableLayoutSidebar.TabIndex = 1;
            // 
            // _sidebarContentFlow
            // 
            this._sidebarContentFlow.AutoScroll = true;
            this._sidebarContentFlow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this._sidebarContentFlow.ColumnCount = 1;
            this._sidebarContentFlow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._sidebarContentFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._sidebarContentFlow.Location = new System.Drawing.Point(0, 0);
            this._sidebarContentFlow.Margin = new System.Windows.Forms.Padding(0);
            this._sidebarContentFlow.Name = "_sidebarContentFlow";
            this._sidebarContentFlow.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this._sidebarContentFlow.RowCount = 1;
            this._sidebarContentFlow.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._sidebarContentFlow.Size = new System.Drawing.Size(266, 541);
            this._sidebarContentFlow.TabIndex = 0;
            // 
            // panelSidebarFooter
            // 
            this.panelSidebarFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelSidebarFooter.Controls.Add(this.buttonRefreshSidebar);
            this.panelSidebarFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSidebarFooter.Location = new System.Drawing.Point(0, 541);
            this.panelSidebarFooter.Margin = new System.Windows.Forms.Padding(0);
            this.panelSidebarFooter.Name = "panelSidebarFooter";
            this.panelSidebarFooter.Size = new System.Drawing.Size(266, 35);
            this.panelSidebarFooter.TabIndex = 1;
            // 
            // buttonRefreshSidebar
            // 
            this.buttonRefreshSidebar.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonRefreshSidebar.AutoSize = true;
            this.buttonRefreshSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.buttonRefreshSidebar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRefreshSidebar.FlatAppearance.BorderSize = 0;
            this.buttonRefreshSidebar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRefreshSidebar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.buttonRefreshSidebar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.buttonRefreshSidebar.Location = new System.Drawing.Point(86, 6);
            this.buttonRefreshSidebar.Margin = new System.Windows.Forms.Padding(0);
            this.buttonRefreshSidebar.MinimumSize = new System.Drawing.Size(94, 23);
            this.buttonRefreshSidebar.Name = "buttonRefreshSidebar";
            this.buttonRefreshSidebar.Size = new System.Drawing.Size(94, 25);
            this.buttonRefreshSidebar.TabIndex = 0;
            this.buttonRefreshSidebar.Text = "⟳ Atualizar";
            this.buttonRefreshSidebar.UseVisualStyleBackColor = false;
            // 
            // tableLayoutFooter
            // 
            this.tableLayoutFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.tableLayoutFooter.ColumnCount = 3;
            this.tableLayoutFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutFooter.Controls.Add(this.labelFooterSystem, 0, 0);
            this.tableLayoutFooter.Controls.Add(this.labelFooterUser, 1, 0);
            this.tableLayoutFooter.Controls.Add(this._footerDateLabel, 2, 0);
            this.tableLayoutFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutFooter.Location = new System.Drawing.Point(0, 600);
            this.tableLayoutFooter.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutFooter.Name = "tableLayoutFooter";
            this.tableLayoutFooter.Padding = new System.Windows.Forms.Padding(9, 0, 9, 0);
            this.tableLayoutFooter.RowCount = 1;
            this.tableLayoutFooter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutFooter.Size = new System.Drawing.Size(1097, 24);
            this.tableLayoutFooter.TabIndex = 2;
            // 
            // labelFooterSystem
            // 
            this.labelFooterSystem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFooterSystem.AutoSize = true;
            this.labelFooterSystem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.labelFooterSystem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFooterSystem.ForeColor = System.Drawing.Color.White;
            this.labelFooterSystem.Location = new System.Drawing.Point(9, 4);
            this.labelFooterSystem.Margin = new System.Windows.Forms.Padding(0);
            this.labelFooterSystem.Name = "labelFooterSystem";
            this.labelFooterSystem.Size = new System.Drawing.Size(115, 15);
            this.labelFooterSystem.TabIndex = 0;
            this.labelFooterSystem.Text = "BRCSISTEM - v3.1.20";
            this.labelFooterSystem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelFooterUser
            // 
            this.labelFooterUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.labelFooterUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFooterUser.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFooterUser.ForeColor = System.Drawing.Color.White;
            this.labelFooterUser.Location = new System.Drawing.Point(124, 0);
            this.labelFooterUser.Margin = new System.Windows.Forms.Padding(0);
            this.labelFooterUser.Name = "labelFooterUser";
            this.labelFooterUser.Size = new System.Drawing.Size(887, 24);
            this.labelFooterUser.TabIndex = 1;
            this.labelFooterUser.Text = "Usuario:";
            this.labelFooterUser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _footerDateLabel
            // 
            this._footerDateLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._footerDateLabel.AutoSize = true;
            this._footerDateLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this._footerDateLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._footerDateLabel.ForeColor = System.Drawing.Color.White;
            this._footerDateLabel.Location = new System.Drawing.Point(1011, 4);
            this._footerDateLabel.Margin = new System.Windows.Forms.Padding(0);
            this._footerDateLabel.Name = "_footerDateLabel";
            this._footerDateLabel.Size = new System.Drawing.Size(77, 15);
            this._footerDateLabel.TabIndex = 2;
            this._footerDateLabel.Text = "dd/MM/yyyy";
            this._footerDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _footerDateTimer
            // 
            this._footerDateTimer.Interval = 60000;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1097, 624);
            this.Controls.Add(this.rootLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.MinimumSize = new System.Drawing.Size(859, 525);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BRCSISTEM v3.1.20 - Principal";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.tableLayoutBody.ResumeLayout(false);
            this.tableLayoutSidebar.ResumeLayout(false);
            this.panelSidebarFooter.ResumeLayout(false);
            this.panelSidebarFooter.PerformLayout();
            this.tableLayoutFooter.ResumeLayout(false);
            this.tableLayoutFooter.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
