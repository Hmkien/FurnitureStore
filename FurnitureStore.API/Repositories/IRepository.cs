using System.Linq.Expressions;
using FurnitureStore.API.Models.Entities;

namespace FurnitureStore.API.Repositories
{
    /// <summary>
    /// Repository chung cho các entity kế thừa <see cref="BaseEntity"/>.
    /// Gom logic truy cập dữ liệu để service không phụ thuộc trực tiếp vào DbContext.
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>Truy vấn gốc (chưa thực thi) để build query động/phân trang.</summary>
        IQueryable<T> Query();

        Task<T?> GetByIdAsync(Guid id);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);

        Task<int> SaveChangesAsync();
    }
}
