using System;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockTransferDetail
    {
        public StockTransferDetail()
        {
            Items = Array.Empty<StockTransferItemDetail>();
        }

        public string Number { get; set; }

        public string OriginWarehouseCode { get; set; }

        public string OriginWarehouseName { get; set; }

        public string DestinationWarehouseCode { get; set; }

        public string DestinationWarehouseName { get; set; }

        public string MovementDateTime { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public string LockedBy { get; set; }

        public StockTransferItemDetail[] Items { get; set; }
    }
}
