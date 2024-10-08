﻿using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Extensions;
using TCommerce.Core.Helpers;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Common;
using TCommerce.Data;
using TCommerce.Services.CacheServices;

namespace TCommerce.Services.IRepositoryServices
{
    public class RepositoryService<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        public DbSet<T> Table { get; private set; }
        public IQueryable<T> Query
        {
            get
            {
                return Table;
            }
        }
        public RepositoryService(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Table = _context.Set<T>();
            _cacheService = cacheService;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }


        public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? func = null, string? cacheKey = null, bool includeDeleted = true)
        {
            if(cacheKey is null)
            {
                cacheKey = CacheKeysDefault<T>.AllPrefix + includeDeleted.ToString();
            }

            // Attempt to get data from cache
            var cachedData = _cacheService.GetData<IEnumerable<T>>(cacheKey);
            Console.WriteLine(cacheKey);
            if (cachedData != null)
            {
                // Return data from cache
                return cachedData;
            }
            else
            {
                // Data not found in cache, retrieve from the database
                IQueryable<T> query = Table;
                query = query.ApplySoftDeleteFilter(includeDeleted);
                query = func != null ? func(query) : query;
                var data = await query.ToListAsync();

                // Store data in cache with an expiration time (adjust as needed)
                _cacheService.SetData(cacheKey, data, DateTimeOffset.UtcNow.AddMinutes(10));

                return data;
            }
        }

        public async Task<T?> GetByIdAsync(int id, string? cacheKey = null, bool includeDeleted = true)
        {
            if (id <= 0)
            {
                return null;
            }

            if (cacheKey is null)
            {
                cacheKey = CacheKeysDefault<T>.ByIdPrefix + id + includeDeleted.ToString();
            }

            Console.WriteLine(cacheKey);
            // Attempt to get data from cache
            var cachedData = _cacheService.GetData<T>(cacheKey);

            if (cachedData != null)
            {
                // Return data from cache
                return cachedData;
            }
            else
            {
                // Data not found in cache, retrieve from the database
                var entity = await Query.ApplySoftDeleteFilter(includeDeleted).FirstOrDefaultAsync(e => e.Id == id);

                if (entity != null)
                {
                    // Store data in cache with an expiration time (adjust as needed)
                    _cacheService.SetData(cacheKey, entity, DateTimeOffset.UtcNow.AddMinutes(10));
                }

                return entity;
            }
        }

        public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids, string? cacheKey = null, bool includeDeleted = true)
        {
            if (ids == null || !ids.Any())
            {
                return new List<T>();
            }

            cacheKey ??= CacheKeysDefault<T>.ByIdsPrefix + string.Join("-", ids) + includeDeleted.ToString();

            Console.WriteLine(cacheKey);

            // Attempt to get data from cache
            var cachedData = _cacheService.GetData<IEnumerable<T>>(cacheKey);

            if (cachedData != null)
            {
                // Return data from cache
                return cachedData;
            }
            else
            {
                var query = Query.ApplySoftDeleteFilter(includeDeleted).Where(entity => ids.Contains(entity.Id));
                var data = await query.ToListAsync();

                // Store data in cache with an expiration time (adjust as needed)
                _cacheService.SetData(cacheKey, data, DateTimeOffset.UtcNow.AddMinutes(10));

                return data;
            }
        }



        public async Task CreateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            // Add entity to the database
            await Table.AddAsync(entity);
            await _context.SaveChangesAsync();
            ClearCacheForAll();
        }


        public async Task UpdateAsync(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            // Update entity in the database
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            ClearCacheForEntity(entity.Id);
            ClearCacheForAll();
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
                ClearCacheForEntity(id);
                ClearCacheForAll();
            }
        }

        public async Task BulkCreateAsync(IEnumerable<T> entities)
        {
            ArgumentNullException.ThrowIfNull(entities);

            await Table.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
        public async Task BulkDeleteAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                throw new ArgumentException("Danh sách ID không được rỗng.", nameof(ids));
            }

            var entitiesToDelete = await Table.Where(entity => ids.Contains(entity.Id)).ToListAsync();

            foreach (var entity in entitiesToDelete)
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
            }

            await _context.SaveChangesAsync();
        }

        // Hàm xóa cache cho một bản ghi cụ thể
        private void ClearCacheForEntity(int id)
        {
            _cacheService.RemoveData(CacheKeysDefault<T>.ByIdPrefix + id + true.ToString());
            _cacheService.RemoveData(CacheKeysDefault<T>.ByIdPrefix + id + false.ToString());
        }

        // Hàm xóa cache cho toàn bộ dữ liệu
        private void ClearCacheForAll()
        {
            _cacheService.RemoveData(CacheKeysDefault<T>.AllPrefix + true.ToString());
            _cacheService.RemoveData(CacheKeysDefault<T>.AllPrefix + false.ToString());
        }
    }
}
