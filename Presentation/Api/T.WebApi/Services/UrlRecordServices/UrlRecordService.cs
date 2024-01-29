using Microsoft.EntityFrameworkCore;
using T.Library.Helpers;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Seo;
using T.WebApi.Helpers;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.UrlRecordServices
{
    public interface IUrlRecordService : IUrlRecordServiceCommon
    {
        Task SaveSlugAsync<T>(T entity, string slug) where T : BaseEntity;
        Task<string> ValidateSlug<T>(T entity, string seName, string name, bool ensureNotEmpty = false) where T : BaseEntity;
        Task<string> GetSeNameAsync<T>(T entity) where T : BaseEntity;
        Task<string> GetActiveSlugAsync(int entityId, string entityName);
    }
    public class UrlRecordService : IUrlRecordService
    {
        private readonly IRepository<UrlRecord> _urlRecordRepository;

        public UrlRecordService(IRepository<UrlRecord> urlRecordRepository)
        {
            _urlRecordRepository = urlRecordRepository;
        }

        public async Task<List<UrlRecord>> GetAllAsync()
        {
            return (await _urlRecordRepository.GetAllAsync()).ToList();
        }

        public async Task<UrlRecord> GetByIdAsync(int id)
        {
            return await _urlRecordRepository.GetByIdAsync(id);
        }

        public async Task<ServiceResponse<bool>> CreateUrlRecordAsync(UrlRecord model)
        {
            await _urlRecordRepository.CreateAsync(model);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> UpdateUrlRecordAsync(UrlRecord model)
        {
            await _urlRecordRepository.UpdateAsync(model);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteUrlRecordByIdAsync(int id)
        {
            await _urlRecordRepository.DeleteAsync(id);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<UrlRecord> GetBySlugAsync(string slug)
        {
            var query = from ur in _urlRecordRepository.Table
                        where ur.Slug == slug
                        orderby ur.IsActive descending, ur.Id
                        select ur;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<string> GetActiveSlugAsync(int entityId, string entityName)
        {
            var query = from ur in _urlRecordRepository.Table
                        where ur.EntityId == entityId &&
                              ur.EntityName == entityName &&
                              ur.IsActive
                        orderby ur.Id descending
                        select ur.Slug;

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<string> GetSeNameAsync<T>(T entity) where T : BaseEntity
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entityName = entity.GetType().Name;

            return await GetActiveSlugAsync(entity.Id, entityName);
        }

        public virtual async Task SaveSlugAsync<T>(T entity, string slug) where T : BaseEntity
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entityId = entity.Id;
            var entityName = entity.GetType().Name;

            var query = from ur in _urlRecordRepository.Table
                        where ur.EntityId == entityId &&
                              ur.EntityName == entityName
                        orderby ur.Id descending
                        select ur;
            var allUrlRecords = await query.ToListAsync();
            var activeUrlRecord = allUrlRecords.FirstOrDefault(x => x.IsActive);

            UrlRecord nonActiveRecordWithSpecifiedSlug = null;

            if (activeUrlRecord == null && !string.IsNullOrWhiteSpace(slug))
            {
                // find in non-active records with the specified slug
                var innerNonActiveRecordWithSpecifiedSlug = allUrlRecords
                    .FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.IsActive);

                if (innerNonActiveRecordWithSpecifiedSlug != null)
                {
                    // mark non-active record as active
                    innerNonActiveRecordWithSpecifiedSlug.IsActive = true;
                    await UpdateUrlRecordAsync(innerNonActiveRecordWithSpecifiedSlug);
                }
                else
                {
                    // new record
                    var urlRecord = new UrlRecord
                    {
                        EntityId = entityId,
                        EntityName = entityName,
                        Slug = slug,
                        IsActive = true
                    };
                    await CreateUrlRecordAsync(urlRecord);
                }
            }

            if (activeUrlRecord != null && string.IsNullOrWhiteSpace(slug))
            {
                // disable the previous active URL record
                activeUrlRecord.IsActive = false;
                await UpdateUrlRecordAsync(activeUrlRecord);
            }

            if (activeUrlRecord == null || string.IsNullOrWhiteSpace(slug))
                return;

            // it should not be the same slug as in active URL record
            if (activeUrlRecord.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                return;

            // find in non-active records with the specified slug
            nonActiveRecordWithSpecifiedSlug = allUrlRecords
                .FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.IsActive);

            if (nonActiveRecordWithSpecifiedSlug != null)
            {
                // mark non-active record as active
                nonActiveRecordWithSpecifiedSlug.IsActive = true;
                await UpdateUrlRecordAsync(nonActiveRecordWithSpecifiedSlug);

                // disable the previous active URL record
                activeUrlRecord.IsActive = false;
                await UpdateUrlRecordAsync(activeUrlRecord);
            }
            else
            {
                // insert new record
                // we do not update the existing record because we should track all previously entered slugs
                // to ensure that URLs will work fine
                var urlRecord = new UrlRecord
                {
                    EntityId = entityId,
                    EntityName = entityName,
                    Slug = slug,
                    IsActive = true
                };
                await CreateUrlRecordAsync(urlRecord);

                // disable the previous active URL record
                activeUrlRecord.IsActive = false;
                await UpdateUrlRecordAsync(activeUrlRecord);
            }
            return;
        }

        public async Task<string> ValidateSlug<T>(T entity, string seName, string name, bool ensureNotEmpty = false) where T : BaseEntity
        {
            // Kiểm tra và sử dụng tên nếu seName không được chỉ định
            if (string.IsNullOrWhiteSpace(seName) && !string.IsNullOrWhiteSpace(name))
            {
                seName = SlugConverter.ConvertToSlug(name);
            }

            // Giới hạn độ dài tối đa cho seName
            int maxLength = 255; // Độ dài tối đa được định nghĩa;
            if (seName.Length > maxLength)
            {
                seName = seName.Substring(0, maxLength);
            }

            // Xác nhận seName không rỗng
            if (ensureNotEmpty && string.IsNullOrWhiteSpace(seName))
            {
                seName = entity.GetType().Name.ToString(); // Sử dụng định danh của thực thể làm seName
            }

            // Đảm bảo seName không trùng lặp
            int counter = 1;
            string originalSeName = seName;
            while (await _urlRecordRepository.Table.AnyAsync(ur => ur.Slug == seName))
            {
                seName = $"{originalSeName}-{counter}";
                counter++;
            }

            return seName;
        }


    }
}
