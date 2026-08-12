using System;
using System.Collections.Generic;

namespace POS.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool TrackInventory { get; set; } 
        public bool IsComposite { get; set; }
        
        public long? LinkedInventoryItemId { get; set; }
        public InventoryItem? LinkedInventoryItem { get; set; }

        public ICollection<ProductRecipeItem> RecipeItems { get; set; } = new List<ProductRecipeItem>();
    }
}