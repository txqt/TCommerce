using System.Net;
using System.Net.Http.Headers;
using T.Library.Model;
using T.Library.Model.Response;

namespace T.Web.Services.Database
{
    public interface IDatabaseControl
    {
        Task<DatabaseControlResponse> GetDbInfo();
        Task<bool> DeleteDb();
        Task<bool> MigrateDb();
        Task<bool> SeedData();
    }
    public class DatabaseControl : IDatabaseControl
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseControl(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            //var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<bool> DeleteDb()
        {
            var response = await _httpClient.DeleteAsync($"api/DbManage");
            return response.IsSuccessStatusCode ? true : false;
        }

        public async Task<DatabaseControlResponse> GetDbInfo()
        {
            var result = await _httpClient.GetFromJsonAsync<DatabaseControlResponse>($"api/DbManage");
            return result;
        }

        public async Task<bool> MigrateDb()
        {
            var result = await _httpClient.PostAsync($"api/DbManage/Migrate", null);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> SeedData()
        {
            var result = await _httpClient.GetAsync($"api/DbManage/Seed-Data");
            return result.IsSuccessStatusCode;
        }
    }
}
