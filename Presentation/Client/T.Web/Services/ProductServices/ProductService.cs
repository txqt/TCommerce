using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Paging;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace T.Web.Services.ProductService
{
    public interface IProductService : IProductServiceCommon
    {
        Task<PagingResponse<Product>> GetAll(ProductParameters productParameters);
    }
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _options;
        private readonly IProductAttributeService _productAttributeService;
        //private readonly IProductAttributeMappingService _productAttributeMappingService;
        private readonly IMapper _mapper;
        public ProductService(JsonSerializerOptions options, HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IProductAttributeService productAttributeService, /*IProductAttributeMappingService productAttributeMappingService,*/ IMapper mapper)
        {
            _options = options;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _productAttributeService = productAttributeService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId)
        {
            using (var content = new MultipartFormDataContent())
            {
                foreach (var image in ListImages)
                {
                    var fileContent = new StreamContent(image.OpenReadStream());
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "formFiles",
                        FileName = image.FileName
                    };
                    content.Add(fileContent);
                }

                var result = await _httpClient.PostAsync($"api/product/{productId}/upload-picture", content);
                result.EnsureSuccessStatusCode();

                return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            }
        }

        public async Task<ServiceResponse<bool>> CreateProductAsync(Product product)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product", product);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteAllProductImage(int productId)
        {
            var result = await _httpClient.DeleteAsync($"api/product/{productId}/delete-all-picture");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(int productId)
        {
            var result = await _httpClient.DeleteAsync($"api/product/{productId}/delete-all-picture");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductImage(int pictureMappingId)
        {
            var result = await _httpClient.DeleteAsync($"api/product/picture-mapping-id/{pictureMappingId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> EditProductAsync(ProductModel product)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/product", product);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<Product>> GetByIdAsync(int id)
        {
            var result = await _httpClient.GetAsync($"api/product/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<Product>>();
        }

        public async Task<PagingResponse<Product>> GetAll(ProductParameters productParameters)
        {

            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = productParameters.PageNumber.ToString(),
                ["searchText"] = productParameters.SearchText == null ? "" : productParameters.SearchText,
                ["pageSize"] = productParameters.PageSize.ToString(),
                ["orderBy"] = productParameters.OrderBy,
            };
            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString($"api/product/{APIRoutes.GetAll}", queryStringParam));

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

        public async Task<ServiceResponse<List<ProductPicture>>> GetProductPicturesByProductIdAsync(int productId)
        {
            var result = await _httpClient.GetAsync($"api/product/{productId}/pictures");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductPicture>>>();
        }

        public Task<List<Product>> GetAllNewestProduct()
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetRandomProduct()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetFirstImagePathByProductId(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<Product>> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<ProductAttribute>>> GetAllProductAttributeByProductIdAsync(int productId)
        {
            var result = await _httpClient.GetAsync($"api/product/{productId}/attributes");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductAttribute>>>();
        }
    }
}
