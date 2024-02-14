using AutoMapper;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Text;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.WebApi.Extensions;
using T.WebApi.Helpers;
using T.WebApi.Services.CacheServices;
using T.WebApi.Services.IRepositoryServices;
using T.WebApi.Services.UrlRecordServices;
using T.WebApi.Services.UserServices;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductService : IProductServiceCommon
    {
        Task<PagedList<Product>> SearchProduct(int pageNumber = 0,
        int pageSize = int.MaxValue,
        IList<int> categoryIds = null,
        IList<int> manufacturerIds = null,
        bool excludeFeaturedProducts = false,
        decimal? priceMin = null,
        decimal? priceMax = null,
        int productTagId = 0,
        string keywords = null,
        bool searchDescriptions = false,
        bool searchManufacturerPartNumber = true,
        bool searchSku = true,
        bool searchProductTags = false,
        string orderBy = null,
        bool showHidden = false,
        List<int> ids = null);
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

        private readonly IRepository<RelatedProduct> _relatedProductRepository;

        private string? APIUrl;

        private IMapper _mapper;

        private readonly IUrlRecordService _urlRecordService;

        private readonly ICacheService _cacheService;

        private readonly IUserService _userSerice;
        #endregion

        #region Ctor
        public ProductService(IConfiguration configuration, IHostEnvironment environment, IRepository<Product> productsRepository, IRepository<ProductAttributeMapping> productAttributeMapping, IRepository<ProductPicture> productPictureMapping, IRepository<Picture> pictureRepository, IMapper mapper, IRepository<ProductCategory> productCategoryRepository, IUrlRecordService urlRecordService, ICacheService cacheService, IRepository<ProductManufacturer> productManufacturerRepository, IRepository<RelatedProduct> relatedProductRepository, IUserService userSerice)
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
            _relatedProductRepository = relatedProductRepository;
            _userSerice = userSerice;
        }
        #endregion

        #region Methods
        public async Task<PagedList<Product>> SearchProduct(int pageNumber = 0,
        int pageSize = int.MaxValue,
        IList<int> categoryIds = null,
        IList<int> manufacturerIds = null,
        bool excludeFeaturedProducts = false,
        decimal? priceMin = null,
        decimal? priceMax = null,
        int productTagId = 0,
        string keywords = null,
        bool searchDescriptions = false,
        bool searchManufacturerPartNumber = true,
        bool searchSku = true,
        bool searchProductTags = false,
        string orderBy = null,
        bool showHidden = false,
        List<int> ids = null)
        {
            var query = _productsRepository.Query;


            if (!showHidden)
                query = query.Where(p => p.Published);

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(p => ids.Contains(p.Id));
            }


            var t = await query.ToListAsync();
            if (categoryIds is not null)
            {
                categoryIds.Remove(0);
                if (categoryIds.Any())
                    query = from p in query
                            join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                            where categoryIds.Contains(pc.CategoryId)
                            select p;
            }

            if (manufacturerIds is not null)
            {
                manufacturerIds.Remove(0);
                if (manufacturerIds.Any())
                    query = from p in query
                            join pm in _productManufacturerRepository.Table on p.Id equals pm.ProductId
                            where manufacturerIds.Contains(pm.ManufacturerId)
                            select p;
            }

            query = query.SearchByString(keywords)
               .Sort(orderBy)
               .Include(x => x.ProductPictures)
               .Where(x => x.Deleted == false);

            query = from p in query
                    where !p.Deleted &&
                          (showHidden ||
                           DateTime.UtcNow >= (p.AvailableStartDateTimeUtc ?? SqlDateTime.MinValue.Value) &&
                           DateTime.UtcNow <= (p.AvailableEndDateTimeUtc ?? SqlDateTime.MaxValue.Value)
                          ) &&
                          (priceMin == null || p.Price >= priceMin) &&
                          (priceMax == null || p.Price <= priceMax)
                    select p;

            return await PagedList<Product>.ToPagedList
                (query, pageNumber, pageSize);
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
                var product = (await _productsRepository.Table.Include(x => x.AttributeMappings).FirstOrDefaultAsync(x => x.Id == model.Id));

                ArgumentNullException.ThrowIfNull(product);

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

        //public async Task<List<ProductAttribute>> GetAllProductAttributeByProductIdAsync(int productId)
        //{
        //    return await _productAttributeMappingRepository.Table
        //            .Where(pam => pam.ProductId == productId)
        //            .Select(pam => pam.ProductAttribute)
        //            .ToListAsync();
        //}

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

                if (picture.UrlPath is null) ;

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
                return from p in _productsRepository.Table
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


        public async Task<List<Product>> GetCategoryFeaturedProductsAsync(int categoryId)
        {
            List<Product> featuredProducts = new List<Product>();

            if (categoryId == 0)
                return featuredProducts;

            var userModel = await _userSerice.GetCurrentUser();
            var user = _mapper.Map<User>(userModel);
            var customerRoleIds = await _userSerice.GetRolesByUserAsync(user);
            var cacheKey = "CategoryFeaturedProductsIdsKey_" + categoryId;

            var featuredProductIds = (await _productsRepository.GetAllAsync(func: query =>
            {
                query = from p in _productsRepository.Table
                        join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                        where p.Published && !p.Deleted &&
                              (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                              (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                              pc.IsFeaturedProduct && categoryId == pc.CategoryId
                        select p;

                return query;
            }, cacheKey: cacheKey)).Select(x => x.Id);

            if (!featuredProducts.Any() && featuredProductIds.Any())
                featuredProducts = (await _productsRepository.GetByIdsAsync(featuredProductIds, null, false)).ToList();

            return featuredProducts;
        }
        #endregion

        #region Related products

        /// <summary>
        /// Deletes a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<ServiceResponse<bool>> DeleteRelatedProductAsync(int relatedProductId)
        {
            await _relatedProductRepository.DeleteAsync(relatedProductId);
            return new ServiceSuccessResponse<bool>();
        }

        /// <summary>
        /// Gets related products by product identifier
        /// </summary>
        /// <param name="productId">The first product identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related products
        /// </returns>
        public virtual async Task<List<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId, bool showHidden = false)
        {
            var query = from rp in _relatedProductRepository.Table
                        join p in _productsRepository.Table on rp.ProductId2 equals p.Id
                        where rp.ProductId1 == productId &&
                              !p.Deleted &&
                              (showHidden || p.Published)
                        orderby rp.DisplayOrder, rp.Id
                        select rp;

            var relatedProducts = (await _relatedProductRepository.GetAllAsync(x =>
            {
                return query;
            })).ToList();

            return relatedProducts;
        }

        /// <summary>
        /// Gets a related product
        /// </summary>
        /// <param name="relatedProductId">Related product identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the related product
        /// </returns>
        public virtual async Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId)
        {
            return await _relatedProductRepository.GetByIdAsync(relatedProductId);
        }

        /// <summary>
        /// Inserts a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<ServiceResponse<bool>> CreateRelatedProductAsync(RelatedProduct relatedProduct)
        {
            await _relatedProductRepository.CreateAsync(relatedProduct);
            return new ServiceSuccessResponse<bool>();
        }

        /// <summary>
        /// Updates a related product
        /// </summary>
        /// <param name="relatedProduct">Related product</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<ServiceResponse<bool>> UpdateRelatedProductAsync(RelatedProduct relatedProduct)
        {
            await _relatedProductRepository.UpdateAsync(relatedProduct);
            return new ServiceSuccessResponse<bool>();
        }

        /// <summary>
        /// Finds a related product item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="productId1">The first product identifier</param>
        /// <param name="productId2">The second product identifier</param>
        /// <returns>Related product</returns>
        public virtual RelatedProduct FindRelatedProduct(IList<RelatedProduct> source, int productId1, int productId2)
        {
            return source.FirstOrDefault(rp => rp.ProductId1 == productId1 && rp.ProductId2 == productId2);
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> ids)
        {
            return (await _productsRepository.GetByIdsAsync(ids)).ToList();
        }


        #endregion
    }
}
