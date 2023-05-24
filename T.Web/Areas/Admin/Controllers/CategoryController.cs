using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using T.Library.Model;
using T.Library.Model.Enum;
using T.Web.Areas.Admin.Models;
using T.Web.Attribute;
using T.Web.Controllers;
using T.Web.Services.CategoryService;
using T.Web.Services.PrepareModel;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/category/[action]")]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IPrepareModelService _prepareModelService;

        public CategoryController(ICategoryService categoryService, IMapper mapper, IPrepareModelService prepareModelService)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _prepareModelService = prepareModelService;
        }

        public IActionResult Index()
        {
            return View(new CategoryModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var categoryList = await _categoryService.GetAllAsync();

            var listModel = _mapper.Map<List<CategoryModel>>(categoryList);
            foreach(var item in listModel)
            {
                if(item.ParentCategoryId != 0)
                {
                    item.ParentCategoryName = (await _categoryService.Get(item.ParentCategoryId)).Data.Name;
                }
            }

            return Json(new { data = listModel });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _prepareModelService.PrepareCategoryModelAsync(new CategoryModel(), null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = _mapper.Map<Category>(model);
            var result = await _categoryService.AddOrEdit(category);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = (await _categoryService.Get(id)).Data ??
                throw new ArgumentException("No category found with the specified id");

            var model = await _prepareModelService.PrepareCategoryModelAsync(new CategoryModel(), category);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = (await _categoryService.Get(model.Id)).Data ??
                throw new ArgumentException("No category found with the specified id");

            category = _mapper.Map(model, category);

            var result = await _categoryService.AddOrEdit(category);
            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareCategoryModelAsync(model, category);
                return View(model);
            }

            SetStatusMessage("Sửa thành công");
            model = await _prepareModelService.PrepareCategoryModelAsync(model, category);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {

            var result = await _categoryService.Delete(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}
