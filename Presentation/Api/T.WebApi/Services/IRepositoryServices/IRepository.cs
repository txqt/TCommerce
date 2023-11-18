using Microsoft.EntityFrameworkCore;

namespace T.WebApi.Services.IRepositoryServices
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> Table { get; }
        IQueryable<T> Query { get; }
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = true);
        Task<T> GetByIdAsync(int id, bool includeDeleted = true);
        Task CreateAsync(T entity);
        Task BulkCreateAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
