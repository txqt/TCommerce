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
        public string? SubTotal { get; set; }
        public string? SubTotalDiscount { get; set; }
        public string? OrderTotalDiscount { get; set; }
        public string? OrderTotal { get; set; }
        public string? Tax { get; set; }
        public string? ShippingFee { get; set; }
        public string PaymentMethodSystemName { get; set; }
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
