using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Component;
using T.Web.Models;
using T.Web.Services.PictureServices;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.ProductService;
using T.Web.Services.ShoppingCartServices;
using T.Web.Services.UrlRecordService;
using T.Web.Services.UserService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace T.Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductAttributeCommon _productAttributeService;
        private readonly IPictureService _pictureService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IShoppingCartModelService _sciModelService;
        private readonly ICategoryServiceCommon _categoryService;
        private readonly IUrlRecordService _urlRecordService;

        public ProductController(IProductService productService, IProductAttributeCommon productAttributeService, IPictureService pictureService, IShoppingCartService shoppingCartService, IMapper mapper, IUserService userService, IShoppingCartModelService sciModelService, ICategoryServiceCommon categoryService, IUrlRecordService urlRecordService)
        {
            _productService = productService;
            _productAttributeService = productAttributeService;
            _pictureService = pictureService;
            _shoppingCartService = shoppingCartService;
            _mapper = mapper;
            _userService = userService;
            _sciModelService = sciModelService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int id, int updatecartitemid = 0)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null || product.Deleted)
            {
                return InvokeHttp404();
            }

            var attributeMapping = await _productAttributeService.GetProductAttributesMappingByProductIdAsync(product.Id);

            var model = new ProductDetailsModel()
            {
                Id = product.Id,
                Title = product.Name + (product.Deleted ? " (Đã xóa)" : ""),
                Price = product.Price,
                OldPrice = product.OldPrice,
                ShortDescription = product.ShortDescription,
                Description = product.FullDescription,
            };

            foreach (var attributeMappingItem in attributeMapping)
            {
                var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(attributeMappingItem.ProductAttributeId);
                var attributeModel = new ProductDetailsModel.ProductAttributeModel
                {
                    Id = attributeMappingItem.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attributeMappingItem.ProductAttributeId,
                    Name = productAttribute.Name,
                    Description = productAttribute.Description,
                    TextPrompt = attributeMappingItem.TextPrompt,
                    IsRequired = attributeMappingItem.IsRequired,
                    DefaultValue = attributeMappingItem.DefaultValue,
                    AttributeControlTypeId = attributeMappingItem.AttributeControlTypeId,
                };
                model.ProductAttributes.Add(attributeModel);

                var attributeValues = await _productAttributeService.GetProductAttributeValuesByMappingIdAsync(attributeMappingItem.Id);

                if (attributeValues.Count > 0)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.PriceAdjustment <= 0 ? $"{attributeValue.Name}" : $"{attributeValue.Name} [+{attributeValue.PriceAdjustment.ToString("N0") + (attributeValue.PriceAdjustmentUsePercentage ? "%" : "")}]",
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity,

                        };
                        if (attributeValue.PictureId > 0)
                        {
                            var picture = await _pictureService.GetPictureByIdAsync(attributeValue.PictureId);
                            if (picture is not null)
                            {
                                valueModel.ImageSquaresPictureModel = new PictureModel()
                                {
                                    ImageUrl = picture.UrlPath,
                                    TitleAttribute = picture.TitleAttribute,
                                    AltAttribute = picture.AltAttribute
                                };
                            }
                        }
                        attributeModel.Values.Add(valueModel);
                    }
                }
            }

            var productPictures = (await _productService.GetProductPicturesByProductIdAsync(product.Id));
            if (productPictures.Any() || productPictures.Count > 0)
            {
                model.MainImage = new PictureModel
                {
                    Id = productPictures.FirstOrDefault().PictureId,
                    ImageUrl = (await _pictureService.GetPictureByIdAsync(productPictures.FirstOrDefault()?.PictureId ?? 0))?.UrlPath
                };

                var pictureTasks = productPictures.Select(async productPicture => new PictureModel
                {
                    Id = productPicture.PictureId,
                    ImageUrl = (await _pictureService.GetPictureByIdAsync(productPicture.PictureId)).UrlPath
                });

                model.ThumbImage = (await Task.WhenAll(pictureTasks)).ToList();
            }
            var addToCartModel = new ProductDetailsModel.AddToCartModel()
            {
                ProductId = product.Id,
                DisableBuyButton = product.DisableBuyButton,
                DisableWishlistButton = product.DisableWishlistButton,
                AvailableForPreOrder = product.AvailableForPreOrder,
                PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc,
                PreOrderAvailabilityStartDateTimeUserTime = product.PreOrderAvailabilityStartDateTimeUtc.ToString(),
                UpdateShoppingCartItemType = ShoppingCartType.ShoppingCart
            };

            if (updatecartitemid > 0)
            {
                var carts = (await _shoppingCartService.GetShoppingCartAsync());
                var cart = carts.FirstOrDefault(x => x.Id == updatecartitemid);
                ArgumentNullException.ThrowIfNull(cart);

                addToCartModel.UpdatedShoppingCartItemId = cart.Id;
                addToCartModel.UpdateShoppingCartItemType = cart.ShoppingCartType;

                var result = new StringBuilder();

                if (cart.Attributes is not null)
                {
                    foreach (var selectedAttribute in cart.Attributes)
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
                                    result.Append("<br/>");
                                }
                                result.Append(formattedAttribute);
                            }
                        }
                    }
                }

                model.ItemUpdateInfo = new ProductDetailsModel.CartItemUpdateInfo()
                {
                    ProductName = product.Name,
                    AttributeInfo = result.ToString()
                };
            }

            model.AddToCart = addToCartModel;

            var categoryIds = (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id)).Select(x => x.CategoryId).ToList();

            var categories = new List<ProductDetailsModel.CategoryOfProduct>();

            foreach (var categoryId in categoryIds)
            {
                var category = await _categoryService.GetCategoryByIdAsync(categoryId);
                var categorySeName = await _urlRecordService.GetSeNameAsync(category);

                categories.Add(new ProductDetailsModel.CategoryOfProduct()
                {
                    CategoryName = category.Name,
                    SeName = string.IsNullOrWhiteSpace(categorySeName) ? "javascript:void(0)" : categorySeName
                });
            }

            model.Categories = categories;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductDetailsAttributeChange(int productId, IFormCollection form)
        {
            var product = await _productService.GetByIdAsync(productId);
            var errors = new List<string>();
            var price = product.Price.ToString();
            var productPicture = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).FirstOrDefault();

            var mainImage = productPicture != null ? (await _pictureService.GetPictureByIdAsync(productPicture.PictureId)).UrlPath : string.Empty;
            

            var productAttributePrefix = "product_attribute_";
            foreach (var fromItem in form.Keys)
            {
                var mappingId = GetNumberFromPrefix(fromItem, productAttributePrefix);
                var attributesMapping = await _productAttributeService.GetProductAttributeMappingByIdAsync(mappingId);

                if (attributesMapping is not null)
                {
                    var controlId = $"{productAttributePrefix}{attributesMapping.Id}";

                    switch (attributesMapping.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                var ctrlAttributes = form[controlId];
                                if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                                {
                                    var selectedAttributeId = int.Parse(ctrlAttributes);
                                    if (selectedAttributeId > 0)
                                    {
                                        var productAttributeValue = await _productAttributeService.GetProductAttributeValuesByIdAsync(selectedAttributeId);

                                        ArgumentNullException.ThrowIfNull(productAttributeValue);

                                        if (productAttributeValue.PriceAdjustment > 0)
                                        {
                                            price = FinalPrice(decimal.Parse(price), productAttributeValue.PriceAdjustment, productAttributeValue.PriceAdjustmentUsePercentage);
                                        }
                                        if (productAttributeValue.PictureId > 0)
                                        {
                                            mainImage = (await _pictureService.GetPictureByIdAsync(productAttributeValue.PictureId)).UrlPath;
                                        }
                                    }

                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return Json(new
            {
                productId,
                price,
                mainImage,
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        private string FinalPrice(decimal productPrice, decimal priceAdjustment, bool priceAdjustmentUsePercentage = false)
        {
            if (priceAdjustmentUsePercentage)
            {
                return (productPrice += (productPrice *= priceAdjustment / 100)).ToString("N0");
            }

            return (productPrice + priceAdjustment).ToString("N0");
        }
    }
}
