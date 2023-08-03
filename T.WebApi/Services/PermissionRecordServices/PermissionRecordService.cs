using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using T.Library.Model.Common;
using T.Library.Model.Response;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;
using T.WebApi.Services.UserServices;

namespace T.WebApi.Services.PermissionRecordServices
{
    public interface IPermissionRecordService
    {
        Task<List<PermissionRecord>> GetAllPermissionRecordAsync();
        Task<ServiceResponse<PermissionRecord>> GetPermissionRecordByIdAsync(int permissionRecordId);
        Task<ServiceResponse<PermissionRecord>> GetPermissionRecordBySystemNameAsync(string permissionRecordSystemName);
        Task<ServiceResponse<List<PermissionRecord>>> GetPermissionRecordsByCustomerRoleIdAsync(Guid roleId);
        Task<ServiceResponse<bool>> CreateOrEditAsync(PermissionRecord permissionRecord);
        Task<ServiceResponse<bool>> DeletePermissionRecordByIdAsync(int id);
        Task<bool> AuthorizeAsync(PermissionRecord permissionRecord);
    }
    public class PermissionRecordService : IPermissionRecordService
    {
        private readonly DatabaseContext _context;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        public PermissionRecordService(DatabaseContext databaseContext, IUserService userService, UserManager<User> userManager)
        {
            _context = databaseContext;
            _userService = userService;
            _userManager = userManager;
        }


        public async Task<bool> AuthorizeAsync(PermissionRecord permissionRecord)
        {
            if (permissionRecord is null)
                return false;

            return await AuthorizeAsync(permissionRecord.SystemName, (await _userService.GetCurrentUser()).Data);
        }
        public async Task<bool> AuthorizeAsync(string permissionSystemName, User user)
        {
            if (user is null)
                return false;

            if (string.IsNullOrEmpty(permissionSystemName))
                return false;

            var userRoles = await _userService.GetRolesByUserAsync(user);
            foreach (var role in userRoles)
            {
                if(await AuthorizeAsync(permissionSystemName, role.Id))
                {
                    return true;
                } 
            }
            return false;
        }
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

        public async Task<ServiceResponse<bool>> CreateOrEditAsync(PermissionRecord permissionRecord)
        {
            var permissionRecordTable = await _context.PermissionRecords.FirstOrDefaultAsync(x => x.Id == permissionRecord.Id);
            if (permissionRecordTable == null)
            {
                _context.PermissionRecords.Add(permissionRecord);
            }
            else
            {
                if (_context.IsRecordUnchanged(permissionRecordTable, permissionRecord))
                {
                    return new ServiceErrorResponse<bool>("Data is unchanged");
                }

                permissionRecordTable.Name = permissionRecord.Name;
                permissionRecordTable.SystemName = permissionRecord.SystemName;
                permissionRecordTable.Category = permissionRecord.Category;
            }
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ServiceErrorResponse<bool>("Create permission record failed");
            }
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> DeletePermissionRecordByIdAsync(int id)
        {
            try
            {
                var permissionRecord = await _context.PermissionRecords.FirstOrDefaultAsync(x => x.Id == id);

                _context.PermissionRecords.Remove(permissionRecord);

                await _context.SaveChangesAsync();

                return new ServiceSuccessResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceErrorResponse<bool>(message: ex.Message);
            }
        }

        public async Task<List<PermissionRecord>> GetAllPermissionRecordAsync()
        {
            List<PermissionRecord> permissionList = new List<PermissionRecord>();

            Type type = typeof(DefaultPermission);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(PermissionRecord))
                {
                    PermissionRecord permission = (PermissionRecord)field.GetValue(null);
                    permissionList.Add(permission);
                }
            }

            return permissionList;
        }

        public async Task<ServiceResponse<PermissionRecord>> GetPermissionRecordByIdAsync(int permissionRecordId)
        {
            var permissionRecord = await _context.PermissionRecords.FirstOrDefaultAsync(x => x.Id == permissionRecordId);

            var response = new ServiceResponse<PermissionRecord>
            {
                Data = permissionRecord,
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<PermissionRecord>> GetPermissionRecordBySystemNameAsync(string permissionRecordSystenName)
        {
            var permissionRecord = await _context.PermissionRecords.FirstOrDefaultAsync(x => x.SystemName == permissionRecordSystenName);

            var response = new ServiceResponse<PermissionRecord>
            {
                Data = permissionRecord,
                Success = true
            };
            return response;
        }

        public async Task<ServiceResponse<List<PermissionRecord>>> GetPermissionRecordsByCustomerRoleIdAsync(Guid roleId)
        {
            var permissionRecords = _context.PermissionRecordUserRoleMappings.Where(x => x.RoleId == roleId).Select(x=>x.PermissionRecord).ToListAsync();

            var response = new ServiceResponse<List<PermissionRecord>>
            {
                Data = await permissionRecords,
                Success = true
            };
            return response;
        }
    }
}
