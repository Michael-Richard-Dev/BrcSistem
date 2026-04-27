using System;
using System.Collections.Generic;
using System.Linq;
using BRCSISTEM.Desktop.Data;
using BRCSISTEM.Desktop.Models;
using BRCSISTEM.Desktop.Interface;

namespace BRCSISTEM.Desktop.Controllers
{
    internal sealed class ProdutoSelecaoController
    {
        private readonly ProdutoSelecaoData _data;
        private readonly ProdutoSelecaoItem[] _itens;

        public ProdutoSelecaoController(ProdutoSelecaoData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _data = data;
            _itens = _data.Listar().ToArray();
        }

        public int Total { get { return _itens.Length; } }

        public IReadOnlyList<ProdutoSelecaoItem> Filtrar(string filtro)
        {
            var termo = (filtro ?? string.Empty).Trim();
            if (termo.Length == 0)
            {
                return _itens;
            }

            return _itens
                .Where(i =>
                    Contem(i.Codigo, termo)
                    || Contem(i.Descricao, termo)
                    || Contem(i.Status, termo))
                .ToArray();
        }

        public LookupOption ObterOpcaoSelecionada(ProdutoSelecaoItem item)
        {
            return item == null ? null : item.OpcaoOriginal;
        }

        private static bool Contem(string fonte, string termo)
        {
            return (fonte ?? string.Empty).IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
