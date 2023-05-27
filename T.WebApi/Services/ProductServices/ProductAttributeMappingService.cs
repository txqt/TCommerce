using T.Library.Model.Response;
using T.Library.Model;
using Microsoft.EntityFrameworkCore;
using T.WebApi.Database.ConfigurationDatabase;
using AutoMapper;
using StackExchange.Redis;
using T.WebApi.Extensions;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeMappingService
    {
        Task<ServiceResponse<ProductAttributeMapping>> GetProductAttributeMappingByIdAsync(int id);
        Task<ServiceResponse<List<ProductAttributeMapping>>> GetProductAttributeMappingByProductIdAsync(int id);
        Task<ServiceResponse<bool>> AddOrUpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping);
        Task<ServiceResponse<List<ProductAttributeValue>>> GetAllValueProductAttribute(int productAttributeMappingId);
        Task<ServiceResponse<bool>> DeleteProductAttributeMapping(int id);
    }
    public class ProductAttributeMappingService : IProductAttributeMappingService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        public ProductAttributeMappingService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<ProductAttributeMapping>> GetProductAttributeMappingByIdAsync(int id)
        {
            using (_context)
            {
                var productAttributeMapping = await _context.Product_ProductAttribute_Mapping.Where(x => x.Deleted == false)
                    .FirstOrDefaultAsync(x => x.Id == id);

                var response = new ServiceResponse<ProductAttributeMapping>
                {
                    Data = productAttributeMapping,
                    Success = true
                };
                return response;
            }
        }
        public async Task<ServiceResponse<bool>> AddOrUpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            var pamInTable = await _context.Product_ProductAttribute_Mapping
                    .Where(pam => pam.ProductId == productAttributeMapping.ProductId && pam.Id == productAttributeMapping.Id)
                    .FirstOrDefaultAsync();

            if (pamInTable != null)
            {
                if (_context.IsRecordUnchanged(pamInTable, productAttributeMapping))
                {
                    return new ServiceErrorResponse<bool>("Data is unchanged");
                }

                pamInTable.ProductAttributeId = productAttributeMapping.ProductAttributeId;
                pamInTable.TextPrompt = productAttributeMapping.TextPrompt;
                pamInTable.IsRequired = productAttributeMapping.IsRequired;
                pamInTable.DisplayOrder = productAttributeMapping.DisplayOrder;

            }
            else
            {
                var _productAttributeMapping = new ProductAttributeMapping()
                {
                    ProductAttributeId = productAttributeMapping.ProductAttributeId,
                    IsRequired = productAttributeMapping.IsRequired,
                    ProductId = productAttributeMapping.ProductId,
                    DisplayOrder = productAttributeMapping.DisplayOrder,
                };

                _context.Product_ProductAttribute_Mapping.Add(_productAttributeMapping);
            }
            
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ServiceErrorResponse<bool>("Add or edit product value failed");
            }
            return new ServiceSuccessResponse<bool>();
        }
        public async Task<ServiceResponse<List<ProductAttributeValue>>> GetAllValueProductAttribute(int productAttributeMappingId)
        {
            var productAttributeValue = await _context.ProductAttributeValue
                                        .Where(pav => pav.ProductAttributeMappingId == productAttributeMappingId)
                                        .Include(x => x.ProductAttributeMappings)
                                        .ToListAsync();

            if (productAttributeValue != null)
            {
                foreach (var item in productAttributeValue.Select(x => x.ProductAttributeMappings))
                {
                    item.ProductAttributeValue = null;
                }
            }

            if (productAttributeValue.Count == 0 || productAttributeValue is null)
                return new ServiceErrorResponse<List<ProductAttributeValue>>("Product Attribute Value Not Found");

            return new ServiceSuccessResponse<List<ProductAttributeValue>>(productAttributeValue);
        }

        public async Task<ServiceResponse<List<ProductAttributeMapping>>> GetProductAttributeMappingByProductIdAsync(int id)
        {
            using (_context)
            {
                var productAttributeMapping = await _context.Product_ProductAttribute_Mapping.Where(x => x.Deleted == false && x.ProductId == id)
                    .Include(x => x.ProductAttribute)
                    .ToListAsync();

                var response = new ServiceResponse<List<ProductAttributeMapping>>
                {
                    Data = productAttributeMapping,
                    Success = true
                };
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeMapping(int id)
        {
            try
            {
                var pam = await _context.Product_ProductAttribute_Mapping.Where(x => x.Id == id).FirstOrDefaultAsync();
                var result = _context.Product_ProductAttribute_Mapping.Remove(pam);
                await _context.SaveChangesAsync();
                return new ServiceSuccessResponse<bool>();
            }
            catch(Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }

        }
    }
}
