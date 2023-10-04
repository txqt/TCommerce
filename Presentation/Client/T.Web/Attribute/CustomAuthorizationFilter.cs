using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Authorization;
using T.Library.Model.Roles.RoleName;

namespace T.Web.Attribute
{
    public class CustomAuthorizationFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public CustomAuthorizationFilter(params string[] roles)
        {
            _roles = roles;
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

            if (_roles == null || _roles.Length == 0)
            {
                if (!user.Identity.IsAuthenticated)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                return;
            }

            // Kiểm tra xem user đã đăng nhập hay chưa
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Kiểm tra xem user có vai trò "Admin" hay không
            if (user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == RoleName.Admin))
            {
                return;
            }

            bool hasRequiredRole = false;
            foreach (var role in _roles)
            {
                if (user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == role))
                {
                    hasRequiredRole = true;
                    break;
                }
            }

            // Kiểm tra xem user có vai trò cần thiết hay không
            if (!hasRequiredRole)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

}
