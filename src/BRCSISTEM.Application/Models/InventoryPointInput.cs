namespace BRCSISTEM.Application.Models
{
    public sealed class InventoryPointInput
    {
        public int Id { get; set; }

        public string PointName { get; set; }

        public string IpAddress { get; set; }

        public string ComputerName { get; set; }

        public string Status { get; set; }
    }
}
