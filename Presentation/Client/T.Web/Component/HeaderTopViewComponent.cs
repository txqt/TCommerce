using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Interface;
using T.Library.Model.Security;

namespace T.Web.Component
{
    public class HeaderTopViewComponent : ViewComponent
    {
        private readonly ISecurityService _securityService;

        public HeaderTopViewComponent(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.IsAdmin = await _securityService.AuthorizeAsync(PermissionSystemName.AccessAdminPanel);
            return View();
        }
    }
}
