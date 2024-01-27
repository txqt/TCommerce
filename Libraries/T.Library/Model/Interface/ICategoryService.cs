using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Catalogs;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoryAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task<Category> GetCategoryByNameAsync(string categoryName);
        Task<ServiceResponse<bool>> CreateCategoryAsync(Category category);
        Task<ServiceResponse<bool>> UpdateCategoryAsync(Category category);
        Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id);
        Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId);
        Task<List<ProductCategory>> GetProductCategoriesByProductIdAsync(int productId);
        Task<ProductCategory> GetProductCategoryByIdAsync(int productCategoryId);
        Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories);
        Task<ServiceResponse<bool>> DeleteCategoryMappingById(int productCategoryId);
        Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory);
    }
}
