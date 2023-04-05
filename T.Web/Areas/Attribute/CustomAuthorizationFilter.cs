using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace T.Web.Areas.Attribute
{
    public class CustomAuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _role;
        public CustomAuthorizationFilter(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Lấy thông tin đăng nhập của user
            var user = context.HttpContext.User;

            // Kiểm tra xem user đã đăng nhập hay chưa
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Kiểm tra xem user có quyền truy cập vào tài nguyên này hay không
            if (!user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == _role))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
