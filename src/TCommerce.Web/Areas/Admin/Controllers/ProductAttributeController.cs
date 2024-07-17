using Microsoft.AspNetCore.Mvc;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Security;
using TCommerce.Web.Areas.Admin.Models;
using TCommerce.Web.Attribute;
using TCommerce.Web.Extensions;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/product-attribute/[action]")]
    [CheckPermission(PermissionSystemName.ManageAttributes)]
    public class ProductAttributeController : BaseAdminController
    {
        private readonly IProductAttributeService _productAttributeService;
        public ProductAttributeController(IProductAttributeService productAttributeService)
        {
            _productAttributeService = productAttributeService;
        }

        public IActionResult Index()
        {
            var model = new DataTableViewModel
            {
                TableTitle = "Danh sách thuộc tính sản phẩm",
                CreateUrl = Url.Action("Create", "ProductAttribute"),
                EditUrl = Url.Action("Edit", "ProductAttribute"),
                DeleteUrl = Url.Action("Delete", "ProductAttribute"),
                GetDataUrl = Url.Action("GetAll", "ProductAttribute"),
                Columns = new List<ColumnDefinition>
                {
                    new ColumnDefinition(nameof(ProductAttribute.Name)) {Title = DisplayNameExtensions.GetPropertyDisplayName<ProductAttribute>(m => m.Name) },
                    new ColumnDefinition(nameof(ProductAttribute.Description)) {Title = DisplayNameExtensions.GetPropertyDisplayName<ProductAttribute>(m => m.Description) },
                    new ColumnDefinition(nameof(ProductAttribute.Id)) { Title = "Edit", RenderType = RenderType.RenderButtonEdit },
                    new ColumnDefinition(nameof(ProductAttribute.Id)) { Title = "Delete", RenderType = RenderType.RenderButtonRemove },
                }
            };
            return View(model);
        }

        public IActionResult test()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _productAttributeService.GetAllProductAttributeAsync();

            return this.JsonWithPascalCase(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductAttribute productAttribute)
        {
            if (!ModelState.IsValid)
            {
                return View(productAttribute);
            }
            var result = await _productAttributeService.CreateProductAttributeAsync(productAttribute);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(productAttribute);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _productAttributeService.GetProductAttributeByIdAsync(id);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductAttribute productAttribute)
        {
            if (!ModelState.IsValid)
            {
                return View(productAttribute);
            }
            var result = await _productAttributeService.UpdateProductAttributeAsync(productAttribute);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(productAttribute);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {

            var result = await _productAttributeService.DeleteProductAttributeByIdAsync(id);

            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }

            return Json(new { success = true, message = result.Message });
        }
    }
}
