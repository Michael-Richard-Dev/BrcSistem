namespace BRCSISTEM.Domain.Models
{
    public sealed class RemoveProductionOutputResult
    {
        public string Number { get; set; }

        public int RemovedItems { get; set; }

        public int RemovedMovements { get; set; }
    }
}
