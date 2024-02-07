using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Catalogs;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;

namespace T.Library.Model.Interface
{
    public interface IProductServiceCommon
    {
        #region Products
        Task<List<Product>> GetAllNewestProduct();
        Task<List<Product>> GetRandomProduct();
        Task<string> GetFirstImagePathByProductId(int productId);
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetByNameAsync(string name);
        Task<ServiceResponse<bool>> CreateProductAsync(ProductModel model);
        Task<ServiceResponse<bool>> EditProductAsync(ProductModel model);
        Task<ServiceResponse<bool>> DeleteProductAsync(int productId);
        Task<ServiceSuccessResponse<bool>> BulkDeleteProductsAsync(IEnumerable<int> productIds);
        Task<List<Product>> GetAllProductsDisplayedOnHomepageAsync();
        #endregion
        #region ProductPictures
        Task<List<ProductPicture>> GetProductPicturesByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId);
        Task<ServiceResponse<bool>> EditProductImageAsync(ProductPicture productPicture);
        Task<ServiceResponse<bool>> DeleteProductImage(int pictureMappingId);
        Task<ServiceResponse<bool>> DeleteAllProductImage(int productId);
        #endregion

        #region Related products
        Task<ServiceResponse<bool>> DeleteRelatedProductAsync(int relatedProductId);
        Task<List<RelatedProduct>> GetRelatedProductsByProductId1Async(int productId1, bool showHidden = false);
        Task<RelatedProduct> GetRelatedProductByIdAsync(int relatedProductId);
        Task<ServiceResponse<bool>> CreateRelatedProductAsync(RelatedProduct relatedProduct);
        Task<ServiceResponse<bool>> UpdateRelatedProductAsync(RelatedProduct relatedProduct);
        RelatedProduct FindRelatedProduct(IList<RelatedProduct> source, int productId1, int productId2);
        #endregion

        //Task<List<ProductAttribute>> GetAllProductAttributeByProductIdAsync(int productId);
    }
}
