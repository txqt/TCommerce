using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Component
{
    public class SearchBoxViewComponent : ViewComponent
    {
        private readonly ICatalogModelService _catalogModelService;

        public SearchBoxViewComponent(ICatalogModelService catalogModelService)
        {
            _catalogModelService = catalogModelService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default")
        {
            return View(viewName, await _catalogModelService.PrepareSearchBoxModelAsync());
        }
    }
}
