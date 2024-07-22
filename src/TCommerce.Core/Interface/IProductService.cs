using Microsoft.AspNetCore.Http;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Core.Interface
{
    public interface IProductService
    {
        #region Products
        Task<PagedList<Product>> SearchProductsAsync(int pageNumber = 0,
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
        List<int>? ids = null);
        Task<PagedList<Product>> SearchProductsAsync(ProductParameters productParameters);
        Task<List<Product>> GetAllNewestProduct();
        Task<List<Product>> GetRandomProduct();
        Task<string> GetFirstImagePathByProductId(int productId);
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetProductsByIdsAsync(List<int> ids);
        Task<Product?> GetByNameAsync(string name);
        Task<ServiceResponse<bool>> CreateProductAsync(Product product);
        Task<ServiceResponse<bool>> UpdateProductAsync(Product product);
        Task<ServiceResponse<bool>> DeleteProductAsync(int productId);
        Task<ServiceSuccessResponse<bool>> BulkDeleteProductsAsync(IEnumerable<int> productIds);
        Task<List<Product>> GetAllProductsDisplayedOnHomepageAsync();
        Task<List<Product>> GetCategoryFeaturedProductsAsync(int categoryId);
        Task<List<Product>> GetManufacturerFeaturedProductsAsync(int manufacturerId);
        public bool ProductIsAvailable(Product product, DateTime? dateTime = null);
        #endregion

        #region ProductPictures
        Task<List<ProductPicture>?> GetProductPicturesByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId);
        Task<ServiceResponse<bool>> EditProductImageAsync(ProductPicture productPicture);
        Task<ServiceResponse<bool>> DeleteProductImage(int pictureMappingId);
        Task<ServiceResponse<bool>> DeleteAllProductImage(int productId);
        #endregion

        #region Related products
        Task<ServiceResponse<bool>> DeleteRelatedProductAsync(int relatedProductId);
        Task<List<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId1, bool showHidden = false);
        Task<RelatedProduct?> GetRelatedProductByIdAsync(int relatedProductId);
        Task<ServiceResponse<bool>> CreateRelatedProductAsync(RelatedProduct relatedProduct);
        Task<ServiceResponse<bool>> UpdateRelatedProductAsync(RelatedProduct relatedProduct);
        RelatedProduct? FindRelatedProduct(IList<RelatedProduct> source, int productId1, int productId2);
        #endregion

        #region Product discounts
        Task ClearDiscountProductMappingAsync(Discount discount);
        Task<List<DiscountProductMapping>> GetAllDiscountsAppliedToProductAsync(int productId);
        Task<List<Product>> GetProductsWithAppliedDiscountAsync(int? discountId = null, bool showHidden = false);
        Task<DiscountProductMapping> GetDiscountAppliedToProductAsync(int productId, int discountId);
        Task CreateDiscountProductMappingAsync(DiscountProductMapping discountProductMapping);
        Task DeleteDiscountProductMappingAsync(int discountProductMappingId);
        #endregion
    }
}
