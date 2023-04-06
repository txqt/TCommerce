using Microsoft.AspNetCore.Mvc;

namespace T.Web.Areas.Identity.Controllers
{
    [Area("Admin")]
    [Route("/admin/home/[action]")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
