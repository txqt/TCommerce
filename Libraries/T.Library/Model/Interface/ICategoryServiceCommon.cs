using T.Library.Model.Catalogs;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;

namespace T.Library.Model.Interface
{
    public interface ICategoryServiceCommon
    {
        Task<List<Category>> GetAllCategoryAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task<Category> GetCategoryByNameAsync(string categoryName);
        Task<ServiceResponse<bool>> CreateCategoryAsync(CategoryModel category);
        Task<ServiceResponse<bool>> UpdateCategoryAsync(CategoryModel category);
        Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id);
        Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId);
        Task<List<ProductCategory>> GetProductCategoriesByProductIdAsync(int productId);
        Task<ProductCategory> GetProductCategoryByIdAsync(int productCategoryId);
        Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories);
        Task<ServiceResponse<bool>> DeleteProductCategoryMappingById(int productCategoryId);
        Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory);
    }
}
