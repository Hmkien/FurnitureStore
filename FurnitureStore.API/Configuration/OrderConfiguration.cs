using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.OrderCode)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(o => o.OrderCode).IsUnique();

            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(o => o.ShippingFee).HasColumnType("decimal(18,2)");
            builder.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.CouponCode).HasMaxLength(50);
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

            builder.Property(o => o.ReceiverName).IsRequired().HasMaxLength(150);
            builder.Property(o => o.ReceiverPhone).IsRequired().HasMaxLength(20);
            builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
            builder.Property(o => o.Note).HasMaxLength(500);

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductName).IsRequired().HasMaxLength(250);
            builder.Property(i => i.VariantInfo).HasMaxLength(300);
            builder.Property(i => i.Price).HasColumnType("decimal(18,2)");
            builder.Property(i => i.LineTotal).HasColumnType("decimal(18,2)");

            builder.HasOne(i => i.Variant)
                .WithMany()
                .HasForeignKey(i => i.VariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
