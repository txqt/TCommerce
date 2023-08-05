using Microsoft.EntityFrameworkCore;
using T.WebApi.Database.ConfigurationDatabase;

namespace T.WebApi.Services.IRepositoryServices
{
    public class RepositoryService<T> : IRepository<T> where T : class
    {
        private readonly DatabaseContext _context;
        public RepositoryService(DatabaseContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public async Task CreateAsync(T entity)
        {
            await Table.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                Table.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await Table.FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
