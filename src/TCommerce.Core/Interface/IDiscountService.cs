using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Users;
using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Core.Interface
{
    public interface IDiscountService
    {
        Task<PagedList<Discount>> SearchDiscount(DiscountParameters discountParameters);
        Task CreateDiscountAsync(Discount discount);
        Task UpdateDiscountAsync(Discount discount);
        Task DeleteDiscountAsync(int id);
        Task<Discount?> GetDiscountByCode(string discountCode);
        Task<Discount?> GetByIdAsync(int id);
        Task<ServiceResponse<string>> ValidateDiscountAsync(Discount discount, User user);
        Task<ServiceResponse<bool>> CheckValidDiscountAsync(Discount discount);
        Task<List<DiscountUsageHistory>> GetAllDiscountUsageHistoryAsync(int? discountId = null,
        Guid? userId = null, int? orderId = null, bool includeCancelledOrders = true);
        Task<DiscountUsageHistory> GetDiscountUsageHistoryByIdAsync(int discountUsageHistoryId);
        Task CreateDiscountUsageHistoryAsync(DiscountUsageHistory discountUsageHistory);
        Task DeleteDiscountUsageHistoryAsync(int id);
    }
}
