using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Roles.RoleName;
using T.Web.Attribute;
using T.Web.Controllers;

namespace T.Web.Areas.Identity.Controllers
{
    [Area("Admin")]
    [Route("/admin/home/[action]")]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
