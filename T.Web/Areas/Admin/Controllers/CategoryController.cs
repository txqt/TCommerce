using Microsoft.AspNetCore.Mvc;

namespace T.Web.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
