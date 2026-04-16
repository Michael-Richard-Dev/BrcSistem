namespace BRCSISTEM.Domain.Models
{
    public sealed class DivergentLotEntry
    {
        public long MovementId { get; set; }

        public string DocumentNumber { get; set; }

        public string Material { get; set; }

        public string MaterialName { get; set; }

        /// <summary>Lote registrado em movimentos_estoque (orfao — divergente).</summary>
        public string LotInMovement { get; set; }

        /// <summary>Lote correto conforme notas_itens.</summary>
        public string LotInNoteItem { get; set; }

        public string Supplier { get; set; }

        public string Warehouse { get; set; }

        public decimal Quantity { get; set; }

        public decimal QuantityInNoteItem { get; set; }

        public string MovementUser { get; set; }

        public string NoteUser { get; set; }

        public string CreatedAt { get; set; }
    }
}
