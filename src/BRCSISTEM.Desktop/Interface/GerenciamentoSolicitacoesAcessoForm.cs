using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Interface
{
    /// <summary>
    /// Porte fiel de views/gerenciar_acessos.py (classe GerenciarAcessos).
    /// Tela administrativa para aprovar/cancelar solicitacoes de acesso pendentes.
    /// </summary>
    public sealed partial class GerenciamentoSolicitacoesAcessoForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly AdministrationController _administrationController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;
        private readonly bool _isDesignerInstance;

        private AppConfiguration _configuration;

        private sealed class AccessRequestRow
        {
            public string Id { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Justificativa { get; set; }
            public string Data { get; set; }
            public string MensagemCompleta { get; set; }
        }

        public GerenciamentoSolicitacoesAcessoForm()
        {
            _isDesignerInstance = true;
            InitializeComponent();
        }

        public GerenciamentoSolicitacoesAcessoForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot ?? throw new ArgumentNullException(nameof(compositionRoot));
            _administrationController = compositionRoot.CreateAdministrationController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _databaseProfile = databaseProfile ?? throw new ArgumentNullException(nameof(databaseProfile));

            InitializeComponent();
        }

        private bool IsDesignModeActive
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                    || _isDesignerInstance
                    || DesignMode
                    || (Site != null && Site.DesignMode);
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (!IsDesignModeActive)
            {
                OnInitialLoad();
            }
        }

        private void OnGridSelectionChanged(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        private void OnGridDoubleClick(object sender, EventArgs e)
        {
            ApproveSelected();
        }

        private void OnApproveClick(object sender, EventArgs e)
        {
            ApproveSelected();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            CancelSelected();
        }

        private void OnRefreshClick(object sender, EventArgs e)
        {
            LoadRequests();
        }

        private void OnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                LoadRequests();
            }
            else if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                ApproveSelected();
            }
            else if (e.KeyCode == Keys.F3 || e.KeyCode == Keys.Delete)
            {
                e.Handled = true;
                CancelSelected();
            }
        }

        private void OnInitialLoad()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                LoadRequests();
                try
                {
                    _administrationController.LogAccessManagementOpened(_configuration, _databaseProfile, _identity.UserName);
                }
                catch
                {
                    // Falha de auditoria nao impede uso da tela (espelha padrao SafeAudit).
                }
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadRequests()
        {
            if (IsDesignModeActive)
            {
                return;
            }

            try
            {
                SetStatus("Carregando solicitacoes...", Color.SteelBlue);
                System.Windows.Forms.Application.DoEvents();

                if (_configuration == null)
                {
                    _configuration = _configurationController.LoadConfiguration();
                }

                var pending = _administrationController.LoadPendingAccessRequests(_configuration, _databaseProfile) ?? new AccessRequest[0];
                var rows = pending.Select(req =>
                {
                    var mensagem = req.Message ?? string.Empty;
                    var display = mensagem.Length > 40 ? mensagem.Substring(0, 37) + "..." : mensagem;
                    return new AccessRequestRow
                    {
                        Id = req.Id,
                        Nome = req.Name,
                        Email = req.Email,
                        Justificativa = display,
                        Data = req.RequestedAt,
                        MensagemCompleta = mensagem,
                    };
                }).ToList();

                _grid.DataSource = rows;
                if (_grid.Rows.Count > 0)
                {
                    _grid.ClearSelection();
                }

                ClearDetails();
                UpdateActionButtons();

                var total = rows.Count;
                if (total == 0)
                {
                    SetStatus("Nenhuma solicitacao pendente", Color.Gray);
                }
                else if (total == 1)
                {
                    SetStatus("1 solicitacao pendente", Color.Green);
                }
                else
                {
                    SetStatus(total + " solicitacoes pendentes", Color.Green);
                }
            }
            catch (Exception exception)
            {
                SetStatus("Erro ao carregar dados", Color.Red);
                ShowError(exception);
            }
        }

        private void OnSelectionChanged()
        {
            var row = GetSelectedRow();
            if (row == null)
            {
                ClearDetails();
                UpdateActionButtons();
                return;
            }

            _nameLabel.Text = "Nome: " + (row.Nome ?? string.Empty);
            _emailLabel.Text = "E-mail: " + (row.Email ?? string.Empty);
            _dateLabel.Text = "Data: " + (row.Data ?? string.Empty);
            _justificationText.Text = string.IsNullOrWhiteSpace(row.MensagemCompleta)
                ? "Nenhuma justificativa informada"
                : row.MensagemCompleta;

            UpdateActionButtons();
        }

        private void ClearDetails()
        {
            _nameLabel.Text = "Nome: ";
            _emailLabel.Text = "E-mail: ";
            _dateLabel.Text = "Data: ";
            _justificationText.Text = "Selecione uma solicitacao para ver os detalhes";
        }

        private void UpdateActionButtons()
        {
            var enabled = GetSelectedRow() != null;
            _approveButton.Enabled = enabled;
            _cancelButton.Enabled = enabled;
        }

        private AccessRequestRow GetSelectedRow()
        {
            if (_grid == null || _grid.SelectedRows.Count == 0)
            {
                return null;
            }

            return _grid.SelectedRows[0].DataBoundItem as AccessRequestRow;
        }

        private void ApproveSelected()
        {
            if (IsDesignModeActive)
            {
                return;
            }

            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show(this, "Selecione uma solicitacao para aprovar", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetStatus("Processando aprovacao...", Color.SteelBlue);

            var msg = "Aprovar solicitacao de acesso?\r\n\r\n"
                    + "Nome: " + row.Nome + "\r\n"
                    + "E-mail: " + row.Email + "\r\n\r\n"
                    + "IMPORTANTE: Voce precisara criar o usuario manualmente "
                    + "na tela 'Cadastro de Usuarios' apos a aprovacao.";

            var confirm = MessageBox.Show(this, msg, "Confirmar Aprovacao",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                SetStatus("Pronto", Color.Gray);
                return;
            }

            try
            {
                _administrationController.ApproveAccessRequest(_configuration, _databaseProfile, _identity.UserName, row.Id);
                LoadRequests();
                SetStatus("Solicitacao de " + row.Nome + " aprovada com sucesso", Color.Green);

                var openUserForm = MessageBox.Show(this,
                    "Solicitacao de " + row.Nome + " foi aprovada!\r\n\r\n"
                    + "Deseja abrir a tela de Cadastro de Usuarios\r\npara criar o usuario agora?",
                    "Cadastro de Usuario", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (openUserForm == DialogResult.Yes)
                {
                    try
                    {
                        using (var dialog = new UserManagementForm(_compositionRoot, _identity, _databaseProfile))
                        {
                            dialog.ShowDialog(this);
                        }
                    }
                    catch (Exception inner)
                    {
                        MessageBox.Show(this,
                            "Nao foi possivel abrir o cadastro de usuarios.\r\n"
                            + "Acesse manualmente: Menu > Parametros > Cadastro de Usuarios\r\n\r\n"
                            + inner.Message,
                            "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception exception)
            {
                SetStatus("Erro ao aprovar solicitacao", Color.Red);
                ShowError(exception);
            }
        }

        private void CancelSelected()
        {
            if (IsDesignModeActive)
            {
                return;
            }

            var row = GetSelectedRow();
            if (row == null)
            {
                MessageBox.Show(this, "Selecione uma solicitacao para cancelar", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetStatus("Processando cancelamento...", Color.SteelBlue);

            var confirm = MessageBox.Show(this,
                "Cancelar solicitacao de acesso de " + row.Nome + "?",
                "Confirmar Cancelamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                SetStatus("Pronto", Color.Gray);
                return;
            }

            try
            {
                _administrationController.CancelAccessRequest(_configuration, _databaseProfile, _identity.UserName, row.Id);
                MessageBox.Show(this,
                    "Solicitacao de " + row.Nome + " foi cancelada.",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRequests();
                SetStatus("Solicitacao de " + row.Nome + " cancelada", Color.DarkOrange);
            }
            catch (Exception exception)
            {
                SetStatus("Erro ao cancelar solicitacao", Color.Red);
                ShowError(exception);
            }
        }

        private void SetStatus(string text, Color color)
        {
            _statusLabel.Text = text;
            _statusLabel.ForeColor = color;
        }

        private void ShowError(Exception exception)
        {
            MessageBox.Show(this, exception.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
