using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using T.Library.Model.Startup;

namespace T.Web.Controllers
{
    public class InstallController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InstallController(HttpClient httpClient, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClient;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            //if(!_webHostEnvironment.IsDevelopment())
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //var result = await _httpClient.GetAsync("api/db-manage/is-installed");
            //if (result.IsSuccessStatusCode)
            //{
            //    return Redirect("ao-thun-nam");
            //    return RedirectToAction("Index", "Home");
            //}
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
