using App.Utilities;
using Azure;
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
using T.WebApi.Extensions;
using T.WebApi.Helpers.TokenHelpers;
using T.WebApi.Services.AccountServices;
using T.WebApi.Services.CacheServices;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountController(IAccountService accountService, ITokenService tokenService)
        {
            _accountService = accountService;
            this._tokenService = tokenService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse<string>>> Login(LoginViewModel model, string? returnUrl = null)
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
        public async Task<ActionResult<ServiceResponse<int>>> Register(RegisterRequest request)
        {
            var response = await _accountService.Register(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
