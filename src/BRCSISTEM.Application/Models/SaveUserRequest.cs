namespace BRCSISTEM.Application.Models
{
    public sealed class SaveUserRequest
    {
        public string OriginalUserName { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Password { get; set; }

        public string UserType { get; set; }

        public string Status { get; set; }

        public string ActorUserName { get; set; }
    }
}
