using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Response;

namespace TCommerce.Core.Interface
{
    public interface IManufacturerService
    {
        Task<List<Manufacturer>> GetAllManufacturerAsync();
        Task<Manufacturer?> GetManufacturerByIdAsync(int manufacturerId);
        Task<List<Manufacturer>?> GetManufacturersByIdsAsync(List<int> ids);
        Task<Manufacturer?> GetManufacturerByNameAsync(string manufacturerName);
        Task<ServiceResponse<bool>> CreateManufacturerAsync(Manufacturer manufacturer);
        Task<ServiceResponse<bool>> UpdateManufacturerAsync(Manufacturer manufacturer);
        Task<ServiceResponse<bool>> DeleteManufacturerByIdAsync(int id);
        Task<List<ProductManufacturer>> GetProductManufacturersByManufacturerIdAsync(int manufacturerId);
        Task<ProductManufacturer?> GetProductManufacturerByIdAsync(int productManufacturerId);
        Task<List<ProductManufacturer>?> GetProductManufacturerByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> CreateProductManufacturerAsync(ProductManufacturer productManufacturer);
        Task<ServiceResponse<bool>> BulkCreateProductManufacturersAsync(List<ProductManufacturer> productManufacturer);
        Task<ServiceResponse<bool>> DeleteManufacturerMappingById(int productManufacturerId);
        Task<ServiceResponse<bool>> UpdateProductManufacturerAsync(ProductManufacturer productManufacturer);

        #region Manufacturer discounts
        Task ClearDiscountManufacturerMappingAsync(Discount discount);
        Task<List<DiscountManufacturerMapping>> GetAllDiscountsAppliedToManufacturerAsync(int manufacturerId);
        Task<List<Manufacturer>> GetManufacturersByAppliedDiscountAsync(int? discountId = null,
        bool showHidden = false);
        Task<DiscountManufacturerMapping> GetDiscountAppliedToManufacturerAsync(int manufacturerId, int discountId);
        Task CreateDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping);
        Task DeleteDiscountManufacturerMappingAsync(int discountManufacturerMappingId);
        #endregion
    }
}
