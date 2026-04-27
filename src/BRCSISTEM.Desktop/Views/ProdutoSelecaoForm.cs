using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Controllers;
using BRCSISTEM.Desktop.Data;
using BRCSISTEM.Desktop.Models;

namespace BRCSISTEM.Desktop.Views
{
    internal sealed partial class ProdutoSelecaoForm : Form
    {
        private readonly ProdutoSelecaoController _controller;
        private readonly bool _isDesignerInstance;

        public ProdutoSelecaoForm()
            : this(null, null, true)
        {
        }

        public ProdutoSelecaoForm(LookupOption[] opcoes)
            : this(opcoes, null, false)
        {
        }

        public ProdutoSelecaoForm(LookupOption[] opcoes, string titulo)
            : this(opcoes, titulo, false)
        {
        }

        private ProdutoSelecaoForm(LookupOption[] opcoes, string titulo, bool designerCtor)
        {
            _isDesignerInstance = designerCtor;
            _controller = designerCtor
                ? null
                : new ProdutoSelecaoController(new ProdutoSelecaoData(opcoes));

            InitializeComponent();

            if (designerCtor)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                Text = titulo;
            }

            AcceptButton = _confirmButton;
        }

        public LookupOption SelectedOption { get; private set; }

        private bool IsDesignModeActive
        {
            get
            {
                if (_isDesignerInstance) return true;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return true;
                if (DesignMode) return true;
                return Site != null && Site.DesignMode;
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;
            AtualizarGrid();
        }

        private void OnFilterTextChanged(object sender, EventArgs e)
        {
            if (IsDesignModeActive) return;
            AtualizarGrid();
        }

        private void OnGridCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
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

        private void AtualizarGrid()
        {
            var itens = _controller.Filtrar(_filterTextBox.Text);
            _grid.DataSource = new List<ProdutoSelecaoItem>(itens);

            if (_grid.Rows.Count > 0)
            {
                _grid.Rows[0].Selected = true;
                _grid.CurrentCell = _grid.Rows[0].Cells[0];
            }
        }

        private void ConfirmarSelecao()
        {
            var linha = _grid.CurrentRow;
            var item = linha == null ? null : linha.DataBoundItem as ProdutoSelecaoItem;
            var opcao = _controller.ObterOpcaoSelecionada(item);

            if (opcao == null)
            {
                MessageBox.Show(this, "Selecione um produto.", "Informacao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SelectedOption = opcao;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
