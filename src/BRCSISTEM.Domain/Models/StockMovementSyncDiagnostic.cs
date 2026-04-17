using System.Collections.Generic;

namespace BRCSISTEM.Domain.Models
{
    public sealed class StockMovementSyncDiagnostic
    {
        public int ActiveMovements { get; set; }

        public int MissingNotes { get; set; }

        public int MissingTransferOutputs { get; set; }

        public int MissingTransferInputs { get; set; }

        public int MissingRequisitions { get; set; }

        public int MissingProductionOutputs { get; set; }

        public IReadOnlyCollection<StockMovementSyncItem> Items { get; set; } = new StockMovementSyncItem[0];

        public int TotalMissing
        {
            get
            {
                return MissingNotes
                    + MissingTransferOutputs
                    + MissingTransferInputs
                    + MissingRequisitions
                    + MissingProductionOutputs;
            }
        }
    }
}
