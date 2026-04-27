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
            this._rootLayout = new TableLayoutPanel();
            this._titleLabel = new Label();
            this._subtitleLabel = new Label();
            this._connectionGroup = new GroupBox();
            this._connectionLayout = new TableLayoutPanel();
            this._hostLabel = new Label();
            this._hostTextBox = new TextBox();
            this._portLabel = new Label();
            this._portTextBox = new TextBox();
            this._userLabel = new Label();
            this._userTextBox = new TextBox();
            this._passwordLabel = new Label();
            this._passwordTextBox = new TextBox();
            this._statusLabel = new Label();
            this._resultsGroup = new GroupBox();
            this._resultsLayout = new TableLayoutPanel();
            this._resultsCheckedListBox = new CheckedListBox();
            this._resultsHintLabel = new Label();
            this._buttonsLayout = new TableLayoutPanel();
            this._searchButton = new Button();
            this._addSelectedButton = new Button();
            this._cancelButton = new Button();
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
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._titleLabel, 0, 0);
            this._rootLayout.Controls.Add(this._subtitleLabel, 0, 1);
            this._rootLayout.Controls.Add(this._connectionGroup, 0, 2);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 3);
            this._rootLayout.Controls.Add(this._resultsGroup, 0, 4);
            this._rootLayout.Controls.Add(this._buttonsLayout, 0, 5);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Padding = new Padding(20);
            this._rootLayout.RowCount = 6;
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle());
            //
            // _titleLabel
            //
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this._titleLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._titleLabel.Margin = new Padding(0, 0, 0, 3);
            this._titleLabel.Text = "Buscar Bancos no Servidor";
            //
            // _subtitleLabel
            //
            this._subtitleLabel.AutoSize = true;
            this._subtitleLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._subtitleLabel.ForeColor = Color.Gray;
            this._subtitleLabel.Margin = new Padding(0, 0, 0, 15);
            this._subtitleLabel.Text = "Conecte ao servidor e descubra os bancos disponiveis.";
            //
            // _connectionGroup
            //
            this._connectionGroup.Controls.Add(this._connectionLayout);
            this._connectionGroup.Dock = DockStyle.Fill;
            this._connectionGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._connectionGroup.Padding = new Padding(10);
            this._connectionGroup.Text = " Dados de Conexao ";
            //
            // _connectionLayout
            //
            this._connectionLayout.ColumnCount = 2;
            this._connectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._connectionLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._connectionLayout.Controls.Add(this._hostLabel, 0, 0);
            this._connectionLayout.Controls.Add(this._hostTextBox, 1, 0);
            this._connectionLayout.Controls.Add(this._portLabel, 0, 1);
            this._connectionLayout.Controls.Add(this._portTextBox, 1, 1);
            this._connectionLayout.Controls.Add(this._userLabel, 0, 2);
            this._connectionLayout.Controls.Add(this._userTextBox, 1, 2);
            this._connectionLayout.Controls.Add(this._passwordLabel, 0, 3);
            this._connectionLayout.Controls.Add(this._passwordTextBox, 1, 3);
            this._connectionLayout.Dock = DockStyle.Fill;
            this._connectionLayout.RowCount = 4;
            this._connectionLayout.RowStyles.Add(new RowStyle());
            this._connectionLayout.RowStyles.Add(new RowStyle());
            this._connectionLayout.RowStyles.Add(new RowStyle());
            this._connectionLayout.RowStyles.Add(new RowStyle());
            //
            this._hostLabel.AutoSize = true;
            this._hostLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._hostLabel.Margin = new Padding(0, 8, 10, 0);
            this._hostLabel.Name = "_hostLabel";
            this._hostLabel.Text = "Host:";
            //
            this._portLabel.AutoSize = true;
            this._portLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._portLabel.Margin = new Padding(0, 8, 10, 0);
            this._portLabel.Name = "_portLabel";
            this._portLabel.Text = "Porta:";
            //
            this._userLabel.AutoSize = true;
            this._userLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._userLabel.Margin = new Padding(0, 8, 10, 0);
            this._userLabel.Name = "_userLabel";
            this._userLabel.Text = "Usuario:";
            //
            this._passwordLabel.AutoSize = true;
            this._passwordLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._passwordLabel.Margin = new Padding(0, 8, 10, 0);
            this._passwordLabel.Name = "_passwordLabel";
            this._passwordLabel.Text = "Senha:";
            //
            this._hostTextBox.Dock = DockStyle.Top;
            this._hostTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._hostTextBox.Margin = new Padding(0, 5, 0, 5);
            this._hostTextBox.Name = "_hostTextBox";
            //
            this._portTextBox.Dock = DockStyle.Top;
            this._portTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._portTextBox.Margin = new Padding(0, 5, 0, 5);
            this._portTextBox.Name = "_portTextBox";
            //
            this._userTextBox.Dock = DockStyle.Top;
            this._userTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._userTextBox.Margin = new Padding(0, 5, 0, 5);
            this._userTextBox.Name = "_userTextBox";
            //
            this._passwordTextBox.Dock = DockStyle.Top;
            this._passwordTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._passwordTextBox.Margin = new Padding(0, 5, 0, 5);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.UseSystemPasswordChar = true;
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._statusLabel.ForeColor = Color.SeaGreen;
            this._statusLabel.Margin = new Padding(0, 10, 0, 10);
            //
            // _resultsGroup
            //
            this._resultsGroup.Controls.Add(this._resultsLayout);
            this._resultsGroup.Dock = DockStyle.Fill;
            this._resultsGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._resultsGroup.Padding = new Padding(10);
            this._resultsGroup.Text = " Bancos Encontrados ";
            //
            // _resultsLayout
            //
            this._resultsLayout.ColumnCount = 1;
            this._resultsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._resultsLayout.Controls.Add(this._resultsCheckedListBox, 0, 0);
            this._resultsLayout.Controls.Add(this._resultsHintLabel, 0, 1);
            this._resultsLayout.Dock = DockStyle.Fill;
            this._resultsLayout.RowCount = 2;
            this._resultsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._resultsLayout.RowStyles.Add(new RowStyle());
            //
            this._resultsCheckedListBox.CheckOnClick = true;
            this._resultsCheckedListBox.Dock = DockStyle.Fill;
            this._resultsCheckedListBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._resultsCheckedListBox.IntegralHeight = false;
            //
            this._resultsHintLabel.AutoSize = true;
            this._resultsHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._resultsHintLabel.ForeColor = Color.Blue;
            this._resultsHintLabel.Margin = new Padding(0, 5, 0, 0);
            this._resultsHintLabel.Text = "Marque um ou mais bancos para adicionar a lista de configuracoes.";
            //
            // _buttonsLayout
            //
            this._buttonsLayout.ColumnCount = 4;
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.Controls.Add(this._searchButton, 0, 0);
            this._buttonsLayout.Controls.Add(this._addSelectedButton, 1, 0);
            this._buttonsLayout.Controls.Add(this._cancelButton, 3, 0);
            this._buttonsLayout.Dock = DockStyle.Fill;
            this._buttonsLayout.Margin = new Padding(0, 10, 0, 0);
            //
            this._searchButton.FlatStyle = FlatStyle.System;
            this._searchButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new Size(110, 28);
            this._searchButton.TabIndex = 0;
            this._searchButton.Text = "Buscar";
            this._searchButton.UseVisualStyleBackColor = true;
            //
            this._addSelectedButton.FlatStyle = FlatStyle.System;
            this._addSelectedButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._addSelectedButton.Name = "_addSelectedButton";
            this._addSelectedButton.Size = new Size(170, 28);
            this._addSelectedButton.TabIndex = 1;
            this._addSelectedButton.Text = "Adicionar Selecionados";
            this._addSelectedButton.UseVisualStyleBackColor = true;
            //
            this._cancelButton.FlatStyle = FlatStyle.System;
            this._cancelButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(110, 28);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancelar";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._addSelectedButton.Enabled = false;
            //
            // NavegadorServidoresBancoDadosForm
            //
            this.AcceptButton = this._searchButton;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new Size(600, 500);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NavegadorServidoresBancoDadosForm";
            this.StartPosition = FormStartPosition.CenterParent;
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
