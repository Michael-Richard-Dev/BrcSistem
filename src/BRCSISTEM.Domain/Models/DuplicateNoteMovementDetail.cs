namespace BRCSISTEM.Domain.Models
{
    /// <summary>
    /// Espelha cada entrada de "propostas_detalhadas" no retorno de
    /// diagnosticar_movimentos_duplicados_notas (Python).
    /// </summary>
    public sealed class DuplicateNoteMovementDetail
    {
        public long   MovementId   { get; set; }
        public string NoteNumber   { get; set; }
        public string Supplier     { get; set; }
        public string Material     { get; set; }
        public string MaterialName { get; set; }
        public string Lot          { get; set; }
        public string Warehouse    { get; set; }
        public decimal Quantity    { get; set; }
        public string MovementDate { get; set; }
        public string CreatedAt    { get; set; }

        public string MovementUser { get; set; }
        public string NoteUser     { get; set; }

        /// <summary>Chave do motivo (como no Python). Ex.:
        /// "item_nao_pertence_versao_ativa_e_usuario_diferente"
        /// "duplicidade_exata_mesma_assinatura_item"</summary>
        public string ReasonCode   { get; set; }

        /// <summary>Motivo legivel (equivalente a _fmt_motivo da tela Python).</summary>
        public string ReasonLabel  { get; set; }

        /// <summary>Compat: se so uma razao, reflete ReasonLabel.</summary>
        public string Reason       { get; set; }

        /// <summary>Regra 2: ID que foi mantido no grupo da mesma assinatura.</summary>
        public long? KeepReferenceId { get; set; }
    }
}
