using T.Library.Model;
using T.Library.Model.Response;

namespace T.Web.Services.ProductService
{
    public interface IProductPictureService
    {
        Task<ServiceResponse<ProductPicture>> GetAsync(int productId);
    }
    public class ProductPictureService : IProductPictureService
    {
        public Task<ServiceResponse<ProductPicture>> GetAsync(int productId)
        {
            throw new NotImplementedException();
        }
    }
}
