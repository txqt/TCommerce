using T.Library.Model.ViewsModel;

namespace T.Web.Models
{
    public class CheckoutShippingAddressModel
    {
        public CheckoutShippingAddressModel()
        {
            ExistingAddresses = new List<DeliveryAddressInfoModel>();
            DefaultShippingAddress = new DeliveryAddressInfoModel();
            NewShippingAddress = new DeliveryAddressModel();
        }

        public List<DeliveryAddressInfoModel> ExistingAddresses { get; set; }
        public DeliveryAddressInfoModel DefaultShippingAddress { get; set; }
        public DeliveryAddressModel NewShippingAddress { get; set; }
    }
}
