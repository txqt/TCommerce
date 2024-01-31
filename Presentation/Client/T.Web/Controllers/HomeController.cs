using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using T.Library.Model.Roles.RoleName;
using T.Web.Attribute;
using T.Web.Models;
using T.Web.Services.ProductService;

namespace T.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductsDisplayedOnHomepageAsync()
        {
            var listModel = await _productService.GetAllProductsDisplayedOnHomepageAsync();
            return Json(new { data = listModel });
        }

        //[CustomAuthorizationFilter(RoleName.Customer)]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}