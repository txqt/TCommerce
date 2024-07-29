using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Payments;
using TCommerce.Services.AddressServices;
using TCommerce.Services.DiscountServices;
using TCommerce.Services.OrderServices;
using TCommerce.Services.PictureServices;
using TCommerce.Services.PrepareModelServices.PrepareAdminModel;
using TCommerce.Services.ProductServices;
using TCommerce.Web.Areas.Admin.Models.Orders;
using TCommerce.Web.Extensions;

namespace TCommerce.Web.Areas.Admin.Services.PrepareAdminModel
{
    public interface IAdminOrderModelService
    {
        Task<OrderSearchModel> PrepareOrderSearchModelAsync(OrderSearchModel searchModel);
        Task<OrderModel> PrepareOrderModelAsync(OrderModel model, Order order);
    }
    public class AdminOrderModelService : IAdminOrderModelService
    {
        private readonly IBaseAdminModelService _baseAdminModelService;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        private readonly IDiscountService _discountService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IOrderProcessingService _orderProcessingService;

        public AdminOrderModelService(IBaseAdminModelService baseAdminModelService, IPaymentService paymentService, IUserService userService, IDiscountService discountService, IOrderService orderService, IProductService productService, IPictureService pictureService, IOrderProcessingService orderProcessingService)
        {
            _baseAdminModelService = baseAdminModelService;
            _paymentService = paymentService;
            _userService = userService;
            _discountService = discountService;
            _orderService = orderService;
            _productService = productService;
            _pictureService = pictureService;
            _orderProcessingService = orderProcessingService;
        }

        public async Task<OrderModel> PrepareOrderModelAsync(OrderModel model, Order order)
        {
            if (order != null)
            {
                //fill in model values from the entity
                model ??= new OrderModel
                {
                    Id = order.Id,
                    OrderStatusId = order.OrderStatusId,
                };

                var customer = await _userService.GetUserById(order.UserId);

                model.OrderGuid = order.OrderGuid;
                model.CustomerId = customer.Id;
                model.OrderStatus = order.OrderStatus.ToString();
                model.CustomerInfo = customer.Email;
                model.CreatedOn = customer.CreatedDate;

                //prepare order totals
                await PrepareOrderModelTotalsAsync(model, order);

                //prepare order items
                await PrepareOrderItemModelsAsync(model.Items, order);

                //prepare payment info
                await PrepareOrderModelPaymentInfoAsync(model, order);

                //prepare nested search model
                PrepareOrderNoteSearchModel(model.OrderNoteSearchModel, order);
            }

            return model;
        }

        protected virtual async Task PrepareOrderModelTotalsAsync(OrderModel model, Order order)
        {
            ArgumentNullException.ThrowIfNull(model);

            ArgumentNullException.ThrowIfNull(order);

            //subtotal
            model.OrderSubtotalInclTax = order.OrderSubtotalInclTax.ToString();
            model.OrderSubtotalExclTax = order.OrderSubtotalExclTax.ToString();
            model.OrderSubtotalInclTaxValue = order.OrderSubtotalInclTax;
            model.OrderSubtotalExclTaxValue = order.OrderSubtotalExclTax;

            //discount (applied to order subtotal)
            var orderSubtotalDiscountInclTaxStr =order.OrderSubTotalDiscountInclTax.ToString();
            var orderSubtotalDiscountExclTaxStr = order.OrderSubTotalDiscountExclTax.ToString();

            if (order.OrderSubTotalDiscountInclTax > decimal.Zero)
                model.OrderSubTotalDiscountInclTax = orderSubtotalDiscountInclTaxStr;
            if (order.OrderSubTotalDiscountExclTax > decimal.Zero)
                model.OrderSubTotalDiscountExclTax = orderSubtotalDiscountExclTaxStr;

            model.OrderSubTotalDiscountInclTaxValue = order.OrderSubTotalDiscountInclTax;
            model.OrderSubTotalDiscountExclTaxValue = order.OrderSubTotalDiscountExclTax;

            //shipping
            model.OrderShippingInclTax = order.ShippingFee + order.OrderTax;

            model.OrderShippingExclTax = order.ShippingFee;

            model.OrderShippingInclTaxValue = order.OrderShippingInclTax;
            model.OrderShippingExclTaxValue = order.OrderShippingExclTax;

            //tax
            model.Tax = order.OrderTax;

            //discount
            if (order.OrderDiscount > 0)
            {
                model.OrderTotalDiscount = order.OrderDiscount.ToString();
            }
            model.OrderTotalDiscountValue = order.OrderDiscount;

            //total
            model.OrderTotal = order.OrderTotal.ToString();
            model.OrderTotalValue = order.OrderTotal;

            //used discounts
            var duh = await _discountService.GetAllDiscountUsageHistoryAsync(orderId: order.Id);
            foreach (var d in duh)
            {
                var discount = await _discountService.GetByIdAsync(d.DiscountId);

                model.UsedDiscounts.Add(new OrderModel.UsedDiscountModel
                {
                    DiscountId = d.DiscountId,
                    DiscountName = discount.Name
                });
            }
        }

        protected virtual async Task PrepareOrderItemModelsAsync(IList<OrderItemModel> models, Order order)
        {
            ArgumentNullException.ThrowIfNull(models);

            ArgumentNullException.ThrowIfNull(order);

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetByIdAsync(orderItem.ProductId);

                //fill in model values from the entity
                var orderItemModel = new OrderItemModel
                {
                    Id = orderItem.Id,
                    ProductId = orderItem.ProductId,
                    ProductName = product.Name,
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeInfo
                };

                //fill in additional values (not existing in the entity)
                orderItemModel.Sku = (await _productService.GetByIdAsync(orderItem.ProductId)).Sku;

                //picture
                var productPictureMapping = await _productService.GetProductPicturesByProductIdAsync(orderItem.ProductId);
                if(productPictureMapping.Any())
                {
                    var orderItemPicture = await _pictureService.GetPictureByIdAsync(productPictureMapping.FirstOrDefault().PictureId);
                    orderItemModel.PictureThumbnailUrl = orderItemPicture.UrlPath;
                }

                orderItemModel.DiscountAmount = orderItem.DiscountAmount;
                orderItemModel.Price = orderItem.Price;

                models.Add(orderItemModel);
            }
        }

        protected virtual async Task PrepareOrderModelPaymentInfoAsync(OrderModel model, Order order)
        {
            ArgumentNullException.ThrowIfNull(model);

            ArgumentNullException.ThrowIfNull(order);

            //payment method info
            var pm = _paymentService.GetPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = pm != null ? pm.Name : order.PaymentMethodSystemName;
            model.PaymentStatus = TEnumExtensions.GetFormattedEnumName(order.PaymentStatus);

            //payment method buttons
            model.CanCancelOrder = _orderProcessingService.CanCancelOrder(order);

            model.CanCapture = _orderProcessingService.CanCapture(order);

            model.CanMarkOrderAsPaid = _orderProcessingService.CanMarkOrderAsPaid(order);

            model.CanRefund = _orderProcessingService.CanRefund(order);

            model.CanRefundOffline = _orderProcessingService.CanRefundOffline(order);

            model.CanPartiallyRefund = _orderProcessingService.CanPartiallyRefund(order, decimal.Zero);

            model.CanPartiallyRefundOffline = _orderProcessingService.CanPartiallyRefundOffline(order, decimal.Zero);

            model.CanVoid = _orderProcessingService.CanVoid(order);

            model.CanVoidOffline = _orderProcessingService.CanVoidOffline(order);
        }

        protected virtual OrderNoteSearchModel PrepareOrderNoteSearchModel(OrderNoteSearchModel searchModel, Order order)
        {
            ArgumentNullException.ThrowIfNull(searchModel);

            ArgumentNullException.ThrowIfNull(order);

            searchModel.OrderId = order.Id;

            return searchModel;
        }


        public async Task<OrderSearchModel> PrepareOrderSearchModelAsync(OrderSearchModel searchModel)
        {
            ArgumentNullException.ThrowIfNull(searchModel);

            await _baseAdminModelService.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
            if (searchModel.AvailableOrderStatuses.Any())
            {
                if (searchModel.OrderStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
                    var statusItems = searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                    foreach (var statusItem in statusItems)
                    {
                        statusItem.Selected = true;
                    }
                }
                else
                    searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
            }

            await _baseAdminModelService.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
            if (searchModel.AvailablePaymentStatuses.Any())
            {
                if (searchModel.PaymentStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                    var statusItems = searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList();
                    foreach (var statusItem in statusItems)
                    {
                        statusItem.Selected = true;
                    }
                }
                else
                    searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
            }

            //prepare available payment methods
            searchModel.AvailablePaymentMethods = (_paymentService.GetAllPaymentMethods()).Select(x => new SelectListItem()
            {
                Value = x.PaymentMethodSystemName,
                Text = x.Name,
                Selected = x.Selected,
            }).ToList();

            return searchModel;
        }
    }
}
