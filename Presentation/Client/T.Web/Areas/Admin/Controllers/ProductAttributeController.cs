using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Attribute;
using T.Web.Extensions;
using T.Web.Services.ProductService;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/product-attribute/[action]")]
    //[CustomAuthorizationFilter(RoleName.Admin)]
    [CheckPermission(PermissionSystemName.ManageAttributes)]
    public class ProductAttributeController : BaseAdminController
    {
        private readonly IProductAttributeCommon _productAttributeService;
        public ProductAttributeController(IProductAttributeCommon productAttributeService)
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
                    new ColumnDefinition { Title = "Edit", RenderType = RenderType.RenderButtonEdit },
                    new ColumnDefinition { Title = "Delete", RenderType = RenderType.RenderButtonRemove },
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
            var json = new { data = result };
            return this.JsonWithPascalCase(json);
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

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productAttributeService.GetProductAttributeByIdAsync(id);
            return View(result);
        }

        //[HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
