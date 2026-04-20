namespace BRCSISTEM.Domain.Models
{
    public sealed class MainSidebarSnapshot
    {
        public MainSidebarSnapshot()
        {
            FifoEntries = new MainSidebarFifoEntry[0];
            CadastroRows = new MainSidebarCadastroRow[0];
            VolumeRows = new MainSidebarVolumeRow[0];
            AuditRows = new MainSidebarAuditRow[0];
            RecentAccesses = new MainSidebarUserAccessRow[0];
        }

        public MainSidebarFifoEntry[] FifoEntries { get; set; }

        public MainSidebarCadastroRow[] CadastroRows { get; set; }

        public MainSidebarVolumeRow[] VolumeRows { get; set; }

        public MainSidebarAuditRow[] AuditRows { get; set; }

        public int ActiveUsersCount { get; set; }

        public MainSidebarUserAccessRow[] RecentAccesses { get; set; }
    }

    public sealed class MainSidebarFifoEntry
    {
        public string LotCode { get; set; }

        public string LotName { get; set; }

        public string Material { get; set; }

        public string ExpirationDate { get; set; }

        public decimal Balance { get; set; }
    }

    public sealed class MainSidebarCadastroRow
    {
        public string TableName { get; set; }

        public int ActiveCount { get; set; }

        public int InactiveCount { get; set; }

        public int TotalCount { get; set; }

        public int? BrcCount { get; set; }
    }

    public sealed class MainSidebarVolumeRow
    {
        public string WarehouseDisplay { get; set; }

        public decimal Volume { get; set; }
    }

    public sealed class MainSidebarAuditRow
    {
        public string Label { get; set; }

        public int Count { get; set; }
    }

    public sealed class MainSidebarUserAccessRow
    {
        public string UserName { get; set; }

        public string LastAccessText { get; set; }
    }
}
