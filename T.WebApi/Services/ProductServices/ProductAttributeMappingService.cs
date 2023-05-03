using T.Library.Model.Response;
using T.Library.Model;
using Microsoft.EntityFrameworkCore;
using T.WebApi.Database.ConfigurationDatabase;
using AutoMapper;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeMappingService
    {
        Task<ServiceResponse<ProductAttributeMapping>> GetProductAttributeMappingByIdAsync(int id);
        Task<ServiceResponse<List<ProductAttributeMapping>>> GetProductAttributeMappingByProductIdAsync(int id);
        Task<ServiceResponse<bool>> AddOrUpdateProductAttributeMapping(ProductAttributeMapping productAttributeMapping);
        Task<ServiceResponse<List<ProductAttributeValue>>> GetAllValueProductAttribute(int productAttributeMappingId);
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
            var pamList = await _context.Product_ProductAttribute_Mapping
                    .Where(pam => pam.ProductId == productAttributeMapping.ProductId && pam.ProductAttributeId == productAttributeMapping.ProductAttributeId)
                    .FirstOrDefaultAsync();
            
            if (pamList != null)
            {
                productAttributeMapping.Id = 0;
                _mapper.Map(productAttributeMapping, pamList);
            }
            else
            {
                _context.Product_ProductAttribute_Mapping.Add(productAttributeMapping);
            }

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ServiceErrorResponse<bool>("Add or edit product mapping failed");
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
                    .ToListAsync();

                var response = new ServiceResponse<List<ProductAttributeMapping>>
                {
                    Data = productAttributeMapping,
                    Success = true
                };
                return response;
            }
        }
    }
}
