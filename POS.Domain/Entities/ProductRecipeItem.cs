using System;

namespace POS.Domain.Entities
{
    public class ProductRecipeItem : BaseEntity
    {
        public long ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public long InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = null!;

        public decimal QuantityRequired { get; set; }
    }
}