using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.WebApi.Attribute;

namespace T.WebApi.Controllers
{
    [Route("api/security")]
    [ApiController]
    //[CustomAuthorizationFilter(RoleName.Admin)]
    [CheckPermissionAttribute(PermissionSystemName.ManagePermissions)]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Role))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoleByRoleId(string roleId)
        {
            var result = await _securityService.GetRoleByRoleId(roleId);

            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("permissions")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> GetPermissons()
        {
            return Ok(await _securityService.GetAllPermissionRecordAsync());
        }

        [HttpGet("role/{roleId}/permission/{permissionId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermissonsRolesMapping(string roleId, int permissionId)
        {
            var result = await _securityService.GetPermissionMappingAsync(roleId, permissionId);

            if (result is not null)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        
        [HttpGet("permission/{permissionId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMappingByPermissionRecordIdAsync(int permissionId)
        {
            var result = await _securityService.GetPermissionRecordByIdAsync(permissionId);

            if (result is not null)
            {
                return Ok(result);
            }

            return NotFound();
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

        [HttpPost("authorize-permission")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> AuthorizePermission(PermissionRecord permissionRecord)
        {
            var success = await _securityService.AuthorizeAsync(permissionRecord);
            if (!success)
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
            return Ok(success);
        }

        [HttpGet("authorize-permission/system-name/{permissionSystemName}")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> AuthorizePermissionBySystemName(string permissionSystemName)
        {
            var success = await _securityService.AuthorizeAsync(permissionSystemName);
            if (!success)
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
            return Ok(success);
        }

        [HttpGet("permission/system-name/{permissionRecordSystemName}")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermissionRecordBySystemNameAsync(string permissionRecordSystemName)
        {
            var result = await _securityService.GetPermissionRecordBySystemNameAsync(permissionRecordSystemName);

            if (result is not null)
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
