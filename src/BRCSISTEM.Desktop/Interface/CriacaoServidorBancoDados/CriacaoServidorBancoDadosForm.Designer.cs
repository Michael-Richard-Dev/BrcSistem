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
            this._rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this._serverGroup = new System.Windows.Forms.GroupBox();
            this._serverLayout = new System.Windows.Forms.TableLayoutPanel();
            this._hostLabel = new System.Windows.Forms.Label();
            this._hostTextBox = new System.Windows.Forms.TextBox();
            this._hostHintLabel = new System.Windows.Forms.Label();
            this._portLabel = new System.Windows.Forms.Label();
            this._portTextBox = new System.Windows.Forms.TextBox();
            this._portHintLabel = new System.Windows.Forms.Label();
            this._adminUserLabel = new System.Windows.Forms.Label();
            this._adminUserTextBox = new System.Windows.Forms.TextBox();
            this._adminUserHintLabel = new System.Windows.Forms.Label();
            this._adminPasswordLabel = new System.Windows.Forms.Label();
            this._adminPasswordTextBox = new System.Windows.Forms.TextBox();
            this._adminPasswordHintLabel = new System.Windows.Forms.Label();
            this._databaseGroup = new System.Windows.Forms.GroupBox();
            this._databaseLayout = new System.Windows.Forms.TableLayoutPanel();
            this._databaseNameLabel = new System.Windows.Forms.Label();
            this._databaseNameTextBox = new System.Windows.Forms.TextBox();
            this._databaseHintLabel = new System.Windows.Forms.Label();
            this._addToListCheckBox = new System.Windows.Forms.CheckBox();
            this._statusLabel = new System.Windows.Forms.Label();
            this._buttonsLayout = new System.Windows.Forms.TableLayoutPanel();
            this._createButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
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
            this._rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._rootLayout.Controls.Add(this._titleLabel, 0, 0);
            this._rootLayout.Controls.Add(this._subtitleLabel, 0, 1);
            this._rootLayout.Controls.Add(this._serverGroup, 0, 2);
            this._rootLayout.Controls.Add(this._databaseGroup, 0, 3);
            this._rootLayout.Controls.Add(this._statusLabel, 0, 4);
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
            this._rootLayout.Size = new System.Drawing.Size(650, 520);
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
            this._titleLabel.Size = new System.Drawing.Size(236, 21);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Criar Novo Banco no Servidor";
            // 
            // _subtitleLabel
            // 
            this._subtitleLabel.AutoSize = true;
            this._subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._subtitleLabel.ForeColor = System.Drawing.Color.Gray;
            this._subtitleLabel.Location = new System.Drawing.Point(20, 44);
            this._subtitleLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this._subtitleLabel.Name = "_subtitleLabel";
            this._subtitleLabel.Size = new System.Drawing.Size(257, 13);
            this._subtitleLabel.TabIndex = 1;
            this._subtitleLabel.Text = "Cria um banco vazio no PostgreSQL (sem tabelas)";
            // 
            // _serverGroup
            // 
            this._serverGroup.Controls.Add(this._serverLayout);
            this._serverGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._serverGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._serverGroup.Location = new System.Drawing.Point(20, 72);
            this._serverGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._serverGroup.Name = "_serverGroup";
            this._serverGroup.Padding = new System.Windows.Forms.Padding(10);
            this._serverGroup.Size = new System.Drawing.Size(610, 207);
            this._serverGroup.TabIndex = 2;
            this._serverGroup.TabStop = false;
            this._serverGroup.Text = " Servidor PostgreSQL ";
            // 
            // _serverLayout
            // 
            this._serverLayout.ColumnCount = 2;
            this._serverLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this._serverLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
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
            this._serverLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._serverLayout.Location = new System.Drawing.Point(10, 26);
            this._serverLayout.Name = "_serverLayout";
            this._serverLayout.RowCount = 8;
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._serverLayout.Size = new System.Drawing.Size(590, 171);
            this._serverLayout.TabIndex = 0;
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
            this._hostTextBox.Location = new System.Drawing.Point(120, 3);
            this._hostTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._hostTextBox.Name = "_hostTextBox";
            this._hostTextBox.Size = new System.Drawing.Size(470, 22);
            this._hostTextBox.TabIndex = 1;
            // 
            // _hostHintLabel
            // 
            this._hostHintLabel.AutoSize = true;
            this._hostHintLabel.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._hostHintLabel.ForeColor = System.Drawing.Color.Gray;
            this._hostHintLabel.Location = new System.Drawing.Point(120, 25);
            this._hostHintLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._hostHintLabel.Name = "_hostHintLabel";
            this._hostHintLabel.Size = new System.Drawing.Size(164, 12);
            this._hostHintLabel.TabIndex = 2;
            this._hostHintLabel.Text = "localhost, 127.0.0.1 ou IP do servidor";
            // 
            // _portLabel
            // 
            this._portLabel.AutoSize = true;
            this._portLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._portLabel.Location = new System.Drawing.Point(0, 48);
            this._portLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this._portLabel.Name = "_portLabel";
            this._portLabel.Size = new System.Drawing.Size(37, 13);
            this._portLabel.TabIndex = 3;
            this._portLabel.Text = "Porta:";
            // 
            // _portTextBox
            // 
            this._portTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._portTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._portTextBox.Location = new System.Drawing.Point(120, 43);
            this._portTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._portTextBox.Name = "_portTextBox";
            this._portTextBox.Size = new System.Drawing.Size(470, 22);
            this._portTextBox.TabIndex = 4;
            // 
            // _portHintLabel
            // 
            this._portHintLabel.AutoSize = true;
            this._portHintLabel.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._portHintLabel.ForeColor = System.Drawing.Color.Gray;
            this._portHintLabel.Location = new System.Drawing.Point(120, 65);
            this._portHintLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._portHintLabel.Name = "_portHintLabel";
            this._portHintLabel.Size = new System.Drawing.Size(61, 12);
            this._portHintLabel.TabIndex = 5;
            this._portHintLabel.Text = "Padrao: 5432";
            // 
            // _adminUserLabel
            // 
            this._adminUserLabel.AutoSize = true;
            this._adminUserLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._adminUserLabel.Location = new System.Drawing.Point(0, 88);
            this._adminUserLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this._adminUserLabel.Name = "_adminUserLabel";
            this._adminUserLabel.Size = new System.Drawing.Size(86, 13);
            this._adminUserLabel.TabIndex = 6;
            this._adminUserLabel.Text = "Usuario Admin:";
            // 
            // _adminUserTextBox
            // 
            this._adminUserTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._adminUserTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._adminUserTextBox.Location = new System.Drawing.Point(120, 83);
            this._adminUserTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._adminUserTextBox.Name = "_adminUserTextBox";
            this._adminUserTextBox.Size = new System.Drawing.Size(470, 22);
            this._adminUserTextBox.TabIndex = 7;
            // 
            // _adminUserHintLabel
            // 
            this._adminUserHintLabel.AutoSize = true;
            this._adminUserHintLabel.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._adminUserHintLabel.ForeColor = System.Drawing.Color.Gray;
            this._adminUserHintLabel.Location = new System.Drawing.Point(120, 105);
            this._adminUserHintLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._adminUserHintLabel.Name = "_adminUserHintLabel";
            this._adminUserHintLabel.Size = new System.Drawing.Size(101, 12);
            this._adminUserHintLabel.TabIndex = 8;
            this._adminUserHintLabel.Text = "Usuario administrador";
            // 
            // _adminPasswordLabel
            // 
            this._adminPasswordLabel.AutoSize = true;
            this._adminPasswordLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._adminPasswordLabel.Location = new System.Drawing.Point(0, 128);
            this._adminPasswordLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this._adminPasswordLabel.Name = "_adminPasswordLabel";
            this._adminPasswordLabel.Size = new System.Drawing.Size(42, 13);
            this._adminPasswordLabel.TabIndex = 9;
            this._adminPasswordLabel.Text = "Senha:";
            // 
            // _adminPasswordTextBox
            // 
            this._adminPasswordTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._adminPasswordTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._adminPasswordTextBox.Location = new System.Drawing.Point(120, 123);
            this._adminPasswordTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._adminPasswordTextBox.Name = "_adminPasswordTextBox";
            this._adminPasswordTextBox.Size = new System.Drawing.Size(470, 22);
            this._adminPasswordTextBox.TabIndex = 10;
            this._adminPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // _adminPasswordHintLabel
            // 
            this._adminPasswordHintLabel.AutoSize = true;
            this._adminPasswordHintLabel.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._adminPasswordHintLabel.ForeColor = System.Drawing.Color.Gray;
            this._adminPasswordHintLabel.Location = new System.Drawing.Point(120, 145);
            this._adminPasswordHintLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this._adminPasswordHintLabel.Name = "_adminPasswordHintLabel";
            this._adminPasswordHintLabel.Size = new System.Drawing.Size(110, 12);
            this._adminPasswordHintLabel.TabIndex = 11;
            this._adminPasswordHintLabel.Text = "Senha do administrador";
            // 
            // _databaseGroup
            // 
            this._databaseGroup.Controls.Add(this._databaseLayout);
            this._databaseGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._databaseGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._databaseGroup.Location = new System.Drawing.Point(20, 289);
            this._databaseGroup.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this._databaseGroup.Name = "_databaseGroup";
            this._databaseGroup.Padding = new System.Windows.Forms.Padding(10);
            this._databaseGroup.Size = new System.Drawing.Size(610, 108);
            this._databaseGroup.TabIndex = 3;
            this._databaseGroup.TabStop = false;
            this._databaseGroup.Text = " Novo Banco ";
            // 
            // _databaseLayout
            // 
            this._databaseLayout.ColumnCount = 2;
            this._databaseLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this._databaseLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._databaseLayout.Controls.Add(this._databaseNameLabel, 0, 0);
            this._databaseLayout.Controls.Add(this._databaseNameTextBox, 1, 0);
            this._databaseLayout.Controls.Add(this._databaseHintLabel, 1, 1);
            this._databaseLayout.Controls.Add(this._addToListCheckBox, 0, 2);
            this._databaseLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._databaseLayout.Location = new System.Drawing.Point(10, 26);
            this._databaseLayout.Name = "_databaseLayout";
            this._databaseLayout.RowCount = 3;
            this._databaseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._databaseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._databaseLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._databaseLayout.Size = new System.Drawing.Size(590, 72);
            this._databaseLayout.TabIndex = 0;
            // 
            // _databaseNameLabel
            // 
            this._databaseNameLabel.AutoSize = true;
            this._databaseNameLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._databaseNameLabel.Location = new System.Drawing.Point(0, 8);
            this._databaseNameLabel.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this._databaseNameLabel.Name = "_databaseNameLabel";
            this._databaseNameLabel.Size = new System.Drawing.Size(92, 13);
            this._databaseNameLabel.TabIndex = 0;
            this._databaseNameLabel.Text = "Nome do Banco:";
            // 
            // _databaseNameTextBox
            // 
            this._databaseNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._databaseNameTextBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._databaseNameTextBox.Location = new System.Drawing.Point(120, 3);
            this._databaseNameTextBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._databaseNameTextBox.Name = "_databaseNameTextBox";
            this._databaseNameTextBox.Size = new System.Drawing.Size(470, 22);
            this._databaseNameTextBox.TabIndex = 1;
            // 
            // _databaseHintLabel
            // 
            this._databaseHintLabel.AutoSize = true;
            this._databaseHintLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._databaseHintLabel.ForeColor = System.Drawing.Color.Gray;
            this._databaseHintLabel.Location = new System.Drawing.Point(120, 25);
            this._databaseHintLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this._databaseHintLabel.Name = "_databaseHintLabel";
            this._databaseHintLabel.Size = new System.Drawing.Size(344, 13);
            this._databaseHintLabel.TabIndex = 2;
            this._databaseHintLabel.Text = "Somente letras, numeros e underscore (_). Ex: brcsistem_producao";
            // 
            // _addToListCheckBox
            // 
            this._addToListCheckBox.AutoSize = true;
            this._databaseLayout.SetColumnSpan(this._addToListCheckBox, 2);
            this._addToListCheckBox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._addToListCheckBox.Location = new System.Drawing.Point(0, 54);
            this._addToListCheckBox.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this._addToListCheckBox.Name = "_addToListCheckBox";
            this._addToListCheckBox.Size = new System.Drawing.Size(161, 17);
            this._addToListCheckBox.TabIndex = 3;
            this._addToListCheckBox.Text = "Adicionar a lista apos criar";
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._statusLabel.ForeColor = System.Drawing.Color.SeaGreen;
            this._statusLabel.Location = new System.Drawing.Point(20, 415);
            this._statusLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(0, 1);
            this._statusLabel.TabIndex = 4;
            // 
            // _buttonsLayout
            // 
            this._buttonsLayout.ColumnCount = 3;
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._buttonsLayout.Controls.Add(this._createButton, 0, 0);
            this._buttonsLayout.Controls.Add(this._cancelButton, 2, 0);
            this._buttonsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this._buttonsLayout.Location = new System.Drawing.Point(20, 400);
            this._buttonsLayout.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this._buttonsLayout.Name = "_buttonsLayout";
            this._buttonsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this._buttonsLayout.Size = new System.Drawing.Size(610, 100);
            this._buttonsLayout.TabIndex = 5;
            // 
            // _createButton
            // 
            this._createButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._createButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._createButton.Location = new System.Drawing.Point(3, 3);
            this._createButton.Name = "_createButton";
            this._createButton.Size = new System.Drawing.Size(130, 28);
            this._createButton.TabIndex = 0;
            this._createButton.Text = "Criar Banco";
            this._createButton.UseVisualStyleBackColor = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._cancelButton.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cancelButton.Location = new System.Drawing.Point(497, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(110, 28);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancelar";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // CriacaoServidorBancoDadosForm
            // 
            this.AcceptButton = this._createButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(650, 520);
            this.Controls.Add(this._rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CriacaoServidorBancoDadosForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
