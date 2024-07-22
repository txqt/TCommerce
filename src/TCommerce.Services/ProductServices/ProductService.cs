using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Data.SqlTypes;
using System.Drawing.Printing;
using System.Globalization;
using System.Text;
using TCommerce.Core.Extensions;
using TCommerce.Core.Helpers;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.CacheServices;
using TCommerce.Services.IRepositoryServices;
using TCommerce.Services.UserServices;

namespace TCommerce.Services.ProductServices
{
    /// <summary>
    /// Product service
    /// </summary>
    public class ProductService : IProductService
    {
        #region Fields

        private readonly IConfiguration _configuration;

        private readonly IHostEnvironment _environment;

        private readonly IRepository<Product> _productRepository;

        private readonly IRepository<ProductAttributeMapping> _productAttributeMappingRepository;

        private readonly IRepository<ProductPicture> _productPictureMappingRepository;

        private readonly IRepository<Picture> _pictureRepository;

        private readonly IRepository<ProductCategory> _productCategoryRepository;

        private readonly IRepository<ProductManufacturer> _productManufacturerRepository;

        private readonly IRepository<RelatedProduct> _relatedProductRepository;

        private string? ClientUrl;

        private IMapper _mapper;

        private readonly IUrlRecordService _urlRecordService;

        private readonly ICacheService _cacheService;

        private readonly IRepository<Category> _categoryRepository;

        private readonly IRepository<ProductAttribute> _productAttributeRepository;

        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;

        private readonly IRepository<DiscountProductMapping> _discountProductMappingRepository;
        #endregion

        #region Ctor
        public ProductService(IConfiguration configuration,
            IHostEnvironment environment, IRepository<Product> productsRepository,
            IRepository<ProductAttributeMapping> productAttributeMapping,
            IRepository<ProductPicture> productPictureMapping,
            IRepository<Picture> pictureRepository,
            IMapper mapper,
            IRepository<ProductCategory> productCategoryRepository,
            IUrlRecordService urlRecordService,
            ICacheService cacheService,
            IRepository<ProductManufacturer> productManufacturerRepository,
            IRepository<RelatedProduct> relatedProductRepository,
            IRepository<Category> categoryRepository,
            IRepository<ProductAttributeValue> productAttributeValueRepository, IRepository<ProductAttribute> productAttributeRepository, IRepository<DiscountProductMapping> discountProductMappingRepository)
        {
            _configuration = configuration;
            _environment = environment;
            ClientUrl = _configuration.GetSection("Url:ClientUrl").Value;
            _productRepository = productsRepository;
            _productAttributeMappingRepository = productAttributeMapping;
            _productPictureMappingRepository = productPictureMapping;
            _pictureRepository = pictureRepository;
            _mapper = mapper;
            _productCategoryRepository = productCategoryRepository;
            _urlRecordService = urlRecordService;
            _cacheService = cacheService;
            _productManufacturerRepository = productManufacturerRepository;
            _relatedProductRepository = relatedProductRepository;
            _categoryRepository = categoryRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _productAttributeRepository = productAttributeRepository;
            _discountProductMappingRepository = discountProductMappingRepository;
        }
        #endregion

        #region Methods
        public async Task<PagedList<Product>> SearchProductsAsync(int pageNumber = 0,
        int pageSize = int.MaxValue,
        IList<int>? categoryIds = null,
        IList<int>? manufacturerIds = null,
        bool excludeFeaturedProducts = false,
        decimal? priceMin = null,
        decimal? priceMax = null,
        int productTagId = 0,
        string? keywords = null,
        bool searchDescriptions = false,
        bool searchManufacturerPartNumber = true,
        bool searchSku = true,
        bool searchProductTags = false,
        string? orderBy = null,
        bool showHidden = false,
        List<int>? ids = null)
        {
            var query = _productRepository.Query;


            if (!showHidden)
                query = query.Where(p => p.Published);

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(p => ids.Contains(p.Id));
            }

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

            query = query.SearchByString(keywords ?? string.Empty)
               .Sort(orderBy ?? string.Empty)
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

        public async Task<PagedList<Product>> SearchProductsAsync(ProductParameters productParameters)
        {
            return await SearchProductsAsync(pageNumber: productParameters.PageNumber,
                pageSize: productParameters.PageSize,
                categoryIds: productParameters.CategoryIds,
                manufacturerIds: productParameters.ManufacturerIds,
                excludeFeaturedProducts: productParameters.ExcludeFeaturedProducts,
                priceMin: productParameters.PriceMin, priceMax: productParameters.PriceMax,
                productTagId: productParameters.ProductTagId,
                keywords: productParameters.SearchText,
                searchDescriptions: productParameters.SearchDescriptions,
                searchManufacturerPartNumber: productParameters.SearchManufacturerPartNumber,
                productParameters.SearchSku,
                productParameters.SearchProductTags,
                productParameters.OrderBy,
                productParameters.ShowHidden,
                productParameters.ids);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<ServiceResponse<bool>> CreateProductAsync(Product product)
        {
            try
            {
                product.CreatedOnUtc = DateTime.Now;

                await _productRepository.CreateAsync(product);

                if (product.Sku is null)
                {
                    product.Sku = GenerateSku(product, "PD");

                    await _productRepository.UpdateAsync(product);
                }
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProductAsync(Product product)
        {
            try
            {

                ArgumentNullException.ThrowIfNull(product);

                product.UpdatedOnUtc = DateTime.Now;

                if (product.Sku is null)
                {
                    product.Sku = GenerateSku(product, "PD");
                }

                await _productRepository.UpdateAsync(product);
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(ex.Message);
            }

            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(int productId)
        {
            await _productRepository.DeleteAsync(productId);
            return new ServiceSuccessResponse<bool>();
        }

        //public async Task<List<ProductAttribute>> GetAllProductAttributeByProductIdAsync(int productId)
        //{
        //    return await _productAttributeMappingRepository.Table
        //            .Where(pam => pam.ProductId == productId)
        //            .Select(pam => pam.ProductAttribute)
        //            .ToListAsync();
        //}

        public async Task<List<ProductPicture>?> GetProductPicturesByProductIdAsync(int productId)
        {
            var productPicture = await _productPictureMappingRepository.Table.Where(x => x.ProductId == productId)
                .ToListAsync();

            if (productPicture is not null)
            {
            }

            return productPicture;
        }

        public async Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId)
        {
            ArgumentNullException.ThrowIfNull(GetByIdAsync(productId));

            try
            {

                string path = Path.Combine(_environment.ContentRootPath, "wwwroot/images/uploads/");
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

                        var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/uploads/", newFileName);
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        var picture = new Picture
                        {
                            UrlPath = "/images/uploads/" + newFileName
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

                var product = await _productRepository.GetByIdAsync(productPicture.ProductId);

                ArgumentNullException.ThrowIfNull(product);

                var picture = await _pictureRepository.GetByIdAsync(productPicture.PictureId);

                ArgumentNullException.ThrowIfNull(picture);

                ArgumentNullException.ThrowIfNull(picture.UrlPath);

                var fileName = picture.UrlPath.Replace("/images/uploads/", "");

                var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/uploads/", fileName);
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
                var product = await _productRepository.GetByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

                var productPicture = await _productPictureMappingRepository.Table.Where(x => x.ProductId == product.Id).ToListAsync()
                    ?? throw new ArgumentException("This product is not mapped to this picture");

                foreach (var item in productPicture)
                {
                    var picture = await _pictureRepository.GetByIdAsync(item.PictureId)
                    ?? throw new ArgumentException("No picture found with the specified id");

                    if (picture.UrlPath is null)
                        throw new ArgumentNullException("Cannot find Url path");

                    var fileName = picture.UrlPath.Replace("/images/uploads/", "");
                    var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/images/uploads/", fileName);
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

            var newProducts = await _productRepository.Table
                .Where(p => p.MarkAsNew && p.Published && !p.Deleted
                    && (!p.MarkAsNewStartDateTimeUtc.HasValue || p.MarkAsNewStartDateTimeUtc <= currentDateTimeUtc)
                    && (!p.MarkAsNewEndDateTimeUtc.HasValue || p.MarkAsNewEndDateTimeUtc >= currentDateTimeUtc))
                .ToListAsync();

            return newProducts;
        }

        public async Task<List<Product>> GetRandomProduct()
        {
            var product = await _productRepository.Table.Where(x => x.Published && !x.Deleted).ToListAsync();

            product.Shuffle();

            return product;
        }

        public async Task<string> GetFirstImagePathByProductId(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            var productPicture = await _productPictureMappingRepository.Table.Where(x => x.ProductId == product.Id).FirstOrDefaultAsync();

            var fileName = "";

            if (productPicture is null)
            {
                fileName = "/images/no-pictrue.png";
            }

            return ClientUrl + fileName;
        }

        public async Task<Product?> GetByNameAsync(string name)
        {
            return await _productRepository.Table
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
            var products = await _productRepository.GetAllAsync(query =>
            {
                return from p in _productRepository.Table
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
            await _productRepository.BulkDeleteAsync(productIds);
            return new ServiceSuccessResponse<bool>();
        }


        public async Task<List<Product>> GetCategoryFeaturedProductsAsync(int categoryId)
        {
            List<Product> featuredProducts = new List<Product>();

            if (categoryId == 0)
                return featuredProducts;

            var cacheKey = "CategoryFeaturedProductsIdsKey_" + categoryId;

            var featuredProductIds = (await _productRepository.GetAllAsync(func: query =>
            {
                query = from p in _productRepository.Table
                        join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                        where p.Published && !p.Deleted &&
                              (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                              (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                              pc.IsFeaturedProduct && categoryId == pc.CategoryId
                        select p;

                return query;
            }, cacheKey: cacheKey)).Select(x => x.Id);

            if (!featuredProducts.Any() && featuredProductIds.Any())
                featuredProducts = (await _productRepository.GetByIdsAsync(featuredProductIds, null, false)).ToList();

            return featuredProducts;
        }
        public async Task<List<Product>> GetManufacturerFeaturedProductsAsync(int manufacturerId)
        {
            List<Product> featuredProducts = new List<Product>();

            if (manufacturerId == 0)
                return featuredProducts;

            var cacheKey = "ManufacturerFeaturedProductsIdsKey_" + manufacturerId;

            var featuredProductIds = (await _productRepository.GetAllAsync(func: query =>
            {
                query = from p in _productRepository.Table
                        join pc in _productManufacturerRepository.Table on p.Id equals pc.ProductId
                        where p.Published && !p.Deleted &&
                              (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                              (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                              pc.IsFeaturedProduct && manufacturerId == pc.ManufacturerId
                        select p;

                return query;
            }, cacheKey: cacheKey)).Select(x => x.Id);

            if (!featuredProducts.Any() && featuredProductIds.Any())
                featuredProducts = (await _productRepository.GetByIdsAsync(featuredProductIds, null, false)).ToList();

            return featuredProducts;
        }
        public bool ProductIsAvailable(Product product, DateTime? dateTime = null)
        {
            ArgumentNullException.ThrowIfNull(product);

            dateTime ??= DateTime.UtcNow;

            if (product.AvailableStartDateTimeUtc.HasValue && product.AvailableStartDateTimeUtc.Value > dateTime)
                return false;

            if (product.AvailableEndDateTimeUtc.HasValue && product.AvailableEndDateTimeUtc.Value < dateTime)
                return false;

            return true;
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
                        join p in _productRepository.Table on rp.ProductId2 equals p.Id
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
        public virtual async Task<RelatedProduct?> GetRelatedProductByIdAsync(int relatedProductId)
        {
            return await _relatedProductRepository.GetByIdAsync(relatedProductId);
        }

        /// <summary>
        /// Creates a related product
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
        public virtual RelatedProduct? FindRelatedProduct(IList<RelatedProduct> source, int productId1, int productId2)
        {
            return source.FirstOrDefault(rp => rp.ProductId1 == productId1 && rp.ProductId2 == productId2);
        }

        public async Task<List<Product>> GetProductsByIdsAsync(List<int> ids)
        {
            return (await _productRepository.GetByIdsAsync(ids)).ToList();
        }

        /// <summary>
        /// Generates a SKU for the product based on available information
        /// </summary>
        public string GenerateSku(Product product, string? prefix = null)
        {
            var skuParts = new List<string>();

            if (!string.IsNullOrEmpty(prefix))
            {
                skuParts.Add(prefix);
            }

            // Add the first letters of the product name
            if (!string.IsNullOrEmpty(product.Name))
            {
                skuParts.Add(GetInitials(product.Name));
            }

            //var categories = (from pc in _productCategoryRepository.Table
            //                  join c in _categoryRepository.Table
            //                  on pc.CategoryId equals c.Id
            //                  where pc.ProductId == product.Id
            //                  select c).ToList();

            //// Add the first letters of the first category name
            //if (categories != null && categories.Any())
            //{
            //    foreach (var category in categories)
            //    {
            //        skuParts.Add(GetInitials(category.Name));
            //    }

            //}

            //var attributes = (from pap in _productAttributeMappingRepository.Table
            //                  join pa in _productAttributeRepository.Table
            //                  on pap.ProductAttributeId equals pa.Id
            //                  where pap.ProductId == product.Id
            //                  select pa).ToList();

            //var attributeSku = new List<string>();

            //if (attributes != null && attributes.Any())
            //{
            //    foreach (var attribute in attributes)
            //    {

            //        attributeSku.Add(GetInitials(attribute.Name ?? "N/A"));
            //        var attributeValues = (from pap in _productAttributeMappingRepository.Table
            //                               join pav in _productAttributeValueRepository.Table
            //                               on pap.Id equals pav.ProductAttributeMappingId
            //                               where pap.ProductAttributeId == attribute.Id && pap.ProductId == product.Id
            //                               select pav).ToList();
            //        foreach (var av in attributeValues)
            //        {
            //            attributeSku.Add(GetInitials(av.Name ?? "N/A"));
            //        }
            //    }
            //}

            //if (attributeSku.Count > 0)
            //{
            //    skuParts.AddRange(attributeSku);
            //}

            if (product.Id > 0)
            {
                skuParts.Add(product.Id.ToString());
            }

            // Combine parts into a single SKU string
            return string.Join("-", skuParts).ToUpper();
        }
        private static string GetInitials(string input)
        {
            // Loại bỏ dấu khỏi chuỗi đầu vào
            string normalizedString = RemoveDiacritics(input);

            // Tách chuỗi thành các từ
            string[] words = normalizedString.Split(' ');

            // Biến để lưu trữ kết quả
            string initials = "";

            // Duyệt qua từng từ và lấy chữ cái đầu
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    initials += word[0];
                }
            }

            return initials;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        #endregion

        #region Product discounts

        /// <summary>
        /// Clean up product references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ClearDiscountProductMappingAsync(Discount discount)
        {
            ArgumentNullException.ThrowIfNull(discount);

            var mappingsWithProducts =
                from dcm in _discountProductMappingRepository.Table
                join p in _productRepository.Table on dcm.EntityId equals p.Id
                where dcm.DiscountId == discount.Id
                select dcm;

            foreach(var pdm in await mappingsWithProducts.ToListAsync())
            {
                await _discountProductMappingRepository.DeleteAsync(pdm.Id);
            }
        }

        /// <summary>
        /// Get a discount-product mapping records by product identifier
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<List<DiscountProductMapping>> GetAllDiscountsAppliedToProductAsync(int productId)
        {
            return (await _discountProductMappingRepository.GetAllAsync(query => query.Where(dcm => dcm.EntityId == productId))).ToList();
        }

        /// <summary>
        /// Get a discount-product mapping record
        /// </summary>
        /// <param name="productId">Product identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<DiscountProductMapping> GetDiscountAppliedToProductAsync(int productId, int discountId)
        {
            return await _discountProductMappingRepository.Table
                .FirstOrDefaultAsync(dcm => dcm.EntityId == productId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Creates a discount-product mapping record
        /// </summary>
        /// <param name="discountProductMapping">Discount-product mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task CreateDiscountProductMappingAsync(DiscountProductMapping discountProductMapping)
        {
            await _discountProductMappingRepository.CreateAsync(discountProductMapping);
        }

        /// <summary>
        /// Deletes a discount-product mapping record
        /// </summary>
        /// <param name="discountProductMapping">Discount-product mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteDiscountProductMappingAsync(int discountProductMappingId)
        {
            await _discountProductMappingRepository.DeleteAsync(discountProductMappingId);
        }

        public async Task<List<Product>> GetProductsWithAppliedDiscountAsync(int? discountId = null, bool showHidden = false)
        {
            var products = _productRepository.Query;

            if (discountId.HasValue)
                products = from product in products
                           join dpm in _discountProductMappingRepository.Table on product.Id equals dpm.EntityId
                           where dpm.DiscountId == discountId.Value
                           select product;

            if (!showHidden)
                products = products.Where(product => !product.Deleted);

            products = products.OrderBy(product => product.DisplayOrder).ThenBy(product => product.Id);

            return await products.ToListAsync();
        }
        #endregion
    }
}

