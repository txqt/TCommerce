using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.ViewsModel;
using T.Web.Areas.Admin.Models;
using T.Web.Attribute;
using T.Web.Controllers;
using T.Web.Services.CategoryService;
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
        //private readonly IProductAttributeMappingService _productAttributeService;
        //private readonly IProductAttributeValueService _productAttributeService;
        private readonly IMapper _mapper;
        private readonly IProductModelService _prepareModelService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICategoryService _categoryService;
        public ProductController(IProductService productService, IMapper mapper, IProductAttributeService productAttributeService,
          //IProductAttributeMappingService productAttributeMappingService, IProductAttributeValueService productAttributeValueService,
          IProductModelService prepareModelService, IProductCategoryService productCategoryService, ICategoryService categoryService)
        {
            _productService = productService;
            _mapper = mapper;
            _productAttributeService = productAttributeService;
            //_productAttributeService = productAttributeMappingService;
            //_productAttributeService = productAttributeValueService;
            _prepareModelService = prepareModelService;
            _productCategoryService = productCategoryService;
            _categoryService = categoryService;
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
        public IActionResult CreateProduct()
        {
            ProductModel model = new ProductModel()
            {
                MarkAsNew = false,
                ShowOnHomepage = true,
                Published = true
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var product = _mapper.Map<Product>(model);

            var result = await _productService.CreateProductAsync(product);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var result = await _productService.GetByIdAsync(productId);
            var res = _mapper.Map<ProductModel>(result.Data);
            res.AttributeMappings = result.Data.AttributeMappings.ToList();
            return View(res);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _productService.EditProductAsync(model);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {

            var result = await _productService.DeleteProductAsync(id);

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
            var result = await _productService.GetAllProductAttributeByProductIdAsync(productId);
            return Json(result.Data);
        }

        [HttpGet]
        public virtual async Task<IActionResult> ProductAttributeMappingCreate(int productId)
        {
            //try to get a product with the specified id
            var product = (await _productService.GetByIdAsync(productId)).Data ??
              throw new ArgumentException("No product found with the specified id");

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

            var product = (await _productService.GetByIdAsync(model.ProductId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            if ((await _productAttributeService.GetProductAttributeMappingByProductIdAsync(product.Id)).Data
              .Any(x => x.ProductAttributeId == model.ProductAttributeId))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thuộc tính này");
                model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(model, product, null);
                return View(model);
            }

            var productAttributeMapping = _mapper.Map<ProductAttributeMapping>(model);

            var result = await _productAttributeService.CreateProductAttributeMappingAsync(productAttributeMapping);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                return View(model);
            }

            productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByProductIdAsync(product.Id)).Data
              .Where(x => x.ProductAttributeId == model.ProductAttributeId).FirstOrDefault() ??
              throw new ArgumentException("No product attribute mapping found with the specified id");

            SetStatusMessage($"Thêm thành công !");
            return RedirectToAction("EditProductAttributeMapping", new
            {
                productAttributeMappingId = productAttributeMapping.Id
            });
        }

        [HttpGet]
        public async Task<IActionResult> EditProductAttributeMapping(int productAttributeMappingId)
        {
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeMappingId)).Data ??
              throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = (await _productService.GetByIdAsync(productAttributeMapping.ProductId)).Data ??
              throw new ArgumentException("No product found with the specified id");

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
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(model.Id)).Data ??
              throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = (await _productService.GetByIdAsync(productAttributeMapping.ProductId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            ModelState.AddModelError(string.Empty, "This product has mapped with this attribute");

            if ((await _productAttributeService.GetProductAttributeMappingByProductIdAsync(product.Id)).Data
              .Any(x => x.ProductAttributeId == model.ProductAttributeId && x.Id != productAttributeMapping.Id))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thuộc tính này");
                model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(model, product, productAttributeMapping);
                return View(model);
            }

            productAttributeMapping = _mapper.Map(model, productAttributeMapping);

            var result = await _productAttributeService.CreateProductAttributeMappingAsync(productAttributeMapping);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(model, product, productAttributeMapping);
                return View(model);
            }

            SetStatusMessage($"Sửa thành công !");
            return RedirectToAction("EditProductAttributeMapping", new
            {
                productAttributeMappingId = productAttributeMapping.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductAttributeMapping(int pamId)
        {

            var result = await _productAttributeService.DeleteProductAttributeByIdAsync(pamId);
            if (!result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
            return Json(new
            {
                success = true,
                message = result.Message
            });
        }

        [HttpGet]
        public virtual async Task<IActionResult> ProductAttributeValueCreate(int productAttributeMappingId)
        {
            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeMappingId)).Data ??
              throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductPicturesByProductIdAsync(productAttributeMapping.ProductId) ??
              throw new ArgumentException("No product found with the specified id");

            //prepare model
            var model = await _prepareModelService.PrepareProductAttributeValueModelAsync(new ProductAttributeValueModel(), productAttributeMapping, null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductAttributeValueCreate(ProductAttributeValueModel model)
        {
            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(model.ProductAttributeMappingId)).Data;
            if (productAttributeMapping == null)
                return RedirectToAction(nameof(Index));

            //try to get a product with the specified id
            var product = await _productService.GetByIdAsync(productAttributeMapping.ProductId) ??
              throw new ArgumentException("No product found with the specified id");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var productAttributeValue = _mapper.Map<ProductAttributeValue>(model);
                productAttributeValue.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;
                var result = await _productAttributeService.CreateProductAttributeValueAsync(productAttributeValue);

                if (!result.Success)
                {
                    SetStatusMessage($"{result.Message}");
                    model = await _prepareModelService.PrepareProductAttributeValueModelAsync(model, productAttributeMapping, productAttributeValue);
                    return View(model);
                }

                SetStatusMessage($"Sửa thành công");
                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _prepareModelService.PrepareProductAttributeValueModelAsync(model, productAttributeMapping, null);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProductAttributeValue(int id)
        {
            //try to get a product attribute value with the specified id
            var productAttributeValue = (await _productAttributeService.GetProductAttributeValuesByIdAsync(id)).Data;
            if (productAttributeValue == null)
                return RedirectToAction(nameof(Index));

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId)).Data;
            if (productAttributeMapping == null)
                return RedirectToAction(nameof(Index));

            //try to get a product with the specified id
            var product = await _productService.GetProductPicturesByProductIdAsync(productAttributeMapping.ProductId) ??
              throw new ArgumentException("No product found with the specified id");

            //prepare model
            var model = await _prepareModelService.PrepareProductAttributeValueModelAsync(null, productAttributeMapping, productAttributeValue);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProductAttributeValue(ProductAttributeValueModel model)
        {
            //try to get a product attribute value with the specified id
            var productAttributeValue = (await _productAttributeService.GetProductAttributeValuesByIdAsync(model.Id)).Data;
            if (productAttributeValue == null)
                return RedirectToAction(nameof(Index));

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId)).Data;
            if (productAttributeMapping == null)
                return RedirectToAction(nameof(Index));

            //try to get a product with the specified id
            var product = await _productService.GetByIdAsync(productAttributeMapping.ProductId) ??
              throw new ArgumentException("No product found with the specified id");

            if (ModelState.IsValid)
            {
                //fill entity from model
                productAttributeValue = _mapper.Map(model, productAttributeValue);
                productAttributeValue.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;
                var result = await _productAttributeService.CreateProductAttributeValueAsync(productAttributeValue);

                if (!result.Success)
                {
                    SetStatusMessage($"{result.Message}");
                    model = await _prepareModelService.PrepareProductAttributeValueModelAsync(model, productAttributeMapping, productAttributeValue);
                    return View(model);
                }

                SetStatusMessage($"Sửa thành công");
                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _prepareModelService.PrepareProductAttributeValueModelAsync(model, productAttributeMapping, productAttributeValue);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductAttributeValue(int pavId)
        {

            var result = await _productAttributeService.DeleteProductAttributeValueAsync(pavId);
            if (!result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
            return Json(new
            {
                success = true,
                message = result.Message
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetListProductMapping(int productId)
        {
            var product = (await _productService.GetByIdAsync(productId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            var model = await _prepareModelService.PrepareProductAttributeMappingListModelAsync(product);

            return Json(new
            {
                data = model
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetValueProductMapping(int productAttributeMapping)
        {
            var productAttributeMappingResponse = (await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeMapping)).Data ??
              throw new ArgumentException("Not found with the specified id");

            var model = await _prepareModelService.PrepareProductAttributeValueListModelAsync(productAttributeMappingResponse);

            return Json(new
            {
                data = model
            });
        }

        [HttpPost]
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
        public async Task<IActionResult> ListPhotos(int id)
        {
            var product = (await _productService.GetByIdAsync(id)).Data ??
              throw new ArgumentException("No product found with the specified id");

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
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
            return Json(new
            {
                success = true,
                message = result.Message
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllPhotos(int productId)
        {
            var result = await _productService.DeleteAllProductImage(productId);
            if (!result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
            return Json(new
            {
                success = true,
                message = result.Message
            });
        }

        public async Task<IActionResult> GetListCategoryMapping(ProductCategoryModel productCategoryModel)
        {
            var product = (await _productService.GetByIdAsync(productCategoryModel.ProductId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            var productCategoryList = (await _productCategoryService.GetByProductId(product.Id)).Data;

            var model = _mapper.Map<List<ProductCategoryModel>>(productCategoryList);

            foreach(var item in model)
            {
                var category = (await _categoryService.Get(item.CategoryId)).Data;
                item.CategoryName = category.ParentCategoryId > 0 ? category.Name 
                    + " >>> " + (await _categoryService.Get(category.ParentCategoryId)).Data.Name : category.Name;
            }

            return Json(new
            {
                data = model
            });
        }

        [HttpGet]
        public virtual async Task<IActionResult> ProductCategoryMappingCreate(int productId)
        {
            //try to get a product with the specified id
            var product = (await _productService.GetByIdAsync(productId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            //prepare model
            var model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(new ProductCategoryModel(), product, null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductCategoryMappingCreate(ProductCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = (await _categoryService.Get(model.CategoryId)).Data ??
              throw new ArgumentException("No category found with the specified id");

            var product = (await _productService.GetByIdAsync(model.ProductId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            ModelState.AddModelError(string.Empty, "This product has mapped with this category");

            if ((await _productCategoryService.GetByProductId(product.Id)).Data
              .Any(x => x.CategoryId == category.Id && x.Id != model.Id))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thể loại này");
                model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(model, product, null);
                return View(model);
            }

            var productCategory = _mapper.Map<ProductCategory>(model);

            var result = await _productCategoryService.AddOrEdit(productCategory);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(model, product, productCategory);
                return View(model);
            }

            productCategory = (await _productCategoryService.GetByProductId(product.Id)).Data
                .Where(x=>x.CategoryId == model.CategoryId).FirstOrDefault() ??
              throw new ArgumentException("No product category found with the specified id");

            SetStatusMessage($"Thêm thành công !");
            return RedirectToAction("EditProductCategory", new
            {
                productCategoryId = productCategory.Id
            });
        }

        [HttpGet]
        public async Task<IActionResult> EditProductCategory(int productCategoryId)
        {
            var productCategory = (await _productCategoryService.Get(productCategoryId)).Data ??
              throw new ArgumentException("No product category mapping found with the specified id");

            var category = (await _categoryService.Get(productCategory.CategoryId)).Data ??
              throw new ArgumentException("No category found with the specified id");

            var product = (await _productService.GetByIdAsync(productCategory.ProductId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            var model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(null, product, productCategory);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProductCategory(ProductCategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var productCategory = (await _productCategoryService.Get(model.Id)).Data ??
              throw new ArgumentException("No product category mapping found with the specified id");

            var category = (await _categoryService.Get(productCategory.CategoryId)).Data ??
              throw new ArgumentException("No category found with the specified id");

            var product = (await _productService.GetByIdAsync(productCategory.ProductId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            ModelState.AddModelError(string.Empty, "This product has mapped with this category");

            if ((await _productCategoryService.GetByProductId(product.Id)).Data
              .Any(x => x.CategoryId == category.Id && x.Id != productCategory.Id))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thể loại này");
                model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(model, product, productCategory);
                return View(model);
            }

            productCategory = _mapper.Map(model, productCategory);
            //productCategory.ProductId = model.Id;
            //productCategory.CategoryId = model.CategoryId;
            //productCategory.IsFeaturedProduct = model.IsFeaturedProduct;
            //productCategory.DisplayOrder = model.DisplayOrder;

            var result = await _productCategoryService.AddOrEdit(productCategory);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(model, product, productCategory);
                return View(model);
            }

            SetStatusMessage($"Sửa thành công !");
            return RedirectToAction("EditProductCategory", new
            {
                productCategoryId = productCategory.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductCategory(int productCategoryId)
        {

            var result = await _productCategoryService.Delete(productCategoryId);
            if (!result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result.Message
                });
            }
            return Json(new
            {
                success = true,
                message = result.Message
            });
        }
    }
}