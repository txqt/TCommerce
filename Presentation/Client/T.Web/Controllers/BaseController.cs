using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace T.Web.Controllers
{
    public class BaseController : Controller
    {
        protected void SetStatusMessage(string message)
        {
            TempData["StatusMessage"] = message;
        }
        protected void AddErrorsFromModel(ModelStateDictionary.ValueEnumerable values)
        {
            foreach (ModelStateEntry modelState in values)
                foreach (ModelError error in modelState.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage.ToString());

                }
        }
    }
}
