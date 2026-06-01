using FurnitureStore.API.Configuration;
using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Data
{
    public class ApplicationDbContext : AuditDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<UserAddress> UserAddresses { get; set; } = default!;
        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<UserRole> UserRoles { get; set; } = default!;
        public DbSet<Permision> Permisions { get; set; } = default!;
        public DbSet<RolePermision> RolePermisions { get; set; } = default!;

        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ProductVariant> ProductVariants { get; set; } = default!;
        public DbSet<ProductImage> ProductImages { get; set; } = default!;

        public DbSet<Cart> Carts { get; set; } = default!;
        public DbSet<CartItem> CartItems { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;

        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<Banner> Banners { get; set; } = default!;
        public DbSet<Slide> Slides { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;
        public DbSet<Coupon> Coupons { get; set; } = default!;
        public DbSet<MembershipTier> MembershipTiers { get; set; } = default!;
        public DbSet<WishlistItem> WishlistItems { get; set; } = default!;

        public DbSet<StockMovement> StockMovements { get; set; } = default!;
        public DbSet<MediaFile> MediaFiles { get; set; } = default!;
        public DbSet<ReturnRequest> ReturnRequests { get; set; } = default!;
        public DbSet<ReturnItem> ReturnItems { get; set; } = default!;
        public DbSet<WarrantyClaim> WarrantyClaims { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserAddressConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new PermisionRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermisionConfiguration());

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductVariantConfiguration());
            modelBuilder.ApplyConfiguration(new ProductImageConfiguration());

            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());

            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new BannerConfiguration());
            modelBuilder.ApplyConfiguration(new SlideConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new CouponConfiguration());
            modelBuilder.ApplyConfiguration(new MembershipTierConfiguration());
            modelBuilder.ApplyConfiguration(new WishlistConfiguration());

            modelBuilder.ApplyConfiguration(new StockMovementConfiguration());
            modelBuilder.ApplyConfiguration(new MediaFileConfiguration());

            modelBuilder.ApplyConfiguration(new ReturnRequestConfiguration());
            modelBuilder.ApplyConfiguration(new ReturnItemConfiguration());
            modelBuilder.ApplyConfiguration(new WarrantyClaimConfiguration());
        }
    }
}

