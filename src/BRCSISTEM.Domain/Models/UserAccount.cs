namespace BRCSISTEM.Domain.Models
{
    public sealed class UserAccount
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        public string UserType { get; set; }

        public string Status { get; set; }
    }
}
