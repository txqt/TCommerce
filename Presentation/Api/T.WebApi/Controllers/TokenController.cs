using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using T.Library.Model;
using T.Library.Model.RefreshToken;
using T.WebApi.Attribute;
using T.WebApi.Services.TokenServices;

namespace T.WebApi.Controllers
{
    [Route("api/token")]
    [ApiController]
    [CheckPermission()]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public TokenController(ITokenService accountService)
        {
            _tokenService = accountService;
        }

        [HttpPost("create")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> Create(AccessTokenRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _tokenService.Create(model);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestModel tokenDto)
        {
            var response = await _tokenService.RefreshToken(tokenDto);

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

            //var result = await _tokenService.Logout(userId);
            return NoContent();
        }
    }
}
