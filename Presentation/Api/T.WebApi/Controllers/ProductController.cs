using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/product")]
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
        [HttpGet(APIRoutes.GetAll)]
        [AllowAnonymous]

        public async Task<ActionResult<List<Product>>> GetAll([FromQuery] ProductParameters productParameters)
        {
            var products = await _productService.GetAll(productParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(products.MetaData));
            return Ok(products);
        }

        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<Product>>> Get(int productId)
        {
            return await _productService.GetByIdAsync(productId);
        }

        [HttpPost()]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateProduct(Product product)
        {
            //if (!await _permissionRecordService.AuthorizeAsync(DefaultPermission.ManageProducts))
            //    return Forbid();

            var result = await _productService.CreateProductAsync(product);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut()]
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
        #endregion

        #region ProductPicture
        [HttpGet("{productId}/pictures")]
        [AllowAnonymous]
        public async Task<ActionResult<ServiceResponse<List<ProductPicture>>>> GetProductPicturesByProductIdAsync(int productId)
        {
            return await _productService.GetProductPicturesByProductIdAsync(productId);
        }

        [HttpPost("{productId}/upload-picture")]
        public async Task<ActionResult> AddProductImage(List<IFormFile> formFiles, int productId)
        {
            var result = await _productService.AddProductImage(formFiles, productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{productId}/picture-id/{pictureId}")]
        public async Task<ActionResult> DeleteProductImage(int productId, int pictureId)
        {
            var result = await _productService.DeleteProductImage(productId, pictureId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{productId}/delete-all-picture")]
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

        #region ProductAttribute
        [AllowAnonymous]
        [HttpGet("{productId}/attributes")]
        public async Task<ActionResult> GetProductAttributes(int productId)
        {
            var result = await _productService.GetAllProductAttributeByProductIdAsync(productId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{productId}/attribute")]
        public async Task<ActionResult<ServiceResponse<List<ProductAttributeMapping>>>> GetProductAttributesMapping(int productId)
        {
            return await _productAttributeService.GetProductAttributesMappingByProductIdAsync(productId);
        }

        [HttpPost("attribute")]
        public async Task<ActionResult> CreateProductAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {

            var result = await _productAttributeService.CreateProductAttributeMappingAsync(productAttributeMapping);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("attribute")]
        public async Task<ActionResult> UpdateAttributeMapping(ProductAttributeMapping productAttributeMapping)
        {
            //try to get a product with the specified id
            var product = (await _productService.GetByIdAsync(productAttributeMapping.ProductId)).Data ??
              throw new ArgumentException("No product found with the specified id");

            var productAttribute = (await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId)).Data ??
                throw new ArgumentException("No product attribute found with the specified id");

            var result = await _productAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("attribute/{productAttributeMappingId}")]
        public async Task<ActionResult<ServiceResponse<ProductAttributeMapping>>> GetProductAttributeMappingByIdAsync(int productAttributeMappingId)
        {
            return await _productAttributeService.GetProductAttributeMappingByIdAsync(productAttributeMappingId);
        }

        [HttpDelete("attribute/{productAttributeId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteProductAttributeMapping(int productAttributeId)
        {
            var result = await _productAttributeService.DeleteProductAttributeMappingByIdAsync(productAttributeId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region ProductAttributeValue
        [AllowAnonymous]
        [HttpGet("attribute/{productAttributeMappingId}/value")]
        public async Task<ActionResult> GetAttributeValues(int productAttributeMappingId)
        {
            var result = await _productAttributeService.GetProductAttributeValuesAsync(productAttributeMappingId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("attribute/value/{productAttributeValueId}")]
        public async Task<IActionResult> GetAttributeValueById(int productAttributeValueId)
        {
            var result = await _productAttributeService.GetProductAttributeValuesByIdAsync(productAttributeValueId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("attribute/value")]
        public async Task<ActionResult> CreateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            var result = await _productAttributeService.CreateProductAttributeValueAsync(productAttributeValue);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("attribute/value")]
        public async Task<ActionResult> UpdateProductAttributeValue(ProductAttributeValue productAttributeValue)
        {
            var result = await _productAttributeService.UpdateProductAttributeValueAsync(productAttributeValue);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("attribute/value/{productAttributeValueId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> DeleteProductAttributeValue(int productAttributeValueId)
        {
            var result = await _productAttributeService.DeleteProductAttributeValueAsync(productAttributeValueId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region ProductCategory
        [HttpGet("category/{productCategoryId}")]
        public async Task<ActionResult<ServiceResponse<ProductCategory>>> GetProductCategoryById(int productCategoryId)
        {
            return await _productCategoryService.GetProductCategoryById(productCategoryId);
        }

        [HttpGet("{productId}/categories")]
        public async Task<ActionResult<ServiceResponse<List<ProductCategory>>>> GetByProductId(int productId)
        {
            return await _productCategoryService.GetProductCategoriesByProductId(productId);
        }

        [HttpPost("category")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> CreateProductCategoryAsync(ProductCategory productCategory)
        {
            var result = await _productCategoryService.CreateProductCategoryAsync(productCategory);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("category")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<ActionResult> UpdateProductCategoryAsync(ProductCategory productCategory)
        {
            var result = await _productCategoryService.UpdateProductCategoryAsync(productCategory);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("category/{productCategoryId}")]
        public async Task<ActionResult> DeleteProductCategoryAsync(int productCategoryId)
        {
            var result = await _productCategoryService.DeleteProductCategoryAsync(productCategoryId);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion
    }
}
