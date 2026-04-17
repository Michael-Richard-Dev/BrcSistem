using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class AuditLogQueryForm
    {
        private sealed class AuditLogGridRow
        {
            public long Id { get; set; }

            public string DateTime { get; set; }

            public string UserName { get; set; }

            public string Action { get; set; }

            public string DetailsSummary { get; set; }
        }

        private void LoadData()
        {
            try
            {
                _configuration = _configurationController.LoadConfiguration();
                _periodComboBox.SelectedItem = "Ultimos 30 dias";
                LoadUsers();
                LoadActions();
                SearchFromFirstPage();
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void LoadUsers()
        {
            var users = _databaseMaintenanceController.LoadAuditUsers(_configuration, _databaseProfile).ToList();
            users.Insert(0, "Todos");
            _userComboBox.BeginUpdate();
            _userComboBox.DataSource = null;
            _userComboBox.DataSource = users;
            _userComboBox.EndUpdate();
            _userComboBox.SelectedIndex = 0;
        }

        private void LoadActions()
        {
            var actions = _databaseMaintenanceController.LoadAuditActions(_configuration, _databaseProfile).ToList();
            actions.Insert(0, "Todas");
            _actionComboBox.BeginUpdate();
            _actionComboBox.DataSource = null;
            _actionComboBox.DataSource = actions;
            _actionComboBox.EndUpdate();
            _actionComboBox.SelectedIndex = 0;
        }

        private void SearchFromFirstPage()
        {
            _currentPage = 1;
            Search();
        }

        private void Search()
        {
            try
            {
                string filterDateFrom;
                string filterDateTo;
                ResolvePeriod(out filterDateFrom, out filterDateTo);

                var filterUser = NormalizeUserFilter();
                var filterAction = NormalizeActionFilter();
                var searchText = (_searchTextBox.Text ?? string.Empty).Trim();
                var offset = (_currentPage - 1) * _pageSize;

                _totalRecords = _databaseMaintenanceController.CountAuditLog(
                    _configuration,
                    _databaseProfile,
                    filterUser,
                    filterAction,
                    filterDateFrom,
                    filterDateTo,
                    searchText);

                _currentEntries = _databaseMaintenanceController.LoadAuditLog(
                    _configuration,
                    _databaseProfile,
                    filterUser,
                    filterAction,
                    filterDateFrom,
                    filterDateTo,
                    searchText,
                    _pageSize,
                    offset).ToArray();

                BindGrid();
                UpdatePagingInfo();
                SetStatus("Logs carregados com sucesso.", false);
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        private void BindGrid()
        {
            var rows = new List<AuditLogGridRow>();
            foreach (var entry in _currentEntries)
            {
                var details = entry.Details ?? string.Empty;
                rows.Add(new AuditLogGridRow
                {
                    Id = entry.Id,
                    DateTime = FormatAuditDate(entry.DateTime),
                    UserName = string.IsNullOrWhiteSpace(entry.UserName) ? "N/A" : entry.UserName,
                    Action = string.IsNullOrWhiteSpace(entry.Action) ? "N/A" : entry.Action,
                    DetailsSummary = details.Length > 100 ? details.Substring(0, 100) + "..." : details,
                });
            }

            _logsGrid.DataSource = rows;
            if (_logsGrid.Rows.Count > 0)
            {
                _logsGrid.ClearSelection();
            }
        }

        private void UpdatePagingInfo()
        {
            var totalPages = Math.Max(1, (_totalRecords + _pageSize - 1) / _pageSize);
            var start = _totalRecords == 0 ? 0 : ((_currentPage - 1) * _pageSize) + 1;
            var end = Math.Min(_currentPage * _pageSize, _totalRecords);

            _infoLabel.Text = "Exibindo " + start + "-" + end + " de " + _totalRecords + " registros";
            _pageLabel.Text = "Pagina " + _currentPage + " de " + totalPages;
            _previousButton.Enabled = _currentPage > 1;
            _nextButton.Enabled = _currentPage < totalPages;
        }

        private void ResolvePeriod(out string filterDateFrom, out string filterDateTo)
        {
            filterDateFrom = string.Empty;
            filterDateTo = string.Empty;

            var period = _periodComboBox.SelectedItem as string ?? "Ultimos 30 dias";
            if (string.Equals(period, "Todos", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var now = DateTime.Now;
            if (string.Equals(period, "Hoje", StringComparison.OrdinalIgnoreCase))
            {
                filterDateFrom = now.Date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                filterDateTo = now.Date.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                return;
            }

            var days = 30;
            if (string.Equals(period, "Ultimos 7 dias", StringComparison.OrdinalIgnoreCase))
            {
                days = 7;
            }
            else if (string.Equals(period, "Ultimos 90 dias", StringComparison.OrdinalIgnoreCase))
            {
                days = 90;
            }

            filterDateFrom = now.AddDays(-days).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            filterDateTo = now.AddDays(1).Date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private string NormalizeUserFilter()
        {
            var value = _userComboBox.SelectedItem as string ?? string.Empty;
            return string.Equals(value, "Todos", StringComparison.OrdinalIgnoreCase) ? string.Empty : value;
        }

        private string NormalizeActionFilter()
        {
            var value = _actionComboBox.SelectedItem as string ?? string.Empty;
            return string.Equals(value, "Todas", StringComparison.OrdinalIgnoreCase) ? string.Empty : value;
        }

        private void ClearFilters()
        {
            _periodComboBox.SelectedItem = "Ultimos 30 dias";
            _userComboBox.SelectedIndex = _userComboBox.Items.Count > 0 ? 0 : -1;
            _actionComboBox.SelectedIndex = _actionComboBox.Items.Count > 0 ? 0 : -1;
            _searchTextBox.Clear();
            SearchFromFirstPage();
        }

        private void GoToPreviousPage()
        {
            if (_currentPage <= 1)
            {
                return;
            }

            _currentPage--;
            Search();
        }

        private void GoToNextPage()
        {
            var totalPages = Math.Max(1, (_totalRecords + _pageSize - 1) / _pageSize);
            if (_currentPage >= totalPages)
            {
                return;
            }

            _currentPage++;
            Search();
        }

        private void OnGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            ShowDetails();
        }

        private void ShowDetails()
        {
            var entry = GetSelectedEntry();
            if (entry == null)
            {
                return;
            }

            using (var dialog = new AuditLogDetailForm(entry))
            {
                dialog.ShowDialog(this);
            }
        }

        private AuditLogEntry GetSelectedEntry()
        {
            if (_logsGrid.CurrentRow == null)
            {
                return null;
            }

            var row = _logsGrid.CurrentRow.DataBoundItem as AuditLogGridRow;
            if (row == null)
            {
                return null;
            }

            return _currentEntries.FirstOrDefault(item => item.Id == row.Id);
        }

        private void OnSearchTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SearchFromFirstPage();
            }
        }

        private void OnFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                Search();
                return;
            }

            if (e.KeyCode == Keys.Enter && ActiveControl == _logsGrid)
            {
                e.Handled = true;
                ShowDetails();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private static string FormatAuditDate(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            DateTime parsed;
            var formats = new[]
            {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ssK",
                "dd/MM/yyyy HH:mm:ss",
            };
            if (DateTime.TryParseExact(rawValue.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out parsed))
            {
                return parsed.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("pt-BR"));
            }

            return rawValue;
        }

        private sealed class AuditLogDetailForm : Form
        {
            public AuditLogDetailForm(AuditLogEntry entry)
            {
                Text = "Detalhes do Log #" + entry.Id;
                StartPosition = FormStartPosition.CenterParent;
                Size = new Size(760, 560);
                MinimumSize = new Size(620, 420);
                BackColor = Color.White;

                var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), RowCount = 5 };
                root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                root.Controls.Add(CreateDetailLabel("Log #" + entry.Id, 12F, true), 0, 0);
                root.Controls.Add(CreateDetailLabel("Data/Hora: " + FormatAuditDate(entry.DateTime), 9.5F, false), 0, 1);
                root.Controls.Add(CreateDetailLabel("Usuario: " + (entry.UserName ?? string.Empty) + "    Acao: " + (entry.Action ?? string.Empty), 9.5F, false), 0, 2);

                var detailsBox = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ScrollBars = ScrollBars.Both,
                    ReadOnly = true,
                    Font = new Font("Consolas", 9F),
                    Text = entry.Details ?? string.Empty,
                };
                root.Controls.Add(detailsBox, 0, 3);

                var closeButton = new Button { Text = "Fechar", AutoSize = true, Anchor = AnchorStyles.Right };
                closeButton.Click += (sender, args) => Close();

                var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Right, AutoSize = true };
                buttonPanel.Controls.Add(closeButton);
                root.Controls.Add(buttonPanel, 0, 4);

                Controls.Add(root);
            }

            private static Label CreateDetailLabel(string text, float size, bool bold)
            {
                return new Label
                {
                    AutoSize = true,
                    Text = text,
                    Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = bold ? Color.FromArgb(27, 54, 93) : Color.Black,
                    Margin = new Padding(0, 0, 0, 8),
                };
            }
        }
    }
}
