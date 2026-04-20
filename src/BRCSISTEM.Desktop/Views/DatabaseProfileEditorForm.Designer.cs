using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class DatabaseProfileEditorForm
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
            this._headerLabel = new Label();
            this._formGroup = new GroupBox();
            this._formLayout = new TableLayoutPanel();
            this._sectionIdLabel = new Label();
            this._nameLabel = new Label();
            this._nameTextBox = new TextBox();
            this._nameHintLabel = new Label();
            this._descriptionLabel = new Label();
            this._descriptionTextBox = new TextBox();
            this._descriptionHintLabel = new Label();
            this._separator1 = new Label();
            this._sectionServerLabel = new Label();
            this._hostLabel = new Label();
            this._hostTextBox = new TextBox();
            this._hostHintLabel = new Label();
            this._portLabel = new Label();
            this._portNumericUpDown = new NumericUpDown();
            this._portHintLabel = new Label();
            this._separator2 = new Label();
            this._sectionDbLabel = new Label();
            this._databaseLabel = new Label();
            this._databaseTextBox = new TextBox();
            this._databaseHintLabel = new Label();
            this._separator3 = new Label();
            this._sectionAuthLabel = new Label();
            this._userLabel = new Label();
            this._userTextBox = new TextBox();
            this._userHintLabel = new Label();
            this._passwordLabel = new Label();
            this._passwordTextBox = new TextBox();
            this._passwordHintLabel = new Label();
            this._statusLabel = new Label();
            this._buttonsPanel = new TableLayoutPanel();
            this._leftButtonsPanel = new FlowLayoutPanel();
            this._saveButton = new Button();
            this._testButton = new Button();
            this._cancelButton = new Button();
            this._rootLayout.SuspendLayout();
            this._formGroup.SuspendLayout();
            this._formLayout.SuspendLayout();
            ((ISupportInitialize)(this._portNumericUpDown)).BeginInit();
            this._buttonsPanel.SuspendLayout();
            this._leftButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // _rootLayout
            //
            this._rootLayout.ColumnCount = 1;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._headerLabel, 0, 0);
            this._rootLayout.Controls.Add(this._formGroup, 0, 1);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 2);
            this._rootLayout.Controls.Add(this._buttonsPanel, 0, 3);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Padding = new Padding(20);
            this._rootLayout.RowCount = 4;
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.RowStyles.Add(new RowStyle());
            this._rootLayout.Name = "_rootLayout";
            //
            // _headerLabel
            //
            this._headerLabel.AutoSize = true;
            this._headerLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this._headerLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._headerLabel.Margin = new Padding(0, 0, 0, 15);
            this._headerLabel.Name = "_headerLabel";
            this._headerLabel.Text = "Nova Configuracao";
            //
            // _formGroup
            //
            this._formGroup.Controls.Add(this._formLayout);
            this._formGroup.Dock = DockStyle.Fill;
            this._formGroup.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._formGroup.Name = "_formGroup";
            this._formGroup.Padding = new Padding(10);
            this._formGroup.Text = " Configuracoes ";
            //
            // _formLayout
            //
            this._formLayout.ColumnCount = 2;
            this._formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            this._formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._formLayout.Controls.Add(this._sectionIdLabel, 0, 0);
            this._formLayout.SetColumnSpan(this._sectionIdLabel, 2);
            this._formLayout.Controls.Add(this._nameLabel, 0, 1);
            this._formLayout.Controls.Add(this._nameTextBox, 1, 1);
            this._formLayout.Controls.Add(this._nameHintLabel, 1, 2);
            this._formLayout.Controls.Add(this._descriptionLabel, 0, 3);
            this._formLayout.Controls.Add(this._descriptionTextBox, 1, 3);
            this._formLayout.Controls.Add(this._descriptionHintLabel, 1, 4);
            this._formLayout.Controls.Add(this._separator1, 0, 5);
            this._formLayout.SetColumnSpan(this._separator1, 2);
            this._formLayout.Controls.Add(this._sectionServerLabel, 0, 6);
            this._formLayout.SetColumnSpan(this._sectionServerLabel, 2);
            this._formLayout.Controls.Add(this._hostLabel, 0, 7);
            this._formLayout.Controls.Add(this._hostTextBox, 1, 7);
            this._formLayout.Controls.Add(this._hostHintLabel, 1, 8);
            this._formLayout.Controls.Add(this._portLabel, 0, 9);
            this._formLayout.Controls.Add(this._portNumericUpDown, 1, 9);
            this._formLayout.Controls.Add(this._portHintLabel, 1, 10);
            this._formLayout.Controls.Add(this._separator2, 0, 11);
            this._formLayout.SetColumnSpan(this._separator2, 2);
            this._formLayout.Controls.Add(this._sectionDbLabel, 0, 12);
            this._formLayout.SetColumnSpan(this._sectionDbLabel, 2);
            this._formLayout.Controls.Add(this._databaseLabel, 0, 13);
            this._formLayout.Controls.Add(this._databaseTextBox, 1, 13);
            this._formLayout.Controls.Add(this._databaseHintLabel, 1, 14);
            this._formLayout.Controls.Add(this._separator3, 0, 15);
            this._formLayout.SetColumnSpan(this._separator3, 2);
            this._formLayout.Controls.Add(this._sectionAuthLabel, 0, 16);
            this._formLayout.SetColumnSpan(this._sectionAuthLabel, 2);
            this._formLayout.Controls.Add(this._userLabel, 0, 17);
            this._formLayout.Controls.Add(this._userTextBox, 1, 17);
            this._formLayout.Controls.Add(this._userHintLabel, 1, 18);
            this._formLayout.Controls.Add(this._passwordLabel, 0, 19);
            this._formLayout.Controls.Add(this._passwordTextBox, 1, 19);
            this._formLayout.Controls.Add(this._passwordHintLabel, 1, 20);
            this._formLayout.Dock = DockStyle.Fill;
            this._formLayout.Font = new Font("Segoe UI", 9F);
            this._formLayout.Name = "_formLayout";
            this._formLayout.RowCount = 21;
            for (int i = 0; i < 21; i++) this._formLayout.RowStyles.Add(new RowStyle());
            //
            ConfigureSectionLabel(this._sectionIdLabel, "Identificacao");
            ConfigureFieldLabel(this._nameLabel, "Nome:");
            ConfigureTextBox(this._nameTextBox);
            ConfigureHintLabel(this._nameHintLabel, "Nome unico");
            ConfigureFieldLabel(this._descriptionLabel, "Descricao:");
            ConfigureTextBox(this._descriptionTextBox);
            ConfigureHintLabel(this._descriptionHintLabel, "Ex: Producao, Testes");
            ConfigureSeparator(this._separator1);
            ConfigureSectionLabel(this._sectionServerLabel, "Servidor");
            ConfigureFieldLabel(this._hostLabel, "Host:");
            ConfigureTextBox(this._hostTextBox);
            ConfigureHintLabel(this._hostHintLabel, "localhost ou IP");
            ConfigureFieldLabel(this._portLabel, "Porta:");
            //
            this._portNumericUpDown.Anchor = AnchorStyles.Left;
            this._portNumericUpDown.Font = new Font("Segoe UI", 9F);
            this._portNumericUpDown.Margin = new Padding(0, 5, 0, 5);
            this._portNumericUpDown.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            this._portNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this._portNumericUpDown.Name = "_portNumericUpDown";
            this._portNumericUpDown.Size = new Size(130, 23);
            this._portNumericUpDown.Value = new decimal(new int[] { 5432, 0, 0, 0 });
            //
            ConfigureHintLabel(this._portHintLabel, "Padrao: 5432");
            ConfigureSeparator(this._separator2);
            ConfigureSectionLabel(this._sectionDbLabel, "Banco de Dados");
            ConfigureFieldLabel(this._databaseLabel, "Database:");
            ConfigureTextBox(this._databaseTextBox);
            ConfigureHintLabel(this._databaseHintLabel, "Nome do banco");
            ConfigureSeparator(this._separator3);
            ConfigureSectionLabel(this._sectionAuthLabel, "Autenticacao");
            ConfigureFieldLabel(this._userLabel, "Usuario:");
            ConfigureTextBox(this._userTextBox);
            ConfigureHintLabel(this._userHintLabel, "Usuario PostgreSQL");
            ConfigureFieldLabel(this._passwordLabel, "Senha:");
            ConfigureTextBox(this._passwordTextBox);
            this._passwordTextBox.UseSystemPasswordChar = true;
            ConfigureHintLabel(this._passwordHintLabel, "Senha do usuario");
            //
            // _statusLabel
            //
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._statusLabel.ForeColor = Color.SeaGreen;
            this._statusLabel.Margin = new Padding(0, 10, 0, 5);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Text = string.Empty;
            //
            // _buttonsPanel
            //
            this._buttonsPanel.ColumnCount = 2;
            this._buttonsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._buttonsPanel.ColumnStyles.Add(new ColumnStyle());
            this._buttonsPanel.Controls.Add(this._leftButtonsPanel, 0, 0);
            this._buttonsPanel.Controls.Add(this._cancelButton, 1, 0);
            this._buttonsPanel.Dock = DockStyle.Fill;
            this._buttonsPanel.Margin = new Padding(0, 5, 0, 0);
            this._buttonsPanel.Name = "_buttonsPanel";
            this._buttonsPanel.RowCount = 1;
            this._buttonsPanel.RowStyles.Add(new RowStyle());
            //
            this._leftButtonsPanel.AutoSize = true;
            this._leftButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this._leftButtonsPanel.Controls.Add(this._saveButton);
            this._leftButtonsPanel.Controls.Add(this._testButton);
            this._leftButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
            this._leftButtonsPanel.Margin = new Padding(0);
            this._leftButtonsPanel.Name = "_leftButtonsPanel";
            //
            this._saveButton.FlatStyle = FlatStyle.System;
            this._saveButton.Margin = new Padding(0, 0, 5, 0);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new Size(100, 28);
            this._saveButton.TabIndex = 0;
            this._saveButton.Text = "Salvar";
            this._saveButton.UseVisualStyleBackColor = true;
            //
            this._testButton.FlatStyle = FlatStyle.System;
            this._testButton.Margin = new Padding(5, 0, 0, 0);
            this._testButton.Name = "_testButton";
            this._testButton.Size = new Size(130, 28);
            this._testButton.TabIndex = 1;
            this._testButton.Text = "Testar Conexao";
            this._testButton.UseVisualStyleBackColor = true;
            //
            this._cancelButton.Anchor = AnchorStyles.Right;
            this._cancelButton.FlatStyle = FlatStyle.System;
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new Size(100, 28);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancelar";
            this._cancelButton.UseVisualStyleBackColor = true;
            //
            // DatabaseProfileEditorForm
            //
            this.AcceptButton = this._saveButton;
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.CancelButton = this._cancelButton;
            this.ClientSize = new Size(550, 700);
            this.Controls.Add(this._rootLayout);
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DatabaseProfileEditorForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Novo Banco";
            this._rootLayout.ResumeLayout(false);
            this._rootLayout.PerformLayout();
            this._formGroup.ResumeLayout(false);
            this._formLayout.ResumeLayout(false);
            this._formLayout.PerformLayout();
            ((ISupportInitialize)(this._portNumericUpDown)).EndInit();
            this._buttonsPanel.ResumeLayout(false);
            this._buttonsPanel.PerformLayout();
            this._leftButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private static void ConfigureSectionLabel(Label label, string text)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(27, 54, 93);
            label.Margin = new Padding(0, 5, 0, 3);
            label.Text = text;
        }

        private static void ConfigureFieldLabel(Label label, string text)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9F);
            label.Margin = new Padding(0, 8, 10, 0);
            label.Text = text;
        }

        private static void ConfigureTextBox(TextBox textBox)
        {
            textBox.Dock = DockStyle.Top;
            textBox.Font = new Font("Segoe UI", 9F);
            textBox.Margin = new Padding(0, 5, 0, 0);
        }

        private static void ConfigureHintLabel(Label label, string text)
        {
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 8F);
            label.ForeColor = Color.Gray;
            label.Margin = new Padding(0, 0, 0, 5);
            label.Text = text;
        }

        private static void ConfigureSeparator(Label separator)
        {
            separator.AutoSize = false;
            separator.BackColor = Color.LightGray;
            separator.Dock = DockStyle.Top;
            separator.Height = 1;
            separator.Margin = new Padding(0, 8, 0, 8);
            separator.Text = string.Empty;
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private Label _headerLabel;
        private GroupBox _formGroup;
        private TableLayoutPanel _formLayout;
        private Label _sectionIdLabel;
        private Label _nameLabel;
        private TextBox _nameTextBox;
        private Label _nameHintLabel;
        private Label _descriptionLabel;
        private TextBox _descriptionTextBox;
        private Label _descriptionHintLabel;
        private Label _separator1;
        private Label _sectionServerLabel;
        private Label _hostLabel;
        private TextBox _hostTextBox;
        private Label _hostHintLabel;
        private Label _portLabel;
        private NumericUpDown _portNumericUpDown;
        private Label _portHintLabel;
        private Label _separator2;
        private Label _sectionDbLabel;
        private Label _databaseLabel;
        private TextBox _databaseTextBox;
        private Label _databaseHintLabel;
        private Label _separator3;
        private Label _sectionAuthLabel;
        private Label _userLabel;
        private TextBox _userTextBox;
        private Label _userHintLabel;
        private Label _passwordLabel;
        private TextBox _passwordTextBox;
        private Label _passwordHintLabel;
        private Label _statusLabel;
        private TableLayoutPanel _buttonsPanel;
        private FlowLayoutPanel _leftButtonsPanel;
        private Button _saveButton;
        private Button _testButton;
        private Button _cancelButton;
    }
}
