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
        Task<ServiceResponse<bool>> EditProduct(ProductUpdateViewModel product);
        Task<ServiceResponse<bool>> DeleteProduct(int id);
        Task<ServiceResponse<Product>> Get(int id);
        Task<ServiceResponse<List<ProductAttribute>>> GetAllAttribute(int id);
        Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValue(int productAttributeMappingId);
        Task<ProductAttributeMappingModel> PrepareProductAttributeMappingModelAsync(ProductAttributeMappingModel model,
            Product product, ProductAttributeMapping productAttributeMapping);
        Task<List<ProductAttributeMappingModel>> PrepareProductAttributeMappingListModelAsync(
            Product product);
    }
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
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



        public async Task<ServiceResponse<bool>> CreateProduct(Product product)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/product/create", product);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        public async Task<ServiceResponse<bool>> DeleteProduct(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/product/delete/{id}");
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

        public async Task<List<ProductAttributeMappingModel>> PrepareProductAttributeMappingListModelAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get product attribute mappings

            var result = (await _productAttributeMappingService
                                .GetProductAttributeMappingByProductId(product.Id)).Data;

            var pamList = _mapper.Map<List<ProductAttributeMappingModel>>(result);

            //var pamList = result.Select(x => new ProductAttributeMappingModel
            //                    {
            //                        Id = x.Id,
            //                        ProductId = x.ProductId,
            //                        ProductAttributeId = x.ProductId,
            //                        ProductAttributeName = x.ProductAttribute.Name,
            //                        AvailableProductAttributes = null,
            //                        TextPrompt = x.TextPrompt,
            //                        IsRequired = x.IsRequired,
            //                        DisplayOrder = x.DisplayOrder,
            //                        ValidationMinLength = x.ValidationMinLength,
            //                        ValidationMaxLength = x.ValidationMaxLength,
            //                        ValidationFileAllowedExtensions = x.ValidationFileAllowedExtensions,
            //                        ValidationFileMaximumSize = x.ValidationFileMaximumSize,
            //                        DefaultValue = x.DefaultValue,
            //                    }).ToList();


            return pamList;
        }

        public async Task<ProductAttributeMappingModel> PrepareProductAttributeMappingModelAsync(ProductAttributeMappingModel model, Product product, ProductAttributeMapping productAttributeMapping)
        {
            if (productAttributeMapping != null)
            {
                //fill in model values from the entity
                model ??= new ProductAttributeMappingModel
                {
                    Id = productAttributeMapping.Id
                };
                _mapper.Map(productAttributeMapping, model);
                model.ProductAttributeName = (await _productAttributeService.Get(productAttributeMapping.ProductAttributeId)).Data.Name;
                model.ProductAttributeId = productAttributeMapping.ProductAttributeId;
                model.TextPrompt = productAttributeMapping.TextPrompt;
                model.IsRequired = productAttributeMapping.IsRequired;
                model.DisplayOrder = productAttributeMapping.DisplayOrder;
                model.ValidationMinLength = productAttributeMapping.ValidationMinLength;
                model.ValidationMaxLength = productAttributeMapping.ValidationMaxLength;
                model.ValidationFileAllowedExtensions = productAttributeMapping.ValidationFileAllowedExtensions;
                model.ValidationFileMaximumSize = productAttributeMapping.ValidationFileMaximumSize;
                model.DefaultValue = productAttributeMapping.DefaultValue;
            }

            model.ProductId = product.Id;

            //prepare available product attributes
            model.AvailableProductAttributes = (await _productAttributeService.GetAll()).Select(productAttribute => new SelectListItem
            {
                Text = productAttribute.Name,
                Value = productAttribute.Id.ToString()
            }).ToList();

            return model;
        }
    }
}
