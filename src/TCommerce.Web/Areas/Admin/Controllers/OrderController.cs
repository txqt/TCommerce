using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Security;
using TCommerce.Web.Areas.Admin.Models.Orders;
using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;
using TCommerce.Web.Attribute;
using TCommerce.Web.Extensions;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/order/[action]")]
    [CheckPermission(PermissionSystemName.ManageOrders)]
    public class OrderController : BaseAdminController
    {
        private readonly IAdminOrderModelService _orderModelService;
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;

        public OrderController(IAdminOrderModelService orderModelService, IOrderService orderService, IAddressService addressService)
        {
            _orderModelService = orderModelService;
            _orderService = orderService;
            _addressService = addressService;
        }

        public virtual async Task<IActionResult> Index(List<int> orderStatuses = null, List<int> paymentStatuses = null)
        {
            //prepare model
            var model = await _orderModelService.PrepareOrderSearchModelAsync(new OrderSearchModel
            {
                OrderStatusIds = orderStatuses,
                PaymentStatusIds = paymentStatuses
            });

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> OrderList(OrderSearchModel searchModel)
        {
            var orderParameters = ParseQueryStringParameters<OrderParameters>();

            orderParameters.osIds = searchModel.OrderStatusIds ;
            orderParameters.psIds = searchModel.PaymentStatusIds;
            orderParameters.PaymentMethodSystemName = searchModel.PaymentMethodSystemName;
            orderParameters.ProductId = searchModel.ProductId;
            orderParameters.OrderNotes = searchModel.OrderNotes;

            var response = await _orderService.SearchOrdersAsync(orderParameters);

            var pagingResponse = new PagingResponse<Order>
            {
                Items = response,
                MetaData = response.MetaData
            };

            var orderModelList = new List<OrderModel>();

            if(pagingResponse.Items is not null)
            {
                foreach(var order in pagingResponse.Items)
                {
                    var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId.Value);

                    var orderModel = new OrderModel
                    {
                        Id = order.Id,
                        OrderStatusId = order.OrderStatusId,
                        PaymentStatusId = order.PaymentStatusId,
                        CustomerEmail = shippingAddress.Email,
                        CustomerFullName = $"{shippingAddress.FirstName} {shippingAddress.LastName}",
                        CustomerId = order.UserId,
                        OrderStatus = order.OrderStatus.ToString()
                    };

                    orderModelList.Add(orderModel);
                }
            }

            var model = ToDatatableReponse(pagingResponse.MetaData.TotalCount, pagingResponse.MetaData.TotalCount, orderModelList);

            return this.JsonWithPascalCase(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.Deleted)
                return RedirectToAction("Index");

            //prepare model
            var model = await _orderModelService.PrepareOrderModelAsync(null, order);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderStatus")]
        public virtual async Task<IActionResult> ChangeOrderStatus(int id, OrderModel model)
        {

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            try
            {
                var prevOrderStatus = order.OrderStatus;

                order.OrderStatusId = model.OrderStatusId;
                await _orderService.UpdateOrderAsync(order);

                //add a note
                //await _orderService.InsertOrderNoteAsync(new OrderNote
                //{
                //    OrderId = order.Id,
                //    Note = $"Order status has been edited. New status: {await _localizationService.GetLocalizedEnumAsync(order.OrderStatus)}",
                //    DisplayToCustomer = false,
                //    CreatedOnUtc = DateTime.UtcNow
                //});

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                model = await _orderModelService.PrepareOrderModelAsync(model, order);

                return View(model);
            }
        }
    }
}
