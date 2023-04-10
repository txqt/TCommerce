using Microsoft.AspNetCore.Mvc;

namespace T.WebApi.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
