using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoryAsync();
        Task<ServiceResponse<Category>> GetCategoryByIdAsync(int categoryId);
        Task<ServiceResponse<Category>> GetCategoryByNameAsync(string categoryName);
        Task<ServiceResponse<bool>> CreateCategoryAsync(Category category);
        Task<ServiceResponse<bool>> UpdateCategoryAsync(Category category);
        Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id);
        Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId);
        Task<ServiceResponse<ProductCategory>> GetProductCategoryByIdAsync(int productCategoryId);
        Task<ServiceResponse<bool>> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories);
        Task<ServiceResponse<bool>> DeleteCategoryMappingById(int productCategoryId);
        Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory);
    }
}
