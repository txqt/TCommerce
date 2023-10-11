using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Interface;
using T.Library.Model.Security;
using T.WebApi.Attribute;

namespace T.WebApi.Controllers
{
    [Route("api/security")]
    [ApiController]
    [AuthorizePermission(PermissionSystemName.ManagePermissions)]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        public SecurityController(ISecurityService permissionRecordService)
        {
            _securityService = permissionRecordService;
        }

        [HttpGet("roles")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await _securityService.GetRoles());
        }

        [HttpGet("role/{roleId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetRoleByRoleId(string roleId)
        {
            var result = await _securityService.GetRoleByRoleId(roleId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("permissions")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetPermissons()
        {
            return Ok(await _securityService.GetAllPermissionRecordAsync());
        }

        [HttpGet("role/{roleId}/permission/{permissionId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetPermissonsRolesMapping(string roleId, int permissionId)
        {
            var result = await _securityService.GetPermissionMappingAsync(roleId, permissionId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        
        [HttpGet("permission/{permissionId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetMappingByPermissionRecordIdAsync(int permissionId)
        {
            var result = await _securityService.GetPermissionRecordByIdAsync(permissionId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("permission-mapping")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateProduct(PermissionRecordUserRoleMapping permissionRecordUserRoleMapping)
        {
            var result = await _securityService.CreatePermissionMappingAsync(permissionRecordUserRoleMapping);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("permission-mapping/{permissionMapping}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteProduct(int permissionMapping)
        {
            var result = await _securityService.DeletePermissionMappingByIdAsync(permissionMapping);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
