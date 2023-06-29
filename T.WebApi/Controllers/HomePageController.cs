using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using T.Library.Model;
using T.Library.Model.Users;
using T.WebApi.Helpers.TokenHelpers;
using T.WebApi.Services.HomePageServices;

namespace T.WebApi.Controllers
{
    [Route("api/home-page")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IHomePageService _homePageService;
        public HomePageController(ITokenService tokenService, IHomePageService homePageService)
        {
            _tokenService = tokenService;
            _homePageService = homePageService;
        }

        [HttpGet("all-product-categories")]
        public async Task<ActionResult<Category>> ShowCategoriesOnHomePage()
        {
            return Ok(await _homePageService.ShowCategoriesOnHomePage());
        }
    }
}
