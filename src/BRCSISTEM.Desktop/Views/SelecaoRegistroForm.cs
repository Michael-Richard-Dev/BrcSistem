using System;
using System.ComponentModel;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Desktop.Models;

namespace BRCSISTEM.Desktop.Views
{
    /// <summary>
    /// View generica de selecao de registros (fornecedor, material,
    /// almoxarifado, lote, etc). Responsavel apenas por exibicao e
    /// eventos de tela; toda a logica de filtro/selecao fica no
    /// <see cref="SelecaoRegistroController"/>.
    /// </summary>
    internal sealed partial class SelecaoRegistroForm : Form
    {
        private readonly SelecaoRegistroController _controller;
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
            _controller = new SelecaoRegistroController(options);
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
        }

        /// <summary>
        /// Opcao escolhida pelo usuario. Mantem o tipo publico
        /// original (<see cref="LookupOption"/>) para preservar
        /// compatibilidade com chamadores existentes.
        /// </summary>
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

            AtualizarGrid();
        }

        private void OnFilterTextChanged(object sender, EventArgs e)
        {
            if (IsDesignModeActive)
            {
                return;
            }

            AtualizarGrid();
        }

        private void OnGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            ConfirmarSelecao();
        }

        private void OnGridKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ConfirmarSelecao();
            }
        }

        private void OnConfirmClick(object sender, EventArgs e)
        {
            ConfirmarSelecao();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AtualizarGrid()
        {
            var itens = _controller.Filtrar(_filterTextBox.Text);

            // DataGridView espera lista mutavel para binding simples.
            var fonte = new System.Collections.Generic.List<SelecaoRegistroItem>(itens);
            _grid.DataSource = fonte;

            if (_grid.Rows.Count > 0)
            {
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private void ConfirmarSelecao()
        {
            var linha = _grid.CurrentRow;
            var item = linha == null ? null : linha.DataBoundItem as SelecaoRegistroItem;
            var opcao = _controller.ObterOpcaoSelecionada(item);

            if (opcao == null)
            {
                MessageBox.Show(this, "Selecione um registro.", "Informacao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedOption = opcao;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
