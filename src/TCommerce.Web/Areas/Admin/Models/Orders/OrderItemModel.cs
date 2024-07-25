using TCommerce.Core.Models.Common;

namespace TCommerce.Web.Areas.Admin.Models.Orders
{
    public class OrderItemModel : BaseEntity
    {
        #region Ctor

        public OrderItemModel()
        {
        }

        #endregion

        #region Properties

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string Sku { get; set; }

        public string PictureThumbnailUrl { get; set; }

        public string UnitPriceInclTax { get; set; }

        public string UnitPriceExclTax { get; set; }

        public decimal UnitPriceInclTaxValue { get; set; }

        public decimal UnitPriceExclTaxValue { get; set; }

        public int Quantity { get; set; }

        public string DiscountInclTax { get; set; }

        public string DiscountExclTax { get; set; }

        public decimal DiscountInclTaxValue { get; set; }

        public decimal DiscountExclTaxValue { get; set; }

        public string SubTotalInclTax { get; set; }

        public string SubTotalExclTax { get; set; }

        public decimal SubTotalInclTaxValue { get; set; }

        public decimal SubTotalExclTaxValue { get; set; }

        public string AttributeInfo { get; set; }

        #endregion
    }
}
