//using T.Library.Model.Roles.RoleName;
//using T.Library.Model.Security;
//using T.Library.Model.Users;
//using T.WebApi.Extensions;
//using T.WebApi.Services.PermissionRecordServices;

//namespace T.WebApi.Services.DataSeederService
//{
//    public class RolePermissionMappingDataSeed : SingletonBase<RolePermissionMappingDataSeed>
//    {
//        public async Task<List<RolePermissionMappingSeedModel>> GetAll()
//        {

//            return new List<RolePermissionMappingSeedModel>()
//            {
//                new RolePermissionMappingSeedModel()
//                {
//                    Roles = new List<Role>()
//                    {
//                        new Role(RoleName.Employee)
//                    },
//                    PermissionRecords = new List<PermissionRecord>()
//                    {
//                        DefaultPermission.AccessAdminPanel,
//                        DefaultPermission.ManageProducts,
//                        DefaultPermission.ManageCategories,
//                        DefaultPermission.ManageAttributes,
//                    }
//                },
//                new RolePermissionMappingSeedModel()
//                {
//                    Roles = new List<Role>()
//                    {
//                        new Role(RoleName.Admin)
//                    },
//                    PermissionRecords = await _permissionRecordService.GetAllPermissionRecordAsync()
//                }
//            };
//        }
//    }
//}
