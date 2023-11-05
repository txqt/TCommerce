using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Library.Model;
using T.Library.Model.Interface;

namespace T.Web.Services.ProductService
{
    //public interface IProductAttributeService
    //{
    //    Task<List<ProductAttribute>> GetAll();
    //    Task<ServiceResponse<bool>> Create(ProductAttribute productAttribute);
    //    Task<ServiceResponse<bool>> Edit(ProductAttribute productAttribute);
    //    Task<ServiceResponse<bool>> Delete(int id);
    //    Task<ServiceResponse<ProductAttribute>> Get(int id);
    //}
    public class ProductAttributeService : IProductAttributeCommon
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductAttributeService(JsonSerializerOptions options, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            //var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        #region ProductAttribute
        public async Task<List<ProductAttribute>> GetAllProductAttributeAsync()
        {
            var response = await _httpClient.GetAsync($"api/product-attribute/{APIRoutes.GetAll}");

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<List<ProductAttribute>>(stream, _options);
        }

        public async Task<ServiceResponse<ProductAttribute>> GetProductAttributeByIdAsync(int id)
        {
            var result = await _httpClient.GetAsync($"api/product-attribute/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductAttribute>>();
        }

        public Task<ServiceResponse<ProductAttribute>> GetProductAttributeByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeAsync(ProductAttribute productAttribute)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product-attribute/create", productAttribute);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeAsync(ProductAttribute productAttribute)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product-attribute/edit", productAttribute);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeByIdAsync(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/product-attribute/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
        #endregion

        #region ProductAttributeMapping
        public async Task<ServiceResponse<ProductAttributeMapping>> GetProductAttributeMappingByIdAsync(int productAttributeMappingId)
        {
            var result = await _httpClient.GetAsync($"api/product/attribute/{productAttributeMappingId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductAttributeMapping>>();
        }

        public async Task<ServiceResponse<List<ProductAttributeMapping>>> GetProductAttributesMappingByProductIdAsync(int productId)
        {
            var result = await _httpClient.GetAsync($"api/product/{productId}/attribute");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductAttributeMapping>>>();
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product/attribute", productAttributeMapping);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/product/attribute", productAttributeMapping);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeMappingByIdAsync(int productAttributeId)
        {
            var result = await _httpClient.DeleteAsync($"api/product/attribute/{productAttributeId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
        #endregion

        #region ProductAttributeValue
        public async Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValuesAsync(int productAttributeMappingId)
        {
            var result = await _httpClient.GetAsync($"api/product/attribute/{productAttributeMappingId}/value");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductAttributeValue>>>();
        }

        public async Task<ServiceResponse<ProductAttributeValue>> GetProductAttributeValuesByIdAsync(int productAttributeValueId)
        {
            var result = await _httpClient.GetAsync($"api/product/attribute/value/{productAttributeValueId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductAttributeValue>>();
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product/attribute/value", productAttributeValue);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/product/attribute/value", productAttributeValue);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeValueAsync(int productAttributeValueId)
        {
            var result = await _httpClient.DeleteAsync($"api/product/attribute/value/{productAttributeValueId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
        #endregion
    }
}
