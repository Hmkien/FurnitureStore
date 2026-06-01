using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class PermisionRoleConfiguration : IEntityTypeConfiguration<Permision>
    {
        public void Configure(EntityTypeBuilder<Permision> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PermisionName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PermisionCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .HasMaxLength(255);
        }
    }
}
