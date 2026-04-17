namespace BRCSISTEM.Domain.Models
{
    public sealed class ChangeDateResult
    {
        public string Supplier { get; set; } = string.Empty;

        public int HeaderRowsUpdated { get; set; }
        public int MovementRowsUpdated { get; set; }

        public int HeaderRowsAffected
        {
            get => HeaderRowsUpdated;
            set => HeaderRowsUpdated = value;
        }

        public int MovementRowsAffected
        {
            get => MovementRowsUpdated;
            set => MovementRowsUpdated = value;
        }
    }
}