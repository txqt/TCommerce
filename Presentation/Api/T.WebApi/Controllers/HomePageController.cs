using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Catalogs;
using T.WebApi.Services.HomePageServices;

namespace T.WebApi.Controllers
{
    [Route("api/home-page")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        private readonly IHomePageService _homePageService;
        public HomePageController(IHomePageService homePageService)
        {
            _homePageService = homePageService;
        }

        [HttpGet("all-product-categories")]
        public async Task<ActionResult<Category>> ShowCategoriesOnHomePage()
        {
            return Ok(await _homePageService.ShowCategoriesOnHomePage());
        }
    }
}
