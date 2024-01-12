using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Seo;
using T.Web.Helpers;

namespace T.Web.Services.UrlRecordService
{
    public interface IUrlRecordService : IUrlRecordServiceCommon
    {

    }
    public class UrlRecordService : HttpClientHelper, IUrlRecordService
    {
        public readonly string defaultApiRoute = "api/url-records/";

        public UrlRecordService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<UrlRecord>> GetAllAsync()
        {
            return await GetAsync<List<UrlRecord>>(defaultApiRoute);
        }

        public async Task<UrlRecord> GetByIdAsync(int id)
        {
            return await GetAsync<UrlRecord>(defaultApiRoute + $"{id}");
        }

        public async Task<ServiceResponse<bool>> CreateUrlRecordAsync(UrlRecord model)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>(defaultApiRoute, model);
        }

        public async Task<ServiceResponse<bool>> UpdateUrlRecordAsync(UrlRecord model)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>(defaultApiRoute, model);
        }

        public async Task<ServiceResponse<bool>> DeleteUrlRecordByIdAsync(int id)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"{defaultApiRoute + id}");
        }

        public async Task<UrlRecord> GetBySlugAsync(string slug)
        {
            return await GetAsync<UrlRecord>(defaultApiRoute + $"slug/{slug}");
        }

        public Task<string> GetActiveSlugAsync(int entityId, string entityName)
        {
            throw new NotImplementedException();
        }
    }
}
