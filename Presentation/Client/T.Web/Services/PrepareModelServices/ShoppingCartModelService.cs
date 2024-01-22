using System.Text;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Web.Models;
using T.Web.Services.ProductService;
using T.Web.Services.ShoppingCartServices;
using T.Web.Services.UrlRecordService;
using T.Web.Services.UserService;

namespace T.Web.Services.PrepareModelServices
{
    public interface IShoppingCartModelService
    {
        Task<MiniShoppingCartModel> PrepareMiniShoppingCartModelAsync();
    }
    public class ShoppingCartModelService : IShoppingCartModelService
    {
        private readonly IUserService _userService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductAttributeCommon _productAttributeService;

        public ShoppingCartModelService(IUserService userService, IShoppingCartService shoppingCartService, IProductService productService, IUrlRecordService urlRecordService, IProductAttributeCommon productAttributeService)
        {
            _userService = userService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _productAttributeService = productAttributeService;
        }

        public virtual async Task<MiniShoppingCartModel> PrepareMiniShoppingCartModelAsync()
        {
            var user = await _userService.GetCurrentUser();
            var model = new MiniShoppingCartModel
            {
                //ShowProductImages = _shoppingCartSettings.ShowProductImagesInMiniShoppingCart,
                //let's always display it
                DisplayShoppingCartButton = true,
            };

            //performance optimization (use "HasShoppingCartItems" property)
            if (user.HasShoppingCartItems)
            {
                var cart = await _shoppingCartService.GetShoppingCartAsync();

                if (cart.Any())
                {
                    model.TotalProducts = cart.Sum(item => item.Quantity);

                    var cartProductIds = cart.Select(ci => ci.ProductId).ToArray();

                    model.SubTotalValue = 0;
                    //products. sort descending (recently added products)
                    foreach (var sci in cart
                                 .OrderByDescending(x => x.Id)
                                 .ToList())
                    {
                        string separator = "<br />";
                        var product = await _productService.GetByIdAsync(sci.ProductId);

                        var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel
                        {
                            Id = sci.Id,
                            ProductId = sci.ProductId,
                            ProductName = product.Name,
                            ProductSeName = await _urlRecordService.GetActiveSlugAsync(product.Id, nameof(Product)),
                            Quantity = sci.Quantity,
                            Price = product.Price.ToString("N0")
                        };

                        model.SubTotalValue += product.Price * sci.Quantity;

                        var result = new StringBuilder();

                        if(sci.Attributes is not null)
                        {
                            foreach (var selectedAttribute in sci.Attributes)
                            {
                                var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(selectedAttribute.ProductAttributeMappingId);
                                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId);

                                var attributeName = productAttribute.Name;

                                foreach (var attributeValueId in selectedAttribute.ProductAttributeValueIds)
                                {
                                    var attributeValue = await _productAttributeService.GetProductAttributeValuesByIdAsync(attributeValueId);
                                    var formattedAttribute = $"{attributeName}: {attributeValue.Name}";



                                    if (!string.IsNullOrEmpty(formattedAttribute))
                                    {
                                        if (result.Length > 0)
                                        {
                                            result.Append(separator);
                                        }
                                        result.Append(formattedAttribute);
                                    }
                                }
                            }
                        }

                        cartItemModel.AttributeInfo = result.ToString();
                        model.SubTotal = model.SubTotalValue.ToString("N0");
                        model.Items.Add(cartItemModel);
                    }
                }
            }

            return model;
        }
    }
}
