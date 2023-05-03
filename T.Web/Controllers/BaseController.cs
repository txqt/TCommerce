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
            // Lấy thông tin đăng nhập của user
            var user = context.HttpContext.User;

            // Kiểm tra xem user đã đăng nhập hay chưa
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
            //var sessions = context.HttpContext.Session.GetString("jwt");
            //if (sessions == null)
            //{
            //    context.Result = new RedirectToActionResult("Login", "Account", null);
            //}
            base.OnActionExecuting(context);
        }
    }
}
