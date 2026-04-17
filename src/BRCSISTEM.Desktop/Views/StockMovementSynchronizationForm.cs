using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed class StockMovementSynchronizationForm : Form
    {
        private const int MaxGridRows = 800;

        private readonly DatabaseMaintenanceController _maintenanceController;
        private readonly ConfigurationController _configurationController;
        private readonly UserIdentity _identity;
        private readonly DatabaseProfile _databaseProfile;

        private AppConfiguration _configuration;
        private StockMovementSyncDiagnostic _diagnostic = new StockMovementSyncDiagnostic();

        private Label _activeMovementsLabel;
        private Label _notesLabel;
        private Label _transferOutputLabel;
        private Label _transferInputLabel;
        private Label _requisitionLabel;
        private Label _productionOutputLabel;
        private Label _totalLabel;
        private Label _statusLabel;
        private Label _gridInfoLabel;
        private Button _refreshButton;
        private Button _forceButton;
        private DataGridView _grid;

        public StockMovementSynchronizationForm(CompositionRoot compositionRoot, UserIdentity identity, DatabaseProfile databaseProfile)
        {
            _maintenanceController = compositionRoot.CreateDatabaseMaintenanceController();
            _configurationController = compositionRoot.CreateConfigurationController();
            _identity = identity;
            _databaseProfile = databaseProfile;

            InitializeComponent();
            Load += OnFormLoad;
        }

        private void InitializeComponent()
        {
            Text = "BRCSISTEM - Sincronizar Movimentos x Estoque";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(1300, 780);
            MinimumSize = new Size(1120, 640);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += OnFormKeyDown;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                RowCount = 5,
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildHeaderPanel(), 0, 0);
            root.Controls.Add(BuildSummaryPanel(), 0, 1);
            root.Controls.Add(BuildActionsPanel(), 0, 2);
            root.Controls.Add(BuildGridPanel(), 0, 3);
            root.Controls.Add(BuildFooterPanel(), 0, 4);

            Controls.Add(root);
        }

        private Control BuildHeaderPanel()
        {
            var panel = new Panel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(0, 0, 0, 6) };

            var subtitle = new Label
            {
                AutoSize = true,
                Text = "Regras: considera apenas registros ATIVOS, na maior versao, e insere somente quando nao existir em movimentos_estoque.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 100, 100),
                Dock = DockStyle.Top,
            };

            var title = new Label
            {
                AutoSize = true,
                Text = "Conferencia de Movimentos Legados x movimentos_estoque",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 54, 93),
                Dock = DockStyle.Top,
            };

            panel.Controls.Add(subtitle);
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildSummaryPanel()
        {
            var group = new GroupBox
            {
                Text = "Resumo",
                Dock = DockStyle.Top,
                Height = 120,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8),
                ColumnCount = 3,
                RowCount = 3,
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            _activeMovementsLabel = CreateSummaryLabel("Movimentos Estoque (ATIVO): 0");
            _notesLabel = CreateSummaryLabel("Notas faltantes: 0");
            _transferOutputLabel = CreateSummaryLabel("Transferencia Saida faltantes: 0");
            _transferInputLabel = CreateSummaryLabel("Transferencia Entrada faltantes: 0");
            _requisitionLabel = CreateSummaryLabel("Requisicoes faltantes: 0");
            _productionOutputLabel = CreateSummaryLabel("Saida Producao faltantes: 0");
            _totalLabel = CreateSummaryLabel("Total faltante: 0", true);

            layout.Controls.Add(_activeMovementsLabel, 0, 0);
            layout.Controls.Add(_notesLabel, 1, 0);
            layout.Controls.Add(_transferOutputLabel, 2, 0);
            layout.Controls.Add(_transferInputLabel, 0, 1);
            layout.Controls.Add(_requisitionLabel, 1, 1);
            layout.Controls.Add(_productionOutputLabel, 2, 1);
            layout.Controls.Add(_totalLabel, 0, 2);

            group.Controls.Add(layout);
            return group;
        }

        private Control BuildActionsPanel()
        {
            var group = new GroupBox
            {
                Text = "Acoes",
                Dock = DockStyle.Top,
                Height = 68,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 4, 8, 4),
                WrapContents = false,
            };

            _refreshButton = CreateButton("Atualizar Diagnostico", (sender, args) => UpdateDiagnostic());
            _forceButton = CreateButton("Forcar Alimentacao", (sender, args) => ForceSynchronization());
            var closeButton = CreateButton("Fechar", (sender, args) => Close());

            _statusLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(12, 8, 0, 0),
            };

            flow.Controls.Add(_refreshButton);
            flow.Controls.Add(_forceButton);
            flow.Controls.Add(closeButton);
            flow.Controls.Add(_statusLabel);

            group.Controls.Add(flow);
            return group;
        }

        private Control BuildGridPanel()
        {
            var group = new GroupBox
            {
                Text = "Amostra de Itens Faltantes",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
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
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9F),
            };

            _grid.Columns.Add(MakeCol("categoria", "Categoria", 170));
            _grid.Columns.Add(MakeCol("documento", "Documento", 120));
            _grid.Columns.Add(MakeCol("item", "Item", 70, DataGridViewContentAlignment.MiddleCenter));
            _grid.Columns.Add(MakeCol("data_movimento", "Data Movimento", 115, DataGridViewContentAlignment.MiddleCenter));
            _grid.Columns.Add(MakeCol("almoxarifado", "Almox", 95, DataGridViewContentAlignment.MiddleCenter));
            _grid.Columns.Add(MakeCol("fornecedor", "Fornecedor", 130));
            _grid.Columns.Add(MakeCol("material", "Material", 100, DataGridViewContentAlignment.MiddleCenter));
            _grid.Columns.Add(MakeCol("lote", "Lote", 120));
            _grid.Columns.Add(MakeCol("quantidade", "Quantidade", 110, DataGridViewContentAlignment.MiddleRight));
            _grid.Columns.Add(MakeCol("vencimento", "Validade", 95, DataGridViewContentAlignment.MiddleCenter, true));

            group.Controls.Add(_grid);
            return group;
        }

        private Control BuildFooterPanel()
        {
            var panel = new Panel { Dock = DockStyle.Top, AutoSize = true };
            _gridInfoLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 100, 100),
            };
            panel.Controls.Add(_gridInfoLabel);
            return panel;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                BeginInvoke((Action)UpdateDiagnostic);
            }
            catch (Exception ex)
            {
                ShowError("Erro ao carregar tela", ex);
            }
        }

        private void UpdateDiagnostic()
        {
            try
            {
                SetBusy(true);
                _statusLabel.Text = "Atualizando diagnostico...";
                System.Windows.Forms.Application.DoEvents();

                _diagnostic = _maintenanceController.DiagnoseStockMovementSynchronization(_configuration, _databaseProfile);
                PopulateSummary();
                PopulateGrid();
                _statusLabel.Text = $"Diagnostico atualizado em {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Total faltante: {_diagnostic.TotalMissing}";
            }
            catch (Exception ex)
            {
                ShowError("Erro ao atualizar diagnostico", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ForceSynchronization()
        {
            var confirmation = MessageBox.Show(
                this,
                "Esta acao vai inserir em movimentos_estoque os itens faltantes detectados,\nrespeitando apenas registros ATIVOS e da maior versao.\n\nDeseja continuar?",
                "Confirmar Forca de Alimentacao",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirmation != DialogResult.Yes)
            {
                return;
            }

            try
            {
                SetBusy(true);
                _statusLabel.Text = "Forcando alimentacao...";
                System.Windows.Forms.Application.DoEvents();

                var result = _maintenanceController.SynchronizeMissingStockMovements(_configuration, _databaseProfile, _identity.UserName);
                if (result.ExpectedItems <= 0)
                {
                    MessageBox.Show(this, "Nenhum item pendente para sincronizar.", "Sincronizacao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateDiagnostic();
                    return;
                }

                MessageBox.Show(
                    this,
                    $"Itens previstos: {result.ExpectedItems}\nItens inseridos: {result.InsertedItems}",
                    "Sincronizacao concluida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                UpdateDiagnostic();
            }
            catch (Exception ex)
            {
                ShowError("Erro ao forcar alimentacao", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void PopulateSummary()
        {
            _activeMovementsLabel.Text = $"Movimentos Estoque (ATIVO): {_diagnostic.ActiveMovements}";
            _notesLabel.Text = $"Notas faltantes: {_diagnostic.MissingNotes}";
            _transferOutputLabel.Text = $"Transferencia Saida faltantes: {_diagnostic.MissingTransferOutputs}";
            _transferInputLabel.Text = $"Transferencia Entrada faltantes: {_diagnostic.MissingTransferInputs}";
            _requisitionLabel.Text = $"Requisicoes faltantes: {_diagnostic.MissingRequisitions}";
            _productionOutputLabel.Text = $"Saida Producao faltantes: {_diagnostic.MissingProductionOutputs}";
            _totalLabel.Text = $"Total faltante: {_diagnostic.TotalMissing}";
        }

        private void PopulateGrid()
        {
            _grid.Rows.Clear();
            var orderedItems = (_diagnostic.Items ?? Array.Empty<StockMovementSyncItem>())
                .OrderBy(item => CategoryOrder(item.Category))
                .ThenBy(item => item.DocumentNumber ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.DocumentItem)
                .ToList();

            var shown = 0;
            foreach (var item in orderedItems)
            {
                if (shown >= MaxGridRows)
                {
                    break;
                }

                _grid.Rows.Add(
                    FormatCategory(item.Category),
                    item.DocumentNumber ?? string.Empty,
                    item.DocumentItem > 0 ? item.DocumentItem.ToString(CultureInfo.InvariantCulture) : string.Empty,
                    FormatDate(item.MovementDate),
                    item.Warehouse ?? string.Empty,
                    !string.IsNullOrWhiteSpace(item.DocumentSupplier) ? item.DocumentSupplier : (item.LotSupplier ?? string.Empty),
                    item.Material ?? string.Empty,
                    item.Lot ?? string.Empty,
                    FormatQuantity(item.Quantity),
                    FormatDate(item.ExpirationDate));

                shown++;
            }

            _gridInfoLabel.Text = orderedItems.Count > MaxGridRows
                ? $"Mostrando {MaxGridRows} de {orderedItems.Count} registros faltantes."
                : $"Mostrando {orderedItems.Count} registros faltantes.";
        }

        private void SetBusy(bool busy)
        {
            _refreshButton.Enabled = !busy;
            _forceButton.Enabled = !busy;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                Close();
            }
            else if (e.KeyCode == Keys.F5)
            {
                UpdateDiagnostic();
            }
        }

        private static int CategoryOrder(string category)
        {
            switch ((category ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "NOTA":
                    return 0;
                case "TRANSFERENCIA_SAIDA":
                    return 1;
                case "TRANSFERENCIA_ENTRADA":
                    return 2;
                case "REQUISICAO":
                    return 3;
                case "SAIDA_PRODUCAO":
                    return 4;
                default:
                    return 99;
            }
        }

        private static string FormatCategory(string category)
        {
            switch ((category ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "TRANSFERENCIA_SAIDA":
                    return "TRANSFERENCIA SAIDA";
                case "TRANSFERENCIA_ENTRADA":
                    return "TRANSFERENCIA ENTRADA";
                case "SAIDA_PRODUCAO":
                    return "SAIDA PRODUCAO";
                default:
                    return (category ?? string.Empty).Trim().ToUpperInvariant();
            }
        }

        private static string FormatDate(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            var trimmed = rawValue.Trim();
            if (trimmed.Length >= 19)
            {
                trimmed = trimmed.Substring(0, 19);
            }

            DateTime parsed;
            var formats = new[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(trimmed, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return parsed.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"));
            }

            if (DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed))
            {
                return parsed.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"));
            }

            return trimmed.Length >= 10 ? trimmed.Substring(0, 10) : trimmed;
        }

        private static string FormatQuantity(decimal value)
        {
            return value.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
        }

        private void ShowError(string title, Exception ex)
        {
            MessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static Label CreateSummaryLabel(string text, bool bold = false)
        {
            return new Label
            {
                AutoSize = true,
                Text = text,
                Font = new Font("Segoe UI", 9F, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Color.FromArgb(27, 54, 93),
                Margin = new Padding(0, 6, 20, 6),
            };
        }

        private static Button CreateButton(string text, EventHandler handler)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                FlatStyle = FlatStyle.System,
                Margin = new Padding(0, 0, 8, 0),
            };
            button.Click += handler;
            return button;
        }

        private static DataGridViewTextBoxColumn MakeCol(string name, string header, int width, DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft, bool fill = false)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                Width = width,
                AutoSizeMode = fill ? DataGridViewAutoSizeColumnMode.Fill : DataGridViewAutoSizeColumnMode.None,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = alignment },
            };
        }
    }
}
