using System.Text;
using TCommerce.Core.Interface;
using TCommerce.Core.Models;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.PriceCalulationServices;
using TCommerce.Services.ProductServices;
using TCommerce.Web.Extensions;
using TCommerce.Web.Models;

namespace TCommerce.Web.PrepareModelServices
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
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IPictureService _pictureService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDiscountService _discountService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPriceCalculationService _priceCalculationService;

        public ShoppingCartModelService(IUserService userService, IShoppingCartService shoppingCartService, IProductService productService, IUrlRecordService urlRecordService, IProductAttributeService productAttributeService, IPictureService pictureService, IHttpContextAccessor httpContextAccessor, IDiscountService discountService, ICategoryService categoryService, IManufacturerService manufacturerService, IProductAttributeConverter productAttributeConverter, IPriceCalculationService priceCalculationService)
        {
            _userService = userService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _productAttributeService = productAttributeService;
            _pictureService = pictureService;
            _httpContextAccessor = httpContextAccessor;
            _discountService = discountService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _productAttributeConverter = productAttributeConverter;
            _priceCalculationService = priceCalculationService;
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
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart);

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

                        model.SubTotalValue += await _priceCalculationService.CalculateAdjustedPriceAsync(product, sci);

                        var result = new StringBuilder();

                        if (sci.AttributeJson is not null)
                        {
                            foreach (var selectedAttribute in _productAttributeConverter.ConvertToObject(sci.AttributeJson))
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
                DisplayShoppingCartButton = true,
            };
            model.Warnings = new List<string>();
            var warnings = new List<string>();

            if (user is not null && user.HasShoppingCartItems)
            {
                var carts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart);

                if (carts.Any())
                {
                    model.TotalProducts = carts.Sum(item => item.Quantity);
                    var cartProductIds = carts.Select(ci => ci.ProductId).ToArray();

                    foreach (var sci in carts.OrderByDescending(x => x.Id).ToList())
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
                            cartItemModel.Picture = new PictureModel
                            {
                                ImageUrl = picture.UrlPath,
                                AltAttribute = picture.AltAttribute,
                                TitleAttribute = picture.TitleAttribute,
                            };
                        }

                        var result = new StringBuilder();

                        if (sci.AttributeJson is not null)
                        {
                            foreach (var selectedAttribute in _productAttributeConverter.ConvertToObject(sci.AttributeJson))
                            {
                                var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(selectedAttribute.ProductAttributeMappingId);
                                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId);

                                var attributeName = productAttribute.Name;

                                foreach (var attributeValueId in selectedAttribute.ProductAttributeValueIds)
                                {
                                    var attributeValue = await _productAttributeService.GetProductAttributeValuesByIdAsync(attributeValueId);

                                    cartItemModel.SubTotalValue += await _priceCalculationService.CalculateAdjustedPriceAsync(product, sci);

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
                        cartItemModel.Warnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(user, ShoppingCartType.ShoppingCart, product, sci.AttributeJson, sci.Quantity);
                        warnings.AddRange(cartItemModel.Warnings);

                        cartItemModel.SubTotal = cartItemModel.SubTotalValue.ToString("N0");

                        model.Items.Add(cartItemModel);
                    }
                    model.Warnings = warnings;
                }
            }
            return model;
        }
        public async Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(List<ShoppingCartItem> carts)
        {
            var model = new OrderTotalsModel();

            if (carts.Any())
            {
                decimal orderSubtotal = await _priceCalculationService.CalculateSubTotalAsync(carts);

                var subTotalDiscountAmount = await _priceCalculationService.CalculateSubtotalDiscountAmountAsync(carts, orderSubtotal);

                var orderSubtotalAfterDiscount = orderSubtotal - subTotalDiscountAmount;

                var taxRate = 10;
                var taxAmount = _priceCalculationService.CalculateTax(orderSubtotalAfterDiscount, taxRate);

                var shippingFee = _priceCalculationService.CalculateShippingFee(carts);

                var orderTotal = orderSubtotalAfterDiscount + taxAmount + shippingFee;

                var orderTotalDiscountAmount = await _priceCalculationService.CalculateOrderTotalDiscount(orderTotal);

                model.SubTotal = orderSubtotal.ToString();
                model.SubTotalDiscount = orderSubtotalAfterDiscount.ToString();

                model.OrderTotal = orderTotal.ToString();
                model.Tax = taxAmount.ToString();
                model.ShippingFee = shippingFee.ToString();
            }

            return model;
        }
    }
}
