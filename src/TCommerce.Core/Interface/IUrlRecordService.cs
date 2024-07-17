using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Seo;

namespace TCommerce.Core.Interface
{
    public interface IUrlRecordService
    {
        Task<List<UrlRecord>> GetAllAsync();
        Task<UrlRecord?> GetByIdAsync(int id);
        Task<ServiceResponse<bool>> CreateUrlRecordAsync(UrlRecord model);
        Task<ServiceResponse<bool>> UpdateUrlRecordAsync(UrlRecord model);
        Task<ServiceResponse<bool>> DeleteUrlRecordByIdAsync(int id);
        Task<UrlRecord> GetBySlugAsync(string slug);
        Task SaveSlugAsync<T>(T entity, string slug) where T : BaseEntity;
        Task<string> ValidateSlug<T>(T entity, string seName, string name, bool ensureNotEmpty = false) where T : BaseEntity;
        Task<string> GetSeNameAsync<T>(T entity) where T : BaseEntity;
        Task<string> GetActiveSlugAsync(int entityId, string entityName);
    }
}
