namespace BRCSISTEM.Application.Models
{
    public sealed class SaveProductRequest
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string ActorUserName { get; set; }
    }
}
