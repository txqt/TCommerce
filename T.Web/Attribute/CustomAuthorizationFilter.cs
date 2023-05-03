using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using T.Library.Model.Enum;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace T.Web.Attribute
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
            // Kiểm tra xem Action hoặc Controller có bị đánh dấu với [AllowAnonymous] không
            bool allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute))
            || context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousFilter));
            if (allowAnonymous)
            {
                return;
            }

            // Các xử lý khác ở đây

            // Lấy thông tin đăng nhập của user
            var user = context.HttpContext.User;

            // Kiểm tra xem user đã đăng nhập hay chưa
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (_role == null)
            {
                var rolenames = typeof(RoleName).GetFields();
                foreach (var item in rolenames)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    string? name = item.GetRawConstantValue().ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    if (!user.HasClaim(c => c.Type == ClaimTypes.Role && item.Name == name))
                    {
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }

            if (user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == RoleName.Admin))
            {
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
