namespace BRCSISTEM.Domain.Models
{
    public sealed class StockMovementSyncResult
    {
        public int ExpectedItems { get; set; }

        public int InsertedItems { get; set; }
    }
}
