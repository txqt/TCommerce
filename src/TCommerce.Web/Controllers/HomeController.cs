using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Services.DbManageServices;

namespace TCommerce.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly IProductService _productService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IServiceProvider _serviceProvider;

        public HomeController(ILogger<HomeController> logger, /*IProductService productService,*/ IWebHostEnvironment webHostEnvironment, IServiceProvider serviceProvider)
        {
            _logger = logger;
            //_productService = productService;
            _webHostEnvironment = webHostEnvironment;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllProductsDisplayedOnHomepageAsync()
        //{
        //    var listModel = await _productService.GetAllProductsDisplayedOnHomepageAsync();
        //    return Json(new { data = listModel });
        //}

        //[CustomAuthorizationFilter(RoleName.Customer)]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}