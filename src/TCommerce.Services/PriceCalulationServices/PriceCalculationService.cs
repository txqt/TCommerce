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

        public PriceCalculationService(IProductAttributeConverter productAttributeConverter, IProductAttributeService productAttributeService, IProductService productService, IHttpContextAccessor httpContextAccessor, IDiscountService discountService, ICategoryService categoryService, IManufacturerService manufacturerService)
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

            if (sci.AttributeJson is not null)
            {
                foreach (var selectedAttribute in _productAttributeConverter.ConvertToObject(sci.AttributeJson))
                {
                    foreach (var attributeValueId in selectedAttribute.ProductAttributeValueIds)
                    {
                        var attributeValue = await _productAttributeService.GetProductAttributeValuesByIdAsync(attributeValueId);

                        if (attributeValue.PriceAdjustment > 0)
                        {
                            if (attributeValue.PriceAdjustmentUsePercentage)
                            {
                                adjustedPrice += (attributeValue.PriceAdjustment * product.Price / 100) * sci.Quantity;
                            }
                            else
                            {
                                adjustedPrice += attributeValue.PriceAdjustment * sci.Quantity;
                            }
                        }
                    }
                }
            }

            return adjustedPrice;
        }
        public decimal CalculateAdjustedPrice(Product product, decimal priceAdjustment, bool priceAdjustmentUsePercentage = false)
        {
            var productPrice = product.Price;
            if (priceAdjustmentUsePercentage)
            {
                return (productPrice += (productPrice *= priceAdjustment / 100));
            }

            return (productPrice + priceAdjustment);
        }
        public async Task<decimal> CalculateDiscountsAsync(List<ShoppingCartItem> carts, decimal subTotal)
        {
            decimal discountAmount = 0;
            var discountSessionKey = "Discount";
            var session = _httpContextAccessor.HttpContext.Session;
            var discounts = session.Get<List<string>>(discountSessionKey) ?? new List<string>();

            // Dictionaries to store the maximum non-cumulative discounts
            var maxDiscounts = new Dictionary<DiscountType, decimal>();

            if (discounts != null)
            {
                foreach (var d in discounts)
                {
                    var discount = await _discountService.GetDiscountByCode(d);
                    if (discount != null && (await _discountService.CheckValidDiscountAsync(discount)).Success)
                    {
                        decimal tempDiscountAmount = 0;
                        switch (discount.DiscountType)
                        {
                            case DiscountType.AssignedToSkus:
                                tempDiscountAmount = await CalculateSkuDiscountsAsync(carts, discount);
                                break;
                            case DiscountType.AssignedToCategories:
                                tempDiscountAmount = await CalculateCategoriesDiscountsAsync(carts, discount);
                                break;
                            case DiscountType.AssignedToManufacturers:
                                tempDiscountAmount = await CalculateManufacturersDiscountsAsync(carts, discount);
                                break;
                            case DiscountType.AssignedToOrderSubTotal:
                                tempDiscountAmount = CalculateOrderSubTotalDiscount(subTotal, discount);
                                break;
                            case DiscountType.AssignedToOrderTotal:
                                // CalculateOrderTotalDiscount will be called later with the final total amount
                                break;
                        }

                        if (discount.IsCumulative)
                        {
                            discountAmount += tempDiscountAmount;
                        }
                        else
                        {
                            if (maxDiscounts.ContainsKey(discount.DiscountType))
                            {
                                if (tempDiscountAmount > maxDiscounts[discount.DiscountType])
                                {
                                    maxDiscounts[discount.DiscountType] = tempDiscountAmount;
                                }
                            }
                            else
                            {
                                maxDiscounts[discount.DiscountType] = tempDiscountAmount;
                            }
                        }
                    }
                    else
                    {
                        discounts.Remove(d);
                        session.Set(discountSessionKey, discounts);
                    }
                }
            }

            // Add the non-cumulative discounts to the total discount amount
            discountAmount += maxDiscounts.Values.Sum();

            return discountAmount;
        }
        private async Task<decimal> CalculateSkuDiscountsAsync(List<ShoppingCartItem> carts, Discount discount)
        {
            decimal discountAmount = 0;

            foreach (var item in carts)
            {
                var product = await _productService.GetByIdAsync(item.ProductId);
                var mapping = await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id);

                var productQuantityToDiscount = item.Quantity;

                if (discount.MaximumDiscountedQuantity.HasValue && productQuantityToDiscount > discount.MaximumDiscountedQuantity)
                {
                    productQuantityToDiscount = discount.MaximumDiscountedQuantity.Value;
                }

                if (mapping != null)
                {
                    if (discount.UsePercentage && discount.DiscountPercentage > 0)
                    {
                        var discountAmountTemp = discount.DiscountPercentage * (product.Price / 100);

                        if (discount.MaximumDiscountAmount.HasValue && discountAmountTemp > discount.MaximumDiscountAmount)
                        {
                            discountAmountTemp = discount.MaximumDiscountAmount.Value;
                        }

                        discountAmount += discountAmountTemp * productQuantityToDiscount;
                    }
                    else
                    {
                        discountAmount += discount.DiscountAmount * productQuantityToDiscount;
                    }
                }
            }

            return discountAmount;
        }
        private async Task<decimal> CalculateCategoriesDiscountsAsync(List<ShoppingCartItem> carts, Discount discount)
        {
            decimal discountAmount = 0;

            foreach (var item in carts)
            {
                var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(item.ProductId);
                var categoryIdList = productCategories.Select(x => x.CategoryId).ToList();
                foreach (var c in categoryIdList)
                {
                    var category = await _categoryService.GetCategoryByIdAsync(c);
                    if (category is not null)
                    {
                        var mapping = await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id);
                        if (mapping != null)
                        {
                            var product = await _productService.GetByIdAsync(item.ProductId);
                            var productQuantity = item.Quantity;

                            if (discount.MaximumDiscountedQuantity.HasValue && productQuantity > discount.MaximumDiscountedQuantity)
                            {
                                productQuantity = discount.MaximumDiscountedQuantity.Value;
                            }

                            if (discount.UsePercentage && discount.DiscountPercentage > 0)
                            {
                                var discountAmountTemp = discount.DiscountPercentage * (product.Price / 100);
                                if (discount.MaximumDiscountAmount.HasValue && discountAmountTemp > discount.MaximumDiscountAmount)
                                {
                                    discountAmountTemp = discount.MaximumDiscountAmount.Value;
                                }
                                discountAmount += discountAmountTemp * productQuantity;
                            }
                            else
                            {
                                discountAmount += discount.DiscountAmount * productQuantity;
                            }
                            break;
                        }
                    }
                }
            }

            return discountAmount;
        }
        private async Task<decimal> CalculateManufacturersDiscountsAsync(List<ShoppingCartItem> carts, Discount discount)
        {
            decimal discountAmount = 0;

            foreach (var item in carts)
            {
                var productManufacturers = await _manufacturerService.GetProductManufacturerByProductIdAsync(item.ProductId);
                var manufacturerIdList = productManufacturers.Select(x => x.ManufacturerId).ToList();
                foreach (var c in manufacturerIdList)
                {
                    var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(c);
                    if (manufacturer is not null)
                    {
                        var mapping = await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufacturer.Id, discount.Id);
                        if (mapping != null)
                        {
                            var product = await _productService.GetByIdAsync(item.ProductId);
                            var productQuantity = item.Quantity;

                            if (discount.MaximumDiscountedQuantity.HasValue && productQuantity > discount.MaximumDiscountedQuantity)
                            {
                                productQuantity = discount.MaximumDiscountedQuantity.Value;
                            }

                            if (discount.UsePercentage && discount.DiscountPercentage > 0)
                            {
                                var discountAmountTemp = discount.DiscountPercentage * (product.Price / 100);
                                if (discount.MaximumDiscountAmount.HasValue && discountAmountTemp > discount.MaximumDiscountAmount)
                                {
                                    discountAmountTemp = discount.MaximumDiscountAmount.Value;
                                }
                                discountAmount += discountAmountTemp * productQuantity;
                            }
                            else
                            {
                                discountAmount += discount.DiscountAmount * productQuantity;
                            }
                            break;
                        }
                    }
                }
            }

            return discountAmount;
        }
        public decimal CalculateOrderSubTotalDiscount(decimal subTotal, Discount discount)
        {
            decimal discountAmount = 0;

            if (discount.UsePercentage && discount.DiscountPercentage > 0)
            {
                discountAmount = discount.DiscountPercentage * (subTotal / 100);
                if (discount.MaximumDiscountAmount.HasValue && discountAmount > discount.MaximumDiscountAmount)
                {
                    discountAmount = discount.MaximumDiscountAmount.Value;
                }
            }
            else
            {
                discountAmount = discount.DiscountAmount;
                if (discount.MaximumDiscountAmount.HasValue && discountAmount > discount.MaximumDiscountAmount)
                {
                    discountAmount = discount.MaximumDiscountAmount.Value;
                }
            }

            return discountAmount;
        }
        public async Task<decimal> CalculateOrderTotalDiscount(decimal orderTotal)
        {
            decimal discountAmount = 0;
            var discountSessionKey = "Discount";
            var session = _httpContextAccessor.HttpContext.Session;
            var discounts = session.Get<List<string>>(discountSessionKey) ?? new List<string>();

            var maxOrderTotalDiscount = 0m;

            if (discounts != null)
            {
                foreach (var d in discounts)
                {
                    var discount = await _discountService.GetDiscountByCode(d);
                    if (discount != null && discount.DiscountType == DiscountType.AssignedToOrderTotal)
                    {
                        var tempDiscountAmount = CalculateOrderTotalDiscount(orderTotal, discount);
                        if (discount.IsCumulative)
                        {
                            discountAmount += tempDiscountAmount;
                        }
                        else
                        {
                            if (tempDiscountAmount > maxOrderTotalDiscount)
                            {
                                maxOrderTotalDiscount = tempDiscountAmount;
                            }
                        }
                    }
                }
            }

            return discountAmount;
        }
        private decimal CalculateOrderTotalDiscount(decimal orderTotal, Discount discount)
        {
            decimal discountAmount = 0;

            if (discount.UsePercentage && discount.DiscountPercentage > 0)
            {
                discountAmount = discount.DiscountPercentage * (orderTotal / 100);
                if (discount.MaximumDiscountAmount.HasValue && discountAmount > discount.MaximumDiscountAmount)
                {
                    discountAmount = discount.MaximumDiscountAmount.Value;
                }
            }
            else
            {
                discountAmount = discount.DiscountAmount;
                if (discount.MaximumDiscountAmount.HasValue && discountAmount > discount.MaximumDiscountAmount)
                {
                    discountAmount = discount.MaximumDiscountAmount.Value;
                }
            }

            return discountAmount;
        }
        public decimal CalculateTax(decimal subTotalAfterDiscount)
        {
            // Add logic to calculate tax
            const decimal taxRate = 0.10m; // 10% tax rate for example
            return subTotalAfterDiscount * taxRate;
        }
        public decimal CalculateShippingFee(List<ShoppingCartItem> carts)
        {
            // Add logic to calculate shipping fee
            return 50_000; // Fixed shipping fee for example
        }
    }
}
