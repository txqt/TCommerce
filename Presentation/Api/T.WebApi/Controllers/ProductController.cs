using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using T.Library.Model;
using T.Library.Model.Catalogs;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
using T.WebApi.Services.CategoryServices;
using T.WebApi.Services.ProductServices;

namespace T.WebApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    //[CustomAuthorizationFilter(RoleName.Admin)]
    [CheckPermission(PermissionSystemName.ManageProducts)]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ISecurityService _permissionRecordService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public ProductController(IProductService productService, ISecurityService permissionRecordService, IMapper mapper, IProductAttributeService productAttributeService, IProductCategoryService productCategoryService, ICategoryService categoryService)
        {
            _productService = productService;
            _permissionRecordService = permissionRecordService;
            _mapper = mapper;
            _productAttributeService = productAttributeService;
            _productCategoryService = productCategoryService;
            _categoryService = categoryService;
        }

        #region Product
        [HttpGet("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> SearchProduct([FromQuery] ProductParameters productParameters)
        {
            var products = await _productService.SearchProduct(pageNumber: productParameters.PageNumber,
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

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(products.MetaData));

            return Ok(products);
        }

        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> Get(int productId)
        {
            return await _productService.GetByIdAsync(productId);
        }

        [HttpGet("get-by-ids")]
        [AllowAnonymous]
        public async Task<ActionResult> GetProductsByIdsAsync([FromQuery] List<int> ids)
        {
            return Ok(await _productService.GetProductsByIdsAsync(ids));
        }

        [HttpPost("")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateProduct(ProductModel model)
        {
            //if (!await _permissionRecordService.AuthorizeAsync(DefaultPermission.ManageProducts))
            //    return Forbid();

            var result = await _productService.CreateProductAsync(model);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> EditProduct(ProductModel model)
        {
            var result = await _productService.EditProductAsync(model);

            await SaveCategoryMappingsAsync(model);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{productId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteProduct(int productId)
        {
            var result = await _productService.DeleteProductAsync(productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("delete-list")]
        public async Task<IActionResult> DeleteProductList(IEnumerable<int> productIds)
        {
            var result = await _productService.BulkDeleteProductsAsync(productIds);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("show-on-home-page")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAllProductsDisplayedOnHomepageAsync()
        {
            var result = await _productService.GetAllProductsDisplayedOnHomepageAsync();
            return Ok(result);
        }

        [HttpGet("features/by-categoryId/{categoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetCategoryFeaturedProductsAsync(int categoryId)
        {
            var result = await _productService.GetCategoryFeaturedProductsAsync(categoryId);
            return Ok(result);
        }
        #endregion

        #region ProductPicture
        [HttpGet("{productId}/pictures")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductPicture>>> GetProductPicturesByProductIdAsync(int productId)
        {
            return await _productService.GetProductPicturesByProductIdAsync(productId);
        }

        [HttpPost("{productId}/pictures")]
        public async Task<ActionResult> AddProductImage(List<IFormFile> formFiles, int productId)
        {
            var result = await _productService.AddProductImage(formFiles, productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("pictures")]
        public async Task<ActionResult> EditProductImage(ProductPicture productPicture)
        {
            var product = await _productService.GetByIdAsync(productPicture.ProductId);

            ArgumentNullException.ThrowIfNull(product);

            var result = await _productService.EditProductImageAsync(productPicture);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("pictures/{pictureMappingId}")]
        public async Task<ActionResult> DeleteProductImage(int pictureMappingId)
        {
            var result = await _productService.DeleteProductImage(pictureMappingId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{productId}/pictures")]
        public async Task<ActionResult> DeleteAllPictureProduct(int productId)
        {
            var result = await _productService.DeleteAllProductImage(productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region RelatedProduct
        [HttpDelete("/api/related-products/{relatedProductId}")]
        public async Task<IActionResult> DeleteRelatedProductAsync(int relatedProductId)
        {
            var result = await _productService.DeleteRelatedProductAsync(relatedProductId);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{productId}/related-products")]
        public async Task<IActionResult> GetRelatedProductsByProductId1Async(int productId, [FromQuery] bool showHidden = false)
        {
            return Ok(await _productService.GetRelatedProductsByProductId1Async(productId, showHidden));
        }
        [HttpGet("/api/related-products/{relatedProductId}")]
        public async Task<IActionResult> GetRelatedProductByIdAsync(int relatedProductId)
        {
            return Ok(await _productService.GetRelatedProductByIdAsync(relatedProductId));
        }
        [HttpPost("/api/related-products")]
        public async Task<IActionResult> CreateRelatedProductAsync(RelatedProduct relatedProduct)
        {
            var result = await _productService.CreateRelatedProductAsync(relatedProduct);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPut("/api/related-products")]
        public async Task<IActionResult> UpdateRelatedProductAsync(RelatedProduct relatedProduct)
        {
            var result = await _productService.UpdateRelatedProductAsync(relatedProduct);

            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Common
        protected virtual async Task SaveCategoryMappingsAsync(ProductModel model)
        {
            // retrieve the current list of categories for the product
            var existingProductCategories = await _categoryService.GetProductCategoriesByProductIdAsync(model.Id);

            // remove categories that do not exist in the model
            var categoriesToDelete = existingProductCategories
                .Where(existingProductCategory => !model.CategoryIds.Contains(existingProductCategory.CategoryId))
                .ToList();

            foreach (var categoryToDelete in categoriesToDelete)
                await _categoryService.DeleteProductCategoryMappingById(categoryToDelete.Id);

            // filter the current list of categories after deletion
            existingProductCategories = existingProductCategories
                .Where(item => !model.CategoryIds.Contains(item.Id))
                .ToList();

            // add new categories
            foreach (var categoryId in model.CategoryIds)
            {
                // check if the category already exists for the product
                if (!existingProductCategories.Any(pc => pc.ProductId == model.Id && pc.CategoryId == categoryId))
                {
                    var displayOrder = 1;

                    // check and calculate display order
                    var existingCategoryMapping = await _categoryService.GetProductCategoriesByCategoryIdAsync(categoryId);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;

                    // create a new ProductCategory object and add it to the list
                    await _categoryService.CreateProductCategoryAsync(new ProductCategory
                    {
                        ProductId = model.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }
        #endregion
    }
}
