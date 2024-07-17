using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.ProductServices;
using TCommerce.Web.Attribute;
using TCommerce.Web.Component;
using TCommerce.Web.PrepareModelServices;
using TCommerce.Core.Extensions;
using TCommerce.Core.Interface;

namespace TCommerce.Web.Controllers
{
    [CheckPermission()]
    public class ShoppingCartController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private static readonly char[] _separator = { ',' };
        private readonly IShoppingCartModelService _sciModelService;
        private readonly IDiscountService _discountService;
        private readonly IProductAttributeConverter _productAttributeConverter;

        public ShoppingCartController(IUserService userService, IProductService productService, IMapper mapper, IProductAttributeService productAttributeService, IShoppingCartService shoppingCartService, IShoppingCartModelService sciModelService, IDiscountService discountSerivce, IProductAttributeConverter productAttributeConverter)
        {
            _userService = userService;
            _productService = productService;
            _mapper = mapper;
            _productAttributeService = productAttributeService;
            _shoppingCartService = shoppingCartService;
            _sciModelService = sciModelService;
            _discountService = discountSerivce;
            _productAttributeConverter = productAttributeConverter;
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
            if (product is null)
            {
                return NotFound();
            }

            var model = new ShoppingCartItemModel
            {
                ProductId = product.Id,
                UserId = customer.Id,
                Attributes = new List<ShoppingCartItemModel.SelectedAttribute>(),
                ShoppingCartType = (ShoppingCartType)shoppingCartTypeId
            };

            int updateCartItemId = ExtractUpdateCartItemId(productId, form);
            if (updateCartItemId > 0)
            {
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, (ShoppingCartType)shoppingCartTypeId);
                var updateCartItem = cart.FirstOrDefault(x => x.Id == updateCartItemId);

                if (updateCartItem is not null && product.Id != updateCartItem.ProductId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This product does not match the passed shopping cart item identifier"
                    });
                }

                model.Id = updateCartItem?.Id ?? 0;
            }

            var attributes = await _productAttributeConverter.ParseProductAttributesAsync(form, product);

            var quantity = 1;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{product.Id}.EnteredQuantity", StringComparison.InvariantCultureIgnoreCase))
                {
                    _ = int.TryParse(form[formKey], out quantity);
                    break;
                }

            model.Quantity = quantity;

            string attributeJson = await _productAttributeConverter.ConvertToJsonAsync(attributes);

            var redirectToCart = false;

            if(updateCartItemId > 0)
            {
                await _shoppingCartService.UpdateCartItemAsync(customer, updateCartItemId, (ShoppingCartType)shoppingCartTypeId, product, attributeJson, quantity);
                redirectToCart = true;
            }
            else
            {
                await _shoppingCartService.AddToCartAsync(customer, (ShoppingCartType)shoppingCartTypeId, product, attributeJson, model.Quantity);
            }

            return await RefreshCartView("Success", redirectToCart);
        }

        private int ExtractUpdateCartItemId(int productId, IFormCollection form)
        {
            var updateCartItemKey = $"addtocart_{productId}.UpdatedShoppingCartItemId";
            if (form.TryGetValue(updateCartItemKey, out var value) && int.TryParse(value, out var updateCartItemId))
            {
                return updateCartItemId;
            }
            return 0;
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
        public virtual async Task<IActionResult> Cart()
        {
            var model = await _sciModelService.PrepareShoppingCartModelAsync();
            if (model.Warnings.Any())
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

            var model = new List<ShoppingCartItem>();
            foreach (var formKey in form.Keys)
            {
                var cartId = GetNumberFromPrefix(formKey, "item_quantity_");
                if (cartId > 0)
                {
                    var quantity = int.Parse(form[formKey]);

                    var carts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart);
                    if (!carts.Select(x => x.Id).Contains(cartId))
                    {
                        throw new ArgumentNullException();
                    }

                    var cartNeedUpdate = carts.FirstOrDefault(x => x.Id == cartId);
                    cartNeedUpdate.Quantity = quantity;
                    model.Add(cartNeedUpdate);
                }
            }
            foreach(var item in model)
            {
                var product = await _productService.GetByIdAsync(item.ProductId);
                await _shoppingCartService.UpdateCartItemAsync(customer, item.Id, ShoppingCartType.ShoppingCart, product, item.AttributeJson, item.Quantity);
            }
            return RedirectToAction(nameof(Cart));
        }

        //[HttpPost]
        //public virtual async Task<IActionResult> ApplyDiscountCoupon(string counponCode)
        //{
        //    if (counponCode != null)
        //        counponCode = counponCode.Trim();
        //}
        private async Task<JsonResult> RefreshCartView(string message, bool redirectToCart = false)
        {
            var updateMiniCartSectionHtml = await RenderViewComponentAsync(typeof(MiniCartDropDownViewComponent));
            var updateCartSectionHtml = await RenderViewAsync("Cart", ControllerContext, await _sciModelService.PrepareShoppingCartModelAsync(), true);
            return Json(new { success = true, updateminicartsectionhtml = updateMiniCartSectionHtml, updatecartsectionhtml = updateCartSectionHtml, message, redirectToCart });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApplyDiscountCode(string discountCode)
        {
            var discount = await _discountService.GetDiscountByCode(discountCode);
            var discountSessionKey = "Discount";
            var discounts = HttpContext.Session.Get<List<string>>(discountSessionKey) ?? new List<string>();

            if (discounts.Contains(discountCode))
            {
                return Json(new { success = false, message = "Đã áp dụng rồi." });
            }
            var result = await _discountService.CheckValidDiscountAsync(discount);

            if (result.Success)
            {
                

                var newDiscount = await _discountService.GetDiscountByCode(discountCode);

                if (newDiscount.IsCumulative || !discounts.Any())
                {
                    if (!discounts.Contains(discountCode))
                    {
                        discounts.Add(discountCode);
                        HttpContext.Session.Set(discountSessionKey, discounts);
                    }
                }
                else
                {
                    var canApplyNewDiscount = true;
                    foreach (var existingDiscountCode in discounts)
                    {
                        var existingDiscount = await _discountService.GetDiscountByCode(existingDiscountCode);
                        if (!existingDiscount.IsCumulative)
                        {
                            canApplyNewDiscount = false;
                            break;
                        }
                    }

                    if (canApplyNewDiscount)
                    {
                        if (!discounts.Contains(discountCode))
                        {
                            discounts.Add(discountCode);
                            HttpContext.Session.Set(discountSessionKey, discounts);
                        }
                    }
                }
                return await RefreshCartView("Success");
            }

            return Json(new { success = false, message = result.Message});
        }
    }
}
