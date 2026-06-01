using FurnitureStore.API.Middlewares;
using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FurnitureStore.API.Data
{
    public abstract class AuditDbContext : DbContext
    {
        private readonly RequestContext? _requestContext;

        public AuditDbContext(DbContextOptions options) : base(options)
        {
            _requestContext = this.GetService<RequestContext>();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            var now = DateTime.Now;
            var currentUserId = GetCurrentUserId();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Created = now;
                    entry.Entity.LastModified = now;
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.LastModifiedBy = currentUserId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(BaseEntity.Created)).IsModified = false;
                    entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;

                    entry.Entity.LastModified = now;
                    entry.Entity.LastModifiedBy = currentUserId;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        private Guid GetCurrentUserId()
        {
            if (_requestContext?.IsAuthenticated == true)
            {
                return _requestContext.GetUserId();
            }

            return Guid.Empty;
        }
    }
}

