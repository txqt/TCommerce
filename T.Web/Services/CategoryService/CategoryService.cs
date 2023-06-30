using T.Library.Model.Response;
using System.Net.Http.Json;
using T.Library.Model.Common;

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

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
