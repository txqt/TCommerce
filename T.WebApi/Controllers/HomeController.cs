using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using T.Library.Model.Users;
using T.WebApi.Helpers.TokenHelpers;

namespace T.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        public HomeController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        [HttpGet]
        public async Task<ActionResult<string>> Hello()
        {
           

            return "Hello";
        }
    }
}
