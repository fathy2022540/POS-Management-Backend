using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence.Configurations
{
    public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.ToTable("StockTransactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.QuantityChanged)
                .HasPrecision(18, 4);

            builder.Property(x => x.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Remarks)
                .HasMaxLength(500);

            builder.HasOne(x => x.InventoryItem)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.InventoryItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ReferenceOrder)
                .WithMany(x => x.StockTransactions)
                .HasForeignKey(x => x.ReferenceOrderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
