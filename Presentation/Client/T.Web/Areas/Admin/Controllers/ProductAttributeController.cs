using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.Web.Attribute;
using T.Web.Services.ProductService;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/product-attribute/[action]")]
    //[CustomAuthorizationFilter(RoleName.Admin)]
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
            return View(new ProductAttribute());
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _productAttributeService.GetAllProductAttributeAsync();
            return Json(new { data = result });
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
            return View(result.Data);
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
            return View(result.Data);
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
