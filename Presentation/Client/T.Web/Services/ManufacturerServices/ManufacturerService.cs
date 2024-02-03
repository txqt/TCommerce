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
        private const string DEFAULT_URL_MANUFACTURER = "api/manufacturers";
        private const string DEFAULT_URL_PRODUCTMANUFACTURER = "api/product-manufacturers";

        public ManufacturerService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<Manufacturer>> GetAllManufacturerAsync()
        {
            return await GetAsync<List<Manufacturer>>(DEFAULT_URL_MANUFACTURER);
        }

        public async Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId)
        {
            return await GetAsync<Manufacturer>(DEFAULT_URL_MANUFACTURER + $"/{manufacturerId}");
        }

        public async Task<Manufacturer> GetManufacturerByNameAsync(string manufacturerName)
        {
            return await GetAsync<Manufacturer>(DEFAULT_URL_MANUFACTURER + $"manufacturerName/{manufacturerName}");
        }

        public async Task<ServiceResponse<bool>> CreateManufacturerAsync(Manufacturer manufacturer)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>(DEFAULT_URL_MANUFACTURER, manufacturer);
        }

        public async Task<ServiceResponse<bool>> UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>(DEFAULT_URL_MANUFACTURER, manufacturer);
        }

        public async Task<ServiceResponse<bool>> DeleteManufacturerByIdAsync(int id)
        {
            return await DeleteAsync<ServiceResponse<bool>>(DEFAULT_URL_MANUFACTURER + $"/{id}");
        }

        public async Task<List<ProductManufacturer>> GetProductManufacturersByManufacturerIdAsync(int manufacturerId)
        {
            return await GetAsync<List<ProductManufacturer>>(DEFAULT_URL_MANUFACTURER + $"/{manufacturerId}/product-manufacturer");
        }

        public async Task<ProductManufacturer> GetProductManufacturerByIdAsync(int productManufacturerId)
        {
            return await GetAsync<ProductManufacturer>(DEFAULT_URL_PRODUCTMANUFACTURER + $"/{productManufacturerId}");
        }

        public async Task<ServiceResponse<bool>> CreateProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>(DEFAULT_URL_PRODUCTMANUFACTURER, productManufacturer);
        }

        public async Task<ServiceResponse<bool>> BulkCreateProductManufacturersAsync(List<ProductManufacturer> productManufacturers)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>(DEFAULT_URL_PRODUCTMANUFACTURER + "/bulk", productManufacturers);
        }

        public async Task<ServiceResponse<bool>> DeleteManufacturerMappingById(int productManufacturerId)
        {
            return await DeleteAsync<ServiceResponse<bool>>(DEFAULT_URL_PRODUCTMANUFACTURER + $"/{productManufacturerId}");
        }

        public async Task<ServiceResponse<bool>> UpdateProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            return await PutAsJsonAsync<ServiceResponse<bool>>(DEFAULT_URL_PRODUCTMANUFACTURER, productManufacturer);
        }
    }
}
