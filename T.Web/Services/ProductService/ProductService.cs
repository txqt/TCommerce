using Microsoft.AspNetCore.WebUtilities;
using System.Drawing;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;

namespace T.Web.Services.ProductService
{
    public interface IProductService
    {
        Task<PagingResponse<Product>> GetAll(ProductParameters productParameters);
        Task<ServiceResponse<bool>> CreateProduct(Product product);
        Task<ServiceResponse<bool>> EditProduct(ProductUpdateViewModel product);
        Task<ServiceResponse<Product>> Get(int id);
    }
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductService(JsonSerializerOptions options, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            Console.WriteLine(accessToken);
        }

        public async Task<ServiceResponse<bool>> CreateProduct(Product product)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product/create", product);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> EditProduct(ProductUpdateViewModel product)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product/edit", product);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<Product>> Get(int id)
        {
            var result = await _httpClient.GetAsync($"api/product/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>();
        }

        public async Task<PagingResponse<Product>> GetAll(ProductParameters productParameters)
        {

            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = productParameters.PageNumber.ToString(),
                ["searchText"] = productParameters.searchText == null ? "" : productParameters.searchText,
                ["pageSize"] = productParameters.PageSize.ToString(),
                ["orderBy"] = productParameters.OrderBy,
            };
            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString($"api/product/get-all", queryStringParam));

            response.EnsureSuccessStatusCode();

            var metaData = JsonSerializer
                .Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options);

            var stream = await response.Content.ReadAsStreamAsync();
            var pagingResponse = new PagingResponse<Product>
            {
                Items = await JsonSerializer.DeserializeAsync<List<Product>>(stream, _options),
                MetaData = metaData
            };

            return pagingResponse;
        }
    }
}
