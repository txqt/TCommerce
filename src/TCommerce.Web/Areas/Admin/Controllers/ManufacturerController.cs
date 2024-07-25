using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Web.Areas.Admin.Models.Catalog;

using TCommerce.Web.Areas.Admin.Services.PrepareAdminModel;
using TCommerce.Web.Extensions;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/manufacturer/[action]")]
    public class ManufacturerController : BaseAdminController
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IAdminManufacturerModelService _manufacturerModelService;
        private readonly IUrlRecordService _urlRecordService;

        public ManufacturerController(IManufacturerService manufacturerService, IProductService productService, IMapper mapper, ICategoryService categoryService, IAdminManufacturerModelService manufacturerModelService, IUrlRecordService urlRecordService)
        {
            _manufacturerService = manufacturerService;
            _productService = productService;
            _mapper = mapper;
            _categoryService = categoryService;
            _manufacturerModelService = manufacturerModelService;
            _urlRecordService = urlRecordService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var manufacturers = await _manufacturerService.GetAllManufacturerAsync();

            _mapper.Map<List<ManufacturerModel>>(manufacturers);

            return this.JsonWithPascalCase(manufacturers);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(await _manufacturerModelService.PrepareManufacturerModelAsync(new ManufacturerModel(), null));
        }
        [HttpPost]
        public async Task<IActionResult> Create(ManufacturerModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(model.Id);

            _mapper.Map(model, manufacturer);

            var result = await _manufacturerService.CreateManufacturerAsync(manufacturer);

            var seName = await _urlRecordService.ValidateSlug(manufacturer, model.SeName ?? "", manufacturer.Name, true);

            await _urlRecordService.SaveSlugAsync(manufacturer, seName);

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
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);

            ArgumentNullException.ThrowIfNull(manufacturer);

            var model = await _manufacturerModelService.PrepareManufacturerModelAsync(new ManufacturerModel(), manufacturer);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ManufacturerModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(model.Id);

            ArgumentNullException.ThrowIfNull(manufacturer);

            var result = await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            if (model.SeName is not null && model.SeName != (await _urlRecordService.GetSeNameAsync(manufacturer)))
            {
                model.SeName = await _urlRecordService.ValidateSlug(manufacturer, model.SeName, manufacturer.Name, true);

                await _urlRecordService.SaveSlugAsync(manufacturer, model.SeName);
            }

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                return View(model);
            }
            else
            {
                SetStatusMessage("Sửa thành công");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {

            var result = await _manufacturerService.DeleteManufacturerByIdAsync(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }

        public async Task<IActionResult> GetProductManufacturerMapping(int manufacturerId)
        {
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId) ??
              throw new ArgumentException("Not found with the specified id");

            var productManufacturerList = (await _manufacturerService.GetProductManufacturersByManufacturerIdAsync(manufacturerId));

            var models = _mapper.Map<List<ProductManufacturerModel>>(productManufacturerList);

            await Task.WhenAll(models.Select(async model =>
            {
                var product = await _productService.GetByIdAsync(model.ProductId);
                model.ProductName = product?.Name;
            }));

            return this.JsonWithPascalCase(models);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteManufacturerMapping(int id)
        {

            var result = await _manufacturerService.DeleteManufacturerMappingById(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }

        public async Task<IActionResult> AddProductToManufacturer(int manufacturerId)
        {
            //var Manufacturer = await _manufacturerService.GetManufacturerByIdAsync(ManufacturerId) ??
            //    throw new ArgumentException("Not found with the specified id");
            ArgumentNullException.ThrowIfNull(await _manufacturerService.GetManufacturerByIdAsync(manufacturerId));

            var model = new ProductManufacturerSearchModel();

            model = await _manufacturerModelService.PrepareAddProductToManufacturerSearchModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductList(ProductManufacturerSearchModel searchModel)
        {
            var productParameters = ExtractQueryStringParameters<ProductParameters>();

            productParameters.CategoryIds = new List<int> { searchModel.SearchByCategoryId };
            productParameters.ManufacturerIds = new List<int> { searchModel.SearchByManufacturerId };

            var pagedList = await _productService.SearchProductsAsync(productParameters);

            var pagingResponse = new PagingResponse<Product>()
            {
                Items = pagedList,
                MetaData = pagedList.MetaData
            };

            var model = ToDatatableReponse<Product>(pagingResponse.MetaData.TotalCount, pagingResponse.MetaData.TotalCount, pagingResponse.Items);

            return this.JsonWithPascalCase(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToManufacturer(AddProductManufacturerModel model)
        {
            if (!model.SelectedProductIds.Any())
            {
                return View(new ProductSearchModel());
            }

            var existingProductManufacturers = await _manufacturerService.GetProductManufacturersByManufacturerIdAsync(model.ManufacturerId);

            var productManufacturersToAdd = model.SelectedProductIds.Except(existingProductManufacturers.Select(pc => pc.ProductId))
                .Select(pid => new ProductManufacturer
                {
                    ManufacturerId = model.ManufacturerId,
                    ProductId = pid,
                    IsFeaturedProduct = false,
                    DisplayOrder = 1
                }).ToList();

            var result = await _manufacturerService.BulkCreateProductManufacturersAsync(productManufacturersToAdd);

            if (!result.Success)
            {
                return View(new ProductManufacturerSearchModel());
            }
            else
            {
                SetStatusMessage("Thêm thành công");
                ViewBag.RefreshPage = true;
            }

            return View(new ProductManufacturerSearchModel());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductManufacturer([FromBody] ProductManufacturer model)
        {
            var productManufacturer = await _manufacturerService.GetProductManufacturerByIdAsync(model.Id) ??
                throw new ArgumentException("Not found with the specified id");

            productManufacturer.IsFeaturedProduct = model.IsFeaturedProduct;
            productManufacturer.DisplayOrder = model.DisplayOrder;

            var result = await _manufacturerService.UpdateProductManufacturerAsync(productManufacturer);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}
