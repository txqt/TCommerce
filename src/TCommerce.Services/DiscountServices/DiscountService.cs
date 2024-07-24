using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using TCommerce.Core.Extensions;
using TCommerce.Core.Helpers;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;
using TCommerce.Services.IRepositoryServices;
using TCommerce.Services.ManufacturerServices;

namespace TCommerce.Services.DiscountServices
{
    public class DiscountService : IDiscountService
    {
        private readonly IRepository<Discount> _discountRepository;
        private readonly IRepository<DiscountMapping> _discountMappingRepository;
        private readonly IRepository<DiscountUsageHistory> _discountUsageHistoryRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IManufacturerService _manufacturerServices;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;

        public DiscountService(IRepository<DiscountMapping> discountMappingRepository, IRepository<Product> productRepository, IRepository<Discount> discountRepository, IRepository<DiscountUsageHistory> discountUsageHistoryRepository, IRepository<Order> orderRepository, IManufacturerService manufacturerServices, ICategoryService categoryService, IUserService userService, IShoppingCartService shoppingCartService, IProductService productService)
        {
            _discountMappingRepository = discountMappingRepository;
            _discountRepository = discountRepository;
            _discountUsageHistoryRepository = discountUsageHistoryRepository;
            _orderRepository = orderRepository;
            _manufacturerServices = manufacturerServices;
            _categoryService = categoryService;
            _userService = userService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
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

        public async Task<ServiceResponse<string>> ValidateDiscountAsync(Discount discount, User user)
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

        public async Task<ServiceResponse<bool>> CheckValidDiscountAsync(Discount discount)
        {
            if (discount == null)
                return new ServiceErrorResponse<bool>("Discount cannot be null.");

            var user = await _userService.GetCurrentUser();
            var carts = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart);
            if (carts == null)
                return new ServiceErrorResponse<bool>("Shopping cart is empty.");

            var (isHaveMapping, mappingMessage) = await CheckDiscountMappingAsync(discount, carts);
            var validCheck = await ValidateDiscountAsync(discount, user);

            if (isHaveMapping && validCheck.Success)
            {
                return new ServiceSuccessResponse<bool>(true);
            }

            return isHaveMapping ? new ServiceErrorResponse<bool>(validCheck.Message) : new ServiceErrorResponse<bool>(mappingMessage);
        }

        private async Task<(bool, string)> CheckDiscountMappingAsync(Discount discount, IEnumerable<ShoppingCartItem> carts)
        {
            foreach (var item in carts)
            {
                var (hasMapping, message) = discount.DiscountType switch
                {
                    DiscountType.AssignedToSkus => await CheckProductMappingAsync(item.ProductId, discount.Id),
                    DiscountType.AssignedToCategories => await CheckCategoryMappingAsync(item.ProductId, discount.Id),
                    DiscountType.AssignedToManufacturers => await CheckManufacturerMappingAsync(item.ProductId, discount.Id),
                    _ => (false, "Invalid discount type.")
                };

                if (hasMapping)
                    return (true, message);
            }
            return (false, "There are no suitable products to apply.");
        }

        private async Task<(bool, string)> CheckProductMappingAsync(int productId, int discountId)
        {
            var mapping = await _productService.GetDiscountAppliedToProductAsync(productId, discountId);
            return (mapping != null, mapping != null ? "" : "There are no suitable products to apply.");
        }

        private async Task<(bool, string)> CheckCategoryMappingAsync(int productId, int discountId)
        {
            var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(productId);
            if (productCategories == null)
                return (false, "No categories found for the product.");

            foreach (var categoryId in productCategories.Select(pc => pc.Id))
            {
                if (await _categoryService.GetDiscountAppliedToCategoryAsync(categoryId, discountId) != null)
                    return (true, "");
            }
            return (false, "There are no suitable products to apply.");
        }

        private async Task<(bool, string)> CheckManufacturerMappingAsync(int productId, int discountId)
        {
            var productManufacturers = await _manufacturerServices.GetProductManufacturerByProductIdAsync(productId);
            if (productManufacturers == null)
                return (false, "No manufacturers found for the product.");

            foreach (var manufacturerId in productManufacturers.Select(pm => pm.Id))
            {
                if (await _manufacturerServices.GetDiscountAppliedToManufacturerAsync(manufacturerId, discountId) != null)
                    return (true, "");
            }
            return (false, "No valid manufacturer mapping found for the discount.");
        }


        public async Task<PagedList<Discount>> SearchDiscount(DiscountParameters discountParameters)
        {
            var query = _discountRepository.Query;

            if (discountParameters.ids != null && discountParameters.ids.Count > 0)
            {
                query = query.Where(p => discountParameters.ids.Contains(p.Id));
            }

            if (discountParameters.StartDateUtc.HasValue)
            {
                query = query.Where(x => x.StartDateUtc >= discountParameters.StartDateUtc.Value);
            }

            if (discountParameters.EndDateUtc.HasValue)
            {
                query = query.Where(x => x.EndDateUtc <= discountParameters.EndDateUtc.Value);
            }

            if (!string.IsNullOrEmpty(discountParameters.CouponCode))
            {
                query = query.Where(x => x.CouponCode == discountParameters.CouponCode);
            }

            if (!string.IsNullOrEmpty(discountParameters.DiscountName))
            {
                query = query.Where(x => x.Name == discountParameters.DiscountName);
            }

            if ((int)discountParameters.DiscountType > 0)
            {
                query = query.Where(x => x.DiscountTypeId == (int)discountParameters.DiscountType);
            }

            if(discountParameters.IsActiveId > 0)
            {
                query = query.Where(x => x.IsActive == (discountParameters.IsActiveId == 1 ? true : false));
            }

            if (!string.IsNullOrEmpty(discountParameters.OrderBy))
            {
                query = query.Sort(discountParameters.OrderBy);
            }

            return await PagedList<Discount>.ToPagedList
                (query, discountParameters.PageNumber, discountParameters.PageSize);
        }

        public async Task<Discount?> GetByIdAsync(int id)
        {
            return await _discountRepository.GetByIdAsync(id);
        }

        public virtual async Task<DiscountUsageHistory> GetDiscountUsageHistoryByIdAsync(int discountUsageHistoryId)
        {
            return await _discountUsageHistoryRepository.GetByIdAsync(discountUsageHistoryId);
        }

        public virtual async Task CreateDiscountUsageHistoryAsync(DiscountUsageHistory discountUsageHistory)
        {
            await _discountUsageHistoryRepository.CreateAsync(discountUsageHistory);
        }

        public virtual async Task DeleteDiscountUsageHistoryAsync(int id)
        {
            await _discountUsageHistoryRepository.DeleteAsync(id);
        }
    }
}
