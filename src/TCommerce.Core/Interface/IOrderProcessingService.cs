using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Orders;

namespace TCommerce.Core.Interface
{
    public interface IOrderProcessingService
    {
        bool CanCancelOrder(Order order);
        bool CanCapture(Order order);
        bool CanMarkOrderAsPaid(Order order);
        bool CanRefund(Order order);
        bool CanRefundOffline(Order order);
        bool CanPartiallyRefund(Order order, decimal amountToRefund);
        bool CanPartiallyRefundOffline(Order order, decimal amountToRefund);
        bool CanVoid(Order order);
        bool CanVoidOffline(Order order);
        Task CancelOrderAsync(Order order, bool notifyCustomer);
        Task MarkOrderAsPaidAsync(Order order);
        Task<List<string>> ReOrderAsync(Order order);
    }
}
