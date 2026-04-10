namespace BRCSISTEM.Application.Models
{
    public sealed class UserTypeSaveResult
    {
        public string SavedName { get; set; }

        public bool IsNewRecord { get; set; }

        public int UpdatedUsersCount { get; set; }
    }
}
