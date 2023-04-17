using Microsoft.AspNetCore.Mvc;
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
    }
}
