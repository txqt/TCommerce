using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Web.Services.ProductService;

namespace T.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/product/[action]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(ProductParameters productParameters)
        {
            ViewBag.SearchText = productParameters.searchText != null ? productParameters.searchText : null;
            ViewBag.OrderBy = productParameters.OrderBy != null ? productParameters.OrderBy : null;
            ViewBag.PageSize = productParameters.PageSize;
            ViewBag.PageNumber = productParameters.PageNumber;
            var result = await _productService.GetAll(productParameters);
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            Product product = new Product()
            {
                MarkAsNew = false,
            };
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            var result = await _productService.CreateProduct(product);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(product);
            }

            return View();
        }
    }
}
