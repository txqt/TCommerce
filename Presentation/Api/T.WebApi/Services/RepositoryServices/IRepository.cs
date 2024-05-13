using Microsoft.EntityFrameworkCore;

namespace T.WebApi.Services.IRepositoryServices
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> Table { get; }
        IQueryable<T> Query { get; }
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? func = null, string? cacheKey = null, bool includeDeleted = true);
        Task<T?> GetByIdAsync(int id, string? cacheKey = null, bool includeDeleted = true);
        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids, string? cacheKey = null, bool includeDeleted = true);
        Task CreateAsync(T entity);
        Task BulkCreateAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task BulkDeleteAsync(IEnumerable<int> ids);
    }
}
