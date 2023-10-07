using T.Library.Model.Response;
using System.Net.Http.Json;
using T.Library.Model.Common;
using System.Net.Http.Headers;

namespace T.Web.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<ServiceResponse<Category>> Get(int id);
        Task<ServiceResponse<bool>> AddOrEdit(Category category);
        Task<ServiceResponse<bool>> Delete(int id);
    }
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoryService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            //var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<ServiceResponse<bool>> AddOrEdit(Category category)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/category/{APIRoutes.AddOrEdit}", category);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/category/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<Category>> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/category/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<Category>>();
        }

        public async Task<List<Category>> GetAllAsync()
        {
            var result = await _httpClient.GetAsync($"api/category/{APIRoutes.GetAll}");
            return await result.Content.ReadFromJsonAsync<List<Category>>();
        }
    }
}
