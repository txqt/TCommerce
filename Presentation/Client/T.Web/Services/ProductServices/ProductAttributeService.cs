using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Web.Helpers;

namespace T.Web.Services.ProductService
{
    //public interface IProductAttributeService
    //{
    //    Task<List<ProductAttribute>> GetAll();
    //    Task<ServiceResponse<bool>> Create(ProductAttribute productAttribute);
    //    Task<ServiceResponse<bool>> Edit(ProductAttribute productAttribute);
    //    Task<ServiceResponse<bool>> Delete(int id);
    //    Task<ProductAttribute> Get(int id);
    //}
    public class ProductAttributeService : HttpClientHelper, IProductAttributeCommon
    {
        private readonly JsonSerializerOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductAttributeService(JsonSerializerOptions options, HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : base(httpClient)
        {
            _options = options;
            _httpContextAccessor = httpContextAccessor;
            //var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            //DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        #region ProductAttribute
        public async Task<List<ProductAttribute>> GetAllProductAttributeAsync()
        {
            return await GetAsync<List<ProductAttribute>>($"api/product-attributes");
        }

        public async Task<ProductAttribute> GetProductAttributeByIdAsync(int id)
        {
            return await GetAsync<ProductAttribute>($"api/product-attributes/{id}");
        }

        public Task<ProductAttribute> GetProductAttributeByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeAsync(ProductAttribute productAttribute)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>($"api/product-attributes", productAttribute);
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeAsync(ProductAttribute productAttribute)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/product-attributes", productAttribute);
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeByIdAsync(int id)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/product-attributes/{id}");
        }
        #endregion

        #region ProductAttributeMapping
        public async Task<ProductAttributeMapping> GetProductAttributeMappingByIdAsync(int productAttributeMappingId)
        {
            return await GetAsync<ProductAttributeMapping>($"api/product-attribute-mappings/{productAttributeMappingId}");
        }

        public async Task<List<ProductAttributeMapping>> GetProductAttributesMappingByProductIdAsync(int productId)
        {
            return await GetAsync<List<ProductAttributeMapping>>($"api/product-attribute-mappings/by-product-id/{productId}");
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>($"api/product-attribute-mappings", productAttributeMapping);
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/product-attribute-mappings", productAttributeMapping);
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeMappingByIdAsync(int productAttributeId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/product-attribute-mappings/{productAttributeId}");
        }
        #endregion

        #region ProductAttributeValue
        public async Task<List<ProductAttributeValue>> GetProductAttributeValuesByMappingIdAsync(int productAttributeMappingId)
        {
            return await GetAsync<List<ProductAttributeValue>>($"api/product-attribute-mappings/{productAttributeMappingId}/value");
        }

        public async Task<ProductAttributeValue> GetProductAttributeValuesByIdAsync(int productAttributeValueId)
        {
            return await GetAsync<ProductAttributeValue>($"api/product-attribute-mappings/value/{productAttributeValueId}");
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>($"api/product-attribute-mappings/value", productAttributeValue);
        }

        public async Task<ServiceResponse<bool>> UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/product-attribute-mappings/value", productAttributeValue);
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeValueAsync(int productAttributeValueId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/product-attribute-mappings/value/{productAttributeValueId}");
        }
        #endregion
    }
}
