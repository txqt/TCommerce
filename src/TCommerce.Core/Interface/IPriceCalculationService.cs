using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Orders;

namespace TCommerce.Core.Interface
{
    public interface IPriceCalculationService
    {
        Task<decimal> CalculateAdjustedPriceAsync(Product product, ShoppingCartItem sci);
        decimal CalculateOrderSubTotalDiscount(decimal subTotal, Discount discount);
        Task<decimal> CalculateOrderTotalDiscount(decimal orderTotal);
        Task<decimal> CalculateDiscountsAsync(List<ShoppingCartItem> carts, decimal subTotal);
        decimal CalculateTax(decimal subTotalAfterDiscount);
        decimal CalculateShippingFee(List<ShoppingCartItem> carts);
        decimal CalculateAdjustedPrice(Product product, decimal priceAdjustment, bool priceAdjustmentUsePercentage = false);
    }
}
