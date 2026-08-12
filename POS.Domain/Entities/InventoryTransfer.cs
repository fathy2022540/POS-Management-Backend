
namespace POS.Domain.Entities
{
    public class InventoryTransfer : BaseEntity
    {
        public long SourceStoreId { get; set; }
        public long DestinationStoreId { get; set; }
        public long InventoryItemId { get; set; }
        public decimal Quantity { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled
    }
}