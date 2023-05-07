using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.WebApi.Database.ConfigurationDatabase;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeValueService
    {
        Task<List<ProductAttributeValue>> GetProductAttributeValuesAsync(int productAttributeMappingId);
    }
    public class ProductAttributeValueService : IProductAttributeValueService
    {
        private readonly DatabaseContext _context;
        public ProductAttributeValueService(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<List<ProductAttributeValue>> GetProductAttributeValuesAsync(int productAttributeMappingId)
        {
            using (_context)
            {
                var pavList = await _context.ProductAttributeValue.Where(x=>x.ProductAttributeMappingId == productAttributeMappingId).ToListAsync();
                return pavList;
            }
        }
    }
}
