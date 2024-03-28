using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Web.Extensions;
using T.Web.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace T.Web.Services.ProductService
{
    public interface IProductService : IProductServiceCommon
    {
        Task<PagingResponse<Product>> GetAll(ProductParameters productParameters);
        bool ProductIsAvailable(Product product, DateTime? dateTime = null);

    }
    public class ProductService : HttpClientHelper, IProductService
    {
        private readonly JsonSerializerOptions _options;
        private readonly IMapper _mapper;
        public ProductService(JsonSerializerOptions options, HttpClient httpClient, IMapper mapper) : base(httpClient)
        {
            _options = options;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> formFiles, int productId)
        {
            return await PutWithFormFileAsync<ServiceResponse<bool>>($"api/products/{productId}/pictures", formFiles);
        }

        public async Task<ServiceResponse<bool>> CreateProductAsync(ProductModel product)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>($"api/products", product);
        }

        public async Task<ServiceResponse<bool>> DeleteAllProductImage(int productId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/products/{productId}/pictures");
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(int productId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/products/{productId}");
        }

        public async Task<ServiceResponse<bool>> DeleteProductImage(int pictureMappingId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/products/pictures/{pictureMappingId}");
        }

        public async Task<ServiceResponse<bool>> EditProductAsync(ProductModel product)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/products", product);
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await GetAsync<Product>($"api/products/{id}");
        }

        public async Task<PagingResponse<Product>> GetAll(ProductParameters productParameters)
        {
            string queryParams = productParameters.ToQueryParameters(p => new
            {
                ids = p.ids != null ? string.Join(",", p.ids) : null,
                CategoryIds = p.CategoryIds != null ? string.Join(",", p.CategoryIds) : null,
                ManufacturerIds = p.ManufacturerIds != null ? string.Join(",", p.ManufacturerIds) : null,
                p.ExcludeFeaturedProducts,
                p.PriceMax,
                p.PriceMin,
                p.ProductTagId,
                p.SearchDescriptions,
                p.SearchManufacturerPartNumber,
                p.SearchSku,
                p.SearchProductTags,
                p.ShowHidden,
                p.OrderBy
            });

            var response = await GetAsync<List<Product>>("api/products?" + queryParams);

            var metaData = JsonSerializer
                .Deserialize<MetaData>(LastResponse.Headers.GetValues("X-Pagination").First(), new JsonSerializerOptions());

            //var stream = await LastResponse.Content.ReadAsStreamAsync();
            var pagingResponse = new PagingResponse<Product>
            {
                Items = response,
                MetaData = metaData
            };

            return pagingResponse;
        }

        public async Task<List<ProductPicture>> GetProductPicturesByProductIdAsync(int productId)
        {
            return await GetAsync<List<ProductPicture>>($"api/products/{productId}/pictures");
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

        public Task<Product> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductAttribute>> GetAllProductAttributeByProductIdAsync(int productId)
        {
            return await GetAsync<List<ProductAttribute>>($"api/products/{productId}/attributes");
        }

        public async Task<ServiceResponse<bool>> EditProductImageAsync(ProductPicture productPicture)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/products/update-picture", productPicture);
        }

        public async Task<List<Product>> GetAllProductsDisplayedOnHomepageAsync()
        {
            return await GetAsync<List<Product>>($"api/products/show-on-home-page");
        }

        public async Task<ServiceSuccessResponse<bool>> BulkDeleteProductsAsync(IEnumerable<int> productIds)
        {
            return await DeleteWithDataAsync<ServiceSuccessResponse<bool>>($"api/products/delete-list", productIds);
        }

        public async Task<ServiceResponse<bool>> DeleteRelatedProductAsync(int relatedProductId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/related-products/{relatedProductId}");
        }

        public async Task<List<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId1, bool showHidden = false)
        {
            return await GetAsync<List<RelatedProduct>>($"api/products/{productId1}/related-products");
        }

        public async Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId)
        {
            return await GetAsync<RelatedProduct>($"api/related-products/{relatedProductId}");
        }

        public async Task<ServiceResponse<bool>> CreateRelatedProductAsync(RelatedProduct relatedProduct)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>($"api/related-products", relatedProduct);
        }

        public async Task<ServiceResponse<bool>> UpdateRelatedProductAsync(RelatedProduct relatedProduct)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/related-products", relatedProduct);
        }

        public RelatedProduct FindRelatedProduct(IList<RelatedProduct> source, int productId1, int productId2)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetProductsByIdsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public bool ProductIsAvailable(Product product, DateTime? dateTime = null)
        {
            ArgumentNullException.ThrowIfNull(product);

            dateTime ??= DateTime.UtcNow;

            if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value > dateTime)
                return false;

            if (product.AvailableEndDateTimeUtc.HasValue && product.AvailableEndDateTimeUtc.Value < dateTime)
                return false;

            return true;
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> ids)
        {
            var queryString = ids.ToQueryParameters(p => new
            {
                ids = p != null ? string.Join(",", p) : null
            });
            return await GetAsync<List<Product>>($"api/products/get-by-ids?{queryString}");
        }

        public async Task<List<Product>> GetCategoryFeaturedProductsAsync(int categoryId)
        {
            return await GetAsync<List<Product>>($"api/products/features/by-categoryId/{categoryId}");
        }
    }
}
