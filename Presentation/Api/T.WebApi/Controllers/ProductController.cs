using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using T.Library.Model;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.Library.Model.Roles.RoleName;
using T.Library.Model.Security;
using T.Library.Model.ViewsModel;
using T.WebApi.Attribute;
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
        private readonly IMapper _mapper;
        public ProductController(IProductService productService, ISecurityService permissionRecordService, IMapper mapper, IProductAttributeService productAttributeService, IProductCategoryService productCategoryService)
        {
            _productService = productService;
            _permissionRecordService = permissionRecordService;
            _mapper = mapper;
            _productAttributeService = productAttributeService;
            _productCategoryService = productCategoryService;
        }

        #region Product
        [HttpGet("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetAll([FromQuery] ProductParameters productParameters)
        {
            var products = await _productService.GetAll(productParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(products.MetaData));
            return Ok(products);
        }

        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> Get(int productId)
        {
            return await _productService.GetByIdAsync(productId);
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

        [HttpGet("homepage")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAllProductsDisplayedOnHomepageAsync()
        {
            var result = await _productService.GetAllProductsDisplayedOnHomepageAsync();
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
            var product = await _productService.GetByIdAsync(productPicture.ProductId)
                ?? throw new ArgumentNullException("Not found product");

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

        //#region ProductAttributeMapping
        //[AllowAnonymous]
        //[HttpGet("{productId}/attributes")]
        //public async Task<ActionResult> GetProductAttributesMapping(int productId)
        //{
        //    return Ok(await _productAttributeService.GetProductAttributesMappingByProductIdAsync(productId));
        //}

        //[HttpPost("attributes")]
        //public async Task<ActionResult> CreateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        //{

        //    var result = await _productAttributeService.CreateProductAttributeMappingAsync(productAttributeMapping);
        //    if (!result.Success)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}

        //[HttpPut("attributes")]
        //public async Task<ActionResult> UpdateAttributeMapping(ProductAttributeMapping productAttributeMapping)
        //{
        //    //try to get a product with the specified id
        //    var product = (await _productService.GetByIdAsync(productAttributeMapping.ProductId)) ??
        //      throw new ArgumentException("No product found with the specified id");

        //    var productAttribute = (await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId)) ??
        //        throw new ArgumentException("No product attribute found with the specified id");

        //    var result = await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
        //    if (!result.Success)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}

        //[AllowAnonymous]
        //[HttpGet("attributes/{productAttributeId}")]
        //public async Task<ActionResult<ServiceResponse<ProductAttributeMapping>>> GetProductAttributeMappingByIdAsync(int productAttributeId)
        //{
        //    return Ok(await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeId));
        //}

        //[HttpDelete("attributes/{productAttributeId}")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<ActionResult> DeleteProductAttributeMapping(int productAttributeId)
        //{
        //    var result = await _productAttributeService.DeleteProductAttributeMappingByIdAsync(productAttributeId);
        //    if (!result.Success)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}
        //#endregion

        //#region ProductAttributeValue
        //[AllowAnonymous]
        //[HttpGet("attributes/{productAttributeId}/value")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult> GetAttributeValues(int productAttributeId)
        //{
        //    var result = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeId);

        //    if (result is null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);
        //}

        //[HttpGet("attributes/value/{valueId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetAttributeValueById(int valueId)
        //{
        //    var result = await _productAttributeService.GetProductAttributeValuesByIdAsync(valueId);

        //    if (result is null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);
        //}

        //[HttpPost("attributes/value")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult> CreateProductAttributeValue(ProductAttributeValue productAttributeValue)
        //{
        //    var result = await _productAttributeService.CreateProductAttributeValueAsync(productAttributeValue);

        //    if (!result.Success)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        //[HttpPut("attributes/value")]
        //public async Task<ActionResult> UpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        //{
        //    var result = await _productAttributeService.UpdateProductAttributeValueAsync(productAttributeValue);
        //    if (!result.Success)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}

        //[HttpDelete("attributes/value/{valueId}")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<ActionResult> DeleteProductAttributeValue(int valueId)
        //{
        //    var result = await _productAttributeService.DeleteProductAttributeValueAsync(valueId);
        //    if (!result.Success)
        //    {
        //        return BadRequest(result);
        //    }
        //    return Ok(result);
        //}
        //#endregion

        //#region ProductCategory
        //[HttpGet("categories/{productCategoryId}")]
        //public async Task<ActionResult<ProductCategory>> GetProductCategoryById(int productCategoryId)
        //{
        //    return await _productCategoryService.GetProductCategoryById(productCategoryId);
        //}

        //[HttpGet("{productId}/categories")]
        //public async Task<ActionResult<List<ProductCategory>>> GetByProductId(int productId)
        //{
        //    return await _productCategoryService.GetProductCategoriesByProductId(productId);
        //}

        //[HttpPost("categories")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<ActionResult> CreateProductCategoryAsync(ProductCategory productCategory)
        //{
        //    var result = await _productCategoryService.CreateProductCategoryAsync(productCategory);
        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        //[HttpPut("categories")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        //public async Task<ActionResult> UpdateProductCategoryAsync(ProductCategory productCategory)
        //{
        //    var result = await _productCategoryService.UpdateProductCategoryAsync(productCategory);
        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        //[HttpDelete("categories/{productCategoryId}")]
        //public async Task<ActionResult> DeleteProductCategoryAsync(int productCategoryId)
        //{
        //    var result = await _productCategoryService.DeleteProductCategoryAsync(productCategoryId);
        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}
        //#endregion
    }
}
