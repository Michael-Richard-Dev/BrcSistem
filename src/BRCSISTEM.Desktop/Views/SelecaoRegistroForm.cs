using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace BRCSISTEM.Desktop.Views
{
    internal sealed partial class SelecaoRegistroForm : Form
    {
        private readonly LookupOption[] _allOptions;
        private readonly bool _isDesignerInstance;
        private readonly string _descriptionHeader;

        public SelecaoRegistroForm()
            : this(null, null, null, true)
        {
        }

        public SelecaoRegistroForm(string title, string descriptionHeader, LookupOption[] options)
            : this(title, descriptionHeader, options, false)
        {
        }

        private SelecaoRegistroForm(string title, string descriptionHeader, LookupOption[] options, bool designerCtor)
        {
            _isDesignerInstance = designerCtor;
            _allOptions = options ?? new LookupOption[0];
            _descriptionHeader = string.IsNullOrWhiteSpace(descriptionHeader)
                ? null
                : descriptionHeader.ToUpperInvariant();

            InitializeComponent();

            if (designerCtor)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                Text = title;
            }

            if (!string.IsNullOrEmpty(_descriptionHeader))
            {
                _colDescricao.HeaderText = _descriptionHeader;
            }

            AcceptButton = _confirmButton;
            CancelButton = _cancelButton;
        }

        public LookupOption SelectedOption { get; private set; }

        private bool IsDesignModeActive
        {
            get
            {
                if (_isDesignerInstance)
                {
                    return true;
                }

                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return true;
                }

                if (DesignMode)
                {
                    return true;
                }

                return Site != null && Site.DesignMode;
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (IsDesignModeActive)
            {
                return;
            }

            RefreshGrid();
        }

        private void OnFilterTextChanged(object sender, EventArgs e)
        {
            if (IsDesignModeActive)
            {
                return;
            }

            RefreshGrid();
        }

        private void OnGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            ConfirmSelection();
        }

        private void OnGridKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ConfirmSelection();
            }
        }

        private void OnConfirmClick(object sender, EventArgs e)
        {
            ConfirmSelection();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void RefreshGrid()
        {
            var filter = (_filterTextBox.Text ?? string.Empty).Trim();
            var items = string.IsNullOrWhiteSpace(filter)
                ? _allOptions
                : _allOptions.Where(item =>
                    (item.Code ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item.Description ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                    || (item.Status ?? string.Empty).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToArray();

            _grid.DataSource = items;

            if (_grid.Rows.Count > 0)
            {
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private void ConfirmSelection()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is LookupOption option))
            {
                MessageBox.Show(this, "Selecione um registro.", "Informacao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedOption = option;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
