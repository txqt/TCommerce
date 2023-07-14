using T.Library.Model.Security;
using T.Library.Model.Users;

namespace T.WebApi.Services.DataSeederService
{
    public class RolePermissionMappingSeedModel
    {
        public List<Role> Roles { get; set; }
        public List<PermissionRecord> PermissionRecords { get; set; }
    }
}
