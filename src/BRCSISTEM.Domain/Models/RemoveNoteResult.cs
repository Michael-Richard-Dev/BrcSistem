namespace BRCSISTEM.Domain.Models
{
    public sealed class RemoveNoteResult
    {
        public string Number { get; set; }

        public string Supplier { get; set; }

        public int RemovedItems { get; set; }

        public int RemovedMovements { get; set; }
    }
}
