using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Text.Json;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Catalogs;
using TCommerce.Core.Models.Response;
using TCommerce.Core.Models.Security;
using TCommerce.Services.UrlRecordServices;
using TCommerce.Web.Areas.Admin.Models.Catalog;
using TCommerce.Web.Areas.Admin.Services.PrepareModel;
using TCommerce.Web.Attribute;
using TCommerce.Web.Extensions;

namespace TCommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/admin/product/[action]")]
    //[CustomAuthorizationFilter(RoleName.Admin)]
    [CheckPermission(PermissionSystemName.ManageProducts)]
    public class ProductController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IMapper _mapper;
        private readonly IAdminProductModelService _prepareModelService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICategoryService _categoryService;
        private readonly JsonSerializerOptions _options;
        private readonly IUrlRecordService _urlRecordService;
        public ProductController(IProductService productService, IMapper mapper, IProductAttributeService productAttributeService,
          IAdminProductModelService prepareModelService, IProductCategoryService productCategoryService, ICategoryService categoryService, JsonSerializerOptions options, IUrlRecordService urlRecordService)
        {
            _productService = productService;
            _mapper = mapper;
            _productAttributeService = productAttributeService;
            _prepareModelService = prepareModelService;
            _productCategoryService = productCategoryService;
            _categoryService = categoryService;
            _options = options;
            _urlRecordService = urlRecordService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ProductSearchModel();

            model = await _prepareModelService.PrepareProductSearchModelModelAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetAll(ProductSearchModel searchModel)
        {
            var productParameters = ParseQueryStringParameters<ProductParameters>();

            productParameters.CategoryIds = new List<int> { searchModel.CategoryId };
            productParameters.ManufacturerIds = new List<int> { searchModel.ManufacturerId };

            var response = await _productService.SearchProductsAsync(pageNumber: productParameters.PageNumber,
                pageSize: productParameters.PageSize,
                categoryIds: productParameters.CategoryIds,
                manufacturerIds: productParameters.ManufacturerIds,
                excludeFeaturedProducts: productParameters.ExcludeFeaturedProducts,
                priceMin: productParameters.PriceMin, priceMax: productParameters.PriceMax,
                productTagId: productParameters.ProductTagId,
                keywords: productParameters.SearchText,
                searchDescriptions: productParameters.SearchDescriptions,
                searchManufacturerPartNumber: productParameters.SearchManufacturerPartNumber,
                productParameters.SearchSku,
                productParameters.SearchProductTags,
                productParameters.OrderBy,
                productParameters.ShowHidden,
                productParameters.ids);

            var pagingResponse = new PagingResponse<Product>
            {
                Items = response,
                MetaData = response.MetaData
            };

            var model = ToDatatableReponse<Product>(pagingResponse.MetaData.TotalCount, pagingResponse.MetaData.TotalCount, pagingResponse.Items);

            return this.JsonWithPascalCase(model);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            ProductModel model = new ProductModel()
            {
                Name = "",
                MarkAsNew = true,
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

            model.SeName = await _urlRecordService.ValidateSlug(product, model.SeName ?? "", product.Name, true);

            await _urlRecordService.SaveSlugAsync(product, model.SeName);

            var result = await _productService.CreateProductAsync(product);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            ArgumentNullException.ThrowIfNull(product);

            var model = await _prepareModelService.PrepareProductModelModelAsync(product, null);

            if(model.AvailableCategories?.Count > 0)
            {
                foreach (var item in model.AvailableCategories)
                {
                    if(item.Value != "0")
                    {
                        if ((await _categoryService.GetProductCategoriesByCategoryIdAsync(int.Parse(item.Value))).FirstOrDefault(x=>x.ProductId == product.Id) is not null)
                            item.Selected = true;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = _mapper.Map<Product>(model);

            var result = await _productService.UpdateProductAsync(product);

            if (model.SeName is not null && model.SeName != (await _urlRecordService.GetSeNameAsync(product)))
            {
                model.SeName = await _urlRecordService.ValidateSlug(product, model.SeName, product.Name, true);

                await _urlRecordService.SaveSlugAsync(product, model.SeName);
            }

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                return View(model);
            }

            SetStatusMessage("Sửa thành công");
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

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {

            if (selectedIds == null || !selectedIds.Any())
                return NoContent();

            var result = await _productService.BulkDeleteProductsAsync(selectedIds);

            return Json(new { Result = result.Success });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllAttribute(int productId)
        //{
        //    var result = await _productService.GetAllProductAttributeByProductIdAsync(productId);
        //    return Json(result);
        //}

        [HttpGet]
        public virtual async Task<IActionResult> ProductAttributeMappingCreate(int productId)
        {
            //try to get a product with the specified id
            var product = (await _productService.GetByIdAsync(productId)) ??
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

            var product = (await _productService.GetByIdAsync(model.ProductId)) ??
              throw new ArgumentException("No product found with the specified id");

            if ((await _productAttributeService.GetProductAttributesMappingByProductIdAsync(product.Id))
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

            productAttributeMapping = (await _productAttributeService.GetProductAttributesMappingByProductIdAsync(product.Id))
              .Where(x => x.ProductAttributeId == model.ProductAttributeId).FirstOrDefault() ??
              throw new ArgumentException("No product attribute mapping found with the specified id");

            SetStatusMessage($"Thêm thành công !");
            return RedirectToAction("EditProductAttributeMapping", new
            {
                id = productAttributeMapping.Id
            });
        }

        [HttpGet]
        public async Task<IActionResult> EditProductAttributeMapping(int id)
        {
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(id)) ??
              throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = (await _productService.GetByIdAsync(productAttributeMapping.ProductId)) ??
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
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(model.Id)) ??
              throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = (await _productService.GetByIdAsync(productAttributeMapping.ProductId)) ??
              throw new ArgumentException("No product found with the specified id");

            ModelState.AddModelError(string.Empty, "This product has mapped with this attribute");

            if ((await _productAttributeService.GetProductAttributesMappingByProductIdAsync(product.Id))
              .Any(x => x.ProductAttributeId == model.ProductAttributeId && x.Id != productAttributeMapping.Id))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thuộc tính này");
                model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(model, product, productAttributeMapping);
                return View(model);
            }

            _mapper.Map(model, productAttributeMapping);

            var result = await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareProductAttributeMappingModelAsync(model, product, productAttributeMapping);
                return View(model);
            }

            SetStatusMessage($"Sửa thành công !");
            return RedirectToAction("EditProductAttributeMapping", new
            {
                id = productAttributeMapping.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductAttributeMapping(int id)
        {

            var result = await _productAttributeService.DeleteProductAttributeMappingByIdAsync(id);
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
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeMappingId)) ??
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
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(model.ProductAttributeMappingId));
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
            var productAttributeValue = (await _productAttributeService.GetProductAttributeValuesByIdAsync(id));
            if (productAttributeValue == null)
                return RedirectToAction(nameof(Index));

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId));
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
            var productAttributeValue = (await _productAttributeService.GetProductAttributeValuesByIdAsync(model.Id));
            if (productAttributeValue == null)
                return RedirectToAction(nameof(Index));

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = (await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId));
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
                var result = await _productAttributeService.UpdateProductAttributeValueAsync(productAttributeValue);

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
        public async Task<IActionResult> DeleteProductAttributeValue(int id)
        {

            var result = await _productAttributeService.DeleteProductAttributeValueAsync(id);
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
        public async Task<IActionResult> GetListProductAttributeMapping(int productId)
        {
            var product = (await _productService.GetByIdAsync(productId)) ??
              throw new ArgumentException("No product found with the specified id");

            var model = await _prepareModelService.PrepareProductAttributeMappingListModelAsync(product);

            return this.JsonWithPascalCase(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetValueProductMapping(int id)
        {
            var productAttributeMappingResponse = (await _productAttributeService.GetProductAttributeMappingByIdAsync(id)) ??
              throw new ArgumentException("Not found with the specified id");

            var model = await _prepareModelService.PrepareProductAttributeValueListModelAsync(productAttributeMappingResponse);

            return this.JsonWithPascalCase(model);
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

        [HttpPost]
        public async Task<ActionResult> UpdateProductImage([FromBody] ProductPictureModel model)
        {
            var productPicture = new ProductPicture()
            {
                DisplayOrder = model.DisplayOrder,
            };

            var result = await _productService.EditProductImageAsync(productPicture);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> ListPhotos(int productId)
        {
            var product = (await _productService.GetByIdAsync(productId)) ??
              throw new ArgumentException("No product found with the specified id");

            var listphotos = await _prepareModelService.PrepareProductPictureModelAsync(product);

            return this.JsonWithPascalCase(listphotos);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int id)
        {

            var result = await _productService.DeleteProductImage(id);
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

        public async Task<IActionResult> GetListCategoryMapping(int productId)
        {
            var product = (await _productService.GetByIdAsync(productId)) ??
              throw new ArgumentException("No product found with the specified id");

            var productCategoryList = (await _productCategoryService.GetProductCategoriesByProductId(product.Id));

            var model = _mapper.Map<List<ProductCategoryModel>>(productCategoryList);

            foreach (var item in model)
            {
                var category = (await _categoryService.GetCategoryByIdAsync(item.CategoryId));
                item.CategoryName = category.ParentCategoryId > 0 ? category.Name
                    + " >>> " + (await _categoryService.GetCategoryByIdAsync(category.ParentCategoryId)).Name : category.Name;
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
            var product = (await _productService.GetByIdAsync(productId)) ??
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

            var category = (await _categoryService.GetCategoryByIdAsync(model.CategoryId)) ??
              throw new ArgumentException("No category found with the specified id");

            var product = (await _productService.GetByIdAsync(model.ProductId)) ??
              throw new ArgumentException("No product found with the specified id");

            ModelState.AddModelError(string.Empty, "This product has mapped with this category");

            if ((await _productCategoryService.GetProductCategoriesByProductId(product.Id))
              .Any(x => x.CategoryId == category.Id && x.Id != model.Id))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thể loại này");
                model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(model, product, null);
                return View(model);
            }

            var productCategory = _mapper.Map<ProductCategory>(model);

            var result = await _productCategoryService.CreateProductCategoryAsync(productCategory);

            if (!result.Success)
            {
                SetStatusMessage($"{result.Message}");
                model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(model, product, productCategory);
                return View(model);
            }

            productCategory = (await _productCategoryService.GetProductCategoriesByProductId(product.Id))
                .Where(x => x.CategoryId == model.CategoryId).FirstOrDefault() ??
              throw new ArgumentException("No product category found with the specified id");

            SetStatusMessage($"Thêm thành công !");
            return RedirectToAction("EditProductCategory", new
            {
                productCategoryId = productCategory.Id
            });
        }

        [HttpGet]
        public async Task<IActionResult> EditProductCategory(int id)
        {
            var productCategory = (await _productCategoryService.GetProductCategoryById(id)) ??
              throw new ArgumentException("No product category mapping found with the specified id");

            var category = (await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId)) ??
              throw new ArgumentException("No category found with the specified id");

            var product = (await _productService.GetByIdAsync(productCategory.ProductId)) ??
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
            var productCategory = (await _productCategoryService.GetProductCategoryById(model.Id)) ??
              throw new ArgumentException("No product category mapping found with the specified id");

            var category = (await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId)) ??
              throw new ArgumentException("No category found with the specified id");

            var product = (await _productService.GetByIdAsync(productCategory.ProductId)) ??
              throw new ArgumentException("No product found with the specified id");

            ModelState.AddModelError(string.Empty, "This product has mapped with this category");

            if ((await _productCategoryService.GetProductCategoriesByProductId(product.Id))
              .Any(x => x.CategoryId == category.Id && x.Id != productCategory.Id))
            {
                SetStatusMessage($"Sản phẩm [{product.Name}] đã liên kết với thể loại này");
                model = await _prepareModelService.PrepareProductCategoryMappingModelAsync(model, product, productCategory);
                return View(model);
            }

            //_mapper.Map(model, productCategory);
            productCategory.ProductId = model.Id;
            productCategory.CategoryId = model.CategoryId;
            productCategory.IsFeaturedProduct = model.IsFeaturedProduct;
            productCategory.DisplayOrder = model.DisplayOrder;

            var result = await _productCategoryService.UpdateProductCategoryAsync(productCategory);

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
        public async Task<IActionResult> DeleteProductCategory(int id)
        {

            var result = await _productCategoryService.DeleteProductCategoryAsync(id);
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

        #region RelatedProduct
        [HttpPost]
        public async Task<IActionResult> DeleteRelatedProductAsync(int id)
        {
            var result = await _productService.DeleteRelatedProductAsync(id);

            return ReturnJsonByServiceResult(result);
        }
        [HttpGet()]
        public async Task<IActionResult> GetRelatedProductsByProductId1Async(int productId)
        {
            var result = await _productService.GetRelatedProductsByProductId1Async(productId, showHidden: true);

            var model = new List<RelatedProductModel>();
            foreach(var item in result)
            {
                model.Add(new RelatedProductModel()
                {
                    Id = item.Id,
                    ProductId2 = item.Id,
                    Product2Name = (await _productService.GetByIdAsync(item.ProductId2)).Name,
                    DisplayOrder = item.DisplayOrder
                });
            }

            return this.JsonWithPascalCase(model);
        }
        [HttpGet()]
        public async Task<IActionResult> GetRelatedProductByIdAsync(int relatedProductId)
        {
            return Ok(await _productService.GetRelatedProductByIdAsync(relatedProductId));
        }

        [HttpGet]
        public async Task<IActionResult> AddRelatedProductPopup(int productId)
        {
            ArgumentNullException.ThrowIfNull(await _productService.GetByIdAsync(productId));

            var model = new RelatedProductSearchModel();

            model = await _prepareModelService.PrepareRelatedProductSearchModel(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RelatedProductList(RelatedProductSearchModel searchModel)
        {
            var productParameters = ParseQueryStringParameters<ProductParameters>();

            productParameters.CategoryIds = new List<int> { searchModel.SearchByCategoryId };
            productParameters.ManufacturerIds = new List<int> { searchModel.SearchByManufacturerId };

            var pagedList = await _productService.SearchProductsAsync(productParameters);

            var pagingResponse = new PagingResponse<Product>()
            {
                MetaData = pagedList.MetaData,
                Items = pagedList
            };

            var model = ToDatatableReponse<Product>(pagingResponse.MetaData.TotalCount, pagingResponse.MetaData.TotalCount, pagingResponse.Items);

            return this.JsonWithPascalCase(model);
        }

        [HttpPost()]
        public async Task<IActionResult> AddRelatedProductPopup(AddRelatedProductModel model)
        {
            var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToList());

            if (selectedProducts.Any())
            {
                var existingRelatedProducts = await _productService.GetRelatedProductsByProductId1Async(model.ProductId, showHidden: true);
                foreach (var product in selectedProducts)
                {

                    if (existingRelatedProducts.FirstOrDefault(rp => rp.ProductId1 == model.ProductId && rp.ProductId2 == product.Id) != null)
                        continue;

                    await _productService.CreateRelatedProductAsync(new RelatedProduct
                    {
                        ProductId1 = model.ProductId,
                        ProductId2 = product.Id,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new RelatedProductSearchModel());
        }


        [HttpPost]
        public async Task<IActionResult> UpdateRelatedProductAsync([FromBody] RelatedProduct model)
        {
            var relatedProduct = await _productService.GetRelatedProductByIdAsync(model.Id) ??
                throw new ArgumentException("Not found with the specified id");

            ArgumentNullException.ThrowIfNull(relatedProduct);

            relatedProduct.DisplayOrder = model.DisplayOrder;

            var result = await _productService.UpdateRelatedProductAsync(relatedProduct);

            return result.Success ? Json(new
            {
                success = true, message = result.Message
            }) 
                : 
            Json(new
            {
                success = false, message = result.Message
            });
        }
        #endregion

        private JsonResult ReturnJsonByServiceResult<T>(ServiceResponse<T> response)
        {
            return response.Success ? Json(new
            {
                success = true,
                message = response.Message
            })
                :
            Json(new
            {
                success = false,
                message = response.Message
            });
        }
    }
}