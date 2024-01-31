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
    public class ManufacturerService : HttpClientHelper, IManufacturerService
    {
        private const string DEFAULT_URL = "api/manufacturers";

        public ManufacturerService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<Manufacturer>> GetAllManufacturerAsync()
        {
            return await GetAsync<List<Manufacturer>>(DEFAULT_URL);
        }

        public async Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId)
        {
            return await GetAsync<Manufacturer>(DEFAULT_URL + $"/{manufacturerId}");
        }

        public async Task<Manufacturer> GetManufacturerByNameAsync(string manufacturerName)
        {
            return await GetAsync<Manufacturer>(DEFAULT_URL + $"/{manufacturerName}");
        }

        public async Task<ServiceResponse<bool>> CreateManufacturerAsync(Manufacturer manufacturer)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>(DEFAULT_URL, manufacturer);
        }

        public async Task<ServiceResponse<bool>> UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>(DEFAULT_URL, manufacturer);
        }

        public async Task<ServiceResponse<bool>> DeleteManufacturerByIdAsync(int id)
        {
            return await DeleteAsync<ServiceResponse<bool>>(DEFAULT_URL + $"/{id}");
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
