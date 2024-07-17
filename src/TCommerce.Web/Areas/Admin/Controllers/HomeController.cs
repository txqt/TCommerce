using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.Areas.Admin.Controllers;

namespace TCommerce.Web.Areas.Identity.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
