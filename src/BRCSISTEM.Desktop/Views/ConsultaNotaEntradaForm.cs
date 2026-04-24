using System;
using System.ComponentModel;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Domain.Models;

namespace BRCSISTEM.Desktop.Views
{
    public sealed partial class ConsultaNotaEntradaForm : Form
    {
        private readonly InboundReceiptController _controller;
        private readonly AppConfiguration _configuration;
        private readonly DatabaseProfile _databaseProfile;
        private readonly bool _isDesignerInstance;

        public ConsultaNotaEntradaForm()
            : this(null, null, null, true)
        {
        }

        public ConsultaNotaEntradaForm(
            InboundReceiptController controller,
            AppConfiguration configuration,
            DatabaseProfile databaseProfile)
            : this(controller, configuration, databaseProfile, false)
        {
        }

        private ConsultaNotaEntradaForm(
            InboundReceiptController controller,
            AppConfiguration configuration,
            DatabaseProfile databaseProfile,
            bool designerCtor)
        {
            _controller = controller;
            _configuration = configuration;
            _databaseProfile = databaseProfile;
            _isDesignerInstance = designerCtor;

            if (!designerCtor)
            {
                if (_controller == null) throw new ArgumentNullException(nameof(controller));
                if (_configuration == null) throw new ArgumentNullException(nameof(configuration));
                if (_databaseProfile == null) throw new ArgumentNullException(nameof(databaseProfile));
            }

            InitializeComponent();

            if (!IsDesignModeActive)
            {
                WireRuntimeEvents();
            }
        }

        public InboundReceiptSummary SelectedReceipt { get; private set; }

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

        private void WireRuntimeEvents()
        {
            Load += OnFormLoad;
            _filterTextBox.TextChanged += OnFilterTextChanged;
            //_searchButton.Click += OnSearchButtonClick;
            _selectButton.Click += OnSelectButtonClick;
            //_closeButton.Click += OnCloseButtonClick;
            _grid.CellDoubleClick += OnGridCellDoubleClick;
            _grid.KeyDown += OnGridKeyDown;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void OnFilterTextChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void OnSearchButtonClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void OnSelectButtonClick(object sender, EventArgs e)
        {
            ConfirmSelection();
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ConfirmSelection();
            }
        }

        private void OnGridKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                ConfirmSelection();
            }
        }

        private void RefreshGrid()
        {
            if (IsDesignModeActive || _controller == null)
            {
                return;
            }

            var items = _controller.SearchReceipts(_configuration, _databaseProfile, _filterTextBox.Text);
            _grid.DataSource = items;

            if (_grid.Rows.Count > 0)
            {
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private void ConfirmSelection()
        {
            if (!(_grid.CurrentRow?.DataBoundItem is InboundReceiptSummary item))
            {
                MessageBox.Show(this, "Selecione uma nota.", "Informacao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedReceipt = item;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
