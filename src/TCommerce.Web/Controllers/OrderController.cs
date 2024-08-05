using Microsoft.AspNetCore.Mvc;
using MimeKit;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.AddressServices;
using TCommerce.Services.OrderServices;
using TCommerce.Services.PaymentServices;
using TCommerce.Services.ProductServices;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Web.Models;
using TCommerce.Web.Services.PrepareModelServices;
using static NodaTime.TimeZones.TzdbZone1970Location;

namespace TCommerce.Web.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IOrderModelService _orderModelService;

        public OrderController(IOrderService orderService, IUserService userService, IOrderModelService orderModelService)
        {
            _orderService = orderService;
            _userService = userService;
            _orderModelService = orderModelService;
        }

        public async Task<IActionResult> UserOrders()
        {
            var model = new UserOrderListModel();
            var customer = await _userService.GetCurrentUser();
            var orders = await _orderService.SearchOrdersAsync(new OrderParameters()
            {
                UserId = customer.Id,
            });
            foreach (var order in orders)
            {
                var orderModel = new UserOrderListModel.OrderDetailsModel
                {
                    Id = order.Id,
                    CreatedOn = order.CreatedOnUtc,
                    OrderStatusEnum = order.OrderStatus,
                    OrderStatus = order.OrderStatus.ToString(),
                    PaymentStatus = order.PaymentStatus.ToString(),
                };
                orderModel.OrderTotal = order.OrderTotal.ToString();

                model.Orders.Add(orderModel);
            }

            return View(model);
        }
        public virtual async Task<IActionResult> Details(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            var currentUser = await _userService.GetCurrentUser();

            if (order == null || order.Deleted || currentUser.Id != order.UserId)
                return Challenge();

            var model = await _orderModelService.PrepareOrderDetailsModelAsync(order);

            return View(model);
        }
        public virtual async Task<IActionResult> PrintOrderDetails(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            var currentUser = await _userService.GetCurrentUser();

            if (order == null || order.Deleted || currentUser.Id != order.UserId)
                return Challenge();

            var model = await _orderModelService.PrepareOrderDetailsModelAsync(order);
            model.PrintMode = true;

            return View("Details", model);
        }
    }
}
