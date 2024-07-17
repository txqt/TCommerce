using TCommerce.Core.Models.Common;

namespace TCommerce.Core.Models.Users
{
    public partial class UserAddressMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier
        /// </summary>
        public int AddressId { get; set; }
    }
}
