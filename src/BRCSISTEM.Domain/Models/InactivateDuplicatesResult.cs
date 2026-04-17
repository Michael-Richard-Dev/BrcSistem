using System.Collections.Generic;

namespace BRCSISTEM.Domain.Models
{
    /// <summary>
    /// Resumo da execucao de inativar_movimentos_duplicados_notas (Python).
    /// </summary>
    public sealed class InactivateDuplicatesResult
    {
        public List<long> RequestedIds  { get; set; } = new List<long>();

        /// <summary>IDs encontrados na tabela (independente de status).</summary>
        public List<long> FoundIds      { get; set; } = new List<long>();

        /// <summary>IDs efetivamente inativados (status ATIVO → INATIVO).</summary>
        public List<long> InactivatedIds { get; set; } = new List<long>();

        public int TotalInactivated => InactivatedIds?.Count ?? 0;
    }
}
