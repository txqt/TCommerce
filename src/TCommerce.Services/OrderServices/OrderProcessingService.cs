using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Payments;
using TCommerce.Core.Models.SendMail;
using TCommerce.Core.Models.Users;
using TCommerce.Services.PaymentServices;

namespace TCommerce.Services.OrderServices
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly IProductAttributeConverter _productAttributeParser;

        public OrderProcessingService(IPaymentService paymentService, IOrderService orderService, IEmailSender emailSender, IUserService userService, IProductAttributeConverter productAttributeParser)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _emailSender = emailSender;
            _userService = userService;
            _productAttributeParser = productAttributeParser;
        }

        public bool CanCancelOrder(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            return true;
        }

        public bool CanCapture(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderStatus == OrderStatus.Cancelled ||
                order.OrderStatus == OrderStatus.Pending)
                return false;

            if (order.PaymentStatus == PaymentStatus.Authorized)
                return true;

            return false;
        }

        public bool CanMarkOrderAsPaid(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.Refunded ||
                order.PaymentStatus == PaymentStatus.Voided)
                return false;

            return true;
        }

        public bool CanPartiallyRefund(Order order, decimal amountToRefund)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            var canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if ((order.PaymentStatus == PaymentStatus.Paid ||
            order.PaymentStatus == PaymentStatus.PartiallyRefunded) &&
                _paymentService.SupportPartiallyRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        public bool CanPartiallyRefundOffline(Order order, decimal amountToRefund)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            var canBeRefunded = order.OrderTotal - order.RefundedAmount;
            if (canBeRefunded <= decimal.Zero)
                return false;

            if (amountToRefund > canBeRefunded)
                return false;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.PartiallyRefunded)
                return true;

            return false;
        }

        public bool CanRefund(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderTotal == decimal.Zero)
                return false;

            //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
            if (order.RefundedAmount > decimal.Zero)
                return false;

            //uncomment the lines below in order to disallow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Paid &&
                _paymentService.SupportRefund(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        public bool CanRefundOffline(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderTotal == decimal.Zero)
                return false;

            //refund cannot be made if previously a partial refund has been already done. only other partial refund can be made in this case
            if (order.RefundedAmount > decimal.Zero)
                return false;

            //uncomment the lines below in order to disallow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //     return false;

            if (order.PaymentStatus == PaymentStatus.Paid)
                return true;

            return false;
        }

        public bool CanVoid(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Authorized &&
                _paymentService.SupportVoid(order.PaymentMethodSystemName))
                return true;

            return false;
        }

        public bool CanVoidOffline(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (order.OrderTotal == decimal.Zero)
                return false;

            //uncomment the lines below in order to allow this operation for cancelled orders
            //if (order.OrderStatus == OrderStatus.Cancelled)
            //    return false;

            if (order.PaymentStatus == PaymentStatus.Authorized)
                return true;

            return false;
        }

        public virtual async Task CancelOrderAsync(Order order, bool notifyCustomer)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (!CanCancelOrder(order))
                throw new Exception("Cannot do cancel for order.");

            //cancel order
            await SetOrderStatusAsync(order, OrderStatus.Cancelled, notifyCustomer);

            //add a note
            await AddOrderNoteAsync(order, "Order has been cancelled");
        }

        protected virtual async Task SetOrderStatusAsync(Order order, OrderStatus os, bool notifyCustomer)
        {
            ArgumentNullException.ThrowIfNull(order);

            var prevOrderStatus = order.OrderStatus;
            if (prevOrderStatus == os)
                return;

            //set and save new order status
            order.OrderStatusId = (int)os;
            await _orderService.UpdateOrderAsync(order);

            //order notes, notifications
            await AddOrderNoteAsync(order, $"Order status has been changed to {os.ToString()}");

            var user = await _userService.GetUserById(order.UserId);

            if (prevOrderStatus != OrderStatus.Processing &&
                os == OrderStatus.Processing
                && notifyCustomer)
            {
                //notification
                await _emailSender.SendEmailAsync(new EmailDto
                {
                    Subject = "Order status",
                    Body = $"Your order #{order.OrderGuid} status: {OrderStatus.Processing.ToString()}",
                    To = user.Email
                });
                await AddOrderNoteAsync(order, $"\"Order processing\" email (to customer): {user.Email}.");
            }

            if (prevOrderStatus != OrderStatus.Complete &&
                os == OrderStatus.Complete
                && notifyCustomer)
            {
                //notification
                await _emailSender.SendEmailAsync(new EmailDto
                {
                    Subject = "Order status",
                    Body = $"Your order #{order.OrderGuid} status: {OrderStatus.Complete.ToString()}",
                    To = user.Email
                });
                await AddOrderNoteAsync(order, $"\"Order complete\" email (to customer): {user.Email}.");
            }

            if (prevOrderStatus != OrderStatus.Cancelled &&
                os == OrderStatus.Cancelled
                && notifyCustomer)
            {
                //notification
                await _emailSender.SendEmailAsync(new EmailDto
                {
                    Subject = "Order status",
                    Body = $"Your order #{order.OrderGuid} status: {OrderStatus.Cancelled.ToString()}",
                    To = user.Email
                });
                await AddOrderNoteAsync(order, $"\"Order cancelled\" email (to customer): {user.Email}.");
            }
        }
        protected virtual async Task AddOrderNoteAsync(Order order, string note)
        {
            await _orderService.CreateOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = note,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });
        }

        public async Task MarkOrderAsPaidAsync(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            if (!CanMarkOrderAsPaid(order))
                throw new Exception("You can't mark this order as paid");

            order.PaymentStatusId = (int)PaymentStatus.Paid;
            order.PaidDateUtc = DateTime.UtcNow;
            await _orderService.UpdateOrderAsync(order);

            //add a note
            await AddOrderNoteAsync(order, "Order has been marked as paid");

            await CheckAndSaveOrderStatusAsync(order, false);

            if (order.PaymentStatus == PaymentStatus.Paid)
                await ProcessOrderPaidAsync(order);
        }
        protected virtual async Task CheckAndSaveOrderStatusAsync(Order order, bool needOrderSave)
        {
            ArgumentNullException.ThrowIfNull(order);

            var completed = false;
            var isOrderSaved = !needOrderSave;

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                if (!order.PaidDateUtc.HasValue)
                {
                    //ensure that paid date is set
                    order.PaidDateUtc = DateTime.UtcNow;
                    isOrderSaved = false;
                }
                //shipping is not required
                completed = true;
            }

            switch (order.OrderStatus)
            {
                case OrderStatus.Pending:
                    if (order.PaymentStatus == PaymentStatus.Authorized ||
                        order.PaymentStatus == PaymentStatus.Paid)
                    {
                        await SetOrderStatusAsync(order, OrderStatus.Processing, !completed);
                        isOrderSaved = true;
                    }

                    break;
                //is order complete?
                case OrderStatus.Cancelled:
                case OrderStatus.Complete:
                    if (!isOrderSaved)
                        await _orderService.UpdateOrderAsync(order);
                    return;
            }

            if (completed)
            {
                await SetOrderStatusAsync(order, OrderStatus.Complete, true);
                isOrderSaved = true;
            }

            if (!isOrderSaved)
                await _orderService.UpdateOrderAsync(order);
        }
        protected virtual async Task ProcessOrderPaidAsync(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);
            var user = await _userService.GetCurrentUser();
            var customer = await _userService.GetUserById(order.UserId);

            //order paid email notification
            if (order.OrderTotal != decimal.Zero)
            {
                //notification
                await _emailSender.SendEmailAsync(new EmailDto
                {
                    Subject = "Order status",
                    Body = $"Your order #{order.OrderGuid} paid status: {order.OrderStatus.ToString()}",
                    To = customer.Email
                });
                await AddOrderNoteAsync(order, $"\"Order paid\" email (to store owner).");

                await _emailSender.SendEmailAsync(new EmailDto
                {
                    Subject = "Order status",
                    Body = $"The order #{order.OrderGuid} paid status: {order.OrderStatus.ToString()}",
                    To = user.Email
                });
                await AddOrderNoteAsync(order, $"\"Order paid\" email (to store owner).");
            }
        }
    }
}
