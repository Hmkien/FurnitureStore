using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("Carts");

            builder.HasKey(c => c.Id);

            // Index unique có lọc: mỗi user một giỏ, nhưng cho phép nhiều giỏ guest (UserId NULL).
            builder.HasIndex(c => c.UserId).IsUnique().HasFilter("[UserId] IS NOT NULL");

            builder.Property(c => c.GuestToken).HasMaxLength(64);
            builder.HasIndex(c => c.GuestToken).IsUnique().HasFilter("[GuestToken] IS NOT NULL");

            builder.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(i => i.Id);

            builder.HasOne(i => i.Variant)
                .WithMany()
                .HasForeignKey(i => i.VariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => new { i.CartId, i.VariantId }).IsUnique();
        }
    }
}
