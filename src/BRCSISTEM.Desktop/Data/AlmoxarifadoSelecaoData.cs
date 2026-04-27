using System;
using System.Collections.Generic;
using System.Linq;
using BRCSISTEM.Desktop.Models;
using BRCSISTEM.Desktop.Views;

namespace BRCSISTEM.Desktop.Data
{
    internal sealed class AlmoxarifadoSelecaoData
    {
        private readonly LookupOption[] _opcoes;

        public AlmoxarifadoSelecaoData(LookupOption[] opcoes)
        {
            _opcoes = opcoes ?? new LookupOption[0];
        }

        public IReadOnlyList<AlmoxarifadoSelecaoItem> Listar()
        {
            return _opcoes
                .Where(o => o != null)
                .Select(o => new AlmoxarifadoSelecaoItem
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
