using AutoMapper;
using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services.ProductService;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductService
    {
        Task<PagedList<Product>> GetAll(ProductParameters productParameters);
        Task<ServiceResponse<Product>> Get(int id);
        Task<ServiceResponse<List<ProductPicture>>> GetProductPicturesByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> CreateProduct(Product product);
        Task<ServiceResponse<bool>> EditProduct(ProductUpdateViewModel product);
        Task<ServiceResponse<bool>> DeleteProduct(int productId);
        Task<ServiceResponse<List<ProductAttribute>>> GetAllProductAttribute(int id);
        Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId);
        Task<ServiceResponse<bool>> DeleteProductImage(int productId, int pictureId);
        Task<ServiceResponse<bool>> DeleteAllProductImage(int productId);
    }
    public class ProductService : IProductService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IPictureService _pictureService;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        public ProductService(DatabaseContext context, IMapper mapper, IPictureService pictureService, IConfiguration configuration, IHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _pictureService = pictureService;
            _configuration = configuration;
            _environment = environment;
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
                var product = await FindProductByIdAsync(id);

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

        public async Task<ServiceResponse<List<ProductPicture>>> GetProductPicturesByProductIdAsync(int productId)
        {
            using (_context)
            {
                var productPicture = await _context.Product_ProductPicture_Mapping.Where(x => x.Deleted == false && x.ProductId == productId)
                    .Include(x => x.Picture)
                    .ToListAsync();

                var response = new ServiceResponse<List<ProductPicture>>
                {
                    Data = productPicture,
                    Success = true
                };
                return response;
            }
        }

        public async Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId)
        {
            var product = await FindProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            using (_context)
            {
                try
                {
                    var apiUrl = _configuration.GetSection("Url:ApiUrl").Value;

                    string path = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    foreach (var imageFile in ListImages)
                    {
                        if (imageFile.Length > 0)
                        {
                            var uniqueFileName = Path.GetRandomFileName();
                            var fileExtension = Path.GetExtension(imageFile.FileName);
                            var newFileName = uniqueFileName + fileExtension;

                            var file = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/", newFileName);
                            using (var fileStream = new FileStream(file, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }

                            var picture = new Picture
                            {
                                UrlPath = apiUrl + "/uploads/" + newFileName
                            };
                            _context.Picture.Add(picture);
                            await _context.SaveChangesAsync();

                            var productPicture = new ProductPicture
                            {
                                ProductId = productId,
                                PictureId = picture.Id
                            };
                            _context.Product_ProductPicture_Mapping.Add(productPicture);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return new ServiceResponse<bool>() { Message = "File upload successfully", Success = true };
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<bool>() { Message = ex.Message, Success = false };
                }
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProductImage(int productId, int pictureId)
        {
            try
            {
                var apiUrl = _configuration.GetSection("Url:ApiUrl").Value;

                var product = await FindProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

                var productPicture = await _context.Product_ProductPicture_Mapping.Where(x => x.ProductId == product.Id && x.PictureId == pictureId).FirstOrDefaultAsync()
                    ?? throw new ArgumentException("This product is not mapped to this picture");

                var picture = await _context.Picture.FirstOrDefaultAsync(x => x.Id == productPicture.PictureId)
                    ?? throw new ArgumentException("No picture found with the specified id");

                var fileName = picture.UrlPath.Replace(apiUrl + "/uploads/", "");
                var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/", fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _context.Picture.Remove(picture);
                await _context.SaveChangesAsync();

                return new ServiceSuccessResponse<bool>() { Message = "Remove the mapped image with this product successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Message = ex.Message, Success = false };
            }
        }

        public async Task<Product> FindProductByIdAsync(int productId)
        {
            return await _context.Product.Where(x => x.Deleted == false)
                    .Include(x => x.AttributeMappings)
                    .ThenInclude(x => x.ProductAttribute)
                    .FirstOrDefaultAsync(x => x.Id == productId);
        }

        public async Task<ServiceResponse<bool>> DeleteAllProductImage(int productId)
        {
            try
            {
                var apiUrl = _configuration.GetSection("Url:ApiUrl").Value;

                var product = await _context.Product.FindProductByIdAsync(productId).FirstOrDefaultAsync()
                ?? throw new ArgumentException("No product found with the specified id");

                var productPicture = await _context.Product_ProductPicture_Mapping.Where(x => x.ProductId == product.Id).ToListAsync()
                    ?? throw new ArgumentException("This product is not mapped to this picture");

                foreach(var item in productPicture)
                {
                    var picture = await _context.Picture.FirstOrDefaultAsync(x => x.Id == item.PictureId)
                    ?? throw new ArgumentException("No picture found with the specified id");
                    var fileName = picture.UrlPath.Replace(apiUrl + "/uploads/", "");
                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/uploads/", fileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    _context.Picture.Remove(picture);
                    await _context.SaveChangesAsync();
                }

                return new ServiceSuccessResponse<bool>() { Message = "Remove the mapped image with this product successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Message = ex.Message, Success = false };
            }
        }
    }
}
