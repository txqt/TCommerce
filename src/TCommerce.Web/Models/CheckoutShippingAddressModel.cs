using TCommerce.Core.Models.ViewsModel;

namespace TCommerce.Web.Models
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
