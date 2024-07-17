using TCommerce.Core.Models.Security;

namespace TCommerce.Services.DataSeederServices
{
    public class RolePermissionMappingSeedModel
    {
        public List<Role>? Roles { get; set; }
        public List<PermissionRecord>? PermissionRecords { get; set; }
    }
}
