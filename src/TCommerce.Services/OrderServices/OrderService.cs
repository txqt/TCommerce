using Microsoft.EntityFrameworkCore;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Orders;
using TCommerce.Services.IRepositoryServices;

namespace TCommerce.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;

        public OrderService(IRepository<Order> orderRepository, IRepository<OrderItem> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task BulkCreateOrderItemsAsync(List<OrderItem> orderItem)
        {
            await _orderItemRepository.BulkCreateAsync(orderItem);
        }

        public async Task CreateOrderAsync(Order order)
        {
            await _orderRepository.CreateAsync(order);
        }

        public async Task CreateOrderItemsAsync(OrderItem orderItem)
        {
            await _orderItemRepository.CreateAsync(orderItem);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _orderRepository.DeleteAsync(id);
        }

        public async Task<List<Order>> GetAllOrdersAsync(bool includeDeleted)
        {
            return (await _orderRepository.GetAllAsync(null, null, includeDeleted)).ToList();
        }

        public async Task<Order> GetOrderByGuidAsync(Guid id)
        {
            return await _orderRepository.Table.Where(x=>x.OrderGuid == id).FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateAsync(order);
        }
    }
}
