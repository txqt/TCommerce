using AutoMapper;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using T.Library.Helpers;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.WebApi.Extensions;
using T.WebApi.Helpers;
using T.WebApi.Services.CacheServices;
using T.WebApi.Services.IRepositoryServices;
using T.WebApi.Services.UrlRecordServices;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductService : IProductServiceCommon
    {
        Task<PagedList<Product>> SearchProduct(ProductParameters productParameters);
    }
    /// <summary>
    /// Product service
    /// </summary>
    public class ProductService : IProductService
    {
        #region Fields

        private readonly IConfiguration _configuration;

        private readonly IHostEnvironment _environment;

        private readonly IRepository<Product> _productsRepository;

        private readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;

        private readonly IRepository<ProductPicture> _productPictureMappingRepository;

        private readonly IRepository<Picture> _pictureRepository;

        private readonly IRepository<ProductCategory> _productCategoryRepository;

        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;

        private string? APIUrl;

        private IMapper _mapper;

        private readonly IUrlRecordService _urlRecordService;

        private readonly ICacheService _cacheService;
        #endregion

        #region Ctor
        public ProductService(IConfiguration configuration, IHostEnvironment environment, IRepository<Product> productsRepository, IRepository<ProductAttributeMapping> productAttributeMapping, IRepository<ProductPicture> productPictureMapping, IRepository<Picture> pictureRepository, IMapper mapper, IRepository<ProductCategory> productCategoryRepository, IUrlRecordService urlRecordService, ICacheService cacheService, IRepository<ProductManufacturer> productManufacturerRepository)
        {
            _configuration = configuration;
            _environment = environment;
            APIUrl = _configuration.GetSection("Url:APIUrl").Value;
            _productsRepository = productsRepository;
            _productAttributeMappingRepository = productAttributeMapping;
            _productPictureMappingRepository = productPictureMapping;
            _pictureRepository = pictureRepository;
            _mapper = mapper;
            _productCategoryRepository = productCategoryRepository;
            _urlRecordService = urlRecordService;
            _cacheService = cacheService;
            _productManufacturerRepository = productManufacturerRepository;
        }
        #endregion

        #region Methods
        public async Task<PagedList<Product>> SearchProduct(ProductParameters productParameters)
        {
            string GetKey()
            {
                var properties = typeof(ProductParameters).GetProperties();
                var keyParts = properties.Select(prop => prop.GetValue(productParameters)?.ToString() ?? "null");
                return string.Join("_", keyParts);
            }

            var cacheKey = CacheKeysDefault<Product>.AllPrefix + GetKey();

            // Attempt to get data from cache
            var cachedData = _cacheService.GetData<PagedList<Product>>(cacheKey);

            if (cachedData != null)
            {
                // Return data from cache
                return cachedData;
            }
            else
            {
                var query = _productsRepository.Query;

                if (productParameters.ids != null && productParameters.ids.Count > 0)
                {
                    query = query.Where(p => productParameters.ids.Contains(p.Id));
                }

                if (productParameters.CategoryId > 0)
                {
                    query = from p in query
                            join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                            where pc.CategoryId == productParameters.CategoryId
                            select p;
                }

                if (productParameters.ManufacturerId > 0)
                {
                    query = from p in query
                            join pc in _productManufacturerRepository.Table on p.Id equals pc.ProductId
                            where pc.ManufacturerId == productParameters.ManufacturerId
                            select p;
                }

                query = query.SearchByString(productParameters.SearchText)
                   .Sort(productParameters.OrderBy)// sắp xếp theo cột sản phẩm 
                   .Include(x => x.ProductPictures)
                   .Where(x => x.Deleted == false);

                query = query.OrderBy(product => product.Id);


                var result = await PagedList<Product>.ToPagedList
                    (query, productParameters.PageNumber, productParameters.PageSize);

                // Store data in cache with an expiration time (adjust as needed)
                _cacheService.SetData(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(10));

                return result;
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _productsRepository.GetByIdAsync(id);
        }

        public async Task<ServiceResponse<bool>> CreateProductAsync(ProductModel model)
        {
            try
            {
                var product = _mapper.Map<Product>(model);

                product.CreatedOnUtc = DateTime.Now;

                await _productsRepository.CreateAsync(product);

                model.SeName = await _urlRecordService.ValidateSlug(product, model.SeName, product.Name, true);

                await _urlRecordService.SaveSlugAsync(product, model.SeName);

                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<bool>> EditProductAsync(ProductModel model)
        {

            try
            {
                var product = (await _productsRepository.Table.Include(x => x.AttributeMappings).FirstOrDefaultAsync(x => x.Id == model.Id))
                    ?? throw new ArgumentException("No product found with the specified id");

                _mapper.Map(model, product);

                product.UpdatedOnUtc = DateTime.Now;

                await _productsRepository.UpdateAsync(product);

                if (model.SeName != (await _urlRecordService.GetSeNameAsync(product)))
                {
                    model.SeName = await _urlRecordService.ValidateSlug(product, model.SeName, product.Name, true);

                    await _urlRecordService.SaveSlugAsync(product, model.SeName);
                }
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }

            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(int productId)
        {
            await _productsRepository.DeleteAsync(productId);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<List<ProductAttribute>> GetAllProductAttributeByProductIdAsync(int productId)
        {
            return await _productAttributeMappingRepository.Table
                    .Where(pam => pam.ProductId == productId)
                    .Select(pam => pam.ProductAttribute)
                    .ToListAsync();
        }

        public async Task<List<ProductPicture>> GetProductPicturesByProductIdAsync(int productId)
        {
            var productPicture = await _productPictureMappingRepository.Table.Where(x => x.ProductId == productId)
                .Include(x => x.Picture)
                .ToListAsync();

            foreach (var pp in productPicture)
            {
                pp.Picture.UrlPath = APIUrl + pp.Picture.UrlPath;
            }

            return productPicture;
        }

        public async Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId)
        {
            ArgumentNullException.ThrowIfNull(GetByIdAsync(productId));

            try
            {

                string path = Path.Combine(_environment.ContentRootPath, "wwwroot/images/");
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

                        var file = Path.Combine(_environment.ContentRootPath, "wwwroot/images/", newFileName);
                        using (var fileStream = new FileStream(file, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        var picture = new Picture
                        {
                            UrlPath = "/images/" + newFileName
                        };
                        await _pictureRepository.CreateAsync(picture);

                        var productPicture = new ProductPicture
                        {
                            ProductId = productId,
                            PictureId = picture.Id
                        };
                        await _productPictureMappingRepository.CreateAsync(productPicture);
                    }
                }
                return new ServiceResponse<bool>() { Message = "File upload successfully", Success = true };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Message = ex.Message, Success = false };
            }

        }

        public async Task<ServiceResponse<bool>> DeleteProductImage(int pictureMappingId)
        {
            try
            {
                var productPicture = await _productPictureMappingRepository.GetByIdAsync(pictureMappingId);

                ArgumentNullException.ThrowIfNull(productPicture);

                var product = await _productsRepository.GetByIdAsync(productPicture.ProductId);

                ArgumentNullException.ThrowIfNull(product);

                var picture = await _pictureRepository.GetByIdAsync(productPicture.PictureId);

                ArgumentNullException.ThrowIfNull(picture);

                if (picture.UrlPath is null);

                ArgumentNullException.ThrowIfNull(picture.UrlPath);

                var fileName = picture.UrlPath.Replace("/images/", "");

                var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/", fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                await _pictureRepository.DeleteAsync(productPicture.PictureId);
                return new ServiceSuccessResponse<bool>() { Message = "Remove the mapped image with this product successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Message = ex.Message, Success = false };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAllProductImage(int productId)
        {
            try
            {
                var product = await _productsRepository.GetByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

                var productPicture = await _productPictureMappingRepository.Table.Where(x => x.ProductId == product.Id).ToListAsync()
                    ?? throw new ArgumentException("This product is not mapped to this picture");

                foreach (var item in productPicture)
                {
                    var picture = await _pictureRepository.GetByIdAsync(item.PictureId)
                    ?? throw new ArgumentException("No picture found with the specified id");

                    if (picture.UrlPath is null)
                        throw new ArgumentNullException("Cannot find Url path");

                    var fileName = picture.UrlPath.Replace("/images/", "");
                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/", fileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    await _pictureRepository.DeleteAsync(picture.Id);
                }

                return new ServiceSuccessResponse<bool>() { Message = "Remove the mapped image with this product successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>() { Message = ex.Message, Success = false };
            }
        }

        public async Task<List<Product>> GetAllNewestProduct()
        {
            DateTime currentDateTimeUtc = DateTime.UtcNow;

            var newProducts = await _productsRepository.Table
                .Where(p => p.MarkAsNew && p.Published && !p.Deleted
                    && (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc <= currentDateTimeUtc)
                    && (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc >= currentDateTimeUtc))
                .ToListAsync();

            return newProducts;
        }

        public async Task<List<Product>> GetRandomProduct()
        {
            var product = await _productsRepository.Table.Where(x => x.Published && !x.Deleted).ToListAsync();

            product.Shuffle();

            return product;
        }

        public async Task<string> GetFirstImagePathByProductId(int productId)
        {
            var product = await _productsRepository.GetByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            var productPicture = await _productPictureMappingRepository.Table.Where(x => x.ProductId == product.Id).Include(x => x.Picture).FirstOrDefaultAsync();

            var fileName = "";

            if (productPicture is null)
            {
                fileName = "/images/no-pictrue.png";
            }
            else
            {
                fileName = productPicture.Picture.UrlPath;
            }

            return APIUrl + fileName;
        }

        public async Task<Product> GetByNameAsync(string name)
        {
            return await _productsRepository.Table
                        .Where(x => x.Name != null && x.Name.ToLower() == name.ToLower())
                        .FirstOrDefaultAsync();
        }

        public async Task<ServiceResponse<bool>> EditProductImageAsync(ProductPicture productPicture)
        {
            await _productPictureMappingRepository.UpdateAsync(productPicture);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<List<Product>> GetAllProductsDisplayedOnHomepageAsync()
        {
            var products = await _productsRepository.GetAllAsync(query =>
            {
                return from p in query
                       orderby p.DisplayOrder, p.Id
                       where p.Published &&
                             !p.Deleted &&
                             p.ShowOnHomepage
                       select p;
            }, CacheKeysDefault<Product>.AllPrefix + "home-page");

            return products.ToList();
        }

        public async Task<ServiceSuccessResponse<bool>> BulkDeleteProductsAsync(IEnumerable<int> productIds)
        {
            await _productsRepository.BulkDeleteAsync(productIds);
            return new ServiceSuccessResponse<bool>();
        }

        #endregion
    }
}
