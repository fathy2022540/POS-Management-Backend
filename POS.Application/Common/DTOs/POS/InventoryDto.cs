namespace POS.Application.Common.DTOs
{
    public class InventoryLevelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
    }
    public class StoreDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

}