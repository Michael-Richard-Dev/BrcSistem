namespace BRCSISTEM.Domain.Models
{
    public sealed class PermissionDefinition
    {
        public PermissionDefinition(string key, string title)
        {
            Key = key;
            Title = title;
        }

        public string Key { get; private set; }

        public string Title { get; private set; }
    }
}
