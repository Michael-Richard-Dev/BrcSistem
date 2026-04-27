using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface
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
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._manualTextBox = new System.Windows.Forms.RichTextBox();
            this._buttonsLayout = new System.Windows.Forms.TableLayoutPanel();
            this._closeButton = new System.Windows.Forms.Button();
            this._rootLayout.SuspendLayout();
            this._buttonsLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._titleLabel, 0, 0);
            this._rootLayout.Controls.Add(this._manualTextBox, 0, 1);
            this._rootLayout.Controls.Add(this._buttonsLayout, 0, 2);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(20);
            this._rootLayout.RowCount = 3;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.Size = new System.Drawing.Size(900, 700);
            this._rootLayout.TabIndex = 0;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Location = new System.Drawing.Point(20, 20);
            this._titleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(198, 21);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Manual de Configuracao";
            // 
            // _manualTextBox
            // 
            this._manualTextBox.BackColor = System.Drawing.Color.White;
            this._manualTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._manualTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._manualTextBox.Font = new System.Drawing.Font("Consolas", 10F);
            this._manualTextBox.Location = new System.Drawing.Point(23, 54);
            this._manualTextBox.Name = "_manualTextBox";
            this._manualTextBox.ReadOnly = true;
            this._manualTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this._manualTextBox.Size = new System.Drawing.Size(854, 513);
            this._manualTextBox.TabIndex = 1;
            this._manualTextBox.Text = "";
            // 
            // _buttonsLayout
            // 
            this._buttonsLayout.ColumnCount = 2;
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.Controls.Add(this._closeButton, 1, 0);
            this._buttonsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._buttonsLayout.Location = new System.Drawing.Point(20, 580);
            this._buttonsLayout.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._buttonsLayout.Name = "_buttonsLayout";
            this._buttonsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._buttonsLayout.Size = new System.Drawing.Size(860, 100);
            this._buttonsLayout.TabIndex = 2;
            // 
            // _closeButton
            // 
            this._closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._closeButton.Location = new System.Drawing.Point(757, 3);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(100, 28);
            this._closeButton.TabIndex = 0;
            this._closeButton.Text = "Fechar";
            this._closeButton.UseVisualStyleBackColor = true;
            // 
            // DatabaseManualForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(900, 700);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "DatabaseManualForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
