using T.Library.Model;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeConverter
    {
        Task<string> ConvertToJsonAsync(List<ProductAttribute> attributeDtos, int productId);
    }
    public class ProductAttributeConverter
    {
    }
}
