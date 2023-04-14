using Microsoft.AspNetCore.WebUtilities;
using System.Drawing;
using System.Text.Json;
using T.Library.Model;

namespace T.Web.Services.ProductService
{
    public interface IProductService
    {
        Task<PagingResponse<Product>> GetAll(ProductParameters productParameters);
    }
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        public ProductService(JsonSerializerOptions options, HttpClient httpClient)
        {
            _options = options;
            _httpClient = httpClient;
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
