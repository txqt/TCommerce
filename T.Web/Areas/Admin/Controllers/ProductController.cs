using AutoMapper;
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
    [Route("/admin/product/[action]")]
    [CustomAuthorizationFilter(RoleName.Admin)]
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
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

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var result = await _productService.Get(productId);
            var res = _mapper.Map<ProductUpdateViewModel>(result.Data);
            return View(res);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductUpdateViewModel product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            var result = await _productService.EditProduct(product);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(product);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
