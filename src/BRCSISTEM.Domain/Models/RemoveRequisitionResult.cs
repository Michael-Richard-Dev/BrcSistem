namespace BRCSISTEM.Domain.Models
{
    public sealed class RemoveRequisitionResult
    {
        public string Number { get; set; }

        public int RemovedItems { get; set; }

        public int RemovedMovements { get; set; }
    }
}
