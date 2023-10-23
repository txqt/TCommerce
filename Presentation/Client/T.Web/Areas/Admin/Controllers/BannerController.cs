using Microsoft.AspNetCore.Mvc;

namespace T.Web.Areas.Admin.Controllers
{
    public class BannerController : Controller
    {
        private readonly HttpClient _httpClient;
        public BannerController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
