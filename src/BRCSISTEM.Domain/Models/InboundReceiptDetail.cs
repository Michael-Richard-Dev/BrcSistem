using System;

namespace BRCSISTEM.Domain.Models
{
    public sealed class InboundReceiptDetail
    {
        public InboundReceiptDetail()
        {
            Items = Array.Empty<InboundReceiptItemDetail>();
        }

        public string Number { get; set; }

        public string SupplierCode { get; set; }

        public string SupplierName { get; set; }

        public string WarehouseCode { get; set; }

        public string WarehouseName { get; set; }

        public string EmissionDate { get; set; }

        public string ReceiptDateTime { get; set; }

        public string Status { get; set; }

        public int Version { get; set; }

        public string LockedBy { get; set; }

        public InboundReceiptItemDetail[] Items { get; set; }
    }
}
