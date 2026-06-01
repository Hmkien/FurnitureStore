using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.ToTable("UserAddresses");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.ReceiverName).IsRequired().HasMaxLength(150);
            builder.Property(a => a.ReceiverPhone).IsRequired().HasMaxLength(20);
            builder.Property(a => a.AddressLine).IsRequired().HasMaxLength(500);
            builder.Property(a => a.Label).HasMaxLength(100);

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => a.UserId);
        }
    }
}
