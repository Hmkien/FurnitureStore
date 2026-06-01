using FurnitureStore.API.Models.Query;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace FurnitureStore.API.Extensions
{
    /// <summary>
    /// Query Helper sử dụng Fluent API Pattern để xử lý query dễ dàng
    /// </summary>
    public static class QueryHelper
    {
        /// <summary>
        /// Tạo QueryBuilder từ IQueryable
        /// </summary>
        public static QueryBuilder<T> ApplyQuery<T>(this IQueryable<T> source, BaseQuery query) where T : class
        {
            return new QueryBuilder<T>(source, query);
        }

        /// <summary>
        /// Áp dụng phân trang cho IQueryable
        /// </summary>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, BaseQuery query)
        {
            return source
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);
        }

        /// <summary>
        /// Áp dụng sắp xếp động cho IQueryable
        /// </summary>
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string? sortBy, bool sortDesc)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return source;

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                return source;

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var methodName = sortDesc ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.PropertyType },
                source.Expression,
                Expression.Quote(orderByExpression)
            );

            return source.Provider.CreateQuery<T>(resultExpression);
        }

        /// <summary>
        /// Áp dụng lọc theo ngày
        /// </summary>
        public static IQueryable<T> ApplyDateFilter<T>(
            this IQueryable<T> source,
            DateTime? fromDate,
            DateTime? toDate,
            Expression<Func<T, DateTime>> dateSelector)
        {
            if (fromDate.HasValue)
            {
                var parameter = dateSelector.Parameters[0];
                var body = Expression.GreaterThanOrEqual(dateSelector.Body, Expression.Constant(fromDate.Value));
                var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
                source = source.Where(lambda);
            }

            if (toDate.HasValue)
            {
                var parameter = dateSelector.Parameters[0];
                var body = Expression.LessThanOrEqual(dateSelector.Body, Expression.Constant(toDate.Value));
                var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
                source = source.Where(lambda);
            }

            return source;
        }

        /// <summary>
        /// Tìm kiếm theo nhiều trường (sử dụng Expression)
        /// </summary>
        public static IQueryable<T> ApplySearch<T>(
            this IQueryable<T> source,
            string? keyword,
            params Expression<Func<T, string>>[] searchProperties)
        {
            if (string.IsNullOrWhiteSpace(keyword) || searchProperties.Length == 0)
                return source;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var property in searchProperties)
            {
                var propertyExpression = Expression.Invoke(property, parameter);
                var nullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
                
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(
                    propertyExpression,
                    containsMethod!,
                    Expression.Constant(keyword)
                );

                var andExpression = Expression.AndAlso(nullCheck, containsExpression);

                combinedExpression = combinedExpression == null
                    ? andExpression
                    : Expression.OrElse(combinedExpression, andExpression);
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                source = source.Where(lambda);
            }

            return source;
        }

        /// <summary>
        /// Tìm kiếm động theo SearchIn[] từ BaseQuery
        /// </summary>
        public static IQueryable<T> ApplyDynamicSearch<T>(
            this IQueryable<T> source,
            string? keyword,
            string[]? searchIn)
        {
            if (string.IsNullOrWhiteSpace(keyword) || searchIn == null || searchIn.Length == 0)
                return source;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var fieldName in searchIn)
            {
                var property = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                
                // Bỏ qua property không tồn tại hoặc không phải string
                if (property == null || property.PropertyType != typeof(string))
                    continue;

                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
                
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var containsExpression = Expression.Call(
                    propertyAccess,
                    containsMethod!,
                    Expression.Constant(keyword)
                );

                var andExpression = Expression.AndAlso(nullCheck, containsExpression);

                combinedExpression = combinedExpression == null
                    ? andExpression
                    : Expression.OrElse(combinedExpression, andExpression);
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                source = source.Where(lambda);
            }

            return source;
        }

        /// <summary>
        /// Thực thi query và trả về DataTableJson
        /// </summary>
        public static async Task<DataTableJson<T>> ToDataTableAsync<T>(
            this IQueryable<T> source,
            BaseQuery query,
            CancellationToken cancellationToken = default) where T : class
        {
            var totalRecords = await source.CountAsync(cancellationToken);
            var data = await source.Paginate(query).ToListAsync(cancellationToken);

            return new DataTableJson<T>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize)
            };
        }

        /// <summary>
        /// Thực thi query và trả về DataTableJson với projection
        /// </summary>
        public static async Task<DataTableJson<TResult>> ToDataTableAsync<T, TResult>(
            this IQueryable<T> source,
            BaseQuery query,
            Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default) where T : class where TResult : class
        {
            var totalRecords = await source.CountAsync(cancellationToken);
            var data = await source
                .Paginate(query)
                .Select(selector)
                .ToListAsync(cancellationToken);

            return new DataTableJson<TResult>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize)
            };
        }
    }

    /// <summary>
    /// Query Builder sử dụng Fluent API Pattern
    /// </summary>
    public class QueryBuilder<T> where T : class
    {
        private IQueryable<T> _query;
        private readonly BaseQuery _baseQuery;

        public QueryBuilder(IQueryable<T> query, BaseQuery baseQuery)
        {
            _query = query;
            _baseQuery = baseQuery;
        }

        /// <summary>
        /// Áp dụng tìm kiếm động theo SearchIn từ BaseQuery
        /// </summary>
        public QueryBuilder<T> WithDynamicSearch()
        {
            _query = _query.ApplyDynamicSearch(_baseQuery.Keyword, _baseQuery.SearchIn);
            return this;
        }

        /// <summary>
        /// Áp dụng tìm kiếm theo nhiều trường (dùng khi muốn fix cứng các field)
        /// </summary>
        public QueryBuilder<T> WithSearch(params Expression<Func<T, string>>[] searchProperties)
        {
            _query = _query.ApplySearch(_baseQuery.Keyword, searchProperties);
            return this;
        }

        /// <summary>
        /// Áp dụng tìm kiếm tùy chỉnh
        /// </summary>
        public QueryBuilder<T> WithCustomSearch(Func<IQueryable<T>, string?, IQueryable<T>> searchFunc)
        {
            if (!string.IsNullOrWhiteSpace(_baseQuery.Keyword))
            {
                _query = searchFunc(_query, _baseQuery.Keyword);
            }
            return this;
        }

        /// <summary>
        /// Áp dụng lọc theo điều kiện
        /// </summary>
        public QueryBuilder<T> WithFilter(Expression<Func<T, bool>> predicate)
        {
            _query = _query.Where(predicate);
            return this;
        }

        /// <summary>
        /// Áp dụng lọc có điều kiện
        /// </summary>
        public QueryBuilder<T> WithFilterIf(bool condition, Expression<Func<T, bool>> predicate)
        {
            if (condition)
            {
                _query = _query.Where(predicate);
            }
            return this;
        }

        /// <summary>
        /// Áp dụng lọc theo ngày
        /// </summary>
        public QueryBuilder<T> WithDateFilter(Expression<Func<T, DateTime>> dateSelector)
        {
            _query = _query.ApplyDateFilter(_baseQuery.FromDate, _baseQuery.ToDate, dateSelector);
            return this;
        }

        /// <summary>
        /// Áp dụng sắp xếp
        /// </summary>
        public QueryBuilder<T> WithSort(string? defaultSortBy = null)
        {
            var sortBy = _baseQuery.SortBy ?? defaultSortBy;
            _query = _query.ApplySort(sortBy, _baseQuery.SortDesc);
            return this;
        }

        /// <summary>
        /// Áp dụng sắp xếp tùy chỉnh
        /// </summary>
        public QueryBuilder<T> WithCustomSort(Func<IQueryable<T>, IQueryable<T>> sortFunc)
        {
            if (string.IsNullOrWhiteSpace(_baseQuery.SortBy))
            {
                _query = sortFunc(_query);
            }
            else
            {
                _query = _query.ApplySort(_baseQuery.SortBy, _baseQuery.SortDesc);
            }
            return this;
        }

        /// <summary>
        /// Include related entities
        /// </summary>
        public QueryBuilder<T> Include(Expression<Func<T, object>> navigationProperty)
        {
            if (_query is IQueryable<T> efQuery)
            {
                _query = efQuery.Include(navigationProperty);
            }
            return this;
        }

        /// <summary>
        /// Include nhiều related entities
        /// </summary>
        public QueryBuilder<T> IncludeMultiple(params Expression<Func<T, object>>[] navigationProperties)
        {
            foreach (var property in navigationProperties)
            {
                Include(property);
            }
            return this;
        }

        /// <summary>
        /// Áp dụng phân trang và trả về kết quả
        /// </summary>
        public async Task<DataTableJson<T>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return await _query.ToDataTableAsync(_baseQuery, cancellationToken);
        }

        /// <summary>
        /// Áp dụng phân trang và projection, sau đó trả về kết quả
        /// </summary>
        public async Task<DataTableJson<TResult>> ExecuteAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default) where TResult : class
        {
            return await _query.ToDataTableAsync(_baseQuery, selector, cancellationToken);
        }

        /// <summary>
        /// Lấy IQueryable để xử lý thêm
        /// </summary>
        public IQueryable<T> GetQuery()
        {
            return _query;
        }

        /// <summary>
        /// Lấy danh sách không phân trang
        /// </summary>
        public async Task<List<T>> ToListAsync(CancellationToken cancellationToken = default)
        {
            return await _query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Đếm tổng số bản ghi
        /// </summary>
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _query.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Kiểm tra có tồn tại hay không
        /// </summary>
        public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        {
            return await _query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Lấy bản ghi đầu tiên hoặc null
        /// </summary>
        public async Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await _query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}

