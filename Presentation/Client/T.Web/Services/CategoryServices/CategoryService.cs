using T.Library.Model.Response;
using System.Net.Http.Json;
using T.Library.Model.Common;
using System.Net.Http.Headers;
using T.Library.Model.Interface;

namespace T.Web.Services.CategoryService
{
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

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            var result = await _httpClient.GetAsync($"api/category/{APIRoutes.GetAll}");
            return await result.Content.ReadFromJsonAsync<List<Category>>();
        }

        public async Task<ServiceResponse<Category>> GetCategoryByIdAsync(int categoryId)
        {
            var result = await _httpClient.GetAsync($"api/category/{categoryId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<Category>>();
        }

        [Obsolete("This method is not needed in this project.")]
        public Task<ServiceResponse<Category>> GetCategoryByNameAsync(string categoryName)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> CreateCategoryAsync(Category category)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/category", category);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> UpdateCategoryAsync(Category category)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/category", category);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/category/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
    }
}
