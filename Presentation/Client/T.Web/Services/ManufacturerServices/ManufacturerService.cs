using System.Collections.Generic;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Web.Helpers;

namespace T.Web.Services.ManufacturerServices
{
    public interface IManufacturerService : IManufacturerServicesCommon
    {

    }
    public class ManufacturerService : IManufacturerService
    {
        private readonly HttpClientHelper _httpClientHelper;
        private const string DEFAULT_URL = "api/manufacturer";

        public ManufacturerService(HttpClientHelper httpClientHelper)
        {
            this._httpClientHelper = httpClientHelper;
        }

        public async Task<List<Manufacturer>> GetAllManufacturerAsync()
        {
            return await _httpClientHelper.GetAsync<List<Manufacturer>>(DEFAULT_URL);
        }

        public Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId)
        {
            throw new NotImplementedException();
        }

        public Task<Manufacturer> GetManufacturerByNameAsync(string manufacturerName)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> CreateManufacturerAsync(Manufacturer manufacturer)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> DeleteManufacturerByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProductManufacturer>> GetProductManufacturersByManufacturerIdAsync(int manufacturerId)
        {
            throw new NotImplementedException();
        }

        public Task<ProductManufacturer> GetProductManufacturerByIdAsync(int productManufacturerId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> CreateProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> BulkCreateProductManufacturersAsync(List<ProductManufacturer> productManufacturer)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> DeleteManufacturerMappingById(int productManufacturerId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> UpdateProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            throw new NotImplementedException();
        }
    }
}
