using Microsoft.EntityFrameworkCore;
using T.Library.Model.Common;
using T.Library.Model.Response;
using T.Library.Model.Security;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Extensions;

namespace T.WebApi.Services.PermissionRecordServices
{
    public interface IPermissionRecordService
    {
        Task<List<PermissionRecord>> GetAllPermissionRecordAsync();
        Task<ServiceResponse<PermissionRecord>> GetPermissionRecordByIdAsync(int permissionRecordId);
        Task<ServiceResponse<PermissionRecord>> GetPermissionRecordByNameAsync(string permissionRecordName);
        Task<ServiceResponse<bool>> CreateOrEditAsync(PermissionRecord permissionRecord);
        Task<ServiceResponse<bool>> DeletePermissionRecordByIdAsync(int id);
    }
    public class PermissionRecordService : IPermissionRecordService
    {
        private readonly DatabaseContext _context;
        public PermissionRecordService(DatabaseContext databaseContext)
        {
            _context = databaseContext;
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
            return await _context.PermissionRecords.ToListAsync();
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

        public async Task<ServiceResponse<PermissionRecord>> GetPermissionRecordByNameAsync(string permissionRecordName)
        {
            var permissionRecord = await _context.PermissionRecords.FirstOrDefaultAsync(x => x.Name == permissionRecordName);

            var response = new ServiceResponse<PermissionRecord>
            {
                Data = permissionRecord,
                Success = true
            };
            return response;
        }
    }
}
