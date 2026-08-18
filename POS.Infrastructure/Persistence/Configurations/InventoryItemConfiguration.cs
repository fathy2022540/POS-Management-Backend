using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence.Configurations
{
    public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItem> builder)
        {
            builder.ToTable("InventoryItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.UnitOfMeasure)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.CurrentStock)
                .HasPrecision(18, 4);

            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.InventoryItem)
                .HasForeignKey(x => x.InventoryItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.AssociatedRecipes)
                .WithOne(x => x.InventoryItem)
                .HasForeignKey(x => x.InventoryItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.LinkedProducts)
                .WithOne(x => x.LinkedInventoryItem)
                .HasForeignKey(x => x.LinkedInventoryItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.InventoryTransfers)
                .WithOne(x => x.InventoryItem)
                .HasForeignKey(x => x.InventoryItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Ignore(x => x.DomainEvents);
        }
    }
}
