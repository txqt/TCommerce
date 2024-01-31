using Microsoft.AspNetCore.Mvc;

namespace T.Web.Controllers
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
