using System.ComponentModel.DataAnnotations.Schema;
using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Payments;

namespace TCommerce.Core.Models.Orders
{
    public class Order : BaseEntity, ISoftDeletedEntity
    {
        public Guid OrderGuid { get; set; }
        public Guid UserId { get; set; }
        public int? BillingAddressId { get; set; }
        public int? ShippingAddressId { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentStatusId { get; set; }
        public decimal OrderSubtotalInclTax { get; set; }
        public decimal OrderSubtotalExclTax { get; set; }
        public decimal OrderSubTotalDiscountInclTax { get; set; }
        public decimal OrderSubTotalDiscountExclTax { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTotal { get; set; }
        public string? TaxRates { get; set; }
        public string? OrderTax { get; set; }
        public string? ShippingFee { get; set; }
        public decimal OrderShippingInclTax { get; set; }
        public decimal OrderShippingExclTax { get; set; }
        public string PaymentMethodSystemName { get; set; }
        public decimal RefundedAmount { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool Deleted { get; set; }

        [NotMapped]
        public OrderStatus OrderStatus
        {
            get => (OrderStatus)OrderStatusId;
            set => OrderStatusId = (int)value;
        }

        [NotMapped]
        public PaymentStatus PaymentStatus
        {
            get => (PaymentStatus)PaymentStatusId;
            set => PaymentStatusId = (int)value;
        }
    }
}
