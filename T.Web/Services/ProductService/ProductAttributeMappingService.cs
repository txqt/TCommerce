using System.Net.Http;
using T.Library.Model.Response;
using T.Library.Model;
using System.Net.Http.Headers;
using System.Text.Json;

namespace T.Web.Services.ProductService
{
    public interface IProductAttributeMappingService
    {
        Task<ServiceResponse<ProductAttributeMapping>> GetProductAttributeMapping(int id);
        Task<ServiceResponse<List<ProductAttributeMapping>>> GetProductAttributeMappingByProductId(int id);
        Task<ServiceResponse<bool>> AddOrUpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping);
        Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValuesAsync(int productAttributeMappingId);

    }
    public class ProductAttributeMappingService : IProductAttributeMappingService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductAttributeMappingService(JsonSerializerOptions options, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<ServiceResponse<ProductAttributeMapping>> GetProductAttributeMapping(int id)
        {
            var result = await _httpClient.GetAsync($"api/product-attribute-mapping/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductAttributeMapping>>();
        }
        public async Task<ServiceResponse<bool>> AddOrUpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product-attribute-mapping/add-or-edit-product-attribute-mapping", productAttributeMapping);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<List<ProductAttributeMapping>>> GetProductAttributeMappingByProductId(int id)
        {
            var result = await _httpClient.GetAsync($"api/product-attribute-mapping/get-by-product-id/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductAttributeMapping>>>();
        }

        public async Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValuesAsync(int productAttributeMappingId)
        {
            var result = await _httpClient.GetAsync($"api/product-attribute-mapping/{productAttributeMappingId}/value");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductAttributeValue>>>();
        }
    }
}
