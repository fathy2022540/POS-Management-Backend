namespace POS.Domain.Entities
{
    public class Store : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<InventoryTransfer> SourceTransfers { get; set; } = new List<InventoryTransfer>();
        public ICollection<InventoryTransfer> DestinationTransfers { get; set; } = new List<InventoryTransfer>();
    }
}