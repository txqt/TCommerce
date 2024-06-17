using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Catalogs;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface IManufacturerServicesCommon
    {
        Task<List<Manufacturer>> GetAllManufacturerAsync();
        Task<Manufacturer?> GetManufacturerByIdAsync(int manufacturerId);
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
    }
}
