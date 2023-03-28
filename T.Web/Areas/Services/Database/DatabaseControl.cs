using System.Net;
using T.Library.Model;

namespace T.Web.Areas.Services.Database
{
    public interface IDatabaseControl
    {
        Task<DatabaseControlResponse> GetDbInfo();
        Task<bool> DeleteDb();
        Task<bool> MigrateDb();
    }
    public class DatabaseControl : IDatabaseControl
    {
        private readonly HttpClient _httpClient;

        public DatabaseControl(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeleteDb()
        {
            var response = await _httpClient.DeleteAsync($"api/DbManage");
            return response.IsSuccessStatusCode? true : false;
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
    }
}
