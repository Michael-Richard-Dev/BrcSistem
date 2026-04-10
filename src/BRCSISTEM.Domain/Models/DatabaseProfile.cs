namespace BRCSISTEM.Domain.Models
{
    public sealed class DatabaseProfile
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Host { get; set; }

        public int Port { get; set; } = 5432;

        public string Database { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string Kind { get; set; } = "rede";

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Description))
                {
                    return Name;
                }

                return Name + " - " + Description;
            }
        }

        public DatabaseProfile Clone()
        {
            return new DatabaseProfile
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Host = Host,
                Port = Port,
                Database = Database,
                User = User,
                Password = Password,
                Kind = Kind,
            };
        }
    }
}
