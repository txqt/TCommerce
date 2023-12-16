using T.Library.Model.Response;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using T.Library.Model.Interface;
using T.Library.Model;
using T.Web.Helpers;
using T.Library.Model.Catalogs;

namespace T.Web.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClientHelper _httpClientHelper;
        private readonly string DEFAULT_ROUTE = "api/categories/";
        public CategoryService(HttpClientHelper httpClientHelper)
        {
            _httpClientHelper = httpClientHelper;
        }

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await _httpClientHelper.GetAsync<List<Category>>(DEFAULT_ROUTE);
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _httpClientHelper.GetAsync<Category>(DEFAULT_ROUTE + $"{categoryId}");
        }

        [Obsolete("This method is not needed in this project.")]
        public Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> CreateCategoryAsync(Category category)
        {
            return await _httpClientHelper.PostAsJsonAsync<ServiceResponse<bool>>(DEFAULT_ROUTE, category);
        }

        public async Task<ServiceResponse<bool>> UpdateCategoryAsync(Category category)
        {
            return await _httpClientHelper.PutAsJsonAsync<ServiceResponse<bool>>(DEFAULT_ROUTE, category);
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id)
        {
            return await _httpClientHelper.DeleteAsync<ServiceResponse<bool>>(DEFAULT_ROUTE + id);
        }

        public async Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId)
        {
            return await _httpClientHelper.GetAsync<List<ProductCategory>>($"api/product-categories/by-category-id/{categoryId}");
        }

        public async Task<ServiceResponse<bool>> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories)
        {
            return await _httpClientHelper.PostAsJsonAsync<ServiceResponse<bool>>("api/product-categories/bulk", productCategories);
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryMappingById(int productCategoryId)
        {
            return await _httpClientHelper.DeleteAsync<ServiceResponse<bool>>($"api/product-categories/{productCategoryId}");
        }

        public async Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory)
        {
            return await _httpClientHelper.PutAsJsonAsync<ServiceResponse<bool>>($"api/product-categories", productCategory);
        }

        public async Task<ProductCategory> GetProductCategoryByIdAsync(int productCategoryId)
        {
            return await _httpClientHelper.GetAsync<ProductCategory>($"api/product-categories/{productCategoryId}");
        }

        public Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory)
        {
            throw new NotImplementedException();
        }
    }
}
