using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence.Configurations
{
    public class ProductRecipeItemConfiguration : IEntityTypeConfiguration<ProductRecipeItem>
    {
        public void Configure(EntityTypeBuilder<ProductRecipeItem> builder)
        {
            builder.ToTable("ProductRecipeItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.QuantityRequired)
                .HasPrecision(18, 4);

            builder.HasOne(x => x.Product)
                .WithMany(x => x.RecipeItems)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.InventoryItem)
                .WithMany(x => x.AssociatedRecipes)
                .HasForeignKey(x => x.InventoryItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
