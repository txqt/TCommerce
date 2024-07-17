using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Core.Interface
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoryAsync();
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<List<Category>?> GetCategoriesByIdsAsync(List<int> ids);
        Task<Category?> GetCategoryByNameAsync(string categoryName);
        Task<ServiceResponse<bool>> CreateCategoryAsync(Category category);
        Task<ServiceResponse<bool>> UpdateCategoryAsync(Category category);
        Task<ServiceResponse<bool>> DeleteCategoryByIdAsync(int id);
        Task<List<ProductCategory>> GetProductCategoriesByCategoryIdAsync(int categoryId);
        Task<List<ProductCategory>> GetProductCategoriesByProductIdAsync(int productId);
        Task<ProductCategory?> GetProductCategoryByIdAsync(int productCategoryId);
        Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> BulkCreateProductCategoriesAsync(List<ProductCategory> productCategories);
        Task<ServiceResponse<bool>> DeleteProductCategoryMappingById(int productCategoryId);
        Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> BulkUpdateProductCategoryAsync(List<ProductCategory> productCategories);

        #region Category discounts
        Task ClearDiscountCategoryMappingAsync(Discount discount);
        Task<List<DiscountCategoryMapping>> GetAllDiscountsAppliedToCategoryAsync(int categoryId);
        Task<List<Category>> GetCategoriesByAppliedDiscountAsync(int? discountId = null,
        bool showHidden = false);
        Task<DiscountCategoryMapping> GetDiscountAppliedToCategoryAsync(int categoryId, int discountId);
        Task CreateDiscountCategoryMappingAsync(DiscountCategoryMapping discountCategoryMapping);
        Task DeleteDiscountCategoryMappingAsync(int discountCategoryMappingId);

        #endregion
    }
}
