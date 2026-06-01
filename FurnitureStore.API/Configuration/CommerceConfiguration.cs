using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Comment).HasMaxLength(1000);
            builder.Property(r => r.ImageUrl).HasMaxLength(500);

            builder.HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Mỗi user chỉ đánh giá một sản phẩm một lần.
            builder.HasIndex(r => new { r.ProductId, r.UserId }).IsUnique();
        }
    }

    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Coupons");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
            builder.HasIndex(c => c.Code).IsUnique();
            builder.Property(c => c.Description).HasMaxLength(300);

            builder.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
            builder.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.MaxDiscount).HasColumnType("decimal(18,2)");
        }
    }

    public class MembershipTierConfiguration : IEntityTypeConfiguration<MembershipTier>
    {
        public void Configure(EntityTypeBuilder<MembershipTier> builder)
        {
            builder.ToTable("MembershipTiers");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Description).HasMaxLength(300);
            builder.Property(t => t.MinSpending).HasColumnType("decimal(18,2)");
            builder.Property(t => t.DiscountPercent).HasColumnType("decimal(5,2)");
        }
    }
}
