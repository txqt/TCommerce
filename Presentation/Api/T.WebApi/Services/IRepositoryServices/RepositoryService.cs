using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.WebApi.Database;
using T.WebApi.Extensions;

namespace T.WebApi.Services.IRepositoryServices
{
    public class RepositoryService<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DatabaseContext _context;
        public DbSet<T> Table { get; private set; }
        public IQueryable<T> Query
        {
            get
            {
                return Table;
            }
        }
        public RepositoryService(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Table = _context.Set<T>();
        }


        public async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = true)
        {
            IQueryable<T> query = Table;

            // Áp dụng bộ lọc soft delete nếu cần
            query = query.ApplySoftDeleteFilter(includeDeleted);

            // Trả về kết quả sau khi áp dụng bộ lọc
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, bool includeDeleted = true)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Giá trị của 'id' phải lớn hơn 0.", nameof(id));
            }

            return await Query.ApplySoftDeleteFilter(includeDeleted).FirstOrDefaultAsync(entity => entity.Id == Convert.ToInt32(id));
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await Table.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Giá trị của 'id' phải lớn hơn 0.", nameof(id));
            }

            var entity = await Table.FindAsync(id);
            if (entity != null)
            {
                if (entity is ISoftDeletedEntity deletableEntity)
                {
                    deletableEntity.Deleted = true; // Thực hiện soft delete
                    _context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    Table.Remove(entity); // Thực hiện hard delete nếu không hỗ trợ soft delete
                }
                await _context.SaveChangesAsync();
            }
        }
        public async Task BulkCreateAsync(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            await Table.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
