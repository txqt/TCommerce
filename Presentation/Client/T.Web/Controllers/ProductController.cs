using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Models;
using T.Web.Services.PictureServices;
using T.Web.Services.ProductService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace T.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductAttributeCommon _productAttributeService;
        private readonly IPictureService _pictureService;

        public ProductController(IProductService productService, IProductAttributeCommon productAttributeService, IPictureService pictureService)
        {
            _productService = productService;
            _productAttributeService = productAttributeService;
            _pictureService = pictureService;
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

                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attributeMappingItem.Id);

                if (attributeValues.Count > 0)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        var picture = await _pictureService.GetPictureByIdAsync(attributeValue.PictureId);
                        var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.PriceAdjustment <= 0 ? $"{attributeValue.Name}" : $"{attributeValue.Name} [+{attributeValue.PriceAdjustment.ToString("N0") + (attributeValue.PriceAdjustmentUsePercentage ? "%" : "")}]",
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                            CustomerEntersQty = attributeValue.CustomerEntersQty,
                            Quantity = attributeValue.Quantity,
                            
                        };
                        if(picture is not null)
                        {
                            valueModel.ImageSquaresPictureModel = new PictureModel()
                            {
                                ImageUrl = picture.UrlPath,
                                TitleAttribute = picture.TitleAttribute,
                                AltAttribute = picture.AltAttribute
                            };
                        }
                        attributeModel.Values.Add(valueModel);
                    }
                }
            }

            var productPictures = (await _productService.GetProductPicturesByProductIdAsync(product.Id));
            if(productPictures.Any() || productPictures.Count > 0)
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
            var productAttributes = await _productAttributeService.GetProductAttributesMappingByProductIdAsync(productId);
            var productAttributePrefix = "product_attribute_";
            var errors = new List<string>();
            var price = product.Price.ToString();
            var mainImage = string.Empty;
            foreach (var attribute in productAttributes)
            {
                var controlId = $"{productAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
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
                                        if(productAttributeValue.PictureId > 0)
                                        {
                                            mainImage = (await _pictureService.GetPictureByIdAsync(productAttributeValue.PictureId)).UrlPath;
                                        }
                                    }

                                }

                            }
                        }
                        break;
                    default:
                        break;
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
            var productAttributes = await _productAttributeService.GetProductAttributesMappingByProductIdAsync(productId);
            var productAttributePrefix = "product_attribute_";
            foreach(var item in form.Keys)
            {
                var value = form[item];
            }
            foreach (var attribute in productAttributes)
            {
                var controlId = $"{productAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
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

                                    }

                                }

                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return Json(new { });
        }
        // Phương thức để xóa phần tiền tố từ chuỗi
        static string RemovePrefix(string input, string prefix)
        {
            if (input.StartsWith(prefix))
            {
                return input.Substring(prefix.Length);
            }
            return input;
        }

        // Phương thức để lấy phần số từ phần tiền tố
        static int GetNumberFromPrefix(string input, string prefix)
        {
            string numberString = RemovePrefix(input, prefix);
            int number;
            if (int.TryParse(numberString, out number))
            {
                return number;
            }
            return -1; // Trả về -1 nếu không thể chuyển đổi thành số
        }
    }
}
