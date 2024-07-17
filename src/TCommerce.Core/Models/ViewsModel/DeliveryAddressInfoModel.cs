using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.ViewsModel
{
    public class AddressInfoModel : BaseEntity
    {
        public string? FullName { get; set; }
        public string? AddressFull { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsDefault { get; set; }
    }
}
