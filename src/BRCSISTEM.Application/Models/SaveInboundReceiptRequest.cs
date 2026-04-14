using System;

namespace BRCSISTEM.Application.Models
{
    public sealed class SaveInboundReceiptRequest
    {
        public SaveInboundReceiptRequest()
        {
            Items = Array.Empty<InboundReceiptItemInput>();
        }

        public string Number { get; set; }

        public string SupplierCode { get; set; }

        public string WarehouseCode { get; set; }

        public string EmissionDate { get; set; }

        public string ReceiptDateTime { get; set; }

        public string ActorUserName { get; set; }

        public InboundReceiptItemInput[] Items { get; set; }
    }
}
