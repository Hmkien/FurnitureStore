using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Size).HasMaxLength(100);
            builder.Property(v => v.Material).HasMaxLength(100);
            builder.Property(v => v.Color).HasMaxLength(100);

            builder.Property(v => v.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(v => v.SkuVariant)
                .IsRequired()
                .HasMaxLength(120);

            builder.HasIndex(v => v.SkuVariant).IsUnique();
        }
    }
}
