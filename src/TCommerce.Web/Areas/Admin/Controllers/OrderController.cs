using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TCommerce.Core.Extensions;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Security;
using TCommerce.Services.OrderServices;
using TCommerce.Services.ProductServices;
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
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IProductService _productService;
        private readonly OrderSettings _orderSettings;

        public OrderController(IAdminOrderModelService orderModelService, IOrderService orderService, IAddressService addressService, IOrderProcessingService orderProcessingService, IProductService productService, OrderSettings orderSettings)
        {
            _orderModelService = orderModelService;
            _orderService = orderService;
            _addressService = addressService;
            _orderProcessingService = orderProcessingService;
            _productService = productService;
            _orderSettings = orderSettings;
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
                await _orderService.CreateOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = $"Order status has been edited. New status: {order.OrderStatus.ToString()}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                model = await _orderModelService.PrepareOrderModelAsync(model, order);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public virtual async Task<IActionResult> CancelOrder(int id)
        {
            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            try
            {
                await _orderProcessingService.CancelOrderAsync(order, true);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelService.PrepareOrderModelAsync(null, order);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderTotals")]
        public virtual async Task<IActionResult> EditOrderTotals(int id, OrderModel model)
        {
            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            order.OrderSubtotalInclTax = model.OrderSubtotalInclTaxValue;
            order.OrderSubtotalExclTax = model.OrderSubtotalExclTaxValue;
            order.OrderSubTotalDiscountInclTax = model.OrderSubTotalDiscountInclTaxValue;
            order.OrderSubTotalDiscountExclTax = model.OrderSubTotalDiscountExclTaxValue;
            order.OrderShippingInclTax = model.OrderShippingInclTaxValue;
            order.OrderShippingExclTax = model.OrderShippingExclTaxValue;
            order.TaxRates = model.TaxRatesValue;
            order.OrderTax = model.TaxValue;
            order.OrderDiscount = model.OrderTotalDiscountValue;
            order.OrderTotal = model.OrderTotalValue;
            await _orderService.UpdateOrderAsync(order);

            //add a note
            await _orderService.CreateOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order totals have been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            return RedirectToAction("Edit", new { id = order.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markorderaspaid")]
        public virtual async Task<IActionResult> MarkOrderAsPaid(int id)
        {
            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            try
            {
                await _orderProcessingService.MarkOrderAsPaidAsync(order);

                return RedirectToAction("Edit", new { id = order.Id });
            }
            catch (Exception exc)
            {
                //prepare model
                var model = await _orderModelService.PrepareOrderModelAsync(null, order);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOrderItem")]
        public virtual async Task<IActionResult> EditOrderItem(int id, IFormCollection form)
        {
            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnSaveOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue["btnSaveOrderItem".Length..]);

            var orderItem = await _orderService.GetOrderItemByIdAsync(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            if (!decimal.TryParse(form["pvPrice" + orderItemId], out var price))
                price = orderItem.Price;
            if (!int.TryParse(form["pvQuantity" + orderItemId], out var quantity))
                quantity = orderItem.Quantity;

            var product = await _productService.GetByIdAsync(orderItem.ProductId);

            if (quantity > 0)
            {
                var qtyDifference = orderItem.Quantity - quantity;

                if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
                {
                    orderItem.Price = price;
                    orderItem.Quantity = quantity;
                    await _orderService.UpdateOrderItemAsync(orderItem);
                }
            }
            else
            {
                //delete item
                await _orderService.DeleteOrderItemAsync(orderItem.Id);
            }

            //add a note
            await _orderService.CreateOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order item has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            return RedirectToAction("Edit", new { id = order.Id });
        }

        public virtual async Task<IActionResult> LoadOrderStatistics(string period)
        {
            var result = new List<object>();

            var nowDt = DateTime.UtcNow.ConvertToUserTime(DateTimeExtensions.GetCurrentTimeZone());
            var timeZone = DateTimeExtensions.GetCurrentTimeZone();

            switch (period)
            {
                case "year":
                    // year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (var i = 0; i <= 12; i++)
                    {
                        result.Add(new
                        {
                            date = searchYearDateUser.Date.ToString("Y", CultureInfo.CurrentCulture),
                            value = (await _orderService.SearchOrdersAsync(new OrderParameters()
                            {
                                StartDate = searchYearDateUser.ConvertToUtcTime(timeZone),
                                EndDate = searchYearDateUser.AddMonths(1).ConvertToUtcTime(timeZone),
                                PageNumber = 1,
                                PageSize = 1
                            })).MetaData.TotalCount.ToString()
                        });

                        searchYearDateUser = searchYearDateUser.AddMonths(1);
                    }

                    break;
                case "month":
                    // month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (var i = 0; i <= 30; i++)
                    {
                        result.Add(new
                        {
                            date = searchMonthDateUser.Date.ToString("M", CultureInfo.CurrentCulture),
                            value = (await _orderService.SearchOrdersAsync(new OrderParameters()
                            {
                                StartDate = searchMonthDateUser.ConvertToUtcTime(timeZone),
                                EndDate = searchMonthDateUser.AddDays(1).ConvertToUtcTime(timeZone),
                                PageNumber = 1,
                                PageSize = 1
                            })).MetaData.TotalCount.ToString()
                        });

                        searchMonthDateUser = searchMonthDateUser.AddDays(1);
                    }

                    break;
                case "week":
                default:
                    // week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new
                        {
                            date = searchWeekDateUser.Date.ToString("d dddd", CultureInfo.CurrentCulture),
                            value = (await _orderService.SearchOrdersAsync(new OrderParameters()
                            {
                                StartDate = searchWeekDateUser.ConvertToUtcTime(timeZone),
                                EndDate = searchWeekDateUser.AddDays(1).ConvertToUtcTime(timeZone),
                                PageNumber = 1,
                                PageSize = 1
                            })).MetaData.TotalCount.ToString()
                        });

                        searchWeekDateUser = searchWeekDateUser.AddDays(1);
                    }

                    break;
            }

            return Json(result);
        }
    }
}
