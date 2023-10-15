using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
using T.WebApi.Services.UserServices;

namespace T.WebApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    [CheckPermission(PermissionSystemName.ManageUsers)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet(APIRoutes.GetAll)]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<UserModel>>> Get(Guid id)
        {
            return await _userService.Get(id);
        }

        [HttpPost()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateUserAsync(UserModel model)
        {
            var result = await _userService.CreateUserAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateUserAsync(UserModel model)
        {
            var result = await _userService.UpdateUserAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteUserByUserIdAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<ActionResult> GetCurrentUser()
        {
            return Ok(await _userService.GetCurrentUser());
        }

        [HttpPost("ban/{userId}")]
        public async Task<ActionResult> BanUser(string userId)
        {
            var result = await _userService.BanUser(userId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
