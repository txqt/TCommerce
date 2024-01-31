using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security;
using System.Security.Claims;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;

namespace T.WebApi.Attribute
{
    /// <summary>
    /// Attribute for checking user permissions before accessing a resource (controller action) in the application.
    /// When no specific permissions are provided, it checks if the user is authenticated.
    /// </summary>
    public class CheckPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        /// <summary>
        /// Initializes an instance of the attribute with a list of permissions to check.
        /// </summary>
        /// <param name="permissions">The list of permissions to check.</param>
        public CheckPermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        /// <summary>
        /// Method to perform permission checks before accessing a resource.
        /// </summary>
        /// <param name="context">The permission checking context.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if the Action or Controller is marked with [AllowAnonymous]
            bool allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute))
                || context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousFilter));
            if (allowAnonymous)
            {
                return;
            }

            // Get user login information
            var user = context.HttpContext.User;

            if ((_permissions == null || _permissions.Length == 0) && user.Identity is not null && !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (_permissions != null)
            {
                var securityService = context.HttpContext.RequestServices.GetService<ISecurityService>();

                ArgumentNullException.ThrowIfNull(securityService);

                foreach (var permission in _permissions)
                {
                    if (!securityService.AuthorizeAsync(permission).Result)
                    {
                        context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                        return;
                    }
                }
            }
        }
    }


}
