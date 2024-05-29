using T.Library.Model.ViewsModel;

namespace T.Web.Models
{
    public class CheckoutShippingAddressModel
    {
        public CheckoutShippingAddressModel()
        {
            ExistingAddresses = new List<AddressInfoModel>();
            DefaultShippingAddress = new AddressInfoModel();
            NewShippingAddress = new AddressModel();
        }

        public List<AddressInfoModel> ExistingAddresses { get; set; }
        public AddressInfoModel DefaultShippingAddress { get; set; }
        public AddressModel NewShippingAddress { get; set; }
    }
}
