namespace BRCSISTEM.Domain.Models
{
    public sealed class PermissionCategory
    {
        public PermissionCategory(string name, PermissionDefinition[] permissions)
        {
            Name = name;
            Permissions = permissions ?? new PermissionDefinition[0];
        }

        public string Name { get; private set; }

        public PermissionDefinition[] Permissions { get; private set; }
    }
}
