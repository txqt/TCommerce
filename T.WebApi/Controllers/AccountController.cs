using App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using T.Library.Model;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.WebApi.Services.AccountServices;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse<string>>> Login(LoginViewModel model, string? returnUrl = null)
        {
            var response = await _accountService.Login(model, returnUrl);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
