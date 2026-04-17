namespace BRCSISTEM.Domain.Models
{
    public sealed class OpenMovementLockSummary
    {
        public string Type { get; set; }

        public string DocumentNumber { get; set; }

        public string Supplier { get; set; }

        public string UserName { get; set; }

        public string LockedAt { get; set; }
    }
}
