using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Startup;

namespace T.Web.Controllers
{
    public class StartupController : Controller
    {
        public IActionResult Index()
        {
            return View(new StartupFormModel());
        }

        [HttpPost("Install")]
        public IActionResult Install(StartupFormModel model)
        {
            if (!ModelState.IsValid)
            {
                // Handle validation errors
                return View(model);
            }

            // Create admin account
            // ...

            //// Save database configuration to appsettings.json
            //AppSettingsExtensions.AddToKey("ConnectionStrings:DefaultConnection", model.GetConnectionString());

            return RedirectToAction("Index");
        }
    }
}
