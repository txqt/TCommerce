using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Web.Areas.Admin.Models;
using T.Web.Areas.Admin.Models.SearchModel;
using T.Web.Attribute;
using T.Web.Services.CategoryService;
using T.Web.Services.PrepareModel;
using T.Web.Services.PrepareModelServices;
using T.Web.Services.ProductService;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/category/[action]")]
    //[CustomAuthorizationFilter(RoleName.Admin)]
    [CheckPermission(PermissionSystemName.ManageCategories)]
    public class CategoryController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ICategoryModelService _prepareModelService;
        private readonly IProductService _productService;

        public CategoryController(ICategoryService categoryService, IMapper mapper, ICategoryModelService prepareModelService, IProductService productService)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _prepareModelService = prepareModelService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View(new CategoryModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var categoryList = await _categoryService.GetAllCategoryAsync();

            var listModel = _mapper.Map<List<CategoryModel>>(categoryList);

            foreach (var item in listModel)
            {
                if (item.ParentCategoryId > 0)
                {
                    item.ParentCategoryName = (await _categoryService.GetCategoryByIdAsync(item.ParentCategoryId)).Name;
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
            var result = await _categoryService.CreateCategoryAsync(category);

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
            var category = await _categoryService.GetCategoryByIdAsync(id) ??
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

            var category = await _categoryService.GetCategoryByIdAsync(model.Id) ??
                throw new ArgumentException("No category found with the specified id");

            _mapper.Map(model, category);

            var result = await _categoryService.UpdateCategoryAsync(category);
            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareCategoryModelAsync(model, category);
                return View(model);
            }
            else
            {
                SetStatusMessage("Sửa thành công");
                model = await _prepareModelService.PrepareCategoryModelAsync(model, category);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {

            var result = await _categoryService.DeleteCategoryByIdAsync(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }

        public async Task<IActionResult> GetListCategoryMapping(int categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId) ??
              throw new ArgumentException("Not found with the specified id");

            var productCategoryList = (await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId));

            var model = _mapper.Map<List<ProductCategoryModel>>(productCategoryList);

            foreach (var item in model)
            {
                item.ProductName = (await _productService.GetByIdAsync(item.ProductId))?.Name;
            }

            //model = model.OrderBy(x => x.DisplayOrder).ToList();

            return Json(new
            {
                data = model
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryMapping(int id)
        {

            var result = await _categoryService.DeleteCategoryMappingById(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }

        public async Task<IActionResult> AddProductToCategory(int categoryId)
        {
            var category = await _categoryService.GetCategoryByIdAsync(categoryId) ??
                throw new ArgumentException("Not found with the specified id");

            var model = new AddProductToCategorySearchModel();

            var category_list = await _categoryService.GetAllCategoryAsync();

            category_list.Insert(0, new Category()
            {
                Id = 0,
                Name = "All"
            });

            model.AvailableCategories = (category_list).Select(productAttribute => new SelectListItem
            {
                Text = productAttribute.Name,
                Value = productAttribute.Id.ToString()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductList(AddProductToCategorySearchModel model)
        {
            var draw = int.Parse(Request.Form["draw"].FirstOrDefault());
            var start = int.Parse(Request.Form["start"].FirstOrDefault());
            var length = int.Parse(Request.Form["length"].FirstOrDefault());
            int orderColumnIndex = int.Parse(Request.Form["order[0][column]"]);
            string orderDirection = Request.Form["order[0][dir]"];
            string orderColumnName = Request.Form["columns[" + orderColumnIndex + "][data]"];

            string orderBy = orderColumnName + " " + orderDirection;
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            // Create ProductParameters from DataTables parameters
            var productParameter = new ProductParameters
            {
                PageNumber = start / length + 1,
                PageSize = length,
                SearchText = searchValue,
                OrderBy = orderBy,
                CategoryId = model.SearchByCategoryId
            };

            // Call the service to get the paged data
            var pagingResponse = await _productService.GetAll(productParameter);

            // Return the data in the format that DataTables expects
            return Json(new DataTableResponse
            {
                Draw = draw,
                RecordsTotal = pagingResponse.MetaData.TotalCount,
                RecordsFiltered = pagingResponse.MetaData.TotalCount,
                Data = pagingResponse.Items.Cast<object>().ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToCategory(AddProductToCategoryModel model)
        {
            if (!model.SelectedProductIds.Any())
            {
                return View(new ProductSearchModel());
            }

            var existingProductCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(model.CategoryId);

            var productCategoriesToAdd = model.SelectedProductIds.Except(existingProductCategories.Select(pc => pc.ProductId))
                .Select(pid => new ProductCategory
                {
                    CategoryId = model.CategoryId,
                    ProductId = pid,
                    IsFeaturedProduct = false,
                    DisplayOrder = 1
                }).ToList();

            var result = await _categoryService.BulkCreateProductCategoriesAsync(productCategoriesToAdd);

            if (!result.Success)
            {
                return View(new AddProductToCategorySearchModel());
            }
            else
            {
                SetStatusMessage("Thêm thành công");
                ViewBag.RefreshPage = true;
            }

            return View(new AddProductToCategorySearchModel());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductCategory([FromBody] ProductCategory model)
        {
            var productCategory = await _categoryService.GetProductCategoryByIdAsync(model.Id) ??
                throw new ArgumentException("Not found with the specified id");

            productCategory.IsFeaturedProduct = model.IsFeaturedProduct;
            productCategory.DisplayOrder = model.DisplayOrder;

            var result = await _categoryService.UpdateProductCategoryAsync(productCategory);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}
