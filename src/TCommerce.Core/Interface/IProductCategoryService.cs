using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Response;

namespace TCommerce.Core.Interface
{
    public interface IProductCategoryService
    {
        //Task<List<ProductCategory>> GetAllProductCategoryAsync();
        Task<ProductCategory?> GetProductCategoryById(int productCategoryId);
        Task<List<ProductCategory>> GetProductCategoriesByProductId(int productId);
        Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> DeleteProductCategoryAsync(int productCategoryId);
    }
}
