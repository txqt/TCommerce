using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using T.Library.Model.Security;
using T.Library.Model.Users;
using T.WebApi.Database.ConfigurationDatabase;
using T.WebApi.Services.PermissionRecordUserRoleMappingServices;

namespace T.WebApi.Attribute
{

    //public class AuthorizePermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    //{
    //    private readonly string _requiredPermission;

    //    public AuthorizePermissionAttribute(string requiredPermission)
    //    {
    //        _requiredPermission = requiredPermission;
    //    }

    //    public async void OnAuthorization(AuthorizationFilterContext context)
    //    {
    //        var user = context.HttpContext.User;

    //        if (!user.Identity.IsAuthenticated)
    //        {
    //            context.Result = new UnauthorizedResult();
    //            return;
    //        }

    //        var permissionMappingRolesService = context.HttpContext.RequestServices.GetService<IPermissionRecordUserRoleMappingService>();

    //        // Kiểm tra xem role của người dùng có liên kết với permission cần thiết hay không
    //        var hasPermission = (await permissionMappingRolesService.AuthorizeAsync(_requiredPermission));

    //        if (!hasPermission)
    //        {
    //            context.Result = new ForbidResult();
    //        }
    //    }
    //}

}
