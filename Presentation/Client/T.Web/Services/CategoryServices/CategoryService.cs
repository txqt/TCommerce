using T.Library.Model.Response;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using T.Library.Model.Interface;
using T.Library.Model;
using T.Web.Helpers;
using T.Library.Model.Catalogs;
using T.Web.Areas.Admin.Models;
using T.Library.Model.ViewsModel;

namespace T.Web.Services.CategoryService
{
    public class CategoryService : HttpClientHelper, ICategoryServiceCommon
    {
        private readonly string DEFAULT_ROUTE = "api/categories/";
        public CategoryService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await GetAsync<List<Category>>(DEFAULT_ROUTE);
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await GetAsync<Category>(DEFAULT_ROUTE + $"{categoryId}");
        }

        [Obsolete("This method is not needed in this project.")]
        public Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> CreateCategoryAsync(CategoryModel category)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>(DEFAULT_ROUTE, category);
        }

        public async Task<ServiceResponse<bool>> UpdateCategoryAsync(CategoryModel category)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>(DEFAULT_ROUTE, category);
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id)
        {
            return await DeleteAsync<ServiceResponse<bool>>(DEFAULT_ROUTE + id);
        }

        public async Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId)
        {
            return await GetAsync<List<ProductCategory>>($"api/product-categories/by-category-id/{categoryId}");
        }

        public async Task<ServiceResponse<bool>> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>("api/product-categories/bulk", productCategories);
        }

        public async Task<ServiceResponse<bool>> DeleteProductCategoryMappingById(int productCategoryId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/product-categories/{productCategoryId}");
        }

        public async Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>($"api/product-categories", productCategory);
        }

        public async Task<ProductCategory> GetProductCategoryByIdAsync(int productCategoryId)
        {
            return await GetAsync<ProductCategory>($"api/product-categories/{productCategoryId}");
        }

        public Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductCategory>> GetProductCategoriesByProductIdAsync(int productId)
        {
            return await GetAsync<List<ProductCategory>>($"api/product-categories/by-product-id/{productId}");
        }
    }
}
