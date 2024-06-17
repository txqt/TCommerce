using System.Text;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Orders;
using T.Web.Models;
using T.Web.Services.PictureServices;
using T.Web.Services.ProductService;
using T.Web.Services.ShoppingCartServices;
using T.Web.Services.UrlRecordService;
using T.Web.Services.UserService;

namespace T.Web.Services.PrepareModelServices
{
    public interface IShoppingCartModelService
    {
        Task<MiniShoppingCartModel> PrepareMiniShoppingCartModelAsync();
        Task<ShoppingCartModel> PrepareShoppingCartModelAsync();
        Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(List<ShoppingCartItem> cart);
    }
    public class ShoppingCartModelService : IShoppingCartModelService
    {
        private readonly IUserService _userService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductAttributeCommon _productAttributeService;
        private readonly IPictureService _pictureService;

        public ShoppingCartModelService(IUserService userService, IShoppingCartService shoppingCartService, IProductService productService, IUrlRecordService urlRecordService, IProductAttributeCommon productAttributeService, IPictureService pictureService)
        {
            _userService = userService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _productAttributeService = productAttributeService;
            _pictureService = pictureService;
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
            if (user is not null && user.HasShoppingCartItems)
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
                            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                            Quantity = sci.Quantity,
                            Price = product.Price.ToString("N0")
                        };

                        model.SubTotalValue += product.Price * sci.Quantity;

                        var result = new StringBuilder();

                        if (sci.Attributes is not null)
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

        public async Task<ShoppingCartModel> PrepareShoppingCartModelAsync()
        {
            var user = await _userService.GetCurrentUser();
            var model = new ShoppingCartModel
            {
                //ShowProductImages = _shoppingCartSettings.ShowProductImagesInMiniShoppingCart,
                //let's always display it
                DisplayShoppingCartButton = true,
            };
            model.Warnings = new List<string>();
            var warnings = new List<string>();
            //performance optimization (use "HasShoppingCartItems" property)
            if (user is not null && user.HasShoppingCartItems)
            {
                var carts = await _shoppingCartService.GetShoppingCartAsync();

                if (carts.Any())
                {
                    model.TotalProducts = carts.Sum(item => item.Quantity);

                    var cartProductIds = carts.Select(ci => ci.ProductId).ToArray();

                    //products. sort descending (recently added products)
                    foreach (var sci in carts
                                 .OrderByDescending(x => x.Id)
                                 .ToList())
                    {
                        string separator = "<br />";
                        var product = await _productService.GetByIdAsync(sci.ProductId);

                        var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
                        {
                            Id = sci.Id,
                            ProductId = sci.ProductId,
                            ProductName = product.Name,
                            ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                            Quantity = sci.Quantity,
                            Price = product.Price.ToString("N0"),
                            PriceValue = product.Price,
                            OrderMinimumQuantity = product.OrderMinimumQuantity,
                            OrderMaximumQuantity = product.OrderMaximumQuantity
                        };

                        var productPictures = await _productService.GetProductPicturesByProductIdAsync(product.Id);
                        if (productPictures?.Count > 0)
                        {
                            var picture = await _pictureService.GetPictureByIdAsync(productPictures.FirstOrDefault().PictureId);
                            cartItemModel.Picture = new Library.Model.ViewsModel.PictureModel()
                            {
                                ImageUrl = picture.UrlPath,
                                AltAttribute = picture.AltAttribute,
                                TitleAttribute = picture.TitleAttribute,
                            };
                        }

                        cartItemModel.SubTotalValue += product.Price * sci.Quantity;

                        var result = new StringBuilder();

                        if (sci.Attributes is not null)
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

                        cartItemModel.Warnings = sci.Warnings;

                        warnings.AddRange(sci.Warnings);

                        cartItemModel.SubTotal = cartItemModel.SubTotalValue.ToString("N0");

                        model.Items.Add(cartItemModel);
                    }
                    model.Warnings = warnings;
                }
            }
            return model;
        }

        public async Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(List<ShoppingCartItem> cart)
        {
            var model = new OrderTotalsModel();

            if (cart.Any())
            {
                decimal subTotal = 0;
                foreach(var item in cart)
                {
                    var product = await _productService.GetByIdAsync(item.ProductId);
                    subTotal += item.Quantity * product.Price;
                }
                model.SubTotal = subTotal.ToString();
            }

            return model;
        }
    }
}
