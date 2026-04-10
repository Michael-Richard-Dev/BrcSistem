namespace BRCSISTEM.Application.Models
{
    public sealed class SavePackagingRequest
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public bool IsBrcEnabled { get; set; }

        public string Status { get; set; }

        public string ActorUserName { get; set; }
    }
}
