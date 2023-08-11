using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.Paging;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace T.Web.Services.ProductService
{
    public interface IProductService
    {
        Task<PagingResponse<Product>> GetAll(ProductParameters productParameters);
        Task<ServiceResponse<bool>> CreateProduct(Product product);
        Task<ServiceResponse<bool>> EditProduct(ProductModel product);
        Task<ServiceResponse<bool>> DeleteProduct(int id);
        Task<ServiceResponse<Product>> Get(int id);
        Task<ServiceResponse<List<ProductPicture>>> GetProductPicturesByProductIdAsync(int productId);
        Task<ServiceResponse<List<ProductAttribute>>> GetAllAttribute(int id);
        Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId);
        Task<ServiceResponse<bool>> DeleteProductImage(int productId, int pictureId);
        Task<ServiceResponse<bool>> DeleteAllProductImage(int productId);

    }
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _options;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeMappingService _productAttributeMappingService;
        private readonly IMapper _mapper;
        public ProductService(JsonSerializerOptions options, HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IProductAttributeService productAttributeService, IProductAttributeMappingService productAttributeMappingService, IMapper mapper)
        {
            _options = options;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("jwt");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _productAttributeService = productAttributeService;
            _productAttributeMappingService = productAttributeMappingService;
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

                var result = await _httpClient.PostAsync($"api/product/{productId}/add-new-picture", content);
                result.EnsureSuccessStatusCode();

                return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            }
        }

        public async Task<ServiceResponse<bool>> CreateProduct(Product product)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product/create", product);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteAllProductImage(int productId)
        {
            var result = await _httpClient.DeleteAsync($"api/product/{productId}/delete-all-picture");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProduct(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/product/delete/{id}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductImage(int productId, int pictureId)
        {
            var result = await _httpClient.DeleteAsync($"api/product/{productId}/delete-picture/{pictureId}");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> EditProduct(ProductModel product)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/product/edit", product);
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

        public async Task<ServiceResponse<List<ProductAttribute>>> GetAllAttribute(int id)
        {
            var result = await _httpClient.GetAsync($"api/product/{id}/attributes");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductAttribute>>>();
        }

        public async Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValue(int productAttributeMappingId)
        {
            var result = await _httpClient.GetAsync($"api/product/product-attribute-mapping/{productAttributeMappingId}/value");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductAttributeValue>>>();
        }

        public async Task<ServiceResponse<List<ProductPicture>>> GetProductPicturesByProductIdAsync(int productId)
        {
            var result = await _httpClient.GetAsync($"api/product/{productId}/all-pictures");
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<ProductPicture>>>();
        }
    }
}
