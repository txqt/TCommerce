namespace TCommerce.Core.Models
{
    public class OrderTotalsModel
    {
        public OrderTotalsModel()
        {
        }

        public string SubTotal { get; set; }
        public string SubTotalDiscount { get; set; }
        public string Shipping { get; set; }
        public bool RequiresShipping { get; set; }
        public string SelectedShippingMethod { get; set; }
        public string Tax { get; set; }
        public string ShippingFee { get; set; }
        public bool HideShippingTotal { get; set; }
        public string SelectedPaymentMethod { get; set; }
        public string OrderTotalDiscount { get; set; }
        public string OrderTotal { get; set; }
    }
}
