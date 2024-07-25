using System.ComponentModel.DataAnnotations;
using TCommerce.Core.Models.Common;
using TCommerce.Web.Models;

namespace TCommerce.Web.Areas.Admin.Models.Orders
{
    public class OrderModel : BaseEntity
    {
        #region Ctor

        public OrderModel()
        {
            Items = new List<OrderItemModel>();
            UsedDiscounts = new List<UsedDiscountModel>();
            OrderNoteSearchModel = new OrderNoteSearchModel();
            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
        }

        #endregion

        #region Properties
        public Guid OrderGuid { get; set; }
        public string CustomOrderNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerInfo { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerIp { get; set; }
        public IList<UsedDiscountModel> UsedDiscounts { get; set; }
        public string OrderSubtotalInclTax { get; set; }
        public string OrderSubtotalExclTax { get; set; }
        public string OrderSubTotalDiscountInclTax { get; set; }
        public string OrderSubTotalDiscountExclTax { get; set; }
        public string OrderShippingInclTax { get; set; }
        public string OrderShippingExclTax { get; set; }
        public string PaymentMethodAdditionalFeeInclTax { get; set; }
        public string PaymentMethodAdditionalFeeExclTax { get; set; }
        public string Tax { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }
        public string OrderTotalDiscount { get; set; }
        public string OrderTotal { get; set; }
        public string RefundedAmount { get; set; }
        public string Profit { get; set; }

        //edit totals
        public decimal OrderSubtotalInclTaxValue { get; set; }
        public decimal OrderSubtotalExclTaxValue { get; set; }
        public decimal OrderSubTotalDiscountInclTaxValue { get; set; }
        public decimal OrderSubTotalDiscountExclTaxValue { get; set; }
        public decimal OrderShippingInclTaxValue { get; set; }
        public decimal OrderShippingExclTaxValue { get; set; }
        public decimal PaymentMethodAdditionalFeeInclTaxValue { get; set; }
        public decimal PaymentMethodAdditionalFeeExclTaxValue { get; set; }
        public decimal TaxValue { get; set; }
        public string TaxRatesValue { get; set; }
        public decimal OrderTotalDiscountValue { get; set; }
        public decimal OrderTotalValue { get; set; }


        //order status
        public string OrderStatus { get; set; }
        public int OrderStatusId { get; set; }

        //payment info
        public string PaymentStatus { get; set; }
        public int PaymentStatusId { get; set; }
        public string PaymentMethod { get; set; }

        public AddressModel ShippingAddress { get; set; }

        //billing info
        public AddressModel BillingAddress { get; set; }
        public IList<OrderItemModel> Items { get; set; }

        //creation date
        public DateTime CreatedOn { get; set; }


        //order notes
        public bool AddOrderNoteDisplayToCustomer { get; set; }
        public string AddOrderNoteMessage { get; set; }

        //refund info
        public decimal AmountToRefund { get; set; }
        public decimal MaxAmountToRefund { get; set; }

        //workflow info
        public bool CanCancelOrder { get; set; }
        public bool CanCapture { get; set; }
        public bool CanMarkOrderAsPaid { get; set; }
        public bool CanRefund { get; set; }
        public bool CanRefundOffline { get; set; }
        public bool CanPartiallyRefund { get; set; }
        public bool CanPartiallyRefundOffline { get; set; }
        public bool CanVoid { get; set; }
        public bool CanVoidOffline { get; set; }

        public OrderNoteSearchModel OrderNoteSearchModel { get; set; }

        #endregion

        #region Nested Classes

        public class UsedDiscountModel : BaseEntity
        {
            public int DiscountId { get; set; }
            public string DiscountName { get; set; }
        }

        #endregion
    }
}
