using System;

namespace BRCSISTEM.Domain.Models
{
    public sealed class MaterialRequisitionDetail
    {
        public MaterialRequisitionDetail()
        {
            Items = Array.Empty<MaterialRequisitionItemDetail>();
        }

        public string Number { get; set; }

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string MovementDateTime { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public string LockedBy { get; set; }

        public MaterialRequisitionItemDetail[] Items { get; set; }
    }
}
