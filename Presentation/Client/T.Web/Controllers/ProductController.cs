using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Globalization;
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
        private static readonly char[] _separator = { ',' };
        private readonly IShoppingCartModelService _sciModelService;

        public ProductController(IProductService productService, IProductAttributeCommon productAttributeService, IPictureService pictureService, IShoppingCartService shoppingCartService, IMapper mapper, IUserService userService, IShoppingCartModelService sciModelService)
        {
            _productService = productService;
            _productAttributeService = productAttributeService;
            _pictureService = pictureService;
            _shoppingCartService = shoppingCartService;
            _mapper = mapper;
            _userService = userService;
            _sciModelService = sciModelService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            var attributeMapping = await _productAttributeService.GetProductAttributesMappingByProductIdAsync(product.Id);

            var model = new ProductDetailsModel()
            {
                Id = product.Id,
                Title = product.Name,
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

            model.AddToCart = new ProductDetailsModel.AddToCartModel()
            {
                ProductId = product.Id,
                DisableBuyButton = product.DisableBuyButton,
                DisableWishlistButton = product.DisableWishlistButton,
                AvailableForPreOrder = product.AvailableForPreOrder,
                PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc,
                PreOrderAvailabilityStartDateTimeUserTime = product.PreOrderAvailabilityStartDateTimeUtc.ToString(),

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductDetailsAttributeChange(int productId, IFormCollection form)
        {
            var product = await _productService.GetByIdAsync(productId);
            var errors = new List<string>();
            var price = product.Price.ToString();
            var mainImage = (await _pictureService.GetPictureByIdAsync(product.Id)).UrlPath;

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
                                        if (productAttributeValue is not null)
                                        {
                                            if (productAttributeValue.PriceAdjustment > 0)
                                            {
                                                price = FinalPrice(decimal.Parse(price), productAttributeValue.PriceAdjustment, productAttributeValue.PriceAdjustmentUsePercentage);
                                            }
                                            if (productAttributeValue.PictureId > 0)
                                            {
                                                mainImage = (await _pictureService.GetPictureByIdAsync(productAttributeValue.PictureId)).UrlPath;
                                            }
                                        }
                                        else
                                        {
                                            throw new ArgumentNullException("Something went wrong.");
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

        [HttpPost]
        public virtual async Task<IActionResult> AddProductToCartDetails(int productId, int shoppingCartTypeId, IFormCollection form)
        {
            var customer = await _userService.GetCurrentUser();

            if (customer is null)
            {
                return NotFound();
            }
            var product = await _productService.GetByIdAsync(productId);
            var productAttributePrefix = "product_attribute_";

            var model = new ShoppingCartItemModel();
            model.ProductId = product.Id;
            model.ProductModel = _mapper.Map<ProductModel>(product);
            model.UserId = customer.Id;
            model.Attributes = new List<ShoppingCartItemModel.SelectedAttribute>();
            var quantity = 1;

            foreach (var formKey in form.Keys)
            {
                var mappingId = GetNumberFromPrefix(formKey, productAttributePrefix);
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
                                        if (productAttributeValue is not null)
                                        {
                                            model.Attributes.Add(new ShoppingCartItemModel.SelectedAttribute()
                                            {
                                                ProductAttributeMappingId = attributesMapping.Id,
                                                ProductAttributeValueIds = new List<int> { productAttributeValue.Id }
                                            });
                                        }
                                        else
                                        {
                                            throw new ArgumentNullException("Something went wrong.");
                                        }
                                    }

                                }
                            }
                            break;
                        case AttributeControlType.Checkboxes:
                            {
                                var cblAttributes = form[controlId];
                                var selectedAttribute = new ShoppingCartItemModel.SelectedAttribute()
                                {
                                    ProductAttributeMappingId = attributesMapping.Id,
                                    ProductAttributeValueIds = new List<int>()
                                };

                                if (!StringValues.IsNullOrEmpty(cblAttributes))
                                {
                                    foreach (var item in cblAttributes.ToString().Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        var selectedAttributeId = int.Parse(item);
                                        if (selectedAttributeId > 0)
                                        {
                                            selectedAttribute.ProductAttributeValueIds.Add(selectedAttributeId);
                                        }
                                    }
                                }

                                if (selectedAttribute.ProductAttributeValueIds.Any())
                                {
                                    model.Attributes.Add(selectedAttribute);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (formKey.Equals($"addtocart_{product.Id}.EnteredQuantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out quantity);
                    model.Quantity = quantity;
                    break;
                }
            }

            var createResult = await _shoppingCartService.CreateAsync(model);

            if (createResult.Success)
            {
                var updateMiniCartSectionHtml = await RenderViewComponent(typeof(MiniCartDropDownViewComponent));

                return Json(new { success = true, updateminicartsectionhtml = updateMiniCartSectionHtml });
            }

            return Json(new
            {
                success = false,
                errors = createResult.Message.Split(",").ToArray()
            });
        }

        static int GetNumberFromPrefix(string input, string prefix)
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
