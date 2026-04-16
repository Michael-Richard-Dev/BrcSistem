namespace BRCSISTEM.Domain.Models
{
    public sealed class ShiftSummary
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}
