using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Component
{
    public class ManufacturerNavigationViewComponent : ViewComponent
    {
        protected readonly ICatalogModelService _catalogModelService;

        public ManufacturerNavigationViewComponent(ICatalogModelService catalogModelService)
        {
            _catalogModelService = catalogModelService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentManufacturerId)
        {
            var model = await _catalogModelService.PrepareManufacturerNavigationModelAsync(currentManufacturerId);
            if (!model.Manufacturers.Any())
                return Content("");

            return View(model);
        }
    }
}
