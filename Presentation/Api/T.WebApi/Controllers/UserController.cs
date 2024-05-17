using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
using T.WebApi.Services.UserRegistrations;
using T.WebApi.Services.UserServices;

namespace T.WebApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    [CheckPermission(PermissionSystemName.ManageUsers)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUserRegistrationService _userRegistrationService;
        public UserController(IUserService userService, IMapper mapper, IUserRegistrationService userRegistrationService)
        {
            _userService = userService;
            _mapper = mapper;
            _userRegistrationService = userRegistrationService;
        }

        [HttpGet(APIRoutes.GETALL)]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> Get(Guid id)
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
            var result = await _userService.UpdateUserAsync(model, true);
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
        [CheckPermission]
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

        [HttpPost("account/register")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var model = _mapper.Map<UserModel>(request);
            var response = await _userService.CreateUserAsync(model);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        //[HttpGet("account/confirm-email")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfirmEmail(string userId, string token)
        //{
        //    if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        //        return NotFound();

        //    var result = await _userRegistrationService.ConfirmEmail(userId, token);

        //    if (!result.Success)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}

        [HttpPost("account/forgot-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<string>>> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound(new ServiceErrorResponse<string>("Chưa nhập email"));

            var result = await _userRegistrationService.SendResetPasswordEmail(email);

            if (result.Success)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }

        [HttpPost("account/reset-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            var result = await _userRegistrationService.ResetPassword(model);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("account/change-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ChangePasword(ChangePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _userRegistrationService.ChangePassword(model);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPut("me/account/info")]
        [CheckPermission]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateUserAccountInfo(AccountInfoModel model)
        {
            var result = await _userService.UpdateUserAccountInfo(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("me/account/addresses")]
        [CheckPermission]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> GetOwnAddressesAsync()
        {
            var result = await _userService.GetOwnAddressesAsync();
            return Ok(result);
        }

        [HttpPost("me/account/address")]
        [CheckPermission]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateUserAddressAsync(DeliveryAddress address)
        {
            var result = await _userService.CreateUserAddressAsync(address);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("me/account/address")]
        [CheckPermission]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateUserAddressAsync(DeliveryAddress address)
        {
            var result = await _userService.UpdateUserAddressAsync(address);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("account/address/{id}")]
        [CheckPermission]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteUserAddressAsync(int id)
        {
            var result = await _userService.DeleteUserAddressAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
