using TCommerce.Core.Models.Payments;

namespace TCommerce.Web.Models
{
    public class CheckoutPaymentModel
    {
        public CheckoutPaymentModel()
        {
            ShippingAddress = new CheckoutShippingAddressModel();
            Cart = new ShoppingCartModel();
        }

        public CheckoutShippingAddressModel ShippingAddress { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public ShoppingCartModel Cart { get; set; }
    }
}
