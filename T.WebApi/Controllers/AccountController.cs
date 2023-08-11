using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using T.Library.Model;
using T.Library.Model.Account;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.WebApi.Attribute;
using T.WebApi.Services.AccountServices;

namespace T.WebApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    [CustomAuthorizationFilter()]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _accountService.Login(model, returnUrl);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto tokenDto)
        {
            var response = await _accountService.RefreshToken(tokenDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await _accountService.Register(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Lấy token từ header Authorization của request
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            string? rawUserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(rawUserId, out Guid userId))
            {
                return Unauthorized();
            }

            var result = await _accountService.Logout(userId);
            return NoContent();
        }

        [HttpGet("ConfirmEmail")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _accountService.ConfirmEmail(userId, token);

            if (result.Success)
            {
                return Redirect($"{result.Data}/account/RegisterConfirmed");
            }

            return BadRequest(result);
        }

        [HttpPost("forgot-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<string>>> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound(new ServiceErrorResponse<string>("Chưa nhập email"));

            var result = await _accountService.SendChangePasswordEmail(email);

            if (result.Success)
                return Ok(result); // 200

            return BadRequest(result); // 400
        }

        [HttpPost("reset-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            var result = await _accountService.ResetPassword(model);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("change-password")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ChangePasword(ChangePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _accountService.ChangePassword(model);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
