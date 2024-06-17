using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using T.Library.Model.Startup;

namespace T.Web.Controllers
{
    public class InstallController : BaseController
    {
        private readonly HttpClient _httpClient;
        public InstallController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View(new StartupFormModel());
        }

        [HttpPost()]
        public async Task<IActionResult> Install(StartupFormModel model)
        {

            if (!ModelState.IsValid)
            {
                AddErrorsFromModel(ModelState.Values);
                return View("Index", model);
            }

            var result = await _httpClient.PostAsJsonAsync("api/db-manage/install", model);

            return RedirectToAction("Index");
        }
        
    }
}
