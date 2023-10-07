using T.Library.Model.Response;
using T.Library.Model;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace T.Web.Services.ProductService
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategory>> GetAllAsync();
        Task<ServiceResponse<ProductCategory>> Get(int id);
        Task<ServiceResponse<List<ProductCategory>>> GetByProductId(int productId);
        Task<ServiceResponse<List<ProductCategory>>> GetByCategoryId(int categoryId);
        Task<ServiceResponse<bool>> AddOrEdit(ProductCategory productCategory);
        Task<ServiceResponse<bool>> Delete(int id);
    }
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductCategoryService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            //var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<ServiceResponse<bool>> AddOrEdit(ProductCategory productCategory)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product-category/{APIRoutes.AddOrEdit}", productCategory);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/product-category/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<ProductCategory>> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/product-category/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductCategory>>();
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            var result = await _httpClient.GetAsync($"api/product-category/{APIRoutes.GetAll}");
            return await result.Content.ReadFromJsonAsync<List<ProductCategory>>();
        }

        public async Task<ServiceResponse<List<ProductCategory>>> GetByCategoryId(int categoryId)
        {
            var result = await _httpClient.GetAsync($"api/product-category/{categoryId}/by-categoryId");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductCategory>>>();
        }

        public async Task<ServiceResponse<List<ProductCategory>>> GetByProductId(int productId)
        {
            var result = await _httpClient.GetAsync($"api/product-category/{productId}/by-productId");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductCategory>>>();
        }
    }
}
