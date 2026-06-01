using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureStore.API.Configuration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(280);
            builder.HasIndex(p => p.Slug).IsUnique();
            builder.Property(p => p.Summary).HasMaxLength(500);
            builder.Property(p => p.ThumbnailUrl).HasMaxLength(500);
            builder.Property(p => p.Category).HasMaxLength(120);
            builder.Property(p => p.Author).HasMaxLength(150);
        }
    }

    public class BannerConfiguration : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToTable("Banners");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
            builder.Property(b => b.ImageUrl).IsRequired().HasMaxLength(500);
            builder.Property(b => b.LinkUrl).HasMaxLength(500);
            builder.Property(b => b.Position).HasMaxLength(100);
        }
    }

    public class SlideConfiguration : IEntityTypeConfiguration<Slide>
    {
        public void Configure(EntityTypeBuilder<Slide> builder)
        {
            builder.ToTable("Slides");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ImageUrl).IsRequired().HasMaxLength(500);
            builder.Property(s => s.LinkUrl).HasMaxLength(500);
            builder.Property(s => s.Caption).HasMaxLength(300);
        }
    }
}
