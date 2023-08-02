using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;

namespace T.WebApi.Attribute
{

    public class AuthorizePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _requiredPermission;

        public AuthorizePermissionAttribute(string requiredPermission)
        {
            _requiredPermission = requiredPermission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var _userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
            var _dbContext = context.HttpContext.RequestServices.GetService<DatabaseContext>();

            // Lấy danh sách các role của người dùng từ JWT
            var userId = _userManager.GetUserId(user);
            var userRoles = _dbContext.UserRoles.Where(ur => ur.UserId.ToString() == userId).Select(ur => ur.RoleId).ToList();

            // Kiểm tra xem role của người dùng có liên kết với permission cần thiết hay không
            var hasPermission = _dbContext.PermissionRecordUserRoleMappings
                .Any(pr => userRoles.Contains(pr.RoleId) && pr.PermissionRecord.SystemName == _requiredPermission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }

}
