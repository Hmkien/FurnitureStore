using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
    {
        public void Configure(EntityTypeBuilder<MediaFile> builder)
        {
            builder.ToTable("MediaFiles");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.FileName).IsRequired().HasMaxLength(260);
            builder.Property(m => m.Url).IsRequired().HasMaxLength(500);
            builder.Property(m => m.ContentType).HasMaxLength(100);
            builder.Property(m => m.Folder).HasMaxLength(100);
        }
    }
}
