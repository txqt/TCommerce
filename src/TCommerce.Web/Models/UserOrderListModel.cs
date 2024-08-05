using TCommerce.Core.Models.Common;
using TCommerce.Core.Models.Orders;

namespace TCommerce.Web.Models
{
    public class UserOrderListModel
    {
        public UserOrderListModel()
        {
            Orders = new List<OrderDetailsModel>();
            RecurringPaymentErrors = new List<string>();
        }

        public List<OrderDetailsModel> Orders { get; set; }
        public List<string> RecurringPaymentErrors { get; set; }

        #region Nested classes

        public class OrderDetailsModel : BaseEntity
        {
            public string OrderTotal { get; set; }
            public OrderStatus OrderStatusEnum { get; set; }
            public string OrderStatus { get; set; }
            public string PaymentStatus { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        #endregion
    }
}