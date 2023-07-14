using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Extensions;

namespace T.WebApi.Services.DataSeederService
{
    public class RolePermissionMappingDataSeed : SingletonBase<RolePermissionMappingDataSeed>
    {
        public List<RolePermissionMappingSeedModel> GetAll()
        {
            return new List<RolePermissionMappingSeedModel>()
            {
                new RolePermissionMappingSeedModel()
                {
                    Roles = new List<Role>()
                    {
                        new Role(RoleName.Employee)
                    },
                    PermissionRecords = new List<PermissionRecord>()
                    {
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.AccessAdminPanel
                        },
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.ManageProducts
                        },
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.ManageCategories
                        },
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.ManageAttributes
                        }
                    }
                },
                new RolePermissionMappingSeedModel()
                {
                    Roles = new List<Role>()
                    {
                        new Role(RoleName.Admin)
                    },
                    PermissionRecords = new List<PermissionRecord>()
                    {
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.AccessAdminPanel
                        },
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.ManageProducts
                        },
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.ManageCategories
                        },
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.ManageAttributes
                        },
                        new PermissionRecord()
                        {
                            SystemName = PermissionSystemName.ManageCustomers
                        }
                    }
                }
            };
        }
    }
}
