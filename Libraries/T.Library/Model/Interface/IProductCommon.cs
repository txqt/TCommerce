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
    public interface IProductCommon
    {
        Task<List<Product>> GetAllNewestProduct();
        Task<List<Product>> GetRandomProduct();
        Task<string> GetFirstImagePathByProductId(int productId);
        Task<ServiceResponse<Product>> GetByIdAsync(int id);
        Task<ServiceResponse<Product>> GetByNameAsync(string name);
        Task<ServiceResponse<List<ProductPicture>>> GetProductPicturesByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> CreateProduct(Product product);
        //Task<ServiceResponse<bool>> CreateProducts(List<Product> products);
        Task<ServiceResponse<bool>> EditProduct(ProductModel model);
        Task<ServiceResponse<bool>> DeleteProduct(int productId);
        Task<ServiceResponse<List<ProductAttribute>>> GetAllProductAttributeByProductIdAsync(int productId);
        Task<ServiceResponse<bool>> AddProductImage(List<IFormFile> ListImages, int productId);
        Task<ServiceResponse<bool>> DeleteProductImage(int productId, int pictureId);
        Task<ServiceResponse<bool>> DeleteAllProductImage(int productId);
    }
}
