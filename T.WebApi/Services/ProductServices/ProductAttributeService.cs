using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Response;
using T.WebApi.Database.ConfigurationDatabase;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeService
    {
        Task<List<ProductAttribute>> GetAllProductAttributeAsync();
        Task<ServiceResponse<ProductAttribute>> GetProductAttributeById(int id);
        Task<ServiceResponse<bool>> CreateProductAttributeAsync(ProductAttribute productAttribute);
        Task<ServiceResponse<bool>> EditProductAttributeAsync(ProductAttribute productAttribute);
        Task<ServiceResponse<bool>> DeleteProductAttributeByIdAsync(int id);
    }
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly DatabaseContext _context;
        public ProductAttributeService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<bool>> CreateProductAttributeAsync(ProductAttribute productAttribute)
        {
            using (_context)
            {
                _context.ProductAttribute.Add(productAttribute);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ServiceErrorResponse<bool>("Create product failed");
                }
                return new ServiceSuccessResponse<bool>();
            }
        }

        public async Task<ServiceResponse<bool>> EditProductAttributeAsync(ProductAttribute productAttribute)
        {
            var productTable = await _context.ProductAttribute.FirstOrDefaultAsync(p => p.Id == productAttribute.Id);
            if (productTable == null)
            {
                return new ServiceErrorResponse<bool>("Product not found");
            }



            try
            {
                productTable.Name = productAttribute.Name;
                productTable.Description = productAttribute.Description;

                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ServiceErrorResponse<bool>("Edit product attribute failed");
                }
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }

            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<ProductAttribute>> GetProductAttributeById(int id)
        {
            using (_context)
            {
                var product = await _context.ProductAttribute.Where(x => x.Deleted == false)
                    .FirstOrDefaultAsync(x => x.Id == id);

                var response = new ServiceResponse<ProductAttribute>
                {
                    Data = product,
                    Success = true
                };
                return response;
            }
        }

        public async Task<List<ProductAttribute>> GetAllProductAttributeAsync()
        {
            using(_context)
            {
                var list = await _context.ProductAttribute.ToListAsync();
                return list;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProductAttributeByIdAsync(int id)
        {
            var product = await _context.ProductAttribute.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null || product.Deleted == true) { throw new Exception($"Cannot find product: {id}"); }
            product.Deleted = true;

            _context.ProductAttribute.Remove(product);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ServiceErrorResponse<bool>("Delete product attribute failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        
    }
}
