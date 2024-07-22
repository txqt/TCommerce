using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Component
{
    public class CategoryNavigationViewComponent : ViewComponent
    {
        private readonly ICatalogModelService _prepareCatalogModel;
        public CategoryNavigationViewComponent(ICatalogModelService prepareCategoryModel)
        {
            _prepareCatalogModel = prepareCategoryModel;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId)
        {
            return View(await _prepareCatalogModel.PrepareCategoryNavigationModelAsync(currentCategoryId));
        }
    }
}
