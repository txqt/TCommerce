using System.Text.Json;
using T.Library.Model.Common;

namespace T.Web.Services.HomePageServices
{
    public interface IHomePageService
    {
        Task<List<Category>> ShowCategoriesOnHomePage();
    }
    public class HomePageService : IHomePageService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public HomePageService(HttpClient httpClient, JsonSerializerOptions options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<List<Category>> ShowCategoriesOnHomePage()
        {
            var result = await _httpClient.GetAsync($"api/home-page/all-product-categories");
            return await result.Content.ReadFromJsonAsync<List<Category>>();
        }
    }
}
