using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class DatabaseServerCreateForm
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
            this._portLabel = new Label();
            this._portTextBox = new TextBox();
            this._adminUserLabel = new Label();
            this._adminUserTextBox = new TextBox();
            this._adminPasswordLabel = new Label();
            this._adminPasswordTextBox = new TextBox();
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
            this._subtitleLabel.Font = new Font("Segoe UI", 8F);
            this._subtitleLabel.ForeColor = Color.Gray;
            this._subtitleLabel.Margin = new Padding(0, 0, 0, 15);
            this._subtitleLabel.Text = "Cria um banco vazio no PostgreSQL e pode adiciona-lo na lista.";
            //
            // _serverGroup
            //
            this._serverGroup.Controls.Add(this._serverLayout);
            this._serverGroup.Dock = DockStyle.Fill;
            this._serverGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._serverGroup.Padding = new Padding(10);
            this._serverGroup.Text = " Servidor PostgreSQL ";
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
            ConfigureFieldLabel(this._hostLabel, "Host:");
            ConfigureFieldLabel(this._portLabel, "Porta:");
            ConfigureFieldLabel(this._adminUserLabel, "Usuario Admin:");
            ConfigureFieldLabel(this._adminPasswordLabel, "Senha:");
            ConfigureTextBox(this._hostTextBox);
            ConfigureTextBox(this._portTextBox);
            ConfigureTextBox(this._adminUserTextBox);
            ConfigureTextBox(this._adminPasswordTextBox);
            this._adminPasswordTextBox.UseSystemPasswordChar = true;
            //
            // _databaseGroup
            //
            this._databaseGroup.Controls.Add(this._databaseLayout);
            this._databaseGroup.Dock = DockStyle.Fill;
            this._databaseGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._databaseGroup.Padding = new Padding(10);
            this._databaseGroup.Text = " Novo Banco ";
            //
            this._databaseLayout.ColumnCount = 2;
            this._databaseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this._databaseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._databaseLayout.Controls.Add(this._databaseNameLabel, 0, 0);
            this._databaseLayout.Controls.Add(this._databaseNameTextBox, 1, 0);
            this._databaseLayout.Controls.Add(this._databaseHintLabel, 1, 1);
            this._databaseLayout.Controls.Add(this._addToListCheckBox, 1, 2);
            this._databaseLayout.Dock = DockStyle.Fill;
            this._databaseLayout.RowCount = 3;
            this._databaseLayout.RowStyles.Add(new RowStyle());
            this._databaseLayout.RowStyles.Add(new RowStyle());
            this._databaseLayout.RowStyles.Add(new RowStyle());
            ConfigureFieldLabel(this._databaseNameLabel, "Nome do Banco:");
            ConfigureTextBox(this._databaseNameTextBox);
            this._databaseHintLabel.AutoSize = true;
            this._databaseHintLabel.Font = new Font("Segoe UI", 8F);
            this._databaseHintLabel.ForeColor = Color.Gray;
            this._databaseHintLabel.Margin = new Padding(0, 0, 0, 8);
            this._databaseHintLabel.Text = "Use somente letras, numeros e underscore (_).";
            this._addToListCheckBox.AutoSize = true;
            this._addToListCheckBox.Text = "Adicionar a lista apos criar";
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._statusLabel.ForeColor = Color.SeaGreen;
            this._statusLabel.Margin = new Padding(0, 10, 0, 10);
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
            ConfigureButton(this._createButton, "Criar Banco", 0, 130);
            ConfigureButton(this._cancelButton, "Cancelar", 1, 110);
            //
            // DatabaseServerCreateForm
            //
            this.AcceptButton = this._createButton;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new Size(650, 420);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseServerCreateForm";
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

        private static void ConfigureFieldLabel(Label label, string text)
        {
            label.AutoSize = true;
            label.Margin = new Padding(0, 8, 10, 0);
            label.Text = text;
        }

        private static void ConfigureTextBox(TextBox textBox)
        {
            textBox.Dock = DockStyle.Top;
            textBox.Margin = new Padding(0, 5, 0, 5);
        }

        private static void ConfigureButton(Button button, string text, int tabIndex, int width)
        {
            button.FlatStyle = FlatStyle.System;
            button.Size = new Size(width, 28);
            button.TabIndex = tabIndex;
            button.Text = text;
            button.UseVisualStyleBackColor = true;
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private Label _titleLabel;
        private Label _subtitleLabel;
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
