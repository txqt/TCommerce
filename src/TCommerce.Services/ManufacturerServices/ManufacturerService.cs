using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Response;
using TCommerce.Services.IRepositoryServices;

namespace TCommerce.Services.ManufacturerServices
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;
        private readonly IRepository<DiscountManufacturerMapping> _discountManufacturerMappingRepository;

        public ManufacturerService(IRepository<Manufacturer> manufacturerRepository, IRepository<ProductManufacturer> productManufacturerRepository, IRepository<DiscountManufacturerMapping> discountManufacturerMappingRepository)
        {
            _manufacturerRepository = manufacturerRepository;
            _productManufacturerRepository = productManufacturerRepository;
            _discountManufacturerMappingRepository = discountManufacturerMappingRepository;
        }

        public async Task<List<Manufacturer>> GetAllManufacturerAsync()
        {
            return (await _manufacturerRepository.GetAllAsync()).ToList();
        }

        public async Task<Manufacturer?> GetManufacturerByIdAsync(int manufacturerId)
        {
            return await _manufacturerRepository.GetByIdAsync(manufacturerId);
        }

        public async Task<Manufacturer?> GetManufacturerByNameAsync(string manufacturerName)
        {
            return await _manufacturerRepository.Table.FirstOrDefaultAsync(x => x.Name == manufacturerName);
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

        public async Task<ProductManufacturer?> GetProductManufacturerByIdAsync(int productManufacturerId)
        {
            return await _productManufacturerRepository.Table.FirstOrDefaultAsync(x => x.Id == productManufacturerId);
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

        public async Task<List<ProductManufacturer>?> GetProductManufacturerByProductIdAsync(int productId)
        {
            return await _productManufacturerRepository.Table.Where(x => x.ProductId == productId).ToListAsync();
        }

        public async Task ClearDiscountManufacturerMappingAsync(Discount discount)
        {
            ArgumentNullException.ThrowIfNull(discount);

            var mappingsWithProducts =
                from dcm in _discountManufacturerMappingRepository.Table
                join p in _manufacturerRepository.Table on dcm.EntityId equals p.Id
                where dcm.DiscountId == discount.Id
                select dcm;

            foreach (var pdm in await mappingsWithProducts.ToListAsync())
            {
                await _discountManufacturerMappingRepository.DeleteAsync(pdm.Id);
            }
        }

        public async Task<List<DiscountManufacturerMapping>> GetAllDiscountsAppliedToManufacturerAsync(int manufacturerId)
        {
            return (await _discountManufacturerMappingRepository.GetAllAsync(query => query.Where(dcm => dcm.EntityId == manufacturerId))).ToList();
        }

        public async Task<DiscountManufacturerMapping> GetDiscountAppliedToManufacturerAsync(int manufacturerId, int discountId)
        {
            return await _discountManufacturerMappingRepository.Table
                .FirstOrDefaultAsync(dcm => dcm.EntityId == manufacturerId && dcm.DiscountId == discountId);
        }

        public async Task CreateDiscountManufacturerMappingAsync(DiscountManufacturerMapping discountManufacturerMapping)
        {
            await _discountManufacturerMappingRepository.CreateAsync(discountManufacturerMapping);
        }

        public async Task DeleteDiscountManufacturerMappingAsync(int discountManufacturerMappingId)
        {
            await _discountManufacturerMappingRepository.DeleteAsync(discountManufacturerMappingId);
        }

        public async Task<List<Manufacturer>?> GetManufacturersByIdsAsync(List<int> ids)
        {
            return (await _manufacturerRepository.GetByIdsAsync(ids)).ToList();
        }

        public async Task<List<Manufacturer>> GetManufacturersByAppliedDiscountAsync(int? discountId = null, bool showHidden = false)
        {
            var manufacturers = _manufacturerRepository.Query;

            if (discountId.HasValue)
                manufacturers = from manufacturer in manufacturers
                             join dcm in _discountManufacturerMappingRepository.Table on manufacturer.Id equals dcm.EntityId
                             where dcm.DiscountId == discountId.Value
                             select manufacturer;

            if (!showHidden)
                manufacturers = manufacturers.Where(category => !category.Deleted);

            manufacturers = manufacturers.OrderBy(category => category.DisplayOrder).ThenBy(category => category.Id);

            return await manufacturers.ToListAsync();
        }
    }
}
