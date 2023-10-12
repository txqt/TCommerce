using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Roles.RoleName;
using T.Web.Areas.Admin.Controllers;
using T.Web.Attribute;

namespace T.Web.Areas.Identity.Controllers
{
    [Area("Admin")]
    [Route("/admin/home/[action]")]
    //[CustomAuthorizationFilter(RoleName.Admin)]
    public class HomeController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
