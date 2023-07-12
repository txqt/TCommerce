using AutoMapper;
using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Response;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategory>> GetAllAsync();
        Task<ServiceResponse<ProductCategory>> Get(int id);
        Task<ServiceResponse<List<ProductCategory>>> GetbyProductId(int id);
        Task<ServiceResponse<List<ProductCategory>>> GetbyCategoryId(int id);
        Task<ServiceResponse<bool>> CreateOrEditAsync(ProductCategory productCategory);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly DatabaseContext _context;
        private readonly Mapper _mapper;

        public ProductCategoryService(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<bool>> CreateOrEditAsync(ProductCategory productCategory)
        {
            
            {
                var productCategoryTable = await _context.Product_ProductCategory_Mapping.FirstOrDefaultAsync(x => x.Id == productCategory.Id);

                if (productCategoryTable == null)
                {
                    _context.Product_ProductCategory_Mapping.Add(productCategory);
                }
                else
                {
                    if (_context.IsRecordUnchanged(productCategoryTable, productCategory))
                    {
                        return new ServiceErrorResponse<bool>("Data is unchanged");
                    }
                    productCategoryTable.ProductId = productCategory.ProductId;
                    productCategoryTable.CategoryId = productCategory.CategoryId;
                    productCategoryTable.IsFeaturedProduct = productCategory.IsFeaturedProduct;
                    productCategoryTable.DisplayOrder = productCategory.DisplayOrder;
                }
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ServiceErrorResponse<bool>("Create product category mapping failed");
                }
                return new ServiceSuccessResponse<bool>();
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var category = await _context.Product_ProductCategory_Mapping.FirstOrDefaultAsync(x => x.Id == id);
                _context.Product_ProductCategory_Mapping.Remove(category);
                await _context.SaveChangesAsync();
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(message: ex.Message);
            }
        }

        public async Task<ServiceResponse<ProductCategory>> Get(int id)
        {
            
            {
                var category = await _context.Product_ProductCategory_Mapping.FirstOrDefaultAsync(x => x.Id == id);

                var response = new ServiceResponse<ProductCategory>
                {
                    Data = category,
                    Success = true
                };
                return response;
            }
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            
            {
                return await _context.Product_ProductCategory_Mapping.ToListAsync();
            }
        }

        public async Task<ServiceResponse<List<ProductCategory>>> GetbyCategoryId(int id)
        {
            
            {
                var category = await _context.Product_ProductCategory_Mapping.Where(x => x.CategoryId == id).ToListAsync();

                var response = new ServiceResponse<List<ProductCategory>>
                {
                    Data = category,
                    Success = true
                };
                return response;
            }
        }

        public async Task<ServiceResponse<List<ProductCategory>>> GetbyProductId(int id)
        {
            
            {
                var category = await _context.Product_ProductCategory_Mapping.Where(x => x.ProductId == id).ToListAsync();

                var response = new ServiceResponse<List<ProductCategory>>
                {
                    Data = category,
                    Success = true
                };
                return response;
            }
        }
    }
}
