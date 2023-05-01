using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;
using T.Library.Model;
using T.Library.Model.Enum;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
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
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeMappingService _productAttributeMappingService;
        private readonly IMapper _mapper;
        public ProductController(IProductService productService, IMapper mapper, IProductAttributeService productAttributeService, IProductAttributeMappingService productAttributeMappingService)
        {
            _productService = productService;
            _mapper = mapper;
            _productAttributeService = productAttributeService;
            _productAttributeMappingService = productAttributeMappingService;
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
            res.AttributeMappings = result.Data.AttributeMappings.ToList();
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
        public async Task<IActionResult> Delete(int id)
        {

            var result = await _productService.DeleteProduct(id);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAttribute(int productId)
        {
            var result = await _productService.GetAllAttribute(productId);
            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> EditProductAttributeValue(int productAttributeMappingId)
        {
            var productAttributeMapping = (await _productAttributeMappingService.GetProductAttributeMapping(productAttributeMappingId)).Data
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = (await _productService.Get(productAttributeMapping.ProductId)).Data
                ?? throw new ArgumentException("No product found with the specified id");

            ProductAttributeMappingModel model = null;

            if (productAttributeMapping != null)
            {
                //fill in model values from the entity
                model ??= new ProductAttributeMappingModel
                {
                    Id = productAttributeMapping.Id
                };

                model.ProductAttributeName = (await _productAttributeService.Get(productAttributeMapping.ProductAttributeId)).Data.Name;
            }

            model.ProductId = product.Id;

            //prepare available product attributes
            model.AvailableProductAttributes = (await _productAttributeService.GetAll()).Select(productAttribute => new SelectListItem
            {
                Text = productAttribute.Name,
                Value = productAttribute.Id.ToString()
            }).ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditProductAttributeValue(ProductAttributeMappingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var productAttributeMapping = (await _productAttributeMappingService.GetProductAttributeMapping(model.Id)).Data
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = (await _productService.Get(productAttributeMapping.ProductId)).Data
                ?? throw new ArgumentException("No product found with the specified id");

            ModelState.AddModelError(string.Empty, "This product has mapped with this attribute");
            if (productAttributeMapping != null)
            {
                //fill in model values from the entity
                model ??= new ProductAttributeMappingModel
                {
                    Id = productAttributeMapping.Id
                };

                model.ProductAttributeName = (await _productAttributeService.Get(productAttributeMapping.ProductAttributeId)).Data.Name;
            }

            model.ProductId = product.Id;

            //prepare available product attributes
            model.AvailableProductAttributes = (await _productAttributeService.GetAll()).Select(productAttribute => new SelectListItem
            {
                Text = productAttribute.Name,
                Value = productAttribute.Id.ToString()
            }).ToList();

            if ((await _productAttributeMappingService.GetProductAttributeMappingByProductId(product.Id)).Data
                .Any(x => x.ProductAttributeId == model.ProductAttributeId || x.Id != productAttributeMapping.Id))
            {
                return View(model);
            }
            
            productAttributeMapping = _mapper.Map<ProductAttributeMappingModel, ProductAttributeMapping>(model);

            var result = await _productAttributeMappingService.AddProductAttributeMapping(productAttributeMapping);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction("EditProductAttributeValue", new { productAttributeMappingId = productAttributeMapping.Id });
        }
    }
}
