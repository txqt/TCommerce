using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services
{
    public class ManufacturerServices : IManufacturerServicesCommon
    {
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;

        public ManufacturerServices(IRepository<Manufacturer> manufacturerRepository, IRepository<ProductManufacturer> productManufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
            _productManufacturerRepository = productManufacturerRepository;
        }

        public async Task<List<Manufacturer>> GetAllManufacturerAsync()
        {
            return (await _manufacturerRepository.GetAllAsync()).ToList();
        }

        public async Task<ServiceResponse<Manufacturer>> GetManufacturerByIdAsync(int manufacturerId)
        {
            return new ServiceResponse<Manufacturer>()
            {
                Data = await _manufacturerRepository.GetByIdAsync(manufacturerId),
            };
        }

        public async Task<ServiceResponse<Manufacturer>> GetManufacturerByNameAsync(string manufacturerName)
        {
            return new ServiceResponse<Manufacturer>()
            {
                Data = await _manufacturerRepository.Table.FirstOrDefaultAsync(x=>x.Name == manufacturerName),
            };
        }

        public async Task<ServiceResponse<bool>> CreateManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.CreateAsync(manufacturer);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> UpdateManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.UpdateAsync(manufacturer);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteManufacturerByIdAsync(int id)
        {
            await _manufacturerRepository.DeleteAsync(id);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<List<ProductManufacturer>> GetProductManufacturersByManufacturerIdAsync(int manufacturerId)
        {
            return await _productManufacturerRepository.Table.Where(x => x.ManufacturerId == manufacturerId).ToListAsync();
        }

        public async Task<ServiceResponse<ProductManufacturer>> GetProductManufacturerByIdAsync(int productManufacturerId)
        {
            return new ServiceResponse<ProductManufacturer>()
            {
                Data = await _productManufacturerRepository.Table.FirstOrDefaultAsync(x => x.Id == productManufacturerId),
            };
        }

        public async Task<ServiceResponse<bool>> CreateProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            try
            {
                await _productManufacturerRepository.CreateAsync(productManufacturer);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<bool>> BulkCreateProductManufacturersAsync(List<ProductManufacturer> productManufacturer)
        {
            await _productManufacturerRepository.BulkCreateAsync(productManufacturer);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteManufacturerMappingById(int productManufacturerId)
        {
            await _productManufacturerRepository.DeleteAsync(productManufacturerId);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> UpdateProductManufacturerAsync(ProductManufacturer productManufacturer)
        {
            await _productManufacturerRepository.UpdateAsync(productManufacturer);
            return new ServiceSuccessResponse<bool>();
        }
    }
}
