using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Web.Models.Catalog;
using TCommerce.Web.PrepareModelServices;

namespace TCommerce.Web.Controllers
{
    public class CatalogController : BaseController
    {
        private readonly ICatalogModelService _prepareCategoryModel;
        private readonly ICategoryService _categoryService;

        public CatalogController(ICatalogModelService prepareCategoryModel, ICategoryService categoryService)
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
