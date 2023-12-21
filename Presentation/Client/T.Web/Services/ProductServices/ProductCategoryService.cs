using T.Library.Model.Response;
using T.Library.Model;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using T.Library.Model.Interface;
using T.Library.Model.Common;
using T.Web.Helpers;

namespace T.Web.Services.ProductService
{
    public class ProductCategoryService : HttpClientHelper, IProductCategoryService
    {
        public ProductCategoryService(HttpClient httpClient) : base(httpClient)
        { 
        }

        public async Task<ProductCategory> GetProductCategoryById(int productCategoryId)
        {
            return await GetAsync<ProductCategory>($"api/product/category/{productCategoryId}");
        }

        public async Task<List<ProductCategory>> GetProductCategoriesByProductId(int productId)
        {
            return await GetAsync<List<ProductCategory>>($"api/product/{productId}/categories");
        }

        public async Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>($"api/product/category", productCategory);
        }

        public async Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/product/category", productCategory);
        }

        public async Task<ServiceResponse<bool>> DeleteProductCategoryAsync(int productCategoryId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/product/category/{productCategoryId}");
        }
    }
}
