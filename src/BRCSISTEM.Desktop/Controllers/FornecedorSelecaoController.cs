using System;
using System.Collections.Generic;
using System.Linq;
using BRCSISTEM.Desktop.Data;
using BRCSISTEM.Desktop.Models;
using BRCSISTEM.Desktop.Views;

namespace BRCSISTEM.Desktop.Controllers
{
    internal sealed class FornecedorSelecaoController
    {
        private readonly FornecedorSelecaoData _data;
        private readonly FornecedorSelecaoItem[] _itens;

        public FornecedorSelecaoController(FornecedorSelecaoData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _data = data;
            _itens = _data.Listar().ToArray();
        }

        public int Total { get { return _itens.Length; } }

        public IReadOnlyList<FornecedorSelecaoItem> Filtrar(string filtro)
        {
            var termo = (filtro ?? string.Empty).Trim();
            if (termo.Length == 0)
            {
                return _itens;
            }

            return _itens
                .Where(i =>
                    Contem(i.Codigo, termo)
                    || Contem(i.Nome, termo)
                    || Contem(i.Status, termo))
                .ToArray();
        }

        public LookupOption ObterOpcaoSelecionada(FornecedorSelecaoItem item)
        {
            return item == null ? null : item.OpcaoOriginal;
        }

        private static bool Contem(string fonte, string termo)
        {
            return (fonte ?? string.Empty).IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
