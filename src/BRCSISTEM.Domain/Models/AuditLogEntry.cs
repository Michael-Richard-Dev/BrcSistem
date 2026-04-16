namespace BRCSISTEM.Domain.Models
{
    public sealed class AuditLogEntry
    {
        public long Id { get; set; }

        public string DateTime { get; set; }

        public string UserName { get; set; }

        public string Action { get; set; }

        public string Details { get; set; }
    }
}
