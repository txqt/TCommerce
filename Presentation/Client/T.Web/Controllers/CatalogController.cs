using Microsoft.AspNetCore.Mvc;
using T.Library.Model.Interface;
using T.Web.Models.Catalog;
using T.Web.Services.PrepareModelServices;

namespace T.Web.Controllers
{
    public class CatalogController : BaseController
    {
        private readonly ICatalogModelService _prepareCategoryModel;
        private readonly ICategoryServiceCommon _categoryService;

        public CatalogController(ICatalogModelService prepareCategoryModel, ICategoryServiceCommon categoryService)
        {
            _prepareCategoryModel = prepareCategoryModel;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public virtual async Task<IActionResult> Category(int id, CatalogProductsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            
            return View(await _prepareCategoryModel.PrepareCategoryModelAsync(category, command));
        }
        public virtual async Task<IActionResult> GetCategoryProducts(int categoryId, CatalogProductsCommand command)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId);

            if (category is null || category.Deleted)
                return InvokeHttp404();

            var model = await _prepareCategoryModel.PrepareCategoryProductsModelAsync(category, command);

            return PartialView("_CatalogProducts", model);
        }
    }
}
