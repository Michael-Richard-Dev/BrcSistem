namespace BRCSISTEM.Domain.Models
{
    public sealed class DuplicateNoteMovementDetail
    {
        public long MovementId { get; set; }

        public string NoteNumber { get; set; }

        public string Supplier { get; set; }

        public string Material { get; set; }

        public string MaterialName { get; set; }

        public string Lot { get; set; }

        public string Warehouse { get; set; }

        public decimal Quantity { get; set; }

        public string MovementDate { get; set; }

        public string Reason { get; set; }
    }
}
