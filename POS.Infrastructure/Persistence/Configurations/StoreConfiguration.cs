using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence.Configurations
{
    public class StoreConfiguration : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.ToTable("Stores");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Location)
                .HasMaxLength(250)
                .IsRequired();

            builder.HasMany(x => x.SourceTransfers)
                .WithOne(x => x.SourceStore)
                .HasForeignKey(x => x.SourceStoreId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.DestinationTransfers)
                .WithOne(x => x.DestinationStore)
                .HasForeignKey(x => x.DestinationStoreId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
