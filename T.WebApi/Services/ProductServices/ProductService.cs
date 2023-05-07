using AutoMapper;
using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductService
    {
        Task<PagedList<Product>> GetAll(ProductParameters productParameters);
        Task<ServiceResponse<Product>> Get(int id);
        Task<ServiceResponse<bool>> CreateProduct(Product product);
        Task<ServiceResponse<bool>> EditProduct(ProductUpdateViewModel product);
        Task<ServiceResponse<bool>> DeleteProduct(int productId);
        Task<ServiceResponse<List<ProductAttribute>>> GetAllProductAttribute(int id);
    }
    public class ProductService : IProductService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        public ProductService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedList<Product>> GetAll(ProductParameters productParameters)
        {
            using (_context)
            {
                var list_product = new List<Product>();

                list_product = await _context.Product
                    .Search(productParameters.searchText)
                    .Sort(productParameters.OrderBy)//sort by product coloumn 
                    .Include(x => x.ProductPictures)
                    .Where(x => x.Deleted == false)
                    .ToListAsync();



                list_product = list_product.DistinctBy(x => x.Id).ToList();
                //list_product.Shuffle();
                return PagedList<Product>
                            .ToPagedList(list_product, productParameters.PageNumber, productParameters.PageSize);
            }
        }

        public async Task<ServiceResponse<Product>> Get(int id)
        {
            using (_context)
            {
                var product = await _context.Product.Where(x => x.Deleted == false)
                    .Include(x => x.AttributeMappings)
                    .ThenInclude(x => x.ProductAttribute)
                    .FirstOrDefaultAsync(x => x.Id == id);

                var response = new ServiceResponse<Product>
                {
                    Data = product,
                    Success = true
                };
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> CreateProduct(Product product)
        {
            using (_context)
            {
                product.CreatedOnUtc = DateTime.Now;
                _context.Product.Add(product);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ServiceErrorResponse<bool>("Create product failed");
                }
                return new ServiceSuccessResponse<bool>();
            }
        }

        public async Task<ServiceResponse<bool>> EditProduct(ProductUpdateViewModel product)
        {
            var productTable = await _context.Product.FirstOrDefaultAsync(p => p.Id == product.Id);
            if (productTable == null)
            {
                return new ServiceErrorResponse<bool>("Product not found");
            }



            try
            {
                var productMapped = _mapper.Map(product, productTable);
                if (_context.IsRecordUnchanged(productTable, productMapped))
                {
                    return new ServiceErrorResponse<bool>("Data is unchanged");
                }
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ServiceErrorResponse<bool>("Edit product failed");
                }
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }

            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteProduct(int productId)
        {
            var product = await _context.Product.FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null || product.Deleted == true) { throw new Exception($"Cannot find product: {productId}"); }
            product.Deleted = true;

            _context.Product.Update(product);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ServiceErrorResponse<bool>("Delete product failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<List<ProductAttribute>>> GetAllProductAttribute(int id)
        {
            var result = await _context.Product_ProductAttribute_Mapping
                    .Where(pam => pam.ProductId == id)
                    .Select(pam => pam.ProductAttribute)
                    .ToListAsync();

            if (result.Count == 0 || result is null)
                return new ServiceErrorResponse<List<ProductAttribute>>();

            return new ServiceSuccessResponse<List<ProductAttribute>>(result);
        }




    }
}
