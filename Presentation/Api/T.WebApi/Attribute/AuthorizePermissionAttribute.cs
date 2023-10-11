using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using T.Library.Model.Interface;

namespace T.WebApi.Attribute
{
    /// <summary>
	/// Represents a filter attribute that confirms access to the admin panel
	/// </summary>
	public sealed class AuthorizePermissionAttribute : TypeFilterAttribute
    {
        #region Ctor

        public AuthorizePermissionAttribute(string permissionSystemName, bool ignore = false)
            : base(typeof(AuthorizePermissionFilter))
        {
            PermissionSystemName = permissionSystemName;
            IgnoreFilter = ignore;
            Arguments = new object[] { permissionSystemName, ignore };
        }


        #endregion

        #region Properties

        public string PermissionSystemName { get; }

        public bool IgnoreFilter { get; }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that confirms access to the admin panel
        /// </summary>
        private class AuthorizePermissionFilter : IAsyncAuthorizationFilter
        {
            #region Fields

            private readonly string permission;
            private readonly bool ignoreFilter;
            private readonly ISecurityService _securityService;
            private readonly IAuthenticationService authenticationService;

            #endregion

            #region Ctor

            public AuthorizePermissionFilter(string permission, bool ignoreFilter, ISecurityService securityService, IAuthenticationService authenticationService)
            {
                this.permission = permission;
                this.ignoreFilter = ignoreFilter;
                this._securityService = securityService;
                this.authenticationService = authenticationService;
            }

            #endregion

            #region Private methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task AuthorizePermissionAsync(AuthorizationFilterContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                //if (!DataSettingsManager.IsDatabaseInstalled())
                //    return;

                // Kiểm tra xem Action hoặc Controller có bị đánh dấu với [AllowAnonymous] không
                bool allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute))
                    || context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousFilter));
                if (allowAnonymous)
                {
                    return;
                }

                //check whether this filter has been overridden for the action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter)
                    .OfType<AuthorizePermissionAttribute>()
                    .Where(a => a.PermissionSystemName == this.permission)
                    .FirstOrDefault();

                //ignore filter (the action is available even if navigation is not allowed)
                if (actionFilter is not null && actionFilter.IgnoreFilter)
                    return; // ignore attribute on controller, allow access for anyone

                //var customer = await authenticationService.GetAuthenticatedCustomerAsync();

                // check whether current customer has permission to access resource
                if (await _securityService.AuthorizeAsync((await _securityService.GetPermissionRecordBySystemNameAsync(permission)).Data))
                    return; // authorized, allow access

                // current user hasn't access
                context.Result = new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                await AuthorizePermissionAsync(context);
            }

            #endregion
        }

        #endregion
    }
}
