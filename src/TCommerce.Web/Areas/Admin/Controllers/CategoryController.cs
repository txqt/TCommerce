using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Security;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Web.Areas.Admin.Models.Catalog;
using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;
using TCommerce.Web.Attribute;
using TCommerce.Web.Extensions;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/category/[action]")]
    [CheckPermission(PermissionSystemName.ManageCategories)]
    public class CategoryController : BaseAdminController
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IAdminCategoryModelService _prepareModelService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;

        public CategoryController(ICategoryService categoryService, IMapper mapper, IAdminCategoryModelService prepareModelService, IProductService productService, IUrlRecordService urlRecordService)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _prepareModelService = prepareModelService;
            _productService = productService;
            _urlRecordService = urlRecordService;
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

            return this.JsonWithPascalCase(listModel);
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

            var category = _mapper.Map(model, new Category());

            var result = await _categoryService.CreateCategoryAsync(category);

            var seName = await _urlRecordService.ValidateSlug(category, model.SeName ?? "", category.Name, true);

            await _urlRecordService.SaveSlugAsync(category, seName);

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
            if (model.SeName is not null && model.SeName != (await _urlRecordService.GetSeNameAsync(category)))
            {
                model.SeName = await _urlRecordService.ValidateSlug(category, model.SeName, category.Name, true);

                await _urlRecordService.SaveSlugAsync(category, model.SeName);
            }
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

            return this.JsonWithPascalCase(model);
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
            var pagedList = await _productService.SearchProductsAsync(productParameters);

            var pagingResponse = new PagingResponse<Product>()
            {
                Items = pagedList,
                MetaData = pagedList.MetaData
            };

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

            var _ = await _prepareModelService.PrepareAddProductToCategorySearchModel(new ProductCategorySearchModel());

            return View(_);
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
