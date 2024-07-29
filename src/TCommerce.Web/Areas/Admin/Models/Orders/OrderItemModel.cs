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

        public decimal Price { get; set; }

        public decimal DiscountAmount { get; set; }

        public int Quantity { get; set; }

        public string AttributeInfo { get; set; }

        #endregion
    }
}
