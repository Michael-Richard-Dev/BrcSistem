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
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._warningLabel = new System.Windows.Forms.Label();
            this._serverGroup = new System.Windows.Forms.GroupBox();
            this._serverLayout = new System.Windows.Forms.TableLayoutPanel();
            this._hostLabel = new System.Windows.Forms.Label();
            this._hostTextBox = new System.Windows.Forms.TextBox();
            this._portLabel = new System.Windows.Forms.Label();
            this._portTextBox = new System.Windows.Forms.TextBox();
            this._adminUserLabel = new System.Windows.Forms.Label();
            this._adminUserTextBox = new System.Windows.Forms.TextBox();
            this._adminPasswordLabel = new System.Windows.Forms.Label();
            this._adminPasswordTextBox = new System.Windows.Forms.TextBox();
            this._statusLabel = new System.Windows.Forms.Label();
            this._resultsGroup = new System.Windows.Forms.GroupBox();
            this._databasesListBox = new System.Windows.Forms.ListBox();
            this._buttonsLayout = new System.Windows.Forms.TableLayoutPanel();
            this._listButton = new System.Windows.Forms.Button();
            this._deleteButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
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
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerLayout, 0, 0);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 2);
            this._rootLayout.Controls.Add(this._resultsGroup, 0, 3);
            this._rootLayout.Controls.Add(this._buttonsLayout, 0, 4);
            this._rootLayout.Controls.Add(this._serverGroup, 0, 1);
            this._rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootLayout.Location = new System.Drawing.Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new System.Windows.Forms.Padding(20);
            this._rootLayout.RowCount = 5;
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 149F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 9F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this._rootLayout.Size = new System.Drawing.Size(600, 480);
            this._rootLayout.TabIndex = 0;
            // 
            // _headerLayout
            // 
            this._headerLayout.ColumnCount = 1;
            this._headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._headerLayout.Controls.Add(this._titleLabel, 0, 0);
            this._headerLayout.Controls.Add(this._warningLabel, 0, 1);
            this._headerLayout.Location = new System.Drawing.Point(20, 20);
            this._headerLayout.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this._headerLayout.Name = "_headerLayout";
            this._headerLayout.RowCount = 2;
            this._headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this._headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._headerLayout.Size = new System.Drawing.Size(560, 44);
            this._headerLayout.TabIndex = 0;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this._titleLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._titleLabel.Location = new System.Drawing.Point(0, 0);
            this._titleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(218, 21);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "EXCLUIR Banco do Servidor";
            // 
            // _warningLabel
            // 
            this._warningLabel.AutoSize = true;
            this._warningLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this._warningLabel.ForeColor = System.Drawing.Color.Firebrick;
            this._warningLabel.Location = new System.Drawing.Point(0, 27);
            this._warningLabel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._warningLabel.Name = "_warningLabel";
            this._warningLabel.Size = new System.Drawing.Size(362, 13);
            this._warningLabel.TabIndex = 1;
            this._warningLabel.Text = "ATENCAO: Esta acao APAGA o banco e TODOS os dados do servidor!";
            // 
            // _serverGroup
            // 
            this._serverGroup.Controls.Add(this._serverLayout);
            this._serverGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._serverGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._serverGroup.Location = new System.Drawing.Point(20, 79);
            this._serverGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._serverGroup.Name = "_serverGroup";
            this._serverGroup.Padding = new System.Windows.Forms.Padding(10);
            this._serverGroup.Size = new System.Drawing.Size(560, 139);
            this._serverGroup.TabIndex = 1;
            this._serverGroup.TabStop = false;
            this._serverGroup.Text = " Conectar ao Servidor ";
            // 
            // _serverLayout
            // 
            this._serverLayout.ColumnCount = 2;
            this._serverLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this._serverLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._serverLayout.Controls.Add(this._hostLabel, 0, 0);
            this._serverLayout.Controls.Add(this._hostTextBox, 1, 0);
            this._serverLayout.Controls.Add(this._portLabel, 0, 1);
            this._serverLayout.Controls.Add(this._portTextBox, 1, 1);
            this._serverLayout.Controls.Add(this._adminUserLabel, 0, 2);
            this._serverLayout.Controls.Add(this._adminUserTextBox, 1, 2);
            this._serverLayout.Controls.Add(this._adminPasswordLabel, 0, 3);
            this._serverLayout.Controls.Add(this._adminPasswordTextBox, 1, 3);
            this._serverLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._serverLayout.Location = new System.Drawing.Point(10, 26);
            this._serverLayout.Name = "_serverLayout";
            this._serverLayout.RowCount = 4;
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this._serverLayout.Size = new System.Drawing.Size(540, 103);
            this._serverLayout.TabIndex = 0;
            // 
            // _hostLabel
            // 
            this._hostLabel.AutoSize = true;
            this._hostLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._hostLabel.Location = new System.Drawing.Point(0, 3);
            this._hostLabel.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this._hostLabel.Name = "_hostLabel";
            this._hostLabel.Size = new System.Drawing.Size(34, 13);
            this._hostLabel.TabIndex = 0;
            this._hostLabel.Text = "Host:";
            // 
            // _hostTextBox
            // 
            this._hostTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._hostTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._hostTextBox.Location = new System.Drawing.Point(120, 3);
            this._hostTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this._hostTextBox.Name = "_hostTextBox";
            this._hostTextBox.Size = new System.Drawing.Size(420, 22);
            this._hostTextBox.TabIndex = 1;
            // 
            // _portLabel
            // 
            this._portLabel.AutoSize = true;
            this._portLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._portLabel.Location = new System.Drawing.Point(0, 29);
            this._portLabel.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this._portLabel.Name = "_portLabel";
            this._portLabel.Size = new System.Drawing.Size(37, 13);
            this._portLabel.TabIndex = 2;
            this._portLabel.Text = "Porta:";
            // 
            // _portTextBox
            // 
            this._portTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._portTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._portTextBox.Location = new System.Drawing.Point(120, 29);
            this._portTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this._portTextBox.Name = "_portTextBox";
            this._portTextBox.Size = new System.Drawing.Size(420, 22);
            this._portTextBox.TabIndex = 3;
            // 
            // _adminUserLabel
            // 
            this._adminUserLabel.AutoSize = true;
            this._adminUserLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._adminUserLabel.Location = new System.Drawing.Point(0, 55);
            this._adminUserLabel.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this._adminUserLabel.Name = "_adminUserLabel";
            this._adminUserLabel.Size = new System.Drawing.Size(86, 13);
            this._adminUserLabel.TabIndex = 4;
            this._adminUserLabel.Text = "Usuario Admin:";
            // 
            // _adminUserTextBox
            // 
            this._adminUserTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._adminUserTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._adminUserTextBox.Location = new System.Drawing.Point(120, 55);
            this._adminUserTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this._adminUserTextBox.Name = "_adminUserTextBox";
            this._adminUserTextBox.Size = new System.Drawing.Size(420, 22);
            this._adminUserTextBox.TabIndex = 5;
            // 
            // _adminPasswordLabel
            // 
            this._adminPasswordLabel.AutoSize = true;
            this._adminPasswordLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._adminPasswordLabel.Location = new System.Drawing.Point(0, 81);
            this._adminPasswordLabel.Margin = new System.Windows.Forms.Padding(0, 3, 10, 3);
            this._adminPasswordLabel.Name = "_adminPasswordLabel";
            this._adminPasswordLabel.Size = new System.Drawing.Size(42, 13);
            this._adminPasswordLabel.TabIndex = 6;
            this._adminPasswordLabel.Text = "Senha:";
            // 
            // _adminPasswordTextBox
            // 
            this._adminPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._adminPasswordTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._adminPasswordTextBox.Location = new System.Drawing.Point(120, 81);
            this._adminPasswordTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this._adminPasswordTextBox.Name = "_adminPasswordTextBox";
            this._adminPasswordTextBox.Size = new System.Drawing.Size(420, 22);
            this._adminPasswordTextBox.TabIndex = 7;
            this._adminPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusLabel.Location = new System.Drawing.Point(20, 236);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(0, 1);
            this._statusLabel.TabIndex = 2;
            // 
            // _resultsGroup
            // 
            this._resultsGroup.Controls.Add(this._databasesListBox);
            this._resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._resultsGroup.Location = new System.Drawing.Point(20, 237);
            this._resultsGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this._resultsGroup.Name = "_resultsGroup";
            this._resultsGroup.Padding = new System.Windows.Forms.Padding(8);
            this._resultsGroup.Size = new System.Drawing.Size(560, 170);
            this._resultsGroup.TabIndex = 3;
            this._resultsGroup.TabStop = false;
            this._resultsGroup.Text = " Selecione o Banco para EXCLUIR ";
            // 
            // _databasesListBox
            // 
            this._databasesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._databasesListBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._databasesListBox.IntegralHeight = false;
            this._databasesListBox.ItemHeight = 15;
            this._databasesListBox.Location = new System.Drawing.Point(8, 24);
            this._databasesListBox.Name = "_databasesListBox";
            this._databasesListBox.Size = new System.Drawing.Size(544, 138);
            this._databasesListBox.TabIndex = 0;
            // 
            // _buttonsLayout
            // 
            this._buttonsLayout.ColumnCount = 4;
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.Controls.Add(this._listButton, 0, 0);
            this._buttonsLayout.Controls.Add(this._deleteButton, 1, 0);
            this._buttonsLayout.Controls.Add(this._cancelButton, 3, 0);
            this._buttonsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._buttonsLayout.Location = new System.Drawing.Point(20, 432);
            this._buttonsLayout.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._buttonsLayout.Name = "_buttonsLayout";
            this._buttonsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._buttonsLayout.Size = new System.Drawing.Size(560, 28);
            this._buttonsLayout.TabIndex = 4;
            // 
            // _listButton
            // 
            this._listButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._listButton.Location = new System.Drawing.Point(5, 0);
            this._listButton.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this._listButton.Name = "_listButton";
            this._listButton.Size = new System.Drawing.Size(96, 28);
            this._listButton.TabIndex = 0;
            this._listButton.Text = "Listar Bancos";
            this._listButton.UseVisualStyleBackColor = true;
            // 
            // _deleteButton
            // 
            this._deleteButton.Enabled = false;
            this._deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._deleteButton.Location = new System.Drawing.Point(111, 0);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(120, 28);
            this._deleteButton.TabIndex = 1;
            this._deleteButton.Text = "EXCLUIR Banco";
            this._deleteButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cancelButton.Location = new System.Drawing.Point(459, 0);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(96, 28);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancelar";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // ExclusaoServidorBancoDadosForm
            // 
            this.AcceptButton = this._listButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(600, 480);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExclusaoServidorBancoDadosForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
