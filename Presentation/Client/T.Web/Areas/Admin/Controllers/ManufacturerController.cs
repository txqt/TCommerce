using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Web.Areas.Admin.Models;
using T.Web.Extensions;
using T.Web.Services.ManufacturerServices;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/manufacturer/[action]")]
    public class ManufacturerController : BaseAdminController
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var manufacturers = await _manufacturerService.GetAllManufacturerAsync();

            var json = new { data = manufacturers };

            return this.JsonWithPascalCase(json);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new Manufacturer());
        }
        //[HttpPost]
        //public async Task<IActionResult> Create()
    }
}
