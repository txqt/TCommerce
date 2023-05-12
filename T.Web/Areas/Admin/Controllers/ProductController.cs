using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using T.Library.Model;
using T.Library.Model.Enum;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Attribute;
using T.Web.Controllers;
using T.Web.Services.PrepareModel;
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
        private readonly IProductAttributeValueService _productAttributeValueService;
        private readonly IMapper _mapper;
        private readonly IPrepareModelService _prepareModelService;
        [TempData]
        public string StatusMessage { get; set; }
        public ProductController(IProductService productService, IMapper mapper, IProductAttributeService productAttributeService, 
            IProductAttributeMappingService productAttributeMappingService, IProductAttributeValueService productAttributeValueService, 
            IPrepareModelService prepareModelService)
        {
            _productService = productService;
            _mapper = mapper;
            _productAttributeService = productAttributeService;
            _productAttributeMappingService = productAttributeMappingService;
            _productAttributeValueService = productAttributeValueService;
            _prepareModelService = prepareModelService;
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
                SetStatusMessage($"{result.Message}");
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
        public virtual async Task<IActionResult> ProductAttributeMappingCreate(int productId)
        {
            //try to get a product with the specified id
            var product = (await _productService.Get(productId)).Data
                ?? throw new ArgumentException("No product found with the specified id");

            //prepare model
            var model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(new ProductAttributeMappingModel(), product, null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductAttributeMappingCreate(ProductAttributeMappingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = (await _productService.Get(model.ProductId)).Data
                ?? throw new ArgumentException("No product found with the specified id");

            if ((await _productAttributeMappingService.GetProductAttributeMappingByProductId(product.Id)).Data
                .Any(x => x.ProductAttributeId == model.ProductAttributeId))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thuộc tính này");
                model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(model, product, null);
                return View(model);
            }

            var productAttributeMapping = _mapper.Map<ProductAttributeMapping>(model);

            var result = await _productAttributeMappingService.AddOrUpdateProductAttributeMapping(productAttributeMapping);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                return View(model);
            }

            productAttributeMapping = (await _productAttributeMappingService.GetProductAttributeMappingByProductId(product.Id)).Data
                .Where(x=>x.ProductAttributeId == model.ProductAttributeId).FirstOrDefault()
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            SetStatusMessage($"Thêm thành công !");
            return RedirectToAction("EditProductAttributeMapping", new { productAttributeMappingId = productAttributeMapping.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EditProductAttributeMapping(int productAttributeMappingId)
        {
            var productAttributeMapping = (await _productAttributeMappingService.GetProductAttributeMapping(productAttributeMappingId)).Data
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = (await _productService.Get(productAttributeMapping.ProductId)).Data
                ?? throw new ArgumentException("No product found with the specified id");

            var model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(null, product, productAttributeMapping);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProductAttributeMapping(ProductAttributeMappingModel model)
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

            

            if ((await _productAttributeMappingService.GetProductAttributeMappingByProductId(product.Id)).Data
                .Any(x => x.ProductAttributeId == model.ProductAttributeId && x.Id != productAttributeMapping.Id))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thuộc tính này");
                model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(model, product, productAttributeMapping);
                return View(model);
            }
            
            productAttributeMapping = _mapper.Map(model, productAttributeMapping);

            var result = await _productAttributeMappingService.AddOrUpdateProductAttributeMapping(productAttributeMapping);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(model, product, productAttributeMapping);
                return View(model);
            }

            SetStatusMessage($"Sửa thành công !");
            return RedirectToAction("EditProductAttributeMapping", new { productAttributeMappingId = productAttributeMapping.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ProductAttributeList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetListProductMapping(int productId)
        {
            var product = (await _productService.Get(productId)).Data
                ?? throw new ArgumentException("No product found with the specified id");

            var model = await _prepareModelService.PrepareProductAttributeMappingListModelAsync(product);

            return Json(new {data = model});
        }

        [HttpGet]
        public async Task<IActionResult> GetValueProductMapping(int productAttributeMapping)
        {
            var productAttributeMappingResponse = (await _productAttributeMappingService.GetProductAttributeMapping(productAttributeMapping)).Data
                ?? throw new ArgumentException("Not found with the specified id");

            var model = await _prepareModelService.PrepareProductAttributeValueListModelAsync(productAttributeMappingResponse);

            return Json(new { data = model });
        }

        [HttpPost("{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult> AddProductImage(List<IFormFile> formFiles, int productId)
        {
            var result = await _productService.AddProductImage(formFiles, productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListPhotos(int id)
        {
            var product = (await _productService.Get(id)).Data
                ?? throw new ArgumentException("No product found with the specified id");

            var listphotos = await _prepareModelService.PrepareProductPictureModelAsync(product);

            return Json(
                new
                {
                    data = listphotos
                });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int productId, int pictureId)
        {

            var result = await _productService.DeleteProductImage(productId, pictureId);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }
    }
}
