using Microsoft.AspNetCore.Mvc;

namespace TCommerce.Web.Controllers
{
    public class CommonController : BaseController
    {
        public virtual IActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            Response.ContentType = "text/html";

            return View();
        }
    }
}
