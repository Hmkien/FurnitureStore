using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
    {
        public void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {
            builder.ToTable("ReturnRequests");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Code).IsRequired().HasMaxLength(30);
            builder.HasIndex(r => r.Code).IsUnique();
            builder.Property(r => r.Reason).IsRequired().HasMaxLength(500);
            builder.Property(r => r.Note).HasMaxLength(500);

            builder.HasOne(r => r.Order).WithMany().HasForeignKey(r => r.OrderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(r => r.Items).WithOne(i => i.ReturnRequest).HasForeignKey(i => i.ReturnRequestId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ReturnItemConfiguration : IEntityTypeConfiguration<ReturnItem>
    {
        public void Configure(EntityTypeBuilder<ReturnItem> builder)
        {
            builder.ToTable("ReturnItems");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.ProductName).IsRequired().HasMaxLength(250);
            builder.HasOne(i => i.Variant).WithMany().HasForeignKey(i => i.VariantId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class WarrantyClaimConfiguration : IEntityTypeConfiguration<WarrantyClaim>
    {
        public void Configure(EntityTypeBuilder<WarrantyClaim> builder)
        {
            builder.ToTable("WarrantyClaims");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Code).IsRequired().HasMaxLength(30);
            builder.HasIndex(w => w.Code).IsUnique();
            builder.Property(w => w.ProductName).IsRequired().HasMaxLength(250);
            builder.Property(w => w.CustomerName).IsRequired().HasMaxLength(150);
            builder.Property(w => w.CustomerPhone).IsRequired().HasMaxLength(20);
            builder.Property(w => w.IssueDescription).IsRequired().HasMaxLength(1000);
            builder.Property(w => w.Note).HasMaxLength(500);

            builder.HasOne(w => w.Order).WithMany().HasForeignKey(w => w.OrderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(w => w.User).WithMany().HasForeignKey(w => w.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(w => w.Variant).WithMany().HasForeignKey(w => w.VariantId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
