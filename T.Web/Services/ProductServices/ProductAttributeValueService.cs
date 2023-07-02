using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.Response;

namespace T.Web.Services.ProductService
{
    public interface IProductAttributeValueService
    {
        Task<ServiceResponse<ProductAttributeValue>> GetProductAttributeValuesByIdAsync(int id);
        Task<ServiceResponse<bool>> AddOrUpdateProductAttributeValue(ProductAttributeValue productAttributeValue);
        Task<ServiceResponse<bool>> DeleteProductAttrbuteValue(int id);
    }
    public class ProductAttributeValueService : IProductAttributeValueService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductAttributeValueService(JsonSerializerOptions options, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<ServiceResponse<bool>> AddOrUpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product-attribute-value/{APIRoutes.AddOrEdit}", productAttributeValue);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttrbuteValue(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/product-attribute-value/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<ProductAttributeValue>> GetProductAttributeValuesByIdAsync(int id)
        {
            var result = await _httpClient.GetAsync($"api/product-attribute-value/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductAttributeValue>>();
        }
    }
}
