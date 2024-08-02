using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Orders
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal? ItemWeight { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal OriginalProductCost { get; set; }
        public string? AttributeInfo { get; set; }
        public string? AttributeJson { get; set; }
    }
}
