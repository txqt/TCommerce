using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.Response;

namespace T.Web.Services.ProductService
{
    public interface IProductAttributeValueService
    {
        Task<List<ProductAttributeValue>> GetProductAttributeValuesAsync(int productAttributeMappingId);
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
        public async Task<List<ProductAttributeValue>> GetProductAttributeValuesAsync(int productAttributeMappingId)
        {
            var result = await _httpClient.GetAsync($"api/product-attribute-value/get-by-mapping-id/{productAttributeMappingId}");
            return await result.Content.ReadFromJsonAsync<List<ProductAttributeValue>>();
        }
    }
}
