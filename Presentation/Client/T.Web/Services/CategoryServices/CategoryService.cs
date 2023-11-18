using T.Library.Model.Response;
using System.Net.Http.Json;
using T.Library.Model.Common;
using System.Net.Http.Headers;
using T.Library.Model.Interface;
using T.Library.Model;
using T.Web.Helpers;

namespace T.Web.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClientHelper _httpClientHelper;
        private readonly string DEFAULT_ROUTE = APIRoutes.CATEGORY + "/";
        public CategoryService(HttpClientHelper httpClientHelper)
        {
            _httpClientHelper = httpClientHelper;
        }

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await _httpClientHelper.GetAsync<List<Category>>(DEFAULT_ROUTE + APIRoutes.GETALL);
        }

        public async Task<ServiceResponse<Category>> GetCategoryByIdAsync(int categoryId)
        {
            return await _httpClientHelper.GetAsync<ServiceResponse<Category>>(DEFAULT_ROUTE + $"{categoryId}");
        }

        [Obsolete("This method is not needed in this project.")]
        public Task<ServiceResponse<Category>> GetCategoryByNameAsync(string categoryName)
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
            return await _httpClientHelper.GetAsync<List<ProductCategory>>(DEFAULT_ROUTE + $"{categoryId}/product-categories");
        }

        public async Task<ServiceResponse<bool>> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories)
        {
            return await _httpClientHelper.PostAsJsonAsync<ServiceResponse<bool>>(DEFAULT_ROUTE + "bulk-product-categories", productCategories);
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryMappingById(int productCategoryId)
        {
            return await _httpClientHelper.DeleteAsync<ServiceResponse<bool>>(DEFAULT_ROUTE + $"product-category/{productCategoryId}");
        }
    }
}
