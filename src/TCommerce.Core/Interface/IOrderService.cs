using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Users;

namespace TCommerce.Core.Interface
{
    public interface IOrderService
    {
        public Task CreateOrderAsync(Order order);
        public Task UpdateOrderAsync(Order order);
        public Task DeleteOrderAsync(int id);
        public Task<List<Order>> GetAllOrdersAsync(bool includeDeleted);
        public Task<PagedList<Order>> SearchOrdersAsync(OrderParameters orderParameters);
        public Task<Order> GetOrderByIdAsync(int id);
        public Task<Order> GetOrderByGuidAsync(Guid id);
        public Task CreateOrderItemsAsync(OrderItem orderItem);
        public Task BulkCreateOrderItemsAsync(List<OrderItem> orderItem);
        public Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        public Task<OrderItem> GetOrderItemByIdAsync(int orderItemId);
        public Task UpdateOrderItemAsync(OrderItem orderItem);
        public Task DeleteOrderItemAsync(int orderItemId);
        Task<OrderNote> GetOrderNoteByIdAsync(int orderNoteId);
        Task<List<OrderNote>> GetOrderNotesByOrderIdAsync(int orderId, bool? displayToCustomer = null);
        Task DeleteOrderNoteAsync(int id);
        Task CreateOrderNoteAsync(OrderNote orderNote);
    }
}
