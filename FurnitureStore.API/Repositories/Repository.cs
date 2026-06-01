using System.Linq.Expressions;
using FurnitureStore.API.Data;
using FurnitureStore.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.API.Repositories
{
    /// <summary>
    /// Hiện thực <see cref="IRepository{T}"/> trên EF Core.
    /// </summary>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public Task<T?> GetByIdAsync(Guid id) => _dbSet.FirstOrDefaultAsync(e => e.Id == id);

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => _dbSet.AnyAsync(predicate);

        public Task AddAsync(T entity) => _dbSet.AddAsync(entity).AsTask();

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
