namespace POS.Domain.Entities
{
    public class InventoryTransfer : BaseEntity
    {
        public long SourceStoreId { get; set; }
        public Store SourceStore { get; set; } = null!;

        public long DestinationStoreId { get; set; }
        public Store DestinationStore { get; set; } = null!;

        public long InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = null!;

        public decimal Quantity { get; set; }
        public string Status { get; set; } = "Pending";
    }
}