using System;
using System.Collections.Generic;
using System.Linq;
using BRCSISTEM.Desktop.Models;
using BRCSISTEM.Desktop.Views;

namespace BRCSISTEM.Desktop.Controllers
{
    /// <summary>
    /// Controlador da tela <see cref="SelecaoRegistroForm"/>.
    /// Concentra a logica de preparacao, filtragem e validacao da
    /// selecao para que a View fique apenas com responsabilidades visuais.
    /// </summary>
    internal sealed class SelecaoRegistroController
    {
        private readonly SelecaoRegistroItem[] _itens;

        public SelecaoRegistroController(IEnumerable<LookupOption> opcoes)
        {
            _itens = (opcoes ?? Enumerable.Empty<LookupOption>())
                .Where(opcao => opcao != null)
                .Select(MapearParaItem)
                .ToArray();
        }

        /// <summary>
        /// Quantidade total de itens disponiveis (sem filtro).
        /// </summary>
        public int Total
        {
            get { return _itens.Length; }
        }

        /// <summary>
        /// Aplica filtro textual (codigo, descricao ou status) e devolve
        /// a lista pronta para ser atribuida ao DataSource do grid.
        /// </summary>
        public IReadOnlyList<SelecaoRegistroItem> Filtrar(string filtro)
        {
            var termo = (filtro ?? string.Empty).Trim();
            if (termo.Length == 0)
            {
                return _itens;
            }

            return _itens
                .Where(item =>
                    Contem(item.Codigo, termo)
                    || Contem(item.Descricao, termo)
                    || Contem(item.Status, termo))
                .ToArray();
        }

        /// <summary>
        /// Valida o item selecionado e devolve a opcao original
        /// (<see cref="LookupOption"/>) esperada pelo chamador.
        /// Retorna <c>null</c> quando nao ha selecao valida.
        /// </summary>
        public LookupOption ObterOpcaoSelecionada(SelecaoRegistroItem item)
        {
            return item == null ? null : item.OpcaoOriginal;
        }

        private static SelecaoRegistroItem MapearParaItem(LookupOption opcao)
        {
            return new SelecaoRegistroItem
            {
                Codigo = opcao.Code ?? string.Empty,
                Descricao = opcao.Description ?? string.Empty,
                Status = opcao.Status ?? string.Empty,
                OpcaoOriginal = opcao,
            };
        }

        private static bool Contem(string fonte, string termo)
        {
            return (fonte ?? string.Empty).IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
