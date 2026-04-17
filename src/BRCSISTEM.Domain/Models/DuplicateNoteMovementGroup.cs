using System.Collections.Generic;

namespace BRCSISTEM.Domain.Models
{
    /// <summary>
    /// Espelha um item de diagnosticar_movimentos_duplicados_notas (Python):
    /// uma NF com propostas consolidadas pelas Regra 1 (item nao pertence a versao
    /// ativa + usuario divergente) e Regra 2 (duplicidade exata de assinatura).
    /// </summary>
    public sealed class DuplicateNoteMovementGroup
    {
        public string NoteNumber    { get; set; }
        public string Supplier      { get; set; }
        public string NoteVersion   { get; set; }
        public string NoteUser      { get; set; }
        public string NoteDate      { get; set; }

        /// <summary>Total de movimentos ATIVOS da NF (nao so os propostos).</summary>
        public int TotalActiveMovements { get; set; }

        /// <summary>"a, b, c" (usuarios distintos dos movimentos ativos).</summary>
        public string ActiveMovementUsers { get; set; }

        /// <summary>Sumario textual da(s) regra(s) acionada(s). "" quando vazio.</summary>
        public string Reason { get; set; }

        public List<long> DuplicateMovementIds { get; set; } = new List<long>();

        /// <summary>Equivalente a "propostas_detalhadas" do Python.</summary>
        public List<DuplicateNoteMovementDetail> Details { get; set; } = new List<DuplicateNoteMovementDetail>();

        public int TotalDuplicates => DuplicateMovementIds?.Count ?? 0;

        public string DisplayLabel => $"{NoteNumber} / {Supplier}";
    }
}
