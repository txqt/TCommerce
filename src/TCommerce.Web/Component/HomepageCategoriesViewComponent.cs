using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Component
{
    public class HomepageCategoriesViewComponent : ViewComponent
    {
        protected readonly ICatalogModelService _catalogModelService;

        public HomepageCategoriesViewComponent(ICatalogModelService catalogModelService)
        {
            _catalogModelService = catalogModelService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _catalogModelService.PrepareHomepageCategoryModelsAsync();
            if (!model.Any())
                return Content("");

            return View(model);
        }
    }
}
