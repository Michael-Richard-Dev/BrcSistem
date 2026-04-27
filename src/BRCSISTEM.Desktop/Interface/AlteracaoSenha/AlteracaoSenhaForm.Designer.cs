namespace BRCSISTEM.Desktop.Interface.AlteracaoSenha
{
    partial class AlteracaoSenhaForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel _layout;
        private System.Windows.Forms.Label _instructionLabel;
        private System.Windows.Forms.Label _userNameLabel;
        private System.Windows.Forms.Label _userNameValueLabel;
        private System.Windows.Forms.Label _newPasswordLabel;
        private System.Windows.Forms.TextBox _newPasswordTextBox;
        private System.Windows.Forms.Label _confirmPasswordLabel;
        private System.Windows.Forms.TextBox _confirmPasswordTextBox;
        private System.Windows.Forms.Label _statusLabel;
        private System.Windows.Forms.FlowLayoutPanel _buttonsPanel;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlteracaoSenhaForm));
            this._layout = new System.Windows.Forms.TableLayoutPanel();
            this._instructionLabel = new System.Windows.Forms.Label();
            this._userNameLabel = new System.Windows.Forms.Label();
            this._userNameValueLabel = new System.Windows.Forms.Label();
            this._newPasswordLabel = new System.Windows.Forms.Label();
            this._newPasswordTextBox = new System.Windows.Forms.TextBox();
            this._confirmPasswordLabel = new System.Windows.Forms.Label();
            this._confirmPasswordTextBox = new System.Windows.Forms.TextBox();
            this._statusLabel = new System.Windows.Forms.Label();
            this._buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._saveButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._layout.SuspendLayout();
            this._buttonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _layout
            // 
            this._layout.ColumnCount = 2;
            this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._layout.Controls.Add(this._instructionLabel, 0, 0);
            this._layout.Controls.Add(this._userNameLabel, 0, 1);
            this._layout.Controls.Add(this._userNameValueLabel, 1, 1);
            this._layout.Controls.Add(this._newPasswordLabel, 0, 2);
            this._layout.Controls.Add(this._newPasswordTextBox, 1, 2);
            this._layout.Controls.Add(this._confirmPasswordLabel, 0, 3);
            this._layout.Controls.Add(this._confirmPasswordTextBox, 1, 3);
            this._layout.Controls.Add(this._statusLabel, 0, 4);
            this._layout.Controls.Add(this._buttonsPanel, 0, 5);
            this._layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._layout.Location = new System.Drawing.Point(0, 0);
            this._layout.Name = "_layout";
            this._layout.Padding = new System.Windows.Forms.Padding(20);
            this._layout.RowCount = 6;
            this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._layout.Size = new System.Drawing.Size(504, 281);
            this._layout.TabIndex = 0;
            // 
            // _instructionLabel
            // 
            this._instructionLabel.AutoSize = true;
            this._layout.SetColumnSpan(this._instructionLabel, 2);
            this._instructionLabel.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this._instructionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._instructionLabel.Location = new System.Drawing.Point(20, 20);
            this._instructionLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this._instructionLabel.Name = "_instructionLabel";
            this._instructionLabel.Size = new System.Drawing.Size(220, 19);
            this._instructionLabel.TabIndex = 0;
            this._instructionLabel.Text = "Altere a senha do usuario atual.";
            // 
            // _userNameLabel
            // 
            this._userNameLabel.AutoSize = true;
            this._userNameLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._userNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._userNameLabel.Location = new System.Drawing.Point(20, 63);
            this._userNameLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this._userNameLabel.Name = "_userNameLabel";
            this._userNameLabel.Size = new System.Drawing.Size(47, 13);
            this._userNameLabel.TabIndex = 1;
            this._userNameLabel.Text = "Usuario";
            // 
            // _userNameValueLabel
            // 
            this._userNameValueLabel.AutoSize = true;
            this._userNameValueLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._userNameValueLabel.Location = new System.Drawing.Point(170, 63);
            this._userNameValueLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this._userNameValueLabel.Name = "_userNameValueLabel";
            this._userNameValueLabel.Size = new System.Drawing.Size(0, 13);
            this._userNameValueLabel.TabIndex = 2;
            // 
            // _newPasswordLabel
            // 
            this._newPasswordLabel.AutoSize = true;
            this._newPasswordLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._newPasswordLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._newPasswordLabel.Location = new System.Drawing.Point(20, 92);
            this._newPasswordLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this._newPasswordLabel.Name = "_newPasswordLabel";
            this._newPasswordLabel.Size = new System.Drawing.Size(67, 13);
            this._newPasswordLabel.TabIndex = 3;
            this._newPasswordLabel.Text = "Nova senha";
            // 
            // _newPasswordTextBox
            // 
            this._newPasswordTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._newPasswordTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._newPasswordTextBox.Location = new System.Drawing.Point(173, 87);
            this._newPasswordTextBox.Name = "_newPasswordTextBox";
            this._newPasswordTextBox.Size = new System.Drawing.Size(308, 22);
            this._newPasswordTextBox.TabIndex = 4;
            this._newPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // _confirmPasswordLabel
            // 
            this._confirmPasswordLabel.AutoSize = true;
            this._confirmPasswordLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._confirmPasswordLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._confirmPasswordLabel.Location = new System.Drawing.Point(20, 120);
            this._confirmPasswordLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this._confirmPasswordLabel.Name = "_confirmPasswordLabel";
            this._confirmPasswordLabel.Size = new System.Drawing.Size(72, 13);
            this._confirmPasswordLabel.TabIndex = 5;
            this._confirmPasswordLabel.Text = "Confirmacao";
            // 
            // _confirmPasswordTextBox
            // 
            this._confirmPasswordTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._confirmPasswordTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._confirmPasswordTextBox.Location = new System.Drawing.Point(173, 115);
            this._confirmPasswordTextBox.Name = "_confirmPasswordTextBox";
            this._confirmPasswordTextBox.Size = new System.Drawing.Size(308, 22);
            this._confirmPasswordTextBox.TabIndex = 6;
            this._confirmPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._layout.SetColumnSpan(this._statusLabel, 2);
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this._statusLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._statusLabel.Location = new System.Drawing.Point(20, 156);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 16, 0, 12);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(0, 13);
            this._statusLabel.TabIndex = 7;
            // 
            // _buttonsPanel
            // 
            this._buttonsPanel.AutoSize = true;
            this._layout.SetColumnSpan(this._buttonsPanel, 2);
            this._buttonsPanel.Controls.Add(this._saveButton);
            this._buttonsPanel.Controls.Add(this._cancelButton);
            this._buttonsPanel.Location = new System.Drawing.Point(23, 184);
            this._buttonsPanel.Name = "_buttonsPanel";
            this._buttonsPanel.Size = new System.Drawing.Size(135, 28);
            this._buttonsPanel.TabIndex = 8;
            // 
            // _saveButton
            // 
            this._saveButton.AutoSize = true;
            this._saveButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._saveButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._saveButton.Location = new System.Drawing.Point(3, 3);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(53, 22);
            this._saveButton.TabIndex = 0;
            this._saveButton.Text = "Salvar";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this.OnSaveButtonClick);
            // 
            // _cancelButton
            // 
            this._cancelButton.AutoSize = true;
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._cancelButton.Location = new System.Drawing.Point(62, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(70, 22);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Fechar";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.OnCancelButtonClick);
            // 
            // AlteracaoSenhaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 281);
            this.Controls.Add(this._layout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 320);
            this.Name = "AlteracaoSenhaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Alterar Senha";
            this._layout.ResumeLayout(false);
            this._layout.PerformLayout();
            this._buttonsPanel.ResumeLayout(false);
            this._buttonsPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
