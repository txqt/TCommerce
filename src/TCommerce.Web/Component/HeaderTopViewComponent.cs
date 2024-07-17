using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Security;

namespace TCommerce.Web.Component
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
