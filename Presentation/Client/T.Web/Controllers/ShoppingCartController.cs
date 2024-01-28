using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Orders;
using T.Library.Model.Response;
using T.Library.Model.ViewsModel;
using T.Web.Attribute;
using T.Web.Component;
using T.Web.Models;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.ProductService;
using T.Web.Services.ShoppingCartServices;
using T.Web.Services.UserService;

namespace T.Web.Controllers
{
    [CheckPermission()]
    public class ShoppingCartController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IProductAttributeCommon _productAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private static readonly char[] _separator = { ',' };
        private readonly IShoppingCartModelService _sciModelService;

        public ShoppingCartController(IUserService userService, IProductService productService, IMapper mapper, IProductAttributeCommon productAttributeService, IShoppingCartService shoppingCartService, IShoppingCartModelService sciModelService)
        {
            _userService = userService;
            _productService = productService;
            _mapper = mapper;
            _productAttributeService = productAttributeService;
            _shoppingCartService = shoppingCartService;
            _sciModelService = sciModelService;
        }

        public IActionResult Index()
        {
            return View();
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
            model.ShoppingCartType = (ShoppingCartType)shoppingCartTypeId;
            var updatecartitemid = 0;
            ShoppingCartItemModel updatecartitem = null;
            foreach (var formKey in form.Keys)
            {
                var updatecartitemkey = $"addtocart_{productId}.UpdatedShoppingCartItemId";
                if (formKey.Equals($"{updatecartitemkey}", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out updatecartitemid);
                }
                else
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
                        _ = int.TryParse(form[formKey], out int quantity);
                        model.Quantity = quantity;
                        break;
                    }
                }
                if (updatecartitemid > 0)
                {
                    //search with the same cart type as specified
                    var cart = await _shoppingCartService.GetShoppingCartAsync();

                    updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                    if (updatecartitem != null && product.Id != updatecartitem.ProductId)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "This product does not match a passed shopping cart item identifier"
                        });
                    }
                    else
                    {
                        model.Id = updatecartitem.Id;
                    }
                }
                
            }

            var result = new ServiceResponse<bool>();

            if(updatecartitem is null)
            {
                result = await _shoppingCartService.CreateAsync(model);
            }
            else
            {
                result = await _shoppingCartService.UpdateAsync(model);
            }

            if (result.Success)
            {
                return await RefreshCartView("Success");
            }

            var resultMessage = result.Message.Split(",").ToArray();
            SetStatusMessage(string.Join("<br/>", resultMessage));

            return Json(new
            {
                success = false,
                errors = resultMessage
            });
        }
        [HttpPost]
        public virtual async Task<IActionResult> DeleteShoppingCartItem(int id)
        {
            var deleteResult = await _shoppingCartService.DeleteAsync(id);

            if (deleteResult.Success)
            {
                return await RefreshCartView("Xóa thành công");
            }

            return Json(new
            {
                success = false,
                errors = deleteResult.Message.Split(",").ToArray()
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

        public virtual async Task<IActionResult> Cart()
        {
            var model = await _sciModelService.PrepareShoppingCartModelAsync();
            if(model.Warnings.Any())
            {
                SetStatusMessage(string.Join("<br/>", model.Warnings));
            }
            return View(model);
        }
        public virtual async Task<IActionResult> UpdateCart(IFormCollection form)
        {
            var customer = await _userService.GetCurrentUser();

            if (customer is null)
            {
                return NotFound();
            }

            var model = new List<ShoppingCartItemModel>();
            foreach (var formKey in form.Keys)
            {
                var cartId = GetNumberFromPrefix(formKey, "item_quantity_");
                if (cartId > 0)
                {
                    var quantity = int.Parse(form[formKey]);

                    var carts = await _shoppingCartService.GetShoppingCartAsync();
                    if (!carts.Select(x => x.Id).Contains(cartId))
                    {
                        throw new ArgumentNullException();
                    }

                    var cartNeedUpdate = carts.FirstOrDefault(x => x.Id == cartId);
                    cartNeedUpdate.Quantity = quantity;
                    model.Add(cartNeedUpdate);
                }
            }
            await _shoppingCartService.UpdateBatchAsync(model);
            return RedirectToAction(nameof(Cart));
        }

        private async Task<JsonResult> RefreshCartView(string message)
        {
            var updateMiniCartSectionHtml = await RenderViewComponentAsync(typeof(MiniCartDropDownViewComponent));
            var updateCartSectionHtml = await RenderViewAsync("Cart", ControllerContext, await _sciModelService.PrepareShoppingCartModelAsync(), true);
            return Json(new { success = true, updateminicartsectionhtml = updateMiniCartSectionHtml, updatecartsectionhtml = updateCartSectionHtml, message });
        }
    }
}
