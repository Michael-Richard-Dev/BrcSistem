using System;
using System.Drawing;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    public sealed class ChangePasswordForm : Form
    {
        private readonly AuthenticationController _authenticationController;
        private readonly AppConfiguration _configuration;
        private readonly DatabaseProfile _databaseProfile;
        private readonly string _userName;
        private readonly bool _forceReset;

        private TextBox _newPasswordTextBox;
        private TextBox _confirmPasswordTextBox;
        private Label _statusLabel;

        public ChangePasswordForm(CompositionRoot compositionRoot, AppConfiguration configuration, DatabaseProfile databaseProfile, string userName, bool forceReset)
        {
            _authenticationController = compositionRoot.CreateAuthenticationController();
            _configuration = configuration;
            _databaseProfile = databaseProfile;
            _userName = userName;
            _forceReset = forceReset;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Alterar Senha";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(520, 320);
            MinimumSize = new Size(520, 320);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(20),
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var titleLabel = new Label
            {
                AutoSize = true,
                Text = _forceReset
                    ? "Senha padrao detectada. Defina uma nova senha para continuar."
                    : "Altere a senha do usuario atual.",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = _forceReset ? Color.DarkOrange : Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 0, 0, 16),
            };
            layout.Controls.Add(titleLabel, 0, 0);
            layout.SetColumnSpan(titleLabel, 2);

            layout.Controls.Add(CreateFieldLabel("Usuario"), 0, 1);
            layout.Controls.Add(new Label
            {
                AutoSize = true,
                Text = _userName,
                Font = new Font("Segoe UI", 10F),
                Margin = new Padding(0, 8, 0, 8),
            }, 1, 1);

            layout.Controls.Add(CreateFieldLabel("Nova senha"), 0, 2);
            _newPasswordTextBox = new TextBox
            {
                UseSystemPasswordChar = true,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10F),
            };
            layout.Controls.Add(_newPasswordTextBox, 1, 2);

            layout.Controls.Add(CreateFieldLabel("Confirmacao"), 0, 3);
            _confirmPasswordTextBox = new TextBox
            {
                UseSystemPasswordChar = true,
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 10F),
            };
            layout.Controls.Add(_confirmPasswordTextBox, 1, 3);

            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.Firebrick,
                Margin = new Padding(0, 16, 0, 12),
            };
            layout.Controls.Add(_statusLabel, 0, 4);
            layout.SetColumnSpan(_statusLabel, 2);

            var buttons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
            };
            var saveButton = new Button { Text = "Salvar", AutoSize = true, FlatStyle = FlatStyle.System };
            saveButton.Click += SavePassword;
            var cancelButton = new Button { Text = _forceReset ? "Cancelar" : "Fechar", AutoSize = true, FlatStyle = FlatStyle.System };
            cancelButton.Click += (sender, args) => Close();
            buttons.Controls.Add(saveButton);
            buttons.Controls.Add(cancelButton);
            layout.Controls.Add(buttons, 0, 5);
            layout.SetColumnSpan(buttons, 2);

            Controls.Add(layout);
        }

        private void SavePassword(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_newPasswordTextBox.Text) || string.IsNullOrWhiteSpace(_confirmPasswordTextBox.Text))
            {
                SetStatus("Preencha a nova senha e a confirmacao.", true);
                return;
            }

            if (!string.Equals(_newPasswordTextBox.Text, _confirmPasswordTextBox.Text, StringComparison.Ordinal))
            {
                SetStatus("A confirmacao nao confere com a nova senha.", true);
                return;
            }

            var result = _authenticationController.ChangePassword(_configuration, _databaseProfile, _userName, _newPasswordTextBox.Text);
            SetStatus(result.Message, !result.Success);
            if (result.Success)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void SetStatus(string message, bool error)
        {
            _statusLabel.Text = message ?? string.Empty;
            _statusLabel.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        }

        private static Label CreateFieldLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 8, 0, 4),
            };
        }
    }
}
