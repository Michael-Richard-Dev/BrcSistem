using System;
using System.Collections.Generic;
using System.Linq;
using BRCSISTEM.Desktop.Models;
using BRCSISTEM.Desktop.Views;

namespace BRCSISTEM.Desktop.Data
{
    /// <summary>
    /// Fonte de dados em memoria para a selecao de fornecedores.
    /// Recebe a lista ja carregada pelo chamador (LookupOption[]) e
    /// converte para o modelo proprio da tela.
    /// </summary>
    internal sealed class FornecedorSelecaoData
    {
        private readonly LookupOption[] _opcoes;

        public FornecedorSelecaoData(LookupOption[] opcoes)
        {
            _opcoes = opcoes ?? new LookupOption[0];
        }

        public IReadOnlyList<FornecedorSelecaoItem> Listar()
        {
            return _opcoes
                .Where(o => o != null)
                .Select(o => new FornecedorSelecaoItem
                {
                    Codigo = o.Code ?? string.Empty,
                    Nome = o.Description ?? string.Empty,
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
