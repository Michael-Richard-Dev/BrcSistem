using System;

namespace BRCSISTEM.Domain.Models
{
    public sealed class ProductionOutputDetail
    {
        public ProductionOutputDetail()
        {
            Items = Array.Empty<ProductionOutputItemDetail>();
        }

        public string Number { get; set; }

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string Purpose { get; set; }

        public string Shift { get; set; }

        public string MovementDateTime { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public string LockedBy { get; set; }

        public ProductionOutputItemDetail[] Items { get; set; }
    }
}
