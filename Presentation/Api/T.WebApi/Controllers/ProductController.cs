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
            var products = await _productService.SearchProduct(productParameters);
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

        [HttpGet("show-on-home-page")]
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
    }
}
