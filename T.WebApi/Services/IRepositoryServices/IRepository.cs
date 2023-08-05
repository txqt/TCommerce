using Microsoft.EntityFrameworkCore;

namespace T.WebApi.Services.IRepositoryServices
{
    public interface IRepository<T> where T : class
    {
        DbSet<T> Table { get; }
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetById(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }

}
