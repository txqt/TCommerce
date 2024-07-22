using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : BaseAdminController
    {
        private ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public async Task<IActionResult> CatalogSettings()
        {
            var model = await _settingService.LoadSettingAsync<CatalogSettings>();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CatalogSettings(CatalogSettings catalogSettings)
        {
            if (!ModelState.IsValid)
            {
                return View(catalogSettings);
            }

            await _settingService.SaveSettingAsync(catalogSettings);

            return View();
        }
    }
}
