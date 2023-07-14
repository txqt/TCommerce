using T.Library.Model.Security;
using T.Library.Model.Users;

namespace T.WebApi.Services.DataSeederService
{
    public class UserRoleMappingModel
    {
        public List<Role> Roles { get; set; }
        public List<User> Users { get; set; }
    }
}
