namespace BRCSISTEM.Domain.Models
{
    public sealed class InventoryPermissions
    {
        public bool CanOpen { get; set; }

        public bool CanCount { get; set; }

        public bool CanClose { get; set; }

        public bool CanCancel { get; set; }
    }
}
