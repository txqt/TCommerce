using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Security;
using T.Web.Attribute;

namespace T.Web.Areas.Admin.Controllers
{
    [CheckPermission(PermissionSystemName.AccessAdminPanel)]
    public class BaseAdminController : Controller
    {
        protected void SetStatusMessage(string message)
        {
            TempData["StatusMessage"] = message;
        }
    }
}
