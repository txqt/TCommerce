using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Enum;
using T.Library.Model.ViewsModel;
using T.Web.Attribute;
using T.Web.Controllers;
using T.Web.Services.ProductService;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/product-attribute/[action]")]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class ProductAttributeController : BaseController
    {
        private readonly IProductAttributeService _productAttributeService;
        public ProductAttributeController(IProductAttributeService productAttributeService)
        {
            _productAttributeService = productAttributeService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _productAttributeService.GetAll();
            return View(result);
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
            var result = await _productAttributeService.Create(productAttribute);

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
            var result = await _productAttributeService.Get(id);
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductAttribute productAttribute)
        {
            if (!ModelState.IsValid)
            {
                return View(productAttribute);
            }
            var result = await _productAttributeService.Edit(productAttribute);

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
            var result = await _productAttributeService.Get(id);
            return View(result.Data);
        }

        //[HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var result = await _productAttributeService.Delete(id);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
