using System;

namespace BRCSISTEM.Application.Models
{
    public sealed class RegisterInventoryCountRequest
    {
        public string InventoryNumber { get; set; }

        public int PointId { get; set; }

        public string WarehouseCode { get; set; }

        public string MaterialCode { get; set; }

        public string LotCode { get; set; }

        public decimal Quantity { get; set; }

        public string ActorUserName { get; set; }

        public string IpAddress { get; set; }

        public string ComputerName { get; set; }

        public string CountedAt { get; set; }

        public string OriginUid { get; set; }

        public void EnsureOriginUid()
        {
            if (string.IsNullOrWhiteSpace(OriginUid))
            {
                OriginUid = Guid.NewGuid().ToString("N");
            }
        }
    }
}
