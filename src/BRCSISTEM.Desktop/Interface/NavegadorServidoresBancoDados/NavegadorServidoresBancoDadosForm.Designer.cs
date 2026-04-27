using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.NavegadorServidoresBancoDados
{
    public sealed partial class NavegadorServidoresBancoDadosForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavegadorServidoresBancoDadosForm));
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this._connectionGroup = new System.Windows.Forms.GroupBox();
            this._connectionLayout = new System.Windows.Forms.TableLayoutPanel();
            this._hostLabel = new System.Windows.Forms.Label();
            this._hostTextBox = new System.Windows.Forms.TextBox();
            this._portLabel = new System.Windows.Forms.Label();
            this._portTextBox = new System.Windows.Forms.TextBox();
            this._userLabel = new System.Windows.Forms.Label();
            this._userTextBox = new System.Windows.Forms.TextBox();
            this._passwordLabel = new System.Windows.Forms.Label();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this._statusLabel = new System.Windows.Forms.Label();
            this._resultsGroup = new System.Windows.Forms.GroupBox();
            this._resultsLayout = new System.Windows.Forms.TableLayoutPanel();
            this._resultsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this._resultsHintLabel = new System.Windows.Forms.Label();
            this._buttonsLayout = new System.Windows.Forms.TableLayoutPanel();
            this._searchButton = new System.Windows.Forms.Button();
            this._addSelectedButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._rootLayout.SuspendLayout();
            this._connectionGroup.SuspendLayout();
            this._connectionLayout.SuspendLayout();
            this._resultsGroup.SuspendLayout();
            this._resultsLayout.SuspendLayout();
            this._buttonsLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._titleLabel, 0, 0);
            this._rootLayout.Controls.Add(this._subtitleLabel, 0, 1);
            this._rootLayout.Controls.Add(this._connectionGroup, 0, 2);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 3);
            this._rootLayout.Controls.Add(this._resultsGroup, 0, 4);
            this._rootLayout.Controls.Add(this._buttonsLayout, 0, 5);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(20);
            this._rootLayout.RowCount = 6;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._rootLayout.Size = new System.Drawing.Size(600, 500);
            this._rootLayout.TabIndex = 0;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(54)))), ((int)(((byte)(93)))));
            this._titleLabel.Location = new System.Drawing.Point(20, 20);
            this._titleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(211, 21);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Buscar Bancos no Servidor";
            // 
            // _subtitleLabel
            // 
            this._subtitleLabel.AutoSize = true;
            this._subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._subtitleLabel.ForeColor = System.Drawing.Color.Gray;
            this._subtitleLabel.Location = new System.Drawing.Point(20, 44);
            this._subtitleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this._subtitleLabel.Name = "_subtitleLabel";
            this._subtitleLabel.Size = new System.Drawing.Size(287, 13);
            this._subtitleLabel.TabIndex = 1;
            this._subtitleLabel.Text = "Conecte ao servidor e descubra os bancos disponiveis.";
            // 
            // _connectionGroup
            // 
            this._connectionGroup.Controls.Add(this._connectionLayout);
            this._connectionGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._connectionGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._connectionGroup.Location = new System.Drawing.Point(23, 75);
            this._connectionGroup.Name = "_connectionGroup";
            this._connectionGroup.Padding = new System.Windows.Forms.Padding(10);
            this._connectionGroup.Size = new System.Drawing.Size(554, 100);
            this._connectionGroup.TabIndex = 2;
            this._connectionGroup.TabStop = false;
            this._connectionGroup.Text = " Dados de Conexao ";
            // 
            // _connectionLayout
            // 
            this._connectionLayout.ColumnCount = 2;
            this._connectionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this._connectionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._connectionLayout.Controls.Add(this._hostLabel, 0, 0);
            this._connectionLayout.Controls.Add(this._hostTextBox, 1, 0);
            this._connectionLayout.Controls.Add(this._portLabel, 0, 1);
            this._connectionLayout.Controls.Add(this._portTextBox, 1, 1);
            this._connectionLayout.Controls.Add(this._userLabel, 0, 2);
            this._connectionLayout.Controls.Add(this._userTextBox, 1, 2);
            this._connectionLayout.Controls.Add(this._passwordLabel, 0, 3);
            this._connectionLayout.Controls.Add(this._passwordTextBox, 1, 3);
            this._connectionLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._connectionLayout.Location = new System.Drawing.Point(10, 26);
            this._connectionLayout.Name = "_connectionLayout";
            this._connectionLayout.RowCount = 4;
            this._connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._connectionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._connectionLayout.Size = new System.Drawing.Size(534, 64);
            this._connectionLayout.TabIndex = 0;
            // 
            // _hostLabel
            // 
            this._hostLabel.AutoSize = true;
            this._hostLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._hostLabel.Location = new System.Drawing.Point(0, 8);
            this._hostLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this._hostLabel.Name = "_hostLabel";
            this._hostLabel.Size = new System.Drawing.Size(34, 13);
            this._hostLabel.TabIndex = 0;
            this._hostLabel.Text = "Host:";
            // 
            // _hostTextBox
            // 
            this._hostTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._hostTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._hostTextBox.Location = new System.Drawing.Point(110, 5);
            this._hostTextBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this._hostTextBox.Name = "_hostTextBox";
            this._hostTextBox.Size = new System.Drawing.Size(424, 22);
            this._hostTextBox.TabIndex = 1;
            // 
            // _portLabel
            // 
            this._portLabel.AutoSize = true;
            this._portLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._portLabel.Location = new System.Drawing.Point(0, 40);
            this._portLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this._portLabel.Name = "_portLabel";
            this._portLabel.Size = new System.Drawing.Size(37, 13);
            this._portLabel.TabIndex = 2;
            this._portLabel.Text = "Porta:";
            // 
            // _portTextBox
            // 
            this._portTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._portTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._portTextBox.Location = new System.Drawing.Point(110, 37);
            this._portTextBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this._portTextBox.Name = "_portTextBox";
            this._portTextBox.Size = new System.Drawing.Size(424, 22);
            this._portTextBox.TabIndex = 3;
            // 
            // _userLabel
            // 
            this._userLabel.AutoSize = true;
            this._userLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._userLabel.Location = new System.Drawing.Point(0, 72);
            this._userLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this._userLabel.Name = "_userLabel";
            this._userLabel.Size = new System.Drawing.Size(50, 13);
            this._userLabel.TabIndex = 4;
            this._userLabel.Text = "Usuario:";
            // 
            // _userTextBox
            // 
            this._userTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._userTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._userTextBox.Location = new System.Drawing.Point(110, 69);
            this._userTextBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this._userTextBox.Name = "_userTextBox";
            this._userTextBox.Size = new System.Drawing.Size(424, 22);
            this._userTextBox.TabIndex = 5;
            // 
            // _passwordLabel
            // 
            this._passwordLabel.AutoSize = true;
            this._passwordLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._passwordLabel.Location = new System.Drawing.Point(0, 104);
            this._passwordLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this._passwordLabel.Name = "_passwordLabel";
            this._passwordLabel.Size = new System.Drawing.Size(42, 13);
            this._passwordLabel.TabIndex = 6;
            this._passwordLabel.Text = "Senha:";
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._passwordTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._passwordTextBox.Location = new System.Drawing.Point(110, 101);
            this._passwordTextBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new System.Drawing.Size(424, 22);
            this._passwordTextBox.TabIndex = 7;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusLabel.Location = new System.Drawing.Point(20, 188);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(0, 13);
            this._statusLabel.TabIndex = 3;
            // 
            // _resultsGroup
            // 
            this._resultsGroup.Controls.Add(this._resultsLayout);
            this._resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._resultsGroup.Location = new System.Drawing.Point(23, 214);
            this._resultsGroup.Name = "_resultsGroup";
            this._resultsGroup.Padding = new System.Windows.Forms.Padding(10);
            this._resultsGroup.Size = new System.Drawing.Size(554, 153);
            this._resultsGroup.TabIndex = 4;
            this._resultsGroup.TabStop = false;
            this._resultsGroup.Text = " Bancos Encontrados ";
            // 
            // _resultsLayout
            // 
            this._resultsLayout.ColumnCount = 1;
            this._resultsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._resultsLayout.Controls.Add(this._resultsCheckedListBox, 0, 0);
            this._resultsLayout.Controls.Add(this._resultsHintLabel, 0, 1);
            this._resultsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsLayout.Location = new System.Drawing.Point(10, 26);
            this._resultsLayout.Name = "_resultsLayout";
            this._resultsLayout.RowCount = 2;
            this._resultsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._resultsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._resultsLayout.Size = new System.Drawing.Size(534, 117);
            this._resultsLayout.TabIndex = 0;
            // 
            // _resultsCheckedListBox
            // 
            this._resultsCheckedListBox.CheckOnClick = true;
            this._resultsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsCheckedListBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._resultsCheckedListBox.IntegralHeight = false;
            this._resultsCheckedListBox.Location = new System.Drawing.Point(3, 3);
            this._resultsCheckedListBox.Name = "_resultsCheckedListBox";
            this._resultsCheckedListBox.Size = new System.Drawing.Size(528, 93);
            this._resultsCheckedListBox.TabIndex = 0;
            // 
            // _resultsHintLabel
            // 
            this._resultsHintLabel.AutoSize = true;
            this._resultsHintLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._resultsHintLabel.ForeColor = System.Drawing.Color.Blue;
            this._resultsHintLabel.Location = new System.Drawing.Point(0, 104);
            this._resultsHintLabel.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this._resultsHintLabel.Name = "_resultsHintLabel";
            this._resultsHintLabel.Size = new System.Drawing.Size(354, 13);
            this._resultsHintLabel.TabIndex = 1;
            this._resultsHintLabel.Text = "Marque um ou mais bancos para adicionar a lista de configuracoes.";
            // 
            // _buttonsLayout
            // 
            this._buttonsLayout.ColumnCount = 4;
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.Controls.Add(this._searchButton, 0, 0);
            this._buttonsLayout.Controls.Add(this._addSelectedButton, 1, 0);
            this._buttonsLayout.Controls.Add(this._cancelButton, 3, 0);
            this._buttonsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._buttonsLayout.Location = new System.Drawing.Point(20, 380);
            this._buttonsLayout.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._buttonsLayout.Name = "_buttonsLayout";
            this._buttonsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._buttonsLayout.Size = new System.Drawing.Size(560, 100);
            this._buttonsLayout.TabIndex = 5;
            // 
            // _searchButton
            // 
            this._searchButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._searchButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._searchButton.Location = new System.Drawing.Point(3, 3);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(110, 28);
            this._searchButton.TabIndex = 0;
            this._searchButton.Text = "Buscar";
            this._searchButton.UseVisualStyleBackColor = true;
            // 
            // _addSelectedButton
            // 
            this._addSelectedButton.Enabled = false;
            this._addSelectedButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._addSelectedButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._addSelectedButton.Location = new System.Drawing.Point(119, 3);
            this._addSelectedButton.Name = "_addSelectedButton";
            this._addSelectedButton.Size = new System.Drawing.Size(170, 28);
            this._addSelectedButton.TabIndex = 1;
            this._addSelectedButton.Text = "Adicionar Selecionados";
            this._addSelectedButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cancelButton.Location = new System.Drawing.Point(447, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(110, 28);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancelar";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // NavegadorServidoresBancoDadosForm
            // 
            this.AcceptButton = this._searchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(600, 500);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NavegadorServidoresBancoDadosForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Buscar Bancos";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._connectionGroup.ResumeLayout(false);
            this._connectionLayout.ResumeLayout(false);
            this._connectionLayout.PerformLayout();
            this._resultsGroup.ResumeLayout(false);
            this._resultsLayout.ResumeLayout(false);
            this._resultsLayout.PerformLayout();
            this._buttonsLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private Label _titleLabel;
        private Label _subtitleLabel;
        private GroupBox _connectionGroup;
        private TableLayoutPanel _connectionLayout;
        private Label _hostLabel;
        private TextBox _hostTextBox;
        private Label _portLabel;
        private TextBox _portTextBox;
        private Label _userLabel;
        private TextBox _userTextBox;
        private Label _passwordLabel;
        private TextBox _passwordTextBox;
        private Label _statusLabel;
        private GroupBox _resultsGroup;
        private TableLayoutPanel _resultsLayout;
        private CheckedListBox _resultsCheckedListBox;
        private Label _resultsHintLabel;
        private TableLayoutPanel _buttonsLayout;
        private Button _searchButton;
        private Button _addSelectedButton;
        private Button _cancelButton;
    }
}
