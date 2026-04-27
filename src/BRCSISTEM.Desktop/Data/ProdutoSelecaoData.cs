using System;
using System.Collections.Generic;
using System.Linq;
using BRCSISTEM.Desktop.Models;
using BRCSISTEM.Desktop.Interface;

namespace BRCSISTEM.Desktop.Data
{
    internal sealed class ProdutoSelecaoData
    {
        private readonly LookupOption[] _opcoes;

        public ProdutoSelecaoData(LookupOption[] opcoes)
        {
            _opcoes = opcoes ?? new LookupOption[0];
        }

        public IReadOnlyList<ProdutoSelecaoItem> Listar()
        {
            return _opcoes
                .Where(o => o != null)
                .Select(o => new ProdutoSelecaoItem
                {
                    Codigo = o.Code ?? string.Empty,
                    Descricao = o.Description ?? string.Empty,
                    Status = o.Status ?? string.Empty,
                    OpcaoOriginal = o,
                })
                .ToArray();
        }

        public LookupOption ObterOpcaoOriginal(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return null;
            }

            return _opcoes.FirstOrDefault(o =>
                o != null && string.Equals(o.Code, codigo, StringComparison.OrdinalIgnoreCase));
        }
    }
}
