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
using T.Web.Areas.Admin.Services.PrepareAdminModel;
using T.Web.Attribute;
using T.Web.Extensions;
using T.Web.Services.CategoryService;

using T.Web.Services.ProductService;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/category/[action]")]
    //[CustomAuthorizationFilter(RoleName.Admin)]
    [CheckPermission(PermissionSystemName.ManageCategories)]
    public class CategoryController : BaseAdminController
    {
        private readonly ICategoryServiceCommon _categoryService;
        private readonly IMapper _mapper;
        private readonly IAdminCategoryModelService _prepareModelService;
        private readonly IProductService _productService;

        public CategoryController(ICategoryServiceCommon categoryService, IMapper mapper, IAdminCategoryModelService prepareModelService, IProductService productService)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _prepareModelService = prepareModelService;
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View(new CategoryModelAdmin());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var categoryList = await _categoryService.GetAllCategoryAsync();

            var listModel = _mapper.Map<List<CategoryModelAdmin>>(categoryList);

            foreach (var item in listModel)
            {
                if (item.ParentCategoryId > 0)
                {
                    item.ParentCategoryName = (await _categoryService.GetCategoryByIdAsync(item.ParentCategoryId)).Name;
                }
            }

            var json = new { data = listModel };

            return this.JsonWithPascalCase(json);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _prepareModelService.PrepareCategoryModelAsync(new CategoryModelAdmin(), null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryModelAdmin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _categoryService.CreateCategoryAsync(model);

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

            var model = await _prepareModelService.PrepareCategoryModelAsync(new CategoryModelAdmin(), category);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModelAdmin model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = await _categoryService.GetCategoryByIdAsync(model.Id) ??
                throw new ArgumentException("No category found with the specified id");

            var result = await _categoryService.UpdateCategoryAsync(model);
            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareCategoryModelAsync(model, category);
                return View(model);
            }
            else
            {
                SetStatusMessage("Sửa thành công");
            }

            return View(nameof(Index));
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

        public async Task<IActionResult> GetProductCategoryMapping(int categoryId)
        {
            ArgumentNullException.ThrowIfNull(await _categoryService.GetCategoryByIdAsync(categoryId));

            var productCategoryList = (await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId));

            var model = _mapper.Map<List<ProductCategoryModel>>(productCategoryList);

            foreach (var item in model)
            {
                item.ProductName = (await _productService.GetByIdAsync(item.ProductId))?.Name;
            }

            var json = new
            {
                data = model
            };

            return this.JsonWithPascalCase(json);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategoryMapping(int id)
        {

            var result = await _categoryService.DeleteProductCategoryMappingById(id);
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

            var model = new ProductCategorySearchModel();

            model = await _prepareModelService.PrepareAddProductToCategorySearchModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductList(ProductCategorySearchModel model)
        {
            // Create ProductParameters from DataTables parameters
            var productParameters = ExtractQueryStringParameters<ProductParameters>();

            productParameters.CategoryIds = new List<int> { model.SearchByCategoryId };
            productParameters.ManufacturerIds = new List<int> { model.SearchByManufacturerId };
            // Call the service to get the paged data
            var pagingResponse = await _productService.GetAll(productParameters);

            var json = ToDatatableReponse<Product>(pagingResponse.MetaData.TotalCount, pagingResponse.MetaData.TotalCount, pagingResponse.Items);

            return this.JsonWithPascalCase(json);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToCategory(AddProductCategoryModel model)
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

            if (result.Success)
            {
                SetStatusMessage("Thêm thành công");
                ViewBag.RefreshPage = true;
            }

            return View(AddProductToCategory(model.CategoryId));
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
