using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.Drawing.Printing;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Payments;
using TCommerce.Core.Models.Users;
using TCommerce.Services.IRepositoryServices;
using TCommerce.Services.PriceCalulationServices;
using TCommerce.Services.ProductServices;

namespace TCommerce.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IProductService _productService;
        private readonly IPriceCalculationService _priceCalculationService;

        public OrderService(IRepository<Order> orderRepository, IRepository<OrderItem> orderItemRepository, IRepository<OrderNote> orderNoteRepository, IRepository<Address> addressRepository, IProductService productService, IPriceCalculationService priceCalculationService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _orderNoteRepository = orderNoteRepository;
            _addressRepository = addressRepository;
            _productService = productService;
            _priceCalculationService = priceCalculationService;
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

        public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            return await _orderItemRepository.Table.Where(x => x.OrderId == orderId).ToListAsync();
        }

        public async Task<PagedList<Order>> SearchOrdersAsync(OrderParameters orderParameters)
        {
            var query = _orderRepository.Query;

            if (orderParameters.UserId is not null && orderParameters.UserId != Guid.Empty)
                query = query.Where(o => o.UserId == orderParameters.UserId);
            
            if (orderParameters.ProductId > 0)
            {
                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        where oi.ProductId == orderParameters.ProductId
                        select o;

                query = query.Distinct();
            }
            
            if (!string.IsNullOrEmpty(orderParameters.PaymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == orderParameters.PaymentMethodSystemName);
            
            if (orderParameters.StartDate.HasValue)
                query = query.Where(o => orderParameters.StartDate.Value <= o.CreatedOnUtc);
            
            if (orderParameters.EndDate.HasValue)
                query = query.Where(o => orderParameters.EndDate.Value >= o.CreatedOnUtc);
            
            if (orderParameters.osIds != null)
            {
                orderParameters.osIds.Remove(0);
                if(orderParameters.osIds.Any())
                    query = query.Where(o => orderParameters.osIds.Contains(o.OrderStatusId));
            }
            
            if (orderParameters.psIds != null)
            {
                orderParameters.psIds.Remove(0);
                if(orderParameters.psIds.Any())
                    query = query.Where(o => orderParameters.psIds.Contains(o.PaymentStatusId));
            }
            
            if (!string.IsNullOrEmpty(orderParameters.OrderNotes))
                query = query.Where(o => _orderNoteRepository.Table.Any(oNote => oNote.OrderId == o.Id && oNote.Note.Contains(orderParameters.OrderNotes)));
            
            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);
            
            //database layer paging
            return await PagedList<Order>.ToPagedList
                (query, orderParameters.PageNumber, orderParameters.PageSize);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateAsync(order);
        }
    }
}
