using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Library.Model;

namespace T.Web.Services.ProductService
{
    public interface IProductAttributeService
    {
        Task<List<ProductAttribute>> GetAll();
        Task<ServiceResponse<bool>> Create(ProductAttribute productAttribute);
        Task<ServiceResponse<bool>> Edit(ProductAttribute productAttribute);
        Task<ServiceResponse<bool>> Delete(int id);
        Task<ServiceResponse<ProductAttribute>> Get(int id);
    }
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductAttributeService(JsonSerializerOptions options, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<ServiceResponse<bool>> Create(ProductAttribute productAttribute)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product-attribute/create", productAttribute);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/product-attribute/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> Edit(ProductAttribute productAttribute)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product-attribute/edit", productAttribute);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<ProductAttribute>> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/product-attribute/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<ProductAttribute>>();
        }

        public async Task<List<ProductAttribute>> GetAll()
        {
            var response = await _httpClient.GetAsync($"api/product-attribute/{APIRoutes.GetAll}");

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<List<ProductAttribute>>(stream, _options);
        }
    }
}
