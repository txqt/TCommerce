

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using T.Library.Model.Interface;

namespace T.Web.Attribute
{
    public class CheckPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public CheckPermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
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
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (_permissions != null)
            {
                var securityService = context.HttpContext.RequestServices.GetService<ISecurityService>();
                foreach (var permission in _permissions)
                {
                    //var permissionRecord = securityService.GetPermissionRecordBySystemNameAsync(permission).Result.Data;
                    if (!securityService.AuthorizeAsync(permission).Result)
                    {
                        context.Result = new ViewResult
                        {
                            ViewName = "AccessDenied"
                        };
                        return;
                    }
                }
            }
        }
    }


}
