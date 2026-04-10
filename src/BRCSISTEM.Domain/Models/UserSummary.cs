namespace BRCSISTEM.Domain.Models
{
    public sealed class UserSummary
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string UserType { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }
    }
}
