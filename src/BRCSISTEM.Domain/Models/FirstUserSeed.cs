namespace BRCSISTEM.Domain.Models
{
    public sealed class FirstUserSeed
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool HasValues
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName)
                    && !string.IsNullOrWhiteSpace(Name)
                    && !string.IsNullOrWhiteSpace(Password);
            }
        }
    }
}
