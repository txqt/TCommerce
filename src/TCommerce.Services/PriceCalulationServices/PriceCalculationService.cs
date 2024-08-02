using Microsoft.AspNetCore.Http;
using TCommerce.Core.Models;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Orders;
using TCommerce.Core.Extensions;
using TCommerce.Services.ProductServices;
using TCommerce.Core.Interface;

namespace TCommerce.Services.PriceCalulationServices
{
    public class PriceCalculationService : IPriceCalculationService
    {
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDiscountService _discountService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;

        public PriceCalculationService(
            IProductAttributeConverter productAttributeConverter,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IHttpContextAccessor httpContextAccessor,
            IDiscountService discountService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService)
        {
            _productAttributeConverter = productAttributeConverter;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _discountService = discountService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
        }

        public async Task<decimal> CalculateAdjustedPriceAsync(Product product, ShoppingCartItem sci)
        {
            decimal adjustedPrice = product.Price * sci.Quantity;

            if (!string.IsNullOrEmpty(sci.AttributeJson))
            {
                var selectedAttributes = _productAttributeConverter.ConvertToObject(sci.AttributeJson);
                foreach (var selectedAttribute in selectedAttributes)
                {
                    foreach (var attributeValueId in selectedAttribute.ProductAttributeValueIds)
                    {
                        var attributeValue = await _productAttributeService.GetProductAttributeValuesByIdAsync(attributeValueId);

                        adjustedPrice += attributeValue.PriceAdjustmentUsePercentage
                            ? (attributeValue.PriceAdjustment * product.Price / 100) * sci.Quantity
                            : attributeValue.PriceAdjustment * sci.Quantity;
                    }
                }
            }

            return adjustedPrice;
        }

        public decimal CalculateAdjustedPrice(Product product, decimal priceAdjustment, bool priceAdjustmentUsePercentage = false)
        {
            return priceAdjustmentUsePercentage
                ? product.Price * (1 + priceAdjustment / 100)
                : product.Price + priceAdjustment;
        }

        public async Task<decimal> CalculateSubtotalDiscountAmountAsync(List<ShoppingCartItem> carts, decimal subTotal)
        {
            var discountAmount = 0m;
            var session = _httpContextAccessor.HttpContext.Session;
            var discounts = session.Get<List<string>>("Discount") ?? new List<string>();
            var maxDiscounts = new Dictionary<DiscountType, decimal>();

            foreach (var discountCode in discounts.ToList())
            {
                var discount = await _discountService.GetDiscountByCode(discountCode);
                if (discount == null || !(await _discountService.CheckValidDiscountAsync(discount)).Success)
                {
                    discounts.Remove(discountCode);
                    session.Set("Discount", discounts);
                    continue;
                }

                var tempDiscountAmount = await CalculateDiscountAmountAsync(carts, subTotal, discount);

                if (discount.IsCumulative)
                {
                    discountAmount += tempDiscountAmount;
                }
                else
                {
                    maxDiscounts[discount.DiscountType] = Math.Max(maxDiscounts.GetValueOrDefault(discount.DiscountType), tempDiscountAmount);
                }
            }

            return discountAmount + maxDiscounts.Values.Sum();
        }

        private async Task<decimal> CalculateDiscountAmountAsync(List<ShoppingCartItem> carts, decimal subTotal, Discount discount)
        {
            return discount.DiscountType switch
            {
                DiscountType.AssignedToSkus => await CalculateSkuDiscountsAsync(carts, discount),
                DiscountType.AssignedToCategories => await CalculateCategoriesDiscountsAsync(carts, discount),
                DiscountType.AssignedToManufacturers => await CalculateManufacturersDiscountsAsync(carts, discount),
                DiscountType.AssignedToOrderSubTotal => CalculateOrderSubTotalDiscount(subTotal, discount),
                DiscountType.AssignedToOrderTotal => 0, // Will be calculated later
                _ => 0,
            };
        }

        private async Task<decimal> CalculateSkuDiscountsAsync(List<ShoppingCartItem> carts, Discount discount)
        {
            var discountAmount = 0m;

            foreach (var item in carts)
            {
                var product = await _productService.GetByIdAsync(item.ProductId);
                if (await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) == null) continue;

                var productQuantity = Math.Min(item.Quantity, discount.MaximumDiscountedQuantity ?? item.Quantity);
                var productDiscount = CalculateProductDiscount(product.Price, productQuantity, discount);
                discountAmount += productDiscount;
            }

            return discountAmount;
        }

        private async Task<decimal> CalculateCategoriesDiscountsAsync(List<ShoppingCartItem> carts, Discount discount)
        {
            var discountAmount = 0m;

            foreach (var item in carts)
            {
                var categoryIdList = (await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId))
                    .Select(x => x.CategoryId)
                    .ToList();

                foreach (var categoryId in categoryIdList)
                {
                    if (await _categoryService.GetDiscountAppliedToCategoryAsync(categoryId, discount.Id) == null) continue;

                    var product = await _productService.GetByIdAsync(item.ProductId);
                    var productQuantity = Math.Min(item.Quantity, discount.MaximumDiscountedQuantity ?? item.Quantity);
                    var productDiscount = CalculateProductDiscount(product.Price, productQuantity, discount);
                    discountAmount += productDiscount;
                    break;
                }
            }

            return discountAmount;
        }

        private async Task<decimal> CalculateManufacturersDiscountsAsync(List<ShoppingCartItem> carts, Discount discount)
        {
            var discountAmount = 0m;

            foreach (var item in carts)
            {
                var manufacturerIdList = (await _manufacturerService.GetProductManufacturerByProductIdAsync(item.ProductId))
                    .Select(x => x.ManufacturerId)
                    .ToList();

                foreach (var manufacturerId in manufacturerIdList)
                {
                    if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturerId, discount.Id) == null) continue;

                    var product = await _productService.GetByIdAsync(item.ProductId);
                    var productQuantity = Math.Min(item.Quantity, discount.MaximumDiscountedQuantity ?? item.Quantity);
                    var productDiscount = CalculateProductDiscount(product.Price, productQuantity, discount);
                    discountAmount += productDiscount;
                    break;
                }
            }

            return discountAmount;
        }

        private static decimal CalculateProductDiscount(decimal productPrice, int productQuantity, Discount discount)
        {
            var discountAmount = discount.UsePercentage
                ? discount.DiscountPercentage * (productPrice / 100)
                : discount.DiscountAmount;

            if (discount.MaximumDiscountAmount.HasValue)
            {
                discountAmount = Math.Min(discountAmount, discount.MaximumDiscountAmount.Value);
            }

            return discountAmount * productQuantity;
        }

        private decimal CalculateOrderSubTotalDiscount(decimal subTotal, Discount discount)
        {
            return CalculateDiscountAmount(subTotal, discount);
        }

        public async Task<decimal> CalculateOrderTotalDiscount(decimal orderTotal)
        {
            var discountAmount = 0m;
            var session = _httpContextAccessor.HttpContext.Session;
            var discounts = session.Get<List<string>>("Discount") ?? new List<string>();

            var maxOrderTotalDiscount = 0m;

            foreach (var discountCode in discounts)
            {
                var discount = await _discountService.GetDiscountByCode(discountCode);
                if (discount?.DiscountType != DiscountType.AssignedToOrderTotal) continue;

                var tempDiscountAmount = CalculateDiscountAmount(orderTotal, discount);

                if (discount.IsCumulative)
                {
                    discountAmount += tempDiscountAmount;
                }
                else
                {
                    maxOrderTotalDiscount = Math.Max(maxOrderTotalDiscount, tempDiscountAmount);
                }
            }

            return discountAmount + maxOrderTotalDiscount;
        }

        private static decimal CalculateDiscountAmount(decimal amount, Discount discount)
        {
            var discountAmount = discount.UsePercentage
                ? discount.DiscountPercentage * (amount / 100)
                : discount.DiscountAmount;

            return discount.MaximumDiscountAmount.HasValue
                ? Math.Min(discountAmount, discount.MaximumDiscountAmount.Value)
                : discountAmount;
        }

        public decimal CalculateTax(decimal subTotalAfterDiscount, decimal taxRates)
        {
            decimal taxRate = taxRates / 10;
            return subTotalAfterDiscount * taxRate;
        }

        public decimal CalculateShippingFee(List<ShoppingCartItem> carts)
        {
            return 0;
        }

        public async Task<decimal> CalculateSubTotalAsync(List<ShoppingCartItem> carts)
        {
            decimal subTotal = 0;
            foreach (var item in carts)
            {
                var product = await _productService.GetByIdAsync(item.ProductId);
                subTotal += await CalculateAdjustedPriceAsync(product, item);
            }
            return subTotal;
        }
    }

}

