

namespace POS.Domain.Entities
{
    public class InventoryItem : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        
        public ICollection<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
        public ICollection<ProductRecipeItem> AssociatedRecipes { get; set; } = new List<ProductRecipeItem>();
    }
}