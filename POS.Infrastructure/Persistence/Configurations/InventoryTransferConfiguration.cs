using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence.Configurations
{
    public class InventoryTransferConfiguration : IEntityTypeConfiguration<InventoryTransfer>
    {
        public void Configure(EntityTypeBuilder<InventoryTransfer> builder)
        {
            builder.ToTable("InventoryTransfers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity)
                .HasPrecision(18, 4);

            builder.Property(x => x.Status)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(x => x.SourceStore)
                .WithMany(x => x.SourceTransfers)
                .HasForeignKey(x => x.SourceStoreId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.DestinationStore)
                .WithMany(x => x.DestinationTransfers)
                .HasForeignKey(x => x.DestinationStoreId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.InventoryItem)
                .WithMany(x => x.InventoryTransfers)
                .HasForeignKey(x => x.InventoryItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
