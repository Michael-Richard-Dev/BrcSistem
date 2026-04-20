using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class DatabaseProfilesForm
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
            this._leftPanel = new TableLayoutPanel();
            this._profilesLabel = new Label();
            this._profilesListBox = new ListBox();
            this._listButtonsPanel = new FlowLayoutPanel();
            this._newButton = new Button();
            this._deleteButton = new Button();
            this._activateButton = new Button();
            this._rightPanel = new TableLayoutPanel();
            this._nameLabel = new Label();
            this._nameTextBox = new TextBox();
            this._descriptionLabel = new Label();
            this._descriptionTextBox = new TextBox();
            this._hostLabel = new Label();
            this._hostTextBox = new TextBox();
            this._portLabel = new Label();
            this._portNumericUpDown = new NumericUpDown();
            this._databaseLabel = new Label();
            this._databaseTextBox = new TextBox();
            this._userFormLabel = new Label();
            this._userTextBox = new TextBox();
            this._passwordFormLabel = new Label();
            this._passwordTextBox = new TextBox();
            this._statusLabel = new Label();
            this._actionsPanel = new FlowLayoutPanel();
            this._saveButton = new Button();
            this._testButton = new Button();
            this._closeButton = new Button();
            this._rootLayout.SuspendLayout();
            this._leftPanel.SuspendLayout();
            this._listButtonsPanel.SuspendLayout();
            this._rightPanel.SuspendLayout();
            ((ISupportInitialize)(this._portNumericUpDown)).BeginInit();
            this._actionsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootLayout
            // 
            this._rootLayout.ColumnCount = 2;
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
            this._rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._leftPanel, 0, 0);
            this._rootLayout.Controls.Add(this._rightPanel, 1, 0);
            this._rootLayout.Dock = DockStyle.Fill;
            this._rootLayout.Location = new Point(0, 0);
            this._rootLayout.Name = "_rootLayout";
            this._rootLayout.Padding = new Padding(12);
            this._rootLayout.RowCount = 1;
            this._rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._rootLayout.Size = new Size(884, 521);
            this._rootLayout.TabIndex = 0;
            // 
            // _leftPanel
            // 
            this._leftPanel.ColumnCount = 1;
            this._leftPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._leftPanel.Controls.Add(this._profilesLabel, 0, 0);
            this._leftPanel.Controls.Add(this._profilesListBox, 0, 1);
            this._leftPanel.Controls.Add(this._listButtonsPanel, 0, 2);
            this._leftPanel.Dock = DockStyle.Fill;
            this._leftPanel.Location = new Point(15, 15);
            this._leftPanel.Name = "_leftPanel";
            this._leftPanel.RowCount = 3;
            this._leftPanel.RowStyles.Add(new RowStyle());
            this._leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this._leftPanel.RowStyles.Add(new RowStyle());
            this._leftPanel.Size = new Size(254, 491);
            this._leftPanel.TabIndex = 0;
            // 
            // _profilesLabel
            // 
            this._profilesLabel.AutoSize = true;
            this._profilesLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this._profilesLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._profilesLabel.Location = new Point(0, 0);
            this._profilesLabel.Margin = new Padding(0, 0, 0, 8);
            this._profilesLabel.Name = "_profilesLabel";
            this._profilesLabel.Size = new Size(126, 19);
            this._profilesLabel.TabIndex = 0;
            this._profilesLabel.Text = "Perfis configurados";
            // 
            // _profilesListBox
            // 
            this._profilesListBox.Dock = DockStyle.Fill;
            this._profilesListBox.Font = new Font("Segoe UI", 10F);
            this._profilesListBox.FormattingEnabled = true;
            this._profilesListBox.ItemHeight = 17;
            this._profilesListBox.Location = new Point(3, 30);
            this._profilesListBox.Name = "_profilesListBox";
            this._profilesListBox.Size = new Size(248, 423);
            this._profilesListBox.TabIndex = 1;
            // 
            // _listButtonsPanel
            // 
            this._listButtonsPanel.AutoSize = true;
            this._listButtonsPanel.Controls.Add(this._newButton);
            this._listButtonsPanel.Controls.Add(this._deleteButton);
            this._listButtonsPanel.Controls.Add(this._activateButton);
            this._listButtonsPanel.Dock = DockStyle.Fill;
            this._listButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
            this._listButtonsPanel.Location = new Point(0, 456);
            this._listButtonsPanel.Margin = new Padding(0);
            this._listButtonsPanel.Name = "_listButtonsPanel";
            this._listButtonsPanel.Size = new Size(254, 35);
            this._listButtonsPanel.TabIndex = 2;
            // 
            // _newButton
            // 
            this._newButton.AutoSize = true;
            this._newButton.FlatStyle = FlatStyle.System;
            this._newButton.Location = new Point(3, 3);
            this._newButton.Name = "_newButton";
            this._newButton.Size = new Size(52, 25);
            this._newButton.TabIndex = 0;
            this._newButton.Text = "Novo";
            this._newButton.UseVisualStyleBackColor = true;
            // 
            // _deleteButton
            // 
            this._deleteButton.AutoSize = true;
            this._deleteButton.FlatStyle = FlatStyle.System;
            this._deleteButton.Location = new Point(61, 3);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new Size(56, 25);
            this._deleteButton.TabIndex = 1;
            this._deleteButton.Text = "Excluir";
            this._deleteButton.UseVisualStyleBackColor = true;
            // 
            // _activateButton
            // 
            this._activateButton.AutoSize = true;
            this._activateButton.FlatStyle = FlatStyle.System;
            this._activateButton.Location = new Point(123, 3);
            this._activateButton.Name = "_activateButton";
            this._activateButton.Size = new Size(86, 25);
            this._activateButton.TabIndex = 2;
            this._activateButton.Text = "Definir Ativo";
            this._activateButton.UseVisualStyleBackColor = true;
            // 
            // _rightPanel
            // 
            this._rightPanel.AutoScroll = true;
            this._rightPanel.ColumnCount = 2;
            this._rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            this._rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this._rightPanel.Controls.Add(this._nameLabel, 0, 0);
            this._rightPanel.Controls.Add(this._nameTextBox, 1, 0);
            this._rightPanel.Controls.Add(this._descriptionLabel, 0, 1);
            this._rightPanel.Controls.Add(this._descriptionTextBox, 1, 1);
            this._rightPanel.Controls.Add(this._hostLabel, 0, 2);
            this._rightPanel.Controls.Add(this._hostTextBox, 1, 2);
            this._rightPanel.Controls.Add(this._portLabel, 0, 3);
            this._rightPanel.Controls.Add(this._portNumericUpDown, 1, 3);
            this._rightPanel.Controls.Add(this._databaseLabel, 0, 4);
            this._rightPanel.Controls.Add(this._databaseTextBox, 1, 4);
            this._rightPanel.Controls.Add(this._userFormLabel, 0, 5);
            this._rightPanel.Controls.Add(this._userTextBox, 1, 5);
            this._rightPanel.Controls.Add(this._passwordFormLabel, 0, 6);
            this._rightPanel.Controls.Add(this._passwordTextBox, 1, 6);
            this._rightPanel.Controls.Add(this._statusLabel, 0, 7);
            this._rightPanel.Controls.Add(this._actionsPanel, 0, 8);
            this._rightPanel.Dock = DockStyle.Fill;
            this._rightPanel.Location = new Point(275, 15);
            this._rightPanel.Name = "_rightPanel";
            this._rightPanel.RowCount = 9;
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.RowStyles.Add(new RowStyle());
            this._rightPanel.Size = new Size(594, 491);
            this._rightPanel.TabIndex = 1;
            // 
            // _nameLabel
            // 
            this._nameLabel.AutoSize = true;
            this._nameLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this._nameLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._nameLabel.Location = new Point(0, 8);
            this._nameLabel.Margin = new Padding(0, 8, 0, 4);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new Size(42, 17);
            this._nameLabel.TabIndex = 0;
            this._nameLabel.Text = "Nome";
            // 
            // _nameTextBox
            // 
            this._nameTextBox.Dock = DockStyle.Top;
            this._nameTextBox.Font = new Font("Segoe UI", 10F);
            this._nameTextBox.Location = new Point(173, 3);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new Size(418, 25);
            this._nameTextBox.TabIndex = 1;
            // 
            // _descriptionLabel
            // 
            this._descriptionLabel.AutoSize = true;
            this._descriptionLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this._descriptionLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._descriptionLabel.Location = new Point(0, 39);
            this._descriptionLabel.Margin = new Padding(0, 8, 0, 4);
            this._descriptionLabel.Name = "_descriptionLabel";
            this._descriptionLabel.Size = new Size(69, 17);
            this._descriptionLabel.TabIndex = 2;
            this._descriptionLabel.Text = "Descricao";
            // 
            // _descriptionTextBox
            // 
            this._descriptionTextBox.Dock = DockStyle.Top;
            this._descriptionTextBox.Font = new Font("Segoe UI", 10F);
            this._descriptionTextBox.Location = new Point(173, 34);
            this._descriptionTextBox.Name = "_descriptionTextBox";
            this._descriptionTextBox.Size = new Size(418, 25);
            this._descriptionTextBox.TabIndex = 3;
            // 
            // _hostLabel
            // 
            this._hostLabel.AutoSize = true;
            this._hostLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this._hostLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._hostLabel.Location = new Point(0, 70);
            this._hostLabel.Margin = new Padding(0, 8, 0, 4);
            this._hostLabel.Name = "_hostLabel";
            this._hostLabel.Size = new Size(35, 17);
            this._hostLabel.TabIndex = 4;
            this._hostLabel.Text = "Host";
            // 
            // _hostTextBox
            // 
            this._hostTextBox.Dock = DockStyle.Top;
            this._hostTextBox.Font = new Font("Segoe UI", 10F);
            this._hostTextBox.Location = new Point(173, 65);
            this._hostTextBox.Name = "_hostTextBox";
            this._hostTextBox.Size = new Size(418, 25);
            this._hostTextBox.TabIndex = 5;
            // 
            // _portLabel
            // 
            this._portLabel.AutoSize = true;
            this._portLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this._portLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._portLabel.Location = new Point(0, 101);
            this._portLabel.Margin = new Padding(0, 8, 0, 4);
            this._portLabel.Name = "_portLabel";
            this._portLabel.Size = new Size(39, 17);
            this._portLabel.TabIndex = 6;
            this._portLabel.Text = "Porta";
            // 
            // _portNumericUpDown
            // 
            this._portNumericUpDown.Dock = DockStyle.Top;
            this._portNumericUpDown.Font = new Font("Segoe UI", 10F);
            this._portNumericUpDown.Location = new Point(173, 96);
            this._portNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._portNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._portNumericUpDown.Name = "_portNumericUpDown";
            this._portNumericUpDown.Size = new Size(140, 25);
            this._portNumericUpDown.TabIndex = 7;
            this._portNumericUpDown.Value = new decimal(new int[] {
            5432,
            0,
            0,
            0});
            // 
            // _databaseLabel
            // 
            this._databaseLabel.AutoSize = true;
            this._databaseLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this._databaseLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._databaseLabel.Location = new Point(0, 132);
            this._databaseLabel.Margin = new Padding(0, 8, 0, 4);
            this._databaseLabel.Name = "_databaseLabel";
            this._databaseLabel.Size = new Size(63, 17);
            this._databaseLabel.TabIndex = 8;
            this._databaseLabel.Text = "Database";
            // 
            // _databaseTextBox
            // 
            this._databaseTextBox.Dock = DockStyle.Top;
            this._databaseTextBox.Font = new Font("Segoe UI", 10F);
            this._databaseTextBox.Location = new Point(173, 127);
            this._databaseTextBox.Name = "_databaseTextBox";
            this._databaseTextBox.Size = new Size(418, 25);
            this._databaseTextBox.TabIndex = 9;
            // 
            // _userFormLabel
            // 
            this._userFormLabel.AutoSize = true;
            this._userFormLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this._userFormLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._userFormLabel.Location = new Point(0, 163);
            this._userFormLabel.Margin = new Padding(0, 8, 0, 4);
            this._userFormLabel.Name = "_userFormLabel";
            this._userFormLabel.Size = new Size(53, 17);
            this._userFormLabel.TabIndex = 10;
            this._userFormLabel.Text = "Usuario";
            // 
            // _userTextBox
            // 
            this._userTextBox.Dock = DockStyle.Top;
            this._userTextBox.Font = new Font("Segoe UI", 10F);
            this._userTextBox.Location = new Point(173, 158);
            this._userTextBox.Name = "_userTextBox";
            this._userTextBox.Size = new Size(418, 25);
            this._userTextBox.TabIndex = 11;
            // 
            // _passwordFormLabel
            // 
            this._passwordFormLabel.AutoSize = true;
            this._passwordFormLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            this._passwordFormLabel.ForeColor = Color.FromArgb(27, 54, 93);
            this._passwordFormLabel.Location = new Point(0, 194);
            this._passwordFormLabel.Margin = new Padding(0, 8, 0, 4);
            this._passwordFormLabel.Name = "_passwordFormLabel";
            this._passwordFormLabel.Size = new Size(45, 17);
            this._passwordFormLabel.TabIndex = 12;
            this._passwordFormLabel.Text = "Senha";
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.Dock = DockStyle.Top;
            this._passwordTextBox.Font = new Font("Segoe UI", 10F);
            this._passwordTextBox.Location = new Point(173, 189);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.Size = new Size(418, 25);
            this._passwordTextBox.TabIndex = 13;
            this._passwordTextBox.UseSystemPasswordChar = true;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this._statusLabel.ForeColor = Color.Firebrick;
            this._statusLabel.Location = new Point(0, 235);
            this._statusLabel.Margin = new Padding(0, 16, 0, 12);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new Size(0, 15);
            this._statusLabel.TabIndex = 14;
            // 
            // _actionsPanel
            // 
            this._actionsPanel.AutoSize = true;
            this._actionsPanel.Controls.Add(this._saveButton);
            this._actionsPanel.Controls.Add(this._testButton);
            this._actionsPanel.Controls.Add(this._closeButton);
            this._actionsPanel.Dock = DockStyle.Top;
            this._actionsPanel.FlowDirection = FlowDirection.LeftToRight;
            this._actionsPanel.Location = new Point(0, 262);
            this._actionsPanel.Margin = new Padding(0, 10, 0, 0);
            this._actionsPanel.Name = "_actionsPanel";
            this._actionsPanel.Size = new Size(594, 31);
            this._actionsPanel.TabIndex = 15;
            // 
            // _saveButton
            // 
            this._saveButton.AutoSize = true;
            this._saveButton.FlatStyle = FlatStyle.System;
            this._saveButton.Location = new Point(3, 3);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new Size(54, 25);
            this._saveButton.TabIndex = 0;
            this._saveButton.Text = "Salvar";
            this._saveButton.UseVisualStyleBackColor = true;
            // 
            // _testButton
            // 
            this._testButton.AutoSize = true;
            this._testButton.FlatStyle = FlatStyle.System;
            this._testButton.Location = new Point(63, 3);
            this._testButton.Name = "_testButton";
            this._testButton.Size = new Size(98, 25);
            this._testButton.TabIndex = 1;
            this._testButton.Text = "Testar Conexao";
            this._testButton.UseVisualStyleBackColor = true;
            // 
            // _closeButton
            // 
            this._closeButton.AutoSize = true;
            this._closeButton.FlatStyle = FlatStyle.System;
            this._closeButton.Location = new Point(167, 3);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new Size(56, 25);
            this._closeButton.TabIndex = 2;
            this._closeButton.Text = "Fechar";
            this._closeButton.UseVisualStyleBackColor = true;
            // 
            // DatabaseProfilesForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(884, 521);
            this.Controls.Add(this._rootLayout);
            this.MinimumSize = new Size(900, 560);
            this.Name = "DatabaseProfilesForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "BRCSISTEM - Gerenciar Bancos";
            this._rootLayout.ResumeLayout(false);
            this._leftPanel.ResumeLayout(false);
            this._leftPanel.PerformLayout();
            this._listButtonsPanel.ResumeLayout(false);
            this._listButtonsPanel.PerformLayout();
            this._rightPanel.ResumeLayout(false);
            this._rightPanel.PerformLayout();
            ((ISupportInitialize)(this._portNumericUpDown)).EndInit();
            this._actionsPanel.ResumeLayout(false);
            this._actionsPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _rootLayout;
        private TableLayoutPanel _leftPanel;
        private Label _profilesLabel;
        private ListBox _profilesListBox;
        private FlowLayoutPanel _listButtonsPanel;
        private Button _newButton;
        private Button _deleteButton;
        private Button _activateButton;
        private TableLayoutPanel _rightPanel;
        private Label _nameLabel;
        private TextBox _nameTextBox;
        private Label _descriptionLabel;
        private TextBox _descriptionTextBox;
        private Label _hostLabel;
        private TextBox _hostTextBox;
        private Label _portLabel;
        private NumericUpDown _portNumericUpDown;
        private Label _databaseLabel;
        private TextBox _databaseTextBox;
        private Label _userFormLabel;
        private TextBox _userTextBox;
        private Label _passwordFormLabel;
        private TextBox _passwordTextBox;
        private Label _statusLabel;
        private FlowLayoutPanel _actionsPanel;
        private Button _saveButton;
        private Button _testButton;
        private Button _closeButton;
    }
}
