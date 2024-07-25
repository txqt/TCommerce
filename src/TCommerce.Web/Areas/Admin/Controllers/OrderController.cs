using Microsoft.AspNetCore.Mvc;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/order/[action]")]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
