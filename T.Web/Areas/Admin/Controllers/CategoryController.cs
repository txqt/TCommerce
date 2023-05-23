using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Enum;
using T.Web.Areas.Admin.Models;
using T.Web.Attribute;
using T.Web.Services.CategoryService;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/category/[action]")]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
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
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            var result = await _categoryService.AddOrEdit(category);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(category);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
