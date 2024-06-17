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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IWebHostEnvironment webHostEnvironment, HttpClient httpClient)
        {
            _logger = logger;
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                var result = await _httpClient.GetAsync("api/db-manage/is-installed");

                if (!result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Install");
                }
            }
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