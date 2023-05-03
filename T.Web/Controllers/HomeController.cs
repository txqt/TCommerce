using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using T.Library.Model.Enum;
using T.Web.Attribute;
using T.Web.Models;

namespace T.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [CustomAuthorizationFilter(RoleName.Customer)]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}