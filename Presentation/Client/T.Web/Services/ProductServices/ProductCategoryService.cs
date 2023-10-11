using T.Library.Model.Response;
using T.Library.Model;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using T.Library.Model.Interface;
using T.Library.Model.Common;

namespace T.Web.Services.ProductService
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductCategoryService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<ProductCategory>> GetProductCategoryById(int productCategoryId)
        {
            var result = await _httpClient.GetAsync($"api/product/category/{productCategoryId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductCategory>>();
        }

        public async Task<ServiceResponse<List<ProductCategory>>> GetProductCategoriesByProductId(int productId)
        {
            var result = await _httpClient.GetAsync($"api/product/{productId}/categories");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductCategory>>>();
        }

        public async Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product/category", productCategory);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/product/category", productCategory);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductCategoryAsync(int productCategoryId)
        {
            var result = await _httpClient.DeleteAsync($"api/product/category/{productCategoryId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
    }
}
