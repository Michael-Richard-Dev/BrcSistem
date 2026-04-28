using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Interface.CriacaoServidorBancoDados
{
    public sealed partial class CriacaoServidorBancoDadosForm
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
            this._serverGroup = new GroupBox();
            this._serverLayout = new TableLayoutPanel();
            this._hostLabel = new Label();
            this._hostTextBox = new TextBox();
            this._hostHintLabel = new Label();
            this._portLabel = new Label();
            this._portTextBox = new TextBox();
            this._portHintLabel = new Label();
            this._adminUserLabel = new Label();
            this._adminUserTextBox = new TextBox();
            this._adminUserHintLabel = new Label();
            this._adminPasswordLabel = new Label();
            this._adminPasswordTextBox = new TextBox();
            this._adminPasswordHintLabel = new Label();
            this._databaseGroup = new GroupBox();
            this._databaseLayout = new TableLayoutPanel();
            this._databaseNameLabel = new Label();
            this._databaseNameTextBox = new TextBox();
            this._databaseHintLabel = new Label();
            this._addToListCheckBox = new CheckBox();
            this._statusLabel = new Label();
            this._buttonsLayout = new TableLayoutPanel();
            this._createButton = new Button();
            this._cancelButton = new Button();
            this._rootLayout.SuspendLayout();
            this._serverGroup.SuspendLayout();
            this._serverLayout.SuspendLayout();
            this._databaseGroup.SuspendLayout();
            this._databaseLayout.SuspendLayout();
            this._buttonsLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // _rootLayout
            //
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._titleLabel, 0, 0);
            this._rootLayout.Controls.Add(this._subtitleLabel, 0, 1);
            this._rootLayout.Controls.Add(this._serverGroup, 0, 2);
            this._rootLayout.Controls.Add(this._databaseGroup, 0, 3);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 4);
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
            this._titleLabel.Text = "Criar Novo Banco no Servidor";
            //
            // _subtitleLabel
            //
            this._subtitleLabel.AutoSize = true;
            this._subtitleLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._subtitleLabel.ForeColor = Color.Gray;
            this._subtitleLabel.Margin = new Padding(0, 0, 0, 15);
            this._subtitleLabel.Text = "Cria um banco vazio no PostgreSQL (sem tabelas)";
            //
            // _serverGroup
            //
            this._serverGroup.Controls.Add(this._serverLayout);
            this._serverGroup.Dock = DockStyle.Top;
            this._serverGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._serverGroup.Margin = new Padding(0, 0, 0, 10);
            this._serverGroup.Padding = new Padding(10);
            this._serverGroup.Text = " Servidor PostgreSQL ";
            //
            this._serverLayout.ColumnCount = 2;
            this._serverLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this._serverLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._serverLayout.Controls.Add(this._hostLabel, 0, 0);
            this._serverLayout.Controls.Add(this._hostTextBox, 1, 0);
            this._serverLayout.Controls.Add(this._hostHintLabel, 1, 1);
            this._serverLayout.Controls.Add(this._portLabel, 0, 2);
            this._serverLayout.Controls.Add(this._portTextBox, 1, 2);
            this._serverLayout.Controls.Add(this._portHintLabel, 1, 3);
            this._serverLayout.Controls.Add(this._adminUserLabel, 0, 4);
            this._serverLayout.Controls.Add(this._adminUserTextBox, 1, 4);
            this._serverLayout.Controls.Add(this._adminUserHintLabel, 1, 5);
            this._serverLayout.Controls.Add(this._adminPasswordLabel, 0, 6);
            this._serverLayout.Controls.Add(this._adminPasswordTextBox, 1, 6);
            this._serverLayout.Controls.Add(this._adminPasswordHintLabel, 1, 7);
            this._serverLayout.Dock = DockStyle.Fill;
            this._serverLayout.RowCount = 8;
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
            this._serverLayout.RowStyles.Add(new RowStyle());
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
            this._adminUserLabel.AutoSize = true;
            this._adminUserLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._adminUserLabel.Margin = new Padding(0, 8, 10, 0);
            this._adminUserLabel.Name = "_adminUserLabel";
            this._adminUserLabel.Text = "Usuario Admin:";
            //
            this._adminPasswordLabel.AutoSize = true;
            this._adminPasswordLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._adminPasswordLabel.Margin = new Padding(0, 8, 10, 0);
            this._adminPasswordLabel.Name = "_adminPasswordLabel";
            this._adminPasswordLabel.Text = "Senha:";
            //
            this._hostTextBox.Dock = DockStyle.Top;
            this._hostTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._hostTextBox.Margin = new Padding(0, 3, 0, 0);
            this._hostTextBox.Name = "_hostTextBox";
            //
            this._hostHintLabel.AutoSize = true;
            this._hostHintLabel.Font = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._hostHintLabel.ForeColor = Color.Gray;
            this._hostHintLabel.Margin = new Padding(0, 0, 0, 3);
            this._hostHintLabel.Name = "_hostHintLabel";
            this._hostHintLabel.Text = "localhost, 127.0.0.1 ou IP do servidor";
            //
            this._portTextBox.Dock = DockStyle.Top;
            this._portTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._portTextBox.Margin = new Padding(0, 3, 0, 0);
            this._portTextBox.Name = "_portTextBox";
            //
            this._portHintLabel.AutoSize = true;
            this._portHintLabel.Font = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._portHintLabel.ForeColor = Color.Gray;
            this._portHintLabel.Margin = new Padding(0, 0, 0, 3);
            this._portHintLabel.Name = "_portHintLabel";
            this._portHintLabel.Text = "Padrao: 5432";
            //
            this._adminUserTextBox.Dock = DockStyle.Top;
            this._adminUserTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._adminUserTextBox.Margin = new Padding(0, 3, 0, 0);
            this._adminUserTextBox.Name = "_adminUserTextBox";
            //
            this._adminUserHintLabel.AutoSize = true;
            this._adminUserHintLabel.Font = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._adminUserHintLabel.ForeColor = Color.Gray;
            this._adminUserHintLabel.Margin = new Padding(0, 0, 0, 3);
            this._adminUserHintLabel.Name = "_adminUserHintLabel";
            this._adminUserHintLabel.Text = "Usuario administrador";
            //
            this._adminPasswordTextBox.Dock = DockStyle.Top;
            this._adminPasswordTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._adminPasswordTextBox.Margin = new Padding(0, 3, 0, 0);
            this._adminPasswordTextBox.Name = "_adminPasswordTextBox";
            this._adminPasswordTextBox.UseSystemPasswordChar = true;
            //
            this._adminPasswordHintLabel.AutoSize = true;
            this._adminPasswordHintLabel.Font = new Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._adminPasswordHintLabel.ForeColor = Color.Gray;
            this._adminPasswordHintLabel.Margin = new Padding(0, 0, 0, 3);
            this._adminPasswordHintLabel.Name = "_adminPasswordHintLabel";
            this._adminPasswordHintLabel.Text = "Senha do administrador";
            //
            // _databaseGroup
            //
            this._databaseGroup.Controls.Add(this._databaseLayout);
            this._databaseGroup.Dock = DockStyle.Top;
            this._databaseGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._databaseGroup.Margin = new Padding(0, 0, 0, 10);
            this._databaseGroup.Padding = new Padding(10);
            this._databaseGroup.Text = " Novo Banco ";
            //
            this._databaseLayout.ColumnCount = 2;
            this._databaseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this._databaseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._databaseLayout.Controls.Add(this._databaseNameLabel, 0, 0);
            this._databaseLayout.Controls.Add(this._databaseNameTextBox, 1, 0);
            this._databaseLayout.Controls.Add(this._databaseHintLabel, 1, 1);
            this._databaseLayout.Controls.Add(this._addToListCheckBox, 0, 2);
            this._databaseLayout.Dock = DockStyle.Fill;
            this._databaseLayout.RowCount = 3;
            this._databaseLayout.RowStyles.Add(new RowStyle());
            this._databaseLayout.RowStyles.Add(new RowStyle());
            this._databaseLayout.RowStyles.Add(new RowStyle());
            this._databaseLayout.SetColumnSpan(this._addToListCheckBox, 2);
            this._databaseNameLabel.AutoSize = true;
            this._databaseNameLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._databaseNameLabel.Margin = new Padding(0, 8, 10, 0);
            this._databaseNameLabel.Name = "_databaseNameLabel";
            this._databaseNameLabel.Text = "Nome do Banco:";
            this._databaseNameTextBox.Dock = DockStyle.Top;
            this._databaseNameTextBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._databaseNameTextBox.Margin = new Padding(0, 3, 0, 0);
            this._databaseNameTextBox.Name = "_databaseNameTextBox";
            this._databaseHintLabel.AutoSize = true;
            this._databaseHintLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._databaseHintLabel.ForeColor = Color.Gray;
            this._databaseHintLabel.Margin = new Padding(0, 0, 0, 8);
            this._databaseHintLabel.Text = "Somente letras, numeros e underscore (_). Ex: brcsistem_producao";
            this._addToListCheckBox.AutoSize = true;
            this._addToListCheckBox.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._addToListCheckBox.Margin = new Padding(0, 8, 0, 0);
            this._addToListCheckBox.Name = "_addToListCheckBox";
            this._addToListCheckBox.Text = "Adicionar a lista apos criar";
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this._statusLabel.ForeColor = Color.SeaGreen;
            this._statusLabel.Margin = new Padding(0, 8, 0, 8);
            //
            // _buttonsLayout
            //
            this._buttonsLayout.ColumnCount = 3;
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._buttonsLayout.ColumnStyles.Add(new ColumnStyle());
            this._buttonsLayout.Controls.Add(this._createButton, 0, 0);
            this._buttonsLayout.Controls.Add(this._cancelButton, 2, 0);
            this._buttonsLayout.Dock = DockStyle.Fill;
            this._buttonsLayout.Margin = new Padding(0, 10, 0, 0);
            this._createButton.FlatStyle = FlatStyle.System;
            this._createButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._createButton.Name = "_createButton";
            this._createButton.Size = new Size(130, 28);
            this._createButton.TabIndex = 0;
            this._createButton.Text = "Criar Banco";
            this._createButton.UseVisualStyleBackColor = true;
            //
            this._cancelButton.FlatStyle = FlatStyle.System;
            this._cancelButton.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(110, 28);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancelar";
            this._cancelButton.UseVisualStyleBackColor = true;
            //
            // CriacaoServidorBancoDadosForm
            //
            this.AcceptButton = this._createButton;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new Size(650, 520);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CriacaoServidorBancoDadosForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Criar Banco PostgreSQL";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._serverGroup.ResumeLayout(false);
            this._serverLayout.ResumeLayout(false);
            this._serverLayout.PerformLayout();
            this._databaseGroup.ResumeLayout(false);
            this._databaseLayout.ResumeLayout(false);
            this._databaseLayout.PerformLayout();
            this._buttonsLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private Label _titleLabel;
        private Label _subtitleLabel;
        private GroupBox _serverGroup;
        private TableLayoutPanel _serverLayout;
        private Label _hostLabel;
        private TextBox _hostTextBox;
        private Label _hostHintLabel;
        private Label _portLabel;
        private TextBox _portTextBox;
        private Label _portHintLabel;
        private Label _adminUserLabel;
        private TextBox _adminUserTextBox;
        private Label _adminUserHintLabel;
        private Label _adminPasswordLabel;
        private TextBox _adminPasswordTextBox;
        private Label _adminPasswordHintLabel;
        private GroupBox _databaseGroup;
        private TableLayoutPanel _databaseLayout;
        private Label _databaseNameLabel;
        private TextBox _databaseNameTextBox;
        private Label _databaseHintLabel;
        private CheckBox _addToListCheckBox;
        private Label _statusLabel;
        private TableLayoutPanel _buttonsLayout;
        private Button _createButton;
        private Button _cancelButton;
    }
}
