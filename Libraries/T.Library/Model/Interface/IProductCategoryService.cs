using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface IProductCategoryService
    {
        //Task<List<ProductCategory>> GetAllProductCategoryAsync();
        Task<ProductCategory> GetProductCategoryById(int productCategoryId);
        Task<List<ProductCategory>> GetProductCategoriesByProductId(int productId);
        Task<ServiceResponse<bool>> CreateProductCategoryAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> UpdateProductCategoryAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> DeleteProductCategoryAsync(int productCategoryId);
    }
}
