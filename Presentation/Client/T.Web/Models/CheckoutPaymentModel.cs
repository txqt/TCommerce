namespace T.Web.Models
{
    public class CheckoutPaymentModel
    {
        public CheckoutPaymentModel()
        {
            ShippingAddress = new CheckoutShippingAddressModel();
            Cart = new ShoppingCartModel();
        }

        public CheckoutShippingAddressModel ShippingAddress { get; set; }
        public ShoppingCartModel Cart { get; set; }
    }
}
