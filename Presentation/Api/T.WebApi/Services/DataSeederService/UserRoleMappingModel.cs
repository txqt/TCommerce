using T.Library.Model.Security;
using T.Library.Model.Users;

namespace T.WebApi.Services.DataSeederService
{
    public class UserRoleMappingModel
    {
        public required List<Role> Roles { get; set; }
        public required List<User> Users { get; set; }
    }
}
