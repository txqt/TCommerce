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
        /// <summary>
        /// Asynchronously calculates the subtotal of a list of shopping cart items.
        /// </summary>
        /// <param name="carts"></param>
        /// <returns></returns>
        Task<decimal> CalculateSubTotalAsync(List<ShoppingCartItem> carts);
        /// <summary>
        /// Asynchronously calculates the adjusted price of a product based on a shopping cart item.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="sci"></param>
        /// <returns></returns>
        Task<decimal> CalculateAdjustedPriceAsync(Product product, ShoppingCartItem sci);
        /// <summary>
        /// Calculates the total discount for an order based on its total amount.
        /// </summary>
        /// <param name="orderTotal"></param>
        /// <returns></returns>
        Task<decimal> CalculateOrderTotalDiscount(decimal orderTotal);
        /// <summary>
        /// Asynchronously calculates the subtotal discount amount for a list of shopping cart items based on their subtotal.
        /// </summary>
        /// <param name="carts"></param>
        /// <param name="subTotal"></param>
        /// <returns></returns>
        Task<decimal> CalculateSubtotalDiscountAmountAsync(List<ShoppingCartItem> carts, decimal subTotal);
        /// <summary>
        /// Calculates the tax amount based on the subtotal after discount and the applicable tax rates.
        /// </summary>
        /// <param name="subTotalAfterDiscount"></param>
        /// <param name="taxRates"></param>
        /// <returns></returns>
        decimal CalculateTax(decimal subTotalAfterDiscount, decimal taxRates);
        /// <summary>
        /// Calculates the shipping fee for a list of shopping cart items.
        /// </summary>
        /// <param name="carts"></param>
        /// <returns></returns>
        decimal CalculateShippingFee(List<ShoppingCartItem> carts);
        /// <summary>
        /// Calculates the adjusted price of a product, considering a price adjustment that can be either a fixed amount or a percentage.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="priceAdjustment"></param>
        /// <param name="priceAdjustmentUsePercentage"></param>
        /// <returns></returns>
        decimal CalculateAdjustedPrice(Product product, decimal priceAdjustment, bool priceAdjustmentUsePercentage = false);
    }
}
