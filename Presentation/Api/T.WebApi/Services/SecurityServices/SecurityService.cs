using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services.IRepositoryServices;
using T.WebApi.Services.UserServices;

namespace T.WebApi.Services.SecurityServices
{
    public class SecurityService : ISecurityService
    {
        private readonly IUserService _userService;
        private readonly IRepository<PermissionRecord> _permissionRepository;
        private readonly IRepository<PermissionRecordUserRoleMapping> _permissionMappingRepository;
        private readonly RoleManager<Role> _roleManager;
        public SecurityService(IUserService userService, IRepository<PermissionRecord> permissionRepository, IRepository<PermissionRecordUserRoleMapping> permissionMappingRepository, RoleManager<Role> roleManager)
        {
            _userService = userService;
            _permissionRepository = permissionRepository;
            _permissionMappingRepository = permissionMappingRepository;
            this._roleManager = roleManager;
        }


        /// <summary>
        /// Kiểm tra xem một đối tượng PermissionRecord có được phép thực hiện hay không.
        /// </summary>
        /// <param name="permissionRecord">Đối tượng PermissionRecord cần kiểm tra.</param>
        /// <returns>Trả về true nếu có quyền thực hiện, ngược lại trả về false.</returns>
        public async Task<bool> AuthorizeAsync(PermissionRecord permissionRecord)
        {
            if (permissionRecord is null)
                return false;

            return await AuthorizeAsync(permissionRecord.SystemName, (await _userService.GetCurrentUser()).Data);
        }
        public async Task<bool> AuthorizeAsync(string permissionSystemname)
        {
            if (string.IsNullOrEmpty(permissionSystemname))
                return false;

            if((await GetPermissionRecordBySystemNameAsync(permissionSystemname)) is null)
                return false;

            return await AuthorizeAsync(permissionSystemname, (await _userService.GetCurrentUser()).Data);
        }

        /// <summary>
        /// Kiểm tra xem một người dùng có được phép thực hiện một quyền cụ thể hay không.
        /// </summary>
        /// <param name="permissionSystemName">Tên hệ thống của quyền cần kiểm tra.</param>
        /// <param name="user">Người dùng cần kiểm tra quyền.</param>
        /// <returns>Trả về true nếu có quyền thực hiện, ngược lại trả về false.</returns>
        public async Task<bool> AuthorizeAsync(string permissionSystemName, User user)
        {
            if (user is null)
                return false;

            if (string.IsNullOrEmpty(permissionSystemName))
                return false;

            var userRoles = await _userService.GetRolesByUserAsync(user);
            foreach (var role in userRoles)
            {
                if (await AuthorizeAsync(permissionSystemName, role.Id))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra xem một role có được phép thực hiện một quyền cụ thể hay không.
        /// </summary>
        /// <param name="permissionSystemName">Tên hệ thống của quyền cần kiểm tra.</param>
        /// <param name="roleId">ID của role cần kiểm tra quyền.</param>
        /// <returns>Trả về true nếu có quyền thực hiện, ngược lại trả về false.</returns>
        public async Task<bool> AuthorizeAsync(string permissionSystemName, Guid roleId)
        {
            if (permissionSystemName is null)
                return false;

            var permissions = await GetPermissionRecordsByCustomerRoleIdAsync(roleId);
            foreach (var permission in permissions.Data)
            {
                if (permission.SystemName.Equals(permissionSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }


        public async Task<ServiceResponse<bool>> CreatePermissionRecord(PermissionRecord permissionRecord)
        {
            try
            {
                await _permissionRepository.CreateAsync(permissionRecord);
                return new ServiceErrorResponse<bool>("Create permission record failed");
            }catch(Exception ex)
            {
                return new ServiceSuccessResponse<bool>() { Message = ex.Message};
            }
        }

        public async Task<ServiceResponse<bool>> DeletePermissionRecordByIdAsync(int id)
        {
            try
            {
                await _permissionRepository.DeleteAsync(id);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(message: ex.Message);
            }
        }

        public async Task<List<PermissionRecord>> GetAllPermissionRecordAsync()
        {
            return await _permissionRepository.Table.ToListAsync();
        }

        public async Task<ServiceResponse<PermissionRecord>> GetPermissionRecordByIdAsync(int permissionRecordId)
        {
            var permissionRecord = await _permissionRepository.Table.FirstOrDefaultAsync(x => x.Id == permissionRecordId);

            var response = new ServiceResponse<PermissionRecord>
            {
                Data = permissionRecord,
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<PermissionRecord>> GetPermissionRecordBySystemNameAsync(string permissionRecordSystenName)
        {
            var permissionRecord = await _permissionRepository.Table.FirstOrDefaultAsync(x => x.SystemName == permissionRecordSystenName);

            var response = new ServiceResponse<PermissionRecord>
            {
                Data = permissionRecord,
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<List<PermissionRecord>>> GetPermissionRecordsByCustomerRoleIdAsync(Guid roleId)
        {
            var permissionRecords = await _permissionMappingRepository.Table.Where(x => x.RoleId == roleId).Select(x=>x.PermissionRecord).ToListAsync();

            var response = new ServiceResponse<List<PermissionRecord>>
            {
                Data = permissionRecords,
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdatePermissionRecord(PermissionRecord permissionRecord)
        {
            try
            {
                await _permissionRepository.UpdateAsync(permissionRecord);
                return new ServiceSuccessResponse<bool>();
            }
            catch(Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message};
            }
        }

        public async Task<ServiceResponse<PermissionRecordUserRoleMapping>> GetPermissionMappingAsync(string roleId, int permissionId)
        {
            var permissionMapping = await _permissionMappingRepository.Table.Where(x => x.RoleId.ToString() == roleId && x.PermissionRecordId == permissionId).FirstOrDefaultAsync();
            return new ServiceSuccessResponse<PermissionRecordUserRoleMapping>() { Data = permissionMapping };
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _roleManager.Roles.Include(r => r.PermissionRecordUserRoleMappings).ThenInclude(p => p.PermissionRecord).ToListAsync();
        }

        public async Task<ServiceResponse<Role>> GetRoleByRoleId(string roleId)
        {
            var role = await _roleManager.Roles.Include(r => r.PermissionRecordUserRoleMappings).FirstOrDefaultAsync(r => r.Id.ToString() == roleId);
            return new ServiceSuccessResponse<Role>() { Data = role };
        }

        public async Task<ServiceResponse<bool>> CreatePermissionMappingAsync(PermissionRecordUserRoleMapping permissionMapping)
        {
            try
            {
                await _permissionMappingRepository.CreateAsync(permissionMapping);

                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<bool>> DeletePermissionMappingByIdAsync(int mappingId)
        {
            try
            {
                await _permissionMappingRepository.DeleteAsync(mappingId);
                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>() { Message = ex.Message };
            }
        }
    }
}
