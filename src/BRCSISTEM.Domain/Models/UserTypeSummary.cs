namespace BRCSISTEM.Domain.Models
{
    public sealed class UserTypeSummary
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int PermissionCount { get; set; }

        public int TotalPermissionCount { get; set; }
    }
}
