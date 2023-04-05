using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using T.Library.Model.Response;

namespace T.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessions = context.HttpContext.Session.GetString("jwt");
            if (sessions == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
            base.OnActionExecuting(context);
        }
    }
}
