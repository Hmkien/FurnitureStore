using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.ToTable("StockMovements");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Note).HasMaxLength(300);

            builder.HasOne(m => m.Variant)
                .WithMany()
                .HasForeignKey(m => m.VariantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.VariantId);
        }
    }
}
