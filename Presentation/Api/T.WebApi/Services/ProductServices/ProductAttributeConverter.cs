using Newtonsoft.Json;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.ViewsModel;
using static T.Library.Model.ViewsModel.ShoppingCartItemModel;

namespace T.WebApi.Services.ProductServices
{
    public interface IProductAttributeConverter
    {
        Task<string> ConvertToJsonAsync(List<ShoppingCartItemModel.SelectedAttribute> attributeDtos, int productId);
        Task<List<ShoppingCartItemModel.SelectedAttribute>> ConvertToObject(string attributesJson);

    }
    public class ProductAttributeConverter : IProductAttributeConverter
    {
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributeConverter(IProductAttributeService productAttributeService)
        {
            _productAttributeService = productAttributeService;
        }

        public async Task<string> ConvertToJsonAsync(List<ShoppingCartItemModel.SelectedAttribute> attributeDtos, int productId)
        {
            var attributesJson = "";

            if (attributeDtos is not { Count: > 0 })
            {
                return attributesJson;
            }

            foreach (var attribute in attributeDtos)
            {
                ArgumentNullException.ThrowIfNull((await _productAttributeService.GetProductAttributeMappingByIdAsync(attribute.ProductAttributeMappingId)));
            }

            attributesJson = JsonConvert.SerializeObject(attributeDtos); ;

            return attributesJson;
        }

        public async Task<List<SelectedAttribute>> ConvertToObject(string attributesJson)
        {
            return JsonConvert.DeserializeObject<List<SelectedAttribute>>(attributesJson);
        }
    }
}
