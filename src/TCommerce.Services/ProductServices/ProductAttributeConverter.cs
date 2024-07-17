using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.ViewsModel;
using static TCommerce.Core.Models.ViewsModel.ShoppingCartItemModel;

namespace TCommerce.Services.ProductServices
{
    public class ProductAttributeConverter : IProductAttributeConverter
    {
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributeConverter(IProductAttributeService productAttributeService)
        {
            _productAttributeService = productAttributeService;
        }

        public async Task<string> ConvertToJsonAsync(List<ShoppingCartItemModel.SelectedAttribute> attributeDtos)
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

            attributesJson = JsonConvert.SerializeObject(attributeDtos);

            return attributesJson;
        }

        public List<SelectedAttribute> ConvertToObject(string attributesJson)
        {
            return JsonConvert.DeserializeObject<List<SelectedAttribute>>(attributesJson)!;
        }

        public async Task<List<SelectedAttribute>> ParseProductAttributesAsync(IFormCollection form, Product product)
        {
            var productAttributePrefix = "product_attribute_";
            var selectedAttributes = new List<ShoppingCartItemModel.SelectedAttribute>();

            foreach (var formKey in form.Keys)
            {
                if (formKey.StartsWith(productAttributePrefix))
                {
                    var mappingId = GetNumberFromPrefix(formKey, productAttributePrefix);
                    var attributesMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(mappingId);

                    if (attributesMapping is null) continue;

                    var controlId = $"{productAttributePrefix}{attributesMapping.Id}";
                    switch (attributesMapping.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                var ctrlAttributes = form[controlId];
                                if (!StringValues.IsNullOrEmpty(ctrlAttributes) && int.TryParse(ctrlAttributes, out var selectedAttributeId) && selectedAttributeId > 0)
                                {
                                    var productAttributeValue = await _productAttributeService.GetProductAttributeValuesByIdAsync(selectedAttributeId);
                                    if (productAttributeValue is not null)
                                    {
                                        selectedAttributes.Add(new ShoppingCartItemModel.SelectedAttribute
                                        {
                                            ProductAttributeMappingId = attributesMapping.Id,
                                            ProductAttributeValueIds = new List<int> { productAttributeValue.Id }
                                        });
                                    }
                                }
                            }
                            break;

                        case AttributeControlType.Checkboxes:
                            {
                                var cblAttributes = form[controlId];
                                if (!StringValues.IsNullOrEmpty(cblAttributes))
                                {
                                    var selectedAttribute = new ShoppingCartItemModel.SelectedAttribute
                                    {
                                        ProductAttributeMappingId = attributesMapping.Id,
                                        ProductAttributeValueIds = cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(int.Parse)
                                            .Where(id => id > 0)
                                            .ToList()
                                    };

                                    if (selectedAttribute.ProductAttributeValueIds.Any())
                                    {
                                        selectedAttributes.Add(selectedAttribute);
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            return selectedAttributes;
        }
        public static int GetNumberFromPrefix(string input, string prefix)
        {
            if (input.StartsWith(prefix))
            {
                string numberString = input.Substring(prefix.Length);

                if (int.TryParse(numberString, out int number))
                {
                    return number;
                }
            }
            return -1; // Trả về -1 nếu không thể chuyển đổi thành số
        }
    }
}
