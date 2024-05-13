using T.Library.Model.Security;

namespace T.WebApi.Services.DataSeederService
{
    public class RolePermissionMappingSeedModel
    {
        public List<Role>? Roles { get; set; }
        public List<PermissionRecord>? PermissionRecords { get; set; }
    }
}
