using Microsoft.AspNetCore.Http;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Core.Interface
{
    public interface IProductAttributeConverter
    {
        Task<string> ConvertToJsonAsync(List<ShoppingCartItemModel.SelectedAttribute> attributeDtos);
        List<ShoppingCartItemModel.SelectedAttribute> ConvertToObject(string attributesJson);
        Task<List<ShoppingCartItemModel.SelectedAttribute>> ParseProductAttributesAsync(IFormCollection form, Product product);

    }
}
