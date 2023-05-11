using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Response;
using T.WebApi.Database.ConfigurationDatabase;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeValueService
    {
        Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValuesAsync(int productAttributeMappingId);
    }
    public class ProductAttributeValueService : IProductAttributeValueService
    {
        private readonly DatabaseContext _context;
        public ProductAttributeValueService(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<List<ProductAttributeValue>>> GetProductAttributeValuesAsync(int productAttributeMappingId)
        {
            using (_context)
            {
                var pavList = await _context.ProductAttributeValue.Where(x=>x.ProductAttributeMappingId == productAttributeMappingId).ToListAsync();
                return new ServiceSuccessResponse<List<ProductAttributeValue>>(pavList);
            }
        }
    }
}
