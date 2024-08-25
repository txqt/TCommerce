using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Payments;
using TCommerce.Core.Models.SendMail;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.PrepareModelServices;
using TCommerce.Core.Extensions;
using TCommerce.Web.Models;
using TCommerce.Web.PrepareModelServices;
using TCommerce.Core.Models.Discounts;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Core.Utilities;

namespace TCommerce.Web.Controllers
{
    public class CheckoutController : BaseController
    {
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IShoppingCartModelService _shoppingCartModelService;
        private readonly IAccountModelService _accountModelService;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDiscountService _discountService;

        public CheckoutController(IAddressService addressService, IUserService userService,
                                  IShoppingCartModelService shoppingCartModelService, IMapper mapper,
                                  IAccountModelService accountModelService, IPaymentService paymentService,
                                  IOrderService orderService, IShoppingCartService shoppingCartService,
                                  IProductService productService, IPriceCalculationService priceCalculationService, IEmailSender emailSender, IConfiguration configuration, IHostEnvironment env, IProductAttributeService productAttributeService, IProductAttributeConverter productAttributeConverter, IHttpContextAccessor httpContextAccessor, IDiscountService discountService)
        {
            _addressService = addressService;
            _userService = userService;
            _shoppingCartModelService = shoppingCartModelService;
            _mapper = mapper;
            _accountModelService = accountModelService;
            _paymentService = paymentService;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _priceCalculationService = priceCalculationService;
            _emailSender = emailSender;
            _configuration = configuration;
            _env = env;
            _productAttributeService = productAttributeService;
            _productAttributeConverter = productAttributeConverter;
            _httpContextAccessor = httpContextAccessor;
            _discountService = discountService;
        }

        public async Task<IActionResult> Confirm()
        {
            var model = new CheckoutPaymentModel();
            var addresses = await _userService.GetOwnAddressesAsync();

            if (addresses != null && addresses.Any())
            {
                model.ShippingAddress = new CheckoutShippingAddressModel
                {
                    DefaultShippingAddress = addresses.FirstOrDefault(x => x.IsDefault),
                    ExistingAddresses = addresses.ToList(),
                };
            }

            model.Cart = await _shoppingCartModelService.PrepareShoppingCartModelAsync();
            model.ShippingAddress.NewShippingAddress = await _accountModelService.PrepareAddressModel(null, new AddressModel());
            model.PaymentMethods = _paymentService.GetAllPaymentMethods();

            var paymentMethodDefault = model.PaymentMethods.FirstOrDefault(x => x.Selected)?.PaymentMethodSystemName;

            var paymentSelected = HttpContext.Session.GetString("PaymentMethodSessionKey");

            if (string.IsNullOrEmpty(paymentSelected))
            {
                paymentSelected = paymentMethodDefault;
                HttpContext.Session.SetString("PaymentMethodSessionKey", paymentSelected);
            }


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder()
        {
            var user = await _userService.GetCurrentUser();
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var defaultAddress = (await _userService.GetOwnAddressesAsync())?.FirstOrDefault(x => x.IsDefault);
            if (defaultAddress == null)
            {
                return Json(new { success = false, message = "Default address not found" });
            }

            var paymentMethodSession = HttpContext.Session.GetString("PaymentMethodSessionKey");
            if (paymentMethodSession == null)
            {
                return Json(new { success = false, message = "Payment method not selected" });
            }

            var paymentMethod = _paymentService.GetPaymentMethodBySystemName(paymentMethodSession);
            if (paymentMethod == null)
            {
                return Json(new { success = false, message = "Payment method not found" });
            }

            var carts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart);
            if (carts == null || !carts.Any())
            {
                return Json(new { success = false, message = "Shopping cart is empty" });
            }

            var order = await CreateOrderAsync(user, defaultAddress, paymentMethod, carts);
            await _orderService.CreateOrderAsync(order);

            var discountSessionKey = "Discount";
            var session = _httpContextAccessor.HttpContext.Session;
            var discounts = session.Get<List<string>>(discountSessionKey) ?? new List<string>();
            if (discounts is not null)
            {
                foreach (var d in discounts)
                {
                    var discount = await _discountService.GetDiscountByCode(d);
                    if (discount is not null)
                    {
                        await _discountService.CreateDiscountUsageHistoryAsync(new DiscountUsageHistory()
                        {
                            DiscountId = discount.Id,
                            OrderId = order.Id,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                    }
                }
            }
            session.Remove(discountSessionKey);

            var orderItems = await CreateOrderItemsAsync(order.Id, carts);
            await _orderService.BulkCreateOrderItemsAsync(orderItems);

            await SendMailConfirmedOrder(order);

            await _shoppingCartService.ClearShoppingCartAsync(user);

            var paymentResult = await _paymentService.ProcessPaymentAsync(order, paymentMethod);

            if (!paymentResult.Success)
            {
                return Json(new { success = false, message = paymentResult.Message });
            }

            if (!string.IsNullOrEmpty(paymentResult.Data))
            {
                return Json(new { success = true, message = "Cảm ơn đã mua hàng, vui lòng kiểm tra email của bạn", redirectUrl = paymentResult.Data });
            }
            var redirectUrl = Url.Action("Thankyou", "Checkout", new { message = "Order confirmed successfully!" });
            return Json(new { success = true, message = "Cảm ơn đã mua hàng, vui lòng kiểm tra email của bạn", redirectUrl });
        }
        public IActionResult Thankyou(string message)
        {
            return View("Thankyou", message);
        }

        private async Task<Order> CreateOrderAsync(User user, AddressInfoModel defaultAddress, PaymentMethod paymentMethod, List<ShoppingCartItem> carts)
        {
            var order = InitializeOrder(user, defaultAddress, paymentMethod);

            decimal orderSubtotal = await _priceCalculationService.CalculateSubTotalAsync(carts);

            var subTotalDiscountAmount = await _priceCalculationService.CalculateSubtotalDiscountAmountAsync(carts, orderSubtotal);

            var orderSubtotalAfterDiscount = orderSubtotal - subTotalDiscountAmount;

            var taxRate = 0;  //%
            var taxAmount = _priceCalculationService.CalculateTax(orderSubtotalAfterDiscount, taxRate);

            var shippingFee = _priceCalculationService.CalculateShippingFee(carts);

            var orderTotal = orderSubtotalAfterDiscount + taxAmount + shippingFee;

            var orderTotalDiscountAmount = await _priceCalculationService.CalculateOrderTotalDiscount(orderTotal);

            // Lưu các giá trị vào đơn hàng
            order.OrderSubtotalInclTax = orderSubtotal + taxAmount;
            order.OrderSubtotalExclTax = orderSubtotal;

            order.OrderSubTotalDiscountInclTax = orderSubtotal - subTotalDiscountAmount + taxAmount;
            order.OrderSubTotalDiscountExclTax = orderSubtotal - subTotalDiscountAmount;

            order.OrderDiscount = orderTotalDiscountAmount;
            order.OrderTotal = orderTotal - orderTotalDiscountAmount;

            order.TaxRates = $"{taxRate}%";
            order.OrderTax = taxAmount;
            order.ShippingFee = shippingFee.ToString();

            order.OrderShippingInclTax = shippingFee + taxAmount;
            order.OrderShippingExclTax = shippingFee;


            return order;
        }

        private Order InitializeOrder(User user, AddressInfoModel defaultAddress, PaymentMethod paymentMethod)
        {
            return new Order
            {
                OrderGuid = Guid.NewGuid(),
                UserId = user.Id,
                ShippingAddressId = defaultAddress.Id,
                OrderStatus = OrderStatus.Pending,
                CreatedOnUtc = DateTime.UtcNow,
                PaymentMethodSystemName = paymentMethod.PaymentMethodSystemName,
                PaymentStatus = PaymentStatus.Pending,
                UserIp = AppUtilities.GetIpAddress(_httpContextAccessor)
            };
        }

        private async Task<List<OrderItem>> CreateOrderItemsAsync(int orderId, IList<ShoppingCartItem> carts)
        {
            var orderItems = new List<OrderItem>();
            foreach (var cart in carts)
            {
                var product = await _productService.GetByIdAsync(cart.ProductId);
                string separator = "<br />";

                var attributeInfo = new StringBuilder();

                if (!string.IsNullOrEmpty(cart.AttributeJson))
                {
                    foreach (var selectedAttribute in _productAttributeConverter.ConvertToObject(cart.AttributeJson))
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
                                if (attributeInfo.Length > 0)
                                {
                                    attributeInfo.Append(separator);
                                }
                                attributeInfo.Append(formattedAttribute);
                            }
                        }
                    }
                }
            

            orderItems.Add(new OrderItem
            {
                OrderId = orderId,
                ProductId = product.Id,
                Quantity = cart.Quantity,
                ItemWeight = product.Weight,
                Price = await _priceCalculationService.CalculateAdjustedPriceAsync(product, cart),
                OriginalProductCost = product.Price,
                AttributeInfo = attributeInfo.ToString(),
                AttributeJson = cart.AttributeJson
            });
        }
            return orderItems;
        }

    [HttpPost]
    public async Task<IActionResult> SelectAddress(int id)
    {
        var address = await _addressService.GetAddressByIdAsync(id);
        if (address == null)
        {
            return Json(new { success = false });
        }

        var addresses = await _userService.GetOwnAddressesAsync();
        if (addresses == null || !addresses.Any(x => x.Id != address.Id))
        {
            return Json(new { success = false });
        }

        address.IsDefault = true;
        var result = await _userService.UpdateUserAddressAsync(address);
        SetStatusMessage(result.Success ? "Success" : "Failed");

        ViewBag.RefreshPage = true;
        return Json(new { success = result.Success });
    }

    [HttpPost]
    public async Task<IActionResult> NewDefaultAddress(AddressModel model)
    {
        if (!ModelState.IsValid)
        {
            model = await _accountModelService.PrepareAddressModel(null, model);
            return View(model);
        }

        var address = _mapper.Map<Address>(model);
        address.CreatedOnUtc = DateTime.UtcNow;
        address.IsDefault = true;

        var result = await _userService.CreateUserAddressAsync(address);
        SetStatusMessage(result.Success ? "Success" : "Failed");

        return RedirectToAction(nameof(Confirm));
    }

    [HttpPost]
    public IActionResult SavePaymentMethod(string payment_method)
    {
        if (string.IsNullOrEmpty(payment_method))
        {
            return Json(new { success = false, message = "Payment method not selected." });
        }

        HttpContext.Session.SetString("PaymentMethodSessionKey", payment_method);

        return Json(new { success = true });
    }

    private async Task SendMailConfirmedOrder(Order order)
    {
        var user = await _userService.GetCurrentUser();
        var carts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart);
        var path = Path.Combine(_env.ContentRootPath, "wwwroot/ordersuccesfullymail.html");


        string bodyBuilder = "";

        List<string> products = new List<string>();

        foreach (var cart in carts)
        {
            var result = new StringBuilder();
            var product = await _productService.GetByIdAsync(cart.ProductId);
            if (!string.IsNullOrEmpty(cart.AttributeJson))
            {
                foreach (var selectedAttribute in _productAttributeConverter.ConvertToObject(cart.AttributeJson))
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
                                result.Append("<br />");
                            }
                            result.Append(formattedAttribute);
                        }
                    }
                }
            }

            var subTotal = await _priceCalculationService.CalculateAdjustedPriceAsync(product, cart);

            products.Add("<tr>\r\n" +
            "<td align=\"left\" style=\"padding:3px 9px\" valign=\"top\">\r\n" +
            $"<span>{product.Name + "<br>\r\n" + result.ToString()}</span>\r\n" +
            "<br>\r\n" +
            "</td>\r\n" +
            "<td align=\"left\" style=\"padding:3px 9px\" valign=\"top\">\r\n" +
            $"<span>{product.Price}đ</span>\r\n" +
            "</td>\r\n                                            " +
                    $"<td align=\"left\" style=\"padding:3px 9px\" valign=\"top\">{cart.Quantity}</td>\r\n" +
                    "<td align=\"left\" style=\"padding:3px 9px\" valign=\"top\">\r\n" +
                    "<span>0đ</span>\r\n" +
                    "</td>\r\n" +
                    "<td align=\"right\" style=\"padding:3px 9px\" valign=\"top\">\r\n" +
            $"<span>{subTotal}đ</span>\r\n" +
            "</td>\r\n" +
                    "</tr>");

            result.Clear();
        }

        using (StreamReader SourceReader = System.IO.File.OpenText(path))
        {
            bodyBuilder = SourceReader.ReadToEnd();
        }

        bodyBuilder = bodyBuilder.Replace("[oder-id]", order.OrderGuid.ToString().ToUpper());
        bodyBuilder = bodyBuilder.Replace("[user-first-name]", user.FirstName);
        bodyBuilder = bodyBuilder.Replace("[order-date]", order.CreatedOnUtc.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
        bodyBuilder = bodyBuilder.Replace("[user-full-name]", user.LastName + " " + user.FirstName);
        bodyBuilder = bodyBuilder.Replace("[user-mail]", user.Email);
        bodyBuilder = bodyBuilder.Replace("[user-list-order]", _configuration["ClientUrl"] + "/user/list-order");
        bodyBuilder = bodyBuilder.Replace("[user-phone-number]", user.PhoneNumber);
        bodyBuilder = bodyBuilder.Replace("[payment-method]", order.PaymentMethodSystemName);
        bodyBuilder = bodyBuilder.Replace("[total-price]", order.OrderTotal.ToString());
        bodyBuilder = bodyBuilder.Replace("[all-products]", String.Join("", products.ToArray()));
        bodyBuilder = bodyBuilder.Replace("[order-subtotal]", order.OrderSubtotalExclTax.ToString());
        bodyBuilder = bodyBuilder.Replace("[order-subtotal-have-discount]", order.OrderSubTotalDiscountExclTax.ToString());
        bodyBuilder = bodyBuilder.Replace("[order-tax]", order.OrderTax.ToString());
        bodyBuilder = bodyBuilder.Replace("[user-address]", (await _userService.GetOwnAddressesAsync()).Where(x => x.IsDefault).FirstOrDefault().AddressFull);

        EmailDto emailDto = new EmailDto
        {
            Subject = $"[TCommerce] Xác nhận đơn hàng #{order.OrderGuid.ToString().ToUpper()}",
            Body = bodyBuilder,
            To = user.Email
        };
        try
        {
            await _emailSender.SendEmailAsync(emailDto);
        }
        catch
        {

        }
    }
}
}
