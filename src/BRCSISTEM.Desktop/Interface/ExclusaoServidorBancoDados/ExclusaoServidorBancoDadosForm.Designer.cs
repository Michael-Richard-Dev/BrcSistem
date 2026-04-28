using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.ExclusaoServidorBancoDados
{
    public sealed partial class ExclusaoServidorBancoDadosForm
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
            this._headerLayout = new TableLayoutPanel();
            this._titleLabel = new Label();
            this._warningLabel = new Label();
            this._serverGroup = new GroupBox();
            this._serverLayout = new TableLayoutPanel();
            this._hostLabel = new Label();
            this._hostTextBox = new TextBox();
            this._portLabel = new Label();
            this._portTextBox = new TextBox();
            this._adminUserLabel = new Label();
            this._adminUserTextBox = new TextBox();
            this._adminPasswordLabel = new Label();
            this._adminPasswordTextBox = new TextBox();
            this._statusLabel = new Label();
            this._resultsGroup = new GroupBox();
            this._databasesListBox = new ListBox();
            this._buttonsLayout = new TableLayoutPanel();
            this._listButton = new Button();
            this._deleteButton = new Button();
            this._cancelButton = new Button();
            this._rootLayout.SuspendLayout();
            this._headerLayout.SuspendLayout();
            this._serverGroup.SuspendLayout();
            this._serverLayout.SuspendLayout();
            this._resultsGroup.SuspendLayout();
            this._buttonsLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // _rootLayout
            //
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerLayout, 0, 0);
            this._rootLayout.Controls.Add(this._serverGroup, 0, 1);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 2);
            this._rootLayout.Controls.Add(this._resultsGroup, 0, 3);
            this._rootLayout.Controls.Add(this._buttonsLayout, 0, 4);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Padding = new Padding(20);
            this._rootLayout.RowCount = 5;
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle());
            //
            // _headerLayout
            //
            this._headerLayout.AutoSize = true;
            this._headerLayout.ColumnCount = 1;
            this._headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._headerLayout.Controls.Add(this._titleLabel, 0, 0);
            this._headerLayout.Controls.Add(this._warningLabel, 0, 1);
            this._headerLayout.Dock = DockStyle.Fill;
            this._headerLayout.Margin = new Padding(0, 0, 0, 15);
            //
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this._titleLabel.ForeColor = Color.Firebrick;
            this._titleLabel.Margin = new Padding(0, 0, 0, 3);
            this._titleLabel.Text = "EXCLUIR Banco do Servidor";
            this._warningLabel.AutoSize = true;
            this._warningLabel.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            this._warningLabel.ForeColor = Color.Firebrick;
            this._warningLabel.Margin = new Padding(0, 3, 0, 0);
            this._warningLabel.Text = "ATENCAO: Esta acao APAGA o banco e TODOS os dados do servidor!";
            //
            // _serverGroup
            //
            this._serverGroup.Controls.Add(this._serverLayout);
            this._serverGroup.Dock = DockStyle.Fill;
            this._serverGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._serverGroup.Margin = new Padding(0, 0, 0, 10);
            this._serverGroup.Padding = new Padding(10);
            this._serverGroup.Text = " Conectar ao Servidor ";
            //
            this._serverLayout.ColumnCount = 2;
            this._serverLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this._serverLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._serverLayout.Controls.Add(this._hostLabel, 0, 0);
            this._serverLayout.Controls.Add(this._hostTextBox, 1, 0);
            this._serverLayout.Controls.Add(this._portLabel, 0, 1);
            this._serverLayout.Controls.Add(this._portTextBox, 1, 1);
            this._serverLayout.Controls.Add(this._adminUserLabel, 0, 2);
            this._serverLayout.Controls.Add(this._adminUserTextBox, 1, 2);
            this._serverLayout.Controls.Add(this._adminPasswordLabel, 0, 3);
            this._serverLayout.Controls.Add(this._adminPasswordTextBox, 1, 3);
            this._serverLayout.Dock = DockStyle.Fill;
            this._serverLayout.RowCount = 4;
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._hostLabel.AutoSize = true;
            this._hostLabel.Font = new Font("Segoe UI", 8.25F);
            this._hostLabel.Margin = new Padding(0, 3, 10, 3);
            this._hostLabel.Text = "Host:";
            this._portLabel.AutoSize = true;
            this._portLabel.Font = new Font("Segoe UI", 8.25F);
            this._portLabel.Margin = new Padding(0, 3, 10, 3);
            this._portLabel.Text = "Porta:";
            this._adminUserLabel.AutoSize = true;
            this._adminUserLabel.Font = new Font("Segoe UI", 8.25F);
            this._adminUserLabel.Margin = new Padding(0, 3, 10, 3);
            this._adminUserLabel.Text = "Usuario Admin:";
            this._adminPasswordLabel.AutoSize = true;
            this._adminPasswordLabel.Font = new Font("Segoe UI", 8.25F);
            this._adminPasswordLabel.Margin = new Padding(0, 3, 10, 3);
            this._adminPasswordLabel.Text = "Senha:";
            this._hostTextBox.Dock = DockStyle.Top;
            this._hostTextBox.Font = new Font("Segoe UI", 8.25F);
            this._hostTextBox.Margin = new Padding(0, 3, 0, 3);
            this._portTextBox.Dock = DockStyle.Top;
            this._portTextBox.Font = new Font("Segoe UI", 8.25F);
            this._portTextBox.Margin = new Padding(0, 3, 0, 3);
            this._adminUserTextBox.Dock = DockStyle.Top;
            this._adminUserTextBox.Font = new Font("Segoe UI", 8.25F);
            this._adminUserTextBox.Margin = new Padding(0, 3, 0, 3);
            this._adminPasswordTextBox.Dock = DockStyle.Top;
            this._adminPasswordTextBox.Font = new Font("Segoe UI", 8.25F);
            this._adminPasswordTextBox.Margin = new Padding(0, 3, 0, 3);
            this._adminPasswordTextBox.UseSystemPasswordChar = true;
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new Font("Segoe UI", 9F);
            this._statusLabel.ForeColor = Color.SeaGreen;
            this._statusLabel.Margin = new Padding(0, 8, 0, 8);
            //
            // _resultsGroup
            //
            this._resultsGroup.Controls.Add(this._databasesListBox);
            this._resultsGroup.Dock = DockStyle.Fill;
            this._resultsGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._resultsGroup.Margin = new Padding(0, 0, 0, 15);
            this._resultsGroup.Padding = new Padding(8);
            this._resultsGroup.Text = " Selecione o Banco para EXCLUIR ";
            //
            this._databasesListBox.Dock = DockStyle.Fill;
            this._databasesListBox.Font = new Font("Segoe UI", 9F);
            this._databasesListBox.IntegralHeight = false;
            //
            // _buttonsLayout
            //
            this._buttonsLayout.ColumnCount = 4;
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.Controls.Add(this._listButton, 0, 0);
            this._buttonsLayout.Controls.Add(this._deleteButton, 1, 0);
            this._buttonsLayout.Controls.Add(this._cancelButton, 3, 0);
            this._buttonsLayout.Dock = DockStyle.Fill;
            this._buttonsLayout.Margin = new Padding(0, 10, 0, 0);
            this._listButton.FlatStyle = FlatStyle.System;
            this._listButton.Margin = new Padding(5, 0, 5, 0);
            this._listButton.Size = new Size(96, 28);
            this._listButton.TabIndex = 0;
            this._listButton.Text = "Listar Bancos";
            this._listButton.UseVisualStyleBackColor = true;
            this._deleteButton.FlatStyle = FlatStyle.System;
            this._deleteButton.Margin = new Padding(5, 0, 5, 0);
            this._deleteButton.Size = new Size(120, 28);
            this._deleteButton.TabIndex = 1;
            this._deleteButton.Text = "EXCLUIR Banco";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._cancelButton.FlatStyle = FlatStyle.System;
            this._cancelButton.Margin = new Padding(5, 0, 5, 0);
            this._cancelButton.Size = new Size(96, 28);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancelar";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._deleteButton.Enabled = false;
            //
            // ExclusaoServidorBancoDadosForm
            //
            this.AcceptButton = this._listButton;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new Size(600, 480);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExclusaoServidorBancoDadosForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Excluir Banco do Servidor PostgreSQL";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._headerLayout.ResumeLayout(false);
            this._headerLayout.PerformLayout();
            this._serverGroup.ResumeLayout(false);
            this._serverLayout.ResumeLayout(false);
            this._serverLayout.PerformLayout();
            this._resultsGroup.ResumeLayout(false);
            this._buttonsLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private TableLayoutPanel _headerLayout;
        private Label _titleLabel;
        private Label _warningLabel;
        private GroupBox _serverGroup;
        private TableLayoutPanel _serverLayout;
        private Label _hostLabel;
        private TextBox _hostTextBox;
        private Label _portLabel;
        private TextBox _portTextBox;
        private Label _adminUserLabel;
        private TextBox _adminUserTextBox;
        private Label _adminPasswordLabel;
        private TextBox _adminPasswordTextBox;
        private Label _statusLabel;
        private GroupBox _resultsGroup;
        private ListBox _databasesListBox;
        private TableLayoutPanel _buttonsLayout;
        private Button _listButton;
        private Button _deleteButton;
        private Button _cancelButton;
    }
}
