using TCommerce.Core.Models.Security;
using TCommerce.Core.Models.Users;

namespace TCommerce.Services.DataSeederServices
{
    public class UserRoleMappingModel
    {
        public required List<Role> Roles { get; set; }
        public required List<User> Users { get; set; }
    }
}
