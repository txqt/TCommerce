using Microsoft.CodeAnalysis;
using System.Net;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.Web.Helpers;

namespace T.Web.Services.SecurityServices
{
    public class SecurityService : HttpClientHelper, ISecurityService
    {
        public SecurityService(HttpClient httpClient) : base(httpClient)
        {

        }

        public async Task<List<Role>> GetRoles()
        {
            return await GetAsync<List<Role>>($"api/security/roles");
        }

        public async Task<List<PermissionRecord>> GetAllPermissionRecordAsync()
        {
            return await GetAsync<List<PermissionRecord>>($"api/security/permissions");
        }

        public async Task<Role> GetRoleByRoleId(string roleId)
        {
            return await GetAsync<Role>($"api/security/role/{roleId}");
        }

        public async Task<ServiceResponse<bool>> CreatePermissionMappingAsync(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping)
        {
            return await PostAsJsonAsync<ServiceResponse<bool>>($"api/security/permission-mapping", permissionRecordUserRoleMapping);
        }

        public async Task<ServiceResponse<bool>> DeletePermissionRecordByIdAsync(int permissionMapping)
        {
            throw new NotImplementedException();
        }

        public async Task<PermissionRecordUserRoleMapping> GetPermissionMappingAsync(string roleId, int permissionId)
        {
            return await GetAsync<PermissionRecordUserRoleMapping>($"api/security/role/{roleId}/permission/{permissionId}");
        }


        public Task<PermissionRecord> GetPermissionRecordByIdAsync(int permissionRecordId)
        {
            throw new NotImplementedException();
        }

        public async Task<PermissionRecord> GetPermissionRecordBySystemNameAsync(string permissionRecordSystemName)
        {
            return await GetAsync<PermissionRecord>($"api/security/permission/system-name/{permissionRecordSystemName}");
        }

        public Task<List<PermissionRecord>> GetPermissionRecordsByCustomerRoleIdAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> CreatePermissionRecord(PermissionRecord permissionRecord)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> UpdatePermissionRecord(PermissionRecord permissionRecord)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AuthorizeAsync(PermissionRecord permissionRecord)
        {
            return await PostAsJsonAsync<bool>($"api/security/authorize-permission", permissionRecord);
        }
        public async Task<bool> AuthorizeAsync(string permissionRecordSystemName)
        {
            return await GetAsync<bool>($"api/security/authorize-permission/system-name/{permissionRecordSystemName}");
        }

        [Obsolete]
        public Task<bool> AuthorizeAsync(string permissionSystemName, User user)
        {
            throw new NotImplementedException();
        }
        [Obsolete]
        public Task<bool> AuthorizeAsync(string permissionSystemName, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<bool>> DeletePermissionMappingByIdAsync(int mappingId)
        {
            return await DeleteAsync<ServiceResponse<bool>>($"api/security/permission-mapping/{mappingId}");
        }

    }
}
