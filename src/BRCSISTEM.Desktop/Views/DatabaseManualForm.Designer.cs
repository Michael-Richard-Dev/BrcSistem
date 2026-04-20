using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class DatabaseManualForm
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this._rootLayout = new TableLayoutPanel();
            this._titleLabel = new Label();
            this._manualTextBox = new RichTextBox();
            this._buttonsLayout = new TableLayoutPanel();
            this._closeButton = new Button();
            this._rootLayout.SuspendLayout();
            this._buttonsLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // _rootLayout
            //
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._titleLabel, 0, 0);
            this._rootLayout.Controls.Add(this._manualTextBox, 0, 1);
            this._rootLayout.Controls.Add(this._buttonsLayout, 0, 2);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Padding = new Padding(20);
            this._rootLayout.RowCount = 3;
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle());
            //
            // _titleLabel
            //
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this._titleLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._titleLabel.Margin = new Padding(0, 0, 0, 10);
            this._titleLabel.Text = "Manual de Configuracao";
            //
            // _manualTextBox
            //
            this._manualTextBox.BackColor = Color.White;
            this._manualTextBox.BorderStyle = BorderStyle.FixedSingle;
            this._manualTextBox.Dock = DockStyle.Fill;
            this._manualTextBox.Font = new Font("Consolas", 10F);
            this._manualTextBox.ReadOnly = true;
            this._manualTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            //
            // _buttonsLayout
            //
            this._buttonsLayout.ColumnCount = 2;
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.Controls.Add(this._closeButton, 1, 0);
            this._buttonsLayout.Dock = DockStyle.Fill;
            this._buttonsLayout.Margin = new Padding(0, 10, 0, 0);
            this._closeButton.FlatStyle = FlatStyle.System;
            this._closeButton.Size = new Size(100, 28);
            this._closeButton.Text = "Fechar";
            this._closeButton.UseVisualStyleBackColor = true;
            //
            // DatabaseManualForm
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.ClientSize = new Size(900, 700);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 9F);
            this.MinimumSize = new Size(700, 500);
            this.Name = "DatabaseManualForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Manual de Configuracao";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._buttonsLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private Label _titleLabel;
        private RichTextBox _manualTextBox;
        private TableLayoutPanel _buttonsLayout;
        private Button _closeButton;
    }
}
