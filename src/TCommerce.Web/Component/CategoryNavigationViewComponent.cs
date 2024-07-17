using Microsoft.AspNetCore.Mvc;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Component
{
    public class CategoryNavigationViewComponent : ViewComponent
    {
        private readonly ICatalogModelService _prepareCategoryModel;
        public CategoryNavigationViewComponent(ICatalogModelService prepareCategoryModel)
        {
            _prepareCategoryModel = prepareCategoryModel;
        }

        public async Task<IViewComponentResult> InvokeAsync(int currentCategoryId)
        {
            return View(await _prepareCategoryModel.PrepareCategoryNavigationModelAsync(currentCategoryId));
        }
    }
}
