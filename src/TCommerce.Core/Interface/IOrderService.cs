using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Orders;

namespace TCommerce.Core.Interface
{
    public interface IOrderService
    {
        public Task CreateOrderAsync(Order order);
        public Task UpdateOrderAsync(Order order);
        public Task DeleteOrderAsync(int id);
        public Task<List<Order>> GetAllOrdersAsync(bool includeDeleted);
        public Task<Order> GetOrderByIdAsync(int id);
        public Task<Order> GetOrderByGuidAsync(Guid id);
        public Task CreateOrderItemsAsync(OrderItem orderItem);
        public Task BulkCreateOrderItemsAsync(List<OrderItem> orderItem);
    }
}
