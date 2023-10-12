using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Response;
using T.Library.Model.Security;
using T.Library.Model.Users;

namespace T.Library.Model.Interface
{
    public interface ISecurityService
    {
        Task<List<PermissionRecord>> GetAllPermissionRecordAsync();
        Task<ServiceResponse<PermissionRecord>> GetPermissionRecordByIdAsync(int permissionRecordId);
        Task<ServiceResponse<PermissionRecord>> GetPermissionRecordBySystemNameAsync(string permissionRecordSystemName);
        Task<ServiceResponse<List<PermissionRecord>>> GetPermissionRecordsByCustomerRoleIdAsync(Guid roleId);
        Task<ServiceResponse<bool>> CreatePermissionRecord(PermissionRecord permissionRecord);
        Task<ServiceResponse<bool>> UpdatePermissionRecord(PermissionRecord permissionRecord);
        Task<ServiceResponse<bool>> DeletePermissionRecordByIdAsync(int id);
        Task<bool> AuthorizeAsync(PermissionRecord permissionRecord);
        Task<bool> AuthorizeAsync(string permissionSystemName, User user);
        Task<bool> AuthorizeAsync(string permissionSystemName, Guid roleId);
        Task<ServiceResponse<PermissionRecordUserRoleMapping>> GetPermissionMappingAsync(string roleId, int permissionId);
        Task<List<Role>> GetRoles();
        Task<ServiceResponse<Role>> GetRoleByRoleId(string roleId);
        Task<ServiceResponse<bool>> CreatePermissionMappingAsync(PermissionRecordUserRoleMapping permissionMapping);
        Task<ServiceResponse<bool>> DeletePermissionMappingByIdAsync(int mappingId);
    }
}
