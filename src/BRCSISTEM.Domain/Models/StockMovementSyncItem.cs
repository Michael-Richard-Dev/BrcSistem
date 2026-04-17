using System;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockMovementSyncItem
    {
        public string Category { get; set; }

        public string DocumentNumber { get; set; }

        public int DocumentItem { get; set; }

        public string MovementDate { get; set; }

        public string Warehouse { get; set; }

        public string DocumentSupplier { get; set; }

        public string LotSupplier { get; set; }

        public string Material { get; set; }

        public string Lot { get; set; }

        public decimal Quantity { get; set; }

        public string ExpirationDate { get; set; }

        public string DocumentType { get; set; }

        public string MovementType { get; set; }

        public string ProductUsed { get; set; }

        public string SupplierToWrite
        {
            get
            {
                return !string.IsNullOrWhiteSpace(DocumentSupplier)
                    ? DocumentSupplier
                    : (LotSupplier ?? string.Empty);
            }
        }
    }
}
