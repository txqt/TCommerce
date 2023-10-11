using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface IProductAttributeService
    {
        #region ProductAttribute
        Task<List<ProductAttribute>> GetAllProductAttributeAsync();
        Task<ServiceResponse<ProductAttribute>> GetProductAttributeByIdAsync(int id);
        Task<ServiceResponse<ProductAttribute>> GetProductAttributeByName(string name);
        Task<ServiceResponse<bool>> CreateProductAttributeAsync(ProductAttribute productAttribute);
        Task<ServiceResponse<bool>> UpdateProductAttributeAsync(ProductAttribute productAttribute);
        Task<ServiceResponse<bool>> DeleteProductAttributeByIdAsync(int id);
        #endregion

        #region ProductAttributeMapping
        Task<ServiceResponse<ProductAttributeMapping>> GetProductAttributeMappingByIdAsync(int productAttributeMappingId);
        Task<ServiceResponse<List<ProductAttributeMapping>>> GetProductAttributesMappingByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> CreateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping);
        Task<ServiceResponse<bool>> UpdateProductAttributeMappingAsync(ProductAttributeMapping productAttributeMapping);
        Task<ServiceResponse<bool>> DeleteProductAttributeMappingByIdAsync(int productAttributeId);
        Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValuesAsync(int productAttributeMappingId);
        #endregion

        #region ProductAttributeValue
        Task<ServiceResponse<ProductAttributeValue>> GetProductAttributeValuesByIdAsync(int productAttributeValueId);
        Task<ServiceResponse<bool>> CreateProductAttributeValueAsync(ProductAttributeValue productAttributeValue);
        Task<ServiceResponse<bool>> UpdateProductAttributeValueAsync(ProductAttributeValue productAttributeValue);
        Task<ServiceResponse<bool>> DeleteProductAttributeValueAsync(int productAttributeValueId);
        #endregion
    }
}
