using System;
using System.Collections.Generic;
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
    public sealed class AccessRequestManagementForm : Form
    {
        private readonly CompositionRoot _compositionRoot;
        private readonly AdministrationController _administrationController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;

        private DataGridView _grid;
        private Label _nameLabel;
        private Label _emailLabel;
        private Label _dateLabel;
        private TextBox _justificationText;
        private Button _approveButton;
        private Button _cancelButton;
        private Button _refreshButton;
        private Button _closeButton;
        private Label _statusLabel;

        private sealed class AccessRequestRow
        {
            public string Id { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
            public string Justificativa { get; set; }
            public string Data { get; set; }
            public string MensagemCompleta { get; set; }
        }

        public AccessRequestManagementForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _compositionRoot = compositionRoot;
            _administrationController = compositionRoot.CreateAdministrationController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            Load += (sender, args) => OnInitialLoad();
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Gerenciar Acessos";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1000, 550);
            MinimumSize = new Size(950, 500);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(15, 10, 15, 10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));           // header
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));      // grid
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));           // detalhes
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));           // botoes
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));           // status

            // Cabecalho
            var header = new Label
            {
                AutoSize = true,
                Text = "Gerenciar Acessos",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 0, 0, 8),
            };
            root.Controls.Add(header, 0, 0);

            // Grid
            var listArea = new GroupBox
            {
                Dock = DockStyle.Fill,
                Text = " Solicitacoes ",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Padding = new Padding(8),
            };
            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 9.5F),
            };
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nome", HeaderText = "Nome", DataPropertyName = "Nome", Width = 180,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Email", HeaderText = "E-mail", DataPropertyName = "Email", Width = 200,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Justificativa", HeaderText = "Justificativa", DataPropertyName = "Justificativa",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Data", HeaderText = "Data", DataPropertyName = "Data", Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                SortMode = DataGridViewColumnSortMode.NotSortable,
            });
            _grid.SelectionChanged += (sender, args) => OnSelectionChanged();
            _grid.DoubleClick += (sender, args) => ApproveSelected();
            listArea.Controls.Add(_grid);
            root.Controls.Add(listArea, 0, 1);

            // Detalhes
            var detailArea = new GroupBox
            {
                Dock = DockStyle.Top,
                Text = " Detalhes ",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Padding = new Padding(6),
                Height = 110,
                Margin = new Padding(0, 8, 0, 0),
            };
            var detailLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
            };
            detailLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            detailLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _nameLabel  = BuildDetailLabel("Nome: ");
            _emailLabel = BuildDetailLabel("E-mail: ");
            _dateLabel  = BuildDetailLabel("Data: ");
            _justificationText = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                WordWrap = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(248, 249, 250),
                Font = new Font("Segoe UI", 8.5F),
                Height = 34,
                Text = "Selecione uma solicitacao para ver os detalhes",
            };

            detailLayout.Controls.Add(_nameLabel,          0, 0);
            detailLayout.Controls.Add(_emailLabel,         0, 1);
            detailLayout.Controls.Add(_dateLabel,          0, 2);
            detailLayout.Controls.Add(_justificationText,  0, 3);
            detailArea.Controls.Add(detailLayout);
            root.Controls.Add(detailArea, 0, 2);

            // Botoes
            var buttonBar = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                AutoSize = true,
                Margin = new Padding(0, 5, 0, 0),
            };
            buttonBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttonBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            buttonBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            buttonBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _approveButton = new Button { Text = "Aprovar (F1)",  AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(0, 0, 4, 0), Enabled = false };
            _cancelButton  = new Button { Text = "Cancelar (F3)", AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(0, 0, 4, 0), Enabled = false };
            _refreshButton = new Button { Text = "Atualizar",     AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(4, 0, 4, 0) };
            _closeButton   = new Button { Text = "Fechar",        AutoSize = true, FlatStyle = FlatStyle.System, Margin = new Padding(4, 0, 0, 0) };

            _approveButton.Click += (sender, args) => ApproveSelected();
            _cancelButton.Click  += (sender, args) => CancelSelected();
            _refreshButton.Click += (sender, args) => LoadRequests();
            _closeButton.Click   += (sender, args) => Close();

            var rightButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false,
            };
            rightButtons.Controls.Add(_closeButton);
            rightButtons.Controls.Add(_refreshButton);

            buttonBar.Controls.Add(_approveButton, 0, 0);
            buttonBar.Controls.Add(_cancelButton,  1, 0);
            buttonBar.Controls.Add(new Label { Width = 1 }, 2, 0);
            buttonBar.Controls.Add(rightButtons,   3, 0);
            root.Controls.Add(buttonBar, 0, 3);

            // Status
            _statusLabel = new Label
            {
                AutoSize = true,
                Text = "Pronto",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.Gray,
                Margin = new Padding(0, 6, 0, 0),
            };
            root.Controls.Add(_statusLabel, 0, 4);

            Controls.Add(root);
        }

        private static Label BuildDetailLabel(string text)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 8.5F),
                Margin = new Padding(0, 2, 0, 0),
            };
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

            _nameLabel.Text  = "Nome: "   + (row.Nome ?? string.Empty);
            _emailLabel.Text = "E-mail: " + (row.Email ?? string.Empty);
            _dateLabel.Text  = "Data: "   + (row.Data ?? string.Empty);
            _justificationText.Text = string.IsNullOrWhiteSpace(row.MensagemCompleta)
                ? "Nenhuma justificativa informada"
                : row.MensagemCompleta;

            UpdateActionButtons();
        }

        private void ClearDetails()
        {
            _nameLabel.Text  = "Nome: ";
            _emailLabel.Text = "E-mail: ";
            _dateLabel.Text  = "Data: ";
            _justificationText.Text = "Selecione uma solicitacao para ver os detalhes";
        }

        private void UpdateActionButtons()
        {
            var enabled = GetSelectedRow() != null;
            _approveButton.Enabled = enabled;
            _cancelButton.Enabled  = enabled;
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
