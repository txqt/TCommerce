using T.Library.Model.Common;
using T.Library.Model.Response;
using T.Library.Model.Security;

namespace T.Web.Services
{
    public interface IPermissionRecordService
    {
        Task<ServiceResponse<PermissionRecord>> GetMappingByPermissionRecordIdAsync(int permissionId);
    }
    public class PermissionRecordService : IPermissionRecordService
    {
        private readonly HttpClient _httpClient;
        public PermissionRecordService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<PermissionRecord>> GetMappingByPermissionRecordIdAsync(int permissionId)
        {
            var result = await _httpClient.GetAsync($"api/manage-access-control/permission/{permissionId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<PermissionRecord>>();
        }
    }
}
