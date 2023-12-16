//using Microsoft.EntityFrameworkCore;
//using T.Library.Model.Response;
//using T.Library.Model.Security;
//using T.WebApi.Database.ConfigurationDatabase;

//namespace T.WebApi.Services.PermissionRecordUserRoleMappingServices
//{
//    public interface IPermissionRecordUserRoleMappingService
//    {
//        Task<List<PermissionRecordUserRoleMapping>> GetAllPermissionRecordUserRoleMappingAsync();
//        Task<PermissionRecordUserRoleMapping> GetPermissionRecordUserRoleMappingByIdAsync(int id);
//        Task<PermissionRecordUserRoleMapping> GetByPermissionRecordIdAsync(int permissionRecordId);
//        Task<ServiceResponse<bool>> CreateOrEditAsync(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping);
//        Task<ServiceResponse<bool>> DeletePermissionRecordUserRoleMappingByIdAsync(int id);
//    }
//    public class PermissionRecordUserRoleMappingService : IPermissionRecordUserRoleMappingService
//    {
//        private readonly DatabaseContext _context;
//        public PermissionRecordUserRoleMappingService(DatabaseContext context)
//        {
//            _context = context;
//        }

//        public async Task<ServiceResponse<bool>> CreateOrEditAsync(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping)
//        {
//            var mappingTable = await _context.PermissionRecordUserRoleMappings.FirstOrDefaultAsync(x => x.Id == permissionRecordUserRoleMapping.Id);
//            if (mappingTable == null) 
//            {
//                _context.PermissionRecordUserRoleMappings.Add(permissionRecordUserRoleMapping);
//            }
//            else
//            {
//                mappingTable.PermissionRecordId = permissionRecordUserRoleMapping.Id;
//                mappingTable.RoleId = permissionRecordUserRoleMapping.RoleId;
//            }
//            var result = await _context.SaveChangesAsync();
//            if (result == 0)
//            {
//                return new ServiceErrorResponse<bool>("Create permissionRecordUserRoleMapping failed");
//            }
//            return new ServiceSuccessResponse<bool>();
//        }

//        public async Task<ServiceResponse<bool>> DeletePermissionRecordUserRoleMappingByIdAsync(int id)
//        {
//            try
//            {
//                var prurm = await _context.PermissionRecordUserRoleMappings.FirstOrDefaultAsync(x => x.Id == id);
//                _context.PermissionRecordUserRoleMappings.Remove(prurm);
//                await _context.SaveChangesAsync();
//                return new ServiceSuccessResponse<bool>();
//            }
//            catch (Exception ex)
//            {
//                return new ServiceErrorResponse<bool>(message: ex.Message);
//            }
//        }

//        public async Task<List<PermissionRecordUserRoleMapping>> GetAllPermissionRecordUserRoleMappingAsync()
//        {
//            return await _context.PermissionRecordUserRoleMappings.ToListAsync();
//        }

//        public async Task<PermissionRecordUserRoleMapping> GetByPermissionRecordIdAsync(int permissionRecordId)
//        {
//            var prurm = await _context.PermissionRecordUserRoleMappings.FirstOrDefaultAsync(x => x.PermissionRecordId == permissionRecordId);

//            var response = new PermissionRecordUserRoleMapping
//            {
//                Data = prurm,
//                Success = true
//            };
//            return response;
//        }

//        public async Task<PermissionRecordUserRoleMapping> GetPermissionRecordUserRoleMappingByIdAsync(int id)
//        {
//            var prurm = await _context.PermissionRecordUserRoleMappings.FirstOrDefaultAsync(x => x.Id == id);

//            var response = new PermissionRecordUserRoleMapping
//            {
//                Data = prurm,
//                Success = true
//            };
//            return response;
//        }
//    }
//}
