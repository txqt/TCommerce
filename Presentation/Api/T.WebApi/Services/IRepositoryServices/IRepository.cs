using Microsoft.EntityFrameworkCore;

namespace T.WebApi.Services.IRepositoryServices
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> Table { get; }
        IQueryable<T> Query { get; }
        Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);
        Task<T> GetByIdAsync(int id, bool includeDeleted = false);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}
