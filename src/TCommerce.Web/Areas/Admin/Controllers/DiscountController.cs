using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Extensions;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Discounts;
using TCommerce.Core.Models.Paging;
using TCommerce.Core.Models.Security;
using TCommerce.Services.CategoryServices;
using TCommerce.Services.ManufacturerServices;
using TCommerce.Services.ProductServices;
using TCommerce.Web.Areas.Admin.Models.Discounts;
using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;
using TCommerce.Web.Attribute;
using TCommerce.Web.Extensions;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/discount/[action]")]
    [CheckPermission(PermissionSystemName.ManageDiscounts)]
    public class DiscountController : BaseAdminController
    {
        private readonly IDiscountService _discountService;
        private readonly IAdminDiscountModelService _discountModelService;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;

        public DiscountController(IDiscountService discountService, IAdminDiscountModelService discountModelService, IMapper mapper, IProductService productService, ICategoryService categoryService, IManufacturerService manufacturerService)
        {
            _discountService = discountService;
            _discountModelService = discountModelService;
            _mapper = mapper;
            _productService = productService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _discountModelService.PrepareDiscountSearchModelModelAsync(new DiscountSearchModel());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllAsync(DiscountSearchModel searchModel)
        {
            var discountParameters = ParseQueryStringParameters<DiscountParameters>();

            discountParameters.DiscountType = (DiscountType)searchModel.SearchDiscountTypeId;
            discountParameters.IsActiveId = searchModel.IsActiveId;

            var response = await _discountService.SearchDiscount(discountParameters);

            var pagingResponse = new PagingResponse<Discount>
            {
                Items = response,
                MetaData = response.MetaData
            };

            List<DiscountModel> discountModels = new List<DiscountModel>();

            foreach (var discount in pagingResponse.Items)
            {
                var discountModel = _mapper.Map<DiscountModel>(discount);
                discountModel.DiscountTypeName = discount.DiscountType.GetFormattedEnumName();
                discountModel.TimesUsed = (await _discountService.GetAllDiscountUsageHistoryAsync(discount.Id)).Count;
                discountModels.Add(discountModel);
            }

            var model = ToDatatableReponse(pagingResponse.MetaData.TotalCount, pagingResponse.MetaData.TotalCount, discountModels);

            return this.JsonWithPascalCase(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDiscount()
        {
            var model = await _discountModelService.PrepareDiscountModelAsync(new DiscountModel(), null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount(DiscountModel discountModel, bool continueEditing)
        {
            if (!ModelState.IsValid)
            {
                var model = await _discountModelService.PrepareDiscountModelAsync(discountModel, null);
                return View(model);
            }

            var discount = _mapper.Map<Discount>(discountModel);
            await _discountService.CreateDiscountAsync(discount);

            if (!continueEditing)
                return RedirectToAction(nameof(Index));

            return RedirectToAction("EditDiscount", new { id = discount.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EditDiscount(int id)
        {
            var discount = await _discountService.GetByIdAsync(id);

            var model = await _discountModelService.PrepareDiscountModelAsync(null, discount);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditDiscount(DiscountModel model, bool continueEditing)
        {
            var discount = await _discountService.GetByIdAsync(model.Id);
            ArgumentNullException.ThrowIfNull(discount);

            if (!ModelState.IsValid)
            {
                model = await _discountModelService.PrepareDiscountModelAsync(null, discount);
                return View(model);
            }


            var prevDiscountType = discount.DiscountType;

            _mapper.Map(model, discount);

            await _discountService.UpdateDiscountAsync(discount);

            if (prevDiscountType != discount.DiscountType)
            {
                switch (prevDiscountType)
                {
                    case DiscountType.AssignedToSkus:
                        await _productService.ClearDiscountProductMappingAsync(discount);
                        break;
                    case DiscountType.AssignedToCategories:
                        await _categoryService.ClearDiscountCategoryMappingAsync(discount);
                        break;
                    case DiscountType.AssignedToManufacturers:
                        await _manufacturerService.ClearDiscountManufacturerMappingAsync(discount);
                        break;
                    default:
                        break;
                }
            }

            if (!continueEditing)
                return RedirectToAction(nameof(Index));

            return RedirectToAction("EditDiscount", new { id = discount.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            await _discountService.DeleteDiscountAsync(id);
            return Json(new { success = true, message = "Success" });
        }

        #region Add Product To Discount
        public async Task<IActionResult> ProductList(int discountId)
        {
            ArgumentNullException.ThrowIfNull(await _discountService.GetByIdAsync(discountId));

            var discountProducts = (await _productService.GetProductsWithAppliedDiscountAsync(discountId));

            var model = _mapper.Map<List<DiscountProductModel>>(discountProducts);

            return this.JsonWithPascalCase(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductDelete(int discountId, int productId)
        {

            //try to get a discount with the specified id
            var discount = await _discountService.GetByIdAsync(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a product with the specified id
            var product = await _productService.GetByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id", nameof(productId));

            var mapping = await _productService.GetDiscountAppliedToProductAsync(product.Id, discountId);

            //remove discount
            if (mapping is DiscountProductMapping discountProductMapping)
                await _productService.DeleteDiscountProductMappingAsync(mapping.Id);

            await _productService.UpdateProductAsync(product);

            return Json(new { success = true, message = "Success" });
        }

        public async Task<IActionResult> AddProductToDiscount(int discountId)
        {
            var discount = await _discountService.GetByIdAsync(discountId) ??
                throw new ArgumentException("Not found with the specified id");

            var model = new DiscountProductSearchModel();

            model = await _discountModelService.PrepareAddProductToDiscountSearchModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToDiscountList(DiscountProductSearchModel model)
        {
            // Create ProductParameters from DataTables parameters
            var productParameters = ParseQueryStringParameters<ProductParameters>();

            productParameters.CategoryIds = new List<int> { model.CategoryId };
            productParameters.ManufacturerIds = new List<int> { model.ManufacturerId };
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
        public async Task<IActionResult> AddProductToDiscount(AddProductDiscountModel model)
        {
            if (!model.SelectedProductIds.Any())
            {
                return View(new DiscountSearchModel());
            }

            var discount = await _discountService.GetByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

            var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToList());

            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (await _productService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is null)
                        await _productService.CreateDiscountProductMappingAsync(new DiscountProductMapping { EntityId = product.Id, DiscountId = discount.Id });

                    await _productService.UpdateProductAsync(product);
                }
            }

            ViewBag.RefreshPage = true;

            return View(new DiscountProductSearchModel());
        }
        #endregion

        #region Add Category To Discount
        public async Task<IActionResult> CategoryList(int discountId)
        {
            ArgumentNullException.ThrowIfNull(await _discountService.GetByIdAsync(discountId));

            var discoutCategories = (await _categoryService.GetCategoriesByAppliedDiscountAsync(discountId));

            var model = _mapper.Map<List<DiscountCategoryModel>>(discoutCategories);

            return this.JsonWithPascalCase(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryDelete(int discountId, int categoryId)
        {

            //try to get a discount with the specified id
            var discount = await _discountService.GetByIdAsync(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a product with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(categoryId)
                ?? throw new ArgumentException("No category found with the specified id", nameof(categoryId));

            var mapping = await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discountId);

            //remove discount
            if (mapping is DiscountCategoryMapping discountCategoryMapping)
                await _categoryService.DeleteDiscountCategoryMappingAsync(mapping.Id);

            await _categoryService.UpdateCategoryAsync(category);

            return Json(new { success = true, message = "Success" });
        }
        public async Task<IActionResult> AddCategoryToDiscount(int discountId)
        {
            var discount = await _discountService.GetByIdAsync(discountId) ??
                throw new ArgumentException("Not found with the specified id");

            return View();
        }

        public async Task<IActionResult> AddCategoryToDiscountList()
        {
            var json = await _categoryService.GetAllCategoryAsync();

            return this.JsonWithPascalCase(json);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryToDiscount(AddCategoryDiscountModel model)
        {
            if (!model.SelectedCategoryIds.Any())
            {
                return View(new DiscountSearchModel());
            }

            var discount = await _discountService.GetByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

            var selectedCategories = await _categoryService.GetCategoriesByIdsAsync(model.SelectedCategoryIds.ToList());

            if (selectedCategories.Any())
            {
                foreach (var category in selectedCategories)
                {
                    if (await _categoryService.GetDiscountAppliedToCategoryAsync(category.Id, discount.Id) is null)
                        await _categoryService.CreateDiscountCategoryMappingAsync(new DiscountCategoryMapping { EntityId = category.Id, DiscountId = discount.Id });

                    await _categoryService.UpdateCategoryAsync(category);
                }
            }

            ViewBag.RefreshPage = true;

            return View();
        }
        #endregion

        #region Add Manufacturer To Discount
        public async Task<IActionResult> ManufacturerList(int discountId)
        {
            ArgumentNullException.ThrowIfNull(await _discountService.GetByIdAsync(discountId));

            var discoutManufacturer = (await _manufacturerService.GetManufacturersByAppliedDiscountAsync(discountId));

            var model = _mapper.Map<List<DiscountManufacturerModel>>(discoutManufacturer);

            return this.JsonWithPascalCase(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerDelete(int discountId, int manufacturerId)
        {

            //try to get a discount with the specified id
            var discount = await _discountService.GetByIdAsync(discountId)
                ?? throw new ArgumentException("No discount found with the specified id", nameof(discountId));

            //try to get a product with the specified id
            var manufactuer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId)
                ?? throw new ArgumentException("No category found with the specified id", nameof(manufacturerId));

            var mapping = await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufactuer.Id, discountId);

            //remove discount
            if (mapping is DiscountManufacturerMapping discountManufacturerMapping)
                await _manufacturerService.DeleteDiscountManufacturerMappingAsync(mapping.Id);

            await _manufacturerService.UpdateManufacturerAsync(manufactuer);

            return Json(new { success = true, message = "Success" });
        }
        public async Task<IActionResult> AddManufacturerToDiscount(int discountId)
        {
            var discount = await _discountService.GetByIdAsync(discountId) ??
                throw new ArgumentException("Not found with the specified id");

            return View();
        }

        public async Task<IActionResult> AddManufacturerToDiscountList()
        {
            var json = await _manufacturerService.GetAllManufacturerAsync();

            return this.JsonWithPascalCase(json);
        }

        [HttpPost]
        public async Task<IActionResult> AddManufacturerToDiscount(AddManufacturerDiscountModel model)
        {
            if (!model.SelectedManufacturerIds.Any())
            {
                return View(new DiscountSearchModel());
            }

            var discount = await _discountService.GetByIdAsync(model.DiscountId)
            ?? throw new ArgumentException("No discount found with the specified id");

            var selectedManufacturers = await _manufacturerService.GetManufacturersByIdsAsync(model.SelectedManufacturerIds.ToList());

            if (selectedManufacturers.Any())
            {
                foreach (var manufactuer in selectedManufacturers)
                {
                    if (await _manufacturerService.GetDiscountAppliedToManufacturerAsync(manufactuer.Id, discount.Id) is null)
                        await _manufacturerService.CreateDiscountManufacturerMappingAsync(new DiscountManufacturerMapping { EntityId = manufactuer.Id, DiscountId = discount.Id });

                    await _manufacturerService.UpdateManufacturerAsync(manufactuer);
                }
            }

            ViewBag.RefreshPage = true;

            return View();
        }
        #endregion
    }
}
