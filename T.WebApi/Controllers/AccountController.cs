using App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using T.Library.Model;
using T.Library.Model.RefreshToken;
using T.Library.Model.Response;
using T.Library.Model.Users;
using T.WebApi.Services.AccountServices;
using T.WebApi.Services.CacheServices;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICacheService _cache;
        public AccountController(IAccountService accountService, ICacheService cache)
        {
            _accountService = accountService;
            _cache = cache;
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
