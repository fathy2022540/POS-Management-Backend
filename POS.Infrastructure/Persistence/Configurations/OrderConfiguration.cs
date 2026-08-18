using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(x => x.OrderNumber)
                .IsUnique();

            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasMany(x => x.OrderItems)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Payments)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.StockTransactions)
                .WithOne(x => x.ReferenceOrder)
                .HasForeignKey(x => x.ReferenceOrderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Ignore(x => x.DomainEvents);
        }
    }
}
