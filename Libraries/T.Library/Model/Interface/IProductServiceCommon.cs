using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;

namespace T.Library.Model.Interface
{
    public interface IProductServiceCommon
    {
        Task<List<Product>> GetAllNewestProduct();
        Task<List<Product>> GetRandomProduct();
        Task<string> GetFirstImagePathByProductId(int productId);
        Task<ServiceResponse<Product>> GetByIdAsync(int id);
        Task<ServiceResponse<Product>> GetByNameAsync(string name);
        Task<ServiceResponse<List<ProductPicture>>> GetProductPicturesByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> CreateProductAsync(Product product);
        //Task<ServiceResponse<bool>> CreateProducts(List<Product> products);
        Task<ServiceResponse<bool>> EditProductAsync(ProductModel model);
        Task<ServiceResponse<bool>> DeleteProductAsync(int productId);
        Task<ServiceResponse<List<ProductAttribute>>> GetAllProductAttributeByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> EditProductImageAsync(ProductPicture productPicture);
        Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId);
        Task<ServiceResponse<bool>> DeleteProductImage(int pictureMappingId);
        Task<ServiceResponse<bool>> DeleteAllProductImage(int productId);
        Task<ServiceResponse<List<Product>>> GetAllProductsDisplayedOnHomepageAsync(); 
    }
}
