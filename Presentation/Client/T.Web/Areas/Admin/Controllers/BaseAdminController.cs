using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Security;
using T.Web.Areas.Admin.Models;
using T.Web.Attribute;
using T.Web.Controllers;

namespace T.Web.Areas.Admin.Controllers
{
    [CheckPermission(PermissionSystemName.AccessAdminPanel)]
    public class BaseAdminController : BaseController
    {
    }
}
