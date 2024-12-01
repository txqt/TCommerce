using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Seo;
using TCommerce.Data.Helpers;
using TCommerce.Services.IRepositoryServices;

namespace TCommerce.Services.UrlRecordServices
{
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

        public async Task<UrlRecord?> GetByIdAsync(int id)
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

            var result = await query.FirstOrDefaultAsync();

            return result!;
        }

        public async Task<string> GetActiveSlugAsync(int entityId, string entityName)
        {
            var query = from ur in _urlRecordRepository.Table
                        where ur.EntityId == entityId &&
                              ur.EntityName == entityName &&
                              ur.IsActive
                        orderby ur.Id descending
                        select ur.Slug;

            return await query.FirstOrDefaultAsync() ?? string.Empty;
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

            UrlRecord? nonActiveRecordWithSpecifiedSlug = null;

            if (activeUrlRecord == null && !string.IsNullOrWhiteSpace(slug))
            {
                // find in non-active records with the specified slug
                var innerNonActiveRecordWithSpecifiedSlug = allUrlRecords
                    .FirstOrDefault(x => x.Slug is not null && x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.IsActive);

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
            if (activeUrlRecord.Slug is not null && activeUrlRecord.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                return;

            // find in non-active records with the specified slug
            nonActiveRecordWithSpecifiedSlug = allUrlRecords
                .FirstOrDefault(x =>x.Slug is not null && x.Slug.Equals(slug, StringComparison.InvariantCultureIgnoreCase) && !x.IsActive);

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
            if (string.IsNullOrWhiteSpace(seName) && !string.IsNullOrWhiteSpace(name))
            {
                seName = SlugConverter.ConvertToSlug(RemoveDiacritics(name));
            }

            int maxLength = 255;
            if (seName.Length > maxLength)
            {
                seName = seName.Substring(0, maxLength);
            }

            if (ensureNotEmpty && string.IsNullOrWhiteSpace(seName))
            {
                seName = entity.GetType().Name.ToString();
            }

            int counter = 1;
            string originalSeName = seName;
            while (await _urlRecordRepository.Table.AnyAsync(ur => ur.Slug == seName))
            {
                seName = $"{originalSeName}-{counter}";
                counter++;
            }

            return seName;
        }
        public static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            string result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            result = result.Replace("đ", "d").Replace("Đ", "D")
                           .Replace("ơ", "o").Replace("Ơ", "O")
                           .Replace("ư", "u").Replace("Ư", "U")
                           .Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("á", "a").Replace("â", "a").Replace("ă", "a")
                           .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ê", "e")
                           .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i")
                           .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ô", "o").Replace("ơ", "o")
                           .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ư", "u")
                           .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y")
                           .Replace("ç", "c")
                           .Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("í", "i").Replace("ó", "o")
                           .Replace("ú", "u").Replace("ñ", "n");

            return result;
        }

    }
}
