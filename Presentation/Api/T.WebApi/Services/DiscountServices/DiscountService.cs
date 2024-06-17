using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using T.Library.Model;
using T.Library.Model.Common;
using T.Library.Model.Discounts;
using T.Library.Model.Interface;
using T.Library.Model.Orders;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.WebApi.Helpers;
using T.WebApi.Services.CategoryServices;
using T.WebApi.Services.IRepositoryServices;
using T.WebApi.Services.ManufacturerServices;
using T.WebApi.Services.ShoppingCartServices;
using T.WebApi.Services.UserServices;

namespace T.WebApi.Services.DiscountServices
{
    public interface IDiscountService : IDiscountServiceCommon
    {
        Task<DiscountMapping?> GetDiscountMapping(int discountId, int entityId, int discountTypeId);
    }
    public class DiscountService : IDiscountService
    {
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<DiscountMapping> _discountMappingRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<DiscountUsageHistory> _discountUsageHistoryRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IManufacturerServices _manufacturerServices;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IShoppingCartService _shoppingCartService;

        public DiscountService(IRepository<DiscountMapping> discountMappingRepository, IRepository<Product> productRepository, IRepository<Discount> discountRepository, IRepository<DiscountUsageHistory> discountUsageHistoryRepository, IRepository<Order> orderRepository, IManufacturerServices manufacturerServices, ICategoryService categoryService, IUserService userService, IShoppingCartService shoppingCartService)
        {
            _discountMappingRepository = discountMappingRepository;
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _discountUsageHistoryRepository = discountUsageHistoryRepository;
            _orderRepository = orderRepository;
            _manufacturerServices = manufacturerServices;
            _categoryService = categoryService;
            _userService = userService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task CreateDiscountAsync(Discount discount)
        {
            await _discountRepository.CreateAsync(discount);
        }

        public async Task DeleteDiscountAsync(int id)
        {
            await _discountRepository.DeleteAsync(id);
        }

        public async Task UpdateDiscountAsync(Discount discount)
        {
            await _discountRepository.UpdateAsync(discount);
        }

        public async Task<ServiceResponse<string>> ValidateDiscountAsync(Discount discount, UserModel user)
        {
            if (discount == null)
                return new ServiceErrorResponse<string>("Invalid coupon code.");

            if (!discount.IsActive)
                return new ServiceErrorResponse<string>("This discount is not active.");

            if (discount.RequiresCouponCode && string.IsNullOrEmpty(discount.CouponCode))
                return new ServiceErrorResponse<string>("This discount requires a coupon code.");

            var currentDate = DateTime.Now;

            if (discount.StartDateUtc.HasValue && discount.StartDateUtc.Value > currentDate)
                return new ServiceErrorResponse<string>("This discount is not yet valid.");

            if (discount.EndDateUtc.HasValue && discount.EndDateUtc.Value < currentDate)
                return new ServiceErrorResponse<string>("This discount has expired.");

            if (discount.DiscountLimitation == DiscountLimitationType.NTimesOnly && discount.LimitationTimes <= 0)
            {
                var usageCount = (await GetAllDiscountUsageHistoryAsync(discount.Id, null, null, false)).Count;
                if (usageCount >= discount.LimitationTimes)
                    return new ServiceErrorResponse<string>("This discount has already been used the maximum number of times.");
            }


            if (discount.DiscountLimitation == DiscountLimitationType.NTimesPerCustomer)
            {
                // Assuming you have a method to get the usage count of the discount by user
                var usageCount = (await GetAllDiscountUsageHistoryAsync(discount.Id, user.Id, null, false)).Count;
                if (usageCount >= discount.LimitationTimes)
                    return new ServiceErrorResponse<string>("You have already used this discount the maximum number of times.");
            }

            return new ServiceSuccessResponse<string>("Valid");
        }
        public virtual async Task<List<DiscountUsageHistory>> GetAllDiscountUsageHistoryAsync(int? discountId = null,
        Guid? userId = null, int? orderId = null, bool includeCancelledOrders = true)
        {
            return (await _discountUsageHistoryRepository.GetAllAsync(query =>
            {
                //filter by discount
                if (discountId.HasValue && discountId.Value > 0)
                    query = query.Where(historyRecord => historyRecord.DiscountId == discountId.Value);

                //filter by user
                if (userId.HasValue && userId.Value != Guid.Empty)
                    query = from duh in query
                            join order in _orderRepository.Table on duh.OrderId equals order.Id
                            where order.UserId == userId
                            select duh;

                //filter by order
                if (orderId.HasValue && orderId.Value > 0)
                    query = query.Where(historyRecord => historyRecord.OrderId == orderId.Value);

                //ignore invalid orders
                query = from duh in query
                        join order in _orderRepository.Table on duh.OrderId equals order.Id
                        where !order.Deleted && (includeCancelledOrders || order.OrderStatusId != (int)OrderStatus.Cancelled)
                        select duh;

                //order
                query = query.OrderByDescending(historyRecord => historyRecord.CreatedOnUtc)
                    .ThenBy(historyRecord => historyRecord.Id);

                return query;
            }, cacheKey: CacheKeysDefault<DiscountUsageHistory>.AllPrefix + $"{discountId}.{userId}.{orderId}.{includeCancelledOrders}")).ToList();
        }

        public async Task<Discount?> GetDiscountByCode(string discountCode)
        {
            return await (from d in _discountRepository.Table
                          where d.RequiresCouponCode && d.CouponCode != null && d.CouponCode.ToLower() == discountCode.ToLower()
                          select d).FirstOrDefaultAsync();
        }

        public async Task<DiscountMapping?> GetDiscountMapping(int discountId, int entityId, int discountTypeId)
        {
            var query = _discountMappingRepository.Query;

            query = query.Where(x => x.DiscountId == discountId);

            if (entityId > 0)
            {
                query = query.Where(x => x.EntityId == entityId);
            }

            if (discountTypeId > 0)
            {
                query = query.Where(x => x.DiscountTypeId == discountTypeId);
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<ServiceResponse<bool>> CheckValidDiscountCode(string discountCode)
        {
            var discount = await GetDiscountByCode(discountCode);
            if (discount != null)
            {
                var user = await _userService.GetCurrentUser();
                var carts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart);
                if (carts != null)
                {
                    var isHaveMapping = false;
                    switch (discount.DiscountType)
                    {
                        case DiscountType.AssignedToSkus:
                            foreach (var item in carts)
                            {
                                var mapping = await GetDiscountMapping(discount.Id, item.ProductId, (int)DiscountType.AssignedToSkus);
                                if (mapping is not null)
                                {
                                    isHaveMapping = true;
                                }
                            }
                            break;
                        case DiscountType.AssignedToCategories:
                            foreach (var item in carts)
                            {

                                var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId);
                                if (productCategories is null)
                                {
                                    return new ServiceErrorResponse<bool>();
                                }
                                var categoryIdList = productCategories.Select(s => s.Id).ToList();
                                foreach (var id in categoryIdList)
                                {
                                    var mapping = await GetDiscountMapping(discount.Id, item.ProductId, (int)DiscountType.AssignedToCategories);
                                    if (mapping is not null)
                                    {
                                        isHaveMapping = true;
                                    }
                                }
                            }
                            break;
                        case DiscountType.AssignedToManufacturers:
                            foreach (var item in carts)
                            {

                                var productManufacturers = await _manufacturerServices.GetProductManufacturerByProductIdAsync(item.ProductId);
                                if (productManufacturers is null)
                                {
                                    return new ServiceErrorResponse<bool>();
                                }
                                var manufacturerIdList = productManufacturers.Select(s => s.Id).ToList();
                                foreach (var id in manufacturerIdList)
                                {
                                    var mapping = await GetDiscountMapping(discount.Id, item.ProductId, (int)DiscountType.AssignedToManufacturers);
                                    if (mapping is not null)
                                    {
                                        isHaveMapping = true;
                                    }
                                }
                            }
                            break;
                        case DiscountType.AssignedToOrderSubTotal:
                        case DiscountType.AssignedToOrderTotal:
                            break;
                    }
                    var validCheck = await ValidateDiscountAsync(discount, user);
                    if (isHaveMapping && validCheck.Success)
                    {
                        return new ServiceSuccessResponse<bool>();
                    }
                }
                else
                {
                    return new ServiceErrorResponse<bool>();
                }
            }

            return new ServiceSuccessResponse<bool>();
        }
    }
}
