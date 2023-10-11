//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Security;
//using System.Text;
//using System.Threading.Tasks;
//using T.Library.Model.Response;
//using T.Library.Model.Security;

//namespace T.Library.Model.Interface
//{
//    public interface IManageAccessControlService
//    {
//        Task<List<Role>> GetRoles();
//        Task<ServiceResponse<Role>> GetRoleByRoleId(string roleId);
//        Task<List<PermissionRecord>> GetPermissionRecords();
//        Task<ServiceResponse<PermissionRecordUserRoleMapping>> GetPermissionRoleMappingAsync(string roleId, int permissionId);
//        Task<ServiceResponse<bool>> CreatePermissionRoleAsync(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping);
//        Task<ServiceResponse<bool>> DeletePermissionRoleByIdAsync(int permissionMapping);
//    }
//}
